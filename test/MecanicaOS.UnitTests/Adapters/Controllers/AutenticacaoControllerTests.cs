using Adapters.Controllers;
using Adapters.DTOs.Requests.Autenticacao;
using Adapters.DTOs.Responses.Autenticacao;
using Adapters.Presenters.Interfaces;
using Core.DTOs.UseCases.Autenticacao;
using Core.Interfaces.UseCases;

namespace MecanicaOS.UnitTests.Adapters.Controllers
{
    public class AutenticacaoControllerTests
    {
        private readonly IAutenticacaoUseCases _autenticacaoUseCases;
        private readonly IAutenticacaoPresenter _autenticacaoPresenter;
        private readonly AutenticacaoController _autenticacaoController;

        public AutenticacaoControllerTests()
        {
            _autenticacaoUseCases = Substitute.For<IAutenticacaoUseCases>();
            _autenticacaoPresenter = Substitute.For<IAutenticacaoPresenter>();
            _autenticacaoController = new AutenticacaoController(_autenticacaoUseCases, _autenticacaoPresenter);
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
