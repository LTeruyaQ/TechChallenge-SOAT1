using Aplicacao.Logs.Models;
using Aplicacao.Logs.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Aplicacao.Servicos.Abstrato
{
    public abstract class AbstractLogService<T> where T : class
    {
        private readonly ICorrelationIdService _correlationIdService;
        private readonly ILogger<T> _logger;
        private readonly JsonSerializerOptions _jsonOptions;
        protected AbstractLogService(ICorrelationIdService correlationIdLog, ILogger<T> logger)
        {
            _correlationIdService = correlationIdLog;
            _jsonOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter()
                },
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            _logger = logger;
        }

        protected void LogInicio(string metodo, object? props = null)
           => LogGeneric(LogLevel.Information, metodo, "Inicio", props);

        protected void LogFim(string metodo, object? retorno = null)
            => LogGeneric(LogLevel.Information, metodo, "Fim", retorno);

        protected void LogErro(string metodo, Exception ex)
        {
            var dados = new
            {
                Mensagem = ex.Message,
                Tipo = ex.GetType().Name,
                ex.StackTrace
            };

            LogGeneric(LogLevel.Error, metodo, "Erro", dados, ex);
        }

        private void LogGeneric(
            LogLevel nivel,
            string metodo,
            string etapa,
            object? dados,
            Exception? exception = null)
        {
            var entry = new LogEntry
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
