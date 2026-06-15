using System;
using System.Security.Cryptography;

namespace ClnLaErmita
{
    /// <summary>
    /// Hash seguro para contraseñas usando PBKDF2.
    /// Formato almacenado: PBKDF2$iteraciones$saltBase64$hashBase64
    /// </summary>
    public static class PasswordHasher
    {
        private const int Iteraciones = 100000;
        private const int SaltBytes = 16;
        private const int HashBytes = 32;

        public static string Hash(string password)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));

            byte[] salt = new byte[SaltBytes];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hash = Derivar(password, salt, Iteraciones, HashBytes);
            return $"PBKDF2${Iteraciones}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        public static bool Verify(string password, string storedHash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHash))
                return false;

            // Compatibilidad con base antigua: si no tiene formato PBKDF2,
            // se valida como texto exacto. Se recomienda migrar con Hash().
            if (!storedHash.StartsWith("PBKDF2$", StringComparison.OrdinalIgnoreCase))
                return password == storedHash;

            string[] parts = storedHash.Split('$');
            if (parts.Length != 4) return false;

            int iteraciones;
            if (!int.TryParse(parts[1], out iteraciones)) return false;

            byte[] salt = Convert.FromBase64String(parts[2]);
            byte[] expected = Convert.FromBase64String(parts[3]);
            byte[] actual = Derivar(password, salt, iteraciones, expected.Length);

            return ComparacionTiempoConstante(actual, expected);
        }

        private static byte[] Derivar(string password, byte[] salt, int iteraciones, int length)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iteraciones, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(length);
            }
        }

        private static bool ComparacionTiempoConstante(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++) diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}
