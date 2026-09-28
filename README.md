# ControlInventario

Sistema de inventario que permite a una empresa **registrar, controlar y gestionar de forma eficiente el ingreso, salida y stock de productos** en uno o varios almacenes.

## Descripción del proyecto

ControlInventario es una aplicación full-stack compuesta por una API REST construida en **.NET 8** y un frontend **SPA en Vue 3**, con una base de datos **SQL Server** cuyo script se encuentra íntegramente en español.

### Módulos principales

- **Gestión de productos:** alta, baja y modificación de productos con código, categoría, proveedor, precios, unidad de medida y stock mínimo.
- **Control de inventario:** registro de entradas y salidas con actualización automática del stock y notificaciones por stock bajo.
- **Categorías y proveedores:** CRUD completo con datos de contacto.
- **Movimientos de inventario:** cada movimiento registra fecha, tipo (entrada/salida), motivo, usuario responsable y puede asociarse a una factura o documento.
- **Reportes y consultas:** stock actual, movimientos por producto/fecha/usuario y productos con stock crítico.
- **Usuarios y roles:** autenticación con JWT y control de acceso por rol (Administrador, Usuario, Auditor).

## Tecnologías utilizadas

| Capa | Tecnología |
|---|---|
| Frontend | Vue 3, Vite, Vue Router, Pinia, Axios, Tailwind CSS, iconos Lucide |
| Backend | .NET 8 LTS, ASP.NET Core Web API, Entity Framework Core, ASP.NET Core Identity, JWT |
| Base de datos | SQL Server (script en español) |
| Pruebas | xUnit, Moq, FluentAssertions |
| Herramientas | Git, Node.js |

## Estructura del proyecto

```
controlinventario/
├── backend/
│   ├── ControlInventario.sln
│   ├── src/
│   │   ├── ControlInventario.API/           → Controllers, Program.cs, middleware
│   │   ├── ControlInventario.Application/   → Services, interfaces, DTOs, validadores
│   │   ├── ControlInventario.Domain/        → Entidades, enums, contratos de repositorio
│   │   └── ControlInventario.Infrastructure/→ Repositorios, DbContext, migraciones
│   └── tests/
│       └── ControlInventario.Tests/         → Pruebas unitarias (xUnit + Moq)
├── frontend/                                → Aplicación Vue 3
├── database/                                → Script SQL en español
└── README.md
```

## Requisitos

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server 2019 o superior](https://www.microsoft.com/es-es/sql-server/sql-server-downloads) (o SQL Server Express/LocalDB)
- [Node.js 18+](https://nodejs.org/)

## Instrucciones para correr la aplicación

### 1. Base de datos

Abrir **SQL Server Management Studio** (o `sqlcmd`) y ejecutar el script:

```text
database/ControlInventario.sql
```

El script crea la base de datos `controlinventario`, todas las tablas en español y los datos de inicio (roles, usuario administrador, almacén, categorías y proveedores de ejemplo).

### 2. Backend (API)

```bash
cd backend
# Ajustar la cadena de conexión en src/ControlInventario.API/appsettings.json
dotnet restore
dotnet run --project src/ControlInventario.API
```

La API quedará disponible en `https://localhost:7xxx/` (puerto indicado al iniciar).

**Pruebas unitarias:**

```bash
dotnet test backend/ControlInventario.sln
```

### 3. Frontend (Vue)

```bash
cd frontend
npm install
npm run dev
```

El frontend quedará disponible en `http://localhost:5173`.

### Credenciales de inicio

| Usuario | Contraseña | Rol |
|---|---|---|
| `admin` | `Admin123*` | Administrador |

> Cambiar la contraseña después del primer inicio de sesión.

## Roles y permisos

| Rol | Permisos |
|---|---|
| Administrador | Todo: CRUD completo, movimientos, reportes y gestión de usuarios |
| Usuario | Consultar productos y registrar movimientos (entradas/salidas) |
| Auditor | Solo lectura: consultas y reportes |

## Licencia

Proyecto académico/privado.
