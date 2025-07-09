using System.Text.Json;
using System.Text.Json.Serialization;
using Aplicacao.DTOs.Log;
using Dominio.Interfaces.Servicos;
using Microsoft.Extensions.Logging;

namespace Aplicacao.Servicos.Logs
{
    public class LogServico<T> : ILogServico<T>
    {
        private readonly IIdCorrelacionalService _correlationIdService;
        private readonly ILogger<T> _logger;
        private readonly JsonSerializerOptions _jsonOptions;
        public LogServico(IIdCorrelacionalService correlationIdService, ILogger<T> logger)
        {
            _correlationIdService = correlationIdService;
            _logger = logger;

            _jsonOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter()
                },
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false
            };
        }

        public void LogErro(string metodo, Exception ex)
        {
            var dados = new
            {
                Mensagem = ex.Message,
                Tipo = ex.GetType().Name,
                ex.StackTrace
            };

            LogGeneric(LogLevel.Error, metodo, "Erro", dados, ex);
        }

        public void LogFim(string metodo, object? retorno = null)
            => LogGeneric(LogLevel.Information, metodo, "Fim", retorno);

        public void LogInicio(string metodo, object? props = null)
            => LogGeneric(LogLevel.Information, metodo, "Inicio", props);


        private void LogGeneric(
            LogLevel nivel,
            string metodo,
            string etapa,
            object? dados,
            Exception? exception = null)
        {
            var entry = new LogEntryDto
            {
                Nivel = nivel,
                Classe = typeof(T).Name,
                Metodo = metodo,
                Etapa = etapa,
                CorrelationId = _correlationIdService.GetCorrelationId(),
                Dados = dados,
                Timestamp = DateTime.UtcNow
            };

            var payload = JsonSerializer.Serialize(entry, _jsonOptions);

            if (nivel == LogLevel.Error)
                _logger.LogError(exception, "{LogEntry}", payload);
            else
                _logger.LogInformation("{LogEntry}", payload);
        }

    }
}
