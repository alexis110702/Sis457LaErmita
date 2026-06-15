/* ============================================================================
   SISTEMA PROFESIONAL CAFETERIA / RESTAURANTE "LA ERMITA"
   Script: ddl_dml_laermita_profesional.sql
   Motor : SQL Server

   OBJETIVO:
   - Ampliar la BD original de 6 tablas a un modelo normalizado 3FN.
   - Mantener compatibilidad con las pantallas existentes del proyecto:
     Rol, Usuario, Categoria, Producto, Venta y DetalleVenta conservan sus nombres.
   - Agregar seguridad, clientes, proveedores, mesas, reservas, compras,
     inventario, movimientos, metodos de pago, permisos, historial y reportes.

   IMPORTANTE:
   - Ejecutar este script en SQL Server Management Studio.
   - Si ya existe una BD anterior, hacer respaldo antes de ejecutar.
   ============================================================================ */

USE master;
GO

IF DB_ID('LabLaErmita') IS NOT NULL
BEGIN
    ALTER DATABASE LabLaErmita SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE LabLaErmita;
END
GO

CREATE DATABASE LabLaErmita;
GO

IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = 'usrlaermita')
BEGIN
    CREATE LOGIN usrlaermita WITH PASSWORD = '12345678',
      DEFAULT_DATABASE = LabLaErmita,
      CHECK_EXPIRATION = OFF,
      CHECK_POLICY     = ON;
END
GO

USE LabLaErmita;
GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'usrlaermita')
BEGIN
    CREATE USER usrlaermita FOR LOGIN usrlaermita;
END
GO
ALTER ROLE db_owner ADD MEMBER usrlaermita;
GO

/* =============================
   SEGURIDAD
   ============================= */
CREATE TABLE Rol (
    id          INT IDENTITY(1,1) NOT NULL,
    nombre      VARCHAR(40)       NOT NULL,
    descripcion VARCHAR(150)      NULL,
    estado      SMALLINT          NOT NULL CONSTRAINT DF_Rol_estado DEFAULT 1,
    fechaRegistro DATETIME2       NOT NULL CONSTRAINT DF_Rol_fecha DEFAULT SYSDATETIME(),
    CONSTRAINT PK_Rol PRIMARY KEY (id),
    CONSTRAINT UQ_Rol_nombre UNIQUE (nombre),
    CONSTRAINT CK_Rol_estado CHECK (estado IN (1,0,-1))
);
GO

CREATE TABLE Permiso (
    id          INT IDENTITY(1,1) NOT NULL,
    modulo      VARCHAR(60)       NOT NULL,
    accion      VARCHAR(40)       NOT NULL,
    descripcion VARCHAR(150)      NULL,
    estado      SMALLINT          NOT NULL CONSTRAINT DF_Permiso_estado DEFAULT 1,
    CONSTRAINT PK_Permiso PRIMARY KEY (id),
    CONSTRAINT UQ_Permiso_modulo_accion UNIQUE (modulo, accion),
    CONSTRAINT CK_Permiso_estado CHECK (estado IN (1,0,-1))
);
GO

CREATE TABLE RolPermiso (
    idRol     INT NOT NULL,
    idPermiso INT NOT NULL,
    CONSTRAINT PK_RolPermiso PRIMARY KEY (idRol, idPermiso),
    CONSTRAINT FK_RolPermiso_Rol FOREIGN KEY (idRol) REFERENCES Rol(id),
    CONSTRAINT FK_RolPermiso_Permiso FOREIGN KEY (idPermiso) REFERENCES Permiso(id)
);
GO

CREATE TABLE Usuario (
    id         INT IDENTITY(1,1) NOT NULL,
    idRol      INT               NOT NULL,
    nombre     VARCHAR(100)      NOT NULL,
    usuario    VARCHAR(50)       NOT NULL,
    contrasena VARCHAR(250)      NOT NULL, -- Formato PBKDF2$iteraciones$salt$hash
    email      VARCHAR(120)      NULL,
    telefono   VARCHAR(30)       NULL,
    estado     SMALLINT          NOT NULL CONSTRAINT DF_Usuario_estado DEFAULT 1,
    fechaRegistro DATETIME2      NOT NULL CONSTRAINT DF_Usuario_fecha DEFAULT SYSDATETIME(),
    ultimoAcceso DATETIME2       NULL,
    CONSTRAINT PK_Usuario PRIMARY KEY (id),
    CONSTRAINT FK_Usuario_Rol FOREIGN KEY (idRol) REFERENCES Rol(id),
    CONSTRAINT UQ_Usuario_usuario UNIQUE (usuario),
    CONSTRAINT CK_Usuario_estado CHECK (estado IN (1,0,-1))
);
GO

/* =============================
   CATALOGOS
   ============================= */
CREATE TABLE Categoria (
    id          INT IDENTITY(1,1) NOT NULL,
    nombre      VARCHAR(50)       NOT NULL,
    descripcion VARCHAR(150)      NOT NULL,
    estado      SMALLINT          NOT NULL CONSTRAINT DF_Categoria_estado DEFAULT 1,
    fechaRegistro DATETIME2      NOT NULL CONSTRAINT DF_Categoria_fecha DEFAULT SYSDATETIME(),
    CONSTRAINT PK_Categoria PRIMARY KEY (id),
    CONSTRAINT UQ_Categoria_nombre UNIQUE (nombre),
    CONSTRAINT CK_Categoria_estado CHECK (estado IN (1,0,-1))
);
GO

CREATE TABLE Producto (
    id            INT IDENTITY(1,1) NOT NULL,
    idCategoria   INT               NOT NULL,
    codigo        VARCHAR(30)       NOT NULL CONSTRAINT DF_Producto_codigo DEFAULT LEFT(CONVERT(VARCHAR(36), NEWID()), 30),
    nombre        VARCHAR(100)      NOT NULL,
    descripcion   VARCHAR(250)      NOT NULL,
    precio        DECIMAL(10,2)     NOT NULL,
    costo         DECIMAL(10,2)     NOT NULL CONSTRAINT DF_Producto_costo DEFAULT 0,
    stock         INT               NOT NULL CONSTRAINT DF_Producto_stock DEFAULT 0,
    stockMinimo   INT               NOT NULL CONSTRAINT DF_Producto_stockMinimo DEFAULT 5,
    estado        SMALLINT          NOT NULL CONSTRAINT DF_Producto_estado DEFAULT 1,
    fechaRegistro DATETIME2         NOT NULL CONSTRAINT DF_Producto_fecha DEFAULT SYSDATETIME(),
    CONSTRAINT PK_Producto PRIMARY KEY (id),
    CONSTRAINT FK_Producto_Categoria FOREIGN KEY (idCategoria) REFERENCES Categoria(id),
    CONSTRAINT UQ_Producto_codigo UNIQUE (codigo),
    CONSTRAINT UQ_Producto_nombre UNIQUE (nombre),
    CONSTRAINT CK_Producto_precio CHECK (precio > 0),
    CONSTRAINT CK_Producto_costo CHECK (costo >= 0),
    CONSTRAINT CK_Producto_stock CHECK (stock >= 0),
    CONSTRAINT CK_Producto_stockMinimo CHECK (stockMinimo >= 0),
    CONSTRAINT CK_Producto_estado CHECK (estado IN (1,0,-1))
);
GO

