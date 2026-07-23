using System.Security.Cryptography;
using AppCarona.Application.Configuracao;
using AppCarona.Application.Mappings;
using AppCarona.Contracts.Auth;
using AppCarona.Domain.Auth;
using AppCarona.Domain.Entities;
using AppCarona.Domain.Enums;
using AppCarona.Domain.Exceptions;
using AppCarona.Domain.Interfaces.Auth;
using AppCarona.Domain.Interfaces.Repositories;
using AppCarona.Domain.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace AppCarona.Application.Services;

public class AuthService : IAuthService
{
    private const int MinutosValidadeCodigo = 10;
    private const int TentativasPorCodigo = 5;

    private readonly IGoogleTokenValidator _googleTokenValidator;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICodigoRecuperacaoRepository _codigoRecuperacaoRepository;
    private readonly IEmailSender _emailSender;
    private readonly AuthOptions _authOptions;
    private readonly AdminOptions _adminOptions;

    public AuthService(
        IGoogleTokenValidator googleTokenValidator,
        IJwtTokenGenerator jwtTokenGenerator,
        IPasswordHasher passwordHasher,
        IUsuarioRepository usuarioRepository,
        ICodigoRecuperacaoRepository codigoRecuperacaoRepository,
        IEmailSender emailSender,
        IOptions<AuthOptions> authOptions,
        IOptions<AdminOptions> adminOptions)
    {
        _googleTokenValidator = googleTokenValidator;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
        _usuarioRepository = usuarioRepository;
        _codigoRecuperacaoRepository = codigoRecuperacaoRepository;
        _emailSender = emailSender;
        _authOptions = authOptions.Value;
        _adminOptions = adminOptions.Value;
    }

    public async Task<AuthResponse> LoginComGoogleAsync(string idToken)
    {
        var dadosGoogle = await _googleTokenValidator.ValidarAsync(idToken);

        ValidarDominio(dadosGoogle.Email);

        var usuario = await _usuarioRepository.ObterPorGoogleSubAsync(dadosGoogle.Sub);

        if (usuario is null)
        {
            usuario = CriarUsuario(dadosGoogle.Email, dadosGoogle.Nome, dadosGoogle.Sub, dadosGoogle.FotoUrl, null);
            await _usuarioRepository.SalvarAsync(usuario);
        }
        else
        {
            await GarantirMasterAsync(usuario);
        }

        return MontarResposta(usuario);
    }

