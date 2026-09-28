/* =========================================================================
   ControlInventario - Script de base de datos
   Motor: Microsoft SQL Server (2019 o superior)
   Nombre de la base de datos: controlinventario
   Descripción: Crea el esquema completo (tablas, índices, restricciones,
                vistas) y los datos de inicio del sistema.
   Nota: Todas las tablas, columnas y variables están nombradas en español.
   ========================================================================= */

USE [master];
GO

/* -------------------------------------------------------------------------
   1. CREACIÓN DE LA BASE DE DATOS
   ------------------------------------------------------------------------- */
IF DB_ID(N'controlinventario') IS NULL
BEGIN
    CREATE DATABASE [controlinventario];
END
GO

USE [controlinventario];
GO

/* -------------------------------------------------------------------------
   2. ELIMINACIÓN DE OBJETOS (para poder re-ejecutar el script)
   ------------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.MovimientoDetalles', N'U') IS NOT NULL DROP TABLE dbo.MovimientoDetalles;
IF OBJECT_ID(N'dbo.Movimientos',        N'U') IS NOT NULL DROP TABLE dbo.Movimientos;
IF OBJECT_ID(N'dbo.StockPorAlmacen',    N'U') IS NOT NULL DROP TABLE dbo.StockPorAlmacen;
IF OBJECT_ID(N'dbo.Productos',          N'U') IS NOT NULL DROP TABLE dbo.Productos;
IF OBJECT_ID(N'dbo.Almacenes',          N'U') IS NOT NULL DROP TABLE dbo.Almacenes;
IF OBJECT_ID(N'dbo.Proveedores',        N'U') IS NOT NULL DROP TABLE dbo.Proveedores;
IF OBJECT_ID(N'dbo.Categorias',         N'U') IS NOT NULL DROP TABLE dbo.Categorias;
IF OBJECT_ID(N'dbo.Usuarios',           N'U') IS NOT NULL DROP TABLE dbo.Usuarios;
IF OBJECT_ID(N'dbo.Roles',              N'U') IS NOT NULL DROP TABLE dbo.Roles;
IF OBJECT_ID(N'dbo.V_ProductosStockCritico', N'V') IS NOT NULL DROP VIEW dbo.V_ProductosStockCritico;
GO

/* -------------------------------------------------------------------------
   3. TABLA ROLES
   ------------------------------------------------------------------------- */
CREATE TABLE dbo.Roles
(
    IdRol       INT IDENTITY (1, 1) NOT NULL,
    Nombre      NVARCHAR (50)  NOT NULL,
    Descripcion NVARCHAR (200) NULL,
    CONSTRAINT PK_Roles PRIMARY KEY CLUSTERED (IdRol),
    CONSTRAINT UQ_Roles_Nombre UNIQUE (Nombre)
);
GO

/* -------------------------------------------------------------------------
   4. TABLA USUARIOS
   ------------------------------------------------------------------------- */
CREATE TABLE dbo.Usuarios
(
    IdUsuario      INT IDENTITY (1, 1) NOT NULL,
    NombreUsuario  NVARCHAR (50)  NOT NULL,
    NombreCompleto NVARCHAR (100) NOT NULL,
    Correo         NVARCHAR (100) NOT NULL,
    ContrasenaHash NVARCHAR (200) NOT NULL,
    IdRol          INT            NOT NULL,
    Activo         BIT            NOT NULL CONSTRAINT DF_Usuarios_Activo DEFAULT (1),
    FechaCreacion  DATETIME2 (3)  NOT NULL CONSTRAINT DF_Usuarios_FechaCreacion DEFAULT (GETDATE()),
    CONSTRAINT PK_Usuarios PRIMARY KEY CLUSTERED (IdUsuario),
    CONSTRAINT UQ_Usuarios_NombreUsuario UNIQUE (NombreUsuario),
    CONSTRAINT UQ_Usuarios_Correo UNIQUE (Correo),
    CONSTRAINT FK_Usuarios_Roles FOREIGN KEY (IdRol) REFERENCES dbo.Roles (IdRol)
);
GO

/* -------------------------------------------------------------------------
   5. TABLA CATEGORIAS
   ------------------------------------------------------------------------- */
