using API.Middlewares;
using Core.Interfaces.Jobs;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Hangfire;
using Hangfire.PostgreSql;
using Infraestrutura.Autenticacao;
using Infraestrutura.Dados;
using Infraestrutura.Dados.Extensions;
using Infraestrutura.Dados.UdT;
using Infraestrutura.Jobs;
using Infraestrutura.Logs;
using Infraestrutura.Notificacoes.OS;
using Infraestrutura.Repositorios;
using Infraestrutura.Servicos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IO.Compression;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers().AddJsonOptions(options =>
             options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MecanicaOS API", Version = "v1" });

    // Adiciona suporte a autenticação JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Cabeçalho de autorização JWT usando o esquema Bearer. Exemplo: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});
builder.Services.AddOpenApi();

builder.Configuration.AddEnvironmentVariables();

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<MecanicaContexto>(options =>
    options.UseNpgsql(connectionString, npgsqlOptionsAction: sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);
    }));

// Configuração JWT
var jwtConfig = builder.Configuration.GetSection("Jwt").Get<ConfiguracaoJwt>();
builder.Services.Configure<ConfiguracaoJwt>(builder.Configuration.GetSection("Jwt"));

// Autenticação JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig.Issuer,
            ValidAudience = jwtConfig.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

// Autorização
builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddHttpContextAccessor();

#region Infraestrutura
// Jobs Hangfire
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString)));

builder.Services.AddHangfireServer();

builder.Services.AddScoped<IVerificarEstoqueJob, VerificarEstoqueJob>();
builder.Services.AddScoped<VerificarEstoqueJob>();
builder.Services.AddScoped<VerificarOrcamentoExpiradoJob>();

// Notificações
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(OrdemServicoEmOrcamentoEvent).Assembly);
});

builder.Services.AddScoped<IServicoEmail, ServicoEmail>();
builder.Services.AddScoped<IServicoJwt, ServicoJwt>();
builder.Services.AddScoped<IServicoSenha, ServicoSenha>();
builder.Services.AddScoped<IUsuarioLogadoServico, UsuarioLogadoServico>();
builder.Services.AddScoped<IIdCorrelacionalService, IdCorrelacionalService>();
builder.Services.AddScoped<IdCorrelacionalLogMiddleware>();
builder.Services.AddScoped<IUnidadeDeTrabalho, UnidadeDeTrabalho>();
builder.Services.AddScoped(typeof(ILogServico<>), typeof(LogServico<>));

// Repositórios
builder.Services.AddScoped(typeof(IRepositorio<>), typeof(Repositorio<>));
#endregion

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
    options.Providers.Add<BrotliCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/json" });
});

var app = builder.Build();

app.UseResponseCompression();

app.UsePathBase(new PathString("/api/v1"));

app.UseRouting();

// Adiciona autenticação e autorização ao pipeline
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseMiddleware<IdCorrelacionalLogMiddleware>();

app.UseSwagger(c =>
{
    c.RouteTemplate = "swagger/{documentName}/swagger.json";
    c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
    {
        swaggerDoc.Servers = new List<OpenApiServer>
        {
            new() { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}/api/v1" }
        };
    });
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/api/v1/swagger/v1/swagger.json", "MecanicaOS API v1");
    c.RoutePrefix = "docs";
});

app.UseReDoc(c =>
{
    c.DocumentTitle = "MecanicaOS API Documentation";
    c.SpecUrl = "/api/v1/swagger/v1/swagger.json";
    c.RoutePrefix = "api-docs";
});

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
});

#if DEBUG
app.UseHangfireDashboard("/hangfire");
#endif

RecurringJob.AddOrUpdate<VerificarEstoqueJob>(
    recurringJobId: "verificar-estoque",
    methodCall: job => job.ExecutarAsync(),
    cronExpression: Cron.Hourly(),
    options: new RecurringJobOptions
    {
        TimeZone = TimeZoneInfo.Local
    }
);

RecurringJob.AddOrUpdate<VerificarOrcamentoExpiradoJob>(
    recurringJobId: "verificar-orcamento-expirado",
    methodCall: job => job.ExecutarAsync(),
    cronExpression: Cron.Hourly(),
    options: new RecurringJobOptions
    {
        TimeZone = TimeZoneInfo.Local
    }
);

try
{
    // Aplicar migrações automaticamente
    using var escopo = app.Services.CreateScope();
    app.Logger.LogInformation("Iniciando aplicação...");

    await app.Services.AplicarMigracoesAsync<MecanicaContexto>();

    app.Logger.LogInformation("Aplicação iniciada com sucesso!");
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Falha crítica ao iniciar a aplicação");
    throw;
}

app.Run();