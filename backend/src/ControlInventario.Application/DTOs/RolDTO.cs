namespace ControlInventario.Application.DTOs;

/// <summary>Datos de un rol.</summary>
public class RolDTO
{
    public int IdRol { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }
}
