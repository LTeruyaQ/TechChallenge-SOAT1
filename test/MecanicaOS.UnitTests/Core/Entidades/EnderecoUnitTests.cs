using Core.Entidades;
using Core.Entidades.Abstratos;

namespace MecanicaOS.UnitTests.Core.Entidades;

public class EnderecoUnitTests
{
    [Fact]
    public void Endereco_QuandoCriado_DeveInicializarComValoresPadrao()
    {
        // Arrange & Act
        var endereco = new Endereco();

        // Assert
        endereco.Should().BeAssignableTo<Entidade>("Endereco deve herdar de Entidade");
        endereco.Id.Should().NotBeEmpty("Id deve ser gerado automaticamente");
        endereco.Ativo.Should().BeTrue("Endereco deve estar ativo por padrão");
        endereco.DataCadastro.Should().NotBe(default(DateTime), "DataCadastro deve ser definida");
        endereco.DataAtualizacao.Should().NotBe(default(DateTime), "DataAtualizacao deve ser definida");
    }

    [Fact]
    public void Endereco_QuandoDefinidaRua_DeveArmazenarCorretamente()
    {
        // Arrange
        var endereco = new Endereco();
        var ruaEsperada = "Rua das Flores, 123";

        // Act
        endereco.Rua = ruaEsperada;

        // Assert
        endereco.Rua.Should().Be(ruaEsperada, "a rua deve ser armazenada corretamente");
    }

    [Fact]
    public void Endereco_QuandoDefinidoBairro_DeveArmazenarCorretamente()
    {
        // Arrange
        var endereco = new Endereco();
        var bairroEsperado = "Centro";

        // Act
        endereco.Bairro = bairroEsperado;

        // Assert
        endereco.Bairro.Should().Be(bairroEsperado, "o bairro deve ser armazenado corretamente");
    }

    [Fact]
    public void Endereco_QuandoDefinidaCidade_DeveArmazenarCorretamente()
    {
        // Arrange
        var endereco = new Endereco();
        var cidadeEsperada = "São Paulo";

        // Act
        endereco.Cidade = cidadeEsperada;

        // Assert
        endereco.Cidade.Should().Be(cidadeEsperada, "a cidade deve ser armazenada corretamente");
    }

    [Fact]
    public void Endereco_QuandoDefinidoNumero_DeveArmazenarCorretamente()
    {
        // Arrange
        var endereco = new Endereco();
        var numeroEsperado = "123";

        // Act
        endereco.Numero = numeroEsperado;

        // Assert
        endereco.Numero.Should().Be(numeroEsperado, "o número deve ser armazenado corretamente");
    }

    [Fact]
    public void Endereco_QuandoDefinidoCEP_DeveArmazenarCorretamente()
    {
        // Arrange
        var endereco = new Endereco();
        var cepEsperado = "01234-567";

        // Act
        endereco.CEP = cepEsperado;

        // Assert
        endereco.CEP.Should().Be(cepEsperado, "o CEP deve ser armazenado corretamente");
    }

    [Fact]
    public void Endereco_QuandoDefinidoComplemento_DeveArmazenarCorretamente()
    {
        // Arrange
        var endereco = new Endereco();
        var complementoEsperado = "Apto 101";

        // Act
        endereco.Complemento = complementoEsperado;

        // Assert
        endereco.Complemento.Should().Be(complementoEsperado, "o complemento deve ser armazenado corretamente");
    }

    [Fact]
    public void Endereco_QuandoDefinidoIdCliente_DeveArmazenarCorretamente()
    {
        // Arrange
        var endereco = new Endereco();
        var idClienteEsperado = Guid.NewGuid();

        // Act
        endereco.IdCliente = idClienteEsperado;

        // Assert
        endereco.IdCliente.Should().Be(idClienteEsperado, "o IdCliente deve ser armazenado corretamente");
    }

    [Fact]
    public void Endereco_QuandoDesativado_DeveMarcarComoInativo()
    {
        // Arrange
        var endereco = new Endereco { Ativo = true };

        // Act
        endereco.Desativar();

        // Assert
        endereco.Ativo.Should().BeFalse("o endereço deve estar marcado como inativo");
    }

    [Fact]
    public void Endereco_QuandoAtivado_DeveMarcarComoAtivo()
    {
        // Arrange
        var endereco = new Endereco { Ativo = false };

        // Act
        endereco.Ativar();

        // Assert
        endereco.Ativo.Should().BeTrue("o endereço deve estar marcado como ativo");
    }

    [Fact]
    public void Endereco_QuandoComparadoComOutroEnderecoComMesmoId_DeveSerIgual()
    {
        // Arrange
        var id = Guid.NewGuid();
        var endereco1 = new Endereco { Id = id, Rua = "Rua A" };
        var endereco2 = new Endereco { Id = id, Rua = "Rua B" };

        // Act & Assert
        endereco1.Should().Be(endereco2, "endereços com mesmo Id devem ser considerados iguais");
        endereco1.GetHashCode().Should().Be(endereco2.GetHashCode(), "hash codes devem ser iguais para objetos iguais");
    }

    [Fact]
    public void Endereco_QuandoComparadoComEnderecoComIdDiferente_NaoDeveSerIgual()
    {
        // Arrange
        var endereco1 = new Endereco { Id = Guid.NewGuid(), Rua = "Rua A" };
        var endereco2 = new Endereco { Id = Guid.NewGuid(), Rua = "Rua A" };

        // Act & Assert
        endereco1.Should().NotBe(endereco2, "endereços com Ids diferentes não devem ser iguais");
    }
}
