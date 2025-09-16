using Core.DTOs.Entidades.Cliente;
using Core.Especificacoes.Cliente;
using MecanicaOS.UnitTests.Fixtures;

namespace MecanicaOS.UnitTests.Core.Especificacoes.Cliente;

public class ClienteEspecificacaoTests
{
    private List<ClienteEntityDto> GetClientesDeTeste()
    {
        var cliente1 = ClienteFixture.CriarClienteEntityDtoValido();
        var cliente2 = ClienteFixture.CriarClienteEntityDtoValido();

        // Ensure unique documents
        cliente1.Documento = "12345678901";
        cliente2.Documento = "98765432100";

        // Ensure unique IDs
        cliente1.Id = Guid.NewGuid();
        cliente2.Id = Guid.NewGuid();

        return new List<ClienteEntityDto> { cliente1, cliente2 };
    }

    [Fact]
    public void ObterClienteComVeiculoPorIdEspecificacao_DeveRetornarClienteCorreto()
    {
        // Arrange
        var clientes = GetClientesDeTeste();
        var clienteId = clientes.First().Id;
        var especificacao = new ObterClienteComVeiculoPorIdEspecificacao(clienteId);

        // Act
        var resultado = clientes.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(1, "deve retornar apenas o cliente com o ID especificado");
        resultado.First().Id.Should().Be(clienteId, "deve retornar o cliente correto");
    }

    [Fact]
    public void ObterClienteComVeiculoPorIdEspecificacao_QuandoClienteNaoExiste_DeveRetornarListaVazia()
    {
        // Arrange
        var clientes = GetClientesDeTeste();
        var clienteIdInexistente = Guid.NewGuid();
        var especificacao = new ObterClienteComVeiculoPorIdEspecificacao(clienteIdInexistente);

        // Act
        var resultado = clientes.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().BeEmpty("não deve retornar nenhum cliente quando ID não existe");
    }

    [Fact]
    public void ObterClienteComVeiculoPorIdEspecificacao_DeveIncluirVeiculos()
    {
        // Arrange
        var especificacao = new ObterClienteComVeiculoPorIdEspecificacao(Guid.NewGuid());

        // Assert
        especificacao.Inclusoes.Should().NotBeEmpty("deve ter inclusões definidas");
        especificacao.Inclusoes.Should().Contain(i => i.ToString().Contains("Veiculos"),
            "deve incluir a propriedade Veiculos");
    }

    [Fact]
    public void ObterClientePorDocumento_DeveRetornarClienteCorreto()
    {
        // Arrange
        var clientes = GetClientesDeTeste();
        var documento = "12345678901";
        var especificacao = new ObterClientePorDocumento(documento);

        // Act
        var resultado = clientes.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(1, "deve retornar apenas o cliente com o documento especificado");
        resultado.First().Documento.Should().Be(documento, "deve retornar o cliente com documento correto");
    }

    [Fact]
    public void ObterClientePorDocumento_QuandoDocumentoNaoExiste_DeveRetornarListaVazia()
    {
        // Arrange
        var clientes = GetClientesDeTeste();
        var documentoInexistente = "99999999999";
        var especificacao = new ObterClientePorDocumento(documentoInexistente);

        // Act
        var resultado = clientes.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().BeEmpty("não deve retornar nenhum cliente quando documento não existe");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ObterClientePorDocumento_ComDocumentoInvalido_DeveRetornarListaVazia(string documentoInvalido)
    {
        // Arrange
        var clientes = GetClientesDeTeste();
        var especificacao = new ObterClientePorDocumento(documentoInvalido);

        // Act
        var resultado = clientes.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().BeEmpty("não deve retornar nenhum cliente quando documento é inválido");
    }

    [Fact]
    public void ObterTodosClienteComVeiculoEspecificacao_DeveRetornarTodosClientes()
    {
        // Arrange
        var clientes = GetClientesDeTeste();
        var especificacao = new ObterTodosClienteComVeiculoEspecificacao();

        // Act
        var resultado = clientes.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(clientes.Count, "deve retornar todos os clientes");
        resultado.Should().OnlyContain(c => c.Ativo, "deve retornar apenas clientes ativos");
    }

    [Fact]
    public void ObterTodosClienteComVeiculoEspecificacao_DeveIncluirVeiculos()
    {
        // Arrange
        var especificacao = new ObterTodosClienteComVeiculoEspecificacao();

        // Assert
        especificacao.Inclusoes.Should().NotBeEmpty("deve ter inclusões definidas");
        especificacao.Inclusoes.Should().Contain(i => i.ToString().Contains("Veiculos"),
            "deve incluir a propriedade Veiculos");
    }

    [Fact]
    public void ObterTodosClienteComVeiculoEspecificacao_ComClientesInativos_DeveRetornarApenasAtivos()
    {
        // Arrange
        var clientes = GetClientesDeTeste();
        clientes.First().Ativo = false; // Inativar um cliente
        var especificacao = new ObterTodosClienteComVeiculoEspecificacao();

        // Act
        var resultado = clientes.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(clientes.Count - 1, "deve excluir clientes inativos");
        resultado.Should().OnlyContain(c => c.Ativo, "deve retornar apenas clientes ativos");
    }
}
