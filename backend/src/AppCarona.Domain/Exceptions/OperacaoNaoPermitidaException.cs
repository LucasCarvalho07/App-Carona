namespace AppCarona.Domain.Exceptions;

/// <summary>Lançada quando o usuário não tem permissão para a operação (ex.: só o master principal pode).</summary>
public class OperacaoNaoPermitidaException : Exception
{
    public OperacaoNaoPermitidaException(string mensagem) : base(mensagem)
    {
    }
}
