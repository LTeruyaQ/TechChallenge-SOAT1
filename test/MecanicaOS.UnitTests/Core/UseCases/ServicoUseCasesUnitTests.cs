using Core.DTOs.UseCases.Servico;
using Core.Entidades;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.UseCases;
using NSubstitute;
using Xunit;

namespace MecanicaOS.UnitTests.Core.UseCases;

public class ServicoUseCasesUnitTests
{
    private readonly ServicoUseCasesFixture _fixture;

    public ServicoUseCasesUnitTests()
    {
        _fixture = new ServicoUseCasesFixture();
    }

    [Fact]
    public async Task CadastrarServicoUseCaseAsync_ComDadosValidos_DeveRetornarServicoCadastrado()
    {
        // Arrange
        var mockServicoGateway = _fixture.CriarMockServicoGateway();
        var mockUdt = _fixture.CriarMockUnidadeDeTrabalho();

        var request = ServicoUseCasesFixture.CriarCadastrarServicoUseCaseDtoValido();
        var servicoEsperado = ServicoUseCasesFixture.CriarServicoValido();

        _fixture.ConfigurarMockServicoGatewayParaCadastro(mockServicoGateway, servicoEsperado);

        var servicoUseCases = _fixture.CriarServicoUseCases(
            mockServicoGateway, null, mockUdt);

        // Act
        var resultado = await servicoUseCases.CadastrarServicoUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Nome.Should().Be(request.Nome);
        resultado.Descricao.Should().Be(request.Descricao);
        resultado.Valor.Should().Be(request.Valor);
        resultado.Disponivel.Should().Be(request.Disponivel);

        mockServicoGateway.Received(1).ObterServicosDisponiveisPorNomeAsync(request.Nome);
        mockServicoGateway.Received(1).CadastrarAsync(Arg.Any<Servico>());
        mockUdt.Received(1).Commit();
    }

    [Fact]
    public async Task CadastrarServicoUseCaseAsync_ComNomeJaCadastrado_DeveLancarDadosJaCadastradosException()
    {
        // Arrange
        var mockServicoGateway = _fixture.CriarMockServicoGateway();
        var request = ServicoUseCasesFixture.CriarCadastrarServicoUseCaseDtoValido();

        _fixture.ConfigurarMockServicoGatewayParaNomeJaCadastrado(mockServicoGateway, request.Nome);

        var servicoUseCases = _fixture.CriarServicoUseCases(mockServicoGateway);

        // Act & Assert
        await servicoUseCases
            .Invoking(x => x.CadastrarServicoUseCaseAsync(request))
            .Should()
            .ThrowAsync<DadosJaCadastradosException>()
            .WithMessage("Serviço já cadastrado");

        mockServicoGateway.Received(1).ObterServicosDisponiveisPorNomeAsync(request.Nome);
        mockServicoGateway.DidNotReceive().CadastrarAsync(Arg.Any<Servico>());
    }

    [Fact]
    public async Task CadastrarServicoUseCaseAsync_ComServicoIndisponivel_DeveCadastrarComSucesso()
    {
        // Arrange
        var mockServicoGateway = _fixture.CriarMockServicoGateway();
        var request = ServicoUseCasesFixture.CriarCadastrarServicoUseCaseDtoIndisponivel();
        var servicoEsperado = ServicoUseCasesFixture.CriarServicoIndisponivel();

        _fixture.ConfigurarMockServicoGatewayParaCadastro(mockServicoGateway, servicoEsperado);

        var servicoUseCases = _fixture.CriarServicoUseCases(mockServicoGateway);

        // Act
        var resultado = await servicoUseCases.CadastrarServicoUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Disponivel.Should().BeFalse();
        resultado.Nome.Should().Be(request.Nome);
        resultado.Valor.Should().Be(request.Valor);
    }

    [Fact]
    public async Task CadastrarServicoUseCaseAsync_ComValorZero_DeveCadastrarComSucesso()
    {
        // Arrange
        var mockServicoGateway = _fixture.CriarMockServicoGateway();
        var request = ServicoUseCasesFixture.CriarCadastrarServicoUseCaseDtoComValorZero();

        _fixture.ConfigurarMockServicoGatewayParaCadastro(mockServicoGateway);

        var servicoUseCases = _fixture.CriarServicoUseCases(mockServicoGateway);

        // Act
        var resultado = await servicoUseCases.CadastrarServicoUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Valor.Should().Be(0.00m);
        resultado.Nome.Should().Be(request.Nome);
    }

    [Fact]
    public async Task AtualizarServicoUseCaseAsync_ComDadosValidos_DeveRetornarServicoAtualizado()
    {
        // Arrange
        var mockServicoGateway = _fixture.CriarMockServicoGateway();
        var mockUdt = _fixture.CriarMockUnidadeDeTrabalho();

        var servicoExistente = ServicoUseCasesFixture.CriarServicoValido();
        var request = ServicoUseCasesFixture.CriarEditarServicoUseCaseDtoValido();

        _fixture.ConfigurarMockServicoGatewayParaAtualizacao(mockServicoGateway, servicoExistente);

        var servicoUseCases = _fixture.CriarServicoUseCases(
            mockServicoGateway, null, mockUdt);

        // Act
        var resultado = await servicoUseCases.EditarServicoUseCaseAsync(servicoExistente.Id, request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(servicoExistente.Id);
        resultado.Nome.Should().Be(request.Nome);
        resultado.Descricao.Should().Be(request.Descricao);
        resultado.Valor.Should().Be(request.Valor);

        mockServicoGateway.Received(1).ObterPorIdAsync(servicoExistente.Id);
        mockServicoGateway.Received(1).EditarAsync(Arg.Any<Servico>());
        mockUdt.Received(1).Commit();
    }

    [Fact]
    public async Task AtualizarServicoUseCaseAsync_ComServicoInexistente_DeveLancarDadosNaoEncontradosException()
    {
        // Arrange
        var mockServicoGateway = _fixture.CriarMockServicoGateway();
        var servicoId = Guid.NewGuid();
        var request = ServicoUseCasesFixture.CriarEditarServicoUseCaseDtoValido();

        _fixture.ConfigurarMockServicoGatewayParaServicoNaoEncontrado(mockServicoGateway, servicoId);

        var servicoUseCases = _fixture.CriarServicoUseCases(mockServicoGateway);

        // Act & Assert
        await servicoUseCases
            .Invoking(x => x.EditarServicoUseCaseAsync(servicoId, request))
            .Should()
            .ThrowAsync<DadosNaoEncontradosException>()
            .WithMessage("Serviço não encontrado");

        mockServicoGateway.Received(1).ObterPorIdAsync(servicoId);
        mockServicoGateway.DidNotReceive().EditarAsync(Arg.Any<Servico>());
    }

    [Fact]
    public async Task ObterPorIdUseCaseAsync_ComIdValido_DeveRetornarServico()
    {
        // Arrange
        var mockServicoGateway = _fixture.CriarMockServicoGateway();
        var servicoExistente = ServicoUseCasesFixture.CriarServicoValido();

        mockServicoGateway
            .ObterPorIdAsync(servicoExistente.Id)
            .Returns(Task.FromResult(servicoExistente));

        var servicoUseCases = _fixture.CriarServicoUseCases(mockServicoGateway);

        // Act
        var resultado = await servicoUseCases.ObterServicoPorIdUseCaseAsync(servicoExistente.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(servicoExistente.Id);
        resultado.Nome.Should().Be(servicoExistente.Nome);
        resultado.Valor.Should().Be(servicoExistente.Valor);

        mockServicoGateway.Received(1).ObterPorIdAsync(servicoExistente.Id);
    }

    [Fact]
    public async Task ObterTodosUseCaseAsync_DeveRetornarListaDeServicos()
    {
        // Arrange
        var mockServicoGateway = _fixture.CriarMockServicoGateway();
        var servicosEsperados = ServicoUseCasesFixture.CriarListaServicosVariados();

        _fixture.ConfigurarMockServicoGatewayParaListagem(mockServicoGateway, servicosEsperados);

        var servicoUseCases = _fixture.CriarServicoUseCases(mockServicoGateway);

        // Act
        var resultado = await servicoUseCases.ObterTodosUseCaseAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(servicosEsperados.Count);
        resultado.Should().BeEquivalentTo(servicosEsperados);

        mockServicoGateway.Received(1).ObterTodosAsync();
    }

    [Fact]
    public async Task ObterServicosDisponiveisUseCaseAsync_DeveRetornarApenasServicosDisponiveis()
    {
        // Arrange
        var mockServicoGateway = _fixture.CriarMockServicoGateway();
        var todosServicos = ServicoUseCasesFixture.CriarListaServicosVariados();
        var servicosDisponiveis = todosServicos.Where(s => s.Disponivel).ToList();

        _fixture.ConfigurarMockServicoGatewayParaListagem(mockServicoGateway, todosServicos);

        var servicoUseCases = _fixture.CriarServicoUseCases(mockServicoGateway);

        // Act
        var resultado = await servicoUseCases.ObterServicosDisponiveisUseCaseAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(servicosDisponiveis.Count);
        resultado.Should().OnlyContain(s => s.Disponivel);

        mockServicoGateway.Received(1).ObterServicoDisponivelAsync();
    }

    [Fact]
    public async Task ObterServicoPorNomeUseCaseAsync_ComNomeExistente_DeveRetornarServico()
    {
        // Arrange
        var mockServicoGateway = _fixture.CriarMockServicoGateway();
        var servicoExistente = ServicoUseCasesFixture.CriarServicoValido();

        _fixture.ConfigurarMockServicoGatewayParaBusca(mockServicoGateway, servicoExistente.Nome, servicoExistente);

        var servicoUseCases = _fixture.CriarServicoUseCases(mockServicoGateway);

        // Act
        var resultado = await servicoUseCases.ObterServicoPorNomeUseCaseAsync(servicoExistente.Nome);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Nome.Should().Be(servicoExistente.Nome);
        resultado.Id.Should().Be(servicoExistente.Id);

        mockServicoGateway.Received(1).ObterServicosDisponiveisPorNomeAsync(servicoExistente.Nome);
    }

    [Fact]
    public async Task ObterServicoPorNomeUseCaseAsync_ComNomeInexistente_DeveRetornarNull()
    {
        // Arrange
        var mockServicoGateway = _fixture.CriarMockServicoGateway();
        var nomeInexistente = "Serviço Inexistente";

        _fixture.ConfigurarMockServicoGatewayParaBusca(mockServicoGateway, nomeInexistente, null);

        var servicoUseCases = _fixture.CriarServicoUseCases(mockServicoGateway);

        // Act
        var resultado = await servicoUseCases.ObterServicoPorNomeUseCaseAsync(nomeInexistente);

        // Assert
        resultado.Should().BeNull();
        mockServicoGateway.Received(1).ObterServicosDisponiveisPorNomeAsync(nomeInexistente);
    }

    [Fact]
    public async Task ExcluirUseCaseAsync_ComServicoExistente_DeveExcluirComSucesso()
    {
        // Arrange
        var mockServicoGateway = _fixture.CriarMockServicoGateway();
        var mockUdt = _fixture.CriarMockUnidadeDeTrabalho();
        var servicoExistente = ServicoUseCasesFixture.CriarServicoValido();

        _fixture.ConfigurarMockServicoGatewayParaExclusao(mockServicoGateway, servicoExistente);

        var servicoUseCases = _fixture.CriarServicoUseCases(
            mockServicoGateway, null, mockUdt);

        // Act
        await servicoUseCases.DeletarServicoUseCaseAsync(servicoExistente.Id);

        // Assert
        mockServicoGateway.Received(1).ObterPorIdAsync(servicoExistente.Id);
        mockServicoGateway.Received(1).DeletarAsync(Arg.Any<Servico>());
        mockUdt.Received(1).Commit();
    }

    [Fact]
    public async Task ExcluirUseCaseAsync_ComServicoInexistente_DeveLancarDadosNaoEncontradosException()
    {
        // Arrange
        var mockServicoGateway = _fixture.CriarMockServicoGateway();
        var servicoId = Guid.NewGuid();

        _fixture.ConfigurarMockServicoGatewayParaServicoNaoEncontrado(mockServicoGateway, servicoId);

        var servicoUseCases = _fixture.CriarServicoUseCases(mockServicoGateway);

        // Act & Assert
        await servicoUseCases
            .Invoking(x => x.DeletarServicoUseCaseAsync(servicoId))
            .Should()
            .ThrowAsync<DadosNaoEncontradosException>()
            .WithMessage("Serviço não encontrado");

        mockServicoGateway.Received(1).ObterPorIdAsync(servicoId);
        mockServicoGateway.DidNotReceive().DeletarAsync(Arg.Any<Servico>());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CadastrarServicoUseCaseAsync_ComDiferentesDisponibilidades_DeveCadastrarCorretamente(bool disponivel)
    {
        // Arrange
        var mockServicoGateway = _fixture.CriarMockServicoGateway();
        var request = ServicoUseCasesFixture.CriarCadastrarServicoUseCaseDtoValido();
        request.Disponivel = disponivel;

        var servicoEsperado = ServicoUseCasesFixture.CriarServicoValido();
        servicoEsperado.Disponivel = disponivel;

        _fixture.ConfigurarMockServicoGatewayParaCadastro(mockServicoGateway, servicoEsperado);

        var servicoUseCases = _fixture.CriarServicoUseCases(mockServicoGateway);

        // Act
        var resultado = await servicoUseCases.CadastrarServicoUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Disponivel.Should().Be(disponivel);
        resultado.Nome.Should().Be(request.Nome);
    }

    [Theory]
    [InlineData(0.00)]
    [InlineData(50.99)]
    [InlineData(1500.00)]
    [InlineData(99999.99)]
    public async Task CadastrarServicoUseCaseAsync_ComDiferentesValores_DeveCadastrarCorretamente(decimal valor)
    {
        // Arrange
        var mockServicoGateway = _fixture.CriarMockServicoGateway();
        var request = ServicoUseCasesFixture.CriarCadastrarServicoUseCaseDtoValido();
        request.Valor = valor;

        var servicoEsperado = ServicoUseCasesFixture.CriarServicoValido();
        servicoEsperado.Valor = valor;

        _fixture.ConfigurarMockServicoGatewayParaCadastro(mockServicoGateway, servicoEsperado);

        var servicoUseCases = _fixture.CriarServicoUseCases(mockServicoGateway);

        // Act
        var resultado = await servicoUseCases.CadastrarServicoUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Valor.Should().Be(valor);
        resultado.Nome.Should().Be(request.Nome);
    }

    [Fact]
    public async Task AtualizarServicoUseCaseAsync_AlterandoDisponibilidade_DeveAtualizarCorretamente()
    {
        // Arrange
        var mockServicoGateway = _fixture.CriarMockServicoGateway();
        var servicoExistente = ServicoUseCasesFixture.CriarServicoValido();
        var request = ServicoUseCasesFixture.CriarEditarServicoUseCaseDtoParaIndisponivel();

        _fixture.ConfigurarMockServicoGatewayParaAtualizacao(mockServicoGateway, servicoExistente);

        var servicoUseCases = _fixture.CriarServicoUseCases(mockServicoGateway);

        // Act
        var resultado = await servicoUseCases.EditarServicoUseCaseAsync(servicoExistente.Id, request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Disponivel.Should().Be(request.Disponivel.Value);
        resultado.Nome.Should().Be(request.Nome);
    }

    [Fact]
    public void Constructor_ComParametrosNulos_DeveLancarArgumentNullException()
    {
        // Arrange
        var mockUdt = _fixture.CriarMockUnidadeDeTrabalho();
        var mockUsuarioLogado = _fixture.CriarMockUsuarioLogadoServico();
        var mockServicoGateway = _fixture.CriarMockServicoGateway();
        var mockLogServico = _fixture.CriarMockLogServico<ServicoUseCases>();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => 
            new ServicoUseCases(
                null!,
                mockUdt,
                mockUsuarioLogado,
                mockServicoGateway));
                
        Assert.Equal("logServico", exception.ParamName);
    }
}
