using System.IO.Compression;
using System.Text.Json.Serialization;
using API.Middlewares;
using Aplicacao.Jobs;
using Aplicacao.Servicos;
using Aplicacao.Servicos.Logs;
using Dominio.Entidades;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using Hangfire;
using Hangfire.PostgreSql;
using Infraestrutura.Dados;
using Infraestrutura.Dados.UoT;
using Infraestrutura.Repositorios;
using Infraestrutura.Servicos;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers().AddJsonOptions(options =>
             options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<MecanicaContexto>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString)));

builder.Services.AddHangfireServer();

// Repositórios
builder.Services.AddScoped<ICrudRepositorio<Servico>, ServicoRepositorio>();
builder.Services.AddScoped<ICrudRepositorio<Estoque>, EstoqueRepositorio>();
builder.Services.AddScoped<ICrudRepositorio<Veiculo>, VeiculoRepositorio>();
builder.Services.AddScoped<ICrudRepositorio<Cliente>, ClienteRepositorio>();
builder.Services.AddScoped<ICrudRepositorio<Endereco>, EnderecoRepositorio>();

// Serviços
builder.Services.AddScoped<Aplicacao.Interfaces.Servicos.IServicoServico, ServicoServico>();
builder.Services.AddScoped<Aplicacao.Interfaces.Servicos.IVeiculoServico, VeiculoServico>();
builder.Services.AddScoped<IUnidadeDeTrabalho, UnidadeDeTrabalho>();
builder.Services.AddScoped(typeof(ILogServico<>), typeof(LogServico<>));
// Aplicacao
builder.Services.AddAutoMapper(
    typeof(Aplicacao.Mappings.AutoMapperServicoProfile),
    typeof(Aplicacao.Mappings.EstoqueProfile),
    typeof(Aplicacao.Mappings.VeiculoProfile));

builder.Services.AddScoped<Aplicacao.Interfaces.Servicos.IEstoqueServico, EstoqueServico>();

// Infraestrutura
builder.Services.AddScoped<IServicoNotificacaoEmail, ServicoNotificacaoEmail>();
builder.Services.AddScoped<ICorrelationIdService, CorrelationIdService>();
builder.Services.AddScoped<IdCorrelacionalLogMiddleware>();

// Configuração de compactação de resposta
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

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseMiddleware<IdCorrelacionalLogMiddleware>();

app.UseSwagger(c =>
{
    c.RouteTemplate = "swagger/{documentName}/swagger.json";
    c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
    {
        swaggerDoc.Servers = new List<Microsoft.OpenApi.Models.OpenApiServer>
        {
            new() { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}/api/v1" }
        };
    });
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "MecanicaOS API v1");
    c.RoutePrefix = "docs";
});

app.UseReDoc(c =>
{
    c.DocumentTitle = "MecanicaOS API Documentation";
    c.SpecUrl = "/api/swagger/v1/swagger.json";
    c.RoutePrefix = "api-docs";
});

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
});

app.UseHangfireDashboard("/hangfire");
RecurringJob.AddOrUpdate<VerificarEstoqueJob>(
    recurringJobId: "verificar-estoque",
    methodCall: job => job.ExecutarAsync(),
    cronExpression: Cron.Hourly(),
    options: new RecurringJobOptions
    {
        TimeZone = TimeZoneInfo.Local
    }
);

app.Run();