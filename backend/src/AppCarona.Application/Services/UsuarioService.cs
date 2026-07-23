using AppCarona.Application.Configuracao;
using AppCarona.Application.Mappings;
using AppCarona.Contracts.Usuarios;
using AppCarona.Domain.Entities;
using AppCarona.Domain.Enums;
using AppCarona.Domain.Exceptions;
using AppCarona.Domain.Interfaces.Repositories;
using AppCarona.Domain.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace AppCarona.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly AdminOptions _adminOptions;

    public UsuarioService(IUsuarioRepository usuarioRepository, IOptions<AdminOptions> adminOptions)
    {
        _usuarioRepository = usuarioRepository;
        _adminOptions = adminOptions.Value;
    }

    public async Task<IList<UsuarioDto>> ListarAsync()
    {
        var usuarios = await _usuarioRepository.ListarAsync();
        return usuarios.Select(MapearComPrincipal).ToList();
    }

    public async Task<IList<UsuarioDto>> ListarMotoristasAtivosAsync()
    {
        var motoristas = await _usuarioRepository.ListarMotoristasAtivosAsync();
        return motoristas.Select(MapearComPrincipal).ToList();
    }

    public async Task<UsuarioDto?> ObterPorIdAsync(int id)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(id);
        return usuario is null ? null : MapearComPrincipal(usuario);
    }

    public async Task<UsuarioDto?> AprovarAsync(int id, IEnumerable<string> papeis)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(id);
        if (usuario is null)
        {
            return null;
        }

        AplicarPapeis(usuario, papeis);
        usuario.Status = StatusUsuario.Ativo;

        await _usuarioRepository.AtualizarAsync(usuario);
        return MapearComPrincipal(usuario);
    }

    public async Task<UsuarioDto?> AtualizarAvatarAsync(int id, string avatar)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(id);
        if (usuario is null)
        {
            return null;
        }

        usuario.Avatar = avatar;
        await _usuarioRepository.AtualizarAsync(usuario);
        return MapearComPrincipal(usuario);
    }

    public async Task<UsuarioDto?> DefinirMasterAsync(int id, bool tornarMaster, string emailSolicitante)
    {
        // Apenas o master PRINCIPAL (e-mail em Admin:MasterEmails) gerencia masters.
        // Masters promovidos não podem promover/rebaixar outros (evita confusão).
        var solicitanteEhPrincipal = UsuarioMapper.EhMasterPrincipal(emailSolicitante, _adminOptions.MasterEmails);
        if (!solicitanteEhPrincipal)
        {
            throw new OperacaoNaoPermitidaException("Apenas o master principal pode gerenciar masters.");
        }

        var usuario = await _usuarioRepository.ObterPorIdAsync(id);
        if (usuario is null)
        {
            return null;
        }

        var alvoEhPrincipal = UsuarioMapper.EhMasterPrincipal(usuario.Email, _adminOptions.MasterEmails);

        if (tornarMaster)
        {
            usuario.Status = StatusUsuario.Ativo;
            usuario.GarantirPapel(Papel.Passageiro);
            usuario.GarantirPapel(Papel.Master);
        }
        else
        {
            if (alvoEhPrincipal)
            {
                throw new OperacaoNaoPermitidaException("Não é possível remover o master principal.");
            }
            var papelMaster = usuario.Papeis.FirstOrDefault(p => p.Papel == Papel.Master);
            if (papelMaster is not null)
            {
                usuario.Papeis.Remove(papelMaster);
            }
        }

        await _usuarioRepository.AtualizarAsync(usuario);
        return MapearComPrincipal(usuario);
    }

    private UsuarioDto MapearComPrincipal(Usuario usuario)
    {
        var dto = UsuarioMapper.ParaDto(usuario);
        dto.EhMasterPrincipal = UsuarioMapper.EhMasterPrincipal(usuario.Email, _adminOptions.MasterEmails);
        return dto;
    }

    private static void AplicarPapeis(Usuario usuario, IEnumerable<string> papeis)
    {
        usuario.Papeis.Clear();

        // Passageiro é o papel base: todo usuário aprovado é passageiro.
        var papeisFinais = new HashSet<Papel> { Papel.Passageiro };

        foreach (var papelTexto in papeis)
        {
            var papelValido = Enum.TryParse<Papel>(papelTexto, ignoreCase: true, out var papel);
            // Aprovação nunca concede Master (auto-promovido à parte).
            if (papelValido && papel != Papel.Master)
            {
                papeisFinais.Add(papel);
            }
        }

        foreach (var papel in papeisFinais)
        {
            usuario.Papeis.Add(new UsuarioPapel { Papel = papel });
        }
    }
}
