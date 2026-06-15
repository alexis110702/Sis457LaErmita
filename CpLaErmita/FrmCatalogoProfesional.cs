using ClnLaErmita;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace CpLaErmita
{
    /// <summary>
    /// Formulario WinForms profesional para CRUD de módulos de catálogo.
    /// Mantiene la estructura Form + Designer.cs para que pueda abrirse desde el diseñador de Visual Studio.
    /// Los campos internos se generan dinámicamente según la configuración segura de ProfesionalCln.
    /// </summary>
    public partial class FrmCatalogoProfesional : Form
    {
        private readonly string _modulo;
        private readonly ModuloConfig _config;
        private int _idActual = 0;
        private readonly Dictionary<string, TextBox> _editores = new Dictionary<string, TextBox>();

        public FrmCatalogoProfesional() : this("Clientes")
        {
            // Constructor sin parámetros para compatibilidad con el diseñador de Visual Studio.
        }

        public FrmCatalogoProfesional(string modulo)
        {
            _modulo = modulo;
            _config = ProfesionalCln.ObtenerModulo(modulo);
            InitializeComponent();
            Tema.Aplicar(this);
            PrepararFormularioPorModulo();
            AsociarEventos();
        }

        private void PrepararFormularioPorModulo()
        {
            Text = _config.Titulo + " - La Ermita";
            lblTitulo.Text = IconoModulo(_modulo) + "  " + _config.Titulo;
            pnlFormulario.Visible = !_config.SoloLectura;
            ConstruirCamposDinamicos();
            LimpiarFormulario();
        }

        private void AsociarEventos()
        {
            Load += FrmCatalogoProfesional_Load;
            txtBuscar.KeyPress += TxtBuscar_KeyPress;
            btnBuscar.Click += BtnBuscar_Click;
            btnNuevo.Click += BtnNuevo_Click;
            btnEditar.Click += BtnEditar_Click;
            btnEliminar.Click += BtnEliminar_Click;
            btnActivar.Click += BtnActivar_Click;
            btnGuardar.Click += BtnGuardar_Click;
            btnCancelar.Click += BtnCancelar_Click;
            btnCerrar.Click += BtnCerrar_Click;
        }

        private void ConstruirCamposDinamicos()
        {
            pnlCampos.Controls.Clear();
            _editores.Clear();

            if (_config.SoloLectura) return;

            int y = 8;
            foreach (var campo in _config.Campos)
            {
                Label etiqueta = new Label();
                etiqueta.Text = campo.Etiqueta + (campo.Obligatorio ? " *" : "");
                etiqueta.Left = 8;
                etiqueta.Top = y;
                etiqueta.Width = pnlCampos.Width - 24;
                etiqueta.Height = 20;
                etiqueta.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                pnlCampos.Controls.Add(etiqueta);

                y += 22;

                TextBox txt = new TextBox();
                txt.Name = "txt" + campo.Nombre;
                txt.Left = 8;
                txt.Top = y;
                txt.Width = pnlCampos.Width - 24;
                txt.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                txt.UseSystemPasswordChar = campo.EsPassword;
                if (campo.EsPassword)
                    txt.PlaceholderTextCompat("Dejar vacío para no cambiar");

                _editores[campo.Nombre] = txt;
                pnlCampos.Controls.Add(txt);

                y += 34;
            }
        }

        private void FrmCatalogoProfesional_Load(object sender, EventArgs e)
        {
            Listar();
        }

        private void TxtBuscar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                Listar();
            }
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            Listar();
        }

        private void BtnNuevo_Click(object sender, EventArgs e)
        {
            Nuevo();
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            Editar();
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            CambiarEstado(0);
        }

        private void BtnActivar_Click(object sender, EventArgs e)
        {
            CambiarEstado(1);
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            Guardar();
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void BtnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Listar()
        {
            try
            {
                DataTable dt = ProfesionalCln.Listar(_modulo, txtBuscar.Text);
                dgvLista.DataSource = dt;

                if (dgvLista.Columns.Contains("id"))
                    dgvLista.Columns["id"].Visible = false;

                dgvLista.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvLista.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders);

                bool hayDatos = dt.Rows.Count > 0;
                btnEditar.Enabled = !_config.SoloLectura && hayDatos;
                btnEliminar.Enabled = !_config.SoloLectura && hayDatos;
                btnActivar.Enabled = !_config.SoloLectura && hayDatos;
                btnNuevo.Enabled = !_config.SoloLectura;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo listar el módulo. Verifique si ejecutó el script profesional.\n" + ex.Message,
                    "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Nuevo()
        {
            _idActual = 0;
            foreach (TextBox txt in _editores.Values)
                txt.Clear();

            pnlFormulario.Enabled = true;
            if (_editores.Count > 0)
                _editores.First().Value.Focus();
        }

        private void Editar()
        {
            if (dgvLista.CurrentRow == null) return;
            _idActual = Convert.ToInt32(dgvLista.CurrentRow.Cells["id"].Value);

            foreach (var campo in _config.Campos)
            {
                if (!_editores.ContainsKey(campo.Nombre)) continue;

                if (campo.EsPassword)
                {
                    _editores[campo.Nombre].Clear();
                    continue;
                }

                if (dgvLista.Columns.Contains(campo.Nombre))
                    _editores[campo.Nombre].Text = Convert.ToString(dgvLista.CurrentRow.Cells[campo.Nombre].Value);
            }
        }

        private void Guardar()
        {
            try
            {
                Dictionary<string, string> valores = _editores.ToDictionary(x => x.Key, x => x.Value.Text);
                ProfesionalCln.Guardar(_modulo, _idActual, valores);
                LimpiarFormulario();
                Listar();
                MessageBox.Show("Registro guardado correctamente.", "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message, "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CambiarEstado(short estado)
        {
            if (dgvLista.CurrentRow == null) return;

            int id = Convert.ToInt32(dgvLista.CurrentRow.Cells["id"].Value);
            string texto = estado == 1 ? "activar" : "desactivar";

            if (MessageBox.Show("¿Desea " + texto + " este registro?", "::: La Ermita :::",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                ProfesionalCln.CambiarEstadoLogico(_modulo, id, estado);
                Listar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cambiar estado: " + ex.Message, "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarFormulario()
        {
            _idActual = 0;
            foreach (TextBox txt in _editores.Values)
                txt.Clear();
        }

        private string IconoModulo(string modulo)
        {
            switch (modulo)
            {
                case "Usuarios": return "👤";
                case "Roles": return "🛡️";
                case "Clientes": return "🤝";
                case "Proveedores": return "🚚";
                case "Mesas": return "🍽️";
                case "MetodosPago": return "💳";
                case "Reservas": return "📅";
                case "Compras": return "🛒";
                case "Inventario": return "📦";
                default: return "📋";
            }
        }
    }

    internal static class TextBoxPlaceholderCompat
    {
        public static void PlaceholderTextCompat(this TextBox txt, string placeholder)
        {
            // .NET Framework WinForms no tiene PlaceholderText nativo.
            // Se guarda en Tag para mantener compatibilidad sin romper compilación.
            txt.Tag = placeholder;
        }
    }
}
