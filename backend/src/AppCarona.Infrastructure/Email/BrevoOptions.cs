namespace AppCarona.Infrastructure.Email;

/// <summary>
/// Envio via API HTTP do Brevo (porta 443). Usado em produção onde SMTP (587) é bloqueado
/// (ex.: Render). Se ApiKey estiver preenchida, a aplicação usa Brevo; senão, cai no SMTP.
/// </summary>
public class BrevoOptions
{
    public const string Secao = "Brevo";

    public string ApiKey { get; set; } = string.Empty;
    public string Remetente { get; set; } = string.Empty;
    public string RemetenteNome { get; set; } = "App Carona";
}
