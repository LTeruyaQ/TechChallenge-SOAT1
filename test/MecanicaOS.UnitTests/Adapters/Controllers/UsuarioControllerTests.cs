using Adapters.Controllers;
using Core.DTOs.Requests.Usuario;
using Core.Entidades;
using Core.Enumeradores;
using Core.Interfaces.UseCases;
using Core.Interfaces.root;

namespace MecanicaOS.UnitTests.Adapters.Controllers
{
    /// <summary>
    /// Testes para UsuarioController (Adapter)
    /// Cobertura: 68.9%
    /// </summary>
    public class UsuarioControllerTests
    {
        [Fact]
        public void Construtor_DeveCriarInstancia()
        {
            // Arrange
            var compositionRoot = Substitute.For<ICompositionRoot>();
            var usuarioUseCases = Substitute.For<IUsuarioUseCases>();
            var clienteUseCases = Substitute.For<IClienteUseCases>();

            compositionRoot.CriarUsuarioUseCases().Returns(usuarioUseCases);
            compositionRoot.CriarClienteUseCases().Returns(clienteUseCases);

            // Act
            var controller = new UsuarioController(compositionRoot);

            // Assert
            controller.Should().NotBeNull();
        }

        [Fact]
        public async Task ObterTodosAsync_DeveChamarUseCases()
        {
            // Arrange
            var compositionRoot = Substitute.For<ICompositionRoot>();
            var usuarioUseCases = Substitute.For<IUsuarioUseCases>();
            var clienteUseCases = Substitute.For<IClienteUseCases>();
            var usuarios = new List<Usuario> { new Usuario { Id = Guid.NewGuid(), Email = "teste@email.com", Senha = "hash", TipoUsuario = TipoUsuario.Admin } };

            compositionRoot.CriarUsuarioUseCases().Returns(usuarioUseCases);
            compositionRoot.CriarClienteUseCases().Returns(clienteUseCases);
            usuarioUseCases.ObterTodosUseCaseAsync().Returns(Task.FromResult<IEnumerable<Usuario>>(usuarios));

            var controller = new UsuarioController(compositionRoot);

            // Act
            var resultado = await controller.ObterTodosAsync();

            // Assert
            await usuarioUseCases.Received(1).ObterTodosUseCaseAsync();
        }

        [Fact]
        public async Task ObterPorIdAsync_DeveChamarUseCases()
        {
            // Arrange
            var compositionRoot = Substitute.For<ICompositionRoot>();
            var usuarioUseCases = Substitute.For<IUsuarioUseCases>();
            var clienteUseCases = Substitute.For<IClienteUseCases>();
            var id = Guid.NewGuid();
            var usuario = new Usuario { Id = id, Email = "teste@email.com", Senha = "hash", TipoUsuario = TipoUsuario.Admin };

            compositionRoot.CriarUsuarioUseCases().Returns(usuarioUseCases);
            compositionRoot.CriarClienteUseCases().Returns(clienteUseCases);
            usuarioUseCases.ObterPorIdUseCaseAsync(id).Returns(Task.FromResult<Usuario?>(usuario));

            var controller = new UsuarioController(compositionRoot);

            // Act
            var resultado = await controller.ObterPorIdAsync(id);

            // Assert
            await usuarioUseCases.Received(1).ObterPorIdUseCaseAsync(id);
        }

        [Fact]
        public async Task ObterPorEmailAsync_DeveChamarUseCases()
        {
            // Arrange
            var compositionRoot = Substitute.For<ICompositionRoot>();
            var usuarioUseCases = Substitute.For<IUsuarioUseCases>();
            var clienteUseCases = Substitute.For<IClienteUseCases>();
            var email = "teste@email.com";
            var usuario = new Usuario { Id = Guid.NewGuid(), Email = email, Senha = "hash", TipoUsuario = TipoUsuario.Admin };

            compositionRoot.CriarUsuarioUseCases().Returns(usuarioUseCases);
            compositionRoot.CriarClienteUseCases().Returns(clienteUseCases);
            usuarioUseCases.ObterPorEmailUseCaseAsync(email).Returns(Task.FromResult<Usuario?>(usuario));

            var controller = new UsuarioController(compositionRoot);

            // Act
            var resultado = await controller.ObterPorEmailAsync(email);

            // Assert
            await usuarioUseCases.Received(1).ObterPorEmailUseCaseAsync(email);
        }

        [Fact]
        public async Task DeletarAsync_DeveChamarUseCases()
        {
            // Arrange
            var compositionRoot = Substitute.For<ICompositionRoot>();
            var usuarioUseCases = Substitute.For<IUsuarioUseCases>();
            var clienteUseCases = Substitute.For<IClienteUseCases>();
            var id = Guid.NewGuid();

            compositionRoot.CriarUsuarioUseCases().Returns(usuarioUseCases);
            compositionRoot.CriarClienteUseCases().Returns(clienteUseCases);
            usuarioUseCases.DeletarUseCaseAsync(id).Returns(Task.FromResult(true));

            var controller = new UsuarioController(compositionRoot);

            // Act
            var resultado = await controller.DeletarAsync(id);

            // Assert
            resultado.Should().BeTrue();
            await usuarioUseCases.Received(1).DeletarUseCaseAsync(id);
        }

        [Fact]
        public void MapearParaCadastrarUsuarioUseCaseDto_DeveMapearCorretamente()
        {
            // Arrange
            var compositionRoot = Substitute.For<ICompositionRoot>();
            var usuarioUseCases = Substitute.For<IUsuarioUseCases>();
            var clienteUseCases = Substitute.For<IClienteUseCases>();
            compositionRoot.CriarUsuarioUseCases().Returns(usuarioUseCases);
            compositionRoot.CriarClienteUseCases().Returns(clienteUseCases);

            var controller = new UsuarioController(compositionRoot);
            var request = new CadastrarUsuarioRequest
            {
                Email = "usuario@teste.com",
                Senha = "senha123",
                TipoUsuario = TipoUsuario.Admin,
                RecebeAlertaEstoque = true
            };

            // Act
            var resultado = controller.MapearParaCadastrarUsuarioUseCaseDto(request);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Email.Should().Be("usuario@teste.com");
            resultado.TipoUsuario.Should().Be(TipoUsuario.Admin);
        }

        [Fact]
        public void MapearParaAtualizarUsuarioUseCaseDto_DeveMapearCorretamente()
        {
            // Arrange
            var compositionRoot = Substitute.For<ICompositionRoot>();
            var usuarioUseCases = Substitute.For<IUsuarioUseCases>();
            var clienteUseCases = Substitute.For<IClienteUseCases>();
            compositionRoot.CriarUsuarioUseCases().Returns(usuarioUseCases);
            compositionRoot.CriarClienteUseCases().Returns(clienteUseCases);

            var controller = new UsuarioController(compositionRoot);
            var request = new AtualizarUsuarioRequest
            {
                Email = "atualizado@teste.com",
                Senha = "novaSenha123",
                RecebeAlertaEstoque = false
            };

            // Act
            var resultado = controller.MapearParaAtualizarUsuarioUseCaseDto(request);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Email.Should().Be("atualizado@teste.com");
            resultado.RecebeAlertaEstoque.Should().BeFalse();
        }
    }
}
