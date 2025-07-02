using Aplicacao.Logs.Middlewares;
using Aplicacao.Logs.Services;
using Aplicacao.Servicos;
using Dominio.Entidades;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Services;
using Infraestrutura.Dados;
using Infraestrutura.Repositorios;
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

builder.Services.AddDbContext<MecanicaContexto>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICrudRepositorio<Servico>, ServicoRepositorio>();
builder.Services.AddScoped<ICrudRepositorio<Estoque>, EstoqueRepositorio>();

builder.Services.AddScoped<IServicoServico, ServicoServico>();
builder.Services.AddScoped<IEstoqueServico, EstoqueServico>();
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

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
