using ControlInventario.Application.Comun;
using ControlInventario.Application.DTOs;
using ControlInventario.Application.Interfaces;
using ControlInventario.Application.Validadores;
using ControlInventario.Domain.Entidades;
using ControlInventario.Domain.Interfaces;
using FluentValidation;

namespace ControlInventario.Application.Servicios;

/// <summary>Servicio CRUD de proveedores con sus datos de contacto.</summary>
public class ServicioProveedores : IServicioProveedores
{
    private readonly IRepositorioProveedores _proveedores;
    private readonly IUnidadTrabajo _unidadTrabajo;
    private readonly IValidator<ProveedorCreacionDTO> _validadorCreacion;
    private readonly IValidator<ProveedorActualizacionDTO> _validadorActualizacion;

    public ServicioProveedores(
        IRepositorioProveedores proveedores,
        IUnidadTrabajo unidadTrabajo,
        IValidator<ProveedorCreacionDTO> validadorCreacion,
        IValidator<ProveedorActualizacionDTO> validadorActualizacion)
    {
        _proveedores = proveedores;
        _unidadTrabajo = unidadTrabajo;
        _validadorCreacion = validadorCreacion;
        _validadorActualizacion = validadorActualizacion;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ProveedorDTO>> ListarAsync(bool soloActivos = true)
    {
        var proveedores = await _proveedores.ListarConProductosAsync();
        var resultado = proveedores.AsEnumerable();
        if (soloActivos)
        {
            resultado = resultado.Where(p => p.Activo);
        }

        return resultado.Select(Mapeador.AProveedorDTO).ToList();
    }

    /// <inheritdoc />
    public async Task<ProveedorDTO> ObtenerPorIdAsync(int id)
    {
        var proveedor = await _proveedores.ObtenerPorIdAsync(id)
            ?? throw new ExcepcionNoEncontrado("Proveedor no encontrado.");

        return Mapeador.AProveedorDTO(proveedor);
    }

    /// <inheritdoc />
    public async Task<ProveedorDTO> CrearAsync(ProveedorCreacionDTO dto)
    {
        await ValidarAsync(_validadorCreacion, dto);

        if (await _proveedores.ExisteNombreAsync(dto.Nombre))
        {
            throw new ExcepcionNegocio($"Ya existe un proveedor con el nombre '{dto.Nombre}'.");
        }

        var proveedor = new Proveedor
        {
            Nombre = dto.Nombre.Trim(),
            Contacto = dto.Contacto?.Trim(),
            Telefono = dto.Telefono?.Trim(),
            Correo = dto.Correo?.Trim(),
            Direccion = dto.Direccion?.Trim(),
            Activo = true
        };

        await _proveedores.AgregarAsync(proveedor);
        await _unidadTrabajo.GuardarCambiosAsync();

        return Mapeador.AProveedorDTO(proveedor);
    }

    /// <inheritdoc />
    public async Task<ProveedorDTO> ActualizarAsync(int id, ProveedorActualizacionDTO dto)
    {
        await ValidarAsync(_validadorActualizacion, dto);

        var proveedor = await _proveedores.ObtenerPorIdAsync(id)
            ?? throw new ExcepcionNoEncontrado("Proveedor no encontrado.");

        if (await _proveedores.ExisteNombreAsync(dto.Nombre, id))
        {
            throw new ExcepcionNegocio($"Ya existe otro proveedor con el nombre '{dto.Nombre}'.");
        }

        proveedor.Nombre = dto.Nombre.Trim();
        proveedor.Contacto = dto.Contacto?.Trim();
        proveedor.Telefono = dto.Telefono?.Trim();
        proveedor.Correo = dto.Correo?.Trim();
        proveedor.Direccion = dto.Direccion?.Trim();
        proveedor.Activo = dto.Activo;

        _proveedores.Actualizar(proveedor);
        await _unidadTrabajo.GuardarCambiosAsync();

        return Mapeador.AProveedorDTO(proveedor);
    }

    /// <inheritdoc />
    public async Task EliminarAsync(int id)
    {
        var proveedor = await _proveedores.ObtenerPorIdAsync(id)
            ?? throw new ExcepcionNoEncontrado("Proveedor no encontrado.");

        // Baja lógica: los productos y movimientos históricos conservan la referencia.
        proveedor.Activo = false;
        _proveedores.Actualizar(proveedor);
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
