// ============================================================
// FrmConfirmarPassword.cs – Reautenticación para zonas críticas
// Se usa antes de ingresar a Permisos para confirmar que el
// administrador actual realmente autoriza el acceso.
// ============================================================
using ClnLaErmita;
using System;
using System.Windows.Forms;

namespace CpLaErmita
{
    public partial class FrmConfirmarPassword : Form
    {
        public bool Autorizado { get; private set; }

        public FrmConfirmarPassword()
        {
            InitializeComponent();
            Tema.Aplicar(this);
        }

        private void FrmConfirmarPassword_Load(object sender, EventArgs e)
        {
            lblUsuario.Text = "Usuario: " + (Sesion.Usuario ?? "");
            txtContrasena.Clear();
            txtContrasena.Focus();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtContrasena.Text))
            {
                MessageBox.Show("Ingrese nuevamente su contraseña.",
                    "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContrasena.Focus();
                return;
            }

            try
            {
                var usuario = UsuarioCln.Login(Sesion.Usuario, txtContrasena.Text.Trim());
                if (usuario == null || usuario.id != Sesion.IdUsuario)
                {
                    MessageBox.Show("Contraseña incorrecta. No se puede ingresar a permisos.",
                        "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtContrasena.Clear();
                    txtContrasena.Focus();
                    return;
                }

                Autorizado = true;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo validar la contraseña.\n" + ex.Message,
                    "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Autorizado = false;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void txtContrasena_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                btnAceptar.PerformClick();
            }
        }
    }
}
