namespace AppCarona.Contracts.Usuarios;

/// <summary>DTO de retorno do usuário para o frontend.</summary>
public class UsuarioDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? FotoUrl { get; set; }
    public string? Avatar { get; set; }
    public string Status { get; set; } = string.Empty;
    public string[] Papeis { get; set; } = [];
    public DateTime CriadoEm { get; set; }

    /// <summary>True se o e-mail está na lista de masters principais (config). Só o principal promove outros a master.</summary>
    public bool EhMasterPrincipal { get; set; }

    /// <summary>True quando o dono comprovou posse do e-mail.</summary>
    public bool EmailVerificado { get; set; }
}
