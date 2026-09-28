using ControlInventario.Application.Comun;
using ControlInventario.Application.DTOs;
using ControlInventario.Application.Interfaces;
using ControlInventario.Application.Validadores;
using ControlInventario.Domain.Entidades;
using ControlInventario.Domain.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace ControlInventario.Application.Servicios;

/// <summary>Servicio de gestión de usuarios y roles (solo administración).</summary>
public class ServicioUsuarios : IServicioUsuarios
{
    private readonly IRepositorioUsuarios _usuarios;
    private readonly IRepositorioRoles _roles;
    private readonly IUnidadTrabajo _unidadTrabajo;
    private readonly IPasswordHasher<Usuario> _hasherContrasenas;
    private readonly IValidator<UsuarioCreacionDTO> _validadorCreacion;
    private readonly IValidator<UsuarioActualizacionDTO> _validadorActualizacion;

    public ServicioUsuarios(
        IRepositorioUsuarios usuarios,
        IRepositorioRoles roles,
        IUnidadTrabajo unidadTrabajo,
        IPasswordHasher<Usuario> hasherContrasenas,
        IValidator<UsuarioCreacionDTO> validadorCreacion,
        IValidator<UsuarioActualizacionDTO> validadorActualizacion)
    {
        _usuarios = usuarios;
        _roles = roles;
        _unidadTrabajo = unidadTrabajo;
        _hasherContrasenas = hasherContrasenas;
        _validadorCreacion = validadorCreacion;
        _validadorActualizacion = validadorActualizacion;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<UsuarioDTO>> ListarAsync()
    {
        var usuarios = await _usuarios.ListarConRolesAsync();
        return usuarios.Select(Mapeador.AUsuarioDTO).ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RolDTO>> ObtenerRolesAsync()
    {
        var roles = await _roles.ListarAsync();
        return roles.Select(r => new RolDTO
        {
            IdRol = r.IdRol,
            Nombre = r.Nombre,
            Descripcion = r.Descripcion
        }).ToList();
    }

    /// <inheritdoc />
    public async Task<UsuarioDTO> ObtenerPorIdAsync(int id)
    {
        var usuario = await _usuarios.ObtenerPorIdAsync(id)
            ?? throw new ExcepcionNoEncontrado("Usuario no encontrado.");

        return Mapeador.AUsuarioDTO(usuario);
    }

    /// <inheritdoc />
    public async Task<UsuarioDTO> CrearAsync(UsuarioCreacionDTO dto)
    {
        await ValidarAsync(_validadorCreacion, dto);

        if (await _usuarios.ExisteUsuarioAsync(dto.NombreUsuario, dto.Correo))
        {
            throw new ExcepcionNegocio("El nombre de usuario o el correo ya están registrados.");
        }

        await VerificarRolAsync(dto.IdRol);

        var usuario = new Usuario
        {
            NombreUsuario = dto.NombreUsuario.Trim(),
            NombreCompleto = dto.NombreCompleto.Trim(),
            Correo = dto.Correo.Trim(),
            IdRol = dto.IdRol,
            Activo = dto.Activo
        };
        usuario.ContrasenaHash = _hasherContrasenas.HashPassword(usuario, dto.Contrasena);

        await _usuarios.AgregarAsync(usuario);
        await _unidadTrabajo.GuardarCambiosAsync();

        return await ObtenerPorIdAsync(usuario.IdUsuario);
    }

    /// <inheritdoc />
    public async Task<UsuarioDTO> ActualizarAsync(int id, UsuarioActualizacionDTO dto)
    {
        await ValidarAsync(_validadorActualizacion, dto);

        var usuario = await _usuarios.ObtenerPorIdAsync(id)
            ?? throw new ExcepcionNoEncontrado("Usuario no encontrado.");

        if (await _usuarios.ExisteUsuarioAsync(dto.NombreUsuario, dto.Correo, id))
        {
            throw new ExcepcionNegocio("El nombre de usuario o el correo ya están registrados por otro usuario.");
        }

        await VerificarRolAsync(dto.IdRol);

        usuario.NombreUsuario = dto.NombreUsuario.Trim();
        usuario.NombreCompleto = dto.NombreCompleto.Trim();
        usuario.Correo = dto.Correo.Trim();
        usuario.IdRol = dto.IdRol;
        usuario.Activo = dto.Activo;

        // La contraseña solo cambia si se envía una nueva.
        if (!string.IsNullOrWhiteSpace(dto.Contrasena))
        {
            usuario.ContrasenaHash = _hasherContrasenas.HashPassword(usuario, dto.Contrasena);
        }

        _usuarios.Actualizar(usuario);
        await _unidadTrabajo.GuardarCambiosAsync();

        return await ObtenerPorIdAsync(id);
    }

    /// <inheritdoc />
    public async Task EliminarAsync(int id)
    {
        var usuario = await _usuarios.ObtenerPorIdAsync(id)
            ?? throw new ExcepcionNoEncontrado("Usuario no encontrado.");

        // Baja lógica para conservar el historial de movimientos del usuario.
        usuario.Activo = false;
        _usuarios.Actualizar(usuario);
        await _unidadTrabajo.GuardarCambiosAsync();
    }

    /// <summary>Valida que el rol indicado exista.</summary>
    private async Task VerificarRolAsync(int idRol)
    {
        _ = await _roles.ObtenerPorIdAsync(idRol)
            ?? throw new ExcepcionNegocio("El rol indicado no existe.");
    }

    private static async Task ValidarAsync<T>(IValidator<T> validador, T dto)
    {
        var validacion = await validador.ValidateAsync(dto);
        if (!validacion.IsValid)
        {
            throw new ExcepcionNegocio(string.Join(" ", validacion.Errors.Select(e => e.ErrorMessage)));
        }
    }
}
