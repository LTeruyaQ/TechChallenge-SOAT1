using Core.DTOs.Entidades.Cliente;
using Core.Especificacoes.Base;
using Core.Especificacoes.Base.Extensoes;
using MecanicaOS.UnitTests.Fixtures;
using System.Linq.Expressions;

namespace MecanicaOS.UnitTests.Core.Especificacoes.Base;

public class EspecificacaoBaseTests
{
    private class EspecificacaoTeste : EspecificacaoBase<ClienteEntityDto>
    {
        private readonly string _nome;

        public EspecificacaoTeste(string nome)
        {
            _nome = nome;
        }

        public override Expression<Func<ClienteEntityDto, bool>> Expressao =>
            c => c.Nome.Contains(_nome);
    }

    private class EspecificacaoTeste2 : EspecificacaoBase<ClienteEntityDto>
    {
        private readonly bool _ativo;

        public EspecificacaoTeste2(bool ativo)
        {
            _ativo = ativo;
        }

        public override Expression<Func<ClienteEntityDto, bool>> Expressao =>
            c => c.Ativo == _ativo;
    }

    [Fact]
    public void EspecificacaoBase_DevePermitirCriacaoDeEspecificacaoPersonalizada()
    {
        // Arrange
        var nome = "João";
        var especificacao = new EspecificacaoTeste(nome);

        // Act & Assert
        especificacao.Should().NotBeNull("especificação deve ser criada corretamente");
        especificacao.Expressao.Should().NotBeNull("expressão deve estar definida");
    }

    [Fact]
    public void EspecificacaoBase_ExpressaoCompilada_DeveFiltrarCorretamente()
    {
        // Arrange
        var clientes = new List<ClienteEntityDto>
        {
            ClienteFixture.CriarClienteEntityDtoValido(),
            ClienteFixture.CriarClienteEntityDtoValido()
        };

        clientes[0].Nome = "João Silva";
        clientes[1].Nome = "Maria Santos";

        var especificacao = new EspecificacaoTeste("João");

        // Act
        var resultado = clientes.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(1, "deve filtrar apenas clientes que atendem à especificação");
        resultado.First().Nome.Should().Contain("João");
    }

    [Fact]
    public void EExtensao_DevePermitirCombinacaoComE()
    {
        // Arrange
        var clientes = new List<ClienteEntityDto>
        {
            ClienteFixture.CriarClienteEntityDtoValido(),
            ClienteFixture.CriarClienteEntityDtoValido(),
            ClienteFixture.CriarClienteEntityDtoValido()
        };

        clientes[0].Nome = "João Silva";
        clientes[0].Ativo = true;

        clientes[1].Nome = "João Santos";
        clientes[1].Ativo = false;

        clientes[2].Nome = "Maria Silva";
        clientes[2].Ativo = true;

        var especificacao1 = new EspecificacaoTeste("João");
        var especificacao2 = new EspecificacaoTeste2(true);

        // Act
        var especificacaoCombinada = especificacao1.E(especificacao2);
        var resultado = clientes.Where(especificacaoCombinada.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(1, "deve retornar apenas clientes que atendem ambas especificações");
        resultado.First().Nome.Should().Be("João Silva");
        resultado.First().Ativo.Should().BeTrue();
    }

    [Fact]
    public void OuExtensao_DevePermitirCombinacaoComOu()
    {
        // Arrange
        var clientes = new List<ClienteEntityDto>
        {
            ClienteFixture.CriarClienteEntityDtoValido(),
            ClienteFixture.CriarClienteEntityDtoValido(),
            ClienteFixture.CriarClienteEntityDtoValido()
        };

        clientes[0].Nome = "João Silva";
        clientes[1].Nome = "Maria Santos";
        clientes[2].Nome = "Pedro Costa";

        var especificacao1 = new EspecificacaoTeste("João");
        var especificacao2 = new EspecificacaoTeste("Maria");

        // Act
        var especificacaoCombinada = especificacao1.Ou(especificacao2);
        var resultado = clientes.Where(especificacaoCombinada.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(2, "deve retornar clientes que atendem qualquer uma das especificações");
        resultado.Should().Contain(c => c.Nome.Contains("João"));
        resultado.Should().Contain(c => c.Nome.Contains("Maria"));
    }

    [Fact]
    public void EspecificacaoBase_ComInclusoes_DevePermitirAcessoAsInclusoes()
    {
        // Arrange
        var especificacao = new EspecificacaoTeste("teste");

        // Assert
        especificacao.Inclusoes.Should().NotBeNull("deve ter propriedade Inclusoes disponível");
    }

    [Theory]
    [InlineData("João")]
    [InlineData("Maria")]
    [InlineData("Pedro")]
    public void EspecificacaoTeste_ComDiferentesNomes_DeveFiltrarCorretamente(string nome)
    {
        // Arrange
        var clientes = new List<ClienteEntityDto>
        {
            ClienteFixture.CriarClienteEntityDtoValido(),
            ClienteFixture.CriarClienteEntityDtoValido(),
            ClienteFixture.CriarClienteEntityDtoValido()
        };

        clientes[0].Nome = "João Silva";
        clientes[1].Nome = "Maria Santos";
        clientes[2].Nome = "Pedro Costa";

        var especificacao = new EspecificacaoTeste(nome);

        // Act
        var resultado = clientes.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        var esperado = clientes.Count(c => c.Nome.Contains(nome));
        resultado.Should().HaveCount(esperado, $"deve retornar {esperado} cliente(s) com nome contendo '{nome}'");
    }

    [Fact]
    public void EspecificacaoBase_DevePermitirVerificacaoDePropriedades()
    {
        // Arrange
        var especificacao = new EspecificacaoTeste("teste");

        // Assert
        especificacao.Inclusoes.Should().NotBeNull("deve ter propriedade Inclusoes disponível");
        especificacao.Expressao.Should().NotBeNull("deve ter propriedade Expressao disponível");
    }
}
