namespace MecanicaOSTests.Dominio.Exceptions
{
    public class ExceptionsTests
    {
        [Fact]
        public void CredenciaisInvalidasException_ParameterlessConstructor_ShouldHaveDefaultMessage()
        {
            // Act
            var exception = new CredenciaisInvalidasException();

            // Assert
            Assert.Equal("Credenciais inválidas", exception.Message);
        }

        [Fact]
        public void DadosJaCadastradosException_ParameterlessConstructor_ShouldCreateInstance()
        {
            // Act
            var exception = new DadosJaCadastradosException();

            // Assert
            Assert.NotNull(exception);
            Assert.Null(exception.InnerException);
            Assert.Equal("Exception of type 'Dominio.Exceptions.DadosJaCadastradosException' was thrown.", exception.Message);
        }

        [Fact]
        public void DadosJaCadastradosException_ConstructorWithInnerException_ShouldSetInnerException()
        {
            // Arrange
            var innerException = new Exception("Inner");
            var message = "Test message";

            // Act
            var exception = new DadosJaCadastradosException(message, innerException);

            // Assert
            Assert.Equal(message, exception.Message);
            Assert.Equal(innerException, exception.InnerException);
        }

        [Fact]
        public void UsuarioInativoException_ParameterlessConstructor_ShouldHaveDefaultMessage()
        {
            // Act
            var exception = new UsuarioInativoException();

            // Assert
            Assert.Equal("Usuário inativo", exception.Message);
        }

        [Fact]
        public void InsumoApagadoException_Constructor_ShouldSetMessage()
        {
            // Arrange
            var message = "Test message";

            // Act
            var exception = new InsumoApagadoException(message);

            // Assert
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public void ServicoIndisponivelException_Constructor_ShouldSetMessage()
        {
            // Arrange
            var message = "Test message";

            // Act
            var exception = new ServicoIndisponivelException(message);

            // Assert
            Assert.Equal(message, exception.Message);
        }
    }
}
