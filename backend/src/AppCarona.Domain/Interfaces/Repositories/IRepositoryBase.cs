namespace AppCarona.Domain.Interfaces.Repositories;

/// <summary>
/// Contrato genérico de acesso a dados. Cada entidade reaproveita estas operações
/// e a interface específica adiciona apenas consultas próprias.
/// </summary>
public interface IRepositoryBase<T> where T : class
{
    Task<T?> ObterPorIdAsync(int id);
    Task<IList<T>> ListarAsync();
    Task SalvarAsync(T entidade);
    Task AtualizarAsync(T entidade);
    Task RemoverAsync(T entidade);
}
