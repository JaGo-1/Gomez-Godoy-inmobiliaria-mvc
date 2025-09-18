-- ======================================
-- Datos de prueba
-- ======================================

-- Vaciar tablas y reiniciar IDs
-- Vaciar tablas y reiniciar IDs
TRUNCATE TABLE pago RESTART IDENTITY CASCADE;
TRUNCATE TABLE contrato RESTART IDENTITY CASCADE;
TRUNCATE TABLE inmueble RESTART IDENTITY CASCADE;
TRUNCATE TABLE inquilino RESTART IDENTITY CASCADE;
TRUNCATE TABLE propietario RESTART IDENTITY CASCADE;

-- Insertar datos en Propietario
INSERT INTO propietario (dni, nombre, apellido, telefono, email, clave, estado)
VALUES
(30111222, 'Carlos', 'Pérez', '2664001000', 'carlos.perez@mail.com', 'clave123', TRUE),
(29222333, 'María', 'Gómez', '2664002000', 'maria.gomez@mail.com', 'pass456', TRUE),
(28444555, 'Jorge', 'Rodríguez', '2664003000', 'jorge.rodriguez@mail.com', 'abc789', TRUE),
(31111666, 'Lucía', 'Fernández', '2664004000', 'lucia.fernandez@mail.com', 'secreta', FALSE),
(32777888, 'Ana', 'Martínez', '2664005000', 'ana.martinez@mail.com', 'clave789', TRUE),
(29888999, 'Roberto', 'Sánchez', '2664006000', 'roberto.sanchez@mail.com', 'qwerty', TRUE),
(31222999, 'Paula', 'Díaz', '2664007000', 'paula.diaz@mail.com', 'password1', FALSE),
(32555333, 'Hernán', 'López', '2664008000', 'hernan.lopez@mail.com', 'test123', TRUE),
(33111444, 'Laura', 'Molina', '2664009000', 'laura.molina@mail.com', 'clave321', TRUE),
(34222555, 'Federico', 'Torres', '2664010000', 'federico.torres@mail.com', 'asdf123', TRUE),
(35444777, 'Andrea', 'Vega', '2664011000', 'andrea.vega@mail.com', 'zxcv456', TRUE),
(36111999, 'Marcelo', 'Acosta', '2664012000', 'marcelo.acosta@mail.com', 'mypass', TRUE),
(37222000, 'Claudia', 'Navarro', '2664013000', 'claudia.navarro@mail.com', 'clave555', FALSE),
(38444111, 'Esteban', 'Méndez', '2664014000', 'esteban.mendez@mail.com', 'test456', TRUE),
(39111222, 'Verónica', 'Suárez', '2664015000', 'veronica.suarez@mail.com', '123abc', TRUE),
(40222333, 'Gustavo', 'Castillo', '2664016000', 'gustavo.castillo@mail.com', 'clave444', TRUE),
(41444555, 'Natalia', 'Ramos', '2664017000', 'natalia.ramos@mail.com', 'qweasd', TRUE),
(42111666, 'Ricardo', 'Ibarra', '2664018000', 'ricardo.ibarra@mail.com', 'clave111', TRUE),
(43222777, 'Patricia', 'Campos', '2664019000', 'patricia.campos@mail.com', 'secret123', TRUE),
(44444999, 'Sergio', 'Moreno', '2664020000', 'sergio.moreno@mail.com', 'pass999', TRUE),
(45111000, 'Gabriela', 'Aguilar', '2664021000', 'gabriela.aguilar@mail.com', 'clave777', TRUE),
(46222111, 'Matías', 'Ponce', '2664022000', 'matias.ponce@mail.com', 'pass111', TRUE),
(47444333, 'Liliana', 'Ortega', '2664023000', 'liliana.ortega@mail.com', 'clave222', FALSE),
(48111444, 'Diego', 'Domínguez', '2664024000', 'diego.dominguez@mail.com', 'mypass2', TRUE),
(49222555, 'Silvina', 'Rey', '2664025000', 'silvina.rey@mail.com', 'test999', TRUE);

