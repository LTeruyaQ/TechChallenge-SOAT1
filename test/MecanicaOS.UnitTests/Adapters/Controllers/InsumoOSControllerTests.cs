using Adapters.Controllers;
using Adapters.DTOs.Requests.OrdemServico.InsumoOS;
using Adapters.Presenters.Interfaces;
using Core.DTOs.UseCases.OrdemServico.InsumoOS;
using Core.Entidades;
using Core.Interfaces.UseCases;

namespace MecanicaOS.UnitTests.Adapters.Controllers
{
    public class InsumoOSControllerTests
    {
        private readonly IInsumoOSUseCases _insumoOSUseCases;
        private readonly IOrdemServicoPresenter _ordemServicoPresenter;
        private readonly InsumoOSController _insumoOSController;

        public InsumoOSControllerTests()
        {
            _insumoOSUseCases = Substitute.For<IInsumoOSUseCases>();
            _ordemServicoPresenter = Substitute.For<IOrdemServicoPresenter>();
            _insumoOSController = new InsumoOSController(_insumoOSUseCases, _ordemServicoPresenter);
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

            var insumosOS = new List<InsumoOS>
            {
                new InsumoOS(),
                new InsumoOS()
            };

            _insumoOSUseCases.CadastrarInsumosUseCaseAsync(
                Arg.Any<Guid>(),
                Arg.Any<List<CadastrarInsumoOSUseCaseDto>>())
                .Returns(insumosOS);

            // Act
            var result = await _insumoOSController.CadastrarInsumos(ordemServicoId, requests);

            // Assert
            await _insumoOSUseCases.Received(1).CadastrarInsumosUseCaseAsync(
                Arg.Is<Guid>(g => g == ordemServicoId),
                Arg.Is<List<CadastrarInsumoOSUseCaseDto>>(dtos =>
                    dtos.Count == requests.Count &&
                    dtos.All(dto =>
                        requests.Any(r =>
                            r.EstoqueId == dto.EstoqueId &&
                            r.Quantidade == dto.Quantidade))));

            result.Should().BeEquivalentTo(insumosOS);
        }

        [Fact]
        public async Task DevolverInsumosAoEstoque_DeveChamarUseCase()
        {
            // Arrange
            var insumosOS = new List<InsumoOS>
            {
                new InsumoOS(),
                new InsumoOS()
            };

            // Act
            await _insumoOSController.DevolverInsumosAoEstoque(insumosOS);

            // Assert
            await _insumoOSUseCases.Received(1).DevolverInsumosAoEstoqueUseCaseAsync(
                Arg.Is<IEnumerable<InsumoOS>>(i => i == insumosOS));
        }
    }
}
