using Microsoft.Extensions.Logging;

namespace Infraestrutura.Logs.DTOs
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
        public string Usuario { get; set; }
    }
}