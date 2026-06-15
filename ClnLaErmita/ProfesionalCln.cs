using CadLaErmita;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;

namespace ClnLaErmita
{
    public class ModuloCampo
    {
        public string Nombre { get; set; }
        public string Etiqueta { get; set; }
        public bool Obligatorio { get; set; }
        public bool EsPassword { get; set; }

        public ModuloCampo(string nombre, string etiqueta, bool obligatorio = true, bool esPassword = false)
        {
            Nombre = nombre;
            Etiqueta = etiqueta;
            Obligatorio = obligatorio;
            EsPassword = esPassword;
        }
    }

    public class ModuloConfig
    {
        public string Id { get; set; }
        public string Titulo { get; set; }
        public string Tabla { get; set; }
        public string SelectSql { get; set; }
        public string[] ColumnasBusqueda { get; set; }
        public List<ModuloCampo> Campos { get; set; }
        public string ColumnaUnica { get; set; }
        public bool SoloLectura { get; set; }
    }

    /// <summary>
    /// CRUD genérico seguro para módulos nuevos.
    /// Usa SQL parametrizado y lista blanca de tablas/columnas.
    /// </summary>
    public static class ProfesionalCln
    {
        private static readonly Dictionary<string, ModuloConfig> Modulos = new Dictionary<string, ModuloConfig>(StringComparer.OrdinalIgnoreCase)
        {
            ["Roles"] = new ModuloConfig
            {
                Id = "Roles", Titulo = "Roles", Tabla = "Rol", ColumnaUnica = "nombre",
                SelectSql = "SELECT id, nombre, descripcion, estado FROM Rol WHERE estado <> -1",
                ColumnasBusqueda = new [] { "nombre", "descripcion" },
                Campos = new List<ModuloCampo> { new ModuloCampo("nombre", "Nombre"), new ModuloCampo("descripcion", "Descripción", false) }
            },
            ["Usuarios"] = new ModuloConfig
            {
                Id = "Usuarios", Titulo = "Usuarios", Tabla = "Usuario", ColumnaUnica = "usuario",
                SelectSql = "SELECT u.id, u.idRol, r.nombre AS rol, u.nombre, u.usuario, u.email, u.telefono, u.estado FROM Usuario u INNER JOIN Rol r ON r.id = u.idRol WHERE u.estado <> -1",
                ColumnasBusqueda = new [] { "u.nombre", "u.usuario", "u.email", "u.telefono" },
                Campos = new List<ModuloCampo>
                {
                    new ModuloCampo("idRol", "Id Rol"), new ModuloCampo("nombre", "Nombre"),
                    new ModuloCampo("usuario", "Usuario"), new ModuloCampo("contrasena", "Contraseña", false, true),
                    new ModuloCampo("email", "Email", false), new ModuloCampo("telefono", "Teléfono", false)
                }
            },
            ["Clientes"] = new ModuloConfig
            {
                Id = "Clientes", Titulo = "Clientes", Tabla = "Cliente", ColumnaUnica = "documento",
                SelectSql = "SELECT id, nombre, documento, telefono, email, direccion, estado FROM Cliente WHERE estado <> -1",
                ColumnasBusqueda = new [] { "nombre", "documento", "telefono", "email" },
                Campos = new List<ModuloCampo>
                {
                    new ModuloCampo("nombre", "Nombre"), new ModuloCampo("documento", "Documento", false),
                    new ModuloCampo("telefono", "Teléfono", false), new ModuloCampo("email", "Email", false),
                    new ModuloCampo("direccion", "Dirección", false)
                }
            },
            ["Proveedores"] = new ModuloConfig
            {
                Id = "Proveedores", Titulo = "Proveedores", Tabla = "Proveedor", ColumnaUnica = "nombre",
                SelectSql = "SELECT id, nombre, nit, telefono, email, direccion, contacto, estado FROM Proveedor WHERE estado <> -1",
                ColumnasBusqueda = new [] { "nombre", "nit", "telefono", "email", "contacto" },
                Campos = new List<ModuloCampo>
                {
                    new ModuloCampo("nombre", "Nombre"), new ModuloCampo("nit", "NIT", false),
                    new ModuloCampo("telefono", "Teléfono", false), new ModuloCampo("email", "Email", false),
                    new ModuloCampo("direccion", "Dirección", false), new ModuloCampo("contacto", "Contacto", false)
                }
            },
            ["Mesas"] = new ModuloConfig
            {
                Id = "Mesas", Titulo = "Mesas", Tabla = "Mesa", ColumnaUnica = "numero",
                SelectSql = "SELECT id, numero, capacidad, estadoMesa, ubicacion, estado FROM Mesa WHERE estado <> -1",
                ColumnasBusqueda = new [] { "numero", "estadoMesa", "ubicacion" },
                Campos = new List<ModuloCampo>
                {
                    new ModuloCampo("numero", "Número"), new ModuloCampo("capacidad", "Capacidad"),
                    new ModuloCampo("estadoMesa", "Estado Mesa (Libre/Ocupada/Reservada)"),
                    new ModuloCampo("ubicacion", "Ubicación", false)
                }
            },
            ["MetodosPago"] = new ModuloConfig
            {
                Id = "MetodosPago", Titulo = "Métodos de Pago", Tabla = "MetodoPago", ColumnaUnica = "nombre",
                SelectSql = "SELECT id, nombre, descripcion, estado FROM MetodoPago WHERE estado <> -1",
                ColumnasBusqueda = new [] { "nombre", "descripcion" },
                Campos = new List<ModuloCampo> { new ModuloCampo("nombre", "Nombre"), new ModuloCampo("descripcion", "Descripción", false) }
            },
            ["Reservas"] = new ModuloConfig
            {
                Id = "Reservas", Titulo = "Reservas", Tabla = "Reserva", ColumnaUnica = null,
                SelectSql = "SELECT r.id, r.idCliente, c.nombre AS cliente, r.idMesa, m.numero AS mesa, r.fechaReserva, r.horaReserva, r.cantidadPersonas, r.estadoReserva, r.observacion, r.estado FROM Reserva r INNER JOIN Cliente c ON c.id = r.idCliente INNER JOIN Mesa m ON m.id = r.idMesa WHERE r.estado <> -1",
                ColumnasBusqueda = new [] { "c.nombre", "m.numero", "r.estadoReserva", "r.observacion" },
                Campos = new List<ModuloCampo>
                {
                    new ModuloCampo("idCliente", "Id Cliente"), new ModuloCampo("idMesa", "Id Mesa"),
                    new ModuloCampo("fechaReserva", "Fecha (AAAA-MM-DD)"), new ModuloCampo("horaReserva", "Hora (HH:MM)"),
                    new ModuloCampo("cantidadPersonas", "Cantidad Personas"),
                    new ModuloCampo("estadoReserva", "Estado (Registrada/Confirmada/Cancelada/Atendida)"),
                    new ModuloCampo("observacion", "Observación", false)
                }
            },
            ["Compras"] = new ModuloConfig
            {
                Id = "Compras", Titulo = "Compras", Tabla = "Compra", ColumnaUnica = "numeroDocumento",
                SelectSql = "SELECT co.id, co.idProveedor, p.nombre AS proveedor, co.idUsuario, u.nombre AS usuario, co.fecha, co.numeroDocumento, co.subtotal, co.impuesto, co.total, co.observacion, co.estado FROM Compra co INNER JOIN Proveedor p ON p.id = co.idProveedor INNER JOIN Usuario u ON u.id = co.idUsuario WHERE co.estado <> -1",
                ColumnasBusqueda = new [] { "p.nombre", "u.nombre", "co.numeroDocumento", "co.observacion" },
                Campos = new List<ModuloCampo>
                {
                    new ModuloCampo("idProveedor", "Id Proveedor"), new ModuloCampo("idUsuario", "Id Usuario"),
                    new ModuloCampo("numeroDocumento", "Documento", false), new ModuloCampo("subtotal", "Subtotal"),
                    new ModuloCampo("impuesto", "Impuesto"), new ModuloCampo("total", "Total"),
                    new ModuloCampo("observacion", "Observación", false)
                }
            },
            ["Inventario"] = new ModuloConfig
            {
                Id = "Inventario", Titulo = "Inventario", Tabla = "vwInventarioActual", SoloLectura = true,
                SelectSql = "SELECT id, codigo, producto, categoria, stockActual, stockMinimo, alerta, fechaActualizacion FROM vwInventarioActual WHERE 1=1",
                ColumnasBusqueda = new [] { "codigo", "producto", "categoria", "alerta" },
                Campos = new List<ModuloCampo>()
            }
        };

