using ControlInventario.Application.DTOs;

namespace ControlInventario.Application.Interfaces;

/// <summary>Servicio de gestión de productos (alta, baja, modificación y consultas).</summary>
public interface IServicioProductos
{
    /// <summary>Lista productos con filtros opcionales de búsqueda, categoría y proveedor.</summary>
    Task<IReadOnlyList<ProductoDTO>> ListarAsync(string? busqueda, int? idCategoria, int? idProveedor, bool soloActivos = true);

    /// <summary>Obtiene un producto por su identificador.</summary>
    Task<ProductoDTO> ObtenerPorIdAsync(int id);

    /// <summary>Crea un producto nuevo (valida código duplicado y relaciones).</summary>
    Task<ProductoDTO> CrearAsync(ProductoCreacionDTO dto);

    /// <summary>Modifica un producto existente.</summary>
    Task<ProductoDTO> ActualizarAsync(int id, ProductoActualizacionDTO dto);

    /// <summary>Baja lógica de un producto (Activo = false).</summary>
    Task EliminarAsync(int id);

    /// <summary>Stock de un producto desglosado por almacén.</summary>
    Task<IReadOnlyList<StockProductoDTO>> ObtenerStockAsync(int id);
}
