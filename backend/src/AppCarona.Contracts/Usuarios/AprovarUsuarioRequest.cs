namespace AppCarona.Contracts.Usuarios;

/// <summary>Aprovação de um usuário: define os papéis e ativa o acesso.</summary>
public class AprovarUsuarioRequest
{
    /// <summary>Papéis a atribuir (ex.: "Motorista", "Passageiro").</summary>
    public string[] Papeis { get; set; } = [];
}
