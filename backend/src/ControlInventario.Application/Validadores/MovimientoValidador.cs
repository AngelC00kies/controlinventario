using ControlInventario.Application.DTOs;
using ControlInventario.Domain.Enums;
using FluentValidation;

namespace ControlInventario.Application.Validadores;

/// <summary>Reglas de validación para registrar movimientos de inventario.</summary>
public class MovimientoCreacionValidador : AbstractValidator<MovimientoCreacionDTO>
{
    public MovimientoCreacionValidador()
    {
        RuleFor(x => x.Tipo)
            .IsInEnum().WithMessage("El tipo de movimiento debe ser Entrada o Salida.");

        RuleFor(x => x.Motivo)
            .MaximumLength(300).WithMessage("El motivo no puede superar los 300 caracteres.");

        RuleFor(x => x.DocumentoReferencia)
            .MaximumLength(50).WithMessage("El documento de referencia no puede superar los 50 caracteres.");

        RuleFor(x => x.IdAlmacen)
            .GreaterThan(0).WithMessage("Debe seleccionar un almacén.");

        RuleFor(x => x.Detalles)
            .NotEmpty().WithMessage("El movimiento debe tener al menos un producto.");

        RuleForEach(x => x.Detalles).ChildRules(detalle =>
        {
            detalle.RuleFor(d => d.IdProducto)
                .GreaterThan(0).WithMessage("Cada línea debe indicar un producto.");

            detalle.RuleFor(d => d.Cantidad)
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor que cero.");

            detalle.RuleFor(d => d.PrecioUnitario)
                .GreaterThanOrEqualTo(0).WithMessage("El precio unitario no puede ser negativo.");
        });

        // Regla de negocio: una entrada con proveedor debe indicar un proveedor válido.
        RuleFor(x => x.IdProveedor)
            .GreaterThan(0).When(x => x.Tipo == TipoMovimiento.Entrada && x.IdProveedor.HasValue)
            .WithMessage("El proveedor indicado no es válido.");
    }
}
