namespace AppCarona.Contracts.Marcacao;

public class MarcarPresencaRequest
{
    public DateTime Data { get; set; }
    public int MotoristaId { get; set; }

    /// <summary>"Ida" ou "Volta".</summary>
    public string Sentido { get; set; } = "Ida";
}

/// <summary>Uma marcação do usuário logado (participação em uma viagem).</summary>
public class MinhaMarcacaoDto
{
    public int ViagemId { get; set; }
    public DateTime Data { get; set; }
    public string Sentido { get; set; } = string.Empty;
    public int MotoristaId { get; set; }
    public string MotoristaNome { get; set; } = string.Empty;
    public decimal ValorPorPessoa { get; set; }
    public IList<OcupanteDto> Ocupantes { get; set; } = new List<OcupanteDto>();
}

/// <summary>Passageiro que ocupa o carro nessa viagem.</summary>
public class OcupanteDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Avatar { get; set; }
}
