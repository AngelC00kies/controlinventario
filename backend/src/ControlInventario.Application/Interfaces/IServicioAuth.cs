using ControlInventario.Application.DTOs;

namespace ControlInventario.Application.Interfaces;

/// <summary>Servicio de autenticación y emisión de tokens JWT.</summary>
public interface IServicioAuth
{
    /// <summary>Valida credenciales y devuelve el token JWT con el perfil del usuario.</summary>
    Task<RespuestaAuthDTO> IniciarSesionAsync(SolicitudLoginDTO solicitud);

    /// <summary>Obtiene el perfil del usuario autenticado por su identificador.</summary>
    Task<RespuestaAuthDTO> ObtenerPerfilAsync(int idUsuario);
}
