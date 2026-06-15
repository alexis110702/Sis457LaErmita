// ============================================================
// FrmConfiguracion.cs
// Pantalla informativa de configuración del sistema.
// ============================================================
using System;
using System.Windows.Forms;

namespace CpLaErmita
{
    public partial class FrmConfiguracion : Form
    {
        public FrmConfiguracion()
        {
            InitializeComponent();
            Tema.Aplicar(this);
        }

        private void FrmConfiguracion_Load(object sender, EventArgs e)
        {
            lblUsuario.Text = "Usuario actual: " + Sesion.Nombre;
            lblRol.Text = "Rol: " + Sesion.Rol;
            lblFecha.Text = "Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            txtDescripcion.Text = "Sistema profesional La Ermita\r\n\r\n" +
                "Módulos implementados:\r\n" +
                "- Seguridad: usuarios, roles y permisos.\r\n" +
                "- Catálogos: categorías, productos, proveedores y métodos de pago.\r\n" +
                "- Restaurante: clientes, mesas y reservas.\r\n" +
                "- Operaciones: ventas, compras e inventario.\r\n" +
                "- Reportes: ventas, compras, inventario y ganancias.\r\n\r\n" +
                "Recomendación: antes de la defensa ejecute el script SQL profesional y pruebe cada módulo.";
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
