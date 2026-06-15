using ClnLaErmita;
using System;
using System.Windows.Forms;

namespace CpLaErmita
{
    public partial class FrmReportes : Form
    {
        public FrmReportes()
        {
            InitializeComponent();
            CargarTiposReporte();
            AsociarEventos();
        }

        private void CargarTiposReporte()
        {
            cbxReporte.Items.Clear();
            cbxReporte.Items.AddRange(new object[]
            {
                "Ventas diarias",
                "Ventas mensuales",
                "Productos más vendidos",
                "Clientes frecuentes",
                "Compras realizadas",
                "Inventario actual",
                "Productos con poco stock",
                "Ingresos y ganancias"
            });

            if (cbxReporte.Items.Count > 0)
                cbxReporte.SelectedIndex = 0;
        }

        private void AsociarEventos()
        {
            Load += FrmReportes_Load;
            btnConsultar.Click += BtnConsultar_Click;
            btnCerrar.Click += BtnCerrar_Click;
        }

        private void FrmReportes_Load(object sender, EventArgs e)
        {
            Consultar();
        }

        private void BtnConsultar_Click(object sender, EventArgs e)
        {
            Consultar();
        }

        private void BtnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Consultar()
        {
            try
            {
                dgvReporte.DataSource = ReportesCln.Ejecutar(cbxReporte.Text);
                dgvReporte.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvReporte.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cargar el reporte. Verifique que ejecutó el script profesional.\n" + ex.Message,
                    "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