CREATE TABLE dbo.Categorias
(
    IdCategoria  INT IDENTITY (1, 1) NOT NULL,
    Nombre       NVARCHAR (100) NOT NULL,
    Descripcion  NVARCHAR (300) NULL,
    Activo       BIT            NOT NULL CONSTRAINT DF_Categorias_Activo DEFAULT (1),
    FechaCreacion DATETIME2 (3) NOT NULL CONSTRAINT DF_Categorias_FechaCreacion DEFAULT (GETDATE()),
    CONSTRAINT PK_Categorias PRIMARY KEY CLUSTERED (IdCategoria),
    CONSTRAINT UQ_Categorias_Nombre UNIQUE (Nombre)
);
GO

/* -------------------------------------------------------------------------
   6. TABLA PROVEEDORES
   ------------------------------------------------------------------------- */
CREATE TABLE dbo.Proveedores
(
    IdProveedor  INT IDENTITY (1, 1) NOT NULL,
    Nombre       NVARCHAR (150) NOT NULL,
    Contacto     NVARCHAR (100) NULL,
    Telefono     NVARCHAR (30)  NULL,
    Correo       NVARCHAR (100) NULL,
    Direccion    NVARCHAR (200) NULL,
    Activo       BIT            NOT NULL CONSTRAINT DF_Proveedores_Activo DEFAULT (1),
    FechaCreacion DATETIME2 (3) NOT NULL CONSTRAINT DF_Proveedores_FechaCreacion DEFAULT (GETDATE()),
    CONSTRAINT PK_Proveedores PRIMARY KEY CLUSTERED (IdProveedor),
    CONSTRAINT UQ_Proveedores_Nombre UNIQUE (Nombre)
);
GO

/* -------------------------------------------------------------------------
   7. TABLA ALMACENES  (soporte para uno o varios almacenes)
   ------------------------------------------------------------------------- */
CREATE TABLE dbo.Almacenes
(
    IdAlmacen   INT IDENTITY (1, 1) NOT NULL,
    Nombre      NVARCHAR (100) NOT NULL,
    Ubicacion   NVARCHAR (200) NULL,
    Descripcion NVARCHAR (300) NULL,
    Activo      BIT            NOT NULL CONSTRAINT DF_Almacenes_Activo DEFAULT (1),
    FechaCreacion DATETIME2 (3) NOT NULL CONSTRAINT DF_Almacenes_FechaCreacion DEFAULT (GETDATE()),
    CONSTRAINT PK_Almacenes PRIMARY KEY CLUSTERED (IdAlmacen),
    CONSTRAINT UQ_Almacenes_Nombre UNIQUE (Nombre)
);
GO

/* -------------------------------------------------------------------------
   8. TABLA PRODUCTOS
   ------------------------------------------------------------------------- */
CREATE TABLE dbo.Productos
(
    IdProducto    INT IDENTITY (1, 1) NOT NULL,
    Codigo        NVARCHAR (30)   NOT NULL,
    Nombre        NVARCHAR (150)  NOT NULL,
    Descripcion   NVARCHAR (500)  NULL,
    IdCategoria   INT             NOT NULL,
    IdProveedor   INT             NOT NULL,
    PrecioCompra  DECIMAL (18, 2) NOT NULL CONSTRAINT DF_Productos_PrecioCompra DEFAULT (0),
    PrecioVenta   DECIMAL (18, 2) NOT NULL CONSTRAINT DF_Productos_PrecioVenta DEFAULT (0),
    UnidadMedida  NVARCHAR (30)   NOT NULL CONSTRAINT DF_Productos_UnidadMedida DEFAULT (N'Unidad'),
    StockMinimo   INT             NOT NULL CONSTRAINT DF_Productos_StockMinimo DEFAULT (0),
    Activo        BIT             NOT NULL CONSTRAINT DF_Productos_Activo DEFAULT (1),
    FechaCreacion DATETIME2 (3)   NOT NULL CONSTRAINT DF_Productos_FechaCreacion DEFAULT (GETDATE()),
    CONSTRAINT PK_Productos PRIMARY KEY CLUSTERED (IdProducto),
    CONSTRAINT UQ_Productos_Codigo UNIQUE (Codigo),
    CONSTRAINT FK_Productos_Categorias FOREIGN KEY (IdCategoria) REFERENCES dbo.Categorias (IdCategoria),
    CONSTRAINT FK_Productos_Proveedores FOREIGN KEY (IdProveedor) REFERENCES dbo.Proveedores (IdProveedor),
    CONSTRAINT CK_Productos_PrecioCompra CHECK (PrecioCompra >= 0),
    CONSTRAINT CK_Productos_PrecioVenta CHECK (PrecioVenta >= 0),
    CONSTRAINT CK_Productos_StockMinimo CHECK (StockMinimo >= 0)
);
GO

