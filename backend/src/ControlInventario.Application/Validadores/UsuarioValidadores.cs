using ControlInventario.Application.DTOs;
using FluentValidation;

namespace ControlInventario.Application.Validadores;

/// <summary>Reglas de validación para crear usuarios.</summary>
public class UsuarioCreacionValidador : AbstractValidator<UsuarioCreacionDTO>
{
    public UsuarioCreacionValidador()
    {
        RuleFor(x => x.NombreUsuario)
            .NotEmpty().WithMessage("El nombre de usuario es obligatorio.")
            .MinimumLength(3).WithMessage("El nombre de usuario debe tener al menos 3 caracteres.")
            .MaximumLength(50).WithMessage("El nombre de usuario no puede superar los 50 caracteres.");

        RuleFor(x => x.NombreCompleto)
            .NotEmpty().WithMessage("El nombre completo es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre completo no puede superar los 100 caracteres.");

        RuleFor(x => x.Correo)
            .NotEmpty().WithMessage("El correo es obligatorio.")
            .EmailAddress().WithMessage("El correo no es válido.")
            .MaximumLength(100).WithMessage("El correo no puede superar los 100 caracteres.");

        RuleFor(x => x.Contrasena)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.");

        RuleFor(x => x.IdRol)
            .GreaterThan(0).WithMessage("Debe seleccionar un rol.");
    }
}

/// <summary>Reglas de validación para modificar usuarios.</summary>
public class UsuarioActualizacionValidador : AbstractValidator<UsuarioActualizacionDTO>
{
    public UsuarioActualizacionValidador()
    {
        RuleFor(x => x.NombreUsuario)
            .NotEmpty().WithMessage("El nombre de usuario es obligatorio.")
            .MaximumLength(50).WithMessage("El nombre de usuario no puede superar los 50 caracteres.");

        RuleFor(x => x.NombreCompleto)
            .NotEmpty().WithMessage("El nombre completo es obligatorio.");

        RuleFor(x => x.Correo)
            .NotEmpty().WithMessage("El correo es obligatorio.")
            .EmailAddress().WithMessage("El correo no es válido.");

        // La contraseña es opcional al modificar: si viene, debe cumplir las reglas.
        RuleFor(x => x.Contrasena)
            .MinimumLength(8).When(x => !string.IsNullOrWhiteSpace(x.Contrasena))
            .WithMessage("La contraseña debe tener al menos 8 caracteres.");

        RuleFor(x => x.IdRol)
            .GreaterThan(0).WithMessage("Debe seleccionar un rol.");
    }
}

/// <summary>Reglas de validación para el inicio de sesión.</summary>
public class SolicitudLoginValidador : AbstractValidator<SolicitudLoginDTO>
{
    public SolicitudLoginValidador()
    {
        RuleFor(x => x.NombreUsuario)
            .NotEmpty().WithMessage("El nombre de usuario es obligatorio.");

        RuleFor(x => x.Contrasena)
            .NotEmpty().WithMessage("La contraseña es obligatoria.");
    }
}
