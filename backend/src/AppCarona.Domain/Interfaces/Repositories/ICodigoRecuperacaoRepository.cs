using AppCarona.Domain.Entities;

namespace AppCarona.Domain.Interfaces.Repositories;

public interface ICodigoRecuperacaoRepository : IRepositoryBase<CodigoRecuperacao>
{
    /// <summary>Código ativo do usuário (não usado e não expirado), o mais recente.</summary>
    Task<CodigoRecuperacao?> ObterAtivoPorUsuarioAsync(int usuarioId);
}
