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
