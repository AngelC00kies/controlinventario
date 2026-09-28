namespace ControlInventario.Application.DTOs;

/// <summary>Datos de un usuario (sin contraseña).</summary>
public class UsuarioDTO
{
    public int IdUsuario { get; set; }

    public string NombreUsuario { get; set; } = string.Empty;

    public string NombreCompleto { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public int IdRol { get; set; }

    public string NombreRol { get; set; } = string.Empty;

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }
}

/// <summary>Datos para crear un usuario.</summary>
public class UsuarioCreacionDTO
{
    public string NombreUsuario { get; set; } = string.Empty;

    public string NombreCompleto { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public string Contrasena { get; set; } = string.Empty;

    public int IdRol { get; set; }

    public bool Activo { get; set; } = true;
}

/// <summary>Datos para modificar un usuario. La contraseña es opcional (vacío = sin cambio).</summary>
public class UsuarioActualizacionDTO
{
    public string NombreUsuario { get; set; } = string.Empty;

    public string NombreCompleto { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public string? Contrasena { get; set; }

    public int IdRol { get; set; }

    public bool Activo { get; set; }
}
