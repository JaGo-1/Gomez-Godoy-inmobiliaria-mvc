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
    EsMulta BOOLEAN NOT NULL DEFAULT FALSE,
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

-- Tabla Usuario
CREATE TABLE Usuario (
                          id SERIAL PRIMARY KEY,
                          nombre VARCHAR(100) NOT NULL,
                          apellido VARCHAR(100) NOT NULL,
                          email VARCHAR(100) UNIQUE NOT NULL,
                          password VARCHAR(255) NOT NULL,
                          avatar VARCHAR(255) DEFAULT '',
                          rol INTEGER NOT NULL
);
CREATE UNIQUE INDEX idx_usuarios_email ON Usuario (email);


CREATE TABLE Auditoria (
    id SERIAL PRIMARY KEY,
    entidad VARCHAR(100) NOT NULL,          
    entidad_id INT NOT NULL,               
    accion VARCHAR(50) NOT NULL,           
    usuario_id INT NOT NULL,             
    fecha TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    datos_anteriores JSONB,                 
    datos_nuevos JSONB,
    estado BOOLEAN DEFAULT TRUE                     
);
