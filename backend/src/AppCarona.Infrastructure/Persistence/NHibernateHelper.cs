using AppCarona.Infrastructure.Persistence.Mappings;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using Environment = NHibernate.Cfg.Environment;

namespace AppCarona.Infrastructure.Persistence;

/// <summary>
/// Constrói a ISessionFactory do NHibernate (singleton). Registra os mapeamentos
/// FluentNHibernate e configura o PostgreSQL.
/// </summary>
public static class NHibernateHelper
{
    public static ISessionFactory CriarSessionFactory(string connectionString, bool exportarSchema = false)
    {
        return Fluently.Configure()
            .Database(PostgreSQLConfiguration.Standard.ConnectionString(connectionString))
            .Mappings(m => m.FluentMappings.AddFromAssemblyOf<UsuarioMap>())
            .ExposeConfiguration(cfg =>
            {
                // Não conectar ao banco no boot só para importar palavras reservadas.
                cfg.SetProperty(Environment.Hbm2ddlKeyWords, "none");

                if (exportarSchema)
                {
                    // Somente em desenvolvimento: cria/atualiza o schema no banco.
                    new SchemaUpdate(cfg).Execute(useStdOut: false, doUpdate: true);
                }
            })
            .BuildSessionFactory();
    }
}
