using Adapters.Controllers;
using Core.DTOs.Requests.Usuario;
using Core.DTOs.Responses.Usuario;
using Core.DTOs.UseCases.Usuario;
using Core.Entidades;
using Core.Enumeradores;
using Core.Interfaces.Controllers;
using Core.Interfaces.Presenters;
using Core.Interfaces.root;
using Core.Interfaces.UseCases;
using NSubstitute;
using FluentAssertions;
using Xunit;

namespace MecanicaOS.UnitTests.Adapters.Controllers
{
    public class UsuarioControllerTests
    {
        private readonly IUsuarioUseCases _usuarioUseCases;
        private readonly IUsuarioPresenter _usuarioPresenter;
        private readonly UsuarioController _usuarioController;
        private readonly ICompositionRoot _compositionRoot;

        public UsuarioControllerTests()
        {
            _usuarioUseCases = Substitute.For<IUsuarioUseCases>();
            _usuarioPresenter = Substitute.For<IUsuarioPresenter>();
            _compositionRoot = Substitute.For<ICompositionRoot>();
            
            _compositionRoot.CriarUsuarioUseCases().Returns(_usuarioUseCases);
            _usuarioController = new UsuarioController(_compositionRoot);
            
            // Usar reflexão para injetar o presenter mockado
            var presenterField = typeof(UsuarioController).GetField("_usuarioPresenter", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            presenterField?.SetValue(_usuarioController, _usuarioPresenter);
        }

        [Fact]
        public void MapearParaCadastrarUsuarioUseCaseDto_ComRequestValido_DeveMapearCorretamente()
        {
            // Arrange
            var request = new CadastrarUsuarioRequest
            {
                Email = "usuario@email.com",
                Senha = "senha123",
                TipoUsuario = TipoUsuario.Admin,
                RecebeAlertaEstoque = true,
                Documento = "12345678900"
            };

            // Act
            var result = _usuarioController.MapearParaCadastrarUsuarioUseCaseDto(request);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(request.Email);
            result.Senha.Should().Be(request.Senha);
            result.TipoUsuario.Should().Be(request.TipoUsuario);
            result.RecebeAlertaEstoque.Should().Be(request.RecebeAlertaEstoque);
            result.Documento.Should().Be(request.Documento);
        }

        [Fact]
        public void MapearParaCadastrarUsuarioUseCaseDto_ComRequestNulo_DeveRetornarNulo()
        {
            // Act
            var result = _usuarioController.MapearParaCadastrarUsuarioUseCaseDto(null);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void MapearParaAtualizarUsuarioUseCaseDto_ComRequestValido_DeveMapearCorretamente()
        {
            // Arrange
            var request = new AtualizarUsuarioRequest
            {
                Email = "atualizado@email.com",
                Senha = "novaSenha456",
                DataUltimoAcesso = DateTime.Now,
                TipoUsuario = TipoUsuario.Cliente,
                RecebeAlertaEstoque = false
            };

            // Act
            var result = _usuarioController.MapearParaAtualizarUsuarioUseCaseDto(request);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(request.Email);
            result.Senha.Should().Be(request.Senha);
            result.DataUltimoAcesso.Should().Be(request.DataUltimoAcesso);
            result.TipoUsuario.Should().Be(request.TipoUsuario);
            result.RecebeAlertaEstoque.Should().Be(request.RecebeAlertaEstoque);
        }

        [Fact]
        public void MapearParaAtualizarUsuarioUseCaseDto_ComRequestNulo_DeveRetornarNulo()
        {
            // Act
            var result = _usuarioController.MapearParaAtualizarUsuarioUseCaseDto(null);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CadastrarAsync_DeveUsarMapeamentoEChamarUseCase()
        {
            // Arrange
            var request = new CadastrarUsuarioRequest
            {
                Email = "usuario@email.com",
                Senha = "senha123",
                TipoUsuario = TipoUsuario.Admin,
                RecebeAlertaEstoque = true,
                Documento = "12345678900"
            };

            var usuario = new Usuario();
            var usuarioResponse = new UsuarioResponse();

            _usuarioUseCases.CadastrarUseCaseAsync(Arg.Any<CadastrarUsuarioUseCaseDto>())
                .Returns(usuario);

            _usuarioPresenter.ParaResponse(usuario)
                .Returns(usuarioResponse);

            // Act
            var result = await _usuarioController.CadastrarAsync(request);

            // Assert
            await _usuarioUseCases.Received(1).CadastrarUseCaseAsync(Arg.Is<CadastrarUsuarioUseCaseDto>(
                dto => dto.Email == request.Email &&
                      dto.Senha == request.Senha &&
                      dto.TipoUsuario == request.TipoUsuario &&
                      dto.RecebeAlertaEstoque == request.RecebeAlertaEstoque &&
                      dto.Documento == request.Documento));

            _usuarioPresenter.Received(1).ParaResponse(usuario);

            result.Should().Be(usuarioResponse);
        }

        [Fact]
        public async Task AtualizarAsync_DeveUsarMapeamentoEChamarUseCase()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new AtualizarUsuarioRequest
            {
                Email = "atualizado@email.com",
                Senha = "novaSenha456",
                DataUltimoAcesso = DateTime.Now,
                TipoUsuario = TipoUsuario.Cliente,
                RecebeAlertaEstoque = false
            };

            var usuario = new Usuario();
            var usuarioResponse = new UsuarioResponse();

            _usuarioUseCases.AtualizarUseCaseAsync(id, Arg.Any<AtualizarUsuarioUseCaseDto>())
                .Returns(usuario);

            _usuarioPresenter.ParaResponse(usuario)
                .Returns(usuarioResponse);

            // Act
            var result = await _usuarioController.AtualizarAsync(id, request);

            // Assert
            await _usuarioUseCases.Received(1).AtualizarUseCaseAsync(
                Arg.Is<Guid>(g => g == id),
                Arg.Is<AtualizarUsuarioUseCaseDto>(
                    dto => dto.Email == request.Email &&
                          dto.Senha == request.Senha &&
                          dto.DataUltimoAcesso == request.DataUltimoAcesso &&
                          dto.TipoUsuario == request.TipoUsuario &&
                          dto.RecebeAlertaEstoque == request.RecebeAlertaEstoque));

            _usuarioPresenter.Received(1).ParaResponse(usuario);

            result.Should().Be(usuarioResponse);
        }
    }
}
