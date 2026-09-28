namespace ControlInventario.Domain.Interfaces;

/// <summary>
/// Unidad de trabajo: permite confirmar o deshacer en bloque los cambios
/// realizados por varios repositorios (misma transacción de base de datos).
/// </summary>
public interface IUnidadTrabajo : IDisposable
{
    /// <summary>Inicia una transacción explícita.</summary>
    Task IniciarTransaccionAsync();

    /// <summary>Confirma todos los cambios pendientes y cierra la transacción.</summary>
    Task ConfirmarAsync();

    /// <summary>Deshace los cambios pendientes y cierra la transacción.</summary>
    Task RevertirAsync();

    /// <summary>Guarda los cambios pendientes (sin transacción explícita).</summary>
    Task<int> GuardarCambiosAsync();
}
