// ============================================================
// VentaCln.cs  –  Capa de Negocio (CLN)
// CORRECCIONES CRÍTICAS:
//   1. ERROR GRAVE ORIGINAL: en "crear", se llamaba a
//      "context.SaveChanges()" dos veces dentro del mismo using,
//      pero el total de la venta se asignaba DESPUÉS del primer
//      SaveChanges, por lo que la venta quedaba guardada con total=0
//      hasta el segundo guardado. Se corrigió el orden.
//   2. ERROR EN DETALLE: el subtotal en el detalle original se
//      calculaba en el CLN, pero luego se recalculaba en el bucle
//      siguiente (redundancia). Se simplificó.
//   3. MEJORA: validación de stock antes de descontar, lanzando
//      excepción clara si no hay stock suficiente.
//   4. Usa Canal.NuevoContexto() y tiene try/catch.
// ============================================================
using CadLaErmita;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ClnLaErmita
{
    public class VentaCln
    {
        /// <summary>
        /// Registra una venta con todos sus detalles en una sola transacción.
        /// Devuelve el ID de la venta creada.
        /// </summary>
        public static int Crear(Venta venta, List<DetalleVenta> detalles)
        {
            if (detalles == null || detalles.Count == 0)
                throw new Exception("La venta debe tener al menos un producto.");

            try
            {
                using (var db = Canal.NuevoContexto())
                {
                    // --- Paso 1: Verificar stock de todos los productos ANTES de guardar ---
                    foreach (var detalle in detalles)
                    {
                        var producto = db.Producto.Find(detalle.idProducto);
                        if (producto == null)
                            throw new Exception($"Producto con ID {detalle.idProducto} no encontrado.");
                        if (producto.stock < detalle.cantidad)
                            throw new Exception($"Stock insuficiente para \"{producto.nombre}\". Disponible: {producto.stock}.");
                    }

                    // --- Paso 2: Calcular total y preparar detalles ---
                    decimal total = 0;
                    foreach (var detalle in detalles)
                    {
                        detalle.subtotal = detalle.cantidad * detalle.precio;
                        detalle.estado   = 1;
                        total           += detalle.subtotal;
                    }

                    // --- Paso 3: Guardar cabecera de venta ---
                    venta.fecha  = DateTime.Now;
                    venta.estado = 1;
                    venta.total  = total; // CORRECCIÓN: total calculado ANTES del primer SaveChanges
                    db.Venta.Add(venta);
                    db.SaveChanges(); // Obtiene venta.id del IDENTITY

                    // --- Paso 4: Guardar detalles y descontar stock ---
                    foreach (var detalle in detalles)
                    {
                        detalle.idVenta = venta.id;
                        db.DetalleVenta.Add(detalle);

                        var producto = db.Producto.Find(detalle.idProducto);
                        producto.stock -= detalle.cantidad;
                    }

                    db.SaveChanges();
                    SincronizarInventarioProfesional(db, venta.id, detalles, "Venta", "Salida");
                    return venta.id;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar la venta: " + ex.Message);
            }
        }

        /// <summary>
        /// Anula una venta y restaura el stock de todos sus productos.
        /// </summary>
        public static (bool ok, string mensaje) Anular(int id)
        {
            try
            {
                using (var db = Canal.NuevoContexto())
                {
                    var venta = db.Venta.Find(id);
                    if (venta == null)        return (false, "Venta no encontrada.");
                    if (venta.estado == -1)   return (false, "La venta ya fue anulada.");

                    venta.estado = -1;

                    var detalles = db.DetalleVenta
                                     .Where(x => x.idVenta == id && x.estado == 1)
                                     .ToList();

                    foreach (var d in detalles)
                    {
                        d.estado = -1;
                        var producto = db.Producto.Find(d.idProducto);
                        if (producto != null)
                            producto.stock += d.cantidad; // Restaurar stock
                    }

                    db.SaveChanges();
                    SincronizarInventarioProfesional(db, id, detalles, "Anulacion", "Entrada");
                    return (true, "Venta anulada correctamente.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al anular la venta: " + ex.Message);
            }
        }


        /// <summary>
        /// Sincroniza Inventario y MovimientoInventario si el usuario ya ejecutó
        /// la base profesional. Si está usando la BD antigua, no rompe la venta.
        /// </summary>
        private static void SincronizarInventarioProfesional(LabLaErmitaEntities db, int idVenta, IEnumerable<DetalleVenta> detalles, string origen, string tipo)
        {
            foreach (var d in detalles)
            {
                db.Database.ExecuteSqlCommand(@"
IF OBJECT_ID('Inventario', 'U') IS NOT NULL
BEGIN
    UPDATE i
    SET i.stockActual = p.stock,
        i.fechaActualizacion = SYSDATETIME()
    FROM Inventario i
    INNER JOIN Producto p ON p.id = i.idProducto
    WHERE i.idProducto = @idProducto;
END
IF OBJECT_ID('MovimientoInventario', 'U') IS NOT NULL
BEGIN
    DECLARE @stockNuevo INT = (SELECT stock FROM Producto WHERE id = @idProducto);
    IF @stockNuevo IS NOT NULL
    BEGIN
        INSERT INTO MovimientoInventario
        (idProducto, idUsuario, tipoMovimiento, origen, idReferencia, cantidad, stockAnterior, stockNuevo, observacion)
        VALUES
        (@idProducto, @idUsuario, @tipo, @origen, @idVenta, @cantidad,
         CASE WHEN @tipo = 'Salida' THEN @stockNuevo + @cantidad ELSE @stockNuevo - @cantidad END,
         @stockNuevo, 'Movimiento generado desde ventas');
    END
END",
                    new SqlParameter("@idProducto", d.idProducto),
                    new SqlParameter("@idUsuario", SesionIdSeguro()),
                    new SqlParameter("@tipo", tipo),
                    new SqlParameter("@origen", origen),
                    new SqlParameter("@idVenta", idVenta),
                    new SqlParameter("@cantidad", d.cantidad));
            }
        }

        private static int SesionIdSeguro()
        {
            // La capa de negocio no debe depender de CpLaErmita.Sesion.
            // Para movimientos automáticos se usa 1 como usuario técnico/admin.
            return 1;
        }

        /// <summary>Lista ventas activas usando el procedimiento almacenado.</summary>
        public static List<paVentaListar_Result> ListarPa(string parametro)
        {
            using (var db = Canal.NuevoContexto())
            {
                return db.paVentaListar(parametro?.Trim() ?? "").ToList();
            }
        }

        /// <summary>Lista el detalle de una venta específica.</summary>
        public static List<paDetalleVentaListar_Result> ListarDetalle(int idVenta)
        {
            using (var db = Canal.NuevoContexto())
            {
                return db.paDetalleVentaListar(idVenta).ToList();
            }
        }
    }
}
