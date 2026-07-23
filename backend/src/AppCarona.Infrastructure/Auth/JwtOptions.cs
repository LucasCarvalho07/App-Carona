namespace AppCarona.Infrastructure.Auth;

public class JwtOptions
{
    public const string Secao = "Jwt";

    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiracaoMinutos { get; set; } = 480;
}
