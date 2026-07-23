namespace AppCarona.Domain.Exceptions;

/// <summary>Lançada quando se tenta cadastrar um e-mail que já existe.</summary>
public class EmailJaCadastradoException : Exception
{
    public EmailJaCadastradoException(string mensagem) : base(mensagem)
    {
    }
}
