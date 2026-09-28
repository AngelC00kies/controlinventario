namespace ControlInventario.Application.DTOs;

/// <summary>Respuesta del login: token JWT y datos del usuario autenticado.</summary>
public class RespuestaAuthDTO
{
    public string Token { get; set; } = string.Empty;

    public int IdUsuario { get; set; }

    public string NombreUsuario { get; set; } = string.Empty;

    public string NombreCompleto { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public string Rol { get; set; } = string.Empty;

    /// <summary>Fecha y hora local de expiración del token.</summary>
    public DateTime ExpiraEn { get; set; }
}
