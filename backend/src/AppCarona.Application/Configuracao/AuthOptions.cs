namespace AppCarona.Application.Configuracao;

public class AuthOptions
{
    public const string Secao = "Auth";

    /// <summary>Domínios de e-mail autorizados a acessar o sistema (ex.: abase.com).</summary>
    public string[] DominiosPermitidos { get; set; } = [];
}