        public static List<ModuloConfig> ObtenerModulos()
        {
            return Modulos.Values.OrderBy(x => x.Titulo).ToList();
        }

        public static ModuloConfig ObtenerModulo(string modulo)
        {
            if (!Modulos.ContainsKey(modulo))
                throw new Exception("Módulo no permitido: " + modulo);
            return Modulos[modulo];
        }

        public static DataTable Listar(string modulo, string parametro)
        {
            var cfg = ObtenerModulo(modulo);
            string sql = cfg.SelectSql;
            bool tieneBusqueda = !string.IsNullOrWhiteSpace(parametro) && cfg.ColumnasBusqueda != null && cfg.ColumnasBusqueda.Length > 0;
            if (tieneBusqueda)
            {
                string filtro = string.Join(" OR ", cfg.ColumnasBusqueda.Select(c => c + " LIKE @p"));
                sql += " AND (" + filtro + ")";
            }
            sql += " ORDER BY 1 DESC";

            using (var cn = CrearConexion())
            using (var da = new SqlDataAdapter(sql, cn))
            {
                if (tieneBusqueda)
                    da.SelectCommand.Parameters.AddWithValue("@p", "%" + parametro.Trim().Replace(" ", "%") + "%");
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public static int Guardar(string modulo, int id, Dictionary<string, string> valores)
        {
            var cfg = ObtenerModulo(modulo);
            if (cfg.SoloLectura) throw new Exception("Este módulo es solo de consulta.");

            ValidarCampos(cfg, id, valores);

            var campos = cfg.Campos
                .Where(c => valores.ContainsKey(c.Nombre))
                .Where(c => !(id > 0 && c.EsPassword && string.IsNullOrWhiteSpace(valores[c.Nombre])))
                .ToList();

            using (var cn = CrearConexion())
            {
                cn.Open();
                using (var cmd = cn.CreateCommand())
                {
                    if (id == 0)
                    {
                        string columnas = string.Join(", ", campos.Select(c => c.Nombre));
                        string parametros = string.Join(", ", campos.Select(c => "@" + c.Nombre));
                        cmd.CommandText = $"INSERT INTO {cfg.Tabla} ({columnas}) VALUES ({parametros}); SELECT SCOPE_IDENTITY();";
                    }
                    else
                    {
                        string sets = string.Join(", ", campos.Select(c => c.Nombre + " = @" + c.Nombre));
                        cmd.CommandText = $"UPDATE {cfg.Tabla} SET {sets} WHERE id = @id";
                        cmd.Parameters.AddWithValue("@id", id);
                    }

                    foreach (var campo in campos)
                    {
                        object valor = PrepararValor(campo, valores[campo.Nombre]);
                        cmd.Parameters.AddWithValue("@" + campo.Nombre, valor ?? DBNull.Value);
                    }

                    object result = cmd.ExecuteScalar();
                    return id == 0 ? Convert.ToInt32(result) : id;
                }
            }
        }

        public static void CambiarEstadoLogico(string modulo, int id, short nuevoEstado)
        {
            var cfg = ObtenerModulo(modulo);
            if (cfg.SoloLectura) throw new Exception("Este módulo es solo de consulta.");

            using (var cn = CrearConexion())
            using (var cmd = cn.CreateCommand())
            {
                cn.Open();
                cmd.CommandText = $"UPDATE {cfg.Tabla} SET estado = @estado WHERE id = @id";
                cmd.Parameters.AddWithValue("@estado", nuevoEstado);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        private static void ValidarCampos(ModuloConfig cfg, int id, Dictionary<string, string> valores)
        {
            foreach (var campo in cfg.Campos.Where(x => x.Obligatorio))
            {
                if (id > 0 && campo.EsPassword) continue;
                if (!valores.ContainsKey(campo.Nombre) || string.IsNullOrWhiteSpace(valores[campo.Nombre]))
                    throw new Exception($"El campo {campo.Etiqueta} es obligatorio.");
            }

            if (!string.IsNullOrWhiteSpace(cfg.ColumnaUnica) && valores.ContainsKey(cfg.ColumnaUnica))
            {
                string valorUnico = valores[cfg.ColumnaUnica];
                if (!string.IsNullOrWhiteSpace(valorUnico) && ExisteDuplicado(cfg, cfg.ColumnaUnica, valorUnico, id))
                    throw new Exception($"Ya existe un registro con el mismo valor en {cfg.ColumnaUnica}.");
            }
        }

        private static bool ExisteDuplicado(ModuloConfig cfg, string columna, string valor, int idExcluir)
        {
            using (var cn = CrearConexion())
            using (var cmd = cn.CreateCommand())
            {
                cn.Open();
                cmd.CommandText = $"SELECT COUNT(1) FROM {cfg.Tabla} WHERE {columna} = @valor AND estado = 1 AND id <> @id";
                cmd.Parameters.AddWithValue("@valor", valor.Trim());
                cmd.Parameters.AddWithValue("@id", idExcluir);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private static object PrepararValor(ModuloCampo campo, string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return DBNull.Value;
            string limpio = valor.Trim();
            if (campo.EsPassword)
            {
                return limpio.StartsWith("PBKDF2$", StringComparison.OrdinalIgnoreCase)
                    ? limpio
                    : PasswordHasher.Hash(limpio);
            }
            return limpio;
        }

        private static SqlConnection CrearConexion()
        {
            string cs = ConfigurationManager.ConnectionStrings["LabLaErmitaEntities"].ConnectionString;
            var builder = new EntityConnectionStringBuilder(cs);
            return new SqlConnection(builder.ProviderConnectionString);
        }
    }
}
