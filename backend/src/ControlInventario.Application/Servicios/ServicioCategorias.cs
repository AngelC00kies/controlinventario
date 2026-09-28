using ControlInventario.Application.Comun;
using ControlInventario.Application.DTOs;
using ControlInventario.Application.Interfaces;
using ControlInventario.Application.Validadores;
using ControlInventario.Domain.Entidades;
using ControlInventario.Domain.Interfaces;
using FluentValidation;

namespace ControlInventario.Application.Servicios;

/// <summary>Servicio CRUD de categorías de productos.</summary>
public class ServicioCategorias : IServicioCategorias
{
    private readonly IRepositorioCategorias _categorias;
    private readonly IUnidadTrabajo _unidadTrabajo;
    private readonly IValidator<CategoriaCreacionDTO> _validadorCreacion;
    private readonly IValidator<CategoriaActualizacionDTO> _validadorActualizacion;

    public ServicioCategorias(
        IRepositorioCategorias categorias,
        IUnidadTrabajo unidadTrabajo,
        IValidator<CategoriaCreacionDTO> validadorCreacion,
        IValidator<CategoriaActualizacionDTO> validadorActualizacion)
    {
        _categorias = categorias;
        _unidadTrabajo = unidadTrabajo;
        _validadorCreacion = validadorCreacion;
        _validadorActualizacion = validadorActualizacion;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CategoriaDTO>> ListarAsync(bool soloActivas = true)
    {
        var categorias = await _categorias.ListarConProductosAsync();
        var resultado = categorias.AsEnumerable();
        if (soloActivas)
        {
            resultado = resultado.Where(c => c.Activo);
        }

        return resultado.Select(Mapeador.ACategoriaDTO).ToList();
    }

    /// <inheritdoc />
    public async Task<CategoriaDTO> ObtenerPorIdAsync(int id)
    {
        var categoria = await _categorias.ObtenerPorIdAsync(id)
            ?? throw new ExcepcionNoEncontrado("Categoría no encontrada.");

        return Mapeador.ACategoriaDTO(categoria);
    }

    /// <inheritdoc />
    public async Task<CategoriaDTO> CrearAsync(CategoriaCreacionDTO dto)
    {
        await ValidarAsync(_validadorCreacion, dto);

        if (await _categorias.ExisteNombreAsync(dto.Nombre))
        {
            throw new ExcepcionNegocio($"Ya existe una categoría con el nombre '{dto.Nombre}'.");
        }

        var categoria = new Categoria
        {
            Nombre = dto.Nombre.Trim(),
            Descripcion = dto.Descripcion?.Trim(),
            Activo = true
        };

        await _categorias.AgregarAsync(categoria);
        await _unidadTrabajo.GuardarCambiosAsync();

        return Mapeador.ACategoriaDTO(categoria);
    }

    /// <inheritdoc />
    public async Task<CategoriaDTO> ActualizarAsync(int id, CategoriaActualizacionDTO dto)
    {
        await ValidarAsync(_validadorActualizacion, dto);

        var categoria = await _categorias.ObtenerPorIdAsync(id)
            ?? throw new ExcepcionNoEncontrado("Categoría no encontrada.");

        if (await _categorias.ExisteNombreAsync(dto.Nombre, id))
        {
            throw new ExcepcionNegocio($"Ya existe otra categoría con el nombre '{dto.Nombre}'.");
        }

        categoria.Nombre = dto.Nombre.Trim();
        categoria.Descripcion = dto.Descripcion?.Trim();
        categoria.Activo = dto.Activo;

        _categorias.Actualizar(categoria);
        await _unidadTrabajo.GuardarCambiosAsync();

        return Mapeador.ACategoriaDTO(categoria);
    }

    /// <inheritdoc />
    public async Task EliminarAsync(int id)
    {
        var categoria = await _categorias.ObtenerPorIdAsync(id)
            ?? throw new ExcepcionNoEncontrado("Categoría no encontrada.");

        // Baja lógica para no perder los productos asociados.
        categoria.Activo = false;
        _categorias.Actualizar(categoria);
        await _unidadTrabajo.GuardarCambiosAsync();
    }

    private static async Task ValidarAsync<T>(IValidator<T> validador, T dto)
    {
        var validacion = await validador.ValidateAsync(dto);
        if (!validacion.IsValid)
        {
            throw new ExcepcionNegocio(string.Join(" ", validacion.Errors.Select(e => e.ErrorMessage)));
        }
    }
}
