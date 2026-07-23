using AppCarona.Domain.Enums;

namespace AppCarona.Domain.Entities;

/// <summary>Passageiro dentro de uma viagem. Coleção pertencente à Viagem.</summary>
public class Participacao
{
    public virtual int Id { get; set; }
    public virtual Usuario Passageiro { get; set; } = null!;
    public virtual OrigemMarcacao Origem { get; set; } = OrigemMarcacao.Manual;

    /// <summary>Valor devido por este passageiro (rateio da viagem). Calculado.</summary>
    public virtual decimal ValorDevido { get; set; }
}
