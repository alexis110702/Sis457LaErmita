// ============================================================
// FrmPrincipal.cs – Menú Principal Profesional
// MEJORAS:
//   1. Menú moderno con colores e iconos por módulo.
//   2. Cada módulo se abre ocultando el menú principal para evitar
//      pantallas una sobre otra.
//   3. Permisos solicita nuevamente la contraseña del usuario actual.
// ============================================================
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CpLaErmita
{
    public partial class FrmPrincipal : Form
    {
        private FlowLayoutPanel panelMenu;

        public FrmPrincipal()
        {
            InitializeComponent();
            Tema.Aplicar(this);
        }

        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            ActualizarEncabezado();
            ConstruirMenuProfesional();
        }

        private void ActualizarEncabezado()
        {
            lblBienvenido.Text = $"Usuario activo: {Sesion.Nombre}  |  Usuario: {Sesion.Usuario}  |  Rol: {Sesion.Rol}  |  {DateTime.Now:dd/MM/yyyy HH:mm}";
        }

        private void ConstruirMenuProfesional()
        {
            if (panelMenu != null)
            {
                Controls.Remove(panelMenu);
                panelMenu.Dispose();
                panelMenu = null;
            }

            ActualizarEncabezado();
            Text = "La Ermita - Sistema Profesional";
            Size = new Size(1160, 760);
            MinimumSize = new Size(1000, 680);
            FormBorderStyle = FormBorderStyle.Sizable;
            BackColor = Tema.Crema;

            btnCategoria.Visible = false;
            btnProducto.Visible = false;
            btnVenta.Visible = false;
            btnSalir.Visible = false;

            lblTitulo.Text = "☕ Cafetería / Restaurante La Ermita";
            lblTitulo.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitulo.ForeColor = Tema.CafeOscuro;
            lblTitulo.Width = ClientSize.Width;
            lblTitulo.Height = 55;
            lblTitulo.Location = new Point(0, 15);
            lblTitulo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            lblBienvenido.Location = new Point(20, 78);
            lblBienvenido.Width = ClientSize.Width - 40;
            lblBienvenido.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblBienvenido.BackColor = Color.FromArgb(255, 252, 246);
            lblBienvenido.ForeColor = Tema.CafeOscuro;
            lblBienvenido.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);

            panelMenu = new FlowLayoutPanel
            {
                Location = new Point(25, 125),
                Size = new Size(ClientSize.Width - 50, ClientSize.Height - 170),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(255, 252, 246)
            };
            Controls.Add(panelMenu);

            bool esAdmin = Sesion.Rol == "Administrador";
            bool esCajero = Sesion.Rol == "Cajero";
            bool esMesero = Sesion.Rol == "Mesero";

            if (esAdmin)
            {
                AgregarBoton("👤 Usuarios", () => AbrirFormulario(new FrmUsuario()));
                AgregarBoton("🛡️ Roles", () => AbrirFormulario(new FrmRol()));
                AgregarBoton("🔐 Permisos", AbrirPermisosConClave);
                AgregarBoton("📂 Categorías", () => AbrirFormulario(new FrmCategoria()));
                AgregarBoton("🍔 Productos", () => AbrirFormulario(new FrmProducto()));
                AgregarBoton("🚚 Proveedores", () => AbrirFormulario(new FrmProveedor()));
                AgregarBoton("💳 Métodos Pago", () => AbrirFormulario(new FrmMetodoPago()));
            }

            if (esAdmin || esCajero || esMesero)
            {
                AgregarBoton("🤝 Clientes", () => AbrirFormulario(new FrmCliente()));
                AgregarBoton("🍽️ Mesas", () => AbrirFormulario(new FrmMesa()));
                AgregarBoton("📅 Reservas", () => AbrirFormulario(new FrmReserva()));
                AgregarBoton("🧾 Ventas", () => AbrirFormulario(new FrmVenta()));
            }

            if (esAdmin || esCajero)
            {
                AgregarBoton("🛒 Compras", () => AbrirFormulario(new FrmCompra()));
                AgregarBoton("📦 Inventario", () => AbrirFormulario(new FrmInventario()));
                AgregarBoton("📊 Reportes", () => AbrirFormulario(new FrmReportes()));
            }

            AgregarBoton("🔄 Cambiar Usuario", CambiarUsuario);
            AgregarBoton("⚙️ Configuración", () => AbrirFormulario(new FrmConfiguracion()));
            AgregarBoton("🚪 Salir", () => Application.Exit());
        }

        private void AgregarBoton(string texto, Action accion)
        {
            var btn = new Button
            {
                Text = texto,
                Width = 240,
                Height = 85,
                Margin = new Padding(10),
                Font = new Font("Segoe UI", 12.5F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(255, 252, 246),
                ForeColor = Tema.CafeOscuro,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Tema.Dorado;
            btn.FlatAppearance.BorderSize = 2;
            btn.Click += (s, e) => accion();
            panelMenu.Controls.Add(btn);
        }

        private void CambiarUsuario()
        {
            using (var frm = new FrmCambiarUsuario())
            {
                if (frm.ShowDialog(this) == DialogResult.OK && frm.UsuarioCambiado)
                {
                    ConstruirMenuProfesional();
                    MessageBox.Show("Los permisos del menú fueron actualizados según el nuevo rol.",
                        "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void AbrirFormulario(Form formulario)
        {
            try
            {
                Hide();
                using (formulario)
                {
                    formulario.StartPosition = FormStartPosition.CenterScreen;
                    formulario.ShowDialog();
                }
            }
            finally
            {
                if (!IsDisposed)
                {
                    Show();
                    Activate();
                }
            }
        }

        private void AbrirPermisosConClave()
        {
            using (var confirmar = new FrmConfirmarPassword())
            {
                if (confirmar.ShowDialog(this) == DialogResult.OK && confirmar.Autorizado)
                    AbrirFormulario(new FrmPermisosRol());
            }
        }

        private void FrmPrincipal_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        // Métodos originales conservados para no romper eventos del Designer.
        private void btnCategoria_Click(object sender, EventArgs e) => AbrirFormulario(new FrmCategoria());
        private void btnProducto_Click(object sender, EventArgs e) => AbrirFormulario(new FrmProducto());
        private void btnVenta_Click(object sender, EventArgs e) => AbrirFormulario(new FrmVenta());
        private void btnSalir_Click(object sender, EventArgs e) => Application.Exit();
    }
}
