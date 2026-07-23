namespace AppCarona.Infrastructure.Auth;

public class GoogleAuthOptions
{
    public const string Secao = "Authentication:Google";

    /// <summary>Client ID do OAuth do Google. O id_token precisa ter esta audience.</summary>
    public string ClientId { get; set; } = string.Empty;
}
