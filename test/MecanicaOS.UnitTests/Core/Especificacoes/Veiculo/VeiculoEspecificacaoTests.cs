using Core.DTOs.Entidades.Veiculo;
using Core.Especificacoes.Veiculo;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures;
using Xunit;

namespace MecanicaOS.UnitTests.Core.Especificacoes.Veiculo;

public class VeiculoEspecificacaoTests
{
    private List<VeiculoEntityDto> GetVeiculosDeTeste()
    {
        var veiculo1 = VeiculoFixture.CriarVeiculoEntityDtoValido();
        veiculo1.Placa = "ABC-1234";
        veiculo1.ClienteId = Guid.NewGuid();

        var veiculo2 = VeiculoFixture.CriarVeiculoEntityDtoValido();
        veiculo2.Placa = "XYZ-5678";
        veiculo2.ClienteId = veiculo1.ClienteId; // Mesmo cliente

        var veiculo3 = VeiculoFixture.CriarVeiculoEntityDtoValido();
        veiculo3.Placa = "DEF-9012";
        veiculo3.ClienteId = Guid.NewGuid(); // Cliente diferente

        var veiculo4 = VeiculoFixture.CriarVeiculoEntityDtoValido();
        veiculo4.Placa = "GHI-3456";
        veiculo4.Ativo = false; // Inativo

        return new List<VeiculoEntityDto> { veiculo1, veiculo2, veiculo3, veiculo4 };
    }

    [Fact]
    public void ObterVeiculoPorPlacaEspecificacao_DeveRetornarVeiculoCorreto()
    {
        // Arrange
        var veiculos = GetVeiculosDeTeste();
        var placa = "ABC-1234";
        var especificacao = new ObterVeiculoPorPlacaEspecificacao(placa);

        // Act
        var resultado = veiculos.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(1, "deve retornar apenas o veículo com a placa especificada");
        resultado.First().Placa.Should().Be(placa, "deve retornar o veículo com placa correta");
    }

