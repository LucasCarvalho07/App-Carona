using AppCarona.Domain.Enums;

namespace AppCarona.Domain.Entities;

/// <summary>
/// Viagem física de um dia (um motorista, um veículo). Os campos "snap*" e de valor
/// são fotografados no cálculo para preservar o histórico mesmo que a config mude.
/// </summary>
public class Viagem
{
    public virtual int Id { get; set; }
    public virtual DateTime Data { get; set; }
    public virtual Sentido Sentido { get; set; } = Sentido.Ida;
    public virtual Usuario Motorista { get; set; } = null!;
    public virtual Veiculo? Veiculo { get; set; }
    public virtual StatusViagem Status { get; set; } = StatusViagem.Registrada;
    public virtual string? Observacao { get; set; }
    public virtual OrigemMarcacao Origem { get; set; } = OrigemMarcacao.Manual;
    public virtual int CriadoPor { get; set; }
    public virtual DateTime CriadoEm { get; set; }

    public virtual IList<Participacao> Participacoes { get; set; } = new List<Participacao>();

    // Snapshot dos parâmetros usados no cálculo
    public virtual decimal SnapKmPorViagem { get; set; }
    public virtual decimal SnapConsumoKmLitro { get; set; }
    public virtual decimal SnapPrecoLitro { get; set; }
    public virtual decimal SnapCustoKmManutencao { get; set; }

    // Resultado do cálculo
    public virtual int QtdPessoas { get; set; }
    public virtual decimal CustoCombustivel { get; set; }
    public virtual decimal CustoTotal { get; set; }
    public virtual decimal ValorPorPessoa { get; set; }
}
