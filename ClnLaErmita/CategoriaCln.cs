// ============================================================
// CategoriaCln.cs  –  Capa de Negocio (CLN)
// CORRECCIONES:
//   1. Se reemplazó "new LabLaErmitaEntities()" por Canal.NuevoContexto()
//      para centralizar la creación del contexto.
//   2. Se agregó manejo de excepciones (try/catch) en cada método.
//   3. Método "eliminar" ahora verifica si la categoría tiene
//      productos activos antes de borrarla (integridad referencial).
// ============================================================
using CadLaErmita;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClnLaErmita
{
    public class CategoriaCln
    {
        /// <summary>Crea una nueva categoría y devuelve su ID generado.</summary>
        public static int Crear(Categoria categoria)
        {
            try
            {
                using (var db = Canal.NuevoContexto())
                {
                    categoria.estado = 1;
                    db.Categoria.Add(categoria);
                    db.SaveChanges();
                    return categoria.id;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear la categoría: " + ex.Message);
            }
        }

        /// <summary>Actualiza nombre y descripción de una categoría existente.</summary>
        public static int Modificar(Categoria categoria)
        {
            try
            {
                using (var db = Canal.NuevoContexto())
                {
                    var existente = db.Categoria.Find(categoria.id);
                    if (existente == null) return 0;

                    existente.nombre      = categoria.nombre;
                    existente.descripcion = categoria.descripcion;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al modificar la categoría: " + ex.Message);
            }
        }

        /// <summary>
        /// Eliminación lógica (estado = -1).
        /// MEJORA: Verifica que no existan productos activos asociados.
        /// </summary>
        public static (bool ok, string mensaje) Eliminar(int id)
        {
            try
            {
                using (var db = Canal.NuevoContexto())
                {
                    // Verificar productos activos en esta categoría
                    bool tieneProductos = db.Producto.Any(p => p.idCategoria == id && p.estado == 1);
                    if (tieneProductos)
                        return (false, "No se puede eliminar: la categoría tiene productos activos asociados.");

                    var existente = db.Categoria.Find(id);
                    if (existente == null) return (false, "Categoría no encontrada.");

                    existente.estado = -1;
                    db.SaveChanges();
                    return (true, "Categoría eliminada correctamente.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar la categoría: " + ex.Message);
            }
        }

        /// <summary>Obtiene una categoría por su ID.</summary>
        public static Categoria ObtenerUno(int id)
        {
            using (var db = Canal.NuevoContexto())
            {
                return db.Categoria.Find(id);
            }
        }

        /// <summary>Lista categorías activas usando el procedimiento almacenado.</summary>
        public static List<paCategoriaListar_Result> ListarPa(string parametro)
        {
            using (var db = Canal.NuevoContexto())
            {
                return db.paCategoriaListar(parametro?.Trim() ?? "").ToList();
            }
        }

        /// <summary>Lista todas las categorías activas ordenadas por nombre.</summary>
        public static List<Categoria> Listar()
        {
            using (var db = Canal.NuevoContexto())
            {
                return db.Categoria
                         .Where(x => x.estado == 1)
                         .OrderBy(x => x.nombre)
                         .ToList();
            }
        }
        /// <summary>Verifica si ya existe una categoría activa con ese nombre (ignora mayúsculas).</summary>
        public static bool ExisteNombre(string nombre, int idExcluir = 0)
        {
            using (var db = Canal.NuevoContexto())
            {
                return db.Categoria.Any(c =>
                    c.estado == 1 &&
                    c.id != idExcluir &&
                    c.nombre.ToLower() == nombre.ToLower().Trim());
            }
        }
    }
}
