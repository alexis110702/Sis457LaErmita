// ============================================================
// FrmReserva.cs
// Formulario WinForms independiente del módulo Reservas.
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
    public partial class FrmReserva : Form
    {
        private bool esNuevo = false;
        private int idActual = 0;

        public FrmReserva()
        {
            InitializeComponent();
            Tema.Aplicar(this);
        }

        private void FrmReserva_Load(object sender, EventArgs e)
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
                CargarCombo(cmbCliente, "Clientes", "nombre"); CargarCombo(cmbMesa, "Mesas", "numero"); cmbEstadoReserva.Items.Clear(); cmbEstadoReserva.Items.AddRange(new object[] { "Registrada", "Confirmada", "Cancelada", "Atendida" }); cmbEstadoReserva.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar datos iniciales. Ejecute el script profesional de base de datos.\n" + ex.Message,
                    "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            txtCantidadPersonas.KeyPress += Util.OnlyNumbers;

        }

        private void Listar()
        {
            try
            {
                DataTable lista = ProfesionalCln.Listar("Reservas", txtParametro.Text);
                dgvLista.DataSource = lista;

                if (dgvLista.Columns.Contains("id")) dgvLista.Columns["id"].Visible = false; if (dgvLista.Columns.Contains("idCliente")) dgvLista.Columns["idCliente"].Visible = false; if (dgvLista.Columns.Contains("idMesa")) dgvLista.Columns["idMesa"].Visible = false;
                if (dgvLista.Columns.Contains("cliente")) dgvLista.Columns["cliente"].HeaderText = "Cliente"; if (dgvLista.Columns.Contains("mesa")) dgvLista.Columns["mesa"].HeaderText = "Mesa"; if (dgvLista.Columns.Contains("fechaReserva")) dgvLista.Columns["fechaReserva"].HeaderText = "Fecha"; if (dgvLista.Columns.Contains("horaReserva")) dgvLista.Columns["horaReserva"].HeaderText = "Hora"; if (dgvLista.Columns.Contains("cantidadPersonas")) dgvLista.Columns["cantidadPersonas"].HeaderText = "Personas"; if (dgvLista.Columns.Contains("estadoReserva")) dgvLista.Columns["estadoReserva"].HeaderText = "Estado Reserva"; if (dgvLista.Columns.Contains("observacion")) dgvLista.Columns["observacion"].HeaderText = "Observación"; if (dgvLista.Columns.Contains("estado")) dgvLista.Columns["estado"].HeaderText = "Estado";

                ConfigurarGrilla(dgvLista);
                bool hayDatos = lista.Rows.Count > 0;
                btnEditar.Enabled = hayDatos;
                btnEliminar.Enabled = hayDatos;
                btnActivar.Enabled = hayDatos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al listar Reservas: " + ex.Message,
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
            cmbCliente.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvLista.CurrentRow == null) return;

            esNuevo = false;
            idActual = Convert.ToInt32(dgvLista.CurrentRow.Cells["id"].Value);
            pnlAcciones.Enabled = false;
            pnlFormulario.Visible = true;
            ResetearErrores();

            SeleccionarCombo(cmbCliente, ValorCeldaObjeto("idCliente")); SeleccionarCombo(cmbMesa, ValorCeldaObjeto("idMesa")); DateTime fechadtpFechaReserva; if (DateTime.TryParse(ValorCelda("fechaReserva"), out fechadtpFechaReserva)) dtpFechaReserva.Value = fechadtpFechaReserva; txtHoraReserva.Text = ValorCelda("horaReserva"); txtCantidadPersonas.Text = ValorCelda("cantidadPersonas"); SeleccionarTexto(cmbEstadoReserva, ValorCelda("estadoReserva")); txtObservacion.Text = ValorCelda("observacion");
            cmbCliente.Focus();
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
                valores["idCliente"] = ObtenerValorCombo(cmbCliente); valores["idMesa"] = ObtenerValorCombo(cmbMesa); valores["fechaReserva"] = dtpFechaReserva.Value.ToString("yyyy-MM-dd"); valores["horaReserva"] = txtHoraReserva.Text.Trim(); valores["cantidadPersonas"] = txtCantidadPersonas.Text.Trim(); valores["estadoReserva"] = Convert.ToString(cmbEstadoReserva.SelectedItem); valores["observacion"] = txtObservacion.Text.Trim();

                ProfesionalCln.Guardar("Reservas", esNuevo ? 0 : idActual, valores);
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
                ProfesionalCln.CambiarEstadoLogico("Reservas", id, estado);
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
            if (cmbCliente.Items.Count > 0) cmbCliente.SelectedIndex = 0; if (cmbMesa.Items.Count > 0) cmbMesa.SelectedIndex = 0; dtpFechaReserva.Value = DateTime.Today; txtHoraReserva.Clear(); txtCantidadPersonas.Clear(); if (cmbEstadoReserva.Items.Count > 0) cmbEstadoReserva.SelectedIndex = 0; txtObservacion.Clear();
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
            if (cmbCliente.SelectedValue == null) { erpPrincipal.SetError(cmbCliente, "Seleccione Cliente."); ok = false; } if (cmbMesa.SelectedValue == null) { erpPrincipal.SetError(cmbMesa, "Seleccione Mesa."); ok = false; } if (string.IsNullOrWhiteSpace(txtHoraReserva.Text)) { erpPrincipal.SetError(txtHoraReserva, "El campo Hora (HH:mm) es obligatorio."); ok = false; } if (string.IsNullOrWhiteSpace(txtCantidadPersonas.Text)) { erpPrincipal.SetError(txtCantidadPersonas, "El campo Cantidad Personas es obligatorio."); ok = false; } if (cmbEstadoReserva.SelectedItem == null) { erpPrincipal.SetError(cmbEstadoReserva, "Seleccione Estado Reserva."); ok = false; }
            int tmptxtCantidadPersonas; if (!string.IsNullOrWhiteSpace(txtCantidadPersonas.Text) && !int.TryParse(txtCantidadPersonas.Text, out tmptxtCantidadPersonas)) { erpPrincipal.SetError(txtCantidadPersonas, "Ingrese un número entero válido."); ok = false; }

            TimeSpan horaTemp; if (!TimeSpan.TryParse(txtHoraReserva.Text.Trim(), out horaTemp)) { erpPrincipal.SetError(txtHoraReserva, "Ingrese una hora válida. Ejemplo: 19:30"); ok = false; }
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
