using ControlInventario.Application.Comun;
using ControlInventario.Application.DTOs;
using ControlInventario.Application.Interfaces;
using ControlInventario.Application.Validadores;
using ControlInventario.Domain.Entidades;
using ControlInventario.Domain.Interfaces;
using FluentValidation;

namespace ControlInventario.Application.Servicios;

/// <summary>Servicio CRUD de almacenes (uno o varios por empresa).</summary>
public class ServicioAlmacenes : IServicioAlmacenes
{
    private readonly IRepositorioAlmacenes _almacenes;
    private readonly IUnidadTrabajo _unidadTrabajo;
    private readonly IValidator<AlmacenCreacionDTO> _validadorCreacion;
    private readonly IValidator<AlmacenActualizacionDTO> _validadorActualizacion;

    public ServicioAlmacenes(
        IRepositorioAlmacenes almacenes,
        IUnidadTrabajo unidadTrabajo,
        IValidator<AlmacenCreacionDTO> validadorCreacion,
        IValidator<AlmacenActualizacionDTO> validadorActualizacion)
    {
        _almacenes = almacenes;
        _unidadTrabajo = unidadTrabajo;
        _validadorCreacion = validadorCreacion;
        _validadorActualizacion = validadorActualizacion;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AlmacenDTO>> ListarAsync(bool soloActivos = true)
    {
        var almacenes = await _almacenes.ListarAsync();
        var resultado = almacenes.AsEnumerable();
        if (soloActivos)
        {
            resultado = resultado.Where(a => a.Activo);
        }

        return resultado.Select(Mapeador.AAlmacenDTO).ToList();
    }

    /// <inheritdoc />
    public async Task<AlmacenDTO> ObtenerPorIdAsync(int id)
    {
        var almacen = await _almacenes.ObtenerPorIdAsync(id)
            ?? throw new ExcepcionNoEncontrado("Almacén no encontrado.");

        return Mapeador.AAlmacenDTO(almacen);
    }

    /// <inheritdoc />
    public async Task<AlmacenDTO> CrearAsync(AlmacenCreacionDTO dto)
    {
        await ValidarAsync(_validadorCreacion, dto);

        if (await _almacenes.ExisteNombreAsync(dto.Nombre))
        {
            throw new ExcepcionNegocio($"Ya existe un almacén con el nombre '{dto.Nombre}'.");
        }

        var almacen = new Almacen
        {
            Nombre = dto.Nombre.Trim(),
            Ubicacion = dto.Ubicacion?.Trim(),
            Descripcion = dto.Descripcion?.Trim(),
            Activo = true
        };

        await _almacenes.AgregarAsync(almacen);
        await _unidadTrabajo.GuardarCambiosAsync();

        return Mapeador.AAlmacenDTO(almacen);
    }

    /// <inheritdoc />
    public async Task<AlmacenDTO> ActualizarAsync(int id, AlmacenActualizacionDTO dto)
    {
        await ValidarAsync(_validadorActualizacion, dto);

        var almacen = await _almacenes.ObtenerPorIdAsync(id)
            ?? throw new ExcepcionNoEncontrado("Almacén no encontrado.");

        if (await _almacenes.ExisteNombreAsync(dto.Nombre, id))
        {
            throw new ExcepcionNegocio($"Ya existe otro almacén con el nombre '{dto.Nombre}'.");
        }

        almacen.Nombre = dto.Nombre.Trim();
        almacen.Ubicacion = dto.Ubicacion?.Trim();
        almacen.Descripcion = dto.Descripcion?.Trim();
        almacen.Activo = dto.Activo;

        _almacenes.Actualizar(almacen);
        await _unidadTrabajo.GuardarCambiosAsync();

        return Mapeador.AAlmacenDTO(almacen);
    }

    /// <inheritdoc />
    public async Task EliminarAsync(int id)
    {
        var almacen = await _almacenes.ObtenerPorIdAsync(id)
            ?? throw new ExcepcionNoEncontrado("Almacén no encontrado.");

        almacen.Activo = false;
        _almacenes.Actualizar(almacen);
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