/* -------------------------------------------------------------------------
   9. TABLA STOCK POR ALMACEN (stock actual producto x almacén)
   ------------------------------------------------------------------------- */
CREATE TABLE dbo.StockPorAlmacen
(
    IdStock     INT IDENTITY (1, 1) NOT NULL,
    IdProducto  INT NOT NULL,
    IdAlmacen   INT NOT NULL,
    Cantidad    INT NOT NULL CONSTRAINT DF_StockPorAlmacen_Cantidad DEFAULT (0),
    CONSTRAINT PK_StockPorAlmacen PRIMARY KEY CLUSTERED (IdStock),
    CONSTRAINT UQ_StockPorAlmacen_ProductoAlmacen UNIQUE (IdProducto, IdAlmacen),
    CONSTRAINT FK_StockPorAlmacen_Productos FOREIGN KEY (IdProducto) REFERENCES dbo.Productos (IdProducto),
    CONSTRAINT FK_StockPorAlmacen_Almacenes FOREIGN KEY (IdAlmacen) REFERENCES dbo.Almacenes (IdAlmacen),
    CONSTRAINT CK_StockPorAlmacen_Cantidad CHECK (Cantidad >= 0)
);
GO

/* -------------------------------------------------------------------------
   10. TABLA MOVIMIENTOS (encabezado: fecha, tipo, motivo, usuario, documento)
   ------------------------------------------------------------------------- */
CREATE TABLE dbo.Movimientos
(
    IdMovimiento        INT IDENTITY (1, 1) NOT NULL,
    Fecha               DATETIME2 (3)  NOT NULL CONSTRAINT DF_Movimientos_Fecha DEFAULT (GETDATE()),
    Tipo                NVARCHAR (10)  NOT NULL,
    Motivo              NVARCHAR (300) NULL,
    DocumentoReferencia NVARCHAR (50)  NULL,
    IdUsuario           INT            NOT NULL,
    IdAlmacen           INT            NOT NULL,
    IdProveedor         INT            NULL,
    CONSTRAINT PK_Movimientos PRIMARY KEY CLUSTERED (IdMovimiento),
    CONSTRAINT FK_Movimientos_Usuarios FOREIGN KEY (IdUsuario) REFERENCES dbo.Usuarios (IdUsuario),
    CONSTRAINT FK_Movimientos_Almacenes FOREIGN KEY (IdAlmacen) REFERENCES dbo.Almacenes (IdAlmacen),
    CONSTRAINT FK_Movimientos_Proveedores FOREIGN KEY (IdProveedor) REFERENCES dbo.Proveedores (IdProveedor),
    CONSTRAINT CK_Movimientos_Tipo CHECK (Tipo IN (N'Entrada', N'Salida'))
);
GO

/* -------------------------------------------------------------------------
   11. TABLA MOVIMIENTO DETALLES (líneas del movimiento: producto y cantidad)
   ------------------------------------------------------------------------- */
CREATE TABLE dbo.MovimientoDetalles
(
    IdMovimientoDetalle INT IDENTITY (1, 1) NOT NULL,
    IdMovimiento        INT            NOT NULL,
    IdProducto          INT            NOT NULL,
    Cantidad            INT            NOT NULL,
    PrecioUnitario      DECIMAL (18, 2) NOT NULL CONSTRAINT DF_MovimientoDetalles_PrecioUnitario DEFAULT (0),
    CONSTRAINT PK_MovimientoDetalles PRIMARY KEY CLUSTERED (IdMovimientoDetalle),
    CONSTRAINT FK_MovimientoDetalles_Movimientos FOREIGN KEY (IdMovimiento) REFERENCES dbo.Movimientos (IdMovimiento) ON DELETE CASCADE,
    CONSTRAINT FK_MovimientoDetalles_Productos FOREIGN KEY (IdProducto) REFERENCES dbo.Productos (IdProducto),
    CONSTRAINT CK_MovimientoDetalles_Cantidad CHECK (Cantidad > 0),
    CONSTRAINT CK_MovimientoDetalles_PrecioUnitario CHECK (PrecioUnitario >= 0)
);
GO

/* -------------------------------------------------------------------------
   12. ÍNDICES ADICIONALES (consultas frecuentes)
   ------------------------------------------------------------------------- */
