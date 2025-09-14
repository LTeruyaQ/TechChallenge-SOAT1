using FluentAssertions;
using Infraestrutura.Autenticacao;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace MecanicaOSTests.Infraestrutura.Autenticacao
{
    public class UsuarioLogadoServicoTests
    {
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<IRepositorio<Usuario>> _usuarioRepositorioMock;

        public UsuarioLogadoServicoTests()
        {
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _usuarioRepositorioMock = new Mock<IRepositorio<Usuario>>();
        }

        private UsuarioLogadoServico CriarServicoComUsuario(ClaimsPrincipal user)
        {
            var httpContext = new DefaultHttpContext { User = user };
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
            return new UsuarioLogadoServico(_httpContextAccessorMock.Object, _usuarioRepositorioMock.Object);
        }

        [Fact]
        public void Propriedades_DevemRetornarNuloOuFalse_QuandoUsuarioNaoAutenticado()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity()); // Not authenticated
            var servico = CriarServicoComUsuario(user);

            // Act & Assert
            servico.UsuarioId.Should().BeNull();
            servico.Email.Should().BeNull();
            servico.Nome.Should().BeNull();
            servico.TipoUsuario.Should().BeNull();
            servico.EstaAutenticado.Should().BeFalse();
            servico.EstaNaRole("Admin").Should().BeFalse();
            servico.PossuiPermissao("editar").Should().BeFalse();
            servico.ObterTodasClaims().Should().BeEmpty();
            servico.ObterUsuarioLogado().Should().BeNull();
        }

        [Fact]
        public void Propriedades_DevemRetornarValoresCorretos_QuandoUsuarioAutenticado()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString()),
                new Claim(ClaimTypes.Email, "teste@teste.com"),
                new Claim(ClaimTypes.Name, "Usuario Teste"),
                new Claim("tipo_usuario", "Admin"),
                new Claim(ClaimTypes.Role, "AdminRole"),
                new Claim("permissao", "ver")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);
            var servico = CriarServicoComUsuario(user);

            // Act & Assert
            servico.UsuarioId.Should().Be(usuarioId);
            servico.Email.Should().Be("teste@teste.com");
            servico.Nome.Should().Be("Usuario Teste");
            servico.TipoUsuario.Should().Be(TipoUsuario.Admin);
            servico.EstaAutenticado.Should().BeTrue();
            servico.EstaNaRole("AdminRole").Should().BeTrue();
            servico.PossuiPermissao("ver").Should().BeTrue();
            servico.ObterTodasClaims().Should().HaveCount(claims.Count);
        }

        [Fact]
        public void ObterUsuarioLogado_DeveRetornarUsuarioDoRepositorio()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);
            var servico = CriarServicoComUsuario(user);

            var usuarioDoRepo = new Usuario { Id = usuarioId, Email = "repo@teste.com" };
            _usuarioRepositorioMock.Setup(r => r.ObterPorIdAsync(usuarioId)).ReturnsAsync(usuarioDoRepo);

            // Act
            var resultado = servico.ObterUsuarioLogado();

            // Assert
            resultado.Should().Be(usuarioDoRepo);
        }
    }
}
