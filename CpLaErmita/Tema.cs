// ============================================================
// Tema.cs – Estilo visual profesional para La Ermita
// Paleta: café oscuro, crema, dorado y verdes de confirmación.
// Aplica colores, bordes, tablas responsivas e iconos de texto
// a los botones sin romper el diseñador WinForms.
// ============================================================
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CpLaErmita
{
    internal static class Tema
    {
        public static readonly Color CafeOscuro = Color.FromArgb(78, 48, 27);
        public static readonly Color CafeMedio = Color.FromArgb(135, 86, 48);
        public static readonly Color Crema = Color.FromArgb(248, 241, 229);
        public static readonly Color CremaPanel = Color.FromArgb(255, 252, 246);
        public static readonly Color Dorado = Color.FromArgb(196, 145, 76);
        public static readonly Color Verde = Color.FromArgb(46, 125, 50);
        public static readonly Color Azul = Color.FromArgb(46, 97, 140);
        public static readonly Color Naranja = Color.FromArgb(190, 102, 38);
        public static readonly Color Rojo = Color.FromArgb(166, 54, 54);
        public static readonly Color Morado = Color.FromArgb(101, 70, 130);
        public static readonly Color Gris = Color.FromArgb(97, 97, 97);

        public static void Aplicar(Form frm)
        {
            if (frm == null) return;

            frm.BackColor = Crema;
            frm.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.FormBorderStyle = FormBorderStyle.Sizable;
            frm.AutoScroll = true;

            AgrandarPantalla(frm);

            foreach (Control control in frm.Controls)
                AplicarControl(control);
        }

        private static void AgrandarPantalla(Form frm)
        {
            // Tamaños mínimos profesionales para que los listados no se corten
            // y las tablas tengan más espacio al presentar el proyecto.
            int minW = 1020;
            int minH = 650;

            if (frm.Name == "FrmLogin") { minW = 520; minH = 360; }
            else if (frm.Name == "FrmConfirmarPassword") { minW = 560; minH = 360; }
            else if (frm.Name == "FrmCambiarUsuario") { minW = 620; minH = 465; }
            else if (frm.Name == "FrmPrincipal") { minW = 1160; minH = 760; }
            else if (frm.Name == "FrmCategoria") { minW = 840; minH = 560; }
            else if (frm.Name == "FrmProducto") { minW = 1080; minH = 650; }
            else if (frm.Name == "FrmVenta") { minW = 1120; minH = 720; }

            frm.MinimumSize = new Size(Math.Max(frm.MinimumSize.Width, minW), Math.Max(frm.MinimumSize.Height, minH));
            frm.Size = new Size(Math.Max(frm.Width, minW), Math.Max(frm.Height, minH));
        }

        private static void AplicarControl(Control control)
        {
            if (control == null) return;

            // Letra un poco más grande en todos los formularios.
            if (!(control is DataGridView))
            {
                FontStyle estilo = control.Font.Style;
                float tam = Math.Max(control.Font.Size + 1.2F, 10.5F);
                if (control is Button) tam = Math.Max(tam, 11.5F);
                if (control is Label && control.Name.ToLower().Contains("titulo")) tam = Math.Max(tam, 17F);
                control.Font = new Font("Segoe UI", Math.Min(tam, 18F), estilo);
            }

            if (control is Panel || control is GroupBox || control is TabPage)
            {
                control.BackColor = CremaPanel;
            }

            if (control is Label lbl)
            {
                lbl.ForeColor = CafeOscuro;
                if (lbl.Name.ToLower().Contains("titulo"))
                {
                    lbl.Font = new Font("Segoe UI", Math.Max(lbl.Font.Size, 14F), FontStyle.Bold);
                    lbl.ForeColor = CafeOscuro;
                }
            }
            else if (control is Button btn)
            {
                AplicarBoton(btn);
            }
            else if (control is TextBox txt)
            {
                txt.BorderStyle = BorderStyle.FixedSingle;
                txt.BackColor = Color.White;
                txt.ForeColor = Color.FromArgb(40, 40, 40);
            }
            else if (control is ComboBox cmb)
            {
                cmb.FlatStyle = FlatStyle.Flat;
                cmb.BackColor = Color.White;
                cmb.ForeColor = Color.FromArgb(40, 40, 40);
            }
            else if (control is DataGridView dgv)
            {
                AplicarGrilla(dgv);
            }
            else if (control is TabControl tab)
            {
                tab.BackColor = Crema;
            }

            foreach (Control hijo in control.Controls)
                AplicarControl(hijo);
        }

        private static void AplicarBoton(Button btn)
        {
            string textoOriginal = (btn.Text ?? string.Empty).Trim();
            string textoBase = QuitarIcono(textoOriginal).Trim();
            string clave = textoBase.ToLowerInvariant();

            btn.Text = IconoPara(clave, textoOriginal) + textoBase;
            btn.TextAlign = ContentAlignment.MiddleCenter;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.ForeColor = Color.White;
            btn.Cursor = Cursors.Hand;
            btn.Font = new Font("Segoe UI", 11.5F, FontStyle.Bold);
            btn.Height = Math.Max(btn.Height, 40);
            btn.Width = Math.Max(btn.Width, 130);
            btn.UseVisualStyleBackColor = false;

            if (clave.Contains("guardar") || clave.Contains("registrar") || clave.Contains("agregar") || clave.Contains("activar") || clave.Contains("ingresar"))
                btn.BackColor = Verde;
            else if (clave.Contains("buscar") || clave.Contains("consultar") || clave.Contains("detalle"))
                btn.BackColor = Azul;
            else if (clave.Contains("nuevo") || clave.Contains("editar") || clave.Contains("actualizar"))
                btn.BackColor = Naranja;
            else if (clave.Contains("eliminar") || clave.Contains("desactivar") || clave.Contains("anular") || clave.Contains("quitar"))
                btn.BackColor = Rojo;
            else if (clave.Contains("permiso") || clave.Contains("config"))
                btn.BackColor = Morado;
            else if (clave.Contains("cancelar") || clave.Contains("cerrar") || clave.Contains("salir") || clave.Contains("limpiar"))
                btn.BackColor = Gris;
            else
                btn.BackColor = CafeMedio;
        }

        private static string QuitarIcono(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return string.Empty;
            texto = texto.Trim();

            // Si ya inicia con un icono/símbolo, quitamos hasta el primer espacio.
            if (texto.Length > 0 && !char.IsLetterOrDigit(texto[0]))
            {
                int espacio = texto.IndexOf(' ');
                if (espacio >= 0 && espacio + 1 < texto.Length)
                    return texto.Substring(espacio + 1);
            }
            return texto;
        }

        private static string IconoPara(string clave, string textoOriginal)
        {
            if (!string.IsNullOrWhiteSpace(textoOriginal) && !char.IsLetterOrDigit(textoOriginal.Trim()[0]))
                return string.Empty;

            if (clave.Contains("cambiar usuario") || clave.Contains("cambiar")) return "🔄 ";
            if (clave.Contains("buscar") || clave.Contains("consultar")) return "🔎 ";
            if (clave.Contains("nuevo")) return "➕ ";
            if (clave.Contains("editar")) return "✏️ ";
            if (clave.Contains("guardar")) return "💾 ";
            if (clave.Contains("cancelar")) return "✖️ ";
            if (clave.Contains("cerrar")) return "🚪 ";
            if (clave.Contains("salir")) return "🚪 ";
            if (clave.Contains("eliminar")) return "🗑️ ";
            if (clave.Contains("desactivar")) return "🚫 ";
            if (clave.Contains("activar")) return "✅ ";
            if (clave.Contains("ingresar")) return "🔐 ";
            if (clave.Contains("permiso")) return "🛡️ ";
            if (clave.Contains("actualizar")) return "🔄 ";
            if (clave.Contains("registrar venta")) return "🧾 ";
            if (clave.Contains("registrar")) return "🧾 ";
            if (clave.Contains("agregar")) return "➕ ";
            if (clave.Contains("quitar")) return "➖ ";
            if (clave.Contains("limpiar")) return "🧹 ";
            if (clave.Contains("anular")) return "🚫 ";
            if (clave.Contains("detalle")) return "📋 ";
            return "☕ ";
        }

        private static void AplicarGrilla(DataGridView dgv)
        {
            dgv.BackgroundColor = CremaPanel;
            dgv.BorderStyle = BorderStyle.None;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = CafeOscuro;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular);
            dgv.RowTemplate.Height = 30;
            dgv.ColumnHeadersHeight = 34;
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(45, 45, 45);
            dgv.DefaultCellStyle.SelectionBackColor = Dorado;
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(253, 247, 238);
            dgv.GridColor = Color.FromArgb(230, 220, 205);
            dgv.ScrollBars = ScrollBars.Both;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.RowHeadersVisible = false;
        }
    }
}
