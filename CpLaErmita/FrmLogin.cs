// ============================================================
// FrmLogin.cs  –  Capa de Presentación (CP)
// CORRECCIONES:
//   1. Se envuelve la llamada a login en try/catch para capturar
//      errores de conexión a la BD (antes la app crasheaba sin mensaje).
//   2. Se actualizan las llamadas a los métodos CLN con nombres en PascalCase.
//   3. MEJORA: al pulsar Enter en txtUsuario pasa el foco a txtContrasena.
// ============================================================
using ClnLaErmita;
using System;
using System.Windows.Forms;

namespace CpLaErmita
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
            Tema.Aplicar(this);
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            txtUsuario.Focus();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsuario.Text) ||
                string.IsNullOrWhiteSpace(txtContrasena.Text))
            {
                MessageBox.Show("Ingrese usuario y contraseña.",
                    "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var resultado = UsuarioCln.Login(txtUsuario.Text.Trim(),
                                                 txtContrasena.Text.Trim());
                if (resultado != null)
                {
                    Sesion.IdUsuario = resultado.id;
                    Sesion.IdRol     = resultado.idRol;
                    Sesion.Nombre    = resultado.nombre;
                    Sesion.Usuario   = resultado.usuario;
                    Sesion.Rol       = resultado.rol;

                    var frmPrincipal = new FrmPrincipal();
                    frmPrincipal.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Usuario o contraseña incorrectos.",
                        "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtContrasena.Clear();
                    txtUsuario.Focus();
                }
            }
            catch (Exception ex)
            {
                // CORRECCIÓN: antes crasheaba sin mostrar nada al usuario
                MessageBox.Show("No se pudo conectar a la base de datos.\n" + ex.Message,
                    "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Presionar Enter en contraseña → ingresar
        private void txtContrasena_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) btnIngresar.PerformClick();
        }

        // MEJORA: Presionar Enter en usuario → pasar a contraseña
        private void txtUsuario_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) txtContrasena.Focus();
        }
    }
}
