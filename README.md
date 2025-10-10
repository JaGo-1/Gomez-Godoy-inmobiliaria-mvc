# Proyecto Inmobiliaria MVC

## Descripción

Sistema de **gestión de alquileres** desarrollado en **ASP.NET** con **PostgreSQL** como base de datos.  
Permite administrar **propietarios, inquilinos, inmuebles, contratos y pagos**, además de gestionar **usuarios, imágenes** y un **registro de auditoría** para el seguimiento de cambios.  
Incluye funcionalidades de **ABM completo**, control de **contratos activos**, **pagos y multas**, y **seguimiento histórico** de modificaciones.

![DER Propietarios e Inquilinos](Diagrams/DER_Inmobiliaria.png)

## Herramientas

- [PostgreSQL](https://www.postgresql.org/download/) (DBMS)
- [pgAdmin](https://www.pgadmin.org/download/) (opcional, para administrar la base de datos)
- [.NET SDK](https://dotnet.microsoft.com/download) (para ejecutar la aplicación ASP.NET Core)

## Instalación y uso

### 1. Clonar el repositorio

```bash
git clone https://github.com/JaGo-1/Gomez-Godoy-inmobiliaria-mvc.git
cd Gomez-Godoy-inmobiliaria-mvc
```

### 2. Crear la base de datos

```bash
CREATE DATABASE inmobiliariabd;
```

### 3. Crear la estructura de tablas

- Abrir Database/schema.sql.
- Ejecutar el script para crear las tablas vacías.

### 4. Cargar datos de prueba (opcional)

- Abrir Database/seeder.sql.
- Ejecutar el script para insertar datos simulación y reiniciar los IDs de las tablas.

### 5. Ejecutar la aplicación

```bash
dotnet run
```

### Integrantes

- [Jacqueline Estefania Gomez](https://github.com/JaGo-1)
- [Santiago Godoy](https://github.com/SantiMGodoy)
