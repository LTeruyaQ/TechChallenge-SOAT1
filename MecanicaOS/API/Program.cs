using Aplicacao.Jobs;
using Aplicacao.Logs.Middlewares;
using Aplicacao.Logs.Servicos;
using Aplicacao.Servicos;
using Dominio.Entidades;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using Hangfire;
using Hangfire.PostgreSql;
using Infraestrutura.Dados;
using Infraestrutura.Dados.UoT;
using Infraestrutura.Repositorios;
using Infraestrutura.Servicos;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

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

builder.Services.AddScoped<ICrudRepositorio<Servico>, ServicoRepositorio>();
builder.Services.AddScoped<ICrudRepositorio<Estoque>, EstoqueRepositorio>();

builder.Services.AddScoped<IServicoServico, ServicoServico>();
builder.Services.AddScoped<IEstoqueServico, EstoqueServico>();
builder.Services.AddScoped<IUnidadeDeTrabalho, UnidadeDeTrabalho>();
builder.Services.AddScoped(typeof(ILogServico<>), typeof(LogServico<>));
builder.Services.AddScoped<IServicoNotificacaoEmail, ServicoNotificacaoEmail>();
builder.Services.AddScoped<ICorrelationIdService, CorrelationIdService>();
builder.Services.AddScoped<CorrelationIdDemoAPILogMiddleware>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseReDoc(c =>
{
    c.DocumentTitle = "MecanicaOS API Documentation";
    c.SpecUrl = "/swagger/v1/swagger.json";
    c.RoutePrefix = "docs";
});

app.MapControllers();
app.UseMiddleware<CorrelationIdDemoAPILogMiddleware>();

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