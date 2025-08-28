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
    CONSTRAINT fk_propietario FOREIGN KEY (PropietarioId)
    REFERENCES Propietario(Id)
    ON DELETE CASCADE
);

