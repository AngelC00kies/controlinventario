using ControlInventario.Application.DTOs;

namespace ControlInventario.Application.Interfaces;

/// <summary>Servicio de gestión de categorías de productos.</summary>
public interface IServicioCategorias
{
    Task<IReadOnlyList<CategoriaDTO>> ListarAsync(bool soloActivas = true);

    Task<CategoriaDTO> ObtenerPorIdAsync(int id);

    Task<CategoriaDTO> CrearAsync(CategoriaCreacionDTO dto);

    Task<CategoriaDTO> ActualizarAsync(int id, CategoriaActualizacionDTO dto);

    Task EliminarAsync(int id);
}
