using AppCarona.Contracts.Escala;
using AppCarona.Contracts.Marcacao;
using AppCarona.Domain.Entities;
using AppCarona.Domain.Enums;
using AppCarona.Domain.Exceptions;
using AppCarona.Domain.Interfaces.Repositories;
using AppCarona.Domain.Interfaces.Services;

namespace AppCarona.Application.Services;

public class EscalaService : IEscalaService
{
    private readonly IViagemRepository _viagemRepository;
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICalculoService _calculoService;

    public EscalaService(
        IViagemRepository viagemRepository,
        IVeiculoRepository veiculoRepository,
        IUsuarioRepository usuarioRepository,
        ICalculoService calculoService)
    {
        _viagemRepository = viagemRepository;
        _veiculoRepository = veiculoRepository;
        _usuarioRepository = usuarioRepository;
        _calculoService = calculoService;
    }

    public async Task EscalarAsync(int motoristaId, DateTime data, Sentido sentido)
    {
        var dia = data.Date;

        var veiculo = await _veiculoRepository.ObterPadraoAsync(motoristaId);
        if (!VeiculoConfigurado(veiculo))
        {
            throw new DadosInvalidosException("Configure o consumo e o km do seu veículo antes de se escalar.");
        }

        var carros = await _viagemRepository.ListarPorDataSentidoAsync(dia, sentido);

        var ehPassageiroNoTrecho = carros.Any(v => v.Participacoes.Any(p => p.Passageiro.Id == motoristaId));
        if (ehPassageiroNoTrecho)
        {
            throw new DadosInvalidosException("Você já está como passageiro nesse dia/sentido. Saia do carro antes de dirigir.");
        }

        var jaEscalado = carros.Any(v => v.Motorista.Id == motoristaId);
        if (jaEscalado)
        {
            return;
        }

        var motorista = await _usuarioRepository.ObterPorIdAsync(motoristaId)
            ?? throw new InvalidOperationException("Motorista não encontrado.");

        var viagem = new Viagem
        {
            Data = dia,
            Sentido = sentido,
            Motorista = motorista,
            Veiculo = veiculo,
            Origem = OrigemMarcacao.Manual,
            CriadoPor = motoristaId,
            CriadoEm = DateTime.UtcNow,
        };

        await _calculoService.RecalcularAsync(viagem);
        await _viagemRepository.SalvarAsync(viagem);
    }

    public async Task EscalarSemanaAsync(int motoristaId, DateTime dataNaSemana)
    {
        var veiculo = await _veiculoRepository.ObterPadraoAsync(motoristaId);
        if (!VeiculoConfigurado(veiculo))
        {
            throw new DadosInvalidosException("Configure o consumo e o km do seu veículo antes de se escalar.");
        }

        var segunda = InicioDaSemana(dataNaSemana);
        for (var i = 0; i < 5; i++)
        {
            var dia = segunda.AddDays(i);
            foreach (var sentido in new[] { Sentido.Ida, Sentido.Volta })
            {
                try
                {
                    await EscalarAsync(motoristaId, dia, sentido);
                }
                catch (DadosInvalidosException)
                {
                    // dia/sentido em conflito (ex.: já é passageiro) é ignorado na escala em lote
                }
            }
        }
    }

    public async Task DesescalarAsync(int motoristaId, DateTime data, Sentido sentido)
    {
        var viagem = await _viagemRepository.ObterPorDataMotoristaSentidoAsync(data.Date, motoristaId, sentido);
        if (viagem is null)
        {
            return;
        }

        if (viagem.Participacoes.Count > 0)
        {
            throw new DadosInvalidosException("Há passageiros nesse dia; eles precisam sair antes.");
        }

        await _viagemRepository.RemoverAsync(viagem);
    }

    public async Task<IList<EscalaCarroDto>> ListarEscalaMesAsync(int usuarioLogadoId, int anoMes)
    {
        var (inicio, fim) = FaixaDoMes(anoMes);
        var viagens = await _viagemRepository.ListarPorPeriodoAsync(inicio, fim);

        var lista = new List<EscalaCarroDto>();
        foreach (var viagem in viagens.OrderBy(v => v.Data).ThenBy(v => v.Sentido).ThenBy(v => v.Motorista.Nome))
        {
            var veiculo = await _veiculoRepository.ObterPadraoAsync(viagem.Motorista.Id);
            lista.Add(new EscalaCarroDto
            {
                ViagemId = viagem.Id,
                Data = viagem.Data,
                Sentido = viagem.Sentido.ToString(),
                MotoristaId = viagem.Motorista.Id,
                MotoristaNome = viagem.Motorista.Nome,
                Avatar = viagem.Motorista.Avatar,
                Configurado = VeiculoConfigurado(veiculo),
                QtdPassageiros = viagem.Participacoes.Count,
                SouMotorista = viagem.Motorista.Id == usuarioLogadoId,
                EstouNesteCarro = viagem.Participacoes.Any(p => p.Passageiro.Id == usuarioLogadoId),
                ValorPorPessoa = viagem.ValorPorPessoa,
                Ocupantes = viagem.Participacoes
                    .Select(p => new OcupanteDto
                    {
                        Id = p.Passageiro.Id,
                        Nome = p.Passageiro.Nome,
                        Avatar = p.Passageiro.Avatar,
                    })
                    .OrderBy(o => o.Nome)
                    .ToList(),
            });
        }

        return lista;
    }

    private static bool VeiculoConfigurado(Veiculo? veiculo)
    {
        return veiculo is not null && veiculo.ConsumoKmLitro > 0 && veiculo.KmPorViagem > 0;
    }

    private static DateTime InicioDaSemana(DateTime data)
    {
        var dia = data.Date;
        var diff = ((int)dia.DayOfWeek + 6) % 7; // segunda = 0
        return dia.AddDays(-diff);
    }

    private static (DateTime inicio, DateTime fim) FaixaDoMes(int anoMes)
    {
        var ano = anoMes / 100;
        var mes = anoMes % 100;
        var inicio = new DateTime(ano, mes, 1);
        return (inicio, inicio.AddMonths(1));
    }
}
