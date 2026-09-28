using ControlInventario.Domain.Interfaces;
using ControlInventario.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ControlInventario.Infrastructure.Repositorios;

/// <summary>
/// Unidad de trabajo sobre EF Core: agrupa los cambios de varios repositorios
/// (comparten el mismo DbContext) en una sola transacción.
/// </summary>
public class UnidadTrabajo : IUnidadTrabajo
{
    private readonly ContextoControlInventario _contexto;
    private IDbContextTransaction? _transaccion;

    public UnidadTrabajo(ContextoControlInventario contexto)
    {
        _contexto = contexto;
    }

    /// <inheritdoc />
    public async Task IniciarTransaccionAsync()
    {
        _transaccion ??= await _contexto.Database.BeginTransactionAsync();
    }

    /// <inheritdoc />
    public async Task ConfirmarAsync()
    {
        if (_transaccion is null)
        {
            await _contexto.SaveChangesAsync();
            return;
        }

        await _contexto.SaveChangesAsync();
        await _transaccion.CommitAsync();
        await _transaccion.DisposeAsync();
        _transaccion = null;
    }

    /// <inheritdoc />
    public async Task RevertirAsync()
    {
        if (_transaccion is null)
        {
            return;
        }

        await _transaccion.RollbackAsync();
        await _transaccion.DisposeAsync();
        _transaccion = null;

        // Se olvidan las entidades modificadas para no re-guardarlas por accidente.
        _contexto.ChangeTracker.Clear();
    }

    /// <inheritdoc />
    public Task<int> GuardarCambiosAsync() => _contexto.SaveChangesAsync();

    /// <inheritdoc />
    public void Dispose()
    {
        _transaccion?.Dispose();
        _transaccion = null;
        GC.SuppressFinalize(this);
    }
}
