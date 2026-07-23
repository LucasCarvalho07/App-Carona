using AppCarona.Domain.Interfaces.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace AppCarona.Infrastructure.Email;

/// <summary>
/// Envio via SMTP genérico (funciona com Brevo, Gmail app password, etc).
/// Em dev, se o Host não estiver configurado, o e-mail é escrito no console
/// (permite testar o fluxo de recuperação sem servidor SMTP).
/// </summary>
public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpOptions _options;

    public SmtpEmailSender(IOptions<SmtpOptions> options)
    {
        _options = options.Value;
    }

    public async Task EnviarAsync(string destino, string assunto, string corpo)
    {
        if (string.IsNullOrWhiteSpace(_options.Host))
        {
            var ambiente = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var ehDesenvolvimento = string.Equals(ambiente, "Development", StringComparison.OrdinalIgnoreCase);
            if (ehDesenvolvimento)
            {
                // Fallback de desenvolvimento: sem SMTP configurado, loga no console.
                Console.WriteLine($"[EMAIL DEV] Para: {destino} | Assunto: {assunto}\n{corpo}");
                return;
            }

            // Fora de dev nunca loga o conteúdo (evita vazar código de recuperação nos logs).
            throw new InvalidOperationException("SMTP não configurado (Smtp:Host vazio).");
        }

        var mensagem = new MimeMessage();
        mensagem.From.Add(new MailboxAddress(_options.RemetenteNome, _options.Remetente));
        mensagem.To.Add(MailboxAddress.Parse(destino));
        mensagem.Subject = assunto;
        mensagem.Body = new TextPart("plain") { Text = corpo };

        using var cliente = new SmtpClient();
        await cliente.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.StartTlsWhenAvailable);

        if (!string.IsNullOrWhiteSpace(_options.Usuario))
        {
            await cliente.AuthenticateAsync(_options.Usuario, _options.Senha);
        }

        await cliente.SendAsync(mensagem);
        await cliente.DisconnectAsync(true);
    }
}