CREATE TABLE Proveedor (
    id          INT IDENTITY(1,1) NOT NULL,
    nombre      VARCHAR(120)      NOT NULL,
    nit         VARCHAR(30)       NULL,
    telefono    VARCHAR(30)       NULL,
    email       VARCHAR(120)      NULL,
    direccion   VARCHAR(180)      NULL,
    contacto    VARCHAR(100)      NULL,
    estado      SMALLINT          NOT NULL CONSTRAINT DF_Proveedor_estado DEFAULT 1,
    CONSTRAINT PK_Proveedor PRIMARY KEY (id),
    CONSTRAINT UQ_Proveedor_nombre UNIQUE (nombre),
    CONSTRAINT CK_Proveedor_estado CHECK (estado IN (1,0,-1))
);
GO

CREATE TABLE MetodoPago (
    id          INT IDENTITY(1,1) NOT NULL,
    nombre      VARCHAR(50)       NOT NULL,
    descripcion VARCHAR(150)      NULL,
    estado      SMALLINT          NOT NULL CONSTRAINT DF_MetodoPago_estado DEFAULT 1,
    CONSTRAINT PK_MetodoPago PRIMARY KEY (id),
    CONSTRAINT UQ_MetodoPago_nombre UNIQUE (nombre),
    CONSTRAINT CK_MetodoPago_estado CHECK (estado IN (1,0,-1))
);
GO

/* =============================
   CLIENTES Y RESTAURANTE
   ============================= */
CREATE TABLE Cliente (
    id          INT IDENTITY(1,1) NOT NULL,
    nombre      VARCHAR(120)      NOT NULL,
    documento   VARCHAR(30)       NULL,
    telefono    VARCHAR(30)       NULL,
    email       VARCHAR(120)      NULL,
    direccion   VARCHAR(180)      NULL,
    estado      SMALLINT          NOT NULL CONSTRAINT DF_Cliente_estado DEFAULT 1,
    fechaRegistro DATETIME2      NOT NULL CONSTRAINT DF_Cliente_fecha DEFAULT SYSDATETIME(),
    CONSTRAINT PK_Cliente PRIMARY KEY (id),
    CONSTRAINT UQ_Cliente_documento UNIQUE (documento),
    CONSTRAINT CK_Cliente_estado CHECK (estado IN (1,0,-1))
);
GO

CREATE TABLE Mesa (
    id          INT IDENTITY(1,1) NOT NULL,
    numero      VARCHAR(20)       NOT NULL,
    capacidad   INT               NOT NULL,
    estadoMesa  VARCHAR(20)       NOT NULL CONSTRAINT DF_Mesa_estadoMesa DEFAULT 'Libre',
    ubicacion    VARCHAR(80)      NULL,
    estado      SMALLINT          NOT NULL CONSTRAINT DF_Mesa_estado DEFAULT 1,
    CONSTRAINT PK_Mesa PRIMARY KEY (id),
    CONSTRAINT UQ_Mesa_numero UNIQUE (numero),
    CONSTRAINT CK_Mesa_capacidad CHECK (capacidad > 0),
    CONSTRAINT CK_Mesa_estadoMesa CHECK (estadoMesa IN ('Libre','Ocupada','Reservada')),
    CONSTRAINT CK_Mesa_estado CHECK (estado IN (1,0,-1))
);
GO

CREATE TABLE Reserva (
    id              INT IDENTITY(1,1) NOT NULL,
    idCliente        INT               NOT NULL,
    idMesa           INT               NOT NULL,
    fechaReserva     DATE              NOT NULL,
    horaReserva      TIME(0)           NOT NULL,
    cantidadPersonas INT               NOT NULL,
    observacion      VARCHAR(250)      NULL,
    estadoReserva    VARCHAR(20)       NOT NULL CONSTRAINT DF_Reserva_estadoReserva DEFAULT 'Registrada',
    estado           SMALLINT          NOT NULL CONSTRAINT DF_Reserva_estado DEFAULT 1,
    fechaRegistro    DATETIME2         NOT NULL CONSTRAINT DF_Reserva_fecha DEFAULT SYSDATETIME(),
    CONSTRAINT PK_Reserva PRIMARY KEY (id),
    CONSTRAINT FK_Reserva_Cliente FOREIGN KEY (idCliente) REFERENCES Cliente(id),
    CONSTRAINT FK_Reserva_Mesa FOREIGN KEY (idMesa) REFERENCES Mesa(id),
    CONSTRAINT CK_Reserva_personas CHECK (cantidadPersonas > 0),
    CONSTRAINT CK_Reserva_estadoReserva CHECK (estadoReserva IN ('Registrada','Confirmada','Cancelada','Atendida')),
    CONSTRAINT CK_Reserva_estado CHECK (estado IN (1,0,-1))
);
GO

/* =============================
   COMPRAS E INVENTARIO
   ============================= */
CREATE TABLE Compra (
    id              INT IDENTITY(1,1) NOT NULL,
    idProveedor      INT               NOT NULL,
    idUsuario        INT               NOT NULL,
    fecha            DATETIME2         NOT NULL CONSTRAINT DF_Compra_fecha DEFAULT SYSDATETIME(),
    numeroDocumento  VARCHAR(50)       NULL,
    subtotal         DECIMAL(10,2)     NOT NULL CONSTRAINT DF_Compra_subtotal DEFAULT 0,
    impuesto         DECIMAL(10,2)     NOT NULL CONSTRAINT DF_Compra_impuesto DEFAULT 0,
    total            DECIMAL(10,2)     NOT NULL CONSTRAINT DF_Compra_total DEFAULT 0,
    observacion      VARCHAR(250)      NULL,
    estado           SMALLINT          NOT NULL CONSTRAINT DF_Compra_estado DEFAULT 1,
    CONSTRAINT PK_Compra PRIMARY KEY (id),
    CONSTRAINT FK_Compra_Proveedor FOREIGN KEY (idProveedor) REFERENCES Proveedor(id),
    CONSTRAINT FK_Compra_Usuario FOREIGN KEY (idUsuario) REFERENCES Usuario(id),
    CONSTRAINT CK_Compra_totales CHECK (subtotal >= 0 AND impuesto >= 0 AND total >= 0),
    CONSTRAINT CK_Compra_estado CHECK (estado IN (1,0,-1))
);
GO

