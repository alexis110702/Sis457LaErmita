// ============================================================
// FrmProveedor.cs
// Formulario independiente del módulo Proveedores.
// Mantiene Windows Forms con archivo Designer.cs para que Visual
// Studio lo muestre como formulario individual en el proyecto.
// Reutiliza la lógica segura de FrmCatalogoProfesional.
// ============================================================

namespace CpLaErmita
{
    public partial class FrmProveedor : FrmCatalogoProfesional
    {
        public FrmProveedor() : base("Proveedores")
        {
            InitializeComponent();
        }
    }
}
