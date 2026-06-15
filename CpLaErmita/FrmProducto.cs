// ============================================================
// FrmProducto.cs  –  Capa de Presentación (CP)
// CORRECCIONES:
//   1. Llamadas a CLN actualizadas a PascalCase.
//   2. Se usa el retorno (bool ok, string mensaje) de Eliminar.
//   3. Try/catch en guardar y eliminar.
//   4. CORRECCIÓN: decimal.Parse puede fallar con separadores regionales;
//      se usa decimal.TryParse con cultura invariante para evitar crashes.
// ============================================================
using CadLaErmita;
using ClnLaErmita;
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace CpLaErmita
{
    public partial class FrmProducto : Form
    {
        private bool esNuevo = false;

        public FrmProducto() { InitializeComponent();
            Tema.Aplicar(this); }

        private void Listar()
        {
            var lista = ProductoCln.ListarPa(txtParametro.Text);
            dgvLista.DataSource = lista;
            dgvLista.Columns["id"].Visible          = false;
            dgvLista.Columns["idCategoria"].Visible = false;
            dgvLista.Columns["estado"].Visible      = false;
            dgvLista.Columns["nombre"].HeaderText      = "Nombre";
            dgvLista.Columns["descripcion"].HeaderText = "Descripción";
            dgvLista.Columns["categoria"].HeaderText   = "Categoría";
            dgvLista.Columns["precio"].HeaderText      = "Precio (Bs.)";
            dgvLista.Columns["stock"].HeaderText       = "Stock";
            ConfigurarGrilla(dgvLista);
            if (lista.Count > 0) dgvLista.CurrentCell = dgvLista.Rows[0].Cells["nombre"];
            btnEditar.Enabled   = lista.Count > 0;
            btnEliminar.Enabled = lista.Count > 0;
        }

        private void CargarCategorias()
        {
            cbxCategoria.DataSource    = CategoriaCln.Listar();
            cbxCategoria.ValueMember   = "id";
            cbxCategoria.DisplayMember = "nombre";
            cbxCategoria.SelectedIndex = -1;
        }

        private void FrmProducto_Load(object sender, EventArgs e)
        {
            Size = new Size(1080, 430);
            pnlFormulario.Visible = false;
            CargarCategorias();
            Listar();
        }

        private void btnBuscar_Click(object sender, EventArgs e) => Listar();
        private void txtParametro_KeyPress(object sender, KeyPressEventArgs e)
        { if (e.KeyChar == (char)Keys.Enter) Listar(); }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            esNuevo = true;
            pnlAcciones.Enabled   = false;
            pnlFormulario.Visible = true;
            Size = new Size(1080, 660);
            Limpiar();
            txtNombre.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            esNuevo = false;
            pnlAcciones.Enabled   = false;
            pnlFormulario.Visible = true;
            Size = new Size(1080, 660);
            ResetarErrores();

            int id = (int)dgvLista.CurrentRow.Cells["id"].Value;
            var p  = ProductoCln.ObtenerUno(id);
            txtNombre.Text             = p.nombre;
            txtDescripcion.Text        = p.descripcion;
            cbxCategoria.SelectedValue = p.idCategoria;
            txtPrecio.Text             = p.precio.ToString("F2", CultureInfo.InvariantCulture);
            nudStock.Value             = p.stock;
            txtNombre.Focus();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            pnlAcciones.Enabled   = true;
            pnlFormulario.Visible = false;
            Size = new Size(1080, 430);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Validar()) return;

            int idActual = esNuevo ? 0 : (int)dgvLista.CurrentRow.Cells["id"].Value;
            if (ProductoCln.ExisteNombre(txtNombre.Text, idActual))
            {
                erpNombre.SetError(txtNombre, "Ya existe un producto con ese nombre.");
                return;
            }

            try
            {
                // CORRECCIÓN: usar InvariantCulture para parsear decimales
                decimal.TryParse(txtPrecio.Text, NumberStyles.Any,
                                 CultureInfo.InvariantCulture, out decimal precio);

                var prod = new Producto
                {
                    idCategoria = (int)cbxCategoria.SelectedValue,
                    nombre      = txtNombre.Text.Trim(),
                    descripcion = txtDescripcion.Text.Trim(),
                    precio      = precio,
                    stock       = (int)nudStock.Value
                };

                if (esNuevo)
                    ProductoCln.Crear(prod);
                else
                {
                    prod.id = (int)dgvLista.CurrentRow.Cells["id"].Value;
                    ProductoCln.Modificar(prod);
                }

                Listar();
                btnCancelar.PerformClick();
                MessageBox.Show("Producto guardado correctamente.",
                    "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message,
                    "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            int    id = (int)dgvLista.CurrentRow.Cells["id"].Value;
            string nm = dgvLista.CurrentRow.Cells["nombre"].Value.ToString();

            if (MessageBox.Show($"¿Eliminar el producto \"{nm}\"?",
                "::: La Ermita :::", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                != DialogResult.Yes) return;

            try
            {
                var (ok, mensaje) = ProductoCln.Eliminar(id);
                if (ok) { Listar(); MessageBox.Show(mensaje, "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Information); }
                else    { MessageBox.Show(mensaje, "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message,
                    "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e) => Close();

        private void Limpiar()
        {
            txtNombre.Clear();
            txtDescripcion.Clear();
            cbxCategoria.SelectedIndex = -1;
            txtPrecio.Clear();
            nudStock.Value = 0;
            ResetarErrores();
        }

        private void ResetarErrores()
        {
            erpNombre.Clear();
            erpDescripcion.Clear();
            erpCategoria.Clear();
            erpPrecio.Clear();
        }

        private bool Validar()
        {
            bool ok = true;
            ResetarErrores();
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            { erpNombre.SetError(txtNombre, "El nombre es obligatorio."); ok = false; }
            if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
            { erpDescripcion.SetError(txtDescripcion, "La descripción es obligatoria."); ok = false; }
            if (cbxCategoria.SelectedIndex == -1)
            { erpCategoria.SetError(cbxCategoria, "Seleccione una categoría."); ok = false; }
            if (string.IsNullOrWhiteSpace(txtPrecio.Text) ||
                !decimal.TryParse(txtPrecio.Text, NumberStyles.Any,
                                  CultureInfo.InvariantCulture, out decimal precioVal) ||
                precioVal <= 0)
            { erpPrecio.SetError(txtPrecio, "Ingrese un precio válido mayor a 0."); ok = false; }
            return ok;
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
        }

        private void txtPrecio_KeyPress(object sender, KeyPressEventArgs e)
        { Util.OnlyDecimals(sender, e); }
    }
}
