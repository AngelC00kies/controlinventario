namespace ControlInventario.Application.Comun;

/// <summary>
/// Opciones de configuración del token JWT, leídas desde appsettings.json.
/// </summary>
public class OpcionesJwt
{
    /// <summary>Clave simétrica usada para firmar el token (mínimo 32 caracteres).</summary>
    public string ClaveSecreta { get; set; } = string.Empty;

    /// <summary>Emisor del token (issuer).</summary>
    public string Emisor { get; set; } = "ControlInventario";

    /// <summary>Audiencia del token (audience).</summary>
    public string Audiencia { get; set; } = "ControlInventarioFrontend";

    /// <summary>Duración del token en minutos.</summary>
    public int DuracionMinutos { get; set; } = 480;
}
