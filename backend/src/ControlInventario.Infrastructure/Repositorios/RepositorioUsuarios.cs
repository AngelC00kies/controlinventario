using ControlInventario.Domain.Entidades;
using ControlInventario.Domain.Interfaces;
using ControlInventario.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace ControlInventario.Infrastructure.Repositorios;

/// <summary>Repositorio de usuarios (autenticación y administración).</summary>
public class RepositorioUsuarios : RepositorioBase<Usuario>, IRepositorioUsuarios
{
    public RepositorioUsuarios(ContextoControlInventario contexto) : base(contexto)
    {
    }

    /// <inheritdoc />
    public override async Task<Usuario?> ObtenerPorIdAsync(int id) =>
        await Conjunto.Include(u => u.Rol).FirstOrDefaultAsync(u => u.IdUsuario == id);

    /// <inheritdoc />
    public async Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario) =>
        await Conjunto
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);

    /// <inheritdoc />
    public async Task<Usuario?> ObtenerPorCorreoAsync(string correo) =>
        await Conjunto
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.Correo == correo);

    /// <inheritdoc />
    public Task<bool> ExisteUsuarioAsync(string nombreUsuario, string correo, int? idUsuarioExcluir = null) =>
        Conjunto.AnyAsync(u =>
            (u.NombreUsuario == nombreUsuario || u.Correo == correo) &&
            (!idUsuarioExcluir.HasValue || u.IdUsuario != idUsuarioExcluir.Value));

    /// <inheritdoc />
    public async Task<IReadOnlyList<Usuario>> ListarConRolesAsync() =>
        await Conjunto
            .AsNoTracking()
            .Include(u => u.Rol)
            .OrderBy(u => u.NombreUsuario)
            .ToListAsync();
}