-- Insertar datos en Inquilino
INSERT INTO inquilino (dni, nombre, apellido, telefono, email, estado)
VALUES
('40111222', 'Sofía', 'Gutiérrez', '2664101000', 'sofia.gutierrez@mail.com', TRUE),
('39222333', 'Martín', 'Alonso', '2664102000', 'martin.alonso@mail.com', TRUE),
('38444555', 'Camila', 'Ruiz', '2664103000', 'camila.ruiz@mail.com', FALSE),
('41111666', 'Diego', 'Castro', '2664104000', 'diego.castro@mail.com', TRUE),
('42777888', 'Valentina', 'Morales', '2664105000', 'valentina.morales@mail.com', TRUE),
('39888999', 'Nicolás', 'Romero', '2664106000', 'nicolas.romero@mail.com', TRUE),
('41222999', 'Julieta', 'Silva', '2664107000', 'julieta.silva@mail.com', FALSE),
('42555333', 'Gonzalo', 'Herrera', '2664108000', 'gonzalo.herrera@mail.com', TRUE),
('43111444', 'Pablo', 'Mendoza', '2664109000', 'pablo.mendoza@mail.com', TRUE),
('44222555', 'Luciana', 'Peralta', '2664110000', 'luciana.peralta@mail.com', TRUE),
('45444777', 'Ramiro', 'Juárez', '2664111000', 'ramiro.juarez@mail.com', TRUE),
('46111999', 'Milagros', 'Paredes', '2664112000', 'milagros.paredes@mail.com', FALSE),
('47222000', 'Tomás', 'Figueroa', '2664113000', 'tomas.figueroa@mail.com', TRUE),
('48444111', 'Florencia', 'Villalba', '2664114000', 'florencia.villalba@mail.com', TRUE),
('49111222', 'Leandro', 'Cabrera', '2664115000', 'leandro.cabrera@mail.com', TRUE),
('50222333', 'Romina', 'Arias', '2664116000', 'romina.arias@mail.com', TRUE),
('51444555', 'Agustín', 'Molina', '2664117000', 'agustin.molina@mail.com', TRUE),
('52111666', 'Cecilia', 'Luna', '2664118000', 'cecilia.luna@mail.com', TRUE),
('53222777', 'Franco', 'Benítez', '2664119000', 'franco.benitez@mail.com', TRUE),
('54444999', 'Carolina', 'Medina', '2664120000', 'carolina.medina@mail.com', FALSE),
('55111000', 'Maximiliano', 'Olivera', '2664121000', 'maxi.olivera@mail.com', TRUE),
('56222111', 'Daniela', 'Sosa', '2664122000', 'daniela.sosa@mail.com', TRUE),
('57444333', 'Alejandro', 'Arce', '2664123000', 'alejandro.arce@mail.com', TRUE),
('58111444', 'Malena', 'Nuñez', '2664124000', 'malena.nunez@mail.com', TRUE),
('59222555', 'Iván', 'Coria', '2664125000', 'ivan.coria@mail.com', TRUE);

