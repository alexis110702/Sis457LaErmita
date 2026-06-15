using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;

namespace ClnLaErmita
{
    public static class PermisosCln
    {
        public static DataTable ListarRoles()
        {
            return EjecutarTabla("SELECT id, nombre FROM Rol WHERE estado = 1 ORDER BY nombre");
        }

        public static DataTable ListarPermisos()
        {
            return EjecutarTabla("SELECT id, modulo + ' - ' + accion AS permiso, descripcion FROM Permiso WHERE estado = 1 ORDER BY modulo, accion");
        }

        public static HashSet<int> ObtenerPermisosRol(int idRol)
        {
            var ids = new HashSet<int>();
            using (var cn = CrearConexion())
            using (var cmd = cn.CreateCommand())
            {
                cn.Open();
                cmd.CommandText = "SELECT idPermiso FROM RolPermiso WHERE idRol = @idRol";
                cmd.Parameters.AddWithValue("@idRol", idRol);
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read()) ids.Add(dr.GetInt32(0));
                }
            }
            return ids;
        }

        public static void GuardarPermisosRol(int idRol, IEnumerable<int> permisos)
        {
            using (var cn = CrearConexion())
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.Transaction = tx;
                        cmd.CommandText = "DELETE FROM RolPermiso WHERE idRol = @idRol";
                        cmd.Parameters.AddWithValue("@idRol", idRol);
                        cmd.ExecuteNonQuery();
                    }

                    foreach (int idPermiso in permisos)
                    {
                        using (var cmd = cn.CreateCommand())
                        {
                            cmd.Transaction = tx;
                            cmd.CommandText = "INSERT INTO RolPermiso (idRol, idPermiso) VALUES (@idRol, @idPermiso)";
                            cmd.Parameters.AddWithValue("@idRol", idRol);
                            cmd.Parameters.AddWithValue("@idPermiso", idPermiso);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    tx.Commit();
                }
            }
        }

        private static DataTable EjecutarTabla(string sql)
        {
            using (var cn = CrearConexion())
            using (var da = new SqlDataAdapter(sql, cn))
            {
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        private static SqlConnection CrearConexion()
        {
            string cs = ConfigurationManager.ConnectionStrings["LabLaErmitaEntities"].ConnectionString;
            var builder = new EntityConnectionStringBuilder(cs);
            return new SqlConnection(builder.ProviderConnectionString);
        }
    }
}
