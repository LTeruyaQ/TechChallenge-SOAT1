using API.Middlewares;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.Jobs;
using Aplicacao.Mapeamentos;
using Aplicacao.Notificacoes.OS;
using Aplicacao.Servicos;
using Aplicacao.UseCases.Estoque.AtualizarEstoque;
using Aplicacao.UseCases.Estoque.CriarEstoque;
using Aplicacao.UseCases.Estoque.DeletarEstoque;
using Aplicacao.UseCases.Estoque.ObterEstoque;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using Hangfire;
using Hangfire.PostgreSql;
using Infraestrutura.Autenticacao;
using Infraestrutura.Dados;
using Infraestrutura.Dados.Extensions;
using Infraestrutura.Dados.UoT;
using Infraestrutura.Logs;
using Infraestrutura.Repositories;
using Infraestrutura.Repositorios;
using Infraestrutura.Servicos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
    .ConfigureApplicationPartManager(manager =>
    {
        // Removendo EstoqueController do pipeline de controllers para teste
        var controllerFeatureProvider = manager.FeatureProviders
            .OfType<ControllerFeatureProvider>()
            .FirstOrDefault();

        if (controllerFeatureProvider != null)
        {
            manager.FeatureProviders.Remove(controllerFeatureProvider);
        }

        manager.FeatureProviders.Add(new FilterEstoqueControllerFeatureProvider());
    });
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

builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString)));

builder.Services.AddHangfireServer();

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

// Serviços
builder.Services.AddScoped<IServicoServico, ServicoServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();
builder.Services.AddScoped<IEstoqueServico, EstoqueServico>();
builder.Services.AddScoped<IClienteServico, ClienteServico>();
builder.Services.AddScoped<IOrdemServicoServico, OrdemServicoServico>();
builder.Services.AddScoped<IInsumoOSServico, InsumoOSServico>();
builder.Services.AddScoped<IOrcamentoServico, OrcamentoServico>();
builder.Services.AddScoped(typeof(ILogServico<>), typeof(LogServico<>));

// Serviços de autenticação
builder.Services.AddScoped<IServicoJwt, ServicoJwt>();
builder.Services.AddScoped<IServicoSenha, ServicoSenha>();
builder.Services.AddScoped<IAutenticacaoServico, AutenticacaoServico>();

// Serviço de usuário logado
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUsuarioLogadoServico, UsuarioLogadoServico>();

// Serviço de usuário deve ser registrado após os serviços de autenticação
builder.Services.AddScoped<IUsuarioServico, UsuarioServico>();
builder.Services.AddScoped(typeof(ILogServico<>), typeof(LogServico<>));

// Jobs Hangfire
builder.Services.AddScoped<VerificarEstoqueJob>();

// Notificações
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(OrdemServicoEmOrcamentoEvent).Assembly);
});

// Aplicacao
builder.Services.AddAutoMapper(
    typeof(ServicoProfile),
    typeof(VeiculoProfile),
    typeof(UsuarioProfile),
    typeof(ClienteProfile),
    typeof(OrdemServicoProfile),
    typeof(InsumoOSProfile));

// Infraestrutura
builder.Services.AddScoped<IServicoEmail, ServicoEmail>();
builder.Services.AddScoped<IServicoJwt, ServicoJwt>();
builder.Services.AddScoped<IServicoSenha, ServicoSenha>();
builder.Services.AddScoped<IIdCorrelacionalService, IdCorrelacionalService>();
builder.Services.AddScoped<IdCorrelacionalLogMiddleware>();
builder.Services.AddScoped<IUnidadeDeTrabalho, UnidadeDeTrabalho>();
builder.Services.AddScoped(typeof(ILogServico<>), typeof(LogServico<>));

// Repositórios
builder.Services.AddScoped(typeof(IRepositorio<>), typeof(Repositorio<>));

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

#region Mapeando manualmente endpoints
var estoqueLogger = LoggerFactory.Create(logging => logging.AddConsole())
                                 .CreateLogger("EstoqueEndpoints");

// Criação do DbContext manual
var options = new DbContextOptionsBuilder<MecanicaContexto>()
    .UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null
        );
    })
    .Options;

// DbContext
var dbContext = new MecanicaContexto(options);

