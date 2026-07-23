using AppCarona.Contracts.Pagamentos;
using AppCarona.Domain.Entities;
using AppCarona.Domain.Enums;
using AppCarona.Domain.Interfaces.Repositories;
using AppCarona.Domain.Interfaces.Services;

namespace AppCarona.Application.Services;

public class PagamentoService : IPagamentoService
{
    private readonly IViagemRepository _viagemRepository;
    private readonly IPagamentoRepository _pagamentoRepository;
    private readonly IMotoristaConfigRepository _motoristaConfigRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public PagamentoService(
        IViagemRepository viagemRepository,
        IPagamentoRepository pagamentoRepository,
        IMotoristaConfigRepository motoristaConfigRepository,
        IUsuarioRepository usuarioRepository)
    {
        _viagemRepository = viagemRepository;
        _pagamentoRepository = pagamentoRepository;
        _motoristaConfigRepository = motoristaConfigRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<IList<PagamentoResumoDto>> ResumoDoPassageiroAsync(int passageiroId, int anoMes)
    {
        var (inicio, fim) = FaixaDoMes(anoMes);
        var viagens = await _viagemRepository.ListarPorParticipantePeriodoAsync(passageiroId, inicio, fim);

        var resumo = new List<PagamentoResumoDto>();
        foreach (var grupo in viagens.GroupBy(v => v.Motorista.Id))
        {
            var motorista = grupo.First().Motorista;
            var total = grupo.Sum(v => ValorDoPassageiro(v, passageiroId));
            var config = await _motoristaConfigRepository.ObterPorMotoristaAsync(motorista.Id);
            var pagamento = await _pagamentoRepository.ObterAsync(passageiroId, motorista.Id, anoMes);

            resumo.Add(new PagamentoResumoDto
            {
                PagamentoId = pagamento?.Id,
                MotoristaId = motorista.Id,
                MotoristaNome = motorista.Nome,
                AnoMes = anoMes,
                QtdDias = grupo.Count(),
                Total = total,
                Status = (pagamento?.Status ?? StatusPagamento.Pendente).ToString(),
                ChavePix = config?.ChavePix,
                TipoChave = config?.TipoChave.ToString(),
            });
        }

        return resumo.OrderBy(r => r.MotoristaNome).ToList();
    }

    public async Task InformarAsync(int passageiroId, int motoristaId, int anoMes)
    {
        var (inicio, fim) = FaixaDoMes(anoMes);
        var viagens = await _viagemRepository.ListarPorMotoristaPeriodoAsync(motoristaId, inicio, fim);
        var total = viagens.Sum(v => ValorDoPassageiro(v, passageiroId));

        var pagamento = await _pagamentoRepository.ObterAsync(passageiroId, motoristaId, anoMes);
        var novo = pagamento is null;

        if (novo)
        {
            pagamento = new Pagamento
            {
                Passageiro = await _usuarioRepository.ObterPorIdAsync(passageiroId)
                    ?? throw new InvalidOperationException("Passageiro não encontrado."),
                Motorista = await _usuarioRepository.ObterPorIdAsync(motoristaId)
                    ?? throw new InvalidOperationException("Motorista não encontrado."),
                AnoMes = anoMes,
            };
        }

        pagamento!.Valor = total;
        pagamento.Status = StatusPagamento.Informado;
        pagamento.InformadoEm = DateTime.UtcNow;
        pagamento.ConfirmadoEm = null;

        if (novo)
        {
            await _pagamentoRepository.SalvarAsync(pagamento);
        }
        else
        {
            await _pagamentoRepository.AtualizarAsync(pagamento);
        }
    }

    public async Task<IList<RecebimentoDto>> RecebimentosAsync(int motoristaId, int anoMes)
    {
        var (inicio, fim) = FaixaDoMes(anoMes);
        var viagens = await _viagemRepository.ListarPorMotoristaPeriodoAsync(motoristaId, inicio, fim);

        // Soma o que cada passageiro deve ao motorista no mês.
        var porPassageiro = new Dictionary<int, (Usuario passageiro, decimal valor)>();
        foreach (var viagem in viagens)
        {
            foreach (var participacao in viagem.Participacoes)
            {
                var pass = participacao.Passageiro;
                if (pass.Id == motoristaId)
                {
                    continue; // motorista não paga a si mesmo
                }
                var atual = porPassageiro.TryGetValue(pass.Id, out var v) ? v.valor : 0m;
                porPassageiro[pass.Id] = (pass, atual + participacao.ValorDevido);
            }
        }

        var pagamentos = await _pagamentoRepository.ListarPorMotoristaMesAsync(motoristaId, anoMes);
        var porPassageiroPagamento = pagamentos.ToDictionary(p => p.Passageiro.Id);

        var lista = new List<RecebimentoDto>();
        foreach (var (passageiroId, dados) in porPassageiro)
        {
            porPassageiroPagamento.TryGetValue(passageiroId, out var pagamento);
            lista.Add(new RecebimentoDto
            {
                PagamentoId = pagamento?.Id,
                PassageiroId = passageiroId,
                PassageiroNome = dados.passageiro.Nome,
                AnoMes = anoMes,
                Valor = dados.valor,
                Status = (pagamento?.Status ?? StatusPagamento.Pendente).ToString(),
            });
        }

        return lista.OrderBy(r => r.PassageiroNome).ToList();
    }

    public async Task<IList<ResumoMensalMotoristaDto>> ResumoMensalAsync(int anoMes)
    {
        var (inicio, fim) = FaixaDoMes(anoMes);
        var viagens = await _viagemRepository.ListarPorPeriodoAsync(inicio, fim);

        var lista = new List<ResumoMensalMotoristaDto>();
        foreach (var grupo in viagens.GroupBy(v => v.Motorista.Id))
        {
            var motorista = grupo.First().Motorista;
            var qtdDiasDirigiu = grupo.Select(v => v.Data.Date).Distinct().Count();
            var qtdViagens = grupo.Count();

            // Agrega por passageiro: valor, dias distintos e nº de viagens.
            var porPassageiro = new Dictionary<int, (Usuario passageiro, decimal valor, HashSet<DateTime> dias, int viagens)>();
            foreach (var viagem in grupo)
            {
                foreach (var participacao in viagem.Participacoes)
                {
                    var pass = participacao.Passageiro;
                    if (pass.Id == motorista.Id)
                    {
                        continue; // motorista não é passageiro de si mesmo
                    }

                    if (!porPassageiro.TryGetValue(pass.Id, out var acumulado))
                    {
                        acumulado = (pass, 0m, new HashSet<DateTime>(), 0);
                    }

                    acumulado.valor += participacao.ValorDevido;
                    acumulado.dias.Add(viagem.Data.Date);
                    acumulado.viagens += 1;
                    porPassageiro[pass.Id] = acumulado;
                }
            }

            var passageiros = porPassageiro.Values
                .Select(x => new ResumoMensalPassageiroDto
                {
                    PassageiroId = x.passageiro.Id,
                    Nome = x.passageiro.Nome,
                    Avatar = x.passageiro.Avatar,
                    QtdDias = x.dias.Count,
                    QtdViagens = x.viagens,
                    Valor = x.valor,
                })
                .OrderBy(p => p.Nome)
                .ToList();

            lista.Add(new ResumoMensalMotoristaDto
            {
                MotoristaId = motorista.Id,
                MotoristaNome = motorista.Nome,
                Avatar = motorista.Avatar,
                TotalValor = passageiros.Sum(p => p.Valor),
                QtdDiasDirigiu = qtdDiasDirigiu,
                QtdViagens = qtdViagens,
                Passageiros = passageiros,
            });
        }

        return lista.OrderByDescending(m => m.TotalValor).ThenBy(m => m.MotoristaNome).ToList();
    }

    public async Task ConfirmarAsync(int motoristaId, int pagamentoId)
    {
        await AlterarStatusAsync(motoristaId, pagamentoId, StatusPagamento.Confirmado);
    }

    public async Task RejeitarAsync(int motoristaId, int pagamentoId)
    {
        await AlterarStatusAsync(motoristaId, pagamentoId, StatusPagamento.Rejeitado);
    }

    private async Task AlterarStatusAsync(int motoristaId, int pagamentoId, StatusPagamento status)
    {
        var pagamento = await _pagamentoRepository.ObterPorIdAsync(pagamentoId);
        if (pagamento is null || pagamento.Motorista.Id != motoristaId)
        {
            return; // não existe ou não é do motorista logado
        }

        pagamento.Status = status;
        pagamento.ConfirmadoEm = status == StatusPagamento.Confirmado ? DateTime.UtcNow : null;
        await _pagamentoRepository.AtualizarAsync(pagamento);
    }

    private static decimal ValorDoPassageiro(Viagem viagem, int passageiroId)
    {
        var participacao = viagem.Participacoes.FirstOrDefault(p => p.Passageiro.Id == passageiroId);
        return participacao?.ValorDevido ?? 0m;
    }

    private static (DateTime inicio, DateTime fim) FaixaDoMes(int anoMes)
    {
        var ano = anoMes / 100;
        var mes = anoMes % 100;
        var inicio = new DateTime(ano, mes, 1);
        return (inicio, inicio.AddMonths(1));
    }
}
