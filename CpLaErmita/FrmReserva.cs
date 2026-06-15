// ============================================================
// FrmReserva.cs
// Formulario independiente del módulo Reservas.
// Mantiene Windows Forms con archivo Designer.cs para que Visual
// Studio lo muestre como formulario individual en el proyecto.
// Reutiliza la lógica segura de FrmCatalogoProfesional.
// ============================================================

namespace CpLaErmita
{
    public partial class FrmReserva : FrmCatalogoProfesional
    {
        public FrmReserva() : base("Reservas")
        {
            InitializeComponent();
        }
    }
}
