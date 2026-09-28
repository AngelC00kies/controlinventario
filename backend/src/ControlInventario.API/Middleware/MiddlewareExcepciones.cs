using System.Net;
using System.Text.Json;
using ControlInventario.Application.Comun;

namespace ControlInventario.API.Middleware;

/// <summary>
/// Middleware global de excepciones: convierte las excepciones de negocio en
/// respuestas HTTP con un mensaje claro y un formato JSON uniforme
/// ({ "mensaje": "..." }), sin exponer detalles internos.
/// </summary>
public class MiddlewareExcepciones
{
    private static readonly JsonSerializerOptions OpcionesJson = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly RequestDelegate _siguiente;
    private readonly ILogger<MiddlewareExcepciones> _logger;

    public MiddlewareExcepciones(RequestDelegate siguiente, ILogger<MiddlewareExcepciones> logger)
    {
        _siguiente = siguiente;
        _logger = logger;
    }

    /// <summary>
    /// Punto de entrada del middleware. El método debe llamarse InvokeAsync
    /// (convención obligatoria de ASP.NET Core).
    /// </summary>
    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await _siguiente(contexto);
        }
        catch (ExcepcionNegocio excepcion)
        {
            await ResponderAsync(contexto, HttpStatusCode.BadRequest, excepcion.Message);
        }
        catch (ExcepcionNoEncontrado excepcion)
        {
            await ResponderAsync(contexto, HttpStatusCode.NotFound, excepcion.Message);
        }
        catch (ExcepcionNoAutorizado excepcion)
        {
            await ResponderAsync(contexto, HttpStatusCode.Unauthorized, excepcion.Message);
        }
        catch (Exception excepcion)
        {
            // Error inesperado: se registra y se responde sin filtrar información.
            _logger.LogError(excepcion, "Error inesperado al procesar la solicitud {Ruta}", contexto.Request.Path);
            await ResponderAsync(contexto, HttpStatusCode.InternalServerError,
                "Ocurrió un error interno. Intente nuevamente o contacte al administrador.");
        }
    }

    private static async Task ResponderAsync(HttpContext contexto, HttpStatusCode estado, string mensaje)
    {
        if (contexto.Response.HasStarted)
        {
            return;
        }

        contexto.Response.StatusCode = (int)estado;
        contexto.Response.ContentType = "application/json; charset=utf-8";
        await contexto.Response.WriteAsync(JsonSerializer.Serialize(new { mensaje }, OpcionesJson));
    }
}
