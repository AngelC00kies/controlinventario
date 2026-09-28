namespace ControlInventario.Application.DTOs;

/// <summary>Credenciales de inicio de sesión.</summary>
public class SolicitudLoginDTO
{
    public string NombreUsuario { get; set; } = string.Empty;

    public string Contrasena { get; set; } = string.Empty;
}
