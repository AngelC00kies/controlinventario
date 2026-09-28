namespace ControlInventario.Application.Comun;

/// <summary>
/// Credenciales o permisos inválidos: se traduce a una respuesta HTTP 401.
/// </summary>
public class ExcepcionNoAutorizado : Exception
{
    public ExcepcionNoAutorizado(string mensaje) : base(mensaje)
    {
    }
}
