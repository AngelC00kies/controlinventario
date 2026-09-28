using ControlInventario.Application.DTOs;
using ControlInventario.Application.Validadores;
using ControlInventario.Domain.Enums;
using FluentAssertions;

namespace ControlInventario.Tests.Validadores;

/// <summary>
/// Pruebas unitarias de las reglas FluentValidation (capa Application).
/// </summary>
public class ValidadoresTests
{
    [Fact]
    public async Task ProductoCreacion_ConDatosValidos_DeberiaSerValido()
    {
        var validador = new ProductoCreacionValidador();
        var dto = new ProductoCreacionDTO
        {
            Codigo = "ELEC-010",
            Nombre = "Producto",
            IdCategoria = 1,
            IdProveedor = 1,
            PrecioCompra = 100,
            PrecioVenta = 200,
            UnidadMedida = "Unidad",
            StockMinimo = 5
        };

        var resultado = await validador.ValidateAsync(dto);

        resultado.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "Nombre")]
    [InlineData("  ", "Nombre")]
    public async Task ProductoCreacion_SinNombre_DeberiaSerInvalido(string nombre, string campo)
    {
        var validador = new ProductoCreacionValidador();
        var dto = new ProductoCreacionDTO
        {
            Codigo = "ELEC-010",
            Nombre = nombre,
            IdCategoria = 1,
            IdProveedor = 1,
            UnidadMedida = "Unidad"
        };

        var resultado = await validador.ValidateAsync(dto);

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.ErrorMessage.Contains(campo, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task ProductoCreacion_ConPreciosNegativos_DeberiaSerInvalido()
    {
        var validador = new ProductoCreacionValidador();
        var dto = new ProductoCreacionDTO
        {
            Codigo = "ELEC-010",
            Nombre = "Producto",
            IdCategoria = 1,
            IdProveedor = 1,
            PrecioCompra = -1,
            PrecioVenta = -2,
            UnidadMedida = "Unidad"
        };

        var resultado = await validador.ValidateAsync(dto);

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().HaveCountGreaterThan(1);
    }

    [Fact]
    public async Task MovimientoCreacion_SinDetalles_DeberiaSerInvalido()
    {
        var validador = new MovimientoCreacionValidador();
        var dto = new MovimientoCreacionDTO
        {
            Tipo = TipoMovimiento.Entrada,
            IdAlmacen = 1,
            Detalles = new List<MovimientoDetalleCreacionDTO>()
        };

        var resultado = await validador.ValidateAsync(dto);

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.ErrorMessage.Contains("producto"));
    }

    [Fact]
    public async Task MovimientoCreacion_ConCantidadNegativa_DeberiaSerInvalido()
    {
        var validador = new MovimientoCreacionValidador();
        var dto = new MovimientoCreacionDTO
        {
            Tipo = TipoMovimiento.Salida,
            IdAlmacen = 1,
            Detalles = new List<MovimientoDetalleCreacionDTO>
            {
                new() { IdProducto = 1, Cantidad = -3, PrecioUnitario = 100 }
            }
        };

        var resultado = await validador.ValidateAsync(dto);

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.ErrorMessage.Contains("cantidad"));
    }

    [Fact]
    public async Task MovimientoCreacion_ConDatosValidos_DeberiaSerValido()
    {
        var validador = new MovimientoCreacionValidador();
        var dto = new MovimientoCreacionDTO
        {
            Tipo = TipoMovimiento.Entrada,
            IdAlmacen = 1,
            Motivo = "Compra",
            Detalles = new List<MovimientoDetalleCreacionDTO>
            {
                new() { IdProducto = 1, Cantidad = 10, PrecioUnitario = 500 }
            }
        };

        var resultado = await validador.ValidateAsync(dto);

        resultado.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task UsuarioCreacion_ConContrasenaCorta_DeberiaSerInvalido()
    {
        var validador = new UsuarioCreacionValidador();
        var dto = new UsuarioCreacionDTO
        {
            NombreUsuario = "usuario1",
            NombreCompleto = "Usuario de Prueba",
            Correo = "usuario@correo.cl",
            Contrasena = "corta",
            IdRol = 1
        };

        var resultado = await validador.ValidateAsync(dto);

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.ErrorMessage.Contains("8 caracteres"));
    }

    [Fact]
    public async Task UsuarioCreacion_ConCorreoInvalido_DeberiaSerInvalido()
    {
        var validador = new UsuarioCreacionValidador();
        var dto = new UsuarioCreacionDTO
        {
            NombreUsuario = "usuario1",
            NombreCompleto = "Usuario de Prueba",
            Correo = "no-es-un-correo",
            Contrasena = "secreta123",
            IdRol = 1
        };

        var resultado = await validador.ValidateAsync(dto);

        resultado.IsValid.Should().BeFalse();
    }
}
