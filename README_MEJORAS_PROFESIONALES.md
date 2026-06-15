# La Ermita - Versión profesional académica

## Archivos importantes agregados

- `sql/ddl_dml_laermita_profesional.sql`: base de datos completa normalizada y compatible con el sistema actual.
- `sql/reportes_consultas_laermita.sql`: pruebas de reportes.
- `docs/DIAGRAMA_ER_LAERMITA.mmd`: diagrama entidad-relación en Mermaid.
- `docs/ANALISIS_Y_DEFENSA.md`: análisis técnico, correcciones y guía para defensa.
- `ClnLaErmita/PasswordHasher.cs`: hash PBKDF2 para contraseñas.
- `ClnLaErmita/ProfesionalCln.cs`: CRUD genérico parametrizado para módulos nuevos.
- `ClnLaErmita/ReportesCln.cs`: ejecución de reportes.
- `ClnLaErmita/PermisosCln.cs`: asignación de permisos por rol.
- `CpLaErmita/FrmCatalogoProfesional.cs`: formulario CRUD genérico.
- `CpLaErmita/FrmReportes.cs`: formulario de reportes.
- `CpLaErmita/FrmPermisosRol.cs`: formulario para asignar permisos.

## Pasos rápidos

1. Ejecutar `sql/ddl_dml_laermita_profesional.sql` en SQL Server.
2. Abrir la solución en Visual Studio.
3. Compilar.
4. Iniciar sesión con:
   - Usuario: `admin`
   - Contraseña: `admin123`

## Qué se corrigió según observaciones

- Base de datos ampliada de 6 tablas a un modelo profesional.
- Validación de duplicados en categorías y productos.
- Listados con scroll, autoajuste y mejor visualización.
- Información contextual del producto en ventas.
- Gestión de clientes, proveedores, mesas, reservas, compras e inventario.
- Reportes gerenciales.
- Menú con iconos y mejor interfaz.
- Contraseñas con PBKDF2 para nuevos datos.
- Consultas nuevas parametrizadas.


## Ajuste WinForms Designer

Esta versión mantiene el patrón clásico de Windows Forms para el trabajo académico:

- Cada formulario nuevo tiene su archivo `.cs`.
- Cada formulario nuevo tiene su archivo `.Designer.cs`.
- Cada formulario nuevo tiene su archivo `.resx`.
- El archivo `CpLaErmita.csproj` referencia los formularios con `DependentUpon`, igual que los formularios originales.
- El ícono `cafe_realista.ico` está dentro de `CpLaErmita` para evitar el error CS7064 al compilar.

Formularios nuevos convertidos al patrón WinForms:

- `FrmCatalogoProfesional.cs` + `FrmCatalogoProfesional.Designer.cs`
- `FrmReportes.cs` + `FrmReportes.Designer.cs`
- `FrmPermisosRol.cs` + `FrmPermisosRol.Designer.cs`
