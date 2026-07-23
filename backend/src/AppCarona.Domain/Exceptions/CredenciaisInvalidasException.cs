namespace AppCarona.Domain.Exceptions;

/// <summary>Lançada quando e-mail/senha do login local não conferem.</summary>
public class CredenciaisInvalidasException : Exception
{
    public CredenciaisInvalidasException(string mensagem) : base(mensagem)
    {
    }
}
