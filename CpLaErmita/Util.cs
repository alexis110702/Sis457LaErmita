// ============================================================
// Util.cs  –  Utilidades de presentación
// CORRECCIONES:
//   1. Métodos renombrados a PascalCase (OnlyNumbers, OnlyDecimals)
//      para cumplir convenciones C#.
//   2. Método Encrypt conservado pero con comentario sobre su uso.
// ============================================================
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace CpLaErmita
{
    internal static class Util
    {
        private const string EncryptionKey = "LaErmita!SIS457";

        /// <summary>
        /// Cifra un texto con AES-256.
        /// NOTA: En producción, usar BCrypt o Argon2 para contraseñas.
        /// </summary>
        public static string Encrypt(string clearText)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(EncryptionKey,
                    new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64,
                                 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV  = pdb.GetBytes(16);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(),
                                                     CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        /// <summary>Permite sólo dígitos enteros en un TextBox.</summary>
        public static void OnlyNumbers(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar));
        }

        /// <summary>Permite dígitos y un único punto decimal en un TextBox.</summary>
        public static void OnlyDecimals(object sender, KeyPressEventArgs e)
        {
            var txt = (TextBox)sender;
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
                e.Handled = false;
            else if (e.KeyChar == '.' && !txt.Text.Contains("."))
                e.Handled = false;
            else
                e.Handled = true;
        }
    }
}