CREATE NONCLUSTERED INDEX IX_Productos_Categoria     ON dbo.Productos (IdCategoria);
CREATE NONCLUSTERED INDEX IX_Productos_Proveedor     ON dbo.Productos (IdProveedor);
CREATE NONCLUSTERED INDEX IX_StockPorAlmacen_Almacen ON dbo.StockPorAlmacen (IdAlmacen);
CREATE NONCLUSTERED INDEX IX_Movimientos_Fecha       ON dbo.Movimientos (Fecha);
CREATE NONCLUSTERED INDEX IX_Movimientos_Usuario     ON dbo.Movimientos (IdUsuario);
CREATE NONCLUSTERED INDEX IX_Movimientos_Tipo        ON dbo.Movimientos (Tipo);
CREATE NONCLUSTERED INDEX IX_MovimientoDetalles_Producto ON dbo.MovimientoDetalles (IdProducto);
GO

/* -------------------------------------------------------------------------
   13. VISTA: PRODUCTOS CON STOCK CRÍTICO
   ------------------------------------------------------------------------- */
CREATE VIEW dbo.V_ProductosStockCritico
AS
    SELECT  p.IdProducto,
            p.Codigo,
            p.Nombre,
            c.Nombre  AS Categoria,
            pr.Nombre AS Proveedor,
            p.UnidadMedida,
            p.StockMinimo,
            ISNULL(SUM(s.Cantidad), 0)                        AS StockActual,
            (p.StockMinimo - ISNULL(SUM(s.Cantidad), 0))      AS Diferencia
    FROM    dbo.Productos p
            LEFT JOIN dbo.StockPorAlmacen s ON s.IdProducto = p.IdProducto
            INNER JOIN dbo.Categorias c     ON c.IdCategoria = p.IdCategoria
            INNER JOIN dbo.Proveedores pr   ON pr.IdProveedor = p.IdProveedor
    WHERE   p.Activo = 1
    GROUP BY p.IdProducto, p.Codigo, p.Nombre, p.UnidadMedida, p.StockMinimo, c.Nombre, pr.Nombre
    HAVING  ISNULL(SUM(s.Cantidad), 0) <= p.StockMinimo;
GO

/* =========================================================================
   14. DATOS DE INICIO (SEMILLA)
   ========================================================================= */

/* --- 14.1 Roles --- */
SET IDENTITY_INSERT dbo.Roles ON;
INSERT INTO dbo.Roles (IdRol, Nombre, Descripcion)
VALUES (1, N'Administrador', N'Acceso total al sistema: productos, movimientos, reportes y usuarios'),
       (2, N'Usuario',       N'Registra movimientos de entrada y salida y consulta productos'),
       (3, N'Auditor',       N'Solo lectura: consultas y reportes');
SET IDENTITY_INSERT dbo.Roles OFF;
GO

/* --- 14.2 Usuario administrador por defecto (contraseña: Admin123*) --- */
/* Hash generado con ASP.NET Core Identity V3 (PBKDF2-HMAC-SHA512, 100000 iteraciones) */
IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE NombreUsuario = N'admin')
BEGIN
    INSERT INTO dbo.Usuarios (NombreUsuario, NombreCompleto, Correo, ContrasenaHash, IdRol, Activo)
    VALUES (N'admin', N'Administrador General', N'admin@controlinventario.local',
            N'AQAAAAIAAYagAAAAEDyna07dG+aQrOcrFhvXLRHqHea7Lm2hN8pMdsA1gsnKagwVg60Xhg4oO9e2psD7Mw==',
            1, 1);
END
GO

/* --- 14.3 Almacenes --- */
SET IDENTITY_INSERT dbo.Almacenes ON;
INSERT INTO dbo.Almacenes (IdAlmacen, Nombre, Ubicacion, Descripcion)
VALUES (1, N'Almacén Central',   N'Bodega principal - Zona Industrial', N'Almacén principal de la empresa'),
       (2, N'Almacén Sucursal',  N'Local comercial - Centro',          N'Almacén de la sucursal comercial');
SET IDENTITY_INSERT dbo.Almacenes OFF;
GO

/* --- 14.4 Categorías --- */
SET IDENTITY_INSERT dbo.Categorias ON;
INSERT INTO dbo.Categorias (IdCategoria, Nombre, Descripcion)
VALUES (1, N'Electrónica',    N'Artículos electrónicos y componentes'),
       (2, N'Oficina',        N'Materiales y útiles de oficina'),
       (3, N'Herramientas',   N'Herramientas manuales y eléctricas'),
       (4, N'Limpieza',       N'Productos de aseo y limpieza');
