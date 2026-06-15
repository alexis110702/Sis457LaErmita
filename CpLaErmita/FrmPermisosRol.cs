using ClnLaErmita;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace CpLaErmita
{
    public partial class FrmPermisosRol : Form
    {
        private DataTable _permisos;

        public FrmPermisosRol()
        {
            InitializeComponent();
            Tema.Aplicar(this);
            AsociarEventos();
        }

        private void AsociarEventos()
        {
            Load += FrmPermisosRol_Load;
            cbxRol.SelectedIndexChanged += CbxRol_SelectedIndexChanged;
            btnGuardar.Click += BtnGuardar_Click;
            btnCerrar.Click += BtnCerrar_Click;
        }

        private void FrmPermisosRol_Load(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void CbxRol_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarPermisosRol();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            Guardar();
        }

        private void BtnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CargarDatos()
        {
            try
            {
                cbxRol.DataSource = PermisosCln.ListarRoles();
                cbxRol.ValueMember = "id";
                cbxRol.DisplayMember = "nombre";

                _permisos = PermisosCln.ListarPermisos();
                chkPermisos.Items.Clear();

                foreach (DataRow row in _permisos.Rows)
                    chkPermisos.Items.Add(new PermisoItem((int)row["id"], row["permiso"].ToString()));

                CargarPermisosRol();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar permisos. Ejecute el script profesional.\n" + ex.Message,
                    "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarPermisosRol()
        {
            if (cbxRol.SelectedValue == null || cbxRol.SelectedValue is DataRowView) return;

            int idRol = Convert.ToInt32(cbxRol.SelectedValue);
            HashSet<int> asignados = PermisosCln.ObtenerPermisosRol(idRol);

            for (int i = 0; i < chkPermisos.Items.Count; i++)
            {
                PermisoItem item = (PermisoItem)chkPermisos.Items[i];
                chkPermisos.SetItemChecked(i, asignados.Contains(item.Id));
            }
        }

        private void Guardar()
        {
            if (cbxRol.SelectedValue == null) return;

            int idRol = Convert.ToInt32(cbxRol.SelectedValue);
            List<int> ids = new List<int>();

            foreach (object obj in chkPermisos.CheckedItems)
                ids.Add(((PermisoItem)obj).Id);

            try
            {
                PermisosCln.GuardarPermisosRol(idRol, ids);
                MessageBox.Show("Permisos actualizados correctamente.", "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar permisos: " + ex.Message, "::: La Ermita :::", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private class PermisoItem
        {
            public int Id { get; private set; }
            public string Texto { get; private set; }

            public PermisoItem(int id, string texto)
            {
                Id = id;
                Texto = texto;
            }

            public override string ToString()
            {
                return Texto;
            }
        }
    }
}
