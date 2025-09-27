-- ======================================
-- Estructura de la base de datos
-- Antes de ejecutar, crear la base de datos:
-- CREATE DATABASE Inmobiliariabd;
-- ======================================

-- Tabla Propietario
CREATE TABLE propietario (
    id SERIAL PRIMARY KEY,
    dni INT NOT NULL,
    nombre VARCHAR(50),
    apellido VARCHAR(50),
    telefono VARCHAR(20),
    email VARCHAR(50),
    clave VARCHAR(100),
    estado BOOLEAN
);

-- Tabla Inquilino
CREATE TABLE inquilino (
    idInquilino SERIAL PRIMARY KEY,
    dni VARCHAR(20) NOT NULL,
    nombre VARCHAR(50),
    apellido VARCHAR(50),
    telefono VARCHAR(20),
    email VARCHAR(50),
    estado BOOLEAN
);

-- Tabla Inmueble
CREATE TABLE Inmueble (
    Id SERIAL PRIMARY KEY,
    Direccion VARCHAR(255) NOT NULL,
    Precio DECIMAL(18,2) NOT NULL,
    Ambientes INT NOT NULL,
    Estado BOOLEAN NOT NULL,
    Latitud DOUBLE PRECISION NOT NULL,
    Longitud DOUBLE PRECISION NOT NULL,
    Uso VARCHAR(50) NOT NULL,
    Tipo VARCHAR(50) NOT NULL,
    PropietarioId INT NOT NULL,
    Portada VARCHAR(255),
    CONSTRAINT fk_propietario FOREIGN KEY (PropietarioId)
    REFERENCES Propietario(Id)
    ON DELETE CASCADE
);

-- Tabla Contrato
CREATE TABLE IF NOT EXISTS Contrato (
    Id SERIAL PRIMARY KEY,
    IdInmueble INTEGER NOT NULL,
    IdInquilino INTEGER NOT NULL,
    Monto DECIMAL(18,2) NOT NULL,
    Fecha_inicio DATE NOT NULL,
    Fecha_fin DATE NOT NULL,
    Estado BOOLEAN NOT NULL,
    Fecha_terminacion_anticipada DATE,
    Multa_calculada DECIMAL(18,2),
    CONSTRAINT fk_contrato_inmueble FOREIGN KEY (IdInmueble)
    REFERENCES Inmueble(Id)
    ON DELETE CASCADE,
    CONSTRAINT fk_contrato_inquilino FOREIGN KEY (IdInquilino)
    REFERENCES Inquilino(idInquilino)
    ON DELETE CASCADE
);

-- Tabla Pago
CREATE TABLE IF NOT EXISTS Pago (
    IdPago SERIAL PRIMARY KEY,
    ContratoId INTEGER NOT NULL,
    NumeroPago INTEGER NOT NULL,
    FechaEsperada DATE NOT NULL DEFAULT CURRENT_DATE,
    FechaPago DATE,
    Importe DECIMAL(18,2) NOT NULL,
    Detalle VARCHAR(255),
    Estado BOOLEAN NOT NULL DEFAULT TRUE,
    CONSTRAINT fk_pago_contrato FOREIGN KEY (ContratoId)
    REFERENCES Contrato(Id)
    ON DELETE CASCADE
);

-- Tabla Imagen
CREATE TABLE IF NOT EXISTS Imagen (
    Id SERIAL PRIMARY KEY,
    InmuebleId INT NOT NULL,
    Url VARCHAR(255) NOT NULL,
    CONSTRAINT fk_inmueble FOREIGN KEY (InmuebleId)
    REFERENCES Inmueble(Id)
    ON DELETE CASCADE
);
