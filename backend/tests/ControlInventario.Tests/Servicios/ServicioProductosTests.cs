using ControlInventario.Application.Comun;
using ControlInventario.Application.Servicios;
using ControlInventario.Application.Validadores;
using ControlInventario.Domain.Entidades;
using ControlInventario.Domain.Interfaces;
using ControlInventario.Tests.Config;
using FluentAssertions;
using Moq;

namespace ControlInventario.Tests.Servicios;

/// <summary>
/// Pruebas unitarias del ServicioProductos: alta, modificación, baja lógica,
/// código duplicado y cálculo de stock.
/// </summary>
public class ServicioProductosTests
{
    private readonly Mock<IRepositorioProductos> _productos = new();
    private readonly Mock<IRepositorioCategorias> _categorias = new();
    private readonly Mock<IRepositorioProveedores> _proveedores = new();
    private readonly Mock<IRepositorioStockPorAlmacen> _stock = new();
    private readonly Mock<IUnidadTrabajo> _unidadTrabajo = new();

    private readonly ServicioProductos _servicio;

    public ServicioProductosTests()
    {
        _servicio = new ServicioProductos(
            _productos.Object,
            _categorias.Object,
            _proveedores.Object,
            _stock.Object,
            _unidadTrabajo.Object,
            new ProductoCreacionValidador(),
            new ProductoActualizacionValidador());

        _categorias.Setup(c => c.ObtenerPorIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Categoria { IdCategoria = 1, Nombre = "Electrónica", Activo = true });
        _proveedores.Setup(p => p.ObtenerPorIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Proveedor { IdProveedor = 1, Nombre = "Proveedor", Activo = true });
        _productos.Setup(p => p.ObtenerCompletoAsync(It.IsAny<int>()))
            .ReturnsAsync(DatosPrueba.Producto());
    }

    private static ControlInventario.Application.DTOs.ProductoCreacionDTO NuevoProducto(
        string codigo = "ELEC-999") => new()
    {
        Codigo = codigo,
        Nombre = "Producto nuevo",
        Descripcion = "Descripción",
        IdCategoria = 1,
        IdProveedor = 1,
        PrecioCompra = 500,
        PrecioVenta = 800,
        UnidadMedida = "Unidad",
        StockMinimo = 5
    };

    [Fact]
    public async Task Crear_ConCodigoRepetido_DeberiaLanzarExcepcionDeNegocio()
    {
        _productos.Setup(p => p.ExisteCodigoAsync("ELEC-999", null)).ReturnsAsync(true);

        var accion = async () => await _servicio.CrearAsync(NuevoProducto());

        await accion.Should().ThrowAsync<ExcepcionNegocio>()
            .WithMessage("*código*");
        _productos.Verify(p => p.AgregarAsync(It.IsAny<Producto>()), Times.Never);
    }

    [Fact]
    public async Task Crear_ProductoValido_DeberiaAgregarloYGuardarCambios()
    {
        _productos.Setup(p => p.ExisteCodigoAsync(It.IsAny<string>(), null)).ReturnsAsync(false);
        _productos.Setup(p => p.AgregarAsync(It.IsAny<Producto>()))
            .Callback<Producto>(p => p.IdProducto = 99);

        var resultado = await _servicio.CrearAsync(NuevoProducto());

        _productos.Verify(p => p.AgregarAsync(It.Is<Producto>(x => x.Activo && x.Codigo == "ELEC-999")), Times.Once);
        _unidadTrabajo.Verify(u => u.GuardarCambiosAsync(), Times.Once);
        resultado.Should().NotBeNull();
    }

    [Fact]
    public async Task Crear_ConCategoriaInexistente_DeberiaLanzarExcepcionDeNegocio()
    {
        _productos.Setup(p => p.ExisteCodigoAsync(It.IsAny<string>(), null)).ReturnsAsync(false);
        _categorias.Setup(c => c.ObtenerPorIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Categoria?)null);

        var accion = async () => await _servicio.CrearAsync(NuevoProducto());

