using AppCarona.Contracts.Motorista;
using AppCarona.Domain.Entities;
using AppCarona.Domain.Enums;
using AppCarona.Domain.Interfaces.Repositories;
using AppCarona.Domain.Interfaces.Services;

namespace AppCarona.Application.Services;

public class MotoristaConfigService : IMotoristaConfigService
{
    private readonly IMotoristaConfigRepository _configRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public MotoristaConfigService(
        IMotoristaConfigRepository configRepository,
        IUsuarioRepository usuarioRepository)
    {
        _configRepository = configRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<MotoristaConfigDto?> ObterAsync(int motoristaId)
    {
        var config = await _configRepository.ObterPorMotoristaAsync(motoristaId);
        return config is null ? null : Mapear(config);
    }

    public async Task<MotoristaConfigDto> SalvarAsync(int motoristaId, SalvarMotoristaConfigRequest request)
    {
        var config = await _configRepository.ObterPorMotoristaAsync(motoristaId);
        var novo = config is null;

        if (novo)
        {
            var motorista = await _usuarioRepository.ObterPorIdAsync(motoristaId)
                ?? throw new InvalidOperationException("Motorista não encontrado.");
            config = new MotoristaConfig { Motorista = motorista };
        }

        config!.ChavePix = request.ChavePix;
        config.TipoChave = Enum.Parse<TipoChavePix>(request.TipoChave, ignoreCase: true);
        config.Titular = request.Titular;
        config.EmailComprovante = request.EmailComprovante;

        if (novo)
        {
            await _configRepository.SalvarAsync(config);
        }
        else
        {
            await _configRepository.AtualizarAsync(config);
        }

        return Mapear(config);
    }

    private static MotoristaConfigDto Mapear(MotoristaConfig c) => new()
    {
        ChavePix = c.ChavePix,
        TipoChave = c.TipoChave.ToString(),
        Titular = c.Titular,
        EmailComprovante = c.EmailComprovante,
    };
}
