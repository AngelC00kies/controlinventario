using ControlInventario.Domain.Entidades;
using ControlInventario.Domain.Interfaces;
using ControlInventario.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace ControlInventario.Infrastructure.Repositorios;

/// <summary>Repositorio de categorías.</summary>
public class RepositorioCategorias : RepositorioBase<Categoria>, IRepositorioCategorias
{
    public RepositorioCategorias(ContextoControlInventario contexto) : base(contexto)
    {
    }

    /// <inheritdoc />
    public Task<bool> ExisteNombreAsync(string nombre, int? idCategoriaExcluir = null) =>
        Conjunto.AnyAsync(c =>
            c.Nombre == nombre &&
            (!idCategoriaExcluir.HasValue || c.IdCategoria != idCategoriaExcluir.Value));

    /// <inheritdoc />
    public async Task<IReadOnlyList<Categoria>> ListarConProductosAsync() =>
        await Conjunto
            .AsNoTracking()
            .Include(c => c.Productos)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
}
