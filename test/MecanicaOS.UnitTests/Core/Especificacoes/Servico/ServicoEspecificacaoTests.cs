using Core.DTOs.Entidades.Servico;
using Core.Especificacoes.Servico;
using MecanicaOS.UnitTests.Fixtures;

namespace MecanicaOS.UnitTests.Core.Especificacoes.Servico;

public class ServicoEspecificacaoTests
{
    private List<ServicoEntityDto> GetServicosDeTeste()
    {
        var servico1 = ServicoFixture.CriarServicoEntityDtoValido();
        servico1.Disponivel = true;

        var servico2 = ServicoFixture.CriarServicoEntityDtoValido();
        servico2.Disponivel = false;

        var servico3 = ServicoFixture.CriarServicoEntityDtoValido();
        servico3.Disponivel = true;
        servico3.Nome = "Troca de Óleo";

        var servico4 = ServicoFixture.CriarServicoEntityDtoValido();
        servico4.Disponivel = false;
        servico4.Nome = "Alinhamento";

        return new List<ServicoEntityDto> { servico1, servico2, servico3, servico4 };
    }

    [Fact]
    public void ObterServicoDisponivelEspecificacao_DeveRetornarApenasServicosDisponiveis()
    {
        // Arrange
        var servicos = GetServicosDeTeste();
        var especificacao = new ObterServicoDisponivelEspecificacao();

        // Act
        var resultado = servicos.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(2, "deve retornar apenas os serviços disponíveis");
        resultado.Should().OnlyContain(s => s.Disponivel, "todos os serviços devem estar disponíveis");
    }

    [Fact]
    public void ObterServicoDisponivelEspecificacao_ComTodosServicosDisponiveis_DeveRetornarTodos()
    {
        // Arrange
        var servicos = new List<ServicoEntityDto>
        {
            ServicoFixture.CriarServicoEntityDtoValido(),
            ServicoFixture.CriarServicoEntityDtoValido(),
            ServicoFixture.CriarServicoEntityDtoValido()
        };

        servicos.ForEach(s => s.Disponivel = true);
        var especificacao = new ObterServicoDisponivelEspecificacao();

        // Act
        var resultado = servicos.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(3, "deve retornar todos os serviços quando todos estão disponíveis");
    }

    [Fact]
    public void ObterServicoDisponivelEspecificacao_ComNenhumServicoDisponivel_DeveRetornarListaVazia()
    {
        // Arrange
        var servicos = new List<ServicoEntityDto>
        {
            ServicoFixture.CriarServicoEntityDtoValido(),
            ServicoFixture.CriarServicoEntityDtoValido()
        };

        servicos.ForEach(s => s.Disponivel = false);
        var especificacao = new ObterServicoDisponivelEspecificacao();

        // Act
        var resultado = servicos.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().BeEmpty("não deve retornar nenhum serviço quando nenhum está disponível");
    }

    [Fact]
    public void ObterServicoPorNomeEspecificacao_DeveRetornarServicoComNomeCorreto()
    {
        // Arrange
        var servicos = GetServicosDeTeste();
        var nomeServico = "Troca de Óleo";
        var especificacao = new ObterServicoPorNomeEspecificacao(nomeServico);

        // Act
        var resultado = servicos.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(1, "deve retornar apenas o serviço com o nome especificado");
        resultado.First().Nome.Should().Be(nomeServico, "deve retornar o serviço com nome correto");
    }

    [Fact]
    public void ObterServicoPorNomeEspecificacao_QuandoNomeNaoExiste_DeveRetornarListaVazia()
    {
        // Arrange
        var servicos = GetServicosDeTeste();
        var nomeInexistente = "Serviço Inexistente";
        var especificacao = new ObterServicoPorNomeEspecificacao(nomeInexistente);

        // Act
        var resultado = servicos.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().BeEmpty("não deve retornar nenhum serviço quando nome não existe");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ObterServicoPorNomeEspecificacao_ComNomeInvalido_DeveRetornarListaVazia(string nomeInvalido)
    {
        // Arrange
        var servicos = GetServicosDeTeste();
        var especificacao = new ObterServicoPorNomeEspecificacao(nomeInvalido);

        // Act
        var resultado = servicos.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().BeEmpty("não deve retornar nenhum serviço quando nome é inválido");
    }

    [Fact]
    public void ObterServicoPorNomeEspecificacao_ComBuscaCaseSensitive_DeveFuncionarCorretamente()
    {
        // Arrange
        var servicos = new List<ServicoEntityDto>
        {
            ServicoFixture.CriarServicoEntityDtoValido()
        };
        servicos.First().Nome = "Troca de Óleo";

        var especificacao1 = new ObterServicoPorNomeEspecificacao("Troca de Óleo");
        var especificacao2 = new ObterServicoPorNomeEspecificacao("troca de óleo");

        // Act
        var resultado1 = servicos.Where(especificacao1.Expressao.Compile()).ToList();
        var resultado2 = servicos.Where(especificacao2.Expressao.Compile()).ToList();

        // Assert
        resultado1.Should().HaveCount(1, "deve encontrar com nome exato");
        resultado2.Should().BeEmpty("não deve encontrar com case diferente");
    }

    [Fact]
    public void ObterServicoPorNomeEspecificacao_ComNomesComEspacos_DeveFuncionarCorretamente()
    {
        // Arrange
        var servicos = new List<ServicoEntityDto>
        {
            ServicoFixture.CriarServicoEntityDtoValido(),
            ServicoFixture.CriarServicoEntityDtoValido()
        };

        servicos[0].Nome = "Troca de Óleo";
        servicos[1].Nome = " Troca de Óleo ";

        var especificacao = new ObterServicoPorNomeEspecificacao("Troca de Óleo");

        // Act
        var resultado = servicos.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(1, "deve encontrar apenas o nome exato, sem espaços extras");
        resultado.First().Nome.Should().Be("Troca de Óleo");
    }
}
