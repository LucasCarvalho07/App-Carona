using AppCarona.Domain.Enums;

namespace AppCarona.Domain.Entities;

/// <summary>
/// Pagamento de um passageiro a um motorista, referente a um mês.
/// Só existe registro quando o passageiro informa o pagamento; sem registro = Pendente.
/// </summary>
public class Pagamento
{
    public virtual int Id { get; set; }
    public virtual Usuario Passageiro { get; set; } = null!;
    public virtual Usuario Motorista { get; set; } = null!;
    public virtual int AnoMes { get; set; }
    public virtual decimal Valor { get; set; }
    public virtual StatusPagamento Status { get; set; } = StatusPagamento.Informado;
    public virtual DateTime? InformadoEm { get; set; }
    public virtual DateTime? ConfirmadoEm { get; set; }
    public virtual string? Observacao { get; set; }
}
