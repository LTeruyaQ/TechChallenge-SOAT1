using Microsoft.Extensions.Logging;

namespace Aplicacao.DTOs.Log
{
    public class LogEntryDto
    {
        public LogLevel Nivel { get; set; }
        public string Classe { get; set; }
        public string Metodo { get; set; }
        public string Etapa { get; set; }
        public string CorrelationId { get; set; }
        public object Dados { get; set; }
        public DateTime Timestamp { get; set; }
    }
}