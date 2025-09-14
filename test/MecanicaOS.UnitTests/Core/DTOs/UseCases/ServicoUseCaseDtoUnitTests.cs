using Core.DTOs.UseCases.Servico;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.UseCases;
using Xunit;

namespace MecanicaOS.UnitTests.Core.DTOs.UseCases;

public class ServicoUseCaseDtoUnitTests
{
    [Fact]
    public void CadastrarServicoUseCaseDto_DeveSerInicializadoComPropriedadesCorretas()
    {
        // Arrange & Act
        var dto = ServicoUseCaseFixture.CriarCadastrarServicoUseCaseDtoValido();

        // Assert
        dto.Should().NotBeNull("o DTO deve ser criado corretamente");
        dto.Nome.Should().Be("Troca de Óleo Completa", "o nome deve ser armazenado corretamente");
        dto.Descricao.Should().Be("Troca de óleo do motor com filtro e verificação de fluidos", 
            "a descrição deve ser armazenada corretamente");
        dto.Valor.Should().Be(120.00m, "o valor deve ser armazenado corretamente");
        dto.Disponivel.Should().BeTrue("deve estar disponível");
    }

    [Fact]
    public void CadastrarServicoUseCaseDto_DevePermitirServicoCaroIndisponivel()
    {
        // Arrange & Act
        var dto = ServicoUseCaseFixture.CriarCadastrarServicoUseCaseDtoCaroIndisponivel();

        // Assert
        dto.Nome.Should().Be("Reparo de Transmissão", "deve aceitar serviços caros");
        dto.Valor.Should().Be(2500.00m, "deve aceitar valores altos");
        dto.Disponivel.Should().BeFalse("deve permitir serviços indisponíveis");
    }

    [Fact]
    public void CadastrarServicoUseCaseDto_DevePermitirServicoGratuito()
    {
        // Arrange & Act
        var dto = ServicoUseCaseFixture.CriarCadastrarServicoUseCaseDtoGratuito();

        // Assert
        dto.Nome.Should().Be("Diagnóstico Inicial", "deve aceitar serviços gratuitos");
        dto.Valor.Should().Be(0.00m, "deve aceitar valor zero");
        dto.Disponivel.Should().BeTrue("serviços gratuitos podem estar disponíveis");
    }

