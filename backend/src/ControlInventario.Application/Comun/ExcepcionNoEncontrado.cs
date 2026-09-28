namespace ControlInventario.Application.Comun;

/// <summary>
/// Recurso solicitado que no existe: se traduce a una respuesta HTTP 404.
/// </summary>
public class ExcepcionNoEncontrado : Exception
{
    public ExcepcionNoEncontrado(string mensaje) : base(mensaje)
    {
    }
}