CREATE TABLE DetalleCompra (
    id          INT IDENTITY(1,1) NOT NULL,
    idCompra    INT               NOT NULL,
    idProducto  INT               NOT NULL,
    cantidad    INT               NOT NULL,
    costo       DECIMAL(10,2)     NOT NULL,
    subtotal    DECIMAL(10,2)     NOT NULL,
    estado      SMALLINT          NOT NULL CONSTRAINT DF_DetalleCompra_estado DEFAULT 1,
    CONSTRAINT PK_DetalleCompra PRIMARY KEY (id),
    CONSTRAINT FK_DetalleCompra_Compra FOREIGN KEY (idCompra) REFERENCES Compra(id),
    CONSTRAINT FK_DetalleCompra_Producto FOREIGN KEY (idProducto) REFERENCES Producto(id),
    CONSTRAINT CK_DetalleCompra_cantidad CHECK (cantidad > 0),
    CONSTRAINT CK_DetalleCompra_costo CHECK (costo >= 0),
    CONSTRAINT CK_DetalleCompra_estado CHECK (estado IN (1,0,-1))
);
GO

CREATE TABLE Inventario (
    id          INT IDENTITY(1,1) NOT NULL,
    idProducto  INT               NOT NULL,
    stockActual INT               NOT NULL CONSTRAINT DF_Inventario_stock DEFAULT 0,
    stockMinimo INT               NOT NULL CONSTRAINT DF_Inventario_minimo DEFAULT 5,
    fechaActualizacion DATETIME2  NOT NULL CONSTRAINT DF_Inventario_fecha DEFAULT SYSDATETIME(),
    CONSTRAINT PK_Inventario PRIMARY KEY (id),
    CONSTRAINT FK_Inventario_Producto FOREIGN KEY (idProducto) REFERENCES Producto(id),
    CONSTRAINT UQ_Inventario_Producto UNIQUE (idProducto),
    CONSTRAINT CK_Inventario_stock CHECK (stockActual >= 0),
    CONSTRAINT CK_Inventario_minimo CHECK (stockMinimo >= 0)
);
GO

CREATE TABLE MovimientoInventario (
    id            INT IDENTITY(1,1) NOT NULL,
    idProducto     INT               NOT NULL,
    idUsuario      INT               NOT NULL,
    tipoMovimiento VARCHAR(20)       NOT NULL, -- Entrada, Salida, Ajuste
    origen         VARCHAR(30)       NOT NULL, -- Compra, Venta, Manual, Anulacion
    idReferencia   INT               NULL,
    cantidad       INT               NOT NULL,
    stockAnterior  INT               NOT NULL,
    stockNuevo     INT               NOT NULL,
    observacion    VARCHAR(250)      NULL,
    fecha          DATETIME2         NOT NULL CONSTRAINT DF_MovimientoInventario_fecha DEFAULT SYSDATETIME(),
    CONSTRAINT PK_MovimientoInventario PRIMARY KEY (id),
    CONSTRAINT FK_MovimientoInventario_Producto FOREIGN KEY (idProducto) REFERENCES Producto(id),
    CONSTRAINT FK_MovimientoInventario_Usuario FOREIGN KEY (idUsuario) REFERENCES Usuario(id),
    CONSTRAINT CK_MovimientoInventario_tipo CHECK (tipoMovimiento IN ('Entrada','Salida','Ajuste')),
    CONSTRAINT CK_MovimientoInventario_origen CHECK (origen IN ('Compra','Venta','Manual','Anulacion')),
    CONSTRAINT CK_MovimientoInventario_cantidad CHECK (cantidad > 0),
    CONSTRAINT CK_MovimientoInventario_stock CHECK (stockAnterior >= 0 AND stockNuevo >= 0)
);
GO

/* =============================
   VENTAS
   Se conserva campo mesa VARCHAR para compatibilidad con EF existente.
   Además se agregan FKs profesionales opcionales: idCliente, idMesa, idMetodoPago.
   ============================= */
CREATE TABLE Venta (
    id            INT IDENTITY(1,1) NOT NULL,
    idUsuario     INT               NOT NULL,
    idCliente     INT               NULL,
    idMesa        INT               NULL,
    idMetodoPago  INT               NULL,
    mesa          VARCHAR(20)       NOT NULL,
    fecha         DATETIME2         NOT NULL CONSTRAINT DF_Venta_fecha DEFAULT SYSDATETIME(),
    subtotal      DECIMAL(10,2)     NOT NULL CONSTRAINT DF_Venta_subtotal DEFAULT 0,
    descuento     DECIMAL(10,2)     NOT NULL CONSTRAINT DF_Venta_descuento DEFAULT 0,
    impuesto      DECIMAL(10,2)     NOT NULL CONSTRAINT DF_Venta_impuesto DEFAULT 0,
    total         DECIMAL(10,2)     NOT NULL CONSTRAINT DF_Venta_total DEFAULT 0,
    observacion   VARCHAR(250)      NULL,
    estado        SMALLINT          NOT NULL CONSTRAINT DF_Venta_estado DEFAULT 1,
    CONSTRAINT PK_Venta PRIMARY KEY (id),
    CONSTRAINT FK_Venta_Usuario FOREIGN KEY (idUsuario) REFERENCES Usuario(id),
    CONSTRAINT FK_Venta_Cliente FOREIGN KEY (idCliente) REFERENCES Cliente(id),
    CONSTRAINT FK_Venta_Mesa FOREIGN KEY (idMesa) REFERENCES Mesa(id),
    CONSTRAINT FK_Venta_MetodoPago FOREIGN KEY (idMetodoPago) REFERENCES MetodoPago(id),
    CONSTRAINT CK_Venta_totales CHECK (subtotal >= 0 AND descuento >= 0 AND impuesto >= 0 AND total >= 0),
    CONSTRAINT CK_Venta_estado CHECK (estado IN (1,0,-1))
);
GO

CREATE TABLE DetalleVenta (
    id          INT IDENTITY(1,1) NOT NULL,
    idVenta     INT               NOT NULL,
    idProducto  INT               NOT NULL,
    cantidad    INT               NOT NULL,
    precio      DECIMAL(10,2)     NOT NULL,
    descuento   DECIMAL(10,2)     NOT NULL CONSTRAINT DF_DetalleVenta_descuento DEFAULT 0,
    subtotal    DECIMAL(10,2)     NOT NULL,
    estado      SMALLINT          NOT NULL CONSTRAINT DF_DetalleVenta_estado DEFAULT 1,
    CONSTRAINT PK_DetalleVenta PRIMARY KEY (id),
    CONSTRAINT FK_DetalleVenta_Venta FOREIGN KEY (idVenta) REFERENCES Venta(id),
    CONSTRAINT FK_DetalleVenta_Producto FOREIGN KEY (idProducto) REFERENCES Producto(id),
    CONSTRAINT CK_DetalleVenta_cantidad CHECK (cantidad > 0),
    CONSTRAINT CK_DetalleVenta_precio CHECK (precio >= 0),
    CONSTRAINT CK_DetalleVenta_descuento CHECK (descuento >= 0),
    CONSTRAINT CK_DetalleVenta_estado CHECK (estado IN (1,0,-1))
);
GO

/* =============================
   AUDITORIA / REPORTES
   ============================= */
CREATE TABLE HistorialAccion (
    id          INT IDENTITY(1,1) NOT NULL,
    idUsuario   INT               NULL,
    modulo      VARCHAR(60)       NOT NULL,
    accion      VARCHAR(60)       NOT NULL,
    descripcion VARCHAR(300)      NULL,
    fecha       DATETIME2         NOT NULL CONSTRAINT DF_HistorialAccion_fecha DEFAULT SYSDATETIME(),
    equipo      VARCHAR(80)       NULL,
    CONSTRAINT PK_HistorialAccion PRIMARY KEY (id),
    CONSTRAINT FK_HistorialAccion_Usuario FOREIGN KEY (idUsuario) REFERENCES Usuario(id)
);
GO

/* =============================
   INDICES
   ============================= */
CREATE INDEX IX_Producto_Categoria ON Producto(idCategoria);
CREATE INDEX IX_Producto_Nombre ON Producto(nombre);
CREATE INDEX IX_Venta_Fecha ON Venta(fecha);
CREATE INDEX IX_Venta_Usuario ON Venta(idUsuario);
CREATE INDEX IX_DetalleVenta_Producto ON DetalleVenta(idProducto);
CREATE INDEX IX_Compra_Fecha ON Compra(fecha);
CREATE INDEX IX_DetalleCompra_Producto ON DetalleCompra(idProducto);
CREATE INDEX IX_MovimientoInventario_ProductoFecha ON MovimientoInventario(idProducto, fecha DESC);
CREATE INDEX IX_Reserva_FechaMesa ON Reserva(fechaReserva, idMesa);
GO

/* =============================
   VISTAS
   ============================= */
CREATE VIEW vwInventarioActual
AS
SELECT
    i.id,
    p.codigo,
    p.nombre AS producto,
    c.nombre AS categoria,
    i.stockActual,
    i.stockMinimo,
    CASE WHEN i.stockActual <= i.stockMinimo THEN 'BAJO' ELSE 'OK' END AS alerta,
    i.fechaActualizacion
FROM Inventario i
INNER JOIN Producto p ON p.id = i.idProducto
INNER JOIN Categoria c ON c.id = p.idCategoria
WHERE p.estado = 1;
GO

CREATE VIEW vwProductosContexto
AS
SELECT
    p.id,
    p.codigo,
    p.nombre,
    p.descripcion,
    c.nombre AS categoria,
    p.precio,
    p.costo,
    p.stock,
    p.stockMinimo,
    CASE WHEN p.stock <= p.stockMinimo THEN 'BAJO' ELSE 'OK' END AS alertaStock
FROM Producto p
INNER JOIN Categoria c ON c.id = p.idCategoria
WHERE p.estado = 1;
GO

/* =============================
   PROCEDIMIENTOS DEL SISTEMA EXISTENTE + MEJORAS
   ============================= */
CREATE PROC paUsuarioLogin
  @usuario    VARCHAR(50),
  @contrasena VARCHAR(250)
AS
BEGIN
  SET NOCOUNT ON;
  SELECT u.id, u.idRol, u.nombre, u.usuario, r.nombre AS rol
  FROM   Usuario u
  INNER JOIN Rol r ON r.id = u.idRol
  WHERE  u.usuario    = @usuario
    AND  u.contrasena = @contrasena
    AND  u.estado     = 1
    AND  r.estado     = 1;
END
GO

CREATE PROC paCategoriaListar @parametro VARCHAR(50)
AS
BEGIN
  SET NOCOUNT ON;
  SELECT id, nombre, descripcion, estado
  FROM   Categoria
  WHERE  estado = 1
    AND  (nombre + ' ' + descripcion) LIKE '%' + REPLACE(ISNULL(@parametro,''),' ','%') + '%'
  ORDER BY nombre;
END
GO

CREATE PROC paProductoListar @parametro VARCHAR(100)
AS
BEGIN
  SET NOCOUNT ON;
  SELECT p.id, p.idCategoria, p.nombre, p.descripcion,
         c.nombre AS categoria,
         p.precio, p.stock, p.estado
  FROM   Producto p
  INNER JOIN Categoria c ON c.id = p.idCategoria
  WHERE  p.estado = 1
    AND  (p.codigo + ' ' + p.nombre + ' ' + p.descripcion + ' ' + c.nombre) LIKE '%' + REPLACE(ISNULL(@parametro,''),' ','%') + '%'
  ORDER BY p.nombre;
END
GO

CREATE PROC paVentaListar @parametro VARCHAR(50)
AS
BEGIN
  SET NOCOUNT ON;
  SELECT v.id, v.idUsuario, v.mesa,
         u.nombre AS cajero,
         v.fecha, v.total, v.estado
  FROM   Venta v
  INNER JOIN Usuario u ON u.id = v.idUsuario
  WHERE  v.estado = 1
    AND  (v.mesa + ' ' + u.nombre) LIKE '%' + REPLACE(ISNULL(@parametro,''),' ','%') + '%'
  ORDER BY v.fecha DESC;
END
GO

CREATE PROC paDetalleVentaListar @idVenta INT
AS
BEGIN
  SET NOCOUNT ON;
  SELECT dv.id, dv.idVenta, dv.idProducto,
         p.nombre AS producto,
         dv.cantidad, dv.precio, dv.subtotal, dv.estado
  FROM   DetalleVenta dv
  INNER JOIN Producto p ON p.id = dv.idProducto
  WHERE  dv.idVenta = @idVenta
    AND  dv.estado  = 1
  ORDER BY dv.id;
END
GO

CREATE PROC paProductoContexto @idProducto INT
AS
BEGIN
  SET NOCOUNT ON;
  SELECT * FROM vwProductosContexto WHERE id = @idProducto;
END
GO

/* =============================
   REPORTES
   ============================= */
CREATE PROC paReporteVentasDiarias
AS
BEGIN
  SET NOCOUNT ON;
  SELECT CONVERT(date, fecha) AS fecha,
         COUNT(*) AS cantidadVentas,
         SUM(total) AS totalVentas
  FROM Venta
  WHERE estado = 1
  GROUP BY CONVERT(date, fecha)
  ORDER BY fecha DESC;
END
GO

CREATE PROC paReporteVentasMensuales
AS
BEGIN
  SET NOCOUNT ON;
  SELECT YEAR(fecha) AS gestion,
         MONTH(fecha) AS mes,
         COUNT(*) AS cantidadVentas,
         SUM(total) AS totalVentas
  FROM Venta
  WHERE estado = 1
  GROUP BY YEAR(fecha), MONTH(fecha)
  ORDER BY gestion DESC, mes DESC;
END
GO

CREATE PROC paReporteProductosMasVendidos
AS
BEGIN
  SET NOCOUNT ON;
  SELECT TOP 20 p.nombre AS producto,
         c.nombre AS categoria,
         SUM(dv.cantidad) AS unidadesVendidas,
         SUM(dv.subtotal) AS totalVendido
  FROM DetalleVenta dv
  INNER JOIN Producto p ON p.id = dv.idProducto
  INNER JOIN Categoria c ON c.id = p.idCategoria
  INNER JOIN Venta v ON v.id = dv.idVenta
  WHERE dv.estado = 1 AND v.estado = 1
  GROUP BY p.nombre, c.nombre
  ORDER BY unidadesVendidas DESC, totalVendido DESC;
