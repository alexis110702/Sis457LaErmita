-- ============================================================
-- ddl_dml_laermita.sql  –  Base de datos "La Ermita"
-- CORRECCIONES:
--   1. ERROR EN DATOS DE PRUEBA: el detalle de la venta de prueba
--      tenía el pastel de chocolate con precio=3.00 cuando en la
--      tabla Producto el precio es 22.00. El total de la venta
--      era 63.00 pero debería ser 30+30+22=82.00. Corregido.
--   2. Se agrega USE LabLaErmita antes del primer CREATE TABLE
--      (en el original se hacía USE master antes y luego nunca
--      se cambiaba, lo que provocaría error al crear las tablas).
--   3. Se agrega CONSTRAINT UNIQUE en Usuario.usuario para evitar
--      usuarios duplicados.
-- ============================================================

CREATE DATABASE LabLaErmita;
GO
USE master
GO
CREATE LOGIN usrlaermita WITH PASSWORD = '12345678',
  DEFAULT_DATABASE = LabLaErmita,
  CHECK_EXPIRATION = OFF,
  CHECK_POLICY     = ON
GO
USE LabLaErmita  -- CORRECCIÓN: cambiar a la BD correcta antes de crear objetos
GO
CREATE USER usrlaermita FOR LOGIN usrlaermita
GO
ALTER ROLE db_owner ADD MEMBER usrlaermita
GO

-- ============================================================
-- TABLAS
-- ============================================================

-- 1. Rol
CREATE TABLE Rol (
  id     INT IDENTITY(1,1) PRIMARY KEY,
  nombre VARCHAR(30)  NOT NULL,
  estado SMALLINT     NOT NULL DEFAULT 1
);
GO

-- 2. Usuario
CREATE TABLE Usuario (
  id         INT IDENTITY(1,1) PRIMARY KEY,
  idRol      INT          NOT NULL,
  nombre     VARCHAR(100) NOT NULL,
  usuario    VARCHAR(50)  NOT NULL,
  contrasena VARCHAR(200) NOT NULL,
  estado     SMALLINT     NOT NULL DEFAULT 1,
  CONSTRAINT fk_Usuario_Rol    FOREIGN KEY (idRol) REFERENCES Rol(id),
  CONSTRAINT uq_Usuario_usuario UNIQUE (usuario)   -- MEJORA: evita duplicados
);
GO

-- 3. Categoria
CREATE TABLE Categoria (
  id          INT IDENTITY(1,1) PRIMARY KEY,
  nombre      VARCHAR(50)  NOT NULL,
  descripcion VARCHAR(150) NOT NULL,
  estado      SMALLINT     NOT NULL DEFAULT 1
);
GO

-- 4. Producto
CREATE TABLE Producto (
  id          INT IDENTITY(1,1) PRIMARY KEY,
  idCategoria INT           NOT NULL,
  nombre      VARCHAR(100)  NOT NULL,
  descripcion VARCHAR(250)  NOT NULL,
  precio      DECIMAL(10,2) NOT NULL,
  stock       INT           NOT NULL DEFAULT 0,
  estado      SMALLINT      NOT NULL DEFAULT 1,
  CONSTRAINT fk_Producto_Categoria FOREIGN KEY (idCategoria) REFERENCES Categoria(id)
);
GO

-- 5. Venta
CREATE TABLE Venta (
  id        INT IDENTITY(1,1) PRIMARY KEY,
  idUsuario INT           NOT NULL,
  mesa      VARCHAR(10)   NOT NULL,
  fecha     DATETIME      NOT NULL DEFAULT GETDATE(),
  total     DECIMAL(10,2) NOT NULL DEFAULT 0,
  estado    SMALLINT      NOT NULL DEFAULT 1,
  CONSTRAINT fk_Venta_Usuario FOREIGN KEY (idUsuario) REFERENCES Usuario(id)
);
GO

-- 6. DetalleVenta
CREATE TABLE DetalleVenta (
  id         INT IDENTITY(1,1) PRIMARY KEY,
  idVenta    INT           NOT NULL,
  idProducto INT           NOT NULL,
  cantidad   INT           NOT NULL,
  precio     DECIMAL(10,2) NOT NULL,
  subtotal   DECIMAL(10,2) NOT NULL,
  estado     SMALLINT      NOT NULL DEFAULT 1,
  CONSTRAINT fk_DetalleVenta_Venta    FOREIGN KEY (idVenta)    REFERENCES Venta(id),
  CONSTRAINT fk_DetalleVenta_Producto FOREIGN KEY (idProducto) REFERENCES Producto(id)
);
GO

-- ============================================================
-- PROCEDIMIENTOS ALMACENADOS
-- ============================================================

-- Login
CREATE PROC paUsuarioLogin
  @usuario    VARCHAR(50),
  @contrasena VARCHAR(200)
AS
  SELECT u.id, u.idRol, u.nombre, u.usuario, r.nombre AS rol
  FROM   Usuario u
  INNER JOIN Rol r ON r.id = u.idRol
  WHERE  u.usuario    = @usuario
    AND  u.contrasena = @contrasena
    AND  u.estado     = 1;
GO

