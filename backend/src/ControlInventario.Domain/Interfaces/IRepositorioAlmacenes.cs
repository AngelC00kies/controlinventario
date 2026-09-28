using ControlInventario.Domain.Entidades;

namespace ControlInventario.Domain.Interfaces;

/// <summary>Contrato del repositorio de almacenes.</summary>
public interface IRepositorioAlmacenes : IRepositorio<Almacen>
{
    /// <summary>Indica si existe un almacén con el nombre indicado.</summary>
    Task<bool> ExisteNombreAsync(string nombre, int? idAlmacenExcluir = null);
}
