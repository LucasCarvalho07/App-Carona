using AppCarona.Domain.Enums;

namespace AppCarona.Domain.Entities;

/// <summary>Dados de recebimento do motorista (1:1 com Usuario).</summary>
public class MotoristaConfig
{
    public virtual int Id { get; set; }
    public virtual Usuario Motorista { get; set; } = null!;
    public virtual string ChavePix { get; set; } = string.Empty;
    public virtual TipoChavePix TipoChave { get; set; }
    public virtual string Titular { get; set; } = string.Empty;
    public virtual string? EmailComprovante { get; set; }
}
