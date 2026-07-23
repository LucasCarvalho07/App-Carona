namespace AppCarona.Contracts.Motorista;

public class MotoristaConfigDto
{
    public string ChavePix { get; set; } = string.Empty;
    public string TipoChave { get; set; } = string.Empty;
    public string Titular { get; set; } = string.Empty;
    public string? EmailComprovante { get; set; }
}

public class SalvarMotoristaConfigRequest
{
    public string ChavePix { get; set; } = string.Empty;
    public string TipoChave { get; set; } = string.Empty;
    public string Titular { get; set; } = string.Empty;
    public string? EmailComprovante { get; set; }
}
