namespace ControlInventario.Domain.Entidades;

/// <summary>
/// Rol de usuario del sistema (Administrador, Usuario, Auditor).
/// </summary>
public class Rol
{
    public int IdRol { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    /// <summary>Usuarios que poseen este rol.</summary>
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
