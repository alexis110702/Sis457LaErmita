// ============================================================
// FrmCompra.cs
// Formulario WinForms independiente del módulo Compras.
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
    public partial class FrmCompra : Form
    {
        private bool esNuevo = false;
        private int idActual = 0;

        public FrmCompra()
        {
            InitializeComponent();
            Tema.Aplicar(this);
        }

        private void FrmCompra_Load(object sender, EventArgs e)
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
                CargarCombo(cmbProveedor, "Proveedores", "nombre");
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar datos iniciales. Ejecute el script profesional de base de datos.\n" + ex.Message,
                    "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            txtSubtotal.KeyPress += Util.OnlyDecimals; txtImpuesto.KeyPress += Util.OnlyDecimals; txtTotal.KeyPress += Util.OnlyDecimals;

        }

        private void Listar()
        {
            try
            {
                DataTable lista = ProfesionalCln.Listar("Compras", txtParametro.Text);
                dgvLista.DataSource = lista;

                if (dgvLista.Columns.Contains("id")) dgvLista.Columns["id"].Visible = false; if (dgvLista.Columns.Contains("idProveedor")) dgvLista.Columns["idProveedor"].Visible = false; if (dgvLista.Columns.Contains("idUsuario")) dgvLista.Columns["idUsuario"].Visible = false;
                if (dgvLista.Columns.Contains("proveedor")) dgvLista.Columns["proveedor"].HeaderText = "Proveedor"; if (dgvLista.Columns.Contains("usuario")) dgvLista.Columns["usuario"].HeaderText = "Usuario"; if (dgvLista.Columns.Contains("fecha")) dgvLista.Columns["fecha"].HeaderText = "Fecha"; if (dgvLista.Columns.Contains("numeroDocumento")) dgvLista.Columns["numeroDocumento"].HeaderText = "Documento"; if (dgvLista.Columns.Contains("subtotal")) dgvLista.Columns["subtotal"].HeaderText = "Subtotal"; if (dgvLista.Columns.Contains("impuesto")) dgvLista.Columns["impuesto"].HeaderText = "Impuesto"; if (dgvLista.Columns.Contains("total")) dgvLista.Columns["total"].HeaderText = "Total"; if (dgvLista.Columns.Contains("observacion")) dgvLista.Columns["observacion"].HeaderText = "Observación"; if (dgvLista.Columns.Contains("estado")) dgvLista.Columns["estado"].HeaderText = "Estado";

                ConfigurarGrilla(dgvLista);
                bool hayDatos = lista.Rows.Count > 0;
                btnEditar.Enabled = hayDatos;
                btnEliminar.Enabled = hayDatos;
                btnActivar.Enabled = hayDatos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al listar Compras: " + ex.Message,
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
            cmbProveedor.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvLista.CurrentRow == null) return;

            esNuevo = false;
            idActual = Convert.ToInt32(dgvLista.CurrentRow.Cells["id"].Value);
            pnlAcciones.Enabled = false;
            pnlFormulario.Visible = true;
            ResetearErrores();

            SeleccionarCombo(cmbProveedor, ValorCeldaObjeto("idProveedor")); txtNumeroDocumento.Text = ValorCelda("numeroDocumento"); txtSubtotal.Text = ValorCelda("subtotal"); txtImpuesto.Text = ValorCelda("impuesto"); txtTotal.Text = ValorCelda("total"); txtObservacion.Text = ValorCelda("observacion");
            cmbProveedor.Focus();
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
                valores["idProveedor"] = ObtenerValorCombo(cmbProveedor); valores["numeroDocumento"] = txtNumeroDocumento.Text.Trim(); valores["subtotal"] = txtSubtotal.Text.Trim(); valores["impuesto"] = txtImpuesto.Text.Trim(); valores["total"] = txtTotal.Text.Trim(); valores["observacion"] = txtObservacion.Text.Trim(); valores["idUsuario"] = (Sesion.IdUsuario > 0 ? Sesion.IdUsuario : 1).ToString();

                ProfesionalCln.Guardar("Compras", esNuevo ? 0 : idActual, valores);
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
                ProfesionalCln.CambiarEstadoLogico("Compras", id, estado);
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
            if (cmbProveedor.Items.Count > 0) cmbProveedor.SelectedIndex = 0; txtNumeroDocumento.Clear(); txtSubtotal.Clear(); txtImpuesto.Clear(); txtTotal.Clear(); txtObservacion.Clear();
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
            if (cmbProveedor.SelectedValue == null) { erpPrincipal.SetError(cmbProveedor, "Seleccione Proveedor."); ok = false; } if (string.IsNullOrWhiteSpace(txtSubtotal.Text)) { erpPrincipal.SetError(txtSubtotal, "El campo Subtotal es obligatorio."); ok = false; } if (string.IsNullOrWhiteSpace(txtImpuesto.Text)) { erpPrincipal.SetError(txtImpuesto, "El campo Impuesto es obligatorio."); ok = false; } if (string.IsNullOrWhiteSpace(txtTotal.Text)) { erpPrincipal.SetError(txtTotal, "El campo Total es obligatorio."); ok = false; }

            decimal dectxtSubtotal; if (!string.IsNullOrWhiteSpace(txtSubtotal.Text) && !decimal.TryParse(txtSubtotal.Text, out dectxtSubtotal)) { erpPrincipal.SetError(txtSubtotal, "Ingrese un importe válido. Ejemplo: 10.50"); ok = false; } decimal dectxtImpuesto; if (!string.IsNullOrWhiteSpace(txtImpuesto.Text) && !decimal.TryParse(txtImpuesto.Text, out dectxtImpuesto)) { erpPrincipal.SetError(txtImpuesto, "Ingrese un importe válido. Ejemplo: 10.50"); ok = false; } decimal dectxtTotal; if (!string.IsNullOrWhiteSpace(txtTotal.Text) && !decimal.TryParse(txtTotal.Text, out dectxtTotal)) { erpPrincipal.SetError(txtTotal, "Ingrese un importe válido. Ejemplo: 10.50"); ok = false; }

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
