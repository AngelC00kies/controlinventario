using ControlInventario.Application.Comun;
using ControlInventario.Application.DTOs;
using ControlInventario.Application.Servicios;
using ControlInventario.Application.Validadores;
using ControlInventario.Domain.Entidades;
using ControlInventario.Domain.Enums;
using ControlInventario.Domain.Interfaces;
using ControlInventario.Tests.Config;
using FluentAssertions;
using Moq;

namespace ControlInventario.Tests.Servicios;

/// <summary>
/// Pruebas unitarias del ServicioMovimientos: entrada/salida de stock,
/// validaciones y transaccionalidad.
/// </summary>
public class ServicioMovimientosTests
{
    private readonly Mock<IRepositorioMovimientos> _movimientos = new();
    private readonly Mock<IRepositorioProductos> _productos = new();
    private readonly Mock<IRepositorioAlmacenes> _almacenes = new();
    private readonly Mock<IRepositorioProveedores> _proveedores = new();
    private readonly Mock<IRepositorioUsuarios> _usuarios = new();
    private readonly Mock<IRepositorioStockPorAlmacen> _stock = new();
    private readonly Mock<IUnidadTrabajo> _unidadTrabajo = new();

    private readonly ServicioMovimientos _servicio;

    public ServicioMovimientosTests()
    {
        _servicio = new ServicioMovimientos(
            _movimientos.Object,
            _productos.Object,
            _almacenes.Object,
            _proveedores.Object,
            _usuarios.Object,
            _stock.Object,
            _unidadTrabajo.Object,
            new MovimientoCreacionValidador());

        // Preparación por defecto: usuario, almacén y producto válidos.
        _usuarios.Setup(u => u.ObtenerPorIdAsync(It.IsAny<int>()))
            .ReturnsAsync(DatosPrueba.Usuario());
        _almacenes.Setup(a => a.ObtenerPorIdAsync(It.IsAny<int>()))
            .ReturnsAsync(DatosPrueba.Almacen());
        _productos.Setup(p => p.ObtenerPorIdAsync(It.IsAny<int>()))
            .ReturnsAsync(DatosPrueba.Producto(cantidades: 20));

        // El movimiento se devuelve completo al final de la creación.
        _movimientos.Setup(m => m.ObtenerConDetallesAsync(It.IsAny<int>()))
            .ReturnsAsync(new Movimiento
            {
                IdMovimiento = 1,
                Tipo = TipoMovimiento.Entrada,
                Detalles = new List<MovimientoDetalle>()
            });
    }

    [Fact]
    public async Task CrearEntrada_DeberiaAumentarElStockDelAlmacen()
    {
        var registroStock = new StockPorAlmacen { IdProducto = 1, IdAlmacen = 1, Cantidad = 10 };
        _stock.Setup(s => s.ObtenerAsync(1, 1)).ReturnsAsync(registroStock);

        var resultado = await _servicio.CrearAsync(DatosPrueba.Movimiento(cantidad: 5), idUsuario: 1);

        registroStock.Cantidad.Should().Be(15);
        _stock.Verify(s => s.Actualizar(registroStock), Times.Once);
        _unidadTrabajo.Verify(u => u.IniciarTransaccionAsync(), Times.Once);
        _unidadTrabajo.Verify(u => u.ConfirmarAsync(), Times.Once);
        _unidadTrabajo.Verify(u => u.RevertirAsync(), Times.Never);
        resultado.Tipo.Should().Be("Entrada");
    }

    [Fact]
    public async Task CrearSalida_DeberiaDisminuirElStockDelAlmacen()
    {
        var registroStock = new StockPorAlmacen { IdProducto = 1, IdAlmacen = 1, Cantidad = 10 };
        _stock.Setup(s => s.ObtenerAsync(1, 1)).ReturnsAsync(registroStock);

        await _servicio.CrearAsync(DatosPrueba.Movimiento(TipoMovimiento.Salida, cantidad: 4), idUsuario: 1);

        registroStock.Cantidad.Should().Be(6);
        _stock.Verify(s => s.Actualizar(registroStock), Times.Once);
        _unidadTrabajo.Verify(u => u.ConfirmarAsync(), Times.Once);
    }

