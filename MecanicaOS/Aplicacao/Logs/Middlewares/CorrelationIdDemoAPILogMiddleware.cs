using Dominio.Interfaces.Servicos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Aplicacao.Logs.Middlewares
{
    public class CorrelationIdDemoAPILogMiddleware : IMiddleware
    {
        private const string CorrelationIdHeader = "X-Correlation-ID";

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var correlationIdDemoAPILogService = context.RequestServices.GetRequiredService<ICorrelationIdService>();

            if (correlationIdDemoAPILogService is not null)
            {
                if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId) &&
                    Guid.TryParse(correlationId, out var correlationIdGuid))
                {
                    correlationIdDemoAPILogService.SetCorrelationId(correlationIdGuid);
                }

                context.Response.OnStarting(() =>
                {
                    if (!context.Response.Headers.ContainsKey(CorrelationIdHeader))
                    {
                        context.Response.Headers.Append(CorrelationIdHeader, correlationIdDemoAPILogService.GetCorrelationId());
                    }
                    return Task.CompletedTask;
                });
            }

            return next(context);
        }
    }
}
