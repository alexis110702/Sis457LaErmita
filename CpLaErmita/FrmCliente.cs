// ============================================================
// FrmCliente.cs
// Formulario independiente del módulo Clientes.
// Mantiene Windows Forms con archivo Designer.cs para que Visual
// Studio lo muestre como formulario individual en el proyecto.
// Reutiliza la lógica segura de FrmCatalogoProfesional.
// ============================================================

namespace CpLaErmita
{
    public partial class FrmCliente : FrmCatalogoProfesional
    {
        public FrmCliente() : base("Clientes")
        {
            InitializeComponent();
        }
    }
}
