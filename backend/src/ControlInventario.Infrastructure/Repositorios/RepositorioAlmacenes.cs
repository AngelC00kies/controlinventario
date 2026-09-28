using ControlInventario.Domain.Entidades;
using ControlInventario.Domain.Interfaces;
using ControlInventario.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace ControlInventario.Infrastructure.Repositorios;

/// <summary>Repositorio de almacenes.</summary>
public class RepositorioAlmacenes : RepositorioBase<Almacen>, IRepositorioAlmacenes
{
    public RepositorioAlmacenes(ContextoControlInventario contexto) : base(contexto)
    {
    }

    /// <inheritdoc />
    public Task<bool> ExisteNombreAsync(string nombre, int? idAlmacenExcluir = null) =>
        Conjunto.AnyAsync(a =>
            a.Nombre == nombre &&
            (!idAlmacenExcluir.HasValue || a.IdAlmacen != idAlmacenExcluir.Value));
}
