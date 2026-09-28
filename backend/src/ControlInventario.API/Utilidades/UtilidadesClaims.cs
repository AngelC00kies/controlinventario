using System.Security.Claims;

namespace ControlInventario.API.Utilidades;

/// <summary>
/// Utilidades sobre los claims del usuario autenticado (token JWT).
/// </summary>
public static class UtilidadesClaims
{
    /// <summary>Obtiene el IdUsuario desde el claim "sub" del token.</summary>
    public static int ObtenerIdUsuario(ClaimsPrincipal usuario)
    {
        var valor = usuario.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(valor) || !int.TryParse(valor, out var idUsuario))
        {
            throw new UnauthorizedAccessException("El token no contiene un identificador de usuario válido.");
        }

        return idUsuario;
    }
}
