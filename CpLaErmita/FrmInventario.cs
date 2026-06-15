// ============================================================
// FrmInventario.cs
// Consulta profesional de inventario, stock mínimo y alertas.
// Mantiene Windows Forms con Designer.cs.
// ============================================================
using ClnLaErmita;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace CpLaErmita
{
    public partial class FrmInventario : Form
    {
        public FrmInventario()
        {
            InitializeComponent();
            Tema.Aplicar(this);
        }

        private void FrmInventario_Load(object sender, EventArgs e)
        {
            Size = new Size(1120, 650);
            Listar();
        }

        private void Listar()
        {
            try
            {
                DataTable lista = ProfesionalCln.Listar("Inventario", txtParametro.Text);
                dgvLista.DataSource = lista;

                if (dgvLista.Columns.Contains("id")) dgvLista.Columns["id"].Visible = false;
                if (dgvLista.Columns.Contains("codigo")) dgvLista.Columns["codigo"].HeaderText = "Código";
                if (dgvLista.Columns.Contains("producto")) dgvLista.Columns["producto"].HeaderText = "Producto";
                if (dgvLista.Columns.Contains("categoria")) dgvLista.Columns["categoria"].HeaderText = "Categoría";
                if (dgvLista.Columns.Contains("stockActual")) dgvLista.Columns["stockActual"].HeaderText = "Stock Actual";
                if (dgvLista.Columns.Contains("stockMinimo")) dgvLista.Columns["stockMinimo"].HeaderText = "Stock Mínimo";
                if (dgvLista.Columns.Contains("alerta")) dgvLista.Columns["alerta"].HeaderText = "Alerta";
                if (dgvLista.Columns.Contains("fechaActualizacion")) dgvLista.Columns["fechaActualizacion"].HeaderText = "Actualización";

                ConfigurarGrilla(dgvLista);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar inventario: " + ex.Message,
                    "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e) => Listar();
        private void btnActualizar_Click(object sender, EventArgs e) => Listar();
        private void btnCerrar_Click(object sender, EventArgs e) => Close();

        private void txtParametro_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                Listar();
            }
        }

        private void ConfigurarGrilla(DataGridView dgv)
        {
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgv.ScrollBars = ScrollBars.Both;
            dgv.AllowUserToResizeColumns = true;
            dgv.AllowUserToResizeRows = false;
            dgv.ReadOnly = true;
            dgv.MultiSelect = false;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }
    }
}
