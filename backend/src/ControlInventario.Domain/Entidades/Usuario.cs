namespace ControlInventario.Domain.Entidades;

/// <summary>
/// Usuario del sistema. Autentica con JWT y obtiene permisos según su rol.
/// </summary>
public class Usuario
{
    public int IdUsuario { get; set; }

    public string NombreUsuario { get; set; } = string.Empty;

    public string NombreCompleto { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    /// <summary>Hash PBKDF2 generado con ASP.NET Core Identity (PasswordHasher).</summary>
    public string ContrasenaHash { get; set; } = string.Empty;

    public int IdRol { get; set; }

    /// <summary>Rol asignado al usuario.</summary>
    public Rol? Rol { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    /// <summary>Movimientos registrados por este usuario.</summary>
    public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}
