using ControlInventario.Application.Comun;
using ControlInventario.Application.DTOs;
using ControlInventario.Application.Interfaces;
using ControlInventario.Domain.Interfaces;

namespace ControlInventario.Application.Servicios;

/// <summary>
/// Servicio de alertas: detecta productos cuyo stock total es igual o
/// inferior al stock mínimo configurado.
/// </summary>
public class ServicioAlertas : IServicioAlertas
{
    private readonly IRepositorioProductos _productos;

    public ServicioAlertas(IRepositorioProductos productos)
    {
        _productos = productos;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AlertaStockDTO>> ObtenerStockBajoAsync()
    {
        var criticos = await _productos.ObtenerStockCriticoAsync();
        return criticos
            .Select(Mapeador.AAlertaStockDTO)
            .OrderByDescending(a => a.Diferencia)
            .ToList();
    }
}
