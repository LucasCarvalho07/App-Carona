using AppCarona.Domain.Enums;

namespace AppCarona.Domain.Entities;

public class Veiculo
{
    public virtual int Id { get; set; }
    public virtual Usuario Motorista { get; set; } = null!;
    public virtual string Nome { get; set; } = string.Empty;
    public virtual string? Modelo { get; set; }

    /// <summary>Consumo médio em km por litro.</summary>
    public virtual decimal ConsumoKmLitro { get; set; }

    /// <summary>Distância padrão de uma viagem (km) com este veículo.</summary>
    public virtual decimal KmPorViagem { get; set; }

    public virtual TipoCombustivel Combustivel { get; set; }
    public virtual bool Padrao { get; set; }
    public virtual bool Ativo { get; set; } = true;
}
