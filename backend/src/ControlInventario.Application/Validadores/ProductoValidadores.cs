using ControlInventario.Application.DTOs;
using FluentValidation;

namespace ControlInventario.Application.Validadores;

/// <summary>Reglas de validación para crear productos.</summary>
public class ProductoCreacionValidador : AbstractValidator<ProductoCreacionDTO>
{
    public ProductoCreacionValidador()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código del producto es obligatorio.")
            .MaximumLength(30).WithMessage("El código no puede superar los 30 caracteres.");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del producto es obligatorio.")
            .MaximumLength(150).WithMessage("El nombre no puede superar los 150 caracteres.");

        RuleFor(x => x.Descripcion)
            .MaximumLength(500).WithMessage("La descripción no puede superar los 500 caracteres.");

        RuleFor(x => x.IdCategoria)
            .GreaterThan(0).WithMessage("Debe seleccionar una categoría.");

        RuleFor(x => x.IdProveedor)
            .GreaterThan(0).WithMessage("Debe seleccionar un proveedor.");

        RuleFor(x => x.PrecioCompra)
            .GreaterThanOrEqualTo(0).WithMessage("El precio de compra no puede ser negativo.");

        RuleFor(x => x.PrecioVenta)
            .GreaterThanOrEqualTo(0).WithMessage("El precio de venta no puede ser negativo.");

        RuleFor(x => x.UnidadMedida)
            .NotEmpty().WithMessage("La unidad de medida es obligatoria.")
            .MaximumLength(30).WithMessage("La unidad de medida no puede superar los 30 caracteres.");

        RuleFor(x => x.StockMinimo)
            .GreaterThanOrEqualTo(0).WithMessage("El stock mínimo no puede ser negativo.");
    }
}

/// <summary>Reglas de validación para modificar productos.</summary>
public class ProductoActualizacionValidador : AbstractValidator<ProductoActualizacionDTO>
{
    public ProductoActualizacionValidador()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código del producto es obligatorio.")
            .MaximumLength(30).WithMessage("El código no puede superar los 30 caracteres.");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del producto es obligatorio.")
            .MaximumLength(150).WithMessage("El nombre no puede superar los 150 caracteres.");

        RuleFor(x => x.IdCategoria)
            .GreaterThan(0).WithMessage("Debe seleccionar una categoría.");

        RuleFor(x => x.IdProveedor)
            .GreaterThan(0).WithMessage("Debe seleccionar un proveedor.");

        RuleFor(x => x.PrecioCompra)
            .GreaterThanOrEqualTo(0).WithMessage("El precio de compra no puede ser negativo.");

        RuleFor(x => x.PrecioVenta)
            .GreaterThanOrEqualTo(0).WithMessage("El precio de venta no puede ser negativo.");

        RuleFor(x => x.UnidadMedida)
            .NotEmpty().WithMessage("La unidad de medida es obligatoria.");

        RuleFor(x => x.StockMinimo)
            .GreaterThanOrEqualTo(0).WithMessage("El stock mínimo no puede ser negativo.");
    }
}
