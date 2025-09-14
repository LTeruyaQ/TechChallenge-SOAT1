using Xunit;
using FluentAssertions;
using Core.DTOs.Repositories.Servico;
using Core.DTOs.Repositories.Autenticacao;

namespace MecanicaOS.UnitTests.Core.DTOs.Repositories;

public class ServicoRepositoryDtoUnitTests
{
    [Fact]
    public void ServicoRepositoryDto_QuandoCriado_DeveHerdarDeRepositoryDto()
    {
        // Arrange & Act
        var dto = new ServicoRepositoryDto { Nome = "Teste", Descricao = "Teste" };

        // Assert
        dto.Should().BeAssignableTo<RepositoryDto>("ServicoRepositoryDto deve herdar de RepositoryDto");
        dto.Id.Should().Be(Guid.Empty, "Id deve ser vazio por padrão no DTO");
        dto.Ativo.Should().BeFalse("Ativo deve ser false por padrão no DTO");
    }

    [Fact]
    public void ServicoRepositoryDto_QuandoDefinidoCamposTecnicos_DevePreservarAuditoria()
    {
        // Arrange
        var dto = new ServicoRepositoryDto { Nome = "Teste", Descricao = "Teste" };
        var id = Guid.NewGuid();
        var dataCadastro = DateTime.Now;
        var dataAtualizacao = DateTime.Now.AddMinutes(5);

        // Act
        dto.Id = id;
        dto.DataCadastro = dataCadastro;
        dto.DataAtualizacao = dataAtualizacao;
        dto.Ativo = true;

        // Assert
        dto.Id.Should().Be(id, "o ID deve ser preservado corretamente");
        dto.DataCadastro.Should().Be(dataCadastro, "a data de cadastro deve ser preservada");
        dto.DataAtualizacao.Should().Be(dataAtualizacao, "a data de atualização deve ser preservada");
        dto.Ativo.Should().BeTrue("o status ativo deve ser preservado");
    }

    [Fact]
    public void ServicoRepositoryDto_QuandoDefinidoNome_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new ServicoRepositoryDto { Nome = "Inicial", Descricao = "Inicial" };
        var nomeEsperado = "Troca de Óleo";

        // Act
        dto.Nome = nomeEsperado;

        // Assert
        dto.Nome.Should().Be(nomeEsperado, "o nome deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void ServicoRepositoryDto_QuandoDefinidaDescricao_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new ServicoRepositoryDto { Nome = "Teste", Descricao = "Inicial" };
        var descricaoEsperada = "Troca completa do óleo do motor";

        // Act
        dto.Descricao = descricaoEsperada;

        // Assert
        dto.Descricao.Should().Be(descricaoEsperada, "a descrição deve ser armazenada corretamente no DTO");
    }

    [Fact]
    public void ServicoRepositoryDto_QuandoDefinidoValor_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new ServicoRepositoryDto { Nome = "Teste", Descricao = "Teste" };
        var valorEsperado = 150.75m;

        // Act
        dto.Valor = valorEsperado;

        // Assert
        dto.Valor.Should().Be(valorEsperado, "o valor deve ser armazenado corretamente no DTO");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ServicoRepositoryDto_QuandoDefinidoDisponivel_DeveArmazenarCorretamente(bool disponivel)
    {
        // Arrange
        var dto = new ServicoRepositoryDto { Nome = "Teste", Descricao = "Teste" };

        // Act
        dto.Disponivel = disponivel;

        // Assert
        dto.Disponivel.Should().Be(disponivel, "a disponibilidade deve ser armazenada corretamente no DTO");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(50.25)]
    [InlineData(1000.99)]
    public void ServicoRepositoryDto_QuandoDefinidoValorPositivoOuZero_DeveArmazenarCorretamente(decimal valor)
    {
        // Arrange
        var dto = new ServicoRepositoryDto { Nome = "Teste", Descricao = "Teste" };

        // Act
        dto.Valor = valor;

        // Assert
        dto.Valor.Should().Be(valor, "valores positivos ou zero devem ser armazenados corretamente no DTO");
    }

    [Theory]
    [InlineData("Troca de Óleo")]
    [InlineData("Revisão Completa")]
    [InlineData("Alinhamento e Balanceamento")]
    public void ServicoRepositoryDto_QuandoDefinidoNomeComValoresDiferentes_DeveArmazenarCorretamente(string nome)
    {
        // Arrange
        var dto = new ServicoRepositoryDto { Nome = "Inicial", Descricao = "Teste" };

        // Act
        dto.Nome = nome;

        // Assert
        dto.Nome.Should().Be(nome, "o nome deve ser armazenado independente do conteúdo no DTO");
    }

    [Theory]
    [InlineData("Serviço básico de manutenção")]
    [InlineData("Serviço completo com garantia")]
    [InlineData("Serviço express")]
    public void ServicoRepositoryDto_QuandoDefinidaDescricaoComValoresDiferentes_DeveArmazenarCorretamente(string descricao)
    {
        // Arrange
        var dto = new ServicoRepositoryDto { Nome = "Teste", Descricao = "Inicial" };

        // Act
        dto.Descricao = descricao;

        // Assert
        dto.Descricao.Should().Be(descricao, "a descrição deve ser armazenada independente do conteúdo no DTO");
    }
}
