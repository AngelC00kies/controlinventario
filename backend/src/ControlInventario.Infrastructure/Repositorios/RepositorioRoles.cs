using ControlInventario.Domain.Entidades;
using ControlInventario.Domain.Interfaces;
using ControlInventario.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace ControlInventario.Infrastructure.Repositorios;

/// <summary>Repositorio de roles.</summary>
public class RepositorioRoles : RepositorioBase<Rol>, IRepositorioRoles
{
    public RepositorioRoles(ContextoControlInventario contexto) : base(contexto)
    {
    }

    /// <inheritdoc />
    public async Task<Rol?> ObtenerPorNombreAsync(string nombre) =>
        await Conjunto.FirstOrDefaultAsync(r => r.Nombre == nombre);
}