END
GO

CREATE PROC paReporteClientesFrecuentes
AS
BEGIN
  SET NOCOUNT ON;
  SELECT TOP 20 ISNULL(c.nombre, 'Consumidor final') AS cliente,
         COUNT(v.id) AS cantidadVentas,
         SUM(v.total) AS totalConsumido
  FROM Venta v
  LEFT JOIN Cliente c ON c.id = v.idCliente
  WHERE v.estado = 1
  GROUP BY ISNULL(c.nombre, 'Consumidor final')
  ORDER BY cantidadVentas DESC, totalConsumido DESC;
END
GO

CREATE PROC paReporteComprasRealizadas
AS
BEGIN
  SET NOCOUNT ON;
  SELECT co.id, co.fecha, pr.nombre AS proveedor,
         u.nombre AS usuario, co.numeroDocumento, co.total, co.estado
  FROM Compra co
  INNER JOIN Proveedor pr ON pr.id = co.idProveedor
  INNER JOIN Usuario u ON u.id = co.idUsuario
  ORDER BY co.fecha DESC;
END
GO

CREATE PROC paReporteInventarioActual
AS
BEGIN
  SET NOCOUNT ON;
  SELECT * FROM vwInventarioActual ORDER BY alerta DESC, producto;
END
GO

CREATE PROC paReporteProductosStockBajo
AS
BEGIN
  SET NOCOUNT ON;
  SELECT * FROM vwInventarioActual WHERE alerta = 'BAJO' ORDER BY stockActual ASC;
END
GO

CREATE PROC paReporteIngresosGanancias
AS
BEGIN
  SET NOCOUNT ON;
  SELECT CONVERT(date, v.fecha) AS fecha,
         SUM(dv.subtotal) AS ingresos,
         SUM(dv.cantidad * p.costo) AS costoEstimado,
         SUM(dv.subtotal - (dv.cantidad * p.costo)) AS gananciaEstimada
  FROM Venta v
  INNER JOIN DetalleVenta dv ON dv.idVenta = v.id
  INNER JOIN Producto p ON p.id = dv.idProducto
  WHERE v.estado = 1 AND dv.estado = 1
  GROUP BY CONVERT(date, v.fecha)
  ORDER BY fecha DESC;
END
GO

/* =============================
   TRIGGERS DE INVENTARIO
   ============================= */
CREATE TRIGGER trgProducto_InsertInventario
ON Producto
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Inventario (idProducto, stockActual, stockMinimo)
    SELECT i.id, i.stock, i.stockMinimo
    FROM inserted i
    WHERE NOT EXISTS (SELECT 1 FROM Inventario inv WHERE inv.idProducto = i.id);
END
GO

CREATE TRIGGER trgDetalleCompra_ActualizarStock
ON DetalleCompra
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE p
    SET p.stock = p.stock + i.cantidad,
        p.costo = i.costo
    FROM Producto p
    INNER JOIN inserted i ON i.idProducto = p.id;

    UPDATE inv
    SET inv.stockActual = p.stock,
        inv.fechaActualizacion = SYSDATETIME()
    FROM Inventario inv
    INNER JOIN Producto p ON p.id = inv.idProducto
    INNER JOIN inserted i ON i.idProducto = p.id;
END
GO

CREATE TRIGGER trgDetalleVenta_SalidaInventario
ON DetalleVenta
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE inv
    SET inv.stockActual = p.stock,
        inv.fechaActualizacion = SYSDATETIME()
    FROM Inventario inv
    INNER JOIN Producto p ON p.id = inv.idProducto
    INNER JOIN inserted i ON i.idProducto = p.id;
END
GO

/* =============================
   DATOS DE PRUEBA
   Contraseñas:
   admin/admin123, jmamani/caj123, mlopez/mes123
   Están en formato PBKDF2 para login seguro desde UsuarioCln.
   ============================= */
INSERT INTO Rol (nombre, descripcion, estado) VALUES
  ('Administrador', 'Acceso total al sistema', 1),
  ('Cajero',        'Registra ventas y consulta productos', 1),
  ('Mesero',        'Gestiona mesas, reservas y ventas', 1);

INSERT INTO Permiso (modulo, accion, descripcion) VALUES
('Usuarios','Ver','Consultar usuarios'), ('Usuarios','Crear','Crear usuarios'), ('Usuarios','Editar','Editar usuarios'), ('Usuarios','Eliminar','Eliminar usuarios'),
('Roles','Ver','Consultar roles'), ('Roles','Editar','Asignar permisos'),
('Categorias','CRUD','Gestionar categorias'), ('Productos','CRUD','Gestionar productos'),
('Clientes','CRUD','Gestionar clientes'), ('Proveedores','CRUD','Gestionar proveedores'),
('Mesas','CRUD','Gestionar mesas'), ('Reservas','CRUD','Gestionar reservas'),
('Ventas','CRUD','Registrar y anular ventas'), ('Compras','CRUD','Registrar y anular compras'),
('Inventario','Ver','Consultar inventario'), ('Reportes','Ver','Consultar reportes');

INSERT INTO RolPermiso (idRol, idPermiso)
SELECT 1, id FROM Permiso;
INSERT INTO RolPermiso (idRol, idPermiso)
SELECT 2, id FROM Permiso WHERE modulo IN ('Productos','Clientes','Ventas','Inventario','Reportes');
INSERT INTO RolPermiso (idRol, idPermiso)
SELECT 3, id FROM Permiso WHERE modulo IN ('Mesas','Reservas','Ventas','Productos');

INSERT INTO Usuario (idRol, nombre, usuario, contrasena, email, telefono, estado) VALUES
  (1, 'Administrador', 'admin',   'PBKDF2$100000$1etSqnIMCB/8fwGazJCtXA==$Y9ZkfzXQqWyZxwyor2q4e9kUyVDa7+HdNiWrfiQGQjU=', 'admin@laermita.local', '70000001', 1),
  (2, 'Juan Mamani',   'jmamani', 'PBKDF2$100000$7S3Q78TWKnF6UAP3EKaz7A==$o0SumKx4sPxolk9ZFv0zdB8p1Gsq3X0o3eaxTzo/ung=', 'caja@laermita.local', '70000002', 1),
  (3, 'Maria Lopez',   'mlopez',  'PBKDF2$100000$1xpJM4p9xN1p0yfZyhxX6A==$4OKCScTXcFxYIzP4TEyMpK9nw0ERcmqvkTsS7+rDA6I=', 'mesas@laermita.local', '70000003', 1);

INSERT INTO Categoria (nombre, descripcion, estado) VALUES
  ('Bebidas Calientes', 'Cafes, tes e infusiones',       1),
  ('Bebidas Frias',     'Jugos, refrescos y batidos',    1),
  ('Desayunos',         'Sandwiches, tostadas y huevos', 1),
  ('Almuerzos',         'Platos principales del dia',    1),
  ('Postres',           'Pasteles, galletas y helados',  1),
  ('Hamburguesas',      'Hamburguesas artesanales',      1),
  ('Mariscos',          'Platos con pescado y mariscos', 1),
  ('Pollo',             'Platos a base de pollo',        1);

