using AppCarona.Domain.Exceptions;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Diagnostics;

namespace AppCarona.Api;

/// <summary>
/// Handler global: traduz exceções de domínio em respostas HTTP com { mensagem }
/// (formato que o frontend entende). Exceções desconhecidas passam adiante e viram
/// 500 genérico, sem vazar stack trace.
/// </summary>
public class ExcecaoDominioHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var mapeado = Mapear(exception);
        if (mapeado is null)
        {
            return false; // não é exceção de domínio conhecida → deixa o pipeline tratar (500)
        }

        var (status, mensagem) = mapeado.Value;
        httpContext.Response.StatusCode = status;
        await httpContext   .Response.WriteAsJsonAsync(new { mensagem }, cancellationToken);
        return true;
    }

    private static (int status, string mensagem)? Mapear(Exception excecao) => excecao switch
    {
        DadosInvalidosException => (StatusCodes.Status400BadRequest, excecao.Message),
        CredenciaisInvalidasException => (StatusCodes.Status401Unauthorized, excecao.Message),
        EmailJaCadastradoException => (StatusCodes.Status409Conflict, excecao.Message),
        DominioNaoPermitidoException => (StatusCodes.Status403Forbidden, excecao.Message),
        OperacaoNaoPermitidaException => (StatusCodes.Status403Forbidden, excecao.Message),
        InvalidJwtException => (StatusCodes.Status401Unauthorized, "Token do Google inválido."),
        _ => null,
    };
}
