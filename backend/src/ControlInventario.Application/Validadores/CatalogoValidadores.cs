using ControlInventario.Application.DTOs;
using FluentValidation;

namespace ControlInventario.Application.Validadores;

/// <summary>Reglas de validación para crear categorías.</summary>
public class CategoriaCreacionValidador : AbstractValidator<CategoriaCreacionDTO>
{
    public CategoriaCreacionValidador()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre de la categoría es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.Descripcion)
            .MaximumLength(300).WithMessage("La descripción no puede superar los 300 caracteres.");
    }
}

/// <summary>Reglas de validación para modificar categorías.</summary>
public class CategoriaActualizacionValidador : AbstractValidator<CategoriaActualizacionDTO>
{
    public CategoriaActualizacionValidador()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre de la categoría es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.Descripcion)
            .MaximumLength(300).WithMessage("La descripción no puede superar los 300 caracteres.");
    }
}

/// <summary>Reglas de validación para crear proveedores.</summary>
public class ProveedorCreacionValidador : AbstractValidator<ProveedorCreacionDTO>
{
    public ProveedorCreacionValidador()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del proveedor es obligatorio.")
            .MaximumLength(150).WithMessage("El nombre no puede superar los 150 caracteres.");

        RuleFor(x => x.Correo)
            .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Correo))
            .WithMessage("El correo del proveedor no es válido.");

        RuleFor(x => x.Telefono)
            .MaximumLength(30).WithMessage("El teléfono no puede superar los 30 caracteres.");
    }
}

/// <summary>Reglas de validación para modificar proveedores.</summary>
public class ProveedorActualizacionValidador : AbstractValidator<ProveedorActualizacionDTO>
{
    public ProveedorActualizacionValidador()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del proveedor es obligatorio.")
            .MaximumLength(150).WithMessage("El nombre no puede superar los 150 caracteres.");

        RuleFor(x => x.Correo)
            .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Correo))
            .WithMessage("El correo del proveedor no es válido.");
    }
}

/// <summary>Reglas de validación para crear almacenes.</summary>
public class AlmacenCreacionValidador : AbstractValidator<AlmacenCreacionDTO>
{
    public AlmacenCreacionValidador()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del almacén es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.Ubicacion)
            .MaximumLength(200).WithMessage("La ubicación no puede superar los 200 caracteres.");
    }
}

/// <summary>Reglas de validación para modificar almacenes.</summary>
public class AlmacenActualizacionValidador : AbstractValidator<AlmacenActualizacionDTO>
{
    public AlmacenActualizacionValidador()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del almacén es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");
    }
}
