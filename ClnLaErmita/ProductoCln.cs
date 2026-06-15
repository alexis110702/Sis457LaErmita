// ============================================================
// ProductoCln.cs  –  Capa de Negocio (CLN)
// CORRECCIONES:
//   1. Usa Canal.NuevoContexto() en lugar de "new LabLaErmitaEntities()".
//   2. Se agrega try/catch en cada método.
//   3. Método "Eliminar" verifica si el producto tiene detalles de
//      venta activos antes de proceder (integridad de datos).
//   4. Validación de stock negativo en Modificar.
// ============================================================
using CadLaErmita;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClnLaErmita
{
    public class ProductoCln
    {
        /// <summary>Crea un nuevo producto y devuelve su ID.</summary>
        public static int Crear(Producto producto)
        {
            try
            {
                using (var db = Canal.NuevoContexto())
                {
                    producto.estado = 1;
                    db.Producto.Add(producto);
                    db.SaveChanges();
                    return producto.id;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el producto: " + ex.Message);
            }
        }

        /// <summary>Modifica los datos de un producto existente.</summary>
        public static int Modificar(Producto producto)
        {
            try
            {
                // MEJORA: validar stock no negativo antes de tocar la BD
                if (producto.stock < 0)
                    throw new Exception("El stock no puede ser negativo.");

                using (var db = Canal.NuevoContexto())
                {
                    var existente = db.Producto.Find(producto.id);
                    if (existente == null) return 0;

                    existente.idCategoria = producto.idCategoria;
                    existente.nombre      = producto.nombre;
                    existente.descripcion = producto.descripcion;
                    existente.precio      = producto.precio;
                    existente.stock       = producto.stock;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al modificar el producto: " + ex.Message);
            }
        }

        /// <summary>
        /// Eliminación lógica.
        /// MEJORA: verifica que el producto no tenga ventas activas.
        /// </summary>
        public static (bool ok, string mensaje) Eliminar(int id)
        {
            try
            {
                using (var db = Canal.NuevoContexto())
                {
                    bool tieneVentas = db.DetalleVenta.Any(dv => dv.idProducto == id && dv.estado == 1);
                    if (tieneVentas)
                        return (false, "No se puede eliminar: el producto está asociado a ventas activas.");

                    var existente = db.Producto.Find(id);
                    if (existente == null) return (false, "Producto no encontrado.");

                    existente.estado = -1;
                    db.SaveChanges();
                    return (true, "Producto eliminado correctamente.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el producto: " + ex.Message);
            }
        }

        /// <summary>Obtiene un producto por su ID.</summary>
        public static Producto ObtenerUno(int id)
        {
            using (var db = Canal.NuevoContexto())
            {
                return db.Producto.Find(id);
            }
        }

        /// <summary>Lista productos usando el procedimiento almacenado.</summary>
        public static List<paProductoListar_Result> ListarPa(string parametro)
        {
            using (var db = Canal.NuevoContexto())
            {
                return db.paProductoListar(parametro?.Trim() ?? "").ToList();
            }
        }

        /// <summary>Lista todos los productos activos ordenados por nombre.</summary>
        public static List<Producto> Listar()
        {
            using (var db = Canal.NuevoContexto())
            {
                return db.Producto
                         .Where(x => x.estado == 1)
                         .OrderBy(x => x.nombre)
                         .ToList();
            }
        }

        /// <summary>Verifica si ya existe un producto activo con ese nombre (ignora mayúsculas).</summary>
        public static bool ExisteNombre(string nombre, int idExcluir = 0)
        {
            using (var db = Canal.NuevoContexto())
            {
                string limpio = (nombre ?? "").Trim().ToLower();
                return db.Producto.Any(p =>
                    p.estado == 1 &&
                    p.id != idExcluir &&
                    p.nombre.ToLower() == limpio);
            }
        }

    }
}
