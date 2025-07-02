using Aplicacao.Logs.Middlewares;
using Aplicacao.Logs.Services;
using Aplicacao.Servicos;
using Dominio.Entidades;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Services;
using Infraestrutura.Dados;
using Infraestrutura.Repositorios;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<CorrelationIdDemoAPILogMiddleware>();
app.UseHttpsRedirection();

app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
