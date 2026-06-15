// ============================================================
// Canal.cs  –  Capa de Acceso a Datos (CAD)
// CORRECCIÓN: Se eliminó el patrón Singleton sobre DbContext.
// DbContext NO debe ser estático ni compartido entre operaciones,
// porque acumula entidades en memoria y provoca errores de
// concurrencia. Cada clase CLN crea su propio "using (var db...)"
// ============================================================
namespace CadLaErmita
{
    /// <summary>
    /// Fábrica de contexto EF.
    /// Uso correcto: using (var db = Canal.NuevoContexto()) { ... }
    /// </summary>
    public static class Canal
    {
        public static LabLaErmitaEntities NuevoContexto()
        {
            return new LabLaErmitaEntities();
        }
    }
}
