// ============================================================
// UsuarioCln.cs  –  Capa de Negocio (CLN)
// MEJORAS PROFESIONALES:
//   1. Login compatible con contraseñas antiguas y PBKDF2.
//   2. Hash seguro para nuevas contraseñas.
//   3. Validaciones básicas y control de usuarios activos.
// ============================================================
using CadLaErmita;
using System;
using System.Data.Entity;
using System.Linq;

namespace ClnLaErmita
{
    public class UsuarioCln
    {
        /// <summary>
        /// Valida credenciales de acceso.
        /// Primero intenta el procedimiento original para no romper BD antigua.
        /// Si no encuentra, valida PBKDF2 directamente contra la tabla Usuario.
        /// </summary>
        public static paUsuarioLogin_Result Login(string usuario, string contrasena)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contrasena))
                    return null;

                string usuarioTrim = usuario.Trim();
                string claveTrim = contrasena.Trim();

                using (var db = Canal.NuevoContexto())
                {
                    // Compatibilidad: login original por procedimiento almacenado.
                    var legacy = db.paUsuarioLogin(usuarioTrim, claveTrim).FirstOrDefault();
                    if (legacy != null) return legacy;

                    // Login profesional: usuario activo + hash PBKDF2.
                    var u = db.Usuario
                              .Include(x => x.Rol)
                              .FirstOrDefault(x => x.usuario1 == usuarioTrim && x.estado == 1);

                    if (u == null || u.Rol == null || u.Rol.estado != 1)
                        return null;

                    if (!PasswordHasher.Verify(claveTrim, u.contrasena))
                        return null;

                    return new paUsuarioLogin_Result
                    {
                        id = u.id,
                        idRol = u.idRol,
                        nombre = u.nombre,
                        usuario = u.usuario1,
                        rol = u.Rol.nombre
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al iniciar sesión: " + ex.Message);
            }
        }

        /// <summary>Genera hash PBKDF2 para almacenar una nueva contraseña.</summary>
        public static string GenerarHashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new Exception("La contraseña no puede estar vacía.");
            return PasswordHasher.Hash(password.Trim());
        }
    }
}
