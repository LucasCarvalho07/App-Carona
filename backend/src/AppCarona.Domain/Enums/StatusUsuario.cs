namespace AppCarona.Domain.Enums;

/// <summary>
/// Situação do usuário no sistema. Novo login com Google entra como AguardandoAprovacao
/// até o master definir o perfil.
/// </summary>
public enum StatusUsuario
{
    AguardandoAprovacao = 1,
    Ativo = 2,
    Inativo = 3
}