INSERT INTO Producto (idCategoria, codigo, nombre, descripcion, precio, costo, stock, stockMinimo, estado) VALUES
  (1, 'BCA-001', 'Cafe Americano',      'Cafe negro espresso con agua',       15.00,  6.00, 100, 10, 1),
  (1, 'BCA-002', 'Capuchino',           'Espresso con leche espumada',        18.00,  8.00, 100, 10, 1),
  (1, 'BCA-003', 'Te de Manzanilla',    'Infusion natural de manzanilla',     10.00,  3.50, 100, 10, 1),
  (2, 'BFR-001', 'Jugo de Naranja',     'Jugo natural exprimido',             20.00,  8.00,  50,  8, 1),
  (2, 'BFR-002', 'Batido de Fresa',     'Batido con leche y fresas frescas',  25.00, 12.00,  50,  8, 1),
  (3, 'DES-001', 'Sandwich Mixto',      'Pan con jamon, queso y tomate',      30.00, 16.00,  30,  5, 1),
  (3, 'DES-002', 'Tostadas con Miel',   'Tostadas con mantequilla y miel',    20.00,  9.00,  30,  5, 1),
  (4, 'ALM-001', 'Almuerzo del Dia',    'Sopa, segundo y refresco incluido',  45.00, 25.00,  20,  5, 1),
  (5, 'POS-001', 'Pastel de Chocolate', 'Porcion de pastel casero',           22.00, 10.00,  15,  4, 1),
  (5, 'POS-002', 'Cheesecake',          'Cheesecake con frutos rojos',        28.00, 14.00,  15,  4, 1),
  (6, 'HAM-001', 'Hamburguesa Clasica', 'Carne, queso y papas',               35.00, 19.00,  20,  5, 1),
  (7, 'MAR-001', 'Ceviche Mixto',       'Ceviche de pescado y mariscos',      55.00, 32.00,  12,  3, 1),
  (8, 'POL-001', 'Pollo a la Plancha',  'Pollo con guarnicion',               42.00, 23.00,  18,  4, 1);

INSERT INTO Proveedor (nombre, nit, telefono, email, direccion, contacto) VALUES
('Distribuidora Santa Cruz', '123456789', '70111111', 'ventas@dsc.local', 'Av. Principal 123', 'Carlos Rojas'),
('Lacteos del Sur', '987654321', '70222222', 'pedidos@lacteos.local', 'Calle Mercado 45', 'Ana Flores'),
('Carnes Premium', '456789123', '70333333', 'contacto@carnes.local', 'Zona Industrial 88', 'Luis Vargas');

INSERT INTO MetodoPago (nombre, descripcion) VALUES
('Efectivo', 'Pago en moneda nacional'),
('QR', 'Pago mediante codigo QR'),
('Tarjeta', 'Tarjeta de debito o credito'),
('Transferencia', 'Transferencia bancaria');

INSERT INTO Cliente (nombre, documento, telefono, email, direccion) VALUES
('Consumidor Final', '0', NULL, NULL, NULL),
('Pedro Quispe', '6543210', '70444444', 'pedro@mail.local', 'Barrio Centro'),
('Lucia Fernandez', '7654321', '70555555', 'lucia@mail.local', 'Av. Libertad');

INSERT INTO Mesa (numero, capacidad, estadoMesa, ubicacion) VALUES
('M-01', 4, 'Libre', 'Salon principal'),
('M-02', 4, 'Libre', 'Salon principal'),
('M-03', 6, 'Reservada', 'Ventana'),
('M-04', 2, 'Libre', 'Terraza'),
('M-05', 8, 'Libre', 'Familiar');

INSERT INTO Reserva (idCliente, idMesa, fechaReserva, horaReserva, cantidadPersonas, observacion, estadoReserva) VALUES
(2, 3, CONVERT(date, DATEADD(day, 1, GETDATE())), '19:30', 4, 'Cumpleanios', 'Confirmada');

-- Sincronizar inventario inicial con productos por seguridad.
-- El trigger trgProducto_InsertInventario ya lo hace automáticamente.
INSERT INTO Inventario (idProducto, stockActual, stockMinimo)
SELECT p.id, p.stock, p.stockMinimo
FROM Producto p
WHERE NOT EXISTS (SELECT 1 FROM Inventario i WHERE i.idProducto = p.id);

-- Venta de prueba compatible con pantalla actual
INSERT INTO Venta (idUsuario, idCliente, idMesa, idMetodoPago, mesa, fecha, subtotal, descuento, impuesto, total, estado) VALUES
  (2, 1, 1, 1, 'Mesa 1', SYSDATETIME(), 82.00, 0.00, 0.00, 82.00, 1);

INSERT INTO DetalleVenta (idVenta, idProducto, cantidad, precio, descuento, subtotal, estado) VALUES
  (1, 1, 2, 15.00, 0.00, 30.00, 1),
  (1, 6, 1, 30.00, 0.00, 30.00, 1),
  (1, 9, 1, 22.00, 0.00, 22.00, 1);

-- Ajuste de stock de la venta de prueba.
UPDATE p
SET p.stock = p.stock - x.cantidad
FROM Producto p
INNER JOIN (
    SELECT idProducto, SUM(cantidad) cantidad
    FROM DetalleVenta
    WHERE idVenta = 1
    GROUP BY idProducto
) x ON x.idProducto = p.id;

UPDATE i
SET i.stockActual = p.stock,
    i.fechaActualizacion = SYSDATETIME()
FROM Inventario i
INNER JOIN Producto p ON p.id = i.idProducto;

-- Compra de prueba
INSERT INTO Compra (idProveedor, idUsuario, numeroDocumento, subtotal, impuesto, total, observacion) VALUES
(1, 1, 'FC-0001', 120.00, 0.00, 120.00, 'Compra inicial de insumos');

INSERT INTO DetalleCompra (idCompra, idProducto, cantidad, costo, subtotal) VALUES
(1, 1, 10, 6.00, 60.00),
(1, 2, 5, 8.00, 40.00),
(1, 3, 5, 4.00, 20.00);
GO

/* =============================
   DATOS AMPLIADOS PARA DEFENSA
   Objetivo: que las grillas de todos los módulos tengan bastante información.
   Incluye 30+ registros en usuarios, categorías, productos, clientes,
   proveedores, mesas, métodos de pago, reservas, compras, ventas,
   movimientos de inventario e historial.
   ============================= */

