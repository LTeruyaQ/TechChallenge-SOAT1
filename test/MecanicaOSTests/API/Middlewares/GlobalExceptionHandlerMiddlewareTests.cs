using API.Middlewares;
using Dominio.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOSTests.API.Middlewares
{
    public class GlobalExceptionHandlerMiddlewareTests
    {
        private readonly Mock<ILogger<GlobalExceptionHandlerMiddleware>> _loggerMock;

        public GlobalExceptionHandlerMiddlewareTests()
        {
            _loggerMock = new Mock<ILogger<GlobalExceptionHandlerMiddleware>>();
        }

        [Theory]
        [InlineData(typeof(DadosInvalidosException), HttpStatusCode.BadRequest)]
        [InlineData(typeof(DadosNaoEncontradosException), HttpStatusCode.NotFound)]
        [InlineData(typeof(KeyNotFoundException), HttpStatusCode.NotFound)]
        [InlineData(typeof(DadosJaCadastradosException), HttpStatusCode.Conflict)]
        [InlineData(typeof(CredenciaisInvalidasException), HttpStatusCode.Unauthorized)]
        [InlineData(typeof(UsuarioInativoException), HttpStatusCode.Unauthorized)]
        [InlineData(typeof(Exception), HttpStatusCode.InternalServerError)]
        public async Task InvokeAsync_DeveDefinirStatusCodeCorreto_ParaDiferentesExcecoes(Type exceptionType, HttpStatusCode expectedStatusCode)
        {
            // Arrange
            var middleware = new GlobalExceptionHandlerMiddleware(
                (innerHttpContext) => throw (Exception)Activator.CreateInstance(exceptionType, "Test Exception"),
                _loggerMock.Object);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be((int)expectedStatusCode);
            context.Response.ContentType.Should().Be("application/json");

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(context.Response.Body);
            var body = await reader.ReadToEndAsync();
            body.Should().Contain("Test Exception");
        }

        [Fact]
        public async Task InvokeAsync_DeveChamarProximoDelegate_QuandoNenhumaExcecaoOcorre()
        {
            // Arrange
            var nextCalled = false;
            RequestDelegate next = (innerHttpContext) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };
            var middleware = new GlobalExceptionHandlerMiddleware(next, _loggerMock.Object);
            var context = new DefaultHttpContext();

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            nextCalled.Should().BeTrue();
        }
    }
}
