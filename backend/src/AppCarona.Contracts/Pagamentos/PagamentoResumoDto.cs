namespace AppCarona.Contracts.Pagamentos;

/// <summary>Quanto o passageiro deve a um motorista em um mês (visão do passageiro).</summary>
public class PagamentoResumoDto
{
    public int? PagamentoId { get; set; }
    public int MotoristaId { get; set; }
    public string MotoristaNome { get; set; } = string.Empty;
    public int AnoMes { get; set; }
    public int QtdDias { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = "Pendente";
    public string? ChavePix { get; set; }
    public string? TipoChave { get; set; }
}

/// <summary>Passageiro informa que pagou um motorista no mês.</summary>
public class InformarPagamentoRequest
{
    public int MotoristaId { get; set; }
    public int AnoMes { get; set; }
}

/// <summary>Visão do motorista: um passageiro que deve/pagou no mês.</summary>
public class RecebimentoDto
{
    public int? PagamentoId { get; set; }
    public int PassageiroId { get; set; }
    public string PassageiroNome { get; set; } = string.Empty;
    public int AnoMes { get; set; }
    public decimal Valor { get; set; }
    public string Status { get; set; } = "Pendente";
}
