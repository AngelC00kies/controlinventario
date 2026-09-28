using ControlInventario.Application.Interfaces;
using ControlInventario.Application.Servicios;
using ControlInventario.Application.Validadores;
using ControlInventario.Domain.Entidades;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ControlInventario.Application;

/// <summary>
/// Registro de dependencias de la capa Application (inversión de dependencias:
/// los controladores consumen interfaces, nunca implementaciones concretas).
/// </summary>
public static class DependenciasApplication
{
    public static IServiceCollection AgregarAplicacion(this IServiceCollection servicios)
    {
        // Validadores FluentValidation (todos los de este ensamblado).
        servicios.AddValidatorsFromAssemblyContaining<SolicitudLoginValidador>();

        // Servicios de negocio.
        servicios.AddScoped<IServicioAuth, ServicioAuth>();
        servicios.AddScoped<IServicioProductos, ServicioProductos>();
        servicios.AddScoped<IServicioCategorias, ServicioCategorias>();
        servicios.AddScoped<IServicioProveedores, ServicioProveedores>();
        servicios.AddScoped<IServicioAlmacenes, ServicioAlmacenes>();
        servicios.AddScoped<IServicioUsuarios, ServicioUsuarios>();
        servicios.AddScoped<IServicioMovimientos, ServicioMovimientos>();
        servicios.AddScoped<IServicioAlertas, ServicioAlertas>();
        servicios.AddScoped<IServicioReportes, ServicioReportes>();
        servicios.AddScoped<IServicioDashboard, ServicioDashboard>();

        // Hash de contraseñas de ASP.NET Core Identity (PBKDF2).
        servicios.AddSingleton<IPasswordHasher<Usuario>>(new PasswordHasher<Usuario>());

        return servicios;
    }
}
