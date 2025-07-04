﻿using Microsoft.Extensions.Logging;

namespace Dominio.DTOs.Logs
{
    public sealed class LogEntryDto
    {
        public LogLevel Nivel { get; init; }
        public string? Classe { get; init; }
        public string? Metodo { get; init; }
        public string? CorrelationId { get; init; }
        public object? Dados { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public string? Etapa { get; set; }
    }
}
