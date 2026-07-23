using AppCarona.Contracts.Veiculos;
using AppCarona.Domain.Entities;
using AppCarona.Domain.Enums;
using AppCarona.Domain.Exceptions;
using AppCarona.Domain.Interfaces.Repositories;
using AppCarona.Domain.Interfaces.Services;

namespace AppCarona.Application.Services;

public class VeiculoService : IVeiculoService
{
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IViagemRepository _viagemRepository;
    private readonly ICalculoService _calculoService;

    public VeiculoService(
        IVeiculoRepository veiculoRepository,
        IUsuarioRepository usuarioRepository,
        IViagemRepository viagemRepository,
        ICalculoService calculoService)
    {
        _veiculoRepository = veiculoRepository;
        _usuarioRepository = usuarioRepository;
        _viagemRepository = viagemRepository;
        _calculoService = calculoService;
    }

    public async Task<IList<VeiculoDto>> ListarDoMotoristaAsync(int motoristaId)
    {
        var veiculos = await _veiculoRepository.ListarPorMotoristaAsync(motoristaId);
        return veiculos.Select(Mapear).ToList();
    }

    public async Task<VeiculoDto> CriarAsync(int motoristaId, SalvarVeiculoRequest request)
    {
        Validar(request);

        var motorista = await _usuarioRepository.ObterPorIdAsync(motoristaId)
            ?? throw new InvalidOperationException("Motorista não encontrado.");

        var veiculo = new Veiculo { Motorista = motorista, Ativo = true };
        AplicarDados(veiculo, request);

        await _veiculoRepository.SalvarAsync(veiculo);
        return Mapear(veiculo);
    }

    public async Task<VeiculoDto?> AtualizarAsync(int motoristaId, int id, SalvarVeiculoRequest request)
    {
        Validar(request);

        var veiculo = await _veiculoRepository.ObterPorIdAsync(id);
        if (veiculo is null || veiculo.Motorista.Id != motoristaId)
        {
            return null;
        }

        var kmAntigo = veiculo.KmPorViagem;
        var consumoAntigo = veiculo.ConsumoKmLitro;

        AplicarDados(veiculo, request);
        await _veiculoRepository.AtualizarAsync(veiculo);

        var mudouCalculo = veiculo.KmPorViagem != kmAntigo || veiculo.ConsumoKmLitro != consumoAntigo;
        if (mudouCalculo)
        {
            await RecalcularMesAtualEmDianteAsync(motoristaId);
        }

        return Mapear(veiculo);
    }

    /// <summary>Recalcula as viagens do motorista do 1º dia do mês atual em diante (preserva o passado).</summary>
    private async Task RecalcularMesAtualEmDianteAsync(int motoristaId)
    {
        var hoje = DateTime.Today;
        var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);
        var viagens = await _viagemRepository.ListarPorMotoristaPeriodoAsync(motoristaId, inicioMes, DateTime.MaxValue);

        foreach (var viagem in viagens)
        {
            await _calculoService.RecalcularAsync(viagem);
            await _viagemRepository.AtualizarAsync(viagem);
        }
    }

    private static void Validar(SalvarVeiculoRequest request)
    {
        var consumoInvalido = request.ConsumoKmLitro <= 0;
        var kmInvalido = request.KmPorViagem <= 0;
        if (consumoInvalido || kmInvalido)
        {
            throw new DadosInvalidosException("Informe consumo (km/l) e km por viagem maiores que zero.");
        }
    }

    private static void AplicarDados(Veiculo veiculo, SalvarVeiculoRequest request)
    {
        veiculo.Nome = request.Nome;
        veiculo.Modelo = request.Modelo;
        veiculo.ConsumoKmLitro = request.ConsumoKmLitro;
        veiculo.KmPorViagem = request.KmPorViagem;
        veiculo.Combustivel = Enum.Parse<TipoCombustivel>(request.Combustivel, ignoreCase: true);
        veiculo.Padrao = request.Padrao;
    }

    private static VeiculoDto Mapear(Veiculo v) => new()
    {
        Id = v.Id,
        Nome = v.Nome,
        Modelo = v.Modelo,
        ConsumoKmLitro = v.ConsumoKmLitro,
        KmPorViagem = v.KmPorViagem,
        Combustivel = v.Combustivel.ToString(),
        Padrao = v.Padrao,
        Ativo = v.Ativo,
    };
}
