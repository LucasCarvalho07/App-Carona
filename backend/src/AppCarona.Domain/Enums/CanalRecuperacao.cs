namespace AppCarona.Domain.Enums;

/// <summary>
/// Canal usado para enviar o código de recuperação de senha.
/// WhatsApp ainda não está disponível (apenas Email por enquanto).
/// </summary>
public enum CanalRecuperacao
{
    Email = 1,
    WhatsApp = 2
}
