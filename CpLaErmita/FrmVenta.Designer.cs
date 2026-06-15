namespace CpLaErmita
{
    partial class FrmVenta
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        { if (disposing && components != null) components.Dispose(); base.Dispose(disposing); }

        private void InitializeComponent()
        {
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabNuevaVenta = new System.Windows.Forms.TabPage();
            this.lblMesa = new System.Windows.Forms.Label();
            this.txtMesa = new System.Windows.Forms.TextBox();
            this.lblProducto = new System.Windows.Forms.Label();
            this.cbxProducto = new System.Windows.Forms.ComboBox();
            this.lblCantidad = new System.Windows.Forms.Label();
            this.nudCantidad = new System.Windows.Forms.NumericUpDown();
            this.lblPrecioUnitario = new System.Windows.Forms.Label();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.btnQuitarProducto = new System.Windows.Forms.Button();
            this.dgvDetalle = new System.Windows.Forms.DataGridView();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnGuardarVenta = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.tabHistorial = new System.Windows.Forms.TabPage();
            this.lblBuscarVenta = new System.Windows.Forms.Label();
            this.txtBuscarVenta = new System.Windows.Forms.TextBox();
            this.btnBuscarVenta = new System.Windows.Forms.Button();
            this.dgvHistorial = new System.Windows.Forms.DataGridView();
            this.dgvDetalleHistorial = new System.Windows.Forms.DataGridView();
            this.btnVerDetalle = new System.Windows.Forms.Button();
            this.btnAnular = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabNuevaVenta.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).BeginInit();
            this.tabHistorial.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorial)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalleHistorial)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabNuevaVenta);
            this.tabControl.Controls.Add(this.tabHistorial);
            this.tabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl.Location = new System.Drawing.Point(10, 10);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(960, 580);
            this.tabControl.TabIndex = 0;
            // 
            // tabNuevaVenta
            // 
            this.tabNuevaVenta.BackgroundImage = global::CpLaErmita.Properties.Resources.images;
            this.tabNuevaVenta.Controls.Add(this.lblMesa);
            this.tabNuevaVenta.Controls.Add(this.txtMesa);
            this.tabNuevaVenta.Controls.Add(this.lblProducto);
            this.tabNuevaVenta.Controls.Add(this.cbxProducto);
            this.tabNuevaVenta.Controls.Add(this.lblCantidad);
            this.tabNuevaVenta.Controls.Add(this.nudCantidad);
            this.tabNuevaVenta.Controls.Add(this.lblPrecioUnitario);
            this.tabNuevaVenta.Controls.Add(this.btnAgregar);
            this.tabNuevaVenta.Controls.Add(this.btnQuitarProducto);
            this.tabNuevaVenta.Controls.Add(this.dgvDetalle);
            this.tabNuevaVenta.Controls.Add(this.lblTotal);
            this.tabNuevaVenta.Controls.Add(this.btnGuardarVenta);
            this.tabNuevaVenta.Controls.Add(this.btnLimpiar);
            this.tabNuevaVenta.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabNuevaVenta.Location = new System.Drawing.Point(4, 25);
            this.tabNuevaVenta.Name = "tabNuevaVenta";
            this.tabNuevaVenta.Size = new System.Drawing.Size(952, 551);
            this.tabNuevaVenta.TabIndex = 0;
            this.tabNuevaVenta.Text = "Nueva Venta";
            // 
            // lblMesa
            // 
            this.lblMesa.AutoSize = true;
            this.lblMesa.Location = new System.Drawing.Point(10, 15);
            this.lblMesa.Name = "lblMesa";
            this.lblMesa.Size = new System.Drawing.Size(49, 16);
            this.lblMesa.TabIndex = 0;
            this.lblMesa.Text = "Mesa:";
            // 
            // txtMesa
            // 
            this.txtMesa.Location = new System.Drawing.Point(70, 12);
            this.txtMesa.Name = "txtMesa";
            this.txtMesa.Size = new System.Drawing.Size(100, 22);
            this.txtMesa.TabIndex = 1;
            // 
            // lblProducto
            // 
            this.lblProducto.AutoSize = true;
            this.lblProducto.Location = new System.Drawing.Point(10, 50);
            this.lblProducto.Name = "lblProducto";
            this.lblProducto.Size = new System.Drawing.Size(73, 16);
            this.lblProducto.TabIndex = 2;
            this.lblProducto.Text = "Producto:";
            // 
            // cbxProducto
            // 
            this.cbxProducto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxProducto.Location = new System.Drawing.Point(90, 47);
            this.cbxProducto.Name = "cbxProducto";
            this.cbxProducto.Size = new System.Drawing.Size(300, 24);
            this.cbxProducto.TabIndex = 3;
            this.cbxProducto.SelectedIndexChanged += new System.EventHandler(this.cbxProducto_SelectedIndexChanged);
            // 
            // lblCantidad
            // 
            this.lblCantidad.AutoSize = true;
            this.lblCantidad.Location = new System.Drawing.Point(400, 50);
            this.lblCantidad.Name = "lblCantidad";
            this.lblCantidad.Size = new System.Drawing.Size(73, 16);
            this.lblCantidad.TabIndex = 4;
            this.lblCantidad.Text = "Cantidad:";
            // 
            // nudCantidad
            // 
            this.nudCantidad.Location = new System.Drawing.Point(470, 47);
            this.nudCantidad.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nudCantidad.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudCantidad.Name = "nudCantidad";
            this.nudCantidad.Size = new System.Drawing.Size(70, 22);
            this.nudCantidad.TabIndex = 5;
            this.nudCantidad.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblPrecioUnitario
            // 
            this.lblPrecioUnitario.AutoSize = true;
            this.lblPrecioUnitario.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblPrecioUnitario.Location = new System.Drawing.Point(550, 50);
            this.lblPrecioUnitario.Name = "lblPrecioUnitario";
            this.lblPrecioUnitario.Size = new System.Drawing.Size(73, 23);
            this.lblPrecioUnitario.TabIndex = 6;
            this.lblPrecioUnitario.Text = "Bs. 0.00";
            // 
            // btnAgregar
            // 
            this.btnAgregar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.btnAgregar.ForeColor = System.Drawing.Color.White;
            this.btnAgregar.Location = new System.Drawing.Point(700, 45);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(100, 30);
            this.btnAgregar.TabIndex = 7;
            this.btnAgregar.Text = "Agregar";
            this.btnAgregar.UseVisualStyleBackColor = false;
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            // 
            // btnQuitarProducto
            // 
            this.btnQuitarProducto.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
            this.btnQuitarProducto.ForeColor = System.Drawing.Color.White;
            this.btnQuitarProducto.Location = new System.Drawing.Point(810, 45);
            this.btnQuitarProducto.Name = "btnQuitarProducto";
            this.btnQuitarProducto.Size = new System.Drawing.Size(120, 30);
            this.btnQuitarProducto.TabIndex = 8;
            this.btnQuitarProducto.Text = "Quitar Item";
            this.btnQuitarProducto.UseVisualStyleBackColor = false;
            this.btnQuitarProducto.Click += new System.EventHandler(this.btnQuitarProducto_Click);
            // 
            // dgvDetalle
            // 
            this.dgvDetalle.AllowUserToAddRows = false;
            this.dgvDetalle.AllowUserToDeleteRows = false;
            this.dgvDetalle.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(230)))), ((int)(((byte)(211)))));
            this.dgvDetalle.ColumnHeadersHeight = 29;
            this.dgvDetalle.Location = new System.Drawing.Point(10, 85);
            this.dgvDetalle.Name = "dgvDetalle";
            this.dgvDetalle.ReadOnly = true;
            this.dgvDetalle.RowHeadersWidth = 51;
            this.dgvDetalle.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDetalle.Size = new System.Drawing.Size(920, 300);
            this.dgvDetalle.TabIndex = 9;
            // 
            // lblTotal
            // 
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTotal.Location = new System.Drawing.Point(10, 395);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(400, 35);
            this.lblTotal.TabIndex = 10;
            this.lblTotal.Text = "TOTAL:  Bs. 0.00";
            // 
            // btnGuardarVenta
            // 
            this.btnGuardarVenta.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.btnGuardarVenta.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnGuardarVenta.ForeColor = System.Drawing.Color.White;
            this.btnGuardarVenta.Location = new System.Drawing.Point(720, 395);
            this.btnGuardarVenta.Name = "btnGuardarVenta";
            this.btnGuardarVenta.Size = new System.Drawing.Size(120, 35);
            this.btnGuardarVenta.TabIndex = 11;
            this.btnGuardarVenta.Text = "Registrar Venta";
            this.btnGuardarVenta.UseVisualStyleBackColor = false;
            this.btnGuardarVenta.Click += new System.EventHandler(this.btnGuardarVenta_Click);
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.Location = new System.Drawing.Point(850, 395);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(80, 35);
            this.btnLimpiar.TabIndex = 12;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            // 
            // tabHistorial
            // 
            this.tabHistorial.Controls.Add(this.lblBuscarVenta);
            this.tabHistorial.Controls.Add(this.txtBuscarVenta);
            this.tabHistorial.Controls.Add(this.btnBuscarVenta);
            this.tabHistorial.Controls.Add(this.dgvHistorial);
            this.tabHistorial.Controls.Add(this.dgvDetalleHistorial);
            this.tabHistorial.Controls.Add(this.btnVerDetalle);
            this.tabHistorial.Controls.Add(this.btnAnular);
            this.tabHistorial.Controls.Add(this.btnCerrar);
            this.tabHistorial.Location = new System.Drawing.Point(4, 25);
            this.tabHistorial.Name = "tabHistorial";
            this.tabHistorial.Size = new System.Drawing.Size(952, 551);
            this.tabHistorial.TabIndex = 1;
            this.tabHistorial.Text = "Historial de Ventas";
            // 
            // lblBuscarVenta
            // 
            this.lblBuscarVenta.AutoSize = true;
            this.lblBuscarVenta.Location = new System.Drawing.Point(10, 15);
            this.lblBuscarVenta.Name = "lblBuscarVenta";
            this.lblBuscarVenta.Size = new System.Drawing.Size(59, 16);
            this.lblBuscarVenta.TabIndex = 0;
            this.lblBuscarVenta.Text = "Buscar:";
            // 
            // txtBuscarVenta
            // 
            this.txtBuscarVenta.Location = new System.Drawing.Point(70, 12);
            this.txtBuscarVenta.Name = "txtBuscarVenta";
            this.txtBuscarVenta.Size = new System.Drawing.Size(250, 22);
            this.txtBuscarVenta.TabIndex = 1;
            // 
            // btnBuscarVenta
            // 
            this.btnBuscarVenta.Location = new System.Drawing.Point(330, 11);
            this.btnBuscarVenta.Name = "btnBuscarVenta";
            this.btnBuscarVenta.Size = new System.Drawing.Size(80, 25);
            this.btnBuscarVenta.TabIndex = 2;
            this.btnBuscarVenta.Text = "Buscar";
            this.btnBuscarVenta.Click += new System.EventHandler(this.btnBuscarVenta_Click);
            // 
            // dgvHistorial
            // 
            this.dgvHistorial.AllowUserToAddRows = false;
            this.dgvHistorial.AllowUserToDeleteRows = false;
            this.dgvHistorial.ColumnHeadersHeight = 29;
            this.dgvHistorial.Location = new System.Drawing.Point(10, 45);
            this.dgvHistorial.Name = "dgvHistorial";
            this.dgvHistorial.ReadOnly = true;
            this.dgvHistorial.RowHeadersWidth = 51;
            this.dgvHistorial.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistorial.Size = new System.Drawing.Size(920, 200);
            this.dgvHistorial.TabIndex = 3;
            // 
            // dgvDetalleHistorial
            // 
            this.dgvDetalleHistorial.AllowUserToAddRows = false;
            this.dgvDetalleHistorial.AllowUserToDeleteRows = false;
            this.dgvDetalleHistorial.ColumnHeadersHeight = 29;
            this.dgvDetalleHistorial.Location = new System.Drawing.Point(10, 295);
            this.dgvDetalleHistorial.Name = "dgvDetalleHistorial";
            this.dgvDetalleHistorial.ReadOnly = true;
            this.dgvDetalleHistorial.RowHeadersWidth = 51;
            this.dgvDetalleHistorial.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDetalleHistorial.Size = new System.Drawing.Size(920, 230);
            this.dgvDetalleHistorial.TabIndex = 4;
            // 
            // btnVerDetalle
            // 
            this.btnVerDetalle.Location = new System.Drawing.Point(10, 255);
            this.btnVerDetalle.Name = "btnVerDetalle";
            this.btnVerDetalle.Size = new System.Drawing.Size(120, 30);
            this.btnVerDetalle.TabIndex = 5;
            this.btnVerDetalle.Text = "Ver Detalle";
            this.btnVerDetalle.Click += new System.EventHandler(this.btnVerDetalle_Click);
            // 
            // btnAnular
            // 
            this.btnAnular.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
            this.btnAnular.ForeColor = System.Drawing.Color.White;
            this.btnAnular.Location = new System.Drawing.Point(140, 255);
            this.btnAnular.Name = "btnAnular";
            this.btnAnular.Size = new System.Drawing.Size(90, 30);
            this.btnAnular.TabIndex = 6;
            this.btnAnular.Text = "Anular";
            this.btnAnular.UseVisualStyleBackColor = false;
            this.btnAnular.Click += new System.EventHandler(this.btnAnular_Click);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(830, 255);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(90, 30);
            this.btnCerrar.TabIndex = 7;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // FrmVenta
            // 
            this.BackgroundImage = global::CpLaErmita.Properties.Resources.images;
            this.ClientSize = new System.Drawing.Size(984, 620);
            this.Controls.Add(this.tabControl);
            this.Name = "FrmVenta";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ventas - La Ermita";
            this.Load += new System.EventHandler(this.FrmVenta_Load);
            this.tabControl.ResumeLayout(false);
            this.tabNuevaVenta.ResumeLayout(false);
            this.tabNuevaVenta.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).EndInit();
            this.tabHistorial.ResumeLayout(false);
            this.tabHistorial.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorial)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalleHistorial)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TabControl   tabControl;
        private System.Windows.Forms.TabPage      tabNuevaVenta, tabHistorial;
        private System.Windows.Forms.Label        lblMesa, lblProducto, lblCantidad, lblPrecioUnitario, lblTotal, lblBuscarVenta;
        private System.Windows.Forms.TextBox      txtMesa, txtBuscarVenta;
        private System.Windows.Forms.ComboBox     cbxProducto;
        private System.Windows.Forms.NumericUpDown nudCantidad;
        private System.Windows.Forms.Button       btnAgregar, btnQuitarProducto, btnGuardarVenta, btnLimpiar, btnBuscarVenta, btnVerDetalle, btnAnular, btnCerrar;
        private System.Windows.Forms.DataGridView dgvDetalle, dgvHistorial, dgvDetalleHistorial;
    }
}
