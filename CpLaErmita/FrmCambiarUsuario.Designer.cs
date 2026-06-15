namespace CpLaErmita
{
    partial class FrmCambiarUsuario
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblActual = new System.Windows.Forms.Label();
            this.lblInstruccion = new System.Windows.Forms.Label();
            this.lblUsuarioRapido = new System.Windows.Forms.Label();
            this.cmbUsuarioRapido = new System.Windows.Forms.ComboBox();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.lblContrasena = new System.Windows.Forms.Label();
            this.txtContrasena = new System.Windows.Forms.TextBox();
            this.btnCambiar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.panelContenido = new System.Windows.Forms.Panel();
            this.panelContenido.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.Location = new System.Drawing.Point(0, 18);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(620, 45);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "🔄 Cambiar Usuario";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblActual
            // 
            this.lblActual.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblActual.Location = new System.Drawing.Point(25, 15);
            this.lblActual.Name = "lblActual";
            this.lblActual.Size = new System.Drawing.Size(510, 28);
            this.lblActual.TabIndex = 1;
            this.lblActual.Text = "Usuario actual:";
            this.lblActual.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblInstruccion
            // 
            this.lblInstruccion.Location = new System.Drawing.Point(25, 50);
            this.lblInstruccion.Name = "lblInstruccion";
            this.lblInstruccion.Size = new System.Drawing.Size(510, 55);
            this.lblInstruccion.TabIndex = 2;
            this.lblInstruccion.Text = "Para cambiar a Cajero, Mesero u otro usuario, ingrese el usuario destino y su contraseña. El sistema actualizará los permisos automáticamente.";
            // 
            // lblUsuarioRapido
            // 
            this.lblUsuarioRapido.AutoSize = true;
            this.lblUsuarioRapido.Location = new System.Drawing.Point(25, 123);
            this.lblUsuarioRapido.Name = "lblUsuarioRapido";
            this.lblUsuarioRapido.Size = new System.Drawing.Size(129, 20);
            this.lblUsuarioRapido.TabIndex = 3;
            this.lblUsuarioRapido.Text = "Usuario rápido:";
            // 
            // cmbUsuarioRapido
            // 
            this.cmbUsuarioRapido.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUsuarioRapido.FormattingEnabled = true;
            this.cmbUsuarioRapido.Location = new System.Drawing.Point(185, 120);
            this.cmbUsuarioRapido.Name = "cmbUsuarioRapido";
            this.cmbUsuarioRapido.Size = new System.Drawing.Size(320, 28);
            this.cmbUsuarioRapido.TabIndex = 4;
            this.cmbUsuarioRapido.SelectedIndexChanged += new System.EventHandler(this.cmbUsuarioRapido_SelectedIndexChanged);
            // 
            // lblUsuario
            // 
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Location = new System.Drawing.Point(25, 170);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(70, 20);
            this.lblUsuario.TabIndex = 5;
            this.lblUsuario.Text = "Usuario:";
            // 
            // txtUsuario
            // 
            this.txtUsuario.Location = new System.Drawing.Point(185, 167);
            this.txtUsuario.MaxLength = 50;
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(320, 27);
            this.txtUsuario.TabIndex = 6;
            this.txtUsuario.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtUsuario_KeyPress);
            // 
            // lblContrasena
            // 
            this.lblContrasena.AutoSize = true;
            this.lblContrasena.Location = new System.Drawing.Point(25, 216);
            this.lblContrasena.Name = "lblContrasena";
            this.lblContrasena.Size = new System.Drawing.Size(141, 20);
            this.lblContrasena.TabIndex = 7;
            this.lblContrasena.Text = "Contraseña nueva:";
            // 
            // txtContrasena
            // 
            this.txtContrasena.Location = new System.Drawing.Point(185, 213);
            this.txtContrasena.MaxLength = 100;
            this.txtContrasena.Name = "txtContrasena";
            this.txtContrasena.PasswordChar = '●';
            this.txtContrasena.Size = new System.Drawing.Size(320, 27);
            this.txtContrasena.TabIndex = 8;
            this.txtContrasena.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtContrasena_KeyPress);
            // 
            // btnCambiar
            // 
            this.btnCambiar.Location = new System.Drawing.Point(185, 270);
            this.btnCambiar.Name = "btnCambiar";
            this.btnCambiar.Size = new System.Drawing.Size(155, 45);
            this.btnCambiar.TabIndex = 9;
            this.btnCambiar.Text = "Cambiar usuario";
            this.btnCambiar.UseVisualStyleBackColor = true;
            this.btnCambiar.Click += new System.EventHandler(this.btnCambiar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Location = new System.Drawing.Point(350, 270);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(155, 45);
            this.btnCancelar.TabIndex = 10;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // panelContenido
            // 
            this.panelContenido.Controls.Add(this.lblActual);
            this.panelContenido.Controls.Add(this.lblInstruccion);
            this.panelContenido.Controls.Add(this.lblUsuarioRapido);
            this.panelContenido.Controls.Add(this.cmbUsuarioRapido);
            this.panelContenido.Controls.Add(this.lblUsuario);
            this.panelContenido.Controls.Add(this.txtUsuario);
            this.panelContenido.Controls.Add(this.lblContrasena);
            this.panelContenido.Controls.Add(this.txtContrasena);
            this.panelContenido.Controls.Add(this.btnCambiar);
            this.panelContenido.Controls.Add(this.btnCancelar);
            this.panelContenido.Location = new System.Drawing.Point(30, 80);
            this.panelContenido.Name = "panelContenido";
            this.panelContenido.Size = new System.Drawing.Size(560, 350);
            this.panelContenido.TabIndex = 11;
            // 
            // FrmCambiarUsuario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 465);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.panelContenido);
            this.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmCambiarUsuario";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "La Ermita - Cambiar Usuario";
            this.Load += new System.EventHandler(this.FrmCambiarUsuario_Load);
            this.panelContenido.ResumeLayout(false);
            this.panelContenido.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblActual;
        private System.Windows.Forms.Label lblInstruccion;
        private System.Windows.Forms.Label lblUsuarioRapido;
        private System.Windows.Forms.ComboBox cmbUsuarioRapido;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.Label lblContrasena;
        private System.Windows.Forms.TextBox txtContrasena;
        private System.Windows.Forms.Button btnCambiar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Panel panelContenido;
    }
}