    [Theory]
    [InlineData(0.00)]
    [InlineData(50.50)]
    [InlineData(1000.00)]
    [InlineData(9999.99)]
    public void CadastrarServicoUseCaseDto_DeveAceitarDiferentesValores(decimal valor)
    {
        // Arrange & Act
        var dto = new CadastrarServicoUseCaseDto
        {
            Nome = "Serviço Teste",
            Descricao = "Descrição Teste",
            Valor = valor,
            Disponivel = true
        };

        // Assert
        dto.Valor.Should().Be(valor, "deve aceitar o valor especificado");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CadastrarServicoUseCaseDto_DeveAceitarDisponibilidade(bool disponivel)
    {
        // Arrange & Act
        var dto = new CadastrarServicoUseCaseDto
        {
            Nome = "Serviço Teste",
            Descricao = "Descrição Teste",
            Valor = 100.00m,
            Disponivel = disponivel
        };

        // Assert
        dto.Disponivel.Should().Be(disponivel, "deve aceitar a disponibilidade especificada");
    }

    [Fact]
    public void EditarServicoUseCaseDto_DeveSerInicializadoComPropriedadesCorretas()
    {
        // Arrange & Act
        var dto = ServicoUseCaseFixture.CriarEditarServicoUseCaseDtoValido();

        // Assert
        dto.Should().NotBeNull("o DTO deve ser criado corretamente");
        dto.Nome.Should().Be("Alinhamento e Balanceamento", "o nome deve ser armazenado corretamente");
        dto.Descricao.Should().Be("Alinhamento e balanceamento das quatro rodas", 
            "a descrição deve ser armazenada corretamente");
        dto.Valor.Should().Be(80.00m, "o valor deve ser armazenado corretamente");
        dto.Disponivel.Should().BeTrue("deve estar disponível");
    }

    [Fact]
    public void EditarServicoUseCaseDto_DevePermitirValoresNulos()
    {
        // Arrange & Act
        var dto = ServicoUseCaseFixture.CriarEditarServicoUseCaseDtoComValoresNulos();

        // Assert
        dto.Nome.Should().NotBeNullOrEmpty("nome é obrigatório mesmo em edição");
        dto.Descricao.Should().NotBeNullOrEmpty("descrição é obrigatória mesmo em edição");
        dto.Valor.Should().BeNull("deve permitir valor nulo para não alterar");
        dto.Disponivel.Should().BeNull("deve permitir disponibilidade nula para não alterar");
    }

    [Fact]
    public void EditarServicoUseCaseDto_DevePermitirServicoIndisponivel()
    {
        // Arrange & Act
        var dto = ServicoUseCaseFixture.CriarEditarServicoUseCaseDtoIndisponivel();

        // Assert
        dto.Nome.Should().Be("Serviço Temporariamente Indisponível", 
            "deve aceitar nome indicando indisponibilidade");
        dto.Disponivel.Should().BeFalse("deve permitir marcar como indisponível");
        dto.Valor.Should().Be(150.00m, "deve manter valor mesmo indisponível");
    }

    [Fact]
    public void CadastrarServicoUseCaseDto_ListaFixture_DeveConterTodosOsItens()
    {
        // Arrange & Act
        var lista = ServicoUseCaseFixture.CriarListaCadastrarServicoUseCaseDto();

        // Assert
        lista.Should().NotBeNull("a lista deve ser criada");
        lista.Should().HaveCount(3, "deve conter todos os DTOs da fixture");
        lista.Should().OnlyContain(dto => !string.IsNullOrEmpty(dto.Nome), 
            "todos os DTOs devem ter nome");
        lista.Should().OnlyContain(dto => !string.IsNullOrEmpty(dto.Descricao), 
            "todos os DTOs devem ter descrição");
        lista.Should().OnlyContain(dto => dto.Valor >= 0, 
            "todos os valores devem ser não negativos");
    }

    [Fact]
    public void EditarServicoUseCaseDto_ListaFixture_DeveConterTodosOsItens()
    {
        // Arrange & Act
        var lista = ServicoUseCaseFixture.CriarListaEditarServicoUseCaseDto();

        // Assert
        lista.Should().NotBeNull("a lista deve ser criada");
        lista.Should().HaveCount(3, "deve conter todos os DTOs da fixture");
        lista.Should().OnlyContain(dto => !string.IsNullOrEmpty(dto.Nome), 
            "todos os DTOs devem ter nome");
        lista.Should().OnlyContain(dto => !string.IsNullOrEmpty(dto.Descricao), 
            "todos os DTOs devem ter descrição");
    }

    [Fact]
    public void ServicoUseCaseDto_DevePermitirAlteracaoDePropriedades()
    {
        // Arrange
        var dto = ServicoUseCaseFixture.CriarCadastrarServicoUseCaseDtoValido();
        var novoNome = "Novo Nome do Serviço";
        var novoValor = 200.00m;

        // Act
        dto.Nome = novoNome;
        dto.Valor = novoValor;

        // Assert
        dto.Nome.Should().Be(novoNome, "deve permitir alteração do nome");
        dto.Valor.Should().Be(novoValor, "deve permitir alteração do valor");
    }

    [Fact]
    public void ServicoUseCaseDto_DeveSerDistintoEntreInstancias()
    {
        // Arrange & Act
        var dto1 = ServicoUseCaseFixture.CriarCadastrarServicoUseCaseDtoValido();
        var dto2 = ServicoUseCaseFixture.CriarCadastrarServicoUseCaseDtoCaroIndisponivel();

        // Assert
        dto1.Should().NotBeSameAs(dto2, "devem ser instâncias diferentes");
        dto1.Nome.Should().NotBe(dto2.Nome, "devem ter nomes diferentes");
        dto1.Valor.Should().NotBe(dto2.Valor, "devem ter valores diferentes");
        dto1.Disponivel.Should().NotBe(dto2.Disponivel, "devem ter disponibilidades diferentes");
    }
}
