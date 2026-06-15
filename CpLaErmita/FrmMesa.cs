// ============================================================
// FrmMesa.cs
// Formulario WinForms independiente del módulo Mesas.
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
    public partial class FrmMesa : Form
    {
        private bool esNuevo = false;
        private int idActual = 0;

        public FrmMesa()
        {
            InitializeComponent();
            Tema.Aplicar(this);
        }

        private void FrmMesa_Load(object sender, EventArgs e)
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
                cmbEstadoMesa.Items.Clear(); cmbEstadoMesa.Items.AddRange(new object[] { "Libre", "Ocupada", "Reservada" }); cmbEstadoMesa.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar datos iniciales. Ejecute el script profesional de base de datos.\n" + ex.Message,
                    "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            txtCapacidad.KeyPress += Util.OnlyNumbers;

        }

        private void Listar()
        {
            try
            {
                DataTable lista = ProfesionalCln.Listar("Mesas", txtParametro.Text);
                dgvLista.DataSource = lista;

                if (dgvLista.Columns.Contains("id")) dgvLista.Columns["id"].Visible = false;
                if (dgvLista.Columns.Contains("numero")) dgvLista.Columns["numero"].HeaderText = "Número"; if (dgvLista.Columns.Contains("capacidad")) dgvLista.Columns["capacidad"].HeaderText = "Capacidad"; if (dgvLista.Columns.Contains("estadoMesa")) dgvLista.Columns["estadoMesa"].HeaderText = "Estado Mesa"; if (dgvLista.Columns.Contains("ubicacion")) dgvLista.Columns["ubicacion"].HeaderText = "Ubicación"; if (dgvLista.Columns.Contains("estado")) dgvLista.Columns["estado"].HeaderText = "Estado";

                ConfigurarGrilla(dgvLista);
                bool hayDatos = lista.Rows.Count > 0;
                btnEditar.Enabled = hayDatos;
                btnEliminar.Enabled = hayDatos;
                btnActivar.Enabled = hayDatos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al listar Mesas: " + ex.Message,
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
            txtNumero.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvLista.CurrentRow == null) return;

            esNuevo = false;
            idActual = Convert.ToInt32(dgvLista.CurrentRow.Cells["id"].Value);
            pnlAcciones.Enabled = false;
            pnlFormulario.Visible = true;
            ResetearErrores();

            txtNumero.Text = ValorCelda("numero"); txtCapacidad.Text = ValorCelda("capacidad"); SeleccionarTexto(cmbEstadoMesa, ValorCelda("estadoMesa")); txtUbicacion.Text = ValorCelda("ubicacion");
            txtNumero.Focus();
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
                valores["numero"] = txtNumero.Text.Trim(); valores["capacidad"] = txtCapacidad.Text.Trim(); valores["estadoMesa"] = Convert.ToString(cmbEstadoMesa.SelectedItem); valores["ubicacion"] = txtUbicacion.Text.Trim();

                ProfesionalCln.Guardar("Mesas", esNuevo ? 0 : idActual, valores);
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
                ProfesionalCln.CambiarEstadoLogico("Mesas", id, estado);
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
            txtNumero.Clear(); txtCapacidad.Clear(); if (cmbEstadoMesa.Items.Count > 0) cmbEstadoMesa.SelectedIndex = 0; txtUbicacion.Clear();
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
            if (string.IsNullOrWhiteSpace(txtNumero.Text)) { erpPrincipal.SetError(txtNumero, "El campo Número es obligatorio."); ok = false; } if (string.IsNullOrWhiteSpace(txtCapacidad.Text)) { erpPrincipal.SetError(txtCapacidad, "El campo Capacidad es obligatorio."); ok = false; } if (cmbEstadoMesa.SelectedItem == null) { erpPrincipal.SetError(cmbEstadoMesa, "Seleccione Estado Mesa."); ok = false; }
            int tmptxtCapacidad; if (!string.IsNullOrWhiteSpace(txtCapacidad.Text) && !int.TryParse(txtCapacidad.Text, out tmptxtCapacidad)) { erpPrincipal.SetError(txtCapacidad, "Ingrese un número entero válido."); ok = false; }


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