    public async Task<AuthResponse> RegistrarLocalAsync(string nome, string email, string telefone, string senha)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DadosInvalidosException("Informe o e-mail.");
        }
        if (string.IsNullOrWhiteSpace(telefone))
        {
            throw new DadosInvalidosException("Informe o telefone.");
        }
        ValidarForcaSenha(senha);

        ValidarDominio(email);

        var existente = await _usuarioRepository.ObterPorEmailAsync(email);
        if (existente is not null)
        {
            throw new EmailJaCadastradoException("Já existe um usuário com este e-mail.");
        }

        var senhaHash = _passwordHasher.Hash(senha);
        var usuario = CriarUsuario(email, nome, googleSub: null, fotoUrl: null, senhaHash);
        usuario.Telefone = telefone.Trim();

        await _usuarioRepository.SalvarAsync(usuario);

        // Master principal por conta local precisa comprovar posse do e-mail antes de virar master:
        // envia um código de verificação. (Usuário comum não precisa — passa pela aprovação do master.)
        if (EhEmailMaster(usuario.Email))
        {
            var codigo = await GerarESalvarCodigoAsync(usuario);
            var corpo =
                $"Olá, {usuario.Nome}.\n\n" +
                $"Seu código para verificar o e-mail no App Carona é: {codigo}\n\n" +
                $"O código expira em {MinutosValidadeCodigo} minutos.";
            await _emailSender.EnviarAsync(usuario.Email, "Verificação de e-mail — App Carona", corpo);
        }

        return MontarResposta(usuario);
    }

    public async Task<AuthResponse> VerificarEmailAsync(string email, string codigo)
    {
        var usuario = await _usuarioRepository.ObterPorEmailAsync(email);
        if (usuario is null)
        {
            throw new DadosInvalidosException("Código inválido ou expirado.");
        }

        if (usuario.EmailVerificado)
        {
            // Já verificado: nada a fazer, devolve sessão atual.
            await GarantirMasterAsync(usuario);
            return MontarResposta(usuario);
        }

        var registro = await _codigoRecuperacaoRepository.ObterAtivoPorUsuarioAsync(usuario.Id);
        if (registro is null || registro.TentativasRestantes <= 0)
        {
            throw new DadosInvalidosException("Código inválido ou expirado.");
        }

        if (!_passwordHasher.Verificar(codigo, registro.CodigoHash))
        {
            registro.TentativasRestantes--;
            await _codigoRecuperacaoRepository.AtualizarAsync(registro);
            throw new DadosInvalidosException("Código inválido ou expirado.");
        }

        registro.Usado = true;
        await _codigoRecuperacaoRepository.AtualizarAsync(registro);

        usuario.EmailVerificado = true;
        await _usuarioRepository.AtualizarAsync(usuario);

        // Agora que o e-mail está comprovado, promove a master se for o principal.
        await GarantirMasterAsync(usuario);

        return MontarResposta(usuario);
    }

    /// <summary>Reenvia o código de verificação de e-mail (usa o mesmo mecanismo do cadastro).</summary>
    public async Task ReenviarVerificacaoAsync(string email)
    {
        var usuario = await _usuarioRepository.ObterPorEmailAsync(email);
        if (usuario is null || usuario.EmailVerificado)
        {
            return; // resposta genérica no controller
        }

        var codigo = await GerarESalvarCodigoAsync(usuario);
        var corpo =
            $"Olá, {usuario.Nome}.\n\n" +
            $"Seu código para verificar o e-mail no App Carona é: {codigo}\n\n" +
            $"O código expira em {MinutosValidadeCodigo} minutos.";
        await _emailSender.EnviarAsync(usuario.Email, "Verificação de e-mail — App Carona", corpo);
    }

    public async Task SolicitarRecuperacaoAsync(string email, CanalRecuperacao canal)
    {
        if (canal != CanalRecuperacao.Email)
        {
            throw new DadosInvalidosException("Canal indisponível no momento. Use o e-mail.");
        }

        var usuario = await _usuarioRepository.ObterPorEmailAsync(email);

        // Não revela se a conta existe: apenas encerra silenciosamente.
        if (usuario is null || usuario.SenhaHash is null)
        {
            return;
        }

        var codigo = await GerarESalvarCodigoAsync(usuario);

        var corpo =
            $"Olá, {usuario.Nome}.\n\n" +
            $"Seu código para redefinir a senha do App Carona é: {codigo}\n\n" +
            $"O código expira em {MinutosValidadeCodigo} minutos. " +
            "Se você não pediu isso, ignore este e-mail.";

        await _emailSender.EnviarAsync(usuario.Email, "Código de recuperação — App Carona", corpo);
    }

    public async Task<string> VerificarCodigoAsync(string email, string codigo)
    {
        var usuario = await _usuarioRepository.ObterPorEmailAsync(email);
        if (usuario is null)
        {
            throw new DadosInvalidosException("Código inválido ou expirado.");
        }

        var registro = await _codigoRecuperacaoRepository.ObterAtivoPorUsuarioAsync(usuario.Id);
        if (registro is null || registro.TentativasRestantes <= 0)
        {
            throw new DadosInvalidosException("Código inválido ou expirado.");
        }

        var codigoConfere = _passwordHasher.Verificar(codigo, registro.CodigoHash);
        if (!codigoConfere)
        {
            registro.TentativasRestantes--;
            await _codigoRecuperacaoRepository.AtualizarAsync(registro);
            throw new DadosInvalidosException("Código inválido ou expirado.");
        }

        // Correto: emite o token de reset (o código só é marcado como usado ao redefinir).
        return _jwtTokenGenerator.GerarTokenReset(usuario.Id);
    }

    public async Task RedefinirSenhaAsync(string resetToken, string novaSenha)
    {
        var usuarioId = _jwtTokenGenerator.ValidarTokenReset(resetToken);
        if (usuarioId is null)
        {
            throw new DadosInvalidosException("Sessão de redefinição inválida ou expirada.");
        }

        ValidarForcaSenha(novaSenha);

        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId.Value);
        if (usuario is null)
        {
            throw new DadosInvalidosException("Sessão de redefinição inválida ou expirada.");
        }

        var registro = await _codigoRecuperacaoRepository.ObterAtivoPorUsuarioAsync(usuario.Id);
        if (registro is null)
        {
            throw new DadosInvalidosException("Sessão de redefinição inválida ou expirada.");
        }

        registro.Usado = true;
        await _codigoRecuperacaoRepository.AtualizarAsync(registro);

        usuario.SenhaHash = _passwordHasher.Hash(novaSenha);
        await _usuarioRepository.AtualizarAsync(usuario);
    }

    private static string GerarCodigo()
    {
        // 6 dígitos, gerado de forma criptograficamente segura.
        return RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
    }

    /// <summary>Invalida código ativo anterior, gera e salva um novo (hash). Retorna o código em claro.</summary>
    private async Task<string> GerarESalvarCodigoAsync(Usuario usuario)
    {
        var codigoAnterior = await _codigoRecuperacaoRepository.ObterAtivoPorUsuarioAsync(usuario.Id);
        if (codigoAnterior is not null)
        {
            codigoAnterior.Usado = true;
            await _codigoRecuperacaoRepository.AtualizarAsync(codigoAnterior);
        }

        var codigo = GerarCodigo();
        var registro = new CodigoRecuperacao
        {
            UsuarioId = usuario.Id,
            CodigoHash = _passwordHasher.Hash(codigo),
            Canal = CanalRecuperacao.Email,
            ExpiraEm = DateTime.UtcNow.AddMinutes(MinutosValidadeCodigo),
            TentativasRestantes = TentativasPorCodigo,
            Usado = false,
            CriadoEm = DateTime.UtcNow
        };
        await _codigoRecuperacaoRepository.SalvarAsync(registro);
        return codigo;
    }

    /// <summary>Regra de senha: mínimo 6 caracteres, ao menos 1 maiúscula e 1 caractere especial.</summary>
    private static void ValidarForcaSenha(string senha)
    {
        var temTamanho = !string.IsNullOrWhiteSpace(senha) && senha.Length >= 6;
        var temMaiuscula = senha is not null && senha.Any(char.IsUpper);
        var temEspecial = senha is not null && senha.Any(c => !char.IsLetterOrDigit(c));

        if (!temTamanho || !temMaiuscula || !temEspecial)
        {
            throw new DadosInvalidosException(
                "A senha deve ter ao menos 6 caracteres, 1 letra maiúscula e 1 caractere especial.");
        }
    }

    public async Task<AuthResponse> LoginLocalAsync(string email, string senha)
    {
        ValidarDominio(email);

        var usuario = await _usuarioRepository.ObterPorEmailAsync(email);

        var semSenhaLocal = usuario?.SenhaHash is null;
        var senhaConfere = usuario is not null && !semSenhaLocal
            && _passwordHasher.Verificar(senha, usuario.SenhaHash!);

        if (usuario is null || semSenhaLocal || !senhaConfere)
        {
            throw new CredenciaisInvalidasException("E-mail ou senha inválidos.");
        }

        await GarantirMasterAsync(usuario);
        return MontarResposta(usuario);
    }

    private Usuario CriarUsuario(string email, string nome, string? googleSub, string? fotoUrl, string? senhaHash)
    {
        // Master só é concedido a conta com e-mail VERIFICADO pelo Google (googleSub != null).
        // Cadastro local não pode virar Master (evita escalada por auto-cadastro com e-mail de master).
        var ehMasterGoogle = EhEmailMaster(email) && googleSub is not null;

        var usuario = new Usuario
        {
            GoogleSub = googleSub,
            Email = email,
            Nome = nome,
            FotoUrl = fotoUrl,
            SenhaHash = senhaHash,
            // Conta Google tem e-mail verificado pelo próprio Google; local começa não verificada.
            EmailVerificado = googleSub is not null,
            Status = ehMasterGoogle ? StatusUsuario.Ativo : StatusUsuario.AguardandoAprovacao,
            CriadoEm = DateTime.UtcNow
        };

        if (ehMasterGoogle)
        {
            AplicarPapeisDeMaster(usuario);
        }

        return usuario;
    }

    private async Task GarantirMasterAsync(Usuario usuario)
    {
        // Master principal é definido por config (Admin:MasterEmails, vindo de env/.env).
        if (!EhEmailMaster(usuario.Email))
        {
            return;
        }

        // Conta local só vira master depois de comprovar posse do e-mail (código).
        // Google já é verificado. Fecha a brecha de alguém cadastrar o e-mail do master.
        if (usuario.GoogleSub is null && !usuario.EmailVerificado)
        {
            return;
        }

        var adicionouPapel = AplicarPapeisDeMaster(usuario);
        var ativou = usuario.Status != StatusUsuario.Ativo;
        usuario.Status = StatusUsuario.Ativo;

        if (adicionouPapel || ativou)
        {
            await _usuarioRepository.AtualizarAsync(usuario);
        }
    }

    /// <summary>
    /// Master tem acesso total aos menus e também participa como motorista e passageiro.
    /// Retorna true se algum papel foi adicionado.
    /// </summary>
    private static bool AplicarPapeisDeMaster(Usuario usuario)
    {
        var adicionouMaster = usuario.GarantirPapel(Papel.Master);
        var adicionouMotorista = usuario.GarantirPapel(Papel.Motorista);
        var adicionouPassageiro = usuario.GarantirPapel(Papel.Passageiro);
        return adicionouMaster || adicionouMotorista || adicionouPassageiro;
    }

    private AuthResponse MontarResposta(Usuario usuario)
    {
        var dto = UsuarioMapper.ParaDto(usuario);
        dto.EhMasterPrincipal = EhEmailMaster(usuario.Email);
        return new AuthResponse
        {
            Token = _jwtTokenGenerator.Gerar(usuario),
            Usuario = dto
        };
    }

    private void ValidarDominio(string email)
    {
        // Lista vazia = qualquer domínio permitido.
        if (_authOptions.DominiosPermitidos.Length == 0)
        {
            return;
        }

        var dominio = email.Split('@').Last();
        var dominioPermitido = _authOptions.DominiosPermitidos
            .Any(d => d.Equals(dominio, StringComparison.OrdinalIgnoreCase));

        if (!dominioPermitido)
        {
            throw new DominioNaoPermitidoException(
                $"Acesso permitido apenas para e-mails dos domínios: {string.Join(", ", _authOptions.DominiosPermitidos)}.");
        }
    }

    private bool EhEmailMaster(string email)
    {
        return _adminOptions.MasterEmails
            .Any(e => e.Equals(email, StringComparison.OrdinalIgnoreCase));
    }
}
