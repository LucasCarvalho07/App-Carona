using System.Net.Http.Json;
using AppCarona.Domain.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace AppCarona.Infrastructure.Email;

/// <summary>
/// Envia e-mail pela API transacional do Brevo (HTTPS/443). Alternativa ao SMTP para
/// ambientes que bloqueiam as portas de SMTP (Render, muitos PaaS gratuitos).
/// </summary>
public class BrevoEmailSender : IEmailSender
{
    private readonly HttpClient _http;
    private readonly BrevoOptions _options;

    public BrevoEmailSender(HttpClient http, IOptions<BrevoOptions> options)
    {
        _http = http;
        _options = options.Value;
    }

    public async Task EnviarAsync(string destino, string assunto, string corpo)
    {
        var payload = new
        {
            sender = new { email = _options.Remetente, name = _options.RemetenteNome },
            to = new[] { new { email = destino } },
            subject = assunto,
            textContent = corpo
        };

        using var requisicao = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email")
        {
            Content = JsonContent.Create(payload)
        };
        requisicao.Headers.Add("api-key", _options.ApiKey);

        var resposta = await _http.SendAsync(requisicao);
        resposta.EnsureSuccessStatusCode();
    }
}
