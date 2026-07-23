namespace AppCarona.Contracts.Auth;

/// <summary>Retorno da verificação: token de uso único para redefinir a senha.</summary>
public class VerificarCodigoResponse
{
    public string ResetToken { get; set; } = string.Empty;
}
