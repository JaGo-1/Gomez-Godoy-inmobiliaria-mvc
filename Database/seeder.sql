-- ======================================
-- Datos de prueba
-- ======================================

-- Vaciar tablas y reiniciar IDs
TRUNCATE TABLE propietario RESTART IDENTITY CASCADE;
TRUNCATE TABLE inquilino RESTART IDENTITY CASCADE;

-- Insertar datos en Propietario
INSERT INTO propietario (dni, nombre, apellido, telefono, email, clave, estado)
VALUES
('30111222', 'Carlos', 'Pérez', '2664001000', 'carlos.perez@mail.com', 'clave123', TRUE),
('29222333', 'María', 'Gómez', '2664002000', 'maria.gomez@mail.com', 'pass456', TRUE),
('28444555', 'Jorge', 'Rodríguez', '2664003000', 'jorge.rodriguez@mail.com', 'abc789', TRUE),
('31111666', 'Lucía', 'Fernández', '2664004000', 'lucia.fernandez@mail.com', 'secreta', FALSE),
('32777888', 'Ana', 'Martínez', '2664005000', 'ana.martinez@mail.com', 'clave789', TRUE),
('29888999', 'Roberto', 'Sánchez', '2664006000', 'roberto.sanchez@mail.com', 'qwerty', TRUE),
('31222999', 'Paula', 'Díaz', '2664007000', 'paula.diaz@mail.com', 'password1', FALSE),
('32555333', 'Hernán', 'López', '2664008000', 'hernan.lopez@mail.com', 'test123', TRUE);

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
('42555333', 'Gonzalo', 'Herrera', '2664108000', 'gonzalo.herrera@mail.com', TRUE);
