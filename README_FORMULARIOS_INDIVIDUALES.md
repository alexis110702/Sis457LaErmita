# Formularios individuales WinForms

Esta versión mantiene Windows Forms clásico y agrega formularios individuales para cada módulo visible en el menú principal.

Cada formulario tiene su estructura completa:

- `Formulario.cs`
- `Formulario.Designer.cs`
- `Formulario.resx`

Formularios agregados:

- `FrmUsuario`
- `FrmRol`
- `FrmCliente`
- `FrmProveedor`
- `FrmMesa`
- `FrmMetodoPago`
- `FrmReserva`
- `FrmCompra`
- `FrmInventario`
- `FrmConfiguracion`

Los formularios `FrmCategoria`, `FrmProducto`, `FrmVenta`, `FrmPermisosRol` y `FrmReportes` se mantienen como formularios propios.

Para evitar repetir código y romper funcionalidades, los formularios de catálogos reutilizan la lógica segura de `FrmCatalogoProfesional`, pero ahora aparecen de forma independiente en el Explorador de soluciones de Visual Studio.
