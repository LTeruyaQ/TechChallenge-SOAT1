using FluentAssertions;
using Infraestrutura.Servicos;
using Microsoft.Extensions.Configuration;

namespace MecanicaOSTests.Infraestrutura.Servicos
{
    public class ServicoEmailTests
    {
        [Fact]
        public void Construtor_DeveInicializarCorretamente()
        {
            // Arrange
            var inMemorySettings = new Dictionary<string, string> {
                {"SendGrid:ApiKey", "dummy_key"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            // Act
            var servico = new ServicoEmail(configuration);

            // Assert
            servico.Should().NotBeNull();
        }
    }
}
