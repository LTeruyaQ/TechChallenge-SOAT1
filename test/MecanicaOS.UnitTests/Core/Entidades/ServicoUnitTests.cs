using Xunit;
using FluentAssertions;
using Core.Entidades;
using Core.Entidades.Abstratos;

namespace MecanicaOS.UnitTests.Core.Entidades;

public class ServicoUnitTests
{
    [Fact]
    public void Servico_QuandoCriado_DeveInicializarComValoresPadrao()
    {
        // Arrange & Act
        var servico = new Servico { Nome = "Teste", Descricao = "Teste" };

        // Assert
        servico.Should().BeAssignableTo<Entidade>("Servico deve herdar de Entidade");
        servico.Id.Should().NotBeEmpty("Id deve ser gerado automaticamente");
        servico.Ativo.Should().BeTrue("Servico deve estar ativo por padrão");
        servico.DataCadastro.Should().NotBe(default(DateTime), "DataCadastro deve ser definida");
        servico.DataAtualizacao.Should().NotBe(default(DateTime), "DataAtualizacao deve ser definida");
    }

    [Fact]
    public void Servico_QuandoDefinidoNome_DeveArmazenarCorretamente()
    {
        // Arrange
        var servico = new Servico { Nome = "Inicial", Descricao = "Inicial" };
        var nomeEsperado = "Troca de Óleo";

        // Act
        servico.Nome = nomeEsperado;

        // Assert
        servico.Nome.Should().Be(nomeEsperado, "o nome deve ser armazenado corretamente");
    }

    [Fact]
    public void Servico_QuandoDefinidaDescricao_DeveArmazenarCorretamente()
    {
        // Arrange
        var servico = new Servico { Nome = "Teste", Descricao = "Inicial" };
        var descricaoEsperada = "Troca completa do óleo do motor";

        // Act
        servico.Descricao = descricaoEsperada;

        // Assert
        servico.Descricao.Should().Be(descricaoEsperada, "a descrição deve ser armazenada corretamente");
    }

    [Fact]
    public void Servico_QuandoDefinidoValor_DeveArmazenarCorretamente()
    {
        // Arrange
        var servico = new Servico { Nome = "Teste", Descricao = "Teste" };
        var valorEsperado = 150.75m;

        // Act
        servico.Valor = valorEsperado;

        // Assert
        servico.Valor.Should().Be(valorEsperado, "o valor deve ser armazenado corretamente");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Servico_QuandoDefinidoDisponivel_DeveArmazenarCorretamente(bool disponivel)
    {
        // Arrange
        var servico = new Servico { Nome = "Teste", Descricao = "Teste" };

        // Act
        servico.Disponivel = disponivel;

        // Assert
        servico.Disponivel.Should().Be(disponivel, "a disponibilidade deve ser armazenada corretamente");
    }

    [Fact]
    public void Servico_QuandoAtualizadoComTodosParametros_DeveAtualizarCorretamente()
    {
        // Arrange
        var servico = new Servico { Nome = "Nome Original", Descricao = "Descrição Original" };
        var novoNome = "Novo Nome";
        var novaDescricao = "Nova Descrição";
        var novoValor = 200.50m;
        var novaDisponibilidade = true;

        // Act
        servico.Atualizar(novoNome, novaDescricao, novoValor, novaDisponibilidade);

        // Assert
        servico.Nome.Should().Be(novoNome, "o nome deve ser atualizado");
        servico.Descricao.Should().Be(novaDescricao, "a descrição deve ser atualizada");
        servico.Valor.Should().Be(novoValor, "o valor deve ser atualizado");
        servico.Disponivel.Should().Be(novaDisponibilidade, "a disponibilidade deve ser atualizada");
    }

    [Fact]
    public void Servico_QuandoAtualizadoComParametrosNulos_NaoDeveAlterarValoresExistentes()
    {
        // Arrange
        var servico = new Servico 
        { 
            Nome = "Nome Original", 
            Descricao = "Descrição Original",
            Valor = 100.00m,
            Disponivel = false
        };

        var valoresOriginais = new
        {
            Nome = servico.Nome,
            Descricao = servico.Descricao,
            Valor = servico.Valor,
            Disponivel = servico.Disponivel
        };

        // Act
        servico.Atualizar(null!, null!, null, null);

        // Assert
        servico.Nome.Should().Be(valoresOriginais.Nome, "o nome não deve ser alterado");
        servico.Descricao.Should().Be(valoresOriginais.Descricao, "a descrição não deve ser alterada");
        servico.Valor.Should().Be(valoresOriginais.Valor, "o valor não deve ser alterado");
        servico.Disponivel.Should().Be(valoresOriginais.Disponivel, "a disponibilidade não deve ser alterada");
    }

    [Fact]
    public void Servico_QuandoAtualizadoComStringVazia_NaoDeveAlterarValoresExistentes()
    {
        // Arrange
        var servico = new Servico 
        { 
            Nome = "Nome Original", 
            Descricao = "Descrição Original"
        };

        var valoresOriginais = new
        {
            Nome = servico.Nome,
            Descricao = servico.Descricao
        };

        // Act
        servico.Atualizar("", "", null, null);

        // Assert
        servico.Nome.Should().Be(valoresOriginais.Nome, "o nome não deve ser alterado com string vazia");
        servico.Descricao.Should().Be(valoresOriginais.Descricao, "a descrição não deve ser alterada com string vazia");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(50.25)]
    [InlineData(1000.99)]
    public void Servico_QuandoDefinidoValorPositivoOuZero_DeveArmazenarCorretamente(decimal valor)
    {
        // Arrange
        var servico = new Servico { Nome = "Teste", Descricao = "Teste" };

        // Act
        servico.Valor = valor;

        // Assert
        servico.Valor.Should().Be(valor, "valores positivos ou zero devem ser armazenados corretamente");
    }

    [Fact]
    public void Servico_QuandoComparadoComOutroServicoComMesmoId_DeveSerIgual()
    {
        // Arrange
        var id = Guid.NewGuid();
        var servico1 = new Servico { Id = id, Nome = "Serviço 1", Descricao = "Descrição 1" };
        var servico2 = new Servico { Id = id, Nome = "Serviço 2", Descricao = "Descrição 2" };

        // Act & Assert
        servico1.Should().Be(servico2, "serviços com mesmo Id devem ser considerados iguais");
        servico1.GetHashCode().Should().Be(servico2.GetHashCode(), "hash codes devem ser iguais para objetos iguais");
    }
}
