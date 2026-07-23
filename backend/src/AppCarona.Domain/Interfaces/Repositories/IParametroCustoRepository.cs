using AppCarona.Domain.Entities;

namespace AppCarona.Domain.Interfaces.Repositories;

public interface IParametroCustoRepository : IRepositoryBase<ParametroCusto>
{
    /// <summary>Parâmetro vigente numa data (o mais recente com VigenteDe &lt;= data).</summary>
    Task<ParametroCusto?> ObterVigenteEmAsync(DateTime data);

    /// <summary>Parâmetro com exatamente essa data de vigência (para upsert).</summary>
    Task<ParametroCusto?> ObterPorVigenteDeAsync(DateTime vigenteDe);
}
