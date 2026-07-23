namespace AppCarona.Contracts.Usuarios;

/// <summary>Concede (true) ou remove (false) o papel Master de um usuário.</summary>
public class DefinirMasterRequest
{
    public bool TornarMaster { get; set; }
}
