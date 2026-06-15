// ============================================================
// FrmPrincipal.cs  –  Menú Principal Profesional
// MEJORAS:
//   1. Menú moderno con iconos por módulo.
//   2. Accesos a módulos nuevos: usuarios, roles, clientes,
//      proveedores, mesas, métodos de pago, reservas, compras,
//      inventario y reportes.
//   3. Restricción visual básica por rol.
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
        }

        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            lblBienvenido.Text = $"Bienvenido: {Sesion.Nombre}  |  Rol: {Sesion.Rol}  |  {DateTime.Now:dd/MM/yyyy HH:mm}";
            ConstruirMenuProfesional();
        }

        private void ConstruirMenuProfesional()
        {
            Text = "La Ermita - Sistema Profesional";
            Size = new Size(980, 650);
            MinimumSize = new Size(900, 600);
            FormBorderStyle = FormBorderStyle.Sizable;

            btnCategoria.Visible = false;
            btnProducto.Visible = false;
            btnVenta.Visible = false;
            btnSalir.Visible = false;

            lblTitulo.Text = "☕ Cafetería / Restaurante La Ermita";
            lblTitulo.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitulo.Width = ClientSize.Width;
            lblTitulo.Height = 55;
            lblTitulo.Location = new Point(0, 15);
            lblTitulo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            lblBienvenido.Location = new Point(20, 78);
            lblBienvenido.Width = ClientSize.Width - 40;
            lblBienvenido.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            panelMenu = new FlowLayoutPanel
            {
                Location = new Point(25, 125),
                Size = new Size(ClientSize.Width - 50, ClientSize.Height - 170),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(10)
            };
            Controls.Add(panelMenu);

            bool esAdmin = Sesion.Rol == "Administrador";
            bool esCajero = Sesion.Rol == "Cajero";
            bool esMesero = Sesion.Rol == "Mesero";

            if (esAdmin)
            {
                AgregarBoton("👤 Usuarios", () => new FrmUsuario().ShowDialog());
                AgregarBoton("🛡️ Roles", () => new FrmRol().ShowDialog());
                AgregarBoton("🔐 Permisos", () => new FrmPermisosRol().ShowDialog());
                AgregarBoton("📂 Categorías", () => new FrmCategoria().ShowDialog());
                AgregarBoton("🍔 Productos", () => new FrmProducto().ShowDialog());
                AgregarBoton("🚚 Proveedores", () => new FrmProveedor().ShowDialog());
                AgregarBoton("💳 Métodos Pago", () => new FrmMetodoPago().ShowDialog());
            }

            if (esAdmin || esCajero || esMesero)
            {
                AgregarBoton("🤝 Clientes", () => new FrmCliente().ShowDialog());
                AgregarBoton("🍽️ Mesas", () => new FrmMesa().ShowDialog());
                AgregarBoton("📅 Reservas", () => new FrmReserva().ShowDialog());
                AgregarBoton("🧾 Ventas", () => new FrmVenta().ShowDialog());
            }

            if (esAdmin || esCajero)
            {
                AgregarBoton("🛒 Compras", () => new FrmCompra().ShowDialog());
                AgregarBoton("📦 Inventario", () => new FrmInventario().ShowDialog());
                AgregarBoton("📊 Reportes", () => new FrmReportes().ShowDialog());
            }

            AgregarBoton("⚙️ Configuración", () => new FrmConfiguracion().ShowDialog());
            AgregarBoton("🚪 Salir", () => Application.Exit());
        }

        private void AgregarBoton(string texto, Action accion)
        {
            var btn = new Button
            {
                Text = texto,
                Width = 200,
                Height = 72,
                Margin = new Padding(10),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(245, 245, 245)
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(210, 210, 210);
            btn.Click += (s, e) => accion();
            panelMenu.Controls.Add(btn);
        }

        private void FrmPrincipal_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        // Métodos originales conservados para no romper eventos del Designer.
        private void btnCategoria_Click(object sender, EventArgs e) => new FrmCategoria().ShowDialog();
        private void btnProducto_Click(object sender, EventArgs e) => new FrmProducto().ShowDialog();
        private void btnVenta_Click(object sender, EventArgs e) => new FrmVenta().ShowDialog();
        private void btnSalir_Click(object sender, EventArgs e) => Application.Exit();
    }
}
