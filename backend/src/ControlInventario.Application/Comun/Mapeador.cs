using ControlInventario.Application.DTOs;
using ControlInventario.Domain.Entidades;

namespace ControlInventario.Application.Comun;

/// <summary>
/// Utilidades de mapeo entre entidades de dominio y DTOs (SRP: centraliza
/// la transformación para no duplicarla en cada servicio).
/// </summary>
public static class Mapeador
{
    /// <summary>Stock total de un producto sumando todos sus almacenes.</summary>
    public static int StockTotal(Producto producto) =>
        producto.StockPorAlmacen?.Sum(s => s.Cantidad) ?? 0;

    /// <summary>Convierte un producto a su DTO, calculando stock y estado crítico.</summary>
    public static ProductoDTO AProductoDTO(Producto producto)
    {
        var stock = StockTotal(producto);
        return new ProductoDTO
        {
            IdProducto = producto.IdProducto,
            Codigo = producto.Codigo,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            IdCategoria = producto.IdCategoria,
            NombreCategoria = producto.Categoria?.Nombre ?? string.Empty,
            IdProveedor = producto.IdProveedor,
            NombreProveedor = producto.Proveedor?.Nombre ?? string.Empty,
            PrecioCompra = producto.PrecioCompra,
            PrecioVenta = producto.PrecioVenta,
            UnidadMedida = producto.UnidadMedida,
            StockMinimo = producto.StockMinimo,
            StockActual = stock,
            StockCritico = stock <= producto.StockMinimo,
            Activo = producto.Activo,
            FechaCreacion = producto.FechaCreacion
        };
    }

    /// <summary>Convierte una categoría a su DTO con el total de productos.</summary>
    public static CategoriaDTO ACategoriaDTO(Categoria categoria) => new()
    {
        IdCategoria = categoria.IdCategoria,
        Nombre = categoria.Nombre,
        Descripcion = categoria.Descripcion,
        Activo = categoria.Activo,
        TotalProductos = categoria.Productos?.Count(p => p.Activo) ?? 0
    };

    /// <summary>Convierte un proveedor a su DTO con el total de productos.</summary>
    public static ProveedorDTO AProveedorDTO(Proveedor proveedor) => new()
    {
        IdProveedor = proveedor.IdProveedor,
        Nombre = proveedor.Nombre,
        Contacto = proveedor.Contacto,
        Telefono = proveedor.Telefono,
        Correo = proveedor.Correo,
        Direccion = proveedor.Direccion,
        Activo = proveedor.Activo,
        TotalProductos = proveedor.Productos?.Count(p => p.Activo) ?? 0
    };

    /// <summary>Convierte un almacén a su DTO.</summary>
    public static AlmacenDTO AAlmacenDTO(Almacen almacen) => new()
    {
        IdAlmacen = almacen.IdAlmacen,
        Nombre = almacen.Nombre,
        Ubicacion = almacen.Ubicacion,
        Descripcion = almacen.Descripcion,
        Activo = almacen.Activo
    };

    /// <summary>Convierte un usuario a su DTO (nunca expone la contraseña).</summary>
    public static UsuarioDTO AUsuarioDTO(Usuario usuario) => new()
    {
        IdUsuario = usuario.IdUsuario,
        NombreUsuario = usuario.NombreUsuario,
        NombreCompleto = usuario.NombreCompleto,
        Correo = usuario.Correo,
        IdRol = usuario.IdRol,
        NombreRol = usuario.Rol?.Nombre ?? string.Empty,
        Activo = usuario.Activo,
        FechaCreacion = usuario.FechaCreacion
    };

    /// <summary>Convierte un movimiento con sus detalles a su DTO.</summary>
    public static MovimientoDTO AMovimientoDTO(Movimiento movimiento)
    {
        var detalles = movimiento.Detalles?.Select(d => new MovimientoDetalleDTO
        {
            IdProducto = d.IdProducto,
            CodigoProducto = d.Producto?.Codigo ?? string.Empty,
            NombreProducto = d.Producto?.Nombre ?? string.Empty,
            UnidadMedida = d.Producto?.UnidadMedida ?? string.Empty,
            Cantidad = d.Cantidad,
            PrecioUnitario = d.PrecioUnitario,
            Subtotal = d.Cantidad * d.PrecioUnitario
        }).ToList() ?? new List<MovimientoDetalleDTO>();

        return new MovimientoDTO
        {
            IdMovimiento = movimiento.IdMovimiento,
            Fecha = movimiento.Fecha,
            Tipo = movimiento.Tipo.ToString(),
            Motivo = movimiento.Motivo,
            DocumentoReferencia = movimiento.DocumentoReferencia,
            IdUsuario = movimiento.IdUsuario,
            NombreUsuario = movimiento.Usuario?.NombreCompleto ?? string.Empty,
            IdAlmacen = movimiento.IdAlmacen,
            NombreAlmacen = movimiento.Almacen?.Nombre ?? string.Empty,
            IdProveedor = movimiento.IdProveedor,
            NombreProveedor = movimiento.Proveedor?.Nombre,
            Total = detalles.Sum(d => d.Subtotal),
            Detalles = detalles
        };
    }

    /// <summary>Convierte un producto con stock crítico en su alerta.</summary>
    public static AlertaStockDTO AAlertaStockDTO(Producto producto)
    {
        var stock = StockTotal(producto);
        return new AlertaStockDTO
        {
            IdProducto = producto.IdProducto,
            Codigo = producto.Codigo,
            Nombre = producto.Nombre,
            NombreCategoria = producto.Categoria?.Nombre ?? string.Empty,
            UnidadMedida = producto.UnidadMedida,
            StockMinimo = producto.StockMinimo,
            StockActual = stock,
            Diferencia = producto.StockMinimo - stock
        };
    }
}
