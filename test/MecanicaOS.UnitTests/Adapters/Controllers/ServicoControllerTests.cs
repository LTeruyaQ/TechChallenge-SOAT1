using Adapters.Controllers;
using Core.DTOs.Requests.Servico;
using Core.DTOs.UseCases.Servico;
using Core.Entidades;
using Core.Interfaces.Presenters;
using Core.Interfaces.root;
using Core.Interfaces.UseCases;

namespace MecanicaOS.UnitTests.Adapters.Controllers
{
    public class ServicoControllerTests
    {
        private readonly IServicoUseCases _servicoUseCases;
        private readonly IServicoPresenter _servicoPresenter;
        private readonly ServicoController _servicoController;
        private readonly ICompositionRoot _compositionRoot;

        public ServicoControllerTests()
        {
            _servicoUseCases = Substitute.For<IServicoUseCases>();
            _servicoPresenter = Substitute.For<IServicoPresenter>();
            _compositionRoot = Substitute.For<ICompositionRoot>();

            _compositionRoot.CriarServicoUseCases().Returns(_servicoUseCases);
            _servicoController = new ServicoController(_compositionRoot);

            // Usar reflexão para injetar o presenter mockado
            var presenterField = typeof(ServicoController).GetField("_servicoPresenter",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            presenterField?.SetValue(_servicoController, _servicoPresenter);
        }

        [Fact]
        public void MapearParaCadastrarServicoUseCaseDto_ComRequestValido_DeveMapearCorretamente()
        {
            // Arrange
            var request = new CadastrarServicoRequest
            {
                Nome = "Serviço Teste",
                Descricao = "Descrição do Serviço Teste",
                Valor = 100.50m,
                Disponivel = true
            };

            // Act
            var result = _servicoController.MapearParaCadastrarServicoUseCaseDto(request);

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be(request.Nome);
            result.Descricao.Should().Be(request.Descricao);
            result.Valor.Should().Be(request.Valor);
            result.Disponivel.Should().Be(request.Disponivel);
        }

        [Fact]
        public void MapearParaCadastrarServicoUseCaseDto_ComRequestNulo_DeveRetornarNulo()
        {
            // Act
            var result = _servicoController.MapearParaCadastrarServicoUseCaseDto(null);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void MapearParaEditarServicoUseCaseDto_ComRequestValido_DeveMapearCorretamente()
        {
            // Arrange
            var request = new EditarServicoRequest
            {
                Nome = "Serviço Atualizado",
                Descricao = "Descrição do Serviço Atualizado",
                Valor = 150.75m,
                Disponivel = false
            };

            // Act
            var result = _servicoController.MapearParaEditarServicoUseCaseDto(request);

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be(request.Nome);
            result.Descricao.Should().Be(request.Descricao);
            result.Valor.Should().Be(request.Valor);
            result.Disponivel.Should().Be(request.Disponivel);
        }

        [Fact]
        public void MapearParaEditarServicoUseCaseDto_ComRequestNulo_DeveRetornarNulo()
        {
            // Act
            var result = _servicoController.MapearParaEditarServicoUseCaseDto(null);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Criar_DeveUsarMapeamentoEChamarUseCase()
        {
            // Arrange
            var request = new CadastrarServicoRequest
            {
                Nome = "Serviço Teste",
                Descricao = "Descrição do Serviço Teste",
                Valor = 100.50m,
                Disponivel = true
            };

            var servico = new Servico { Nome = "Serviço Teste", Descricao = "Descrição do Serviço Teste" };
            _servicoUseCases.CadastrarServicoUseCaseAsync(Arg.Any<CadastrarServicoUseCaseDto>())
                .Returns(servico);

            // Act
            await _servicoController.Criar(request);

            // Assert
            await _servicoUseCases.Received(1).CadastrarServicoUseCaseAsync(Arg.Is<CadastrarServicoUseCaseDto>(
                dto => dto.Nome == request.Nome &&
                      dto.Descricao == request.Descricao &&
                      dto.Valor == request.Valor &&
                      dto.Disponivel == request.Disponivel));

            _servicoPresenter.Received(1).ParaResponse(servico);
        }

        [Fact]
        public async Task Atualizar_DeveUsarMapeamentoEChamarUseCase()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new EditarServicoRequest
            {
                Nome = "Serviço Atualizado",
                Descricao = "Descrição do Serviço Atualizado",
                Valor = 150.75m,
                Disponivel = false
            };

            var servico = new Servico { Nome = "Serviço Atualizado", Descricao = "Descrição do Serviço Atualizado" };
            _servicoUseCases.EditarServicoUseCaseAsync(id, Arg.Any<EditarServicoUseCaseDto>())
                .Returns(servico);

            // Act
            await _servicoController.Atualizar(id, request);

            // Assert
            await _servicoUseCases.Received(1).EditarServicoUseCaseAsync(
                Arg.Is<Guid>(g => g == id),
                Arg.Is<EditarServicoUseCaseDto>(
                    dto => dto.Nome == request.Nome &&
                          dto.Descricao == request.Descricao &&
                          dto.Valor == request.Valor &&
                          dto.Disponivel == request.Disponivel));

            _servicoPresenter.Received(1).ParaResponse(servico);
        }
    }
}
