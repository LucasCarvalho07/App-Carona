using AppCarona.Contracts.Custo;
using AppCarona.Domain.Entities;
using AppCarona.Domain.Interfaces.Repositories;
using AppCarona.Domain.Interfaces.Services;

namespace AppCarona.Application.Services;

public class ParametroCustoService : IParametroCustoService
{
    private readonly IParametroCustoRepository _parametroRepository;

    public ParametroCustoService(IParametroCustoRepository parametroRepository)
    {
        _parametroRepository = parametroRepository;
    }

    public async Task<IList<ParametroCustoDto>> ListarAsync()
    {
        var parametros = await _parametroRepository.ListarAsync();
        return parametros.OrderByDescending(p => p.VigenteDe).Select(Mapear).ToList();
    }

    public async Task<ParametroCustoDto> SalvarAsync(SalvarParametroCustoRequest request)
    {
        var vigenteDe = request.VigenteDe.Date;
        var parametro = await _parametroRepository.ObterPorVigenteDeAsync(vigenteDe);
        var novo = parametro is null;

        parametro ??= new ParametroCusto { VigenteDe = vigenteDe };
        parametro.PrecoLitro = request.PrecoLitro;
        parametro.CustoKmManutencao = request.CustoKmManutencao;

        if (novo)
        {
            await _parametroRepository.SalvarAsync(parametro);
        }
        else
        {
            await _parametroRepository.AtualizarAsync(parametro);
        }

        return Mapear(parametro);
    }

    private static ParametroCustoDto Mapear(ParametroCusto p) => new()
    {
        Id = p.Id,
        VigenteDe = p.VigenteDe,
        PrecoLitro = p.PrecoLitro,
        CustoKmManutencao = p.CustoKmManutencao,
    };
}
