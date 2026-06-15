// ============================================================
// FrmRol.cs
// Formulario independiente del módulo Roles.
// Mantiene Windows Forms con archivo Designer.cs para que Visual
// Studio lo muestre como formulario individual en el proyecto.
// Reutiliza la lógica segura de FrmCatalogoProfesional.
// ============================================================

namespace CpLaErmita
{
    public partial class FrmRol : FrmCatalogoProfesional
    {
        public FrmRol() : base("Roles")
        {
            InitializeComponent();
        }
    }
}
