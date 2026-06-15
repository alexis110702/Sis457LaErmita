// ============================================================
// FrmConfiguracion.cs
// Formulario independiente de configuración del sistema.
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
        }

        private void FrmConfiguracion_Load(object sender, EventArgs e)
        {
            txtInformacion.Text =
                "Sistema profesional Cafetería / Restaurante La Ermita" + Environment.NewLine +
                "Base de datos: LabLaErmita" + Environment.NewLine +
                "Arquitectura: 3 capas + Entity Framework + SQL Server" + Environment.NewLine +
                "Seguridad: roles, permisos y contraseñas cifradas PBKDF2" + Environment.NewLine +
                "Módulos: usuarios, roles, categorías, productos, clientes, proveedores, mesas, reservas, ventas, compras, inventario y reportes.";
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
