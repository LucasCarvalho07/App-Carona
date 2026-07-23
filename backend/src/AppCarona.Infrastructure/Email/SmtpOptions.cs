namespace AppCarona.Infrastructure.Email;

public class SmtpOptions
{
    public const string Secao = "Smtp";

    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string Usuario { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string Remetente { get; set; } = string.Empty;
    public string RemetenteNome { get; set; } = "App Carona";
}
