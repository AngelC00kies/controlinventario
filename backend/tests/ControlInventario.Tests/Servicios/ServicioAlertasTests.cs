using ControlInventario.Application.Servicios;
using ControlInventario.Domain.Entidades;
using ControlInventario.Domain.Interfaces;
using ControlInventario.Tests.Config;
using FluentAssertions;
using Moq;

namespace ControlInventario.Tests.Servicios;

/// <summary>
/// Pruebas unitarias del ServicioAlertas: detección y orden de productos con stock bajo.
/// </summary>
public class ServicioAlertasTests
{
    private readonly Mock<IRepositorioProductos> _productos = new();
    private readonly ServicioAlertas _servicio;

    public ServicioAlertasTests()
    {
        _servicio = new ServicioAlertas(_productos.Object);
    }

    [Fact]
    public async Task ObtenerStockBajo_DeberiaDevolverSoloLosProductosCriticos()
    {
        // Mínimo 10: con 25 no es crítico; con 4 y 0 sí lo son.
        _productos.Setup(p => p.ObtenerStockCriticoAsync())
            .ReturnsAsync(new List<Producto>
            {
                DatosPrueba.Producto(idProducto: 1, stockMinimo: 10, cantidades: 4),
                DatosPrueba.Producto(idProducto: 2, stockMinimo: 10, cantidades: 0)
            });

        var resultado = await _servicio.ObtenerStockBajoAsync();

        resultado.Should().HaveCount(2);
        resultado.Should().NotContain(a => a.StockActual > a.StockMinimo);
    }

    [Fact]
    public async Task ObtenerStockBajo_DeberiaOrdenarPorMayorDiferencia()
    {
        _productos.Setup(p => p.ObtenerStockCriticoAsync())
            .ReturnsAsync(new List<Producto>
            {
                // Stock 7, mínimo 10 => diferencia 3
                DatosPrueba.Producto(idProducto: 1, stockMinimo: 10, cantidades: 7),
                // Stock 0, mínimo 10 => diferencia 10
                DatosPrueba.Producto(idProducto: 2, stockMinimo: 10, cantidades: 0)
            });

        var resultado = await _servicio.ObtenerStockBajoAsync();

        resultado[0].IdProducto.Should().Be(2);
        resultado[0].Diferencia.Should().Be(10);
        resultado[1].Diferencia.Should().Be(3);
    }

    [Fact]
    public async Task ObtenerStockBajo_SinCriticos_DeberiaDevolverListaVacia()
    {
        _productos.Setup(p => p.ObtenerStockCriticoAsync())
            .ReturnsAsync(new List<Producto>());

        var resultado = await _servicio.ObtenerStockBajoAsync();

        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task ObtenerStockBajo_DeberiaIncluirLosDatosDelProducto()
    {
        _productos.Setup(p => p.ObtenerStockCriticoAsync())
            .ReturnsAsync(new List<Producto> { DatosPrueba.Producto(stockMinimo: 10, cantidades: 2) });

        var resultado = await _servicio.ObtenerStockBajoAsync();

        var alerta = resultado.Single();
        alerta.Codigo.Should().Be("ELEC-001");
        alerta.Nombre.Should().NotBeNullOrEmpty();
        alerta.NombreCategoria.Should().Be("Electrónica");
        alerta.StockMinimo.Should().Be(10);
        alerta.StockActual.Should().Be(2);
        alerta.Diferencia.Should().Be(8);
    }
}
