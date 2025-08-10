using FluentAssertions;
using Infraestrutura.Autenticacao;
using Xunit;

namespace MecanicaOSTests.Infraestrutura.Autenticacao
{
    public class ServicoSenhaTests
    {
        private readonly ServicoSenha _servicoSenha;

        public ServicoSenhaTests()
        {
            _servicoSenha = new ServicoSenha();
        }

        [Fact]
        public void CriptografarSenha_DeveRetornarHashDiferenteDaSenha()
        {
            // Arrange
            var senha = "minhaSenhaSuperSecreta";

            // Act
            var hash = _servicoSenha.CriptografarSenha(senha);

            // Assert
            hash.Should().NotBeNullOrEmpty();
            hash.Should().NotBe(senha);
        }

        [Fact]
        public void VerificarSenha_DeveRetornarTrue_ParaSenhaCorreta()
        {
            // Arrange
            var senha = "minhaSenhaSuperSecreta";
            var hash = _servicoSenha.CriptografarSenha(senha);

            // Act
            var resultado = _servicoSenha.VerificarSenha(senha, hash);

            // Assert
            resultado.Should().BeTrue();
        }

        [Fact]
        public void VerificarSenha_DeveRetornarFalse_ParaSenhaIncorreta()
        {
            // Arrange
            var senhaCorreta = "minhaSenhaSuperSecreta";
            var senhaIncorreta = "senhaErrada";
            var hash = _servicoSenha.CriptografarSenha(senhaCorreta);

            // Act
            var resultado = _servicoSenha.VerificarSenha(senhaIncorreta, hash);

            // Assert
            resultado.Should().BeFalse();
        }
    }
}
