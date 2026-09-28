using ControlInventario.Domain.Interfaces;
using ControlInventario.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace ControlInventario.Infrastructure.Repositorios;

/// <summary>
/// Implementación base de los repositorios sobre EF Core. Cada repositorio
/// concreto agrega sus consultas específicas (LSP: todos cumplen el contrato
/// de <see cref="IRepositorio{TEntidad}"/>).
/// </summary>
/// <typeparam name="TEntidad">Entidad de dominio.</typeparam>
public abstract class RepositorioBase<TEntidad> : IRepositorio<TEntidad> where TEntidad : class
{
    protected readonly ContextoControlInventario Contexto;
    protected readonly DbSet<TEntidad> Conjunto;

    protected RepositorioBase(ContextoControlInventario contexto)
    {
        Contexto = contexto;
        Conjunto = contexto.Set<TEntidad>();
    }

    /// <inheritdoc />
    public virtual async Task<TEntidad?> ObtenerPorIdAsync(int id) =>
        await Conjunto.FindAsync(id);

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<TEntidad>> ListarAsync() =>
        await Conjunto.AsNoTracking().ToListAsync();

    /// <inheritdoc />
    public virtual async Task AgregarAsync(TEntidad entidad) =>
        await Conjunto.AddAsync(entidad);

    /// <inheritdoc />
    public virtual void Actualizar(TEntidad entidad) =>
        Conjunto.Update(entidad);

    /// <inheritdoc />
    public virtual void Eliminar(TEntidad entidad) =>
        Conjunto.Remove(entidad);
}
