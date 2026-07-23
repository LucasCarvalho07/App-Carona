using System.Text;
using System.Threading.RateLimiting;
using AppCarona.Api;
using AppCarona.Application;
using AppCarona.Application.Configuracao;
using AppCarona.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Camadas
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// Regras de acesso (domínios de e-mail permitidos) e masters
builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection(AuthOptions.Secao));
builder.Services.Configure<AdminOptions>(builder.Configuration.GetSection(AdminOptions.Secao));

builder.Services.AddControllers();

// Validação de modelo ([Required], [EmailAddress]...): devolve { mensagem } (formato que o front lê),
// em vez do ValidationProblemDetails padrão.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var mensagem = context.ModelState.Values
            .SelectMany(estado => estado.Errors)
            .Select(erro => erro.ErrorMessage)
            .FirstOrDefault(m => !string.IsNullOrWhiteSpace(m)) ?? "Dados inválidos.";
        return new BadRequestObjectResult(new { mensagem });
    };
});

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ExcecaoDominioHandler>();

// Autenticação JWT (o token é emitido pela própria API após validar o login Google)
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key não configurada.");

// Não permite subir em produção com a chave placeholder de desenvolvimento.
const string ChaveJwtPlaceholder = "troque-esta-chave-dev-por-uma-secreta-de-no-minimo-32-chars";
if (!builder.Environment.IsDevelopment() && jwtKey == ChaveJwtPlaceholder)
{
    throw new InvalidOperationException(
        "Configure uma chave JWT forte via 'Jwt__Key' em produção (a chave padrão não é permitida).");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// Rate limiting: protege endpoints sensíveis de auth contra brute force / abuso.
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("auth", contexto =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: contexto.Connection.RemoteIpAddress?.ToString() ?? "desconhecido",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
            }));
});

// CORS para o frontend (origens configuráveis; default = Vite local)
var corsOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>()
    ?? new[] { "http://localhost:5173" };
const string CorsFrontend = "frontend";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsFrontend, policy =>
    {
        policy.WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Handler global de exceções (mapeia exceções de domínio → HTTP; sem vazar stack).
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseHsts();
}

// Cabeçalhos de segurança básicos.
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    await next();
});

app.UseHttpsRedirection();
app.UseCors(CorsFrontend);

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapControllers();

app.Run();