-- Más usuarios de prueba: usuario04 ... usuario30, contraseña: admin123
DECLARE @u INT = 4;
WHILE @u <= 30
BEGIN
    INSERT INTO Usuario (idRol, nombre, usuario, contrasena, email, telefono, estado)
    VALUES (
        ((@u - 1) % 3) + 1,
        CONCAT('Usuario Prueba ', RIGHT('00' + CAST(@u AS VARCHAR(2)), 2)),
        CONCAT('usuario', RIGHT('00' + CAST(@u AS VARCHAR(2)), 2)),
        'PBKDF2$100000$1etSqnIMCB/8fwGazJCtXA==$Y9ZkfzXQqWyZxwyor2q4e9kUyVDa7+HdNiWrfiQGQjU=',
        CONCAT('usuario', RIGHT('00' + CAST(@u AS VARCHAR(2)), 2), '@laermita.local'),
        CONCAT('70', RIGHT('000000' + CAST(@u AS VARCHAR(6)), 6)),
        1
    );
    SET @u += 1;
END
GO

-- Más categorías hasta completar 30
INSERT INTO Categoria (nombre, descripcion, estado) VALUES
('Ensaladas', 'Ensaladas frescas y saludables', 1),
('Pastas', 'Pastas con salsas caseras', 1),
('Pizzas', 'Pizzas artesanales', 1),
('Sopas', 'Sopas y cremas del día', 1),
('Jugos Naturales', 'Jugos de frutas de temporada', 1),
('Smoothies', 'Bebidas cremosas con fruta', 1),
('Sandwiches', 'Sandwiches calientes y fríos', 1),
('Tés Especiales', 'Tés e infusiones premium', 1),
('Café Especialidad', 'Bebidas de café de especialidad', 1),
('Bollería', 'Masitas, croissants y panes dulces', 1),
('Helados', 'Helados y postres fríos', 1),
('Parrilla', 'Carnes y platos a la parrilla', 1),
('Comida Rápida', 'Opciones rápidas para llevar', 1),
('Menú Infantil', 'Platos para niños', 1),
('Platos Vegetarianos', 'Opciones vegetarianas', 1),
('Platos Veganos', 'Opciones sin derivados animales', 1),
('Promociones', 'Combos y promociones', 1),
('Bebidas Embotelladas', 'Agua, gaseosas y bebidas selladas', 1),
('Entradas', 'Entradas y piqueos', 1),
('Especiales de la Casa', 'Platos destacados de La Ermita', 1),
('Menú Ejecutivo', 'Opciones ejecutivas para almuerzo', 1),
('Salsas y Extras', 'Aderezos y complementos', 1);
GO

-- Más productos hasta completar 60 productos
DECLARE @p INT = 14;
DECLARE @cat INT;
DECLARE @precio DECIMAL(10,2);
DECLARE @costo DECIMAL(10,2);
DECLARE @stock INT;
WHILE @p <= 60
BEGIN
    SET @cat = ((@p - 1) % 30) + 1;
    SET @precio = 12 + ((@p % 25) * 2.50);
    SET @costo = 6 + ((@p % 15) * 1.25);
    SET @stock = 25 + (@p % 80);

    INSERT INTO Producto (idCategoria, codigo, nombre, descripcion, precio, costo, stock, stockMinimo, estado)
    VALUES (
        @cat,
        CONCAT('PRD-', RIGHT('000' + CAST(@p AS VARCHAR(3)), 3)),
        CONCAT('Producto Especial ', RIGHT('000' + CAST(@p AS VARCHAR(3)), 3)),
        CONCAT('Producto de cafetería/restaurante para pruebas número ', @p),
        @precio,
        @costo,
        @stock,
        5 + (@p % 6),
        1
    );
    SET @p += 1;
END
GO

-- Más proveedores hasta completar 30
DECLARE @pr INT = 4;
WHILE @pr <= 30
BEGIN
    INSERT INTO Proveedor (nombre, nit, telefono, email, direccion, contacto, estado)
    VALUES (
        CONCAT('Proveedor La Ermita ', RIGHT('00' + CAST(@pr AS VARCHAR(2)), 2)),
        CONCAT('900', RIGHT('000000' + CAST(@pr AS VARCHAR(6)), 6)),
        CONCAT('71', RIGHT('000000' + CAST(@pr AS VARCHAR(6)), 6)),
        CONCAT('proveedor', RIGHT('00' + CAST(@pr AS VARCHAR(2)), 2), '@laermita.local'),
        CONCAT('Av. Comercial Nro. ', @pr),
        CONCAT('Contacto ', RIGHT('00' + CAST(@pr AS VARCHAR(2)), 2)),
        1
    );
    SET @pr += 1;
END
GO

-- Más métodos de pago hasta completar 30
INSERT INTO MetodoPago (nombre, descripcion) VALUES
('Billetera Móvil', 'Pago por aplicación móvil'),
('Tigo Money', 'Pago mediante billetera Tigo Money'),
('Pago Simple', 'Pago electrónico simple'),
('Vale de Consumo', 'Vale interno del restaurante'),
('Crédito Cliente', 'Pago a crédito para cliente frecuente'),
('Débito Bancario', 'Débito bancario autorizado'),
('Depósito Bancario', 'Pago mediante depósito'),
('QR Banco Unión', 'Pago QR Banco Unión'),
('QR BNB', 'Pago QR BNB'),
('QR Mercantil', 'Pago QR Banco Mercantil'),
('QR Bisa', 'Pago QR Banco Bisa'),
('QR Fassil', 'Pago QR banco local'),
('Pago Mixto', 'Combinación de métodos de pago'),
('Cortesía', 'Consumo registrado como cortesía'),
('Gift Card', 'Tarjeta de regalo'),
('Cupón Promocional', 'Cupón de descuento o promoción'),
('Pago Web', 'Pago por enlace web'),
('Pago App', 'Pago desde aplicación'),
('Pago POS', 'Pago con terminal POS'),
('Pago Delivery', 'Pago asociado a entrega'),
('Pago Empresa', 'Pago facturado a empresa'),
('Pago Anticipado', 'Pago realizado con anticipación'),
('Pago Reserva', 'Pago por reserva'),
('Pago Evento', 'Pago para evento'),
('Pago Convenio', 'Pago por convenio institucional'),
('Otro', 'Otro método de pago');
GO

-- Más clientes hasta completar 30
DECLARE @cl INT = 4;
WHILE @cl <= 30
BEGIN
    INSERT INTO Cliente (nombre, documento, telefono, email, direccion, estado)
    VALUES (
        CONCAT('Cliente Frecuente ', RIGHT('00' + CAST(@cl AS VARCHAR(2)), 2)),
        CONCAT('800', RIGHT('0000' + CAST(@cl AS VARCHAR(4)), 4)),
        CONCAT('72', RIGHT('000000' + CAST(@cl AS VARCHAR(6)), 6)),
        CONCAT('cliente', RIGHT('00' + CAST(@cl AS VARCHAR(2)), 2), '@correo.local'),
        CONCAT('Zona Centro Calle ', @cl),
        1
    );
    SET @cl += 1;
END
GO

