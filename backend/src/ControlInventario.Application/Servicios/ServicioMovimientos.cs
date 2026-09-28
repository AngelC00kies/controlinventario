using ControlInventario.Application.Comun;
using ControlInventario.Application.DTOs;
using ControlInventario.Application.Interfaces;
using ControlInventario.Application.Validadores;
using ControlInventario.Domain.Entidades;
using ControlInventario.Domain.Enums;
using ControlInventario.Domain.Interfaces;
using FluentValidation;

namespace ControlInventario.Application.Servicios;

/// <summary>
/// Servicio de movimientos de inventario. Registra entradas y salidas y
/// actualiza el stock automáticamente dentro de una transacción, garantizando
/// que nunca quede stock negativo.
/// </summary>
public class ServicioMovimientos : IServicioMovimientos
{
    private readonly IRepositorioMovimientos _movimientos;
    private readonly IRepositorioProductos _productos;
    private readonly IRepositorioAlmacenes _almacenes;
    private readonly IRepositorioProveedores _proveedores;
    private readonly IRepositorioUsuarios _usuarios;
    private readonly IRepositorioStockPorAlmacen _stock;
    private readonly IUnidadTrabajo _unidadTrabajo;
    private readonly IValidator<MovimientoCreacionDTO> _validador;

    public ServicioMovimientos(
        IRepositorioMovimientos movimientos,
        IRepositorioProductos productos,
        IRepositorioAlmacenes almacenes,
        IRepositorioProveedores proveedores,
        IRepositorioUsuarios usuarios,
        IRepositorioStockPorAlmacen stock,
        IUnidadTrabajo unidadTrabajo,
        IValidator<MovimientoCreacionDTO> validador)
    {
        _movimientos = movimientos;
        _productos = productos;
        _almacenes = almacenes;
        _proveedores = proveedores;
        _usuarios = usuarios;
        _stock = stock;
        _unidadTrabajo = unidadTrabajo;
        _validador = validador;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<MovimientoDTO>> BuscarAsync(FiltroMovimientoDTO filtros)
    {
        var movimientos = await _movimientos.BuscarAsync(
            filtros.IdProducto, filtros.FechaDesde, filtros.FechaHasta, filtros.IdUsuario, filtros.Tipo);

        return movimientos.Select(Mapeador.AMovimientoDTO).ToList();
    }

    /// <inheritdoc />
    public async Task<MovimientoDTO> ObtenerPorIdAsync(int id)
    {
        var movimiento = await _movimientos.ObtenerConDetallesAsync(id)
            ?? throw new ExcepcionNoEncontrado("Movimiento no encontrado.");

        return Mapeador.AMovimientoDTO(movimiento);
    }

    /// <inheritdoc />
    public async Task<MovimientoDTO> CrearAsync(MovimientoCreacionDTO dto, int idUsuario)
    {
        var validacion = await _validador.ValidateAsync(dto);
        if (!validacion.IsValid)
        {
            throw new ExcepcionNegocio(string.Join(" ", validacion.Errors.Select(e => e.ErrorMessage)));
        }

        var usuario = await _usuarios.ObtenerPorIdAsync(idUsuario)
            ?? throw new ExcepcionNoEncontrado("El usuario responsable no existe.");
        if (!usuario.Activo)
        {
            throw new ExcepcionNegocio("El usuario responsable está inactivo.");
        }

        var almacen = await _almacenes.ObtenerPorIdAsync(dto.IdAlmacen)
            ?? throw new ExcepcionNoEncontrado("El almacén indicado no existe.");
        if (!almacen.Activo)
        {
            throw new ExcepcionNegocio("El almacén indicado está inactivo.");
        }

        Proveedor? proveedor = null;
        if (dto.IdProveedor.HasValue)
        {
            proveedor = await _proveedores.ObtenerPorIdAsync(dto.IdProveedor.Value)
                ?? throw new ExcepcionNoEncontrado("El proveedor indicado no existe.");
            if (!proveedor.Activo)
            {
                throw new ExcepcionNegocio("El proveedor indicado está inactivo.");
            }
        }

        // Si el mismo producto aparece en varias líneas, se consolidan en una sola.
        var lineas = ConsolidarLineas(dto.Detalles);

        // Validar productos y stock ANTES de tocar la base de datos.
        var detalles = new List<MovimientoDetalle>();
        foreach (var linea in lineas)
        {
            var producto = await _productos.ObtenerPorIdAsync(linea.IdProducto)
                ?? throw new ExcepcionNoEncontrado($"El producto con id {linea.IdProducto} no existe.");

            if (!producto.Activo)
            {
                throw new ExcepcionNegocio($"El producto '{producto.Nombre}' está inactivo.");
            }

            if (dto.Tipo == TipoMovimiento.Salida)
            {
                var registro = await _stock.ObtenerAsync(producto.IdProducto, dto.IdAlmacen);
                var disponible = registro?.Cantidad ?? 0;
                if (linea.Cantidad > disponible)
                {
                    throw new ExcepcionNegocio(
                        $"Stock insuficiente de '{producto.Nombre}' en '{almacen.Nombre}': " +
                        $"disponible {disponible}, solicitado {linea.Cantidad}.");
                }
            }

            // Si no se informa precio, se usa el precio por defecto del producto.
            var precioUnitario = linea.PrecioUnitario > 0
                ? linea.PrecioUnitario
                : (dto.Tipo == TipoMovimiento.Entrada ? producto.PrecioCompra : producto.PrecioVenta);

            detalles.Add(new MovimientoDetalle
            {
                IdProducto = producto.IdProducto,
                Cantidad = linea.Cantidad,
                PrecioUnitario = precioUnitario
            });
        }

        var movimiento = new Movimiento
        {
            Fecha = DateTime.Now,
            Tipo = dto.Tipo,
            Motivo = dto.Motivo?.Trim(),
            DocumentoReferencia = dto.DocumentoReferencia?.Trim(),
            IdUsuario = usuario.IdUsuario,
            IdAlmacen = almacen.IdAlmacen,
            IdProveedor = proveedor?.IdProveedor,
            Detalles = detalles
        };

        await _unidadTrabajo.IniciarTransaccionAsync();
        try
        {
            // 1) Registrar el movimiento con sus detalles.
            await _movimientos.AgregarAsync(movimiento);
            await _unidadTrabajo.GuardarCambiosAsync();

            // 2) Actualizar el stock del almacén afectado.
            foreach (var detalle in movimiento.Detalles)
            {
                await ActualizarStockAsync(detalle.IdProducto, dto.IdAlmacen, dto.Tipo, detalle.Cantidad);
            }

            await _unidadTrabajo.GuardarCambiosAsync();

            // 3) Confirmar todo de forma atómica.
            await _unidadTrabajo.ConfirmarAsync();
        }
        catch
        {
            await _unidadTrabajo.RevertirAsync();
            throw;
        }

        return await ObtenerPorIdAsync(movimiento.IdMovimiento);
    }

    /// <summary>
    /// Suma (entrada) o resta (salida) la cantidad en el stock del almacén,
    /// creando el registro si la entrada es la primera de ese producto allí.
    /// </summary>
    private async Task ActualizarStockAsync(int idProducto, int idAlmacen, TipoMovimiento tipo, int cantidad)
    {
        var registro = await _stock.ObtenerAsync(idProducto, idAlmacen);

        if (registro is null)
        {
            if (tipo == TipoMovimiento.Salida)
            {
                // Nunca debería ocurrir: ya se validó el stock antes.
                throw new ExcepcionNegocio("No existe stock registrado para el producto en el almacén indicado.");
            }

            await _stock.AgregarAsync(new StockPorAlmacen
            {
                IdProducto = idProducto,
                IdAlmacen = idAlmacen,
                Cantidad = cantidad
            });
            return;
        }

        registro.Cantidad = tipo == TipoMovimiento.Entrada
            ? registro.Cantidad + cantidad
            : registro.Cantidad - cantidad;

        _stock.Actualizar(registro);
    }

    /// <summary>Agrupa las líneas repetidas del mismo producto en una sola.</summary>
    private static List<MovimientoDetalleCreacionDTO> ConsolidarLineas(List<MovimientoDetalleCreacionDTO> detalles) =>
        detalles
            .GroupBy(d => d.IdProducto)
            .Select(g => new MovimientoDetalleCreacionDTO
            {
                IdProducto = g.Key,
                Cantidad = g.Sum(x => x.Cantidad),
                PrecioUnitario = g.First().PrecioUnitario
            })
            .ToList();
}
