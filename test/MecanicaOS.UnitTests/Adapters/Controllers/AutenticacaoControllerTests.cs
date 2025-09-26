using Adapters.Controllers;
using Core.DTOs.Requests.Autenticacao;
using Core.DTOs.Responses.Autenticacao;
using Core.DTOs.UseCases.Autenticacao;
using Core.Interfaces.Presenters;
using Core.Interfaces.root;
using Core.Interfaces.UseCases;

namespace MecanicaOS.UnitTests.Adapters.Controllers
{
    public class AutenticacaoControllerTests
    {
        private readonly IAutenticacaoUseCases _autenticacaoUseCases;
        private readonly IAutenticacaoPresenter _autenticacaoPresenter;
        private readonly AutenticacaoController _autenticacaoController;
        private readonly ICompositionRoot _compositionRoot;

        public AutenticacaoControllerTests()
        {
            _autenticacaoUseCases = Substitute.For<IAutenticacaoUseCases>();
            _autenticacaoPresenter = Substitute.For<IAutenticacaoPresenter>();
            _compositionRoot = Substitute.For<ICompositionRoot>();

            _compositionRoot.CriarAutenticacaoUseCases().Returns(_autenticacaoUseCases);
            _autenticacaoController = new AutenticacaoController(_compositionRoot);

            // Usar reflexão para injetar o presenter mockado
            var presenterField = typeof(AutenticacaoController).GetField("_autenticacaoPresenter",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            presenterField?.SetValue(_autenticacaoController, _autenticacaoPresenter);
        }

        [Fact]
        public void MapearParaAutenticacaoUseCaseDto_ComRequestValido_DeveMapearCorretamente()
        {
            // Arrange
            var request = new AutenticacaoRequest
            {
                Email = "teste@email.com",
                Senha = "senha123"
            };

            // Act
            var result = _autenticacaoController.MapearParaAutenticacaoUseCaseDto(request);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(request.Email);
            result.Senha.Should().Be(request.Senha);
        }

        [Fact]
        public void MapearParaAutenticacaoUseCaseDto_ComRequestNulo_DeveRetornarNulo()
        {
            // Act
            var result = _autenticacaoController.MapearParaAutenticacaoUseCaseDto(null);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task AutenticarAsync_DeveUsarMapeamentoEChamarUseCase()
        {
            // Arrange
            var request = new AutenticacaoRequest
            {
                Email = "teste@email.com",
                Senha = "senha123"
            };

            var autenticacaoDto = new AutenticacaoDto();
            var autenticacaoResponse = new AutenticacaoResponse();

            _autenticacaoUseCases.AutenticarUseCaseAsync(Arg.Any<AutenticacaoUseCaseDto>())
                .Returns(autenticacaoDto);

            _autenticacaoPresenter.ParaResponse(autenticacaoDto)
                .Returns(autenticacaoResponse);

            // Act
            var result = await _autenticacaoController.AutenticarAsync(request);

            // Assert
            await _autenticacaoUseCases.Received(1).AutenticarUseCaseAsync(Arg.Is<AutenticacaoUseCaseDto>(
                dto => dto.Email == request.Email && dto.Senha == request.Senha));

            _autenticacaoPresenter.Received(1).ParaResponse(autenticacaoDto);

            result.Should().Be(autenticacaoResponse);
        }
    }
}