    [Fact]
    public void ObterVeiculoPorPlacaEspecificacao_QuandoPlacaNaoExiste_DeveRetornarListaVazia()
    {
        // Arrange
        var veiculos = GetVeiculosDeTeste();
        var placaInexistente = "ZZZ-9999";
        var especificacao = new ObterVeiculoPorPlacaEspecificacao(placaInexistente);

        // Act
        var resultado = veiculos.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().BeEmpty("não deve retornar nenhum veículo quando placa não existe");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ObterVeiculoPorPlacaEspecificacao_ComPlacaInvalida_DeveRetornarListaVazia(string placaInvalida)
    {
        // Arrange
        var veiculos = GetVeiculosDeTeste();
        var especificacao = new ObterVeiculoPorPlacaEspecificacao(placaInvalida);

        // Act
        var resultado = veiculos.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().BeEmpty("não deve retornar nenhum veículo quando placa é inválida");
    }

    [Fact]
    public void ObterVeiculoPorClienteEspecificacao_DeveRetornarVeiculosDoCliente()
    {
        // Arrange
        var veiculos = GetVeiculosDeTeste();
        var clienteId = veiculos.First().ClienteId;
        var especificacao = new ObterVeiculoPorClienteEspecificacao(clienteId.Value);

        // Act
        var resultado = veiculos.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(2, "deve retornar todos os veículos do cliente especificado");
        resultado.Should().OnlyContain(v => v.ClienteId == clienteId, 
            "todos os veículos devem pertencer ao cliente especificado");
    }

    [Fact]
    public void ObterVeiculoPorClienteEspecificacao_QuandoClienteNaoTemVeiculos_DeveRetornarListaVazia()
    {
        // Arrange
        var veiculos = GetVeiculosDeTeste();
        var clienteIdSemVeiculos = Guid.NewGuid();
        var especificacao = new ObterVeiculoPorClienteEspecificacao(clienteIdSemVeiculos);

        // Act
        var resultado = veiculos.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().BeEmpty("não deve retornar nenhum veículo quando cliente não tem veículos");
    }

    [Fact]
    public void ObterVeiculosResumidosEspecificacao_DeveRetornarApenasVeiculosAtivos()
    {
        // Arrange
        var veiculos = GetVeiculosDeTeste();
        var especificacao = new ObterVeiculosResumidosEspecificacao();

        // Act
        var resultado = veiculos.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(3, "deve retornar apenas veículos ativos");
        resultado.Should().OnlyContain(v => v.Ativo, "todos os veículos devem estar ativos");
    }

    [Fact]
    public void ObterVeiculosResumidosEspecificacao_ComTodosVeiculosAtivos_DeveRetornarTodos()
    {
        // Arrange
        var veiculos = new List<VeiculoEntityDto>
        {
            VeiculoFixture.CriarVeiculoEntityDtoValido(),
            VeiculoFixture.CriarVeiculoEntityDtoValido(),
            VeiculoFixture.CriarVeiculoEntityDtoValido()
        };
        
        veiculos.ForEach(v => v.Ativo = true);
        var especificacao = new ObterVeiculosResumidosEspecificacao();

        // Act
        var resultado = veiculos.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(3, "deve retornar todos os veículos quando todos estão ativos");
    }

    [Fact]
    public void ObterVeiculosResumidosEspecificacao_ComTodosVeiculosInativos_DeveRetornarListaVazia()
    {
        // Arrange
        var veiculos = new List<VeiculoEntityDto>
        {
            VeiculoFixture.CriarVeiculoEntityDtoValido(),
            VeiculoFixture.CriarVeiculoEntityDtoValido()
        };
        
        veiculos.ForEach(v => v.Ativo = false);
        var especificacao = new ObterVeiculosResumidosEspecificacao();

        // Act
        var resultado = veiculos.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().BeEmpty("não deve retornar veículos inativos");
    }

    [Fact]
    public void ObterVeiculoPorPlacaEspecificacao_ComBuscaCaseSensitive_DeveFuncionarCorretamente()
    {
        // Arrange
        var veiculos = new List<VeiculoEntityDto>
        {
            VeiculoFixture.CriarVeiculoEntityDtoValido()
        };
        veiculos.First().Placa = "ABC-1234";
        
        var especificacao1 = new ObterVeiculoPorPlacaEspecificacao("ABC-1234");
        var especificacao2 = new ObterVeiculoPorPlacaEspecificacao("abc-1234");

        // Act
        var resultado1 = veiculos.Where(especificacao1.Expressao.Compile()).ToList();
        var resultado2 = veiculos.Where(especificacao2.Expressao.Compile()).ToList();

        // Assert
        resultado1.Should().HaveCount(1, "deve encontrar com placa exata");
        resultado2.Should().BeEmpty("não deve encontrar com case diferente");
    }

    [Fact]
    public void ObterVeiculoPorPlacaEspecificacao_ComPlacasComEspacos_DeveFuncionarCorretamente()
    {
        // Arrange
        var veiculos = new List<VeiculoEntityDto>
        {
            VeiculoFixture.CriarVeiculoEntityDtoValido(),
            VeiculoFixture.CriarVeiculoEntityDtoValido()
        };
        
        veiculos[0].Placa = "ABC-1234";
        veiculos[1].Placa = " ABC-1234 ";
        
        var especificacao = new ObterVeiculoPorPlacaEspecificacao("ABC-1234");

        // Act
        var resultado = veiculos.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(1, "deve encontrar apenas a placa exata, sem espaços extras");
        resultado.First().Placa.Should().Be("ABC-1234");
    }

    [Theory]
    [InlineData("ABC-1234")]
    [InlineData("XYZ-5678")]
    [InlineData("DEF-9012")]
    public void ObterVeiculoPorPlacaEspecificacao_ComDiferentesPlacas_DeveFiltrarCorretamente(string placa)
    {
        // Arrange
        var veiculos = GetVeiculosDeTeste();
        var especificacao = new ObterVeiculoPorPlacaEspecificacao(placa);

        // Act
        var resultado = veiculos.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        var esperado = veiculos.Count(v => v.Placa == placa);
        resultado.Should().HaveCount(esperado, $"deve retornar {esperado} veículo(s) com placa {placa}");
        
        if (esperado > 0)
        {
            resultado.Should().OnlyContain(v => v.Placa == placa, 
                "todos os veículos devem ter a placa especificada");
        }
    }
}
