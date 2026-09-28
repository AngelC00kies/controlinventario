using ControlInventario.Application.Comun;
using ControlInventario.Application.DTOs;
using ControlInventario.Application.Interfaces;
using ControlInventario.Application.Validadores;
using ControlInventario.Domain.Entidades;
using ControlInventario.Domain.Interfaces;
using FluentValidation;

namespace ControlInventario.Application.Servicios;

/// <summary>
/// Servicio de productos: alta, baja lógica, modificación y consultas de stock.
/// </summary>
public class ServicioProductos : IServicioProductos
{
    private readonly IRepositorioProductos _productos;
    private readonly IRepositorioCategorias _categorias;
    private readonly IRepositorioProveedores _proveedores;
    private readonly IRepositorioStockPorAlmacen _stock;
    private readonly IUnidadTrabajo _unidadTrabajo;
    private readonly IValidator<ProductoCreacionDTO> _validadorCreacion;
    private readonly IValidator<ProductoActualizacionDTO> _validadorActualizacion;

    public ServicioProductos(
        IRepositorioProductos productos,
        IRepositorioCategorias categorias,
        IRepositorioProveedores proveedores,
        IRepositorioStockPorAlmacen stock,
        IUnidadTrabajo unidadTrabajo,
        IValidator<ProductoCreacionDTO> validadorCreacion,
        IValidator<ProductoActualizacionDTO> validadorActualizacion)
    {
        _productos = productos;
        _categorias = categorias;
        _proveedores = proveedores;
        _stock = stock;
        _unidadTrabajo = unidadTrabajo;
        _validadorCreacion = validadorCreacion;
        _validadorActualizacion = validadorActualizacion;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ProductoDTO>> ListarAsync(string? busqueda, int? idCategoria, int? idProveedor, bool soloActivos = true)
    {
        var productos = await _productos.BuscarAsync(busqueda, idCategoria, idProveedor, soloActivos);
        return productos.Select(Mapeador.AProductoDTO).ToList();
    }

    /// <inheritdoc />
    public async Task<ProductoDTO> ObtenerPorIdAsync(int id)
    {
        var producto = await _productos.ObtenerCompletoAsync(id)
            ?? throw new ExcepcionNoEncontrado("Producto no encontrado.");

        return Mapeador.AProductoDTO(producto);
    }

    /// <inheritdoc />
    public async Task<ProductoDTO> CrearAsync(ProductoCreacionDTO dto)
    {
        await ValidarAsync(_validadorCreacion, dto);

        if (await _productos.ExisteCodigoAsync(dto.Codigo))
        {
            throw new ExcepcionNegocio($"Ya existe un producto con el código '{dto.Codigo}'.");
        }

        await VerificarRelacionesAsync(dto.IdCategoria, dto.IdProveedor);

        var producto = new Producto
        {
            Codigo = dto.Codigo.Trim(),
            Nombre = dto.Nombre.Trim(),
            Descripcion = dto.Descripcion?.Trim(),
            IdCategoria = dto.IdCategoria,
            IdProveedor = dto.IdProveedor,
            PrecioCompra = dto.PrecioCompra,
            PrecioVenta = dto.PrecioVenta,
            UnidadMedida = dto.UnidadMedida.Trim(),
            StockMinimo = dto.StockMinimo,
            Activo = true
        };

        await _productos.AgregarAsync(producto);
        await _unidadTrabajo.GuardarCambiosAsync();

        return await ObtenerPorIdAsync(producto.IdProducto);
    }

    /// <inheritdoc />
    public async Task<ProductoDTO> ActualizarAsync(int id, ProductoActualizacionDTO dto)
    {
        await ValidarAsync(_validadorActualizacion, dto);

        var producto = await _productos.ObtenerPorIdAsync(id)
            ?? throw new ExcepcionNoEncontrado("Producto no encontrado.");

        if (await _productos.ExisteCodigoAsync(dto.Codigo, id))
        {
            throw new ExcepcionNegocio($"Ya existe otro producto con el código '{dto.Codigo}'.");
        }

        await VerificarRelacionesAsync(dto.IdCategoria, dto.IdProveedor);

        producto.Codigo = dto.Codigo.Trim();
        producto.Nombre = dto.Nombre.Trim();
        producto.Descripcion = dto.Descripcion?.Trim();
        producto.IdCategoria = dto.IdCategoria;
        producto.IdProveedor = dto.IdProveedor;
        producto.PrecioCompra = dto.PrecioCompra;
        producto.PrecioVenta = dto.PrecioVenta;
        producto.UnidadMedida = dto.UnidadMedida.Trim();
        producto.StockMinimo = dto.StockMinimo;
        producto.Activo = dto.Activo;

        _productos.Actualizar(producto);
        await _unidadTrabajo.GuardarCambiosAsync();

        return await ObtenerPorIdAsync(id);
    }

    /// <inheritdoc />
    public async Task EliminarAsync(int id)
    {
        var producto = await _productos.ObtenerPorIdAsync(id)
            ?? throw new ExcepcionNoEncontrado("Producto no encontrado.");

        // Baja lógica: se conserva el historial de movimientos.
        producto.Activo = false;
        _productos.Actualizar(producto);
        await _unidadTrabajo.GuardarCambiosAsync();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<StockProductoDTO>> ObtenerStockAsync(int id)
    {
        _ = await _productos.ObtenerPorIdAsync(id)
            ?? throw new ExcepcionNoEncontrado("Producto no encontrado.");

        var stock = await _stock.ListarPorProductoAsync(id);
        return stock.Select(s => new StockProductoDTO
        {
            IdAlmacen = s.IdAlmacen,
            NombreAlmacen = s.Almacen?.Nombre ?? string.Empty,
            Cantidad = s.Cantidad
        }).ToList();
    }

    /// <summary>Ejecuta un validador FluentValidation y lanza ExcepcionNegocio si falla.</summary>
    private static async Task ValidarAsync<T>(IValidator<T> validador, T dto)
    {
        var validacion = await validador.ValidateAsync(dto);
        if (!validacion.IsValid)
        {
            throw new ExcepcionNegocio(string.Join(" ", validacion.Errors.Select(e => e.ErrorMessage)));
        }
    }

    /// <summary>Verifica que la categoría y el proveedor existan y estén activos.</summary>
    private async Task VerificarRelacionesAsync(int idCategoria, int idProveedor)
    {
        var categoria = await _categorias.ObtenerPorIdAsync(idCategoria);
        if (categoria is null || !categoria.Activo)
        {
            throw new ExcepcionNegocio("La categoría indicada no existe o está inactiva.");
        }

        var proveedor = await _proveedores.ObtenerPorIdAsync(idProveedor);
        if (proveedor is null || !proveedor.Activo)
        {
            throw new ExcepcionNegocio("El proveedor indicado no existe o está inactivo.");
        }
    }
}
