namespace AppCarona.Domain.Exceptions;

/// <summary>Lançada quando o e-mail do login não pertence a um domínio autorizado.</summary>
public class DominioNaoPermitidoException : Exception
{
    public DominioNaoPermitidoException(string mensagem) : base(mensagem)
    {
    }
}
