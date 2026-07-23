using AppCarona.Application.Services;
using AppCarona.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AppCarona.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICalculoService, CalculoService>();
        services.AddScoped<IVeiculoService, VeiculoService>();
        services.AddScoped<IMotoristaConfigService, MotoristaConfigService>();
        services.AddScoped<IParametroCustoService, ParametroCustoService>();
        services.AddScoped<IMarcacaoService, MarcacaoService>();
        services.AddScoped<IEscalaService, EscalaService>();
        services.AddScoped<IPagamentoService, PagamentoService>();
        services.AddScoped<IMotoristaService, MotoristaService>();

        return services;
    }
}
