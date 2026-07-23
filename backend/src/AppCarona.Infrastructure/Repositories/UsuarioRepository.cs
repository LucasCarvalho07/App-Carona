using AppCarona.Domain.Entities;
using AppCarona.Domain.Enums;
using AppCarona.Domain.Interfaces.Repositories;
using NHibernate;
using NHibernate.Linq;

namespace AppCarona.Infrastructure.Repositories;

public class UsuarioRepository : RepositoryBase<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(ISession session) : base(session)
    {
    }

    public async Task<Usuario?> ObterPorGoogleSubAsync(string googleSub)
    {
        return await Session.Query<Usuario>()
            .SingleOrDefaultAsync(u => u.GoogleSub == googleSub);
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        return await Session.Query<Usuario>()
            .SingleOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IList<Usuario>> ListarMotoristasAtivosAsync()
    {
        return await Session.Query<Usuario>()
            .Where(u => u.Status == StatusUsuario.Ativo
                && u.Papeis.Any(p => p.Papel == Papel.Motorista))
            .ToListAsync();
    }
}