-- Más mesas hasta completar 30
DECLARE @m INT = 6;
WHILE @m <= 30
BEGIN
    INSERT INTO Mesa (numero, capacidad, estadoMesa, ubicacion, estado)
    VALUES (
        CONCAT('M-', RIGHT('00' + CAST(@m AS VARCHAR(2)), 2)),
        CASE WHEN @m % 5 = 0 THEN 8 WHEN @m % 3 = 0 THEN 6 WHEN @m % 2 = 0 THEN 4 ELSE 2 END,
        CASE WHEN @m % 10 = 0 THEN 'Reservada' WHEN @m % 7 = 0 THEN 'Ocupada' ELSE 'Libre' END,
        CASE WHEN @m % 4 = 0 THEN 'Terraza' WHEN @m % 3 = 0 THEN 'Ventana' ELSE 'Salon principal' END,
        1
    );
    SET @m += 1;
END
GO

-- Más reservas hasta completar 30
DECLARE @r INT = 2;
WHILE @r <= 30
BEGIN
    INSERT INTO Reserva (idCliente, idMesa, fechaReserva, horaReserva, cantidadPersonas, observacion, estadoReserva, estado)
    VALUES (
        ((@r - 1) % 30) + 1,
        ((@r - 1) % 30) + 1,
        CONVERT(date, DATEADD(day, @r, GETDATE())),
        CAST(DATEADD(minute, (@r % 6) * 30, CAST('18:00' AS time)) AS time(0)),
        2 + (@r % 5),
        CONCAT('Reserva generada para demostración número ', @r),
        CASE WHEN @r % 4 = 0 THEN 'Atendida' WHEN @r % 3 = 0 THEN 'Confirmada' ELSE 'Registrada' END,
        1
    );
    SET @r += 1;
END
GO

-- Más compras y detalles hasta completar 30 compras
DECLARE @co INT = 2;
DECLARE @idCompra INT;
DECLARE @prodCompra INT;
DECLARE @cantCompra INT;
DECLARE @costoCompra DECIMAL(10,2);
DECLARE @subtotalCompra DECIMAL(10,2);
DECLARE @stockAntesCompra INT;
DECLARE @stockDespuesCompra INT;
WHILE @co <= 30
BEGIN
    SET @prodCompra = ((@co + 12) % 60) + 1;
    SET @cantCompra = 5 + (@co % 8);
    SELECT @costoCompra = costo FROM Producto WHERE id = @prodCompra;
    SET @subtotalCompra = @cantCompra * @costoCompra;
    SELECT @stockAntesCompra = stock FROM Producto WHERE id = @prodCompra;

    INSERT INTO Compra (idProveedor, idUsuario, fecha, numeroDocumento, subtotal, impuesto, total, observacion, estado)
    VALUES (((@co - 1) % 30) + 1, ((@co - 1) % 30) + 1, DATEADD(day, -@co, SYSDATETIME()),
            CONCAT('FC-', RIGHT('0000' + CAST(@co AS VARCHAR(4)), 4)),
            @subtotalCompra, 0, @subtotalCompra, CONCAT('Compra de prueba ', @co), 1);

    SET @idCompra = SCOPE_IDENTITY();

    INSERT INTO DetalleCompra (idCompra, idProducto, cantidad, costo, subtotal, estado)
    VALUES (@idCompra, @prodCompra, @cantCompra, @costoCompra, @subtotalCompra, 1);

    SELECT @stockDespuesCompra = stock FROM Producto WHERE id = @prodCompra;
    INSERT INTO MovimientoInventario (idProducto, idUsuario, tipoMovimiento, origen, idReferencia, cantidad, stockAnterior, stockNuevo, observacion)
    VALUES (@prodCompra, 1, 'Entrada', 'Compra', @idCompra, @cantCompra, @stockAntesCompra, @stockDespuesCompra, CONCAT('Entrada automática por compra ', @idCompra));

    SET @co += 1;
END
GO

-- Más ventas y detalles hasta completar 30 ventas
DECLARE @ve INT = 2;
DECLARE @idVenta INT;
DECLARE @prodVenta INT;
DECLARE @cantVenta INT;
DECLARE @precioVenta DECIMAL(10,2);
DECLARE @subtotalVenta DECIMAL(10,2);
DECLARE @stockAntesVenta INT;
DECLARE @stockDespuesVenta INT;
WHILE @ve <= 30
BEGIN
    SET @prodVenta = ((@ve + 5) % 60) + 1;
    SET @cantVenta = 1 + (@ve % 3);
    SELECT @precioVenta = precio, @stockAntesVenta = stock FROM Producto WHERE id = @prodVenta;
    SET @subtotalVenta = @cantVenta * @precioVenta;

    INSERT INTO Venta (idUsuario, idCliente, idMesa, idMetodoPago, mesa, fecha, subtotal, descuento, impuesto, total, observacion, estado)
    VALUES (((@ve - 1) % 30) + 1, ((@ve - 1) % 30) + 1, ((@ve - 1) % 30) + 1, ((@ve - 1) % 30) + 1,
            CONCAT('Mesa ', ((@ve - 1) % 30) + 1), DATEADD(day, -(@ve % 20), SYSDATETIME()),
            @subtotalVenta, 0, 0, @subtotalVenta, CONCAT('Venta de prueba ', @ve), 1);

    SET @idVenta = SCOPE_IDENTITY();

    INSERT INTO DetalleVenta (idVenta, idProducto, cantidad, precio, descuento, subtotal, estado)
    VALUES (@idVenta, @prodVenta, @cantVenta, @precioVenta, 0, @subtotalVenta, 1);

    UPDATE Producto SET stock = stock - @cantVenta WHERE id = @prodVenta;
    SELECT @stockDespuesVenta = stock FROM Producto WHERE id = @prodVenta;

    UPDATE Inventario SET stockActual = @stockDespuesVenta, fechaActualizacion = SYSDATETIME()
    WHERE idProducto = @prodVenta;

    INSERT INTO MovimientoInventario (idProducto, idUsuario, tipoMovimiento, origen, idReferencia, cantidad, stockAnterior, stockNuevo, observacion)
    VALUES (@prodVenta, 1, 'Salida', 'Venta', @idVenta, @cantVenta, @stockAntesVenta, @stockDespuesVenta, CONCAT('Salida automática por venta ', @idVenta));

    SET @ve += 1;
END
GO

-- Historial de acciones para auditoría y reportes
DECLARE @ha INT = 1;
WHILE @ha <= 30
BEGIN
    INSERT INTO HistorialAccion (idUsuario, modulo, accion, descripcion, equipo)
    VALUES (((@ha - 1) % 30) + 1,
            CASE WHEN @ha % 6 = 0 THEN 'Ventas' WHEN @ha % 5 = 0 THEN 'Compras' WHEN @ha % 4 = 0 THEN 'Inventario' WHEN @ha % 3 = 0 THEN 'Productos' WHEN @ha % 2 = 0 THEN 'Clientes' ELSE 'Sistema' END,
            CASE WHEN @ha % 3 = 0 THEN 'Editar' WHEN @ha % 2 = 0 THEN 'Crear' ELSE 'Consultar' END,
            CONCAT('Acción de prueba registrada para auditoría número ', @ha),
            HOST_NAME());
    SET @ha += 1;
END
GO

PRINT 'Base de datos profesional LabLaErmita creada correctamente con datos ampliados.';
GO
