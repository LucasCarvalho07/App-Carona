namespace AppCarona.Contracts.Pagamentos;

/// <summary>Resumo do mês por motorista: quem foi com ele, dias e valores.</summary>
public class ResumoMensalMotoristaDto
{
    public int MotoristaId { get; set; }
    public string MotoristaNome { get; set; } = string.Empty;
    public string? Avatar { get; set; }

    /// <summary>Total que os passageiros devem a este motorista no mês.</summary>
    public decimal TotalValor { get; set; }

    /// <summary>Dias distintos em que o motorista dirigiu no mês.</summary>
    public int QtdDiasDirigiu { get; set; }

    /// <summary>Total de viagens (ida/volta contam separado) do motorista no mês.</summary>
    public int QtdViagens { get; set; }

    public List<ResumoMensalPassageiroDto> Passageiros { get; set; } = new();
}

/// <summary>Um passageiro dentro do resumo de um motorista.</summary>
public class ResumoMensalPassageiroDto
{
    public int PassageiroId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Avatar { get; set; }

    /// <summary>Dias distintos em que este passageiro foi com o motorista.</summary>
    public int QtdDias { get; set; }

    /// <summary>Total de viagens (ida/volta separado) deste passageiro com o motorista.</summary>
    public int QtdViagens { get; set; }

    /// <summary>Valor que este passageiro deve ao motorista no mês.</summary>
    public decimal Valor { get; set; }
}
