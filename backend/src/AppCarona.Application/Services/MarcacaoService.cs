using AppCarona.Contracts.Marcacao;
using AppCarona.Domain.Entities;
using AppCarona.Domain.Enums;
using AppCarona.Domain.Exceptions;
using AppCarona.Domain.Interfaces.Repositories;
using AppCarona.Domain.Interfaces.Services;

namespace AppCarona.Application.Services;

public class MarcacaoService : IMarcacaoService
{
    private readonly IViagemRepository _viagemRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICalculoService _calculoService;

    public MarcacaoService(
        IViagemRepository viagemRepository,
        IUsuarioRepository usuarioRepository,
        ICalculoService calculoService)
    {
        _viagemRepository = viagemRepository;
        _usuarioRepository = usuarioRepository;
        _calculoService = calculoService;
    }

    public async Task<MinhaMarcacaoDto> MarcarAsync(int passageiroId, DateTime data, int motoristaId, Sentido sentido)
    {
        var dia = data.Date;

        // Só é possível entrar num carro de motorista escalado (a viagem já existe).
        var viagem = await _viagemRepository.ObterPorDataMotoristaSentidoAsync(dia, motoristaId, sentido);
        if (viagem is null)
        {
            throw new DadosInvalidosException("Motorista não escalado nesse dia.");
        }

        var carros = await _viagemRepository.ListarPorDataSentidoAsync(dia, sentido);

        // Conflito: quem é motorista escalado no trecho não pode entrar como passageiro.
        var ehMotoristaNoTrecho = carros.Any(v => v.Motorista.Id == passageiroId);
        if (ehMotoristaNoTrecho)
        {
            throw new DadosInvalidosException("Você está escalado como motorista nesse dia/sentido.");
        }

        // Um carro por trecho: sai de qualquer outro carro do mesmo dia/sentido.
        foreach (var outro in carros.Where(v => v.Id != viagem.Id))
        {
            var participacaoAntiga = outro.Participacoes.FirstOrDefault(p => p.Passageiro.Id == passageiroId);
            if (participacaoAntiga is not null)
            {
                outro.Participacoes.Remove(participacaoAntiga);
                await _calculoService.RecalcularAsync(outro);
                await _viagemRepository.AtualizarAsync(outro);
            }
        }

        var jaParticipa = viagem.Participacoes.Any(p => p.Passageiro.Id == passageiroId);
        if (!jaParticipa)
        {
            var passageiro = await _usuarioRepository.ObterPorIdAsync(passageiroId)
                ?? throw new InvalidOperationException("Passageiro não encontrado.");
            viagem.Participacoes.Add(new Participacao
            {
                Passageiro = passageiro,
                Origem = OrigemMarcacao.Manual,
            });
        }

        await _calculoService.RecalcularAsync(viagem);
        await _viagemRepository.AtualizarAsync(viagem);

        return Mapear(viagem);
    }

    public async Task DesmarcarAsync(int passageiroId, DateTime data, int motoristaId, Sentido sentido)
    {
        var viagem = await _viagemRepository.ObterPorDataMotoristaSentidoAsync(data.Date, motoristaId, sentido);
        var participacao = viagem?.Participacoes.FirstOrDefault(p => p.Passageiro.Id == passageiroId);
        if (viagem is null || participacao is null)
        {
            return;
        }

        // Mantém a viagem mesmo vazia: o motorista segue escalado (vai sozinho).
        viagem.Participacoes.Remove(participacao);
        await _calculoService.RecalcularAsync(viagem);
        await _viagemRepository.AtualizarAsync(viagem);
    }

    public async Task<IList<MinhaMarcacaoDto>> ListarMinhasAsync(int passageiroId, int anoMes)
    {
        var (inicio, fim) = FaixaDoMes(anoMes);
        var viagens = await _viagemRepository.ListarPorParticipantePeriodoAsync(passageiroId, inicio, fim);
        return viagens
            .OrderBy(v => v.Data).ThenBy(v => v.Sentido)
            .Select(Mapear)
            .ToList();
    }

    private static (DateTime inicio, DateTime fim) FaixaDoMes(int anoMes)
    {
        var ano = anoMes / 100;
        var mes = anoMes % 100;
        var inicio = new DateTime(ano, mes, 1);
        return (inicio, inicio.AddMonths(1));
    }

    private static MinhaMarcacaoDto Mapear(Viagem v) => new()
    {
        ViagemId = v.Id,
        Data = v.Data,
        Sentido = v.Sentido.ToString(),
        MotoristaId = v.Motorista.Id,
        MotoristaNome = v.Motorista.Nome,
        ValorPorPessoa = v.ValorPorPessoa,
        Ocupantes = v.Participacoes
            .Select(p => new OcupanteDto
            {
                Id = p.Passageiro.Id,
                Nome = p.Passageiro.Nome,
                Avatar = p.Passageiro.Avatar,
            })
            .OrderBy(o => o.Nome)
            .ToList(),
    };
}
