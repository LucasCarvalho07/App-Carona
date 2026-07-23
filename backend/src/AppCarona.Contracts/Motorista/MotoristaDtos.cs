namespace AppCarona.Contracts.Motorista;

/// <summary>Motorista disponível para seleção na marcação.</summary>
public class MotoristaOpcaoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Avatar { get; set; }

    /// <summary>Tem veículo padrão com consumo e km informados. Só configurado pode ser selecionado.</summary>
    public bool Configurado { get; set; }
}

/// <summary>Detalhes de custo que o motorista repassa ao passageiro (para o modal).</summary>
public class DetalheMotoristaDto
{
    public int MotoristaId { get; set; }
    public string MotoristaNome { get; set; } = string.Empty;
    public int AnoMes { get; set; }
    public bool Configurado { get; set; }
    public bool TemConfigMes { get; set; }

    public string? VeiculoNome { get; set; }
    public decimal ConsumoKmLitro { get; set; }
    public decimal KmPorViagem { get; set; }
    public decimal PrecoLitro { get; set; }
    public decimal CustoKmManutencao { get; set; }

    public decimal CustoCombustivel { get; set; }
    public decimal CustoTotal { get; set; }
}
