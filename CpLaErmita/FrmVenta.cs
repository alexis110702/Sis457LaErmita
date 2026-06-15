// ============================================================
// FrmVenta.cs  –  Capa de Presentación (CP)
// CORRECCIONES:
//   1. ERROR DE RENDIMIENTO CRÍTICO: en actualizarGrilla se llamaba
//      "ProductoCln.listar()" en cada iteración del Select (N consultas
//      a la BD por cada fila del detalle). Se corrigió cargando la lista
//      UNA SOLA VEZ antes del Select.
//   2. Llamadas a CLN actualizadas a PascalCase.
//   3. VentaCln.Crear y VentaCln.Anular ahora lanzan excepciones con
//      mensajes claros (stock insuficiente, etc.). Se capturan aquí.
//   4. MEJORA: se deshabilita btnGuardarVenta durante el proceso para
//      evitar doble clic accidental.
// ============================================================
using CadLaErmita;
using ClnLaErmita;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CpLaErmita
{
    public partial class FrmVenta : Form
    {
        private List<DetalleVenta> detalles = new List<DetalleVenta>();
        private List<Producto>     _productos;   // cache local de productos
        private decimal total = 0;

        public FrmVenta() { InitializeComponent(); }

        private void FrmVenta_Load(object sender, EventArgs e)
        {
            CargarProductos();
            ListarHistorial();
            ActualizarTotal();
            ConfigurarGrilla(dgvDetalle);
            ConfigurarGrilla(dgvHistorial);
            ConfigurarGrilla(dgvDetalleHistorial);
        }

        private void CargarProductos()
        {
            _productos                = ProductoCln.Listar();
            cbxProducto.DataSource    = _productos;
            cbxProducto.ValueMember   = "id";
            cbxProducto.DisplayMember = "nombre";
            cbxProducto.SelectedIndex = -1;
        }

        private void cbxProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxProducto.SelectedIndex == -1) { lblPrecioUnitario.Text = "Seleccione un producto"; return; }
            var prod = (Producto)cbxProducto.SelectedItem;
            lblPrecioUnitario.Text = $"Código: {prod.id} | {prod.nombre} | Cat. {prod.idCategoria} | Bs. {prod.precio:F2} | Stock: {prod.stock} | {prod.descripcion}";
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (cbxProducto.SelectedIndex == -1)
            { MessageBox.Show("Seleccione un producto.", "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (nudCantidad.Value <= 0)
            { MessageBox.Show("La cantidad debe ser mayor a cero.", "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            var prod = (Producto)cbxProducto.SelectedItem;

            // Validar stock considerando lo que ya está en el detalle
            int yaEnDetalle = detalles.Where(d => d.idProducto == prod.id).Sum(d => d.cantidad);
            if (yaEnDetalle + (int)nudCantidad.Value > prod.stock)
            {
                MessageBox.Show($"Stock insuficiente para \"{prod.nombre}\".\nDisponible: {prod.stock}  |  Ya en pedido: {yaEnDetalle}",
                    "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var existente = detalles.FirstOrDefault(d => d.idProducto == prod.id);
            if (existente != null)
                existente.cantidad += (int)nudCantidad.Value;
            else
                detalles.Add(new DetalleVenta
                {
                    idProducto = prod.id,
                    cantidad   = (int)nudCantidad.Value,
                    precio     = prod.precio,
                    subtotal   = prod.precio * (int)nudCantidad.Value
                });

            // Recalcular subtotales
            foreach (var d in detalles) d.subtotal = d.cantidad * d.precio;

            ActualizarGrilla();
            cbxProducto.SelectedIndex = -1;
            nudCantidad.Value = 1;
        }

        private void btnQuitarProducto_Click(object sender, EventArgs e)
        {
            if (dgvDetalle.CurrentRow == null) return;
            int idProducto = (int)dgvDetalle.CurrentRow.Cells["idProducto"].Value;
            detalles.RemoveAll(d => d.idProducto == idProducto);
            ActualizarGrilla();
        }

        private void ActualizarGrilla()
        {
            // CORRECCIÓN: se usa _productos (ya cargado) en lugar de
            // llamar ProductoCln.Listar() dentro del Select (evita N consultas a BD)
            var lista = detalles.Select(d => new
            {
                d.idProducto,
                producto = _productos.FirstOrDefault(p => p.id == d.idProducto)?.nombre ?? "?",
                d.cantidad,
                precio   = d.precio.ToString("F2"),
                subtotal = d.subtotal.ToString("F2")
            }).ToList();

            dgvDetalle.DataSource = lista;
            ConfigurarGrilla(dgvDetalle);
            if (dgvDetalle.Columns.Contains("idProducto"))
                dgvDetalle.Columns["idProducto"].Visible = false;

            ActualizarTotal();
        }

        private void ActualizarTotal()
        {
            total          = detalles.Sum(d => d.subtotal);
            lblTotal.Text  = $"TOTAL:  Bs. {total:F2}";
        }

        private void btnGuardarVenta_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMesa.Text))
            { MessageBox.Show("Ingrese el número de mesa.", "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (detalles.Count == 0)
            { MessageBox.Show("Agregue al menos un producto.", "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            // MEJORA: deshabilitar para evitar doble registro
            btnGuardarVenta.Enabled = false;

            try
            {
                var venta = new Venta
                {
                    idUsuario = Sesion.IdUsuario,
                    mesa      = txtMesa.Text.Trim()
                };

                int idVenta = VentaCln.Crear(venta, detalles);
                MessageBox.Show($"Venta #{idVenta} registrada correctamente.\nTotal: Bs. {total:F2}",
                    "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Information);

                detalles.Clear();
                txtMesa.Clear();
                CargarProductos();   // Refresca stock actualizado
                ActualizarGrilla();
                ListarHistorial();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar la venta:\n" + ex.Message,
                    "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnGuardarVenta.Enabled = true;
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            detalles.Clear();
            ActualizarGrilla();
            txtMesa.Clear();
            cbxProducto.SelectedIndex = -1;
            nudCantidad.Value = 1;
        }

        private void ListarHistorial()
        {
            var lista = VentaCln.ListarPa(txtBuscarVenta.Text);
            dgvHistorial.DataSource = lista;
            if (dgvHistorial.Columns.Count == 0) return;
            dgvHistorial.Columns["id"].Visible        = false;
            dgvHistorial.Columns["idUsuario"].Visible = false;
            dgvHistorial.Columns["estado"].Visible    = false;
            dgvHistorial.Columns["mesa"].HeaderText   = "Mesa";
            dgvHistorial.Columns["cajero"].HeaderText = "Cajero";
            dgvHistorial.Columns["fecha"].HeaderText  = "Fecha";
            dgvHistorial.Columns["total"].HeaderText  = "Total (Bs.)";
            ConfigurarGrilla(dgvHistorial);
        }

        private void btnBuscarVenta_Click(object sender, EventArgs e) => ListarHistorial();

        private void btnVerDetalle_Click(object sender, EventArgs e)
        {
            if (dgvHistorial.CurrentRow == null) return;
            int idVenta = (int)dgvHistorial.CurrentRow.Cells["id"].Value;
            var det = VentaCln.ListarDetalle(idVenta);
            dgvDetalleHistorial.DataSource = det;
            if (dgvDetalleHistorial.Columns.Count == 0) return;
            dgvDetalleHistorial.Columns["id"].Visible         = false;
            dgvDetalleHistorial.Columns["idVenta"].Visible    = false;
            dgvDetalleHistorial.Columns["idProducto"].Visible = false;
            dgvDetalleHistorial.Columns["estado"].Visible     = false;
            dgvDetalleHistorial.Columns["producto"].HeaderText  = "Producto";
            dgvDetalleHistorial.Columns["cantidad"].HeaderText  = "Cantidad";
            dgvDetalleHistorial.Columns["precio"].HeaderText    = "Precio";
            dgvDetalleHistorial.Columns["subtotal"].HeaderText  = "Subtotal";
            ConfigurarGrilla(dgvDetalleHistorial);
        }

        private void btnAnular_Click(object sender, EventArgs e)
        {
            if (dgvHistorial.CurrentRow == null) return;
            int idVenta = (int)dgvHistorial.CurrentRow.Cells["id"].Value;

            if (MessageBox.Show($"¿Anular la venta #{idVenta}?",
                "::: La Ermita :::", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                != DialogResult.Yes) return;

            try
            {
                var (ok, mensaje) = VentaCln.Anular(idVenta);
                if (ok)
                {
                    ListarHistorial();
                    dgvDetalleHistorial.DataSource = null;
                    CargarProductos(); // Refresca stock restaurado
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
                MessageBox.Show("Error al anular: " + ex.Message,
                    "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        }

        private void btnCerrar_Click(object sender, EventArgs e) => Close();
    }
}
