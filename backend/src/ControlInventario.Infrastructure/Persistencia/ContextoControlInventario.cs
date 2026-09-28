using ControlInventario.Domain.Entidades;
using ControlInventario.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ControlInventario.Infrastructure.Persistencia;

/// <summary>
/// Contexto de Entity Framework Core. Mapea las entidades de dominio a las
/// tablas de la base de datos "controlinventario" (script en español).
/// </summary>
public class ContextoControlInventario : DbContext
{
    public ContextoControlInventario(DbContextOptions<ContextoControlInventario> opciones)
        : base(opciones)
    {
    }

    public DbSet<Rol> Roles => Set<Rol>();

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<Categoria> Categorias => Set<Categoria>();

    public DbSet<Proveedor> Proveedores => Set<Proveedor>();

    public DbSet<Almacen> Almacenes => Set<Almacen>();

    public DbSet<Producto> Productos => Set<Producto>();

    public DbSet<StockPorAlmacen> StockPorAlmacen => Set<StockPorAlmacen>();

    public DbSet<Movimiento> Movimientos => Set<Movimiento>();

    public DbSet<MovimientoDetalle> MovimientoDetalles => Set<MovimientoDetalle>();

    /// <summary>
    /// Define el esquema exacto: nombres de tablas/columnas en español,
    /// tipos de datos, restricciones e índices (espejo del script SQL).
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelo)
    {
        base.OnModelCreating(modelo);

        ConfigurarRoles(modelo);
        ConfigurarUsuarios(modelo);
        ConfigurarCategorias(modelo);
        ConfigurarProveedores(modelo);
        ConfigurarAlmacenes(modelo);
        ConfigurarProductos(modelo);
        ConfigurarStockPorAlmacen(modelo);
        ConfigurarMovimientos(modelo);
        ConfigurarMovimientoDetalles(modelo);
    }

    private static void ConfigurarRoles(ModelBuilder modelo)
    {
        modelo.Entity<Rol>(entidad =>
        {
            entidad.ToTable("Roles");
            entidad.HasKey(e => e.IdRol);
            entidad.Property(e => e.IdRol).HasColumnName("IdRol");
            entidad.Property(e => e.Nombre).HasColumnName("Nombre").IsRequired().HasMaxLength(50);
            entidad.Property(e => e.Descripcion).HasColumnName("Descripcion").HasMaxLength(200);
            entidad.HasIndex(e => e.Nombre).IsUnique();
        });
    }

    private static void ConfigurarUsuarios(ModelBuilder modelo)
    {
        modelo.Entity<Usuario>(entidad =>
        {
            entidad.ToTable("Usuarios");
            entidad.HasKey(e => e.IdUsuario);
            entidad.Property(e => e.IdUsuario).HasColumnName("IdUsuario");
            entidad.Property(e => e.NombreUsuario).HasColumnName("NombreUsuario").IsRequired().HasMaxLength(50);
            entidad.Property(e => e.NombreCompleto).HasColumnName("NombreCompleto").IsRequired().HasMaxLength(100);
            entidad.Property(e => e.Correo).HasColumnName("Correo").IsRequired().HasMaxLength(100);
            entidad.Property(e => e.ContrasenaHash).HasColumnName("ContrasenaHash").IsRequired().HasMaxLength(200);
            entidad.Property(e => e.IdRol).HasColumnName("IdRol");
            entidad.Property(e => e.Activo).HasColumnName("Activo").HasDefaultValue(true);
            entidad.Property(e => e.FechaCreacion).HasColumnName("FechaCreacion").HasColumnType("datetime2(3)")
                .HasDefaultValueSql("GETDATE()");

            entidad.HasIndex(e => e.NombreUsuario).IsUnique();
            entidad.HasIndex(e => e.Correo).IsUnique();

            entidad.HasOne(e => e.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(e => e.IdRol)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigurarCategorias(ModelBuilder modelo)
    {
        modelo.Entity<Categoria>(entidad =>
        {
            entidad.ToTable("Categorias");
            entidad.HasKey(e => e.IdCategoria);
            entidad.Property(e => e.IdCategoria).HasColumnName("IdCategoria");
            entidad.Property(e => e.Nombre).HasColumnName("Nombre").IsRequired().HasMaxLength(100);
            entidad.Property(e => e.Descripcion).HasColumnName("Descripcion").HasMaxLength(300);
            entidad.Property(e => e.Activo).HasColumnName("Activo").HasDefaultValue(true);
            entidad.Property(e => e.FechaCreacion).HasColumnName("FechaCreacion").HasColumnType("datetime2(3)")
                .HasDefaultValueSql("GETDATE()");
            entidad.HasIndex(e => e.Nombre).IsUnique();
        });
    }

    private static void ConfigurarProveedores(ModelBuilder modelo)
    {
        modelo.Entity<Proveedor>(entidad =>
        {
            entidad.ToTable("Proveedores");
            entidad.HasKey(e => e.IdProveedor);
            entidad.Property(e => e.IdProveedor).HasColumnName("IdProveedor");
            entidad.Property(e => e.Nombre).HasColumnName("Nombre").IsRequired().HasMaxLength(150);
            entidad.Property(e => e.Contacto).HasColumnName("Contacto").HasMaxLength(100);
            entidad.Property(e => e.Telefono).HasColumnName("Telefono").HasMaxLength(30);
            entidad.Property(e => e.Correo).HasColumnName("Correo").HasMaxLength(100);
            entidad.Property(e => e.Direccion).HasColumnName("Direccion").HasMaxLength(200);
            entidad.Property(e => e.Activo).HasColumnName("Activo").HasDefaultValue(true);
            entidad.Property(e => e.FechaCreacion).HasColumnName("FechaCreacion").HasColumnType("datetime2(3)")
                .HasDefaultValueSql("GETDATE()");
            entidad.HasIndex(e => e.Nombre).IsUnique();
        });
    }

    private static void ConfigurarAlmacenes(ModelBuilder modelo)
    {
        modelo.Entity<Almacen>(entidad =>
        {
            entidad.ToTable("Almacenes");
            entidad.HasKey(e => e.IdAlmacen);
            entidad.Property(e => e.IdAlmacen).HasColumnName("IdAlmacen");
            entidad.Property(e => e.Nombre).HasColumnName("Nombre").IsRequired().HasMaxLength(100);
            entidad.Property(e => e.Ubicacion).HasColumnName("Ubicacion").HasMaxLength(200);
            entidad.Property(e => e.Descripcion).HasColumnName("Descripcion").HasMaxLength(300);
            entidad.Property(e => e.Activo).HasColumnName("Activo").HasDefaultValue(true);
            entidad.Property(e => e.FechaCreacion).HasColumnName("FechaCreacion").HasColumnType("datetime2(3)")
                .HasDefaultValueSql("GETDATE()");
            entidad.HasIndex(e => e.Nombre).IsUnique();
        });
    }

    private static void ConfigurarProductos(ModelBuilder modelo)
    {
        modelo.Entity<Producto>(entidad =>
        {
            entidad.ToTable("Productos");
            entidad.HasKey(e => e.IdProducto);
            entidad.Property(e => e.IdProducto).HasColumnName("IdProducto");
            entidad.Property(e => e.Codigo).HasColumnName("Codigo").IsRequired().HasMaxLength(30);
            entidad.Property(e => e.Nombre).HasColumnName("Nombre").IsRequired().HasMaxLength(150);
            entidad.Property(e => e.Descripcion).HasColumnName("Descripcion").HasMaxLength(500);
            entidad.Property(e => e.IdCategoria).HasColumnName("IdCategoria");
            entidad.Property(e => e.IdProveedor).HasColumnName("IdProveedor");
            entidad.Property(e => e.PrecioCompra).HasColumnName("PrecioCompra").HasColumnType("decimal(18,2)");
            entidad.Property(e => e.PrecioVenta).HasColumnName("PrecioVenta").HasColumnType("decimal(18,2)");
            entidad.Property(e => e.UnidadMedida).HasColumnName("UnidadMedida").IsRequired().HasMaxLength(30);
            entidad.Property(e => e.StockMinimo).HasColumnName("StockMinimo");
            entidad.Property(e => e.Activo).HasColumnName("Activo").HasDefaultValue(true);
            entidad.Property(e => e.FechaCreacion).HasColumnName("FechaCreacion").HasColumnType("datetime2(3)")
                .HasDefaultValueSql("GETDATE()");

            entidad.HasIndex(e => e.Codigo).IsUnique();

            entidad.HasOne(e => e.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(e => e.IdCategoria)
                .OnDelete(DeleteBehavior.Restrict);

            entidad.HasOne(e => e.Proveedor)
                .WithMany(p => p.Productos)
                .HasForeignKey(e => e.IdProveedor)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigurarStockPorAlmacen(ModelBuilder modelo)
    {
        modelo.Entity<StockPorAlmacen>(entidad =>
        {
            entidad.ToTable("StockPorAlmacen");
            entidad.HasKey(e => e.IdStock);
            entidad.Property(e => e.IdStock).HasColumnName("IdStock");
            entidad.Property(e => e.IdProducto).HasColumnName("IdProducto");
            entidad.Property(e => e.IdAlmacen).HasColumnName("IdAlmacen");
            entidad.Property(e => e.Cantidad).HasColumnName("Cantidad");

            // Un único registro de stock por producto y almacén.
            entidad.HasIndex(e => new { e.IdProducto, e.IdAlmacen }).IsUnique();

            entidad.HasOne(e => e.Producto)
                .WithMany(p => p.StockPorAlmacen)
                .HasForeignKey(e => e.IdProducto)
                .OnDelete(DeleteBehavior.Restrict);

            entidad.HasOne(e => e.Almacen)
                .WithMany(a => a.StockPorAlmacen)
                .HasForeignKey(e => e.IdAlmacen)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigurarMovimientos(ModelBuilder modelo)
    {
        modelo.Entity<Movimiento>(entidad =>
        {
            entidad.ToTable("Movimientos");
            entidad.HasKey(e => e.IdMovimiento);
            entidad.Property(e => e.IdMovimiento).HasColumnName("IdMovimiento");
            entidad.Property(e => e.Fecha).HasColumnName("Fecha").HasColumnType("datetime2(3)");

            // Tipo de movimiento persistido como texto en español: Entrada / Salida.
            entidad.Property(e => e.Tipo)
                .HasColumnName("Tipo")
                .HasConversion<string>()
                .HasMaxLength(10);

            entidad.Property(e => e.Motivo).HasColumnName("Motivo").HasMaxLength(300);
            entidad.Property(e => e.DocumentoReferencia).HasColumnName("DocumentoReferencia").HasMaxLength(50);
            entidad.Property(e => e.IdUsuario).HasColumnName("IdUsuario");
            entidad.Property(e => e.IdAlmacen).HasColumnName("IdAlmacen");
            entidad.Property(e => e.IdProveedor).HasColumnName("IdProveedor");

            entidad.HasIndex(e => e.Fecha);
            entidad.HasIndex(e => e.IdUsuario);
            entidad.HasIndex(e => e.Tipo);

            entidad.HasOne(e => e.Usuario)
                .WithMany(u => u.Movimientos)
                .HasForeignKey(e => e.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            entidad.HasOne(e => e.Almacen)
                .WithMany(a => a.Movimientos)
                .HasForeignKey(e => e.IdAlmacen)
                .OnDelete(DeleteBehavior.Restrict);

            entidad.HasOne(e => e.Proveedor)
                .WithMany(p => p.Movimientos)
                .HasForeignKey(e => e.IdProveedor)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigurarMovimientoDetalles(ModelBuilder modelo)
    {
        modelo.Entity<MovimientoDetalle>(entidad =>
        {
            entidad.ToTable("MovimientoDetalles");
            entidad.HasKey(e => e.IdMovimientoDetalle);
            entidad.Property(e => e.IdMovimientoDetalle).HasColumnName("IdMovimientoDetalle");
            entidad.Property(e => e.IdMovimiento).HasColumnName("IdMovimiento");
            entidad.Property(e => e.IdProducto).HasColumnName("IdProducto");
            entidad.Property(e => e.Cantidad).HasColumnName("Cantidad");
            entidad.Property(e => e.PrecioUnitario).HasColumnName("PrecioUnitario").HasColumnType("decimal(18,2)");

            entidad.HasIndex(e => e.IdProducto);

            // Borrar un movimiento borra sus líneas.
            entidad.HasOne(e => e.Movimiento)
                .WithMany(m => m.Detalles)
                .HasForeignKey(e => e.IdMovimiento)
                .OnDelete(DeleteBehavior.Cascade);

            entidad.HasOne(e => e.Producto)
                .WithMany(p => p.MovimientoDetalles)
                .HasForeignKey(e => e.IdProducto)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