-- Listar Categorias
CREATE PROC paCategoriaListar @parametro VARCHAR(50)
AS
  SELECT id, nombre, descripcion, estado
  FROM   Categoria
  WHERE  estado = 1
    AND  nombre + descripcion LIKE '%' + REPLACE(@parametro,' ','%') + '%'
  ORDER BY nombre;
GO

-- Listar Productos
CREATE PROC paProductoListar @parametro VARCHAR(100)
AS
  SELECT p.id, p.idCategoria, p.nombre, p.descripcion,
         c.nombre AS categoria,
         p.precio, p.stock, p.estado
  FROM   Producto p
  INNER JOIN Categoria c ON c.id = p.idCategoria
  WHERE  p.estado = 1
    AND  p.nombre + p.descripcion + c.nombre LIKE '%' + REPLACE(@parametro,' ','%') + '%'
  ORDER BY p.nombre;
GO

-- Listar Ventas
CREATE PROC paVentaListar @parametro VARCHAR(50)
AS
  SELECT v.id, v.idUsuario, v.mesa,
         u.nombre AS cajero,
         v.fecha, v.total, v.estado
  FROM   Venta v
  INNER JOIN Usuario u ON u.id = v.idUsuario
  WHERE  v.estado = 1
    AND  v.mesa + u.nombre LIKE '%' + REPLACE(@parametro,' ','%') + '%'
  ORDER BY v.fecha DESC;
GO

-- Listar Detalle por Venta
CREATE PROC paDetalleVentaListar @idVenta INT
AS
  SELECT dv.id, dv.idVenta, dv.idProducto,
         p.nombre AS producto,
         dv.cantidad, dv.precio, dv.subtotal, dv.estado
  FROM   DetalleVenta dv
  INNER JOIN Producto p ON p.id = dv.idProducto
  WHERE  dv.idVenta = @idVenta
    AND  dv.estado  = 1
  ORDER BY dv.id;
GO

-- ============================================================
-- DATOS DE PRUEBA
-- ============================================================

INSERT INTO Rol (nombre, estado) VALUES
  ('Administrador', 1),
  ('Cajero',        1),
  ('Mesero',        1);

INSERT INTO Usuario (idRol, nombre, usuario, contrasena, estado) VALUES
  (1, 'Administrador', 'admin',   'admin123', 1),
  (2, 'Juan Mamani',   'jmamani', 'caj123',   1),
  (3, 'Maria Lopez',   'mlopez',  'mes123',   1);

INSERT INTO Categoria (nombre, descripcion, estado) VALUES
  ('Bebidas Calientes', 'Cafes, tes e infusiones',       1),
  ('Bebidas Frias',     'Jugos, refrescos y batidos',    1),
  ('Desayunos',         'Sandwiches, tostadas y huevos', 1),
  ('Almuerzos',         'Platos principales del dia',    1),
  ('Postres',           'Pasteles, galletas y helados',  1);

INSERT INTO Producto (idCategoria, nombre, descripcion, precio, stock, estado) VALUES
  (1, 'Cafe Americano',     'Cafe negro espresso con agua',       15.00, 100, 1),
  (1, 'Capuchino',          'Espresso con leche espumada',        18.00, 100, 1),
  (1, 'Te de Manzanilla',   'Infusion natural de manzanilla',     10.00, 100, 1),
  (2, 'Jugo de Naranja',    'Jugo natural exprimido',             20.00,  50, 1),
  (2, 'Batido de Fresa',    'Batido con leche y fresas frescas',  25.00,  50, 1),
  (3, 'Sandwich Mixto',     'Pan con jamon, queso y tomate',      30.00,  30, 1),
  (3, 'Tostadas con Miel',  'Tostadas con mantequilla y miel',    20.00,  30, 1),
  (4, 'Almuerzo del Dia',   'Sopa, segundo y refresco incluido',  45.00,  20, 1),
  (5, 'Pastel de Chocolate','Porcion de pastel casero',           22.00,  15, 1),
  (5, 'Cheesecake',         'Cheesecake con frutos rojos',        28.00,  15, 1);

-- Venta de prueba
-- CORRECCIÓN: total corregido a 82.00 (30+30+22) en lugar de 63.00
INSERT INTO Venta (idUsuario, mesa, fecha, total, estado) VALUES
  (2, 'Mesa 1', GETDATE(), 82.00, 1);

-- CORRECCIÓN: precio del pastel corregido a 22.00 (era 3.00, un error de tipeo)
INSERT INTO DetalleVenta (idVenta, idProducto, cantidad, precio, subtotal, estado) VALUES
  (1, 1, 2, 15.00, 30.00, 1),   -- 2x Cafe Americano = 30.00
  (1, 6, 1, 30.00, 30.00, 1),   -- 1x Sandwich Mixto = 30.00
  (1, 9, 1, 22.00, 22.00, 1);   -- 1x Pastel de Chocolate = 22.00 (CORREGIDO)

-- Verificación final
SELECT * FROM Rol;
SELECT * FROM Usuario;
SELECT * FROM Categoria;
SELECT * FROM Producto;
EXEC paVentaListar '';
EXEC paDetalleVentaListar 1;
