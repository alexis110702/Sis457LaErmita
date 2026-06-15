using System;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;

namespace ClnLaErmita
{
    public static class ReportesCln
    {
        public static DataTable Ejecutar(string reporte)
        {
            string procedimiento = ObtenerProcedimiento(reporte);
            using (var cn = CrearConexion())
            using (var da = new SqlDataAdapter(procedimiento, cn))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        private static string ObtenerProcedimiento(string reporte)
        {
            switch (reporte)
            {
                case "Ventas diarias": return "paReporteVentasDiarias";
                case "Ventas mensuales": return "paReporteVentasMensuales";
                case "Productos más vendidos": return "paReporteProductosMasVendidos";
                case "Clientes frecuentes": return "paReporteClientesFrecuentes";
                case "Compras realizadas": return "paReporteComprasRealizadas";
                case "Inventario actual": return "paReporteInventarioActual";
                case "Productos con poco stock": return "paReporteProductosStockBajo";
                case "Ingresos y ganancias": return "paReporteIngresosGanancias";
                default: throw new Exception("Reporte no permitido.");
            }
        }

        private static SqlConnection CrearConexion()
        {
            string cs = ConfigurationManager.ConnectionStrings["LabLaErmitaEntities"].ConnectionString;
            var builder = new EntityConnectionStringBuilder(cs);
            return new SqlConnection(builder.ProviderConnectionString);
        }
    }
}
