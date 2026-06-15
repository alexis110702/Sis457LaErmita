// ============================================================
// RolCln.cs  –  Capa de Negocio (CLN)
// CORRECCIÓN: Usa Canal.NuevoContexto().
// ============================================================
using CadLaErmita;
using System.Collections.Generic;
using System.Linq;

namespace ClnLaErmita
{
    public class RolCln
    {
        /// <summary>Lista todos los roles activos ordenados por nombre.</summary>
        public static List<Rol> Listar()
        {
            using (var db = Canal.NuevoContexto())
            {
                return db.Rol
                         .Where(x => x.estado == 1)
                         .OrderBy(x => x.nombre)
                         .ToList();
            }
        }
    }
}
