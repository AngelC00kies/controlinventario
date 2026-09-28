using ControlInventario.Domain.Entidades;

namespace ControlInventario.Domain.Interfaces;

/// <summary>Contrato del repositorio de proveedores.</summary>
public interface IRepositorioProveedores : IRepositorio<Proveedor>
{
    /// <summary>Indica si existe un proveedor con el nombre indicado.</summary>
    Task<bool> ExisteNombreAsync(string nombre, int? idProveedorExcluir = null);

    /// <summary>Lista los proveedores con el total de productos asociados.</summary>
    Task<IReadOnlyList<Proveedor>> ListarConProductosAsync();
}
