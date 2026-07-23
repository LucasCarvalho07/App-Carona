using AppCarona.Contracts.Marcacao;

namespace AppCarona.Contracts.Escala;

public class EscalarRequest
{
    public DateTime Data { get; set; }

    /// <summary>"Ida" ou "Volta".</summary>
    public string Sentido { get; set; } = "Ida";
}

/// <summary>Escala a semana inteira (seg–sex, ida e volta) do motorista logado.</summary>
public class EscalarSemanaRequest
{
    public DateTime DataNaSemana { get; set; }
}

/// <summary>Um carro escalado num trecho (data + sentido).</summary>
public class EscalaCarroDto
{
    public int ViagemId { get; set; }
    public DateTime Data { get; set; }
    public string Sentido { get; set; } = string.Empty;
    public int MotoristaId { get; set; }
    public string MotoristaNome { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public bool Configurado { get; set; }
    public int QtdPassageiros { get; set; }
    public bool SouMotorista { get; set; }
    public bool EstouNesteCarro { get; set; }
    public decimal ValorPorPessoa { get; set; }
    public IList<OcupanteDto> Ocupantes { get; set; } = new List<OcupanteDto>();
}
