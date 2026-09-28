using ControlInventario.Application.DTOs;
using ControlInventario.Domain.Entidades;
using ControlInventario.Domain.Enums;

namespace ControlInventario.Tests.Config;

/// <summary>
/// Fábrica de objetos de prueba: entidades de ejemplo para las pruebas unitarias.
/// </summary>
public static class DatosPrueba
{
    /// <summary>Producto de ejemplo con stock por almacén.</summary>
    public static Producto Producto(int idProducto = 1, string codigo = "ELEC-001", int stockMinimo = 10, params int[] cantidades) =>
        new()
        {
            IdProducto = idProducto,
            Codigo = codigo,
            Nombre = "Producto de prueba",
            Descripcion = "Descripción de prueba",
            IdCategoria = 1,
            Categoria = new Categoria { IdCategoria = 1, Nombre = "Electrónica" },
            IdProveedor = 1,
            Proveedor = new Proveedor { IdProveedor = 1, Nombre = "Proveedor de prueba" },
            PrecioCompra = 1000,
            PrecioVenta = 1500,
            UnidadMedida = "Unidad",
            StockMinimo = stockMinimo,
            Activo = true,
            StockPorAlmacen = cantidades
                .Select((cantidad, indice) => new StockPorAlmacen
                {
                    IdStock = indice + 1,
                    IdProducto = idProducto,
                    IdAlmacen = indice + 1,
                    Cantidad = cantidad
                })
                .ToList()
        };

    /// <summary>Almacén activo de ejemplo.</summary>
    public static Almacen Almacen(int idAlmacen = 1, bool activo = true) =>
        new()
        {
            IdAlmacen = idAlmacen,
            Nombre = "Almacén Central",
            Ubicacion = "Bodega principal",
            Activo = activo
        };

    /// <summary>Usuario activo con rol de ejemplo.</summary>
    public static Usuario Usuario(int idUsuario = 1, bool activo = true, string rol = "Administrador") =>
        new()
        {
            IdUsuario = idUsuario,
            NombreUsuario = "admin",
            NombreCompleto = "Administrador General",
            Correo = "admin@controlinventario.local",
            ContrasenaHash = "hash-de-prueba",
            IdRol = 1,
            Rol = new Rol { IdRol = 1, Nombre = rol },
            Activo = activo
        };

    /// <summary>Solicitud de movimiento válida de ejemplo.</summary>
    public static MovimientoCreacionDTO Movimiento(
        TipoMovimiento tipo = TipoMovimiento.Entrada,
        int idAlmacen = 1,
        int cantidad = 5,
        int idProducto = 1,
        decimal precioUnitario = 0) =>
        new()
        {
            Tipo = tipo,
            Motivo = "Compra de mercadería",
            DocumentoReferencia = "FAC-001",
            IdAlmacen = idAlmacen,
            Detalles = new List<MovimientoDetalleCreacionDTO>
            {
                new() { IdProducto = idProducto, Cantidad = cantidad, PrecioUnitario = precioUnitario }
            }
        };
}
