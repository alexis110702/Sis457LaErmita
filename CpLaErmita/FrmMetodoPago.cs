// ============================================================
// FrmMetodoPago.cs
// Formulario WinForms independiente del módulo Métodos de Pago.
// Incluye búsqueda, nuevo, editar, guardar, activar/desactivar
// y validaciones, siguiendo la estructura de FrmCategoria.
// ============================================================
using ClnLaErmita;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace CpLaErmita
{
    public partial class FrmMetodoPago : Form
    {
        private bool esNuevo = false;
        private int idActual = 0;

        public FrmMetodoPago()
        {
            InitializeComponent();
            Tema.Aplicar(this);
        }

        private void FrmMetodoPago_Load(object sender, EventArgs e)
        {
            Size = new Size(1120, 720);
            pnlFormulario.Visible = false;
            CargarDatosIniciales();
            Listar();
        }

        private void CargarDatosIniciales()
        {
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar datos iniciales. Ejecute el script profesional de base de datos.\n" + ex.Message,
                    "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }



        }

        private void Listar()
        {
            try
            {
                DataTable lista = ProfesionalCln.Listar("MetodosPago", txtParametro.Text);
                dgvLista.DataSource = lista;

                if (dgvLista.Columns.Contains("id")) dgvLista.Columns["id"].Visible = false;
                if (dgvLista.Columns.Contains("nombre")) dgvLista.Columns["nombre"].HeaderText = "Nombre"; if (dgvLista.Columns.Contains("descripcion")) dgvLista.Columns["descripcion"].HeaderText = "Descripción"; if (dgvLista.Columns.Contains("estado")) dgvLista.Columns["estado"].HeaderText = "Estado";

                ConfigurarGrilla(dgvLista);
                bool hayDatos = lista.Rows.Count > 0;
                btnEditar.Enabled = hayDatos;
                btnEliminar.Enabled = hayDatos;
                btnActivar.Enabled = hayDatos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al listar Métodos de Pago: " + ex.Message,
                    "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e) => Listar();

        private void txtParametro_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                Listar();
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            esNuevo = true;
            idActual = 0;
            pnlAcciones.Enabled = false;
            pnlFormulario.Visible = true;
            Limpiar();
            txtNombre.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvLista.CurrentRow == null) return;

            esNuevo = false;
            idActual = Convert.ToInt32(dgvLista.CurrentRow.Cells["id"].Value);
            pnlAcciones.Enabled = false;
            pnlFormulario.Visible = true;
            ResetearErrores();

            txtNombre.Text = ValorCelda("nombre"); txtDescripcion.Text = ValorCelda("descripcion");
            txtNombre.Focus();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            pnlAcciones.Enabled = true;
            pnlFormulario.Visible = false;
            Limpiar();
            txtParametro.Focus();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Validar()) return;

            try
            {
                var valores = new Dictionary<string, string>();
                valores["nombre"] = txtNombre.Text.Trim(); valores["descripcion"] = txtDescripcion.Text.Trim();

                ProfesionalCln.Guardar("MetodosPago", esNuevo ? 0 : idActual, valores);
                Listar();
                btnCancelar.PerformClick();
                MessageBox.Show("Registro guardado correctamente.",
                    "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message,
                    "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            CambiarEstado(0, "desactivar");
        }

        private void btnActivar_Click(object sender, EventArgs e)
        {
            CambiarEstado(1, "activar");
        }

        private void CambiarEstado(short estado, string accion)
        {
            if (dgvLista.CurrentRow == null) return;

            int id = Convert.ToInt32(dgvLista.CurrentRow.Cells["id"].Value);
            if (MessageBox.Show("¿Desea " + accion + " este registro?",
                "::: La Ermita :::", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                ProfesionalCln.CambiarEstadoLogico("MetodosPago", id, estado);
                Listar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cambiar estado: " + ex.Message,
                    "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e) => Close();

        private void Limpiar()
        {
            txtNombre.Clear(); txtDescripcion.Clear();
            ResetearErrores();
        }

        private void ResetearErrores()
        {
            erpPrincipal.Clear();
        }

        private bool Validar()
        {
            bool ok = true;
            ResetearErrores();
            if (string.IsNullOrWhiteSpace(txtNombre.Text)) { erpPrincipal.SetError(txtNombre, "El campo Nombre es obligatorio."); ok = false; }



            return ok;
        }

        private string ValorCelda(string columna)
        {
            object valor = ValorCeldaObjeto(columna);
            return valor == null || valor == DBNull.Value ? string.Empty : Convert.ToString(valor);
        }

        private object ValorCeldaObjeto(string columna)
        {
            if (dgvLista.CurrentRow == null) return null;
            if (!dgvLista.Columns.Contains(columna)) return null;
            return dgvLista.CurrentRow.Cells[columna].Value;
        }

        private void CargarCombo(ComboBox combo, string modulo, string columnaMostrar)
        {
            DataTable dt = ProfesionalCln.Listar(modulo, string.Empty);
            combo.DataSource = dt;
            combo.DisplayMember = columnaMostrar;
            combo.ValueMember = "id";
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private string ObtenerValorCombo(ComboBox combo)
        {
            return combo.SelectedValue == null ? string.Empty : Convert.ToString(combo.SelectedValue);
        }

        private void SeleccionarCombo(ComboBox combo, object valor)
        {
            if (valor == null || valor == DBNull.Value) return;
            try { combo.SelectedValue = Convert.ToInt32(valor); } catch { }
        }

        private void SeleccionarTexto(ComboBox combo, string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return;
            int indice = combo.Items.IndexOf(texto);
            if (indice >= 0) combo.SelectedIndex = indice;
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
