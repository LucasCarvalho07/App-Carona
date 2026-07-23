using AppCarona.Domain.Interfaces.Auth;
using AppCarona.Domain.Interfaces.Repositories;
using AppCarona.Domain.Interfaces.Services;
using AppCarona.Infrastructure.Auth;
using AppCarona.Infrastructure.Email;
using AppCarona.Infrastructure.Persistence;
using AppCarona.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;

namespace AppCarona.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "ConnectionString 'Postgres' não configurada. Defina via user-secrets (dev) ou " +
                "variável de ambiente 'ConnectionStrings__Postgres' (produção/Docker).");
        }

        var exportarSchema = configuration.GetValue<bool>("NHibernate:ExportarSchema");

        // SessionFactory é caro de construir: singleton para toda a aplicação.
        var sessionFactory = NHibernateHelper.CriarSessionFactory(connectionString, exportarSchema);
        services.AddSingleton(sessionFactory);

        // Uma ISession por requisição.
        services.AddScoped(sp => sp.GetRequiredService<ISessionFactory>().OpenSession());

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IVeiculoRepository, VeiculoRepository>();
        services.AddScoped<IMotoristaConfigRepository, MotoristaConfigRepository>();
        services.AddScoped<IParametroCustoRepository, ParametroCustoRepository>();
        services.AddScoped<IViagemRepository, ViagemRepository>();
        services.AddScoped<IPagamentoRepository, PagamentoRepository>();
        services.AddScoped<ICodigoRecuperacaoRepository, CodigoRecuperacaoRepository>();

        // Autenticação
        services.Configure<GoogleAuthOptions>(configuration.GetSection(GoogleAuthOptions.Secao));
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.Secao));
        services.AddScoped<IGoogleTokenValidator, GoogleTokenValidator>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        // Envio de e-mail (SMTP genérico)
        services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.Secao));
        services.AddScoped<IEmailSender, SmtpEmailSender>();

        return services;
    }
}
