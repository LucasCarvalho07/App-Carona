using AppCarona.Domain.Entities;
using AppCarona.Domain.Interfaces.Repositories;
using NHibernate;
using NHibernate.Linq;

namespace AppCarona.Infrastructure.Repositories;

public class CodigoRecuperacaoRepository : RepositoryBase<CodigoRecuperacao>, ICodigoRecuperacaoRepository
{
    public CodigoRecuperacaoRepository(ISession session) : base(session)
    {
    }

    public async Task<CodigoRecuperacao?> ObterAtivoPorUsuarioAsync(int usuarioId)
    {
        var agora = DateTime.UtcNow;
        return await Session.Query<CodigoRecuperacao>()
            .Where(c => c.UsuarioId == usuarioId && !c.Usado && c.ExpiraEm > agora)
            .OrderByDescending(c => c.CriadoEm)
            .FirstOrDefaultAsync();
    }
}
