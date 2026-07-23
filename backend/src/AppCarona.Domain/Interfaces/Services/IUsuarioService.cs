using AppCarona.Contracts.Usuarios;

namespace AppCarona.Domain.Interfaces.Services;

public interface IUsuarioService
{
    Task<IList<UsuarioDto>> ListarAsync();
    Task<IList<UsuarioDto>> ListarMotoristasAtivosAsync();
    Task<UsuarioDto?> ObterPorIdAsync(int id);

    /// <summary>Define os papéis do usuário e ativa o acesso.</summary>
    Task<UsuarioDto?> AprovarAsync(int id, IEnumerable<string> papeis);

    /// <summary>
    /// Concede ou remove o papel Master de um usuário. Só o master principal
    /// (e-mail em Admin:MasterEmails) pode executar.
    /// </summary>
    Task<UsuarioDto?> DefinirMasterAsync(int id, bool tornarMaster, string emailSolicitante);

    /// <summary>Atualiza o avatar do próprio usuário.</summary>
    Task<UsuarioDto?> AtualizarAvatarAsync(int id, string avatar);
}
