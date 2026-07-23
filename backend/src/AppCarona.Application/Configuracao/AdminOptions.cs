namespace AppCarona.Application.Configuracao;

public class AdminOptions
{
    public const string Secao = "Admin";

    /// <summary>E-mails que entram como master (status Ativo + papel Master) automaticamente.</summary>
    public string[] MasterEmails { get; set; } = [];
}
