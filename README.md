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

El orden recomendado es: **base de datos → backend → frontend**.

### 1. Base de datos

El script `database/ControlInventario.sql` crea la base de datos **`controlinventario`**, las 9 tablas con nombres y columnas en español, la vista `V_ProductosStockCritico` y los datos de inicio:

- 3 roles (Administrador, Usuario, Auditor) y el usuario `admin`.
- 2 almacenes, 4 categorías, 3 proveedores y 6 productos con su stock.
- 3 movimientos de ejemplo.

**Opción A — SQL Server Management Studio (recomendada):** abrir SSMS y ejecutar el script completo.

**Opción B — `sqlcmd` (importante):** el script está codificado en **UTF-8 con BOM** para conservar los acentos. Ejecutarlo con:

```bash
sqlcmd -S localhost -E -i database/ControlInventario.sql -f 65001
```

> `-f 65001` fuerza la página de códigos UTF-8; sin él, `sqlcmd` puede corromper los textos con acentos (á, é, í, ó, ú, ñ).

Si SQL Server usa autenticación mixta, reemplazar `-E` por `-U sa -P <contraseña>`.

### 2. Backend (API)

Revisar la cadena de conexión en `backend/src/ControlInventario.API/appsettings.json`:

```json
"ConexionPredeterminada": "Server=localhost;Database=controlinventario;Trusted_Connection=True;TrustServerCertificate=True"
```

Luego compilar y ejecutar:

```bash
cd backend
dotnet restore
dotnet run --project src/ControlInventario.API --launch-profile http
```

La API quedará disponible en **`http://localhost:5087`** y la documentación Swagger en `http://localhost:5087/swagger`.

**Pruebas unitarias** (39 pruebas):

```bash
dotnet test backend/ControlInventario.sln
```

### 3. Frontend (Vue)

```bash
cd frontend
npm install
npm run dev
```

El frontend quedará disponible en **`http://localhost:5173`**. Vite está configurado para proxyar las llamadas `/api/*` hacia `http://localhost:5087`, por lo que **el backend debe estar ejecutándose** antes de iniciar sesión.

Verificación rápida de la aplicación en ejecución:

```bash
# Build de producción (opcional)
cd frontend && npm run build
```

### Credenciales de inicio

| Usuario | Contraseña | Rol |
|---|---|---|
| `admin` | `Admin123*` | Administrador |

> Cambiar la contraseña después del primer inicio de sesión. Para probar los otros roles, crear usuarios desde la vista **Usuarios** (solo el Administrador puede hacerlo) y asignarles el rol `Usuario` o `Auditor`.

## Roles y permisos

| Rol | Permisos |
|---|---|
| Administrador | Todo: CRUD completo, movimientos, reportes y gestión de usuarios |
| Usuario | Consultar productos y registrar movimientos (entradas/salidas) |
| Auditor | Solo lectura: consultas y reportes |

## Pantallas principales

| Pantalla | Contenido |
|---|---|
| Inicio de sesión | Autenticación con JWT |
| Panel principal | KPIs (productos, valor de inventario, movimientos del mes, stock crítico), alertas y últimos movimientos |
| Productos | Búsqueda con filtros por categoría/proveedor, CRUD, badge de estado de stock y detalle por almacén |
| Categorías / Proveedores / Almacenes | CRUD con baja lógica e inclusión de inactivos |
| Movimientos | Listado con filtros (producto, tipo, rango de fechas), registro de entradas/salidas con detalle de líneas y vista de detalle con documento/factura y usuario responsable |
| Reportes | Pestañas de stock actual, movimientos y stock crítico, con filtros, exportación a CSV e impresión |
| Usuarios | Alta/edición/baja de usuarios y asignación de roles (solo Administrador) |

La interfaz se adapta al rol: los botones de alta/modificación/baja solo aparecen para el Administrador, y el registro de movimientos está disponible para Administrador y Usuario.

## Verificación funcional

Flujo recomendado para comprobar el sistema de punta a punta:

1. Iniciar sesión con `admin` / `Admin123*`.
2. Revisar el panel: KPIs, alertas de stock crítico y últimos movimientos.
3. Registrar una **entrada** de producto y verificar que el stock aumenta en Productos.
4. Registrar una **salida** mayor al stock disponible y verificar el mensaje de error.
5. Consultar **Reportes** y exportar a CSV.
6. Crear un usuario con rol `Auditor`, iniciar sesión con él y verificar que no aparecen botones de escritura.

## Licencia

Proyecto académico/privado.
