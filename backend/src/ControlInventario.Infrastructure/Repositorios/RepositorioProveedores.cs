using ControlInventario.Domain.Entidades;
using ControlInventario.Domain.Interfaces;
using ControlInventario.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace ControlInventario.Infrastructure.Repositorios;

/// <summary>Repositorio de proveedores.</summary>
public class RepositorioProveedores : RepositorioBase<Proveedor>, IRepositorioProveedores
{
    public RepositorioProveedores(ContextoControlInventario contexto) : base(contexto)
    {
    }

    /// <inheritdoc />
    public Task<bool> ExisteNombreAsync(string nombre, int? idProveedorExcluir = null) =>
        Conjunto.AnyAsync(p =>
            p.Nombre == nombre &&
            (!idProveedorExcluir.HasValue || p.IdProveedor != idProveedorExcluir.Value));

    /// <inheritdoc />
    public async Task<IReadOnlyList<Proveedor>> ListarConProductosAsync() =>
        await Conjunto
            .AsNoTracking()
            .Include(p => p.Productos)
            .OrderBy(p => p.Nombre)
            .ToListAsync();
}
