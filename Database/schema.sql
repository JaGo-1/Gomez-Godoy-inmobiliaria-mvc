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
