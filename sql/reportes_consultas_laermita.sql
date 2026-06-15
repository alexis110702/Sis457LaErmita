/* ============================================================================
   CONSULTAS Y PRUEBAS DE REPORTES - LA ERMITA
   Ejecutar despues de ddl_dml_laermita_profesional.sql
   ============================================================================ */
USE LabLaErmita;
GO

-- 1. Ventas diarias
EXEC paReporteVentasDiarias;
GO

-- 2. Ventas mensuales
EXEC paReporteVentasMensuales;
GO

-- 3. Productos mas vendidos
EXEC paReporteProductosMasVendidos;
GO

-- 4. Clientes frecuentes
EXEC paReporteClientesFrecuentes;
GO

-- 5. Compras realizadas
EXEC paReporteComprasRealizadas;
GO

-- 6. Inventario actual
EXEC paReporteInventarioActual;
GO

-- 7. Productos con poco stock
EXEC paReporteProductosStockBajo;
GO

-- 8. Ingresos y ganancias
EXEC paReporteIngresosGanancias;
GO

-- 9. Informacion contextual del producto para el modulo de ventas
EXEC paProductoContexto 1;
GO
