namespace AppCarona.Contracts.Veiculos;

public class VeiculoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Modelo { get; set; }
    public decimal ConsumoKmLitro { get; set; }
    public decimal KmPorViagem { get; set; }
    public string Combustivel { get; set; } = string.Empty;
    public bool Padrao { get; set; }
    public bool Ativo { get; set; }
}

public class SalvarVeiculoRequest
{
    public string Nome { get; set; } = string.Empty;
    public string? Modelo { get; set; }
    public decimal ConsumoKmLitro { get; set; }
    public decimal KmPorViagem { get; set; }
    public string Combustivel { get; set; } = string.Empty;
    public bool Padrao { get; set; }
}