SET IDENTITY_INSERT dbo.Categorias OFF;
GO

/* --- 14.5 Proveedores --- */
SET IDENTITY_INSERT dbo.Proveedores ON;
INSERT INTO dbo.Proveedores (IdProveedor, Nombre, Contacto, Telefono, Correo, Direccion)
VALUES (1, N'Distribuidora Norte S.A.',  N'Carlos Méndez',  N'+56 9 8765 4321', N'ventas@distribuidoranorte.cl',  N'Av. Industrial 1234'),
       (2, N'TecnoSuministros Ltda.',    N'María Rojas',    N'+56 9 1234 5678', N'contacto@tecnosuministros.cl', N'Calle Comercio 567'),
       (3, N'Útiles del Sur SpA',        N'Jorge Pérez',    N'+56 9 5555 1234', N'pedidos@utilessurs.cl',        N'Ruta 5 Km 12');
SET IDENTITY_INSERT dbo.Proveedores OFF;
GO

/* --- 14.6 Productos --- */
SET IDENTITY_INSERT dbo.Productos ON;
INSERT INTO dbo.Productos (IdProducto, Codigo, Nombre, Descripcion, IdCategoria, IdProveedor, PrecioCompra, PrecioVenta, UnidadMedida, StockMinimo)
VALUES (1,  N'ELEC-001', N'Teclado Mecánico USB',      N'Teclado mecánico retroiluminado',        1, 1, 25000,   39990,  N'Unidad', 10),
       (2,  N'ELEC-002', N'Mouse Óptico Inalámbrico',  N'Mouse inalámbrico 2.4GHz',               1, 1, 8000,    14990,  N'Unidad', 10),
       (3,  N'ELEC-003', N'Monitor LED 24"',           N'Monitor Full HD 24 pulgadas',            1, 2, 95000,   139990, N'Unidad', 5),
       (4,  N'OFIC-001', N'Resma Papel Carta',         N'500 hojas papel bond 75g',               2, 3, 3500,    5990,   N'Caja',   20),
       (5,  N'HERR-001', N'Taladro Percutor 650W',     N'Taladro percutor con maletín',           3, 2, 45000,   69990,  N'Unidad', 4),
       (6,  N'LIMP-001', N'Detergente Multiuso 1L',    N'Detergente concentrado multiuso',        4, 3, 1800,    3290,   N'Unidad', 30);
SET IDENTITY_INSERT dbo.Productos OFF;
GO

/* --- 14.7 Stock inicial por almacén (ya refleja el efecto de los movimientos de ejemplo) --- */
INSERT INTO dbo.StockPorAlmacen (IdProducto, IdAlmacen, Cantidad)
VALUES (1, 1, 25),   -- Teclado: por encima del mínimo (10)
       (2, 1, 4),    -- Mouse: BAJO (mínimo 10)  -> alerta de stock crítico
       (3, 1, 7),    -- Monitor: OK (mínimo 5)
       (4, 1, 50),   -- Resma papel: OK (mínimo 20)
       (4, 2, 12),   -- Resma papel en sucursal: suma 62 total
       (5, 1, 1),    -- Taladro: BAJO (mínimo 4) -> alerta de stock crítico
       (6, 1, 15);   -- Detergente: BAJO (mínimo 30) -> alerta de stock crítico
GO

/* --- 14.8 Movimientos de ejemplo (encabezado + detalles) --- */
INSERT INTO dbo.Movimientos (Fecha, Tipo, Motivo, DocumentoReferencia, IdUsuario, IdAlmacen, IdProveedor)
VALUES (DATEADD(DAY, -10, GETDATE()), N'Entrada', N'Compra inicial de mercadería',    N'FAC-0001', 1, 1, 1),
       (DATEADD(DAY, -5,  GETDATE()), N'Salida',  N'Venta a cliente - nota de pedido', N'DOC-0001', 1, 1, NULL),
       (DATEADD(DAY, -2,  GETDATE()), N'Salida',  N'Reposición de sucursal',           N'DOC-0002', 1, 1, NULL);

INSERT INTO dbo.MovimientoDetalles (IdMovimiento, IdProducto, Cantidad, PrecioUnitario)
VALUES (1, 1, 30, 25000),
       (1, 4, 70, 3500),
       (2, 1, 5,  39990),
       (3, 4, 8,  5990);
GO

PRINT N'Base de datos "controlinventario" creada exitosamente.';
GO
