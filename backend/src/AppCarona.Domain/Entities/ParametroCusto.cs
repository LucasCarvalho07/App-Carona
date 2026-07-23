namespace AppCarona.Domain.Entities;

/// <summary>
/// Parâmetros de custo com vigência por data (globais, editados pelo master).
/// O cálculo de uma viagem usa o parâmetro vigente na data da viagem.
/// </summary>
public class ParametroCusto
{
    public virtual int Id { get; set; }

    /// <summary>Data a partir da qual estes valores passam a valer.</summary>
    public virtual DateTime VigenteDe { get; set; }

    public virtual decimal PrecoLitro { get; set; }

    /// <summary>Custo de desgaste por km (ex.: 0,16).</summary>
    public virtual decimal CustoKmManutencao { get; set; }
}