    [Fact]
    public async Task CrearSalida_ConStockInsuficiente_DeberiaLanzarExcepcionDeNegocio()
    {
        var registroStock = new StockPorAlmacen { IdProducto = 1, IdAlmacen = 1, Cantidad = 3 };
        _stock.Setup(s => s.ObtenerAsync(1, 1)).ReturnsAsync(registroStock);

        var accion = async () => await _servicio.CrearAsync(
            DatosPrueba.Movimiento(TipoMovimiento.Salida, cantidad: 10), idUsuario: 1);

        await accion.Should().ThrowAsync<ExcepcionNegocio>()
            .WithMessage("*Stock insuficiente*");

        // No se confirma ninguna transacción ante el error.
        _unidadTrabajo.Verify(u => u.ConfirmarAsync(), Times.Never);
        _movimientos.Verify(m => m.AgregarAsync(It.IsAny<Movimiento>()), Times.Never);
    }

    [Fact]
    public async Task CrearSalida_SinRegistroDeStock_DeberiaLanzarExcepcionDeNegocio()
    {
        _stock.Setup(s => s.ObtenerAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((StockPorAlmacen?)null);

        var accion = async () => await _servicio.CrearAsync(
            DatosPrueba.Movimiento(TipoMovimiento.Salida, cantidad: 1), idUsuario: 1);

        await accion.Should().ThrowAsync<ExcepcionNegocio>();
    }

    [Fact]
    public async Task CrearEntrada_PrimeraVezEnAlmacen_DeberiaCrearElRegistroDeStock()
    {
        _stock.Setup(s => s.ObtenerAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((StockPorAlmacen?)null);

        await _servicio.CrearAsync(DatosPrueba.Movimiento(TipoMovimiento.Entrada, cantidad: 7), idUsuario: 1);

        _stock.Verify(s => s.AgregarAsync(It.Is<StockPorAlmacen>(x => x.Cantidad == 7)), Times.Once);
        _unidadTrabajo.Verify(u => u.ConfirmarAsync(), Times.Once);
    }

    [Fact]
    public async Task Crear_ConCantidadCero_DeberiaLanzarExcepcionDeNegocio()
    {
        var solicitud = DatosPrueba.Movimiento(cantidad: 0);

        var accion = async () => await _servicio.CrearAsync(solicitud, idUsuario: 1);

        await accion.Should().ThrowAsync<ExcepcionNegocio>()
            .WithMessage("*cantidad debe ser mayor que cero*");
    }

    [Fact]
    public async Task Crear_SinDetalles_DeberiaLanzarExcepcionDeNegocio()
    {
        var solicitud = DatosPrueba.Movimiento();
        solicitud.Detalles = new List<MovimientoDetalleCreacionDTO>();

        var accion = async () => await _servicio.CrearAsync(solicitud, idUsuario: 1);

        await accion.Should().ThrowAsync<ExcepcionNegocio>();
    }

    [Fact]
    public async Task Crear_EnAlmacenInactivo_DeberiaLanzarExcepcionDeNegocio()
    {
        _almacenes.Setup(a => a.ObtenerPorIdAsync(It.IsAny<int>()))
            .ReturnsAsync(DatosPrueba.Almacen(activo: false));

        var accion = async () => await _servicio.CrearAsync(DatosPrueba.Movimiento(), idUsuario: 1);

        await accion.Should().ThrowAsync<ExcepcionNegocio>()
            .WithMessage("*inactivo*");
    }

    [Fact]
    public async Task Crear_Entrada_DeberiaUsarElPrecioDeCompraCuandoNoSeInformaPrecio()
    {
        var registroStock = new StockPorAlmacen { IdProducto = 1, IdAlmacen = 1, Cantidad = 0 };
        _stock.Setup(s => s.ObtenerAsync(1, 1)).ReturnsAsync(registroStock);

        MovimientoDetalle? detalleRegistrado = null;
        _movimientos
            .Setup(m => m.AgregarAsync(It.IsAny<Movimiento>()))
            .Callback<Movimiento>(m => detalleRegistrado = m.Detalles.First());

        await _servicio.CrearAsync(DatosPrueba.Movimiento(precioUnitario: 0), idUsuario: 1);

        // El producto de prueba tiene PrecioCompra = 1000.
        detalleRegistrado!.PrecioUnitario.Should().Be(1000);
    }

    [Fact]
    public async Task Crear_CuandoFallueElStock_NoDeberiaConfirmarLaTransaccion()
    {
        _productos.Setup(p => p.ObtenerPorIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Producto?)null);

        var accion = async () => await _servicio.CrearAsync(DatosPrueba.Movimiento(), idUsuario: 1);

        await accion.Should().ThrowAsync<ExcepcionNoEncontrado>();
        _unidadTrabajo.Verify(u => u.ConfirmarAsync(), Times.Never);
    }
}
