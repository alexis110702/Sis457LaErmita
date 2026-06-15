// ============================================================
// FrmCategoria.cs  –  Capa de Presentación (CP)
// CORRECCIONES:
//   1. Todas las llamadas a CLN actualizadas a PascalCase.
//   2. Se usa el nuevo retorno (bool ok, string mensaje) de Eliminar,
//      mostrando al usuario si no se puede eliminar por tener productos.
//   3. Se agrega try/catch en operaciones de guardado y eliminación.
//   4. MEJORA: se restablece el foco a txtParametro tras cancelar.
// ============================================================
using CadLaErmita;
using ClnLaErmita;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CpLaErmita
{
    public partial class FrmCategoria : Form
    {
        private bool esNuevo = false;

        public FrmCategoria() { InitializeComponent();
            Tema.Aplicar(this); }

        private void Listar()
        {
            var lista = CategoriaCln.ListarPa(txtParametro.Text);
            dgvLista.DataSource = lista;
            dgvLista.Columns["id"].Visible          = false;
            dgvLista.Columns["estado"].Visible      = false;
            dgvLista.Columns["nombre"].HeaderText      = "Nombre";
            dgvLista.Columns["descripcion"].HeaderText = "Descripción";
            ConfigurarGrilla(dgvLista);
            if (lista.Count > 0) dgvLista.CurrentCell = dgvLista.Rows[0].Cells["nombre"];
            btnEditar.Enabled   = lista.Count > 0;
            btnEliminar.Enabled = lista.Count > 0;
        }

        private void FrmCategoria_Load(object sender, EventArgs e)
        {
            Size = new Size(840, 420);
            pnlFormulario.Visible = false;
            Listar();
        }

        private void btnBuscar_Click(object sender, EventArgs e)  => Listar();
        private void txtParametro_KeyPress(object sender, KeyPressEventArgs e)
        { if (e.KeyChar == (char)Keys.Enter) Listar(); }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            esNuevo = true;
            pnlAcciones.Enabled   = false;
            pnlFormulario.Visible = true;
            Size = new Size(840, 580);
            Limpiar();
            txtNombre.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            esNuevo = false;
            pnlAcciones.Enabled   = false;
            pnlFormulario.Visible = true;
            Size = new Size(840, 580);
            ResetarErrores();

            int id  = (int)dgvLista.CurrentRow.Cells["id"].Value;
            var cat = CategoriaCln.ObtenerUno(id);
            txtNombre.Text      = cat.nombre;
            txtDescripcion.Text = cat.descripcion;
            txtNombre.Focus();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            pnlAcciones.Enabled   = true;
            pnlFormulario.Visible = false;
            Size = new Size(840, 420);
            txtParametro.Focus();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Validar()) return;

            // Validación: no permitir nombre duplicado
            int idActual = esNuevo ? 0 : (int)dgvLista.CurrentRow.Cells["id"].Value;
            if (CategoriaCln.ExisteNombre(txtNombre.Text, idActual))
            {
                erpNombre.SetError(txtNombre, "Ya existe una categoría con ese nombre.");
                return;
            }

            try
            {
                var cat = new Categoria
                {
                    nombre = txtNombre.Text.Trim(),
                    descripcion = txtDescripcion.Text.Trim()
                };

                if (esNuevo)
                {
                    CategoriaCln.Crear(cat);
                }
                else
                {
                    cat.id = (int)dgvLista.CurrentRow.Cells["id"].Value;
                    CategoriaCln.Modificar(cat);
                }

                Listar();
                btnCancelar.PerformClick();
                MessageBox.Show("Categoría guardada correctamente.",
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

            if (MessageBox.Show($"¿Eliminar la categoría \"{nm}\"?",
                "::: La Ermita :::", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                != DialogResult.Yes) return;

            try
            {
                // CORRECCIÓN: ahora recibimos si fue exitoso o hay restricción
                var (ok, mensaje) = CategoriaCln.Eliminar(id);
                if (ok)
                {
                    Listar();
                    MessageBox.Show(mensaje, "::: La Ermita :::",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(mensaje, "::: La Ermita :::",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
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
            ResetarErrores();
        }

        private void ResetarErrores()
        {
            erpNombre.Clear();
            erpDescripcion.Clear();
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

        private bool Validar()
        {
            bool ok = true;
            ResetarErrores();
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            { erpNombre.SetError(txtNombre, "El nombre es obligatorio."); ok = false; }
            if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
            { erpDescripcion.SetError(txtDescripcion, "La descripción es obligatoria."); ok = false; }
            return ok;
        }
    }
}