-- Insertar datos en Inmueble
INSERT INTO inmueble (Direccion, Precio, Ambientes, Estado, Latitud, Longitud, Uso, Tipo, PropietarioId)
VALUES
('San Martín 123', 45000, 3, TRUE, -33.301, -66.336, 'Residencial', 'Departamento', 1),
('Belgrano 456', 60000, 4, TRUE, -33.310, -66.337, 'Residencial', 'Casa', 2),
('Rivadavia 789', 38000, 2, TRUE, -33.312, -66.340, 'Comercial', 'Local', 3),
('Mitre 150', 70000, 5, TRUE, -33.320, -66.342, 'Residencial', 'Casa', 4),
('Colón 200', 52000, 3, FALSE, -33.325, -66.345, 'Residencial', 'Departamento', 5),
('Pringles 333', 55000, 3, TRUE, -33.330, -66.350, 'Comercial', 'Local', 6),
('Junín 400', 48000, 2, TRUE, -33.335, -66.352, 'Residencial', 'Departamento', 7),
('Ayacucho 500', 75000, 4, TRUE, -33.340, -66.355, 'Residencial', 'Casa', 8),
('Ituzaingó 600', 46000, 2, TRUE, -33.345, -66.357, 'Residencial', 'Departamento', 9),
('Chacabuco 700', 80000, 5, TRUE, -33.350, -66.360, 'Residencial', 'Casa', 10),
('Junín 800', 39000, 1, TRUE, -33.355, -66.365, 'Comercial', 'Local', 11),
('Bolívar 900', 47000, 3, TRUE, -33.360, -66.370, 'Residencial', 'Departamento', 12),
('Maipú 950', 65000, 4, TRUE, -33.365, -66.372, 'Residencial', 'Casa', 13),
('Av. España 1100', 41000, 2, TRUE, -33.370, -66.374, 'Comercial', 'Local', 14),
('San Juan 1200', 56000, 3, TRUE, -33.375, -66.376, 'Residencial', 'Departamento', 15),
('Illia 1300', 73000, 5, TRUE, -33.380, -66.380, 'Residencial', 'Casa', 16),
('Lafinur 1400', 49500, 2, TRUE, -33.385, -66.382, 'Residencial', 'Departamento', 17),
('Riobamba 1500', 54000, 3, FALSE, -33.390, -66.384, 'Comercial', 'Local', 18),
('Balcarce 1600', 62000, 4, TRUE, -33.395, -66.386, 'Residencial', 'Casa', 19),
('España 1700', 37000, 1, TRUE, -33.400, -66.388, 'Residencial', 'Departamento', 20),
('Caseros 1800', 58000, 3, TRUE, -33.405, -66.390, 'Residencial', 'Casa', 21),
('Lavalle 1900', 44500, 2, TRUE, -33.410, -66.392, 'Comercial', 'Local', 22),
('Catamarca 2000', 72000, 5, TRUE, -33.415, -66.394, 'Residencial', 'Casa', 23),
('Mendoza 2100', 40000, 2, TRUE, -33.420, -66.396, 'Residencial', 'Departamento', 24),
('Buenos Aires 2200', 67000, 4, TRUE, -33.425, -66.398, 'Residencial', 'Casa', 25);

-- Insertar datos en Contrato
INSERT INTO contrato (IdInmueble, IdInquilino, Monto, Fecha_inicio, Fecha_fin, Estado, Fecha_terminacion_anticipada, Multa_calculada)
VALUES
(1, 1, 45000, '2023-01-01', '2023-12-31', TRUE, NULL, NULL),
(2, 2, 60000, '2023-02-01', '2024-01-31', TRUE, NULL, NULL),
(3, 3, 38000, '2023-03-01', '2024-02-29', TRUE, NULL, NULL),
(4, 4, 70000, '2023-04-01', '2024-03-31', TRUE, NULL, NULL),
(5, 5, 52000, '2023-05-01', '2024-04-30', FALSE, '2023-11-15', 5000),
(6, 6, 55000, '2023-06-01', '2024-05-31', TRUE, NULL, NULL),
(7, 7, 48000, '2023-07-01', '2024-06-30', TRUE, NULL, NULL),
(8, 8, 75000, '2023-08-01', '2024-07-31', TRUE, NULL, NULL),
(9, 9, 46000, '2023-09-01', '2024-08-31', TRUE, NULL, NULL),
(10, 10, 80000, '2023-10-01', '2024-09-30', TRUE, NULL, NULL),
(11, 11, 39000, '2023-11-01', '2024-10-31', TRUE, NULL, NULL),
(12, 12, 47000, '2023-12-01', '2024-11-30', TRUE, NULL, NULL),
(13, 13, 65000, '2024-01-01', '2024-12-31', TRUE, NULL, NULL),
(14, 14, 41000, '2024-02-01', '2025-01-31', TRUE, NULL, NULL),
(15, 15, 56000, '2024-03-01', '2025-02-28', TRUE, NULL, NULL),
(16, 16, 73000, '2024-04-01', '2025-03-31', TRUE, NULL, NULL),
(17, 17, 49500, '2024-05-01', '2025-04-30', TRUE, NULL, NULL),
(18, 18, 54000, '2024-06-01', '2025-05-31', FALSE, '2024-12-15', 7000),
(19, 19, 62000, '2024-07-01', '2025-06-30', TRUE, NULL, NULL),
(20, 20, 37000, '2024-08-01', '2025-07-31', TRUE, NULL, NULL),
(21, 21, 58000, '2024-09-01', '2025-08-31', TRUE, NULL, NULL),
(22, 22, 44500, '2024-10-01', '2025-09-30', TRUE, NULL, NULL),
(23, 23, 72000, '2024-11-01', '2025-10-31', TRUE, NULL, NULL),
(24, 24, 40000, '2024-12-01', '2025-11-30', TRUE, NULL, NULL),
(25, 25, 67000, '2025-01-01', '2025-12-31', TRUE, NULL, NULL);