        await accion.Should().ThrowAsync<ExcepcionNegocio>()
            .WithMessage("*categoría*");
    }

    [Fact]
    public async Task Crear_ConStockMinimoNegativo_DeberiaLanzarExcepcionDeNegocio()
    {
        var dto = NuevoProducto();
        dto.StockMinimo = -1;
        _productos.Setup(p => p.ExisteCodigoAsync(It.IsAny<string>(), null)).ReturnsAsync(false);

        var accion = async () => await _servicio.CrearAsync(dto);

        await accion.Should().ThrowAsync<ExcepcionNegocio>()
            .WithMessage("*stock mínimo*");
    }

    [Fact]
    public async Task Actualizar_ProductoInexistente_DeberiaLanzarNoEncontrado()
    {
        _productos.Setup(p => p.ObtenerPorIdAsync(404)).ReturnsAsync((Producto?)null);

        var accion = async () => await _servicio.ActualizarAsync(404, new()
        {
            Codigo = "X-1",
            Nombre = "Nombre",
            IdCategoria = 1,
            IdProveedor = 1,
            PrecioCompra = 1,
            PrecioVenta = 2,
            UnidadMedida = "Unidad",
            StockMinimo = 1,
            Activo = true
        });

        await accion.Should().ThrowAsync<ExcepcionNoEncontrado>();
    }

    [Fact]
    public async Task Eliminar_DeberiaRealizarBajaLogicaSinBorrarElRegistro()
    {
        var producto = DatosPrueba.Producto();
        _productos.Setup(p => p.ObtenerPorIdAsync(1)).ReturnsAsync(producto);

        await _servicio.EliminarAsync(1);

        producto.Activo.Should().BeFalse();
        _productos.Verify(p => p.Eliminar(It.IsAny<Producto>()), Times.Never);
        _unidadTrabajo.Verify(u => u.GuardarCambiosAsync(), Times.Once);
    }

    [Fact]
    public async Task Listar_DeberiaCalcularElStockTotalYElEstadoCritico()
    {
        // Stock: 25 + 12 = 37 > mínimo 10 => no crítico.
        _productos.Setup(p => p.BuscarAsync(null, null, null, true))
            .ReturnsAsync(new List<Producto> { DatosPrueba.Producto(cantidades: new[] { 25, 12 }) });

        var resultado = await _servicio.ListarAsync(null, null, null);

        resultado.Should().HaveCount(1);
        resultado[0].StockActual.Should().Be(37);
        resultado[0].StockCritico.Should().BeFalse();
    }

    [Fact]
    public async Task Listar_ConStockIgualAlMinimo_DeberiaMarcarStockCritico()
    {
        // Stock: 10 == mínimo 10 => crítico (regla: stock <= mínimo).
        _productos.Setup(p => p.BuscarAsync(null, null, null, true))
            .ReturnsAsync(new List<Producto> { DatosPrueba.Producto(stockMinimo: 10, cantidades: 10) });

        var resultado = await _servicio.ListarAsync(null, null, null);

        resultado[0].StockCritico.Should().BeTrue();
    }

    [Fact]
    public async Task ObtenerStock_DeberiaDevolverElDesglosePorAlmacen()
    {
        _productos.Setup(p => p.ObtenerPorIdAsync(1)).ReturnsAsync(DatosPrueba.Producto());
        _stock.Setup(s => s.ListarPorProductoAsync(1))
            .ReturnsAsync(new List<StockPorAlmacen>
            {
                new() { IdAlmacen = 1, Almacen = new Almacen { IdAlmacen = 1, Nombre = "Central" }, Cantidad = 20 },
                new() { IdAlmacen = 2, Almacen = new Almacen { IdAlmacen = 2, Nombre = "Sucursal" }, Cantidad = 5 }
            });

        var resultado = await _servicio.ObtenerStockAsync(1);

        resultado.Should().HaveCount(2);
        resultado.Sum(s => s.Cantidad).Should().Be(25);
        resultado[1].NombreAlmacen.Should().Be("Sucursal");
    }
}
