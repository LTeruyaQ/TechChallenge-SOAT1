using Adapters.Controllers;
using Core.DTOs.Requests.Autenticacao;
using Core.DTOs.Responses.Autenticacao;
using Core.DTOs.UseCases.Autenticacao;
using Core.Entidades;
using Core.Interfaces.Presenters;
using Core.Interfaces.root;
using Core.Interfaces.UseCases;

namespace MecanicaOS.UnitTests.Adapters.Controllers
{
    public class AutenticacaoControllerTests
    {
        private readonly IAutenticacaoUseCases _autenticacaoUseCases;
        private readonly IUsuarioUseCases _usuarioUseCases;
        private readonly IAutenticacaoPresenter _autenticacaoPresenter;
        private readonly AutenticacaoController _autenticacaoController;
        private readonly ICompositionRoot _compositionRoot;

        public AutenticacaoControllerTests()
        {
            _autenticacaoUseCases = Substitute.For<IAutenticacaoUseCases>();
            _usuarioUseCases = Substitute.For<IUsuarioUseCases>();
            _autenticacaoPresenter = Substitute.For<IAutenticacaoPresenter>();
            _compositionRoot = Substitute.For<ICompositionRoot>();

            _compositionRoot.CriarAutenticacaoUseCases().Returns(_autenticacaoUseCases);
            _compositionRoot.CriarUsuarioUseCases().Returns(_usuarioUseCases);
            _autenticacaoController = new AutenticacaoController(_compositionRoot);

            // Usar reflexão para injetar o presenter mockado
            var presenterField = typeof(AutenticacaoController).GetField("_autenticacaoPresenter",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            presenterField?.SetValue(_autenticacaoController, _autenticacaoPresenter);
            
            // Injetar usuarioUseCases mockado
            var usuarioUseCasesField = typeof(AutenticacaoController).GetField("_usuarioUseCases",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            usuarioUseCasesField?.SetValue(_autenticacaoController, _usuarioUseCases);
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

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Email = "teste@email.com",
                Senha = "senha_hash"
            };
            
            // Act
            var result = _autenticacaoController.MapearParaAutenticacaoUseCaseDto(request, usuario);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(request.Email);
            result.Senha.Should().Be(request.Senha);
            result.UsuarioExistente.Should().Be(usuario);
        }

        [Fact]
        public void MapearParaAutenticacaoUseCaseDto_ComRequestNulo_DeveRetornarNulo()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Email = "teste@email.com",
                Senha = "senha_hash"
            };
            
            // Act
            var result = _autenticacaoController.MapearParaAutenticacaoUseCaseDto(null, usuario);

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

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Email = "teste@email.com",
                Senha = "senha_hash"
            };

            var autenticacaoDto = new AutenticacaoDto();
            var autenticacaoResponse = new AutenticacaoResponse();

            _usuarioUseCases.ObterPorEmailUseCaseAsync(request.Email).Returns(usuario);
            _autenticacaoUseCases.AutenticarUseCaseAsync(Arg.Any<AutenticacaoUseCaseDto>())
                .Returns(autenticacaoDto);

            _autenticacaoPresenter.ParaResponse(autenticacaoDto)
                .Returns(autenticacaoResponse);

            // Act
            var result = await _autenticacaoController.AutenticarAsync(request);

            // Assert
            await _usuarioUseCases.Received(1).ObterPorEmailUseCaseAsync(request.Email);
            await _autenticacaoUseCases.Received(1).AutenticarUseCaseAsync(Arg.Is<AutenticacaoUseCaseDto>(
                dto => dto.Email == request.Email && dto.Senha == request.Senha && dto.UsuarioExistente == usuario));

            _autenticacaoPresenter.Received(1).ParaResponse(autenticacaoDto);

            result.Should().Be(autenticacaoResponse);
        }
    }
}
