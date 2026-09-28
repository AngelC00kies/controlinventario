using ControlInventario.Domain.Entidades;

namespace ControlInventario.Domain.Interfaces;

/// <summary>Contrato del repositorio de categorías.</summary>
public interface IRepositorioCategorias : IRepositorio<Categoria>
{
    /// <summary>Indica si existe una categoría con el nombre indicado.</summary>
    Task<bool> ExisteNombreAsync(string nombre, int? idCategoriaExcluir = null);

    /// <summary>Lista las categorías con el total de productos asociados.</summary>
    Task<IReadOnlyList<Categoria>> ListarConProductosAsync();
}
