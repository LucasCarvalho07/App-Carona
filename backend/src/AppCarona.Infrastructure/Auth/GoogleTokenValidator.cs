using AppCarona.Domain.Auth;
using AppCarona.Domain.Interfaces.Auth;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;

namespace AppCarona.Infrastructure.Auth;

public class GoogleTokenValidator : IGoogleTokenValidator
{
    private readonly GoogleAuthOptions _options;

    public GoogleTokenValidator(IOptions<GoogleAuthOptions> options)
    {
        _options = options.Value;
    }

    public async Task<DadosUsuarioGoogle> ValidarAsync(string idToken)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new[] { _options.ClientId }
        };

        // Lança InvalidJwtException se o token for inválido/expirado/audience errada.
        var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

        return new DadosUsuarioGoogle
        {
            Sub = payload.Subject,
            Email = payload.Email,
            Nome = payload.Name ?? payload.Email,
            FotoUrl = payload.Picture
        };
    }
}
