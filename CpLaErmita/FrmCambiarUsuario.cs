// ============================================================
// FrmCambiarUsuario.cs – Cambio seguro de usuario activo
// Permite cambiar de Administrador a Cajero/Mesero u otro usuario
// sin cerrar la aplicación, validando la contraseña del usuario destino.
// ============================================================
using ClnLaErmita;
using System;
using System.Windows.Forms;

namespace CpLaErmita
{
    public partial class FrmCambiarUsuario : Form
    {
        public bool UsuarioCambiado { get; private set; }

        public FrmCambiarUsuario()
        {
            InitializeComponent();
            Tema.Aplicar(this);
        }

        private void FrmCambiarUsuario_Load(object sender, EventArgs e)
        {
            lblActual.Text = $"Usuario actual: {Sesion.Nombre}  |  Rol: {Sesion.Rol}";
            CargarUsuariosRapidos();
            txtUsuario.Focus();
        }

        private void CargarUsuariosRapidos()
        {
            cmbUsuarioRapido.Items.Clear();
            cmbUsuarioRapido.Items.Add("admin - Administrador");
            cmbUsuarioRapido.Items.Add("jmamani - Cajero");
            cmbUsuarioRapido.Items.Add("mlopez - Mesero");
            cmbUsuarioRapido.Items.Add("usuario04 - Usuario de prueba");
            cmbUsuarioRapido.SelectedIndex = -1;
        }

        private void cmbUsuarioRapido_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbUsuarioRapido.SelectedItem == null) return;

            string seleccionado = cmbUsuarioRapido.SelectedItem.ToString();
            int posicionGuion = seleccionado.IndexOf(" - ");
            txtUsuario.Text = posicionGuion > 0 ? seleccionado.Substring(0, posicionGuion).Trim() : seleccionado.Trim();
            txtContrasena.Clear();
            txtContrasena.Focus();
        }

        private void btnCambiar_Click(object sender, EventArgs e)
        {
            string usuarioDestino = txtUsuario.Text.Trim();
            string contrasenaDestino = txtContrasena.Text.Trim();

            if (string.IsNullOrWhiteSpace(usuarioDestino) || string.IsNullOrWhiteSpace(contrasenaDestino))
            {
                MessageBox.Show("Ingrese el usuario y la contraseña del usuario al que desea cambiar.",
                    "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var resultado = UsuarioCln.Login(usuarioDestino, contrasenaDestino);

                if (resultado == null)
                {
                    MessageBox.Show("Usuario o contraseña incorrectos. No se puede cambiar de usuario.",
                        "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtContrasena.Clear();
                    txtContrasena.Focus();
                    return;
                }

                string usuarioAnterior = Sesion.Usuario;
                string rolAnterior = Sesion.Rol;

                Sesion.IdUsuario = resultado.id;
                Sesion.IdRol = resultado.idRol;
                Sesion.Nombre = resultado.nombre;
                Sesion.Usuario = resultado.usuario;
                Sesion.Rol = resultado.rol;

                UsuarioCambiado = true;

                MessageBox.Show(
                    $"Cambio de usuario realizado correctamente.\n\nAnterior: {usuarioAnterior} ({rolAnterior})\nActual: {Sesion.Usuario} ({Sesion.Rol})",
                    "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cambiar de usuario.\n" + ex.Message,
                    "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            UsuarioCambiado = false;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void txtUsuario_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                txtContrasena.Focus();
            }
        }

        private void txtContrasena_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                btnCambiar.PerformClick();
            }
        }
    }
}
