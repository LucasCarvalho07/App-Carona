using AppCarona.Domain.Entities;

namespace AppCarona.Domain.Interfaces.Repositories;

public interface IUsuarioRepository : IRepositoryBase<Usuario>
{
    Task<Usuario?> ObterPorGoogleSubAsync(string googleSub);
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<IList<Usuario>> ListarMotoristasAtivosAsync();
}
