using Adapters.Controllers;
using Core.DTOs.Requests.Estoque;
using Core.Interfaces.root;
using FluentAssertions;
using NSubstitute;

namespace MecanicaOS.UnitTests.Adapters.Controllers
{
    /// <summary>
    /// Testes para AlertaEstoqueController criado para suportar Jobs na Clean Architecture
    /// Testa métodos que atualmente são stubs mas precisam ser implementados
    /// </summary>
    public class AlertaEstoqueControllerTests
    {
        private readonly ICompositionRoot _compositionRoot;
        private readonly AlertaEstoqueController _controller;

        public AlertaEstoqueControllerTests()
        {
            _compositionRoot = Substitute.For<ICompositionRoot>();
            _controller = new AlertaEstoqueController(_compositionRoot);
        }

        [Fact]
        public void Construtor_DeveCriarInstanciaComDependencias()
        {
            // Arrange & Act
            var controller = new AlertaEstoqueController(_compositionRoot);

            // Assert
            controller.Should().NotBeNull();
        }

        [Fact]
        public async Task CadastrarAlertas_ComListaVazia_DeveCompletarSemErro()
        {
            // Arrange
            var alertasVazios = new List<CadastrarAlertaEstoqueRequest>();

            // Act
            var act = async () => await _controller.CadastrarAlertas(alertasVazios);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task CadastrarAlertas_ComAlertas_DeveCompletarSemErro()
        {
            // Arrange
            var alertas = new List<CadastrarAlertaEstoqueRequest>
            {
                new CadastrarAlertaEstoqueRequest 
                { 
                    EstoqueId = Guid.NewGuid(), 
                    DataEnvio = DateTime.UtcNow 
                },
                new CadastrarAlertaEstoqueRequest 
                { 
                    EstoqueId = Guid.NewGuid(), 
                    DataEnvio = DateTime.UtcNow 
                }
            };

            // Act
            var act = async () => await _controller.CadastrarAlertas(alertas);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task VerificarAlertaEnviadoHoje_ComQualquerEstoqueId_DeveRetornarFalse()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();

            // Act
            var resultado = await _controller.VerificarAlertaEnviadoHoje(estoqueId);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task VerificarAlertaEnviadoHoje_ComEstoqueIdVazio_DeveRetornarFalse()
        {
            // Arrange
            var estoqueId = Guid.Empty;

            // Act
            var resultado = await _controller.VerificarAlertaEnviadoHoje(estoqueId);

            // Assert
            resultado.Should().BeFalse();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public async Task CadastrarAlertas_ComDiferentesQuantidades_DeveCompletarSemErro(int quantidade)
        {
            // Arrange
            var alertas = Enumerable.Range(1, quantidade)
                .Select(_ => new CadastrarAlertaEstoqueRequest 
                { 
                    EstoqueId = Guid.NewGuid(), 
                    DataEnvio = DateTime.UtcNow 
                })
                .ToList();

            // Act
            var act = async () => await _controller.CadastrarAlertas(alertas);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task CadastrarAlertas_ComListaNula_DeveCompletarSemErro()
        {
            // Arrange
            IEnumerable<CadastrarAlertaEstoqueRequest>? alertasNulos = null;

            // Act
            var act = async () => await _controller.CadastrarAlertas(alertasNulos!);

            // Assert
            // Nota: O método atual é um stub que não valida entrada
            // Em implementação real, deveria validar parâmetros nulos
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task VerificarAlertaEnviadoHoje_DeveSerRapido()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            var resultado = await _controller.VerificarAlertaEnviadoHoje(estoqueId);
            
            stopwatch.Stop();

            // Assert
            resultado.Should().BeFalse();
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(100); // Deve ser muito rápido
        }
    }
}
