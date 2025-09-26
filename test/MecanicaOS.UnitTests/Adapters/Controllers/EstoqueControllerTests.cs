using Adapters.Controllers;
using Core.DTOs.Requests.Estoque;
using Core.DTOs.UseCases.Estoque;
using Core.Entidades;
using Core.Interfaces.Presenters;
using Core.Interfaces.root;
using Core.Interfaces.UseCases;

namespace MecanicaOS.UnitTests.Adapters.Controllers
{
    public class EstoqueControllerTests
    {
        private readonly IEstoqueUseCases _estoqueUseCases;
        private readonly IEstoquePresenter _estoquePresenter;
        private readonly EstoqueController _estoqueController;
        private readonly ICompositionRoot _compositionRoot;

        public EstoqueControllerTests()
        {
            _estoqueUseCases = Substitute.For<IEstoqueUseCases>();
            _estoquePresenter = Substitute.For<IEstoquePresenter>();
            _compositionRoot = Substitute.For<ICompositionRoot>();

            _compositionRoot.CriarEstoqueUseCases().Returns(_estoqueUseCases);
            _estoqueController = new EstoqueController(_compositionRoot);

            // Usar reflexão para injetar o presenter mockado
            var presenterField = typeof(EstoqueController).GetField("_estoquePresenter",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            presenterField?.SetValue(_estoqueController, _estoquePresenter);
        }

        [Fact]
        public void MapearParaCadastrarEstoqueUseCaseDto_ComRequestValido_DeveMapearCorretamente()
        {
            // Arrange
            var request = new CadastrarEstoqueRequest
            {
                Insumo = "Insumo Teste",
                Descricao = "Descrição do Insumo Teste",
                Preco = 50.75,
                QuantidadeDisponivel = 100,
                QuantidadeMinima = 10
            };

            // Act
            var result = _estoqueController.MapearParaCadastrarEstoqueUseCaseDto(request);

            // Assert
            result.Should().NotBeNull();
            result.Insumo.Should().Be(request.Insumo);
            result.Descricao.Should().Be(request.Descricao);
            result.Preco.Should().Be(Convert.ToDecimal(request.Preco));
            result.QuantidadeDisponivel.Should().Be(request.QuantidadeDisponivel);
            result.QuantidadeMinima.Should().Be(request.QuantidadeMinima);
        }

        [Fact]
        public void MapearParaCadastrarEstoqueUseCaseDto_ComRequestNulo_DeveRetornarNulo()
        {
            // Act
            var result = _estoqueController.MapearParaCadastrarEstoqueUseCaseDto(null);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void MapearParaAtualizarEstoqueUseCaseDto_ComRequestValido_DeveMapearCorretamente()
        {
            // Arrange
            var request = new AtualizarEstoqueRequest
            {
                Insumo = "Insumo Atualizado",
                Descricao = "Descrição do Insumo Atualizado",
                Preco = 75.50m,
                QuantidadeDisponivel = 150,
                QuantidadeMinima = 15
            };

            // Act
            var result = _estoqueController.MapearParaAtualizarEstoqueUseCaseDto(request);

            // Assert
            result.Should().NotBeNull();
            result.Insumo.Should().Be(request.Insumo);
            result.Descricao.Should().Be(request.Descricao);
            result.Preco.Should().Be(request.Preco);
            result.QuantidadeDisponivel.Should().Be(request.QuantidadeDisponivel);
            result.QuantidadeMinima.Should().Be(request.QuantidadeMinima);
        }

        [Fact]
        public void MapearParaAtualizarEstoqueUseCaseDto_ComRequestNulo_DeveRetornarNulo()
        {
            // Act
            var result = _estoqueController.MapearParaAtualizarEstoqueUseCaseDto(null);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Cadastrar_DeveUsarMapeamentoEChamarUseCase()
        {
            // Arrange
            var request = new CadastrarEstoqueRequest
            {
                Insumo = "Insumo Teste",
                Descricao = "Descrição do Insumo Teste",
                Preco = 50.75,
                QuantidadeDisponivel = 100,
                QuantidadeMinima = 10
            };

            var estoque = new Estoque();
            _estoqueUseCases.CadastrarUseCaseAsync(Arg.Any<CadastrarEstoqueUseCaseDto>())
                .Returns(estoque);

            // Act
            await _estoqueController.Cadastrar(request);

            // Assert
            await _estoqueUseCases.Received(1).CadastrarUseCaseAsync(Arg.Is<CadastrarEstoqueUseCaseDto>(
                dto => dto.Insumo == request.Insumo &&
                      dto.Descricao == request.Descricao &&
                      dto.QuantidadeDisponivel == request.QuantidadeDisponivel));

            _estoquePresenter.Received(1).ParaResponse(estoque);
        }

        [Fact]
        public async Task Atualizar_DeveUsarMapeamentoEChamarUseCase()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new AtualizarEstoqueRequest
            {
                Insumo = "Insumo Atualizado",
                Descricao = "Descrição do Insumo Atualizado",
                Preco = 75.50m,
                QuantidadeDisponivel = 150,
                QuantidadeMinima = 15
            };

            var estoque = new Estoque();
            _estoqueUseCases.AtualizarUseCaseAsync(id, Arg.Any<AtualizarEstoqueUseCaseDto>())
                .Returns(estoque);

            // Act
            await _estoqueController.Atualizar(id, request);

            // Assert
            await _estoqueUseCases.Received(1).AtualizarUseCaseAsync(
                Arg.Is<Guid>(g => g == id),
                Arg.Is<AtualizarEstoqueUseCaseDto>(
                    dto => dto.Insumo == request.Insumo &&
                          dto.Descricao == request.Descricao &&
                          dto.Preco == request.Preco));

            _estoquePresenter.Received(1).ParaResponse(estoque);
        }
    }
}
