namespace AppCarona.Domain.Interfaces.Services;

/// <summary>Envio de e-mail transacional (código de recuperação, avisos).</summary>
public interface IEmailSender
{
    Task EnviarAsync(string destino, string assunto, string corpo);
}