// Repositório e UoW
var estoqueRepo = new EstoqueRepositorio(dbContext);
var udt = new UnidadeDeTrabalho(dbContext);

// UseCase
var criarEstoqueUseCase = new CriarEstoqueUseCase(estoqueRepo, udt);
var atualizarEstoqueUseCase = new AtualizarEstoqueUseCase(estoqueRepo, udt);
var deletarEstoqueUseCase = new DeletarEstoqueUseCase(estoqueRepo, udt);
var obterEstoquePorIdUseCase = new ObterEstoquePorIdUseCase(estoqueRepo);

// Endpoints manuais
app.MapPost("/Estoque", async (CriarEstoqueRequest request) =>
{
    try
    {
        var response = await criarEstoqueUseCase.ExecuteAsync(request);
        estoqueLogger.LogInformation("Estoque criado com sucesso {@Response}", response);
        return Results.Created($"/Estoque/{response.Id}", response);
    }
    catch (DomainException ex)
    {
        estoqueLogger.LogError(ex, "Regra de negócio violada ao criar estoque");
        return Results.Problem(
            detail: ex.Message,
            statusCode: StatusCodes.Status400BadRequest
        );
    }
    catch (PersistirDadosException ex)
    {
        estoqueLogger.LogError(ex, "Erro ao persistir dados");
        return Results.Problem(ex.Message);
    }
    catch (Exception ex)
    {
        estoqueLogger.LogError(ex, "Erro ao criar estoque");
        return Results.Problem("Erro interno no servidor");
    }
});

app.MapGet("/Estoque/{id:guid}", async (Guid id) =>
{
    try
    {
        var response = await obterEstoquePorIdUseCase.ExecuteAsync(id);

        estoqueLogger.LogInformation("Estoque consultado com sucesso {@Response}", response);

        return Results.Ok(response);
    }
    catch (DadosNaoEncontradosException ex)
    {
        estoqueLogger.LogError(ex, "Estoque não encontrado");
        return Results.NotFound(ex.Message);
    }
    catch (Exception ex)
    {
        estoqueLogger.LogError(ex, "Erro ao obter estoque");
        return Results.Problem("Erro interno no servidor");
    }
});

app.MapPatch("/Estoque/{id:guid}", async (Guid id, AtualizarEstoqueRequest request) =>
{
    try
    {
        var response = await atualizarEstoqueUseCase.ExecuteAsync(id, request);

        estoqueLogger.LogInformation("Estoque atualizado com sucesso {@Response}", response);

        return Results.Ok(response);
    }
    catch (DomainException ex)
    {
        estoqueLogger.LogWarning(ex, "Regra de negócio violada ao atualizar estoque ID {Id}", id);
        return Results.Problem(ex.Message, statusCode: 400);
    }
    catch (PersistirDadosException ex)
    {
        estoqueLogger.LogError(ex, "Erro ao persistir dados ao atualizar estoque ID {Id}", id);
        return Results.Problem(ex.Message, statusCode: 500);
    }
    catch (Exception ex)
    {
        estoqueLogger.LogError(ex, "Erro inesperado ao atualizar estoque ID {Id}", id);
        return Results.Problem("Erro interno no servidor", statusCode: 500);
    }
});

app.MapDelete("/Estoque/{id:guid}", async (Guid id) =>
{
    try
    {
        var response = await deletarEstoqueUseCase.ExecuteAsync(id);
        estoqueLogger.LogInformation("Estoque apagado com sucesso");
        return Results.NoContent();

    }
    catch (DadosNaoEncontradosException ex)
    {
        estoqueLogger.LogError(ex, "Estoque não encontrado");
        return Results.NotFound(ex.Message);
    }
    catch (Exception ex)
    {
        estoqueLogger.LogError(ex, "Erro ao obter estoque");
        return Results.Problem("Erro interno no servidor");
    }
});
#endregion

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

// FeatureProvider customizado
public class FilterEstoqueControllerFeatureProvider : ControllerFeatureProvider
{
    protected override bool IsController(TypeInfo typeInfo)
    {
        // Ignora EstoqueController
        if (typeInfo.AsType() == typeof(API.Controllers.EstoqueController))
            return false;

        return base.IsController(typeInfo);
    }
}