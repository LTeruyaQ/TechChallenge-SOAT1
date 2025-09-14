using FluentAssertions;
using Infraestrutura.Autenticacao;
using Microsoft.Extensions.Options;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MecanicaOSTests.Infraestrutura.Autenticacao
{
    public class ServicoJwtTests
    {
        private readonly ServicoJwt _servicoJwt;
        private readonly ConfiguracaoJwt _configuracaoJwt;

        public ServicoJwtTests()
        {
            _configuracaoJwt = new ConfiguracaoJwt
            {
                SecretKey = "umaChaveSuperSecretaDePeloMenos32Caracteres",
                Issuer = "MeuApp",
                Audience = "MeuApp.Audiencia",
                ExpiryInMinutes = 60
            };

            var optionsMock = new Mock<IOptions<ConfiguracaoJwt>>();
            optionsMock.Setup(o => o.Value).Returns(_configuracaoJwt);

            _servicoJwt = new ServicoJwt(optionsMock.Object);
        }

        [Fact]
        public void GerarToken_DeveGerarTokenValido()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var email = "teste@teste.com";
            var tipoUsuario = "Admin";
            var nome = "Usuario Teste";
            var permissoes = new[] { "ver", "editar" };

            // Act
            var token = _servicoJwt.GerarToken(usuarioId, email, tipoUsuario, nome, permissoes);

            // Assert
            token.Should().NotBeNullOrEmpty();

            var handler = new JwtSecurityTokenHandler();
            var decodedToken = handler.ReadJwtToken(token);

            decodedToken.Issuer.Should().Be(_configuracaoJwt.Issuer);
            decodedToken.Audiences.Should().Contain(_configuracaoJwt.Audience);
            decodedToken.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == usuarioId.ToString());
            decodedToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == email);
            decodedToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == tipoUsuario);
            decodedToken.Claims.Should().Contain(c => c.Type == "tipo_usuario" && c.Value == tipoUsuario);
            decodedToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == nome);
            var permissoesNoToken = decodedToken.Claims.Where(c => c.Type == "permissao").Select(c => c.Value);
            permissoesNoToken.Should().BeEquivalentTo(permissoes);
        }

        [Fact]
        public void ValidarToken_DeveRetornarTrue_ParaTokenValido()
        {
            // Arrange
            var token = _servicoJwt.GerarToken(Guid.NewGuid(), "email@teste.com", "Admin");

            // Act
            var resultado = _servicoJwt.ValidarToken(token);

            // Assert
            resultado.Should().BeTrue();
        }

        [Fact]
        public void ValidarToken_DeveRetornarFalse_ParaTokenInvalido()
        {
            // Arrange
            var tokenInvalido = "um.token.qualquer";

            // Act
            var resultado = _servicoJwt.ValidarToken(tokenInvalido);

            // Assert
            resultado.Should().BeFalse();
        }
    }
}