-- Insertar datos en Pago
INSERT INTO pago (ContratoId, NumeroPago, FechaEsperada, FechaPago, Importe, Detalle, Estado)
VALUES
(1, 1, '2023-02-01', '2023-02-01', 45000, 'Pago inicial contrato 1', TRUE),
(2, 1, '2023-03-01', '2023-03-02', 60000, 'Pago inicial contrato 2', TRUE),
(3, 1, '2023-04-01', '2023-04-01', 38000, 'Pago inicial contrato 3', TRUE),
(4, 1, '2023-05-01', '2023-05-01', 70000, 'Pago inicial contrato 4', TRUE),
(5, 1, '2023-06-01', '2023-06-15', 52000, 'Pago inicial contrato 5', FALSE),
(6, 1, '2023-07-01', '2023-07-01', 55000, 'Pago inicial contrato 6', TRUE),
(7, 1, '2023-08-01', '2023-08-02', 48000, 'Pago inicial contrato 7', TRUE),
(8, 1, '2023-09-01', '2023-09-01', 75000, 'Pago inicial contrato 8', TRUE),
(9, 1, '2023-10-01', '2023-10-01', 46000, 'Pago inicial contrato 9', TRUE),
(10, 1, '2023-11-01', '2023-11-01', 80000, 'Pago inicial contrato 10', TRUE),
(11, 1, '2023-12-01', '2023-12-02', 39000, 'Pago inicial contrato 11', TRUE),
(12, 1, '2024-01-01', '2024-01-01', 47000, 'Pago inicial contrato 12', TRUE),
(13, 1, '2024-02-01', '2024-02-01', 65000, 'Pago inicial contrato 13', TRUE),
(14, 1, '2024-03-01', '2024-03-01', 41000, 'Pago inicial contrato 14', TRUE),
(15, 1, '2024-04-01', '2024-04-01', 56000, 'Pago inicial contrato 15', TRUE),
(16, 1, '2024-05-01', '2024-05-01', 73000, 'Pago inicial contrato 16', TRUE),
(17, 1, '2024-06-01', '2024-06-01', 49500, 'Pago inicial contrato 17', TRUE),
(18, 1, '2024-07-01', '2024-07-15', 54000, 'Pago inicial contrato 18', FALSE),
(19, 1, '2024-08-01', '2024-08-01', 62000, 'Pago inicial contrato 19', TRUE),
(20, 1, '2024-09-01', '2024-09-01', 37000, 'Pago inicial contrato 20', TRUE),
(21, 1, '2024-10-01', '2024-10-01', 58000, 'Pago inicial contrato 21', TRUE),
(22, 1, '2024-11-01', '2024-11-02', 44500, 'Pago inicial contrato 22', TRUE),
(23, 1, '2024-12-01', '2024-12-01', 72000, 'Pago inicial contrato 23', TRUE),
(24, 1, '2025-01-01', '2025-01-01', 40000, 'Pago inicial contrato 24', TRUE),
(25, 1, '2025-02-01', '2025-02-01', 67000, 'Pago inicial contrato 25', TRUE);
