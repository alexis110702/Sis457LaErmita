// ============================================================
// FrmMetodoPago.cs
// Formulario independiente del módulo Métodos de Pago.
// Mantiene Windows Forms con archivo Designer.cs para que Visual
// Studio lo muestre como formulario individual en el proyecto.
// Reutiliza la lógica segura de FrmCatalogoProfesional.
// ============================================================

namespace CpLaErmita
{
    public partial class FrmMetodoPago : FrmCatalogoProfesional
    {
        public FrmMetodoPago() : base("MetodosPago")
        {
            InitializeComponent();
        }
    }
}
