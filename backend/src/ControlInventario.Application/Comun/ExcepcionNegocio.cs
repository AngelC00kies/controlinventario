namespace ControlInventario.Application.Comun;

/// <summary>
/// Excepción de negocio: se traduce a una respuesta HTTP 400 con el mensaje.
/// Se lanza desde los servicios cuando una regla de negocio no se cumple.
/// </summary>
public class ExcepcionNegocio : Exception
{
    public ExcepcionNegocio(string mensaje) : base(mensaje)
    {
    }
}
