namespace ControlInventario.Application.DTOs;

/// <summary>Alerta de un producto con stock igual o inferior al mínimo.</summary>
public class AlertaStockDTO
{
    public int IdProducto { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string NombreCategoria { get; set; } = string.Empty;

    public string UnidadMedida { get; set; } = string.Empty;

    public int StockMinimo { get; set; }

    public int StockActual { get; set; }

    /// <summary>Unidades que faltan para alcanzar el stock mínimo.</summary>
    public int Diferencia { get; set; }
}
