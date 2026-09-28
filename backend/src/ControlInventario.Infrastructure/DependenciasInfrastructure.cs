using ControlInventario.Domain.Interfaces;
using ControlInventario.Infrastructure.Persistencia;
using ControlInventario.Infrastructure.Repositorios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ControlInventario.Infrastructure;

/// <summary>
/// Registro de dependencias de la capa Infrastructure: aquí se concreta cada
/// abstracción definida en Domain (inversión de dependencias).
/// </summary>
public static class DependenciasInfrastructure
{
    public static IServiceCollection AgregarInfraestructura(this IServiceCollection servicios, string cadenaConexion)
    {
        servicios.AddDbContext<ContextoControlInventario>(opciones =>
            opciones.UseSqlServer(cadenaConexion));

        // Repositorios concretos (interfaz de Domain -> implementación de Infrastructure).
        servicios.AddScoped<IRepositorioProductos, RepositorioProductos>();
        servicios.AddScoped<IRepositorioCategorias, RepositorioCategorias>();
        servicios.AddScoped<IRepositorioProveedores, RepositorioProveedores>();
        servicios.AddScoped<IRepositorioAlmacenes, RepositorioAlmacenes>();
        servicios.AddScoped<IRepositorioUsuarios, RepositorioUsuarios>();
        servicios.AddScoped<IRepositorioRoles, RepositorioRoles>();
        servicios.AddScoped<IRepositorioMovimientos, RepositorioMovimientos>();
        servicios.AddScoped<IRepositorioStockPorAlmacen, RepositorioStockPorAlmacen>();
        servicios.AddScoped<IUnidadTrabajo, UnidadTrabajo>();

        return servicios;
    }
}
