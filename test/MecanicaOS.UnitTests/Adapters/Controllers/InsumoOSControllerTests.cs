using Adapters.Controllers;
using Core.DTOs.Requests.OrdemServico.InsumoOS;
using Core.DTOs.Responses.OrdemServico;
using Core.DTOs.UseCases.OrdemServico.InsumoOS;
using Core.Entidades;
using Core.Interfaces.Controllers;
using Core.Interfaces.Presenters;
using Core.Interfaces.root;
using Core.Interfaces.UseCases;
using NSubstitute;
using FluentAssertions;
using Xunit;
using Core.DTOs.Responses.OrdemServico.InsumoOrdemServico;

namespace MecanicaOS.UnitTests.Adapters.Controllers
{
    public class InsumoOSControllerTests
    {
        private readonly IInsumoOSUseCases _insumoOSUseCases;
        private readonly IInsumoPresenter _insumoPresenter;
        private readonly InsumoOSController _insumoOSController;
        private readonly ICompositionRoot _compositionRoot;

        public InsumoOSControllerTests()
        {
            _insumoOSUseCases = Substitute.For<IInsumoOSUseCases>();
            _insumoPresenter = Substitute.For<IInsumoPresenter>();
            _compositionRoot = Substitute.For<ICompositionRoot>();
            
            _compositionRoot.CriarInsumoOSUseCases().Returns(_insumoOSUseCases);
            _insumoOSController = new InsumoOSController(_compositionRoot);
            
            // Usar reflexão para injetar o presenter mockado
            var presenterField = typeof(InsumoOSController).GetField("_insumoPresenter", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            presenterField?.SetValue(_insumoOSController, _insumoPresenter);
        }

        [Fact]
        public void MapearParaCadastrarInsumoOSUseCaseDto_ComRequestValido_DeveMapearCorretamente()
        {
            // Arrange
            var request = new CadastrarInsumoOSRequest
            {
                EstoqueId = Guid.NewGuid(),
                Quantidade = 5
            };

            // Act
            var result = _insumoOSController.MapearParaCadastrarInsumoOSUseCaseDto(request);

            // Assert
            result.Should().NotBeNull();
            result.EstoqueId.Should().Be(request.EstoqueId);
            result.Quantidade.Should().Be(request.Quantidade);
        }

        [Fact]
        public void MapearParaCadastrarInsumoOSUseCaseDto_ComRequestNulo_DeveRetornarNulo()
        {
            // Act
            var result = _insumoOSController.MapearParaCadastrarInsumoOSUseCaseDto(null);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CadastrarInsumos_DeveUsarMapeamentoEChamarUseCase()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var requests = new List<CadastrarInsumoOSRequest>
            {
                new CadastrarInsumoOSRequest
                {
                    EstoqueId = Guid.NewGuid(),
                    Quantidade = 5
                },
                new CadastrarInsumoOSRequest
                {
                    EstoqueId = Guid.NewGuid(),
                    Quantidade = 10
                }
            };

            // Crie uma lista simples de InsumoOS para o retorno do mock
            var insumosOS = new List<InsumoOS>
            {
                new InsumoOS { Id = Guid.NewGuid() },
                new InsumoOS { Id = Guid.NewGuid() }
            };

            // Configure o mock para retornar a lista de insumos
            _insumoOSUseCases.CadastrarInsumosUseCaseAsync(
                Arg.Any<Guid>(),
                Arg.Any<List<CadastrarInsumoOSUseCaseDto>>())
                .Returns(insumosOS);

            // Crie uma lista simples de InsumoOSResponse para o retorno do presenter
            var responses = new List<InsumoOSResponse>
            {
                new InsumoOSResponse(),
                new InsumoOSResponse()
            };

            // Configure o presenter para retornar as respostas
            _insumoPresenter.ToResponse(Arg.Any<IEnumerable<InsumoOS>>()).Returns(responses);

            // Act
            var result = await _insumoOSController.CadastrarInsumos(ordemServicoId, requests);

            // Assert
            // Verifique se o método do usecase foi chamado com os parâmetros corretos
            await _insumoOSUseCases.Received(1).CadastrarInsumosUseCaseAsync(
                Arg.Is<Guid>(g => g == ordemServicoId),
                Arg.Any<List<CadastrarInsumoOSUseCaseDto>>());

            // Verifique se o presenter foi chamado com a lista de insumos
            _insumoPresenter.Received(1).ToResponse(Arg.Any<IEnumerable<InsumoOS>>());

            // Verifique se o resultado é o mesmo que o retornado pelo presenter
            result.Should().BeSameAs(responses);
        }

        [Fact]
        public async Task DevolverInsumosAoEstoque_DeveChamarUseCase()
        {
            // Arrange
            var insumosOS = new List<DevolverInsumoOSRequest>
            {
                new DevolverInsumoOSRequest { EstoqueId = Guid.NewGuid(), Quantidade = 1 },
                new DevolverInsumoOSRequest { EstoqueId = Guid.NewGuid(), Quantidade = 2 }
            };

            // Act
            await _insumoOSController.DevolverInsumosAoEstoque(insumosOS);

            // Assert
            await _insumoOSUseCases.Received(1).DevolverInsumosAoEstoqueUseCaseAsync(
                Arg.Is<IEnumerable<DevolverInsumoOSRequest>>(i => i == insumosOS));
        }
    }
}
