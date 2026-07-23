namespace AppCarona.Contracts.Custo;

public class ParametroCustoDto
{
    public int Id { get; set; }
    public DateTime VigenteDe { get; set; }
    public decimal PrecoLitro { get; set; }
    public decimal CustoKmManutencao { get; set; }
}

public class SalvarParametroCustoRequest
{
    public DateTime VigenteDe { get; set; }
    public decimal PrecoLitro { get; set; }
    public decimal CustoKmManutencao { get; set; }
}
