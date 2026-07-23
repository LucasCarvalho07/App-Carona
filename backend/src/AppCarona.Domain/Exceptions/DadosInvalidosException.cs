namespace AppCarona.Domain.Exceptions;

/// <summary>Lançada quando dados enviados não passam na validação de negócio.</summary>
public class DadosInvalidosException : Exception
{
    public DadosInvalidosException(string mensagem) : base(mensagem)
    {
    }
}
