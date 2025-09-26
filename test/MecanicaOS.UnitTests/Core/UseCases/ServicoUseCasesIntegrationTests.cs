using Core.Entidades;
using Core.Exceptions;
using MecanicaOS.UnitTests.Fixtures.UseCases;

namespace MecanicaOS.UnitTests.Core.UseCases;

/// <summary>
/// Testes de integração para ServicoUseCases focando no comportamento da interface
/// após a migração para handlers individuais
/// </summary>
public class ServicoUseCasesIntegrationTests
{
    private readonly ServicoUseCasesFixture _fixture;

    public ServicoUseCasesIntegrationTests()
    {
        _fixture = new ServicoUseCasesFixture();
    }

    [Fact]
    public async Task CadastrarServicoUseCaseAsync_ComDadosValidos_DeveRetornarServicoCadastrado()
    {
        // Arrange
        var request = ServicoUseCasesFixture.CriarCadastrarServicoUseCaseDtoValido();
        var servicoEsperado = ServicoUseCasesFixture.CriarServicoValido();

        var servicoUseCases = _fixture.CriarServicoUseCases();
        servicoUseCases.CadastrarServicoUseCaseAsync(request).Returns(servicoEsperado);

        // Act
        var resultado = await servicoUseCases.CadastrarServicoUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Nome.Should().Be(servicoEsperado.Nome);
        resultado.Descricao.Should().Be(servicoEsperado.Descricao);
        resultado.Valor.Should().Be(servicoEsperado.Valor);
        resultado.Disponivel.Should().Be(servicoEsperado.Disponivel);

        await servicoUseCases.Received(1).CadastrarServicoUseCaseAsync(request);
    }

    [Fact]
    public async Task CadastrarServicoUseCaseAsync_ComNomeJaCadastrado_DeveLancarDadosJaCadastradosException()
    {
        // Arrange
        var request = ServicoUseCasesFixture.CriarCadastrarServicoUseCaseDtoValido();

        var servicoUseCases = _fixture.CriarServicoUseCases();
        servicoUseCases.CadastrarServicoUseCaseAsync(request)
            .Returns<Servico>(x => throw new DadosJaCadastradosException("Serviço já cadastrado"));

        // Act & Assert
        await servicoUseCases.Invoking(x => x.CadastrarServicoUseCaseAsync(request))
            .Should().ThrowAsync<DadosJaCadastradosException>()
            .WithMessage("Serviço já cadastrado");
    }

    [Fact]
    public async Task EditarServicoUseCaseAsync_ComDadosValidos_DeveRetornarServicoAtualizado()
    {
        // Arrange
        var servicoId = Guid.NewGuid();
        var request = ServicoUseCasesFixture.CriarEditarServicoUseCaseDtoValido();
        var servicoEsperado = ServicoUseCasesFixture.CriarServicoValido();

        var servicoUseCases = _fixture.CriarServicoUseCases();
        servicoUseCases.EditarServicoUseCaseAsync(servicoId, request).Returns(servicoEsperado);

        // Act
        var resultado = await servicoUseCases.EditarServicoUseCaseAsync(servicoId, request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().Be(servicoEsperado);

        await servicoUseCases.Received(1).EditarServicoUseCaseAsync(servicoId, request);
    }

    [Fact]
    public async Task EditarServicoUseCaseAsync_ComServicoInexistente_DeveLancarDadosNaoEncontradosException()
    {
        // Arrange
        var servicoId = Guid.NewGuid();
        var request = ServicoUseCasesFixture.CriarEditarServicoUseCaseDtoValido();

        var servicoUseCases = _fixture.CriarServicoUseCases();
        servicoUseCases.EditarServicoUseCaseAsync(servicoId, request)
            .Returns<Servico>(x => throw new DadosNaoEncontradosException("Serviço não encontrado"));

        // Act & Assert
        await servicoUseCases.Invoking(x => x.EditarServicoUseCaseAsync(servicoId, request))
            .Should().ThrowAsync<DadosNaoEncontradosException>()
            .WithMessage("Serviço não encontrado");
    }

    [Fact]
    public async Task ObterServicoPorIdUseCaseAsync_ComIdExistente_DeveRetornarServico()
    {
        // Arrange
        var servicoId = Guid.NewGuid();
        var servicoEsperado = ServicoUseCasesFixture.CriarServicoValido();

        var servicoUseCases = _fixture.CriarServicoUseCases();
        servicoUseCases.ObterServicoPorIdUseCaseAsync(servicoId).Returns(servicoEsperado);

        // Act
        var resultado = await servicoUseCases.ObterServicoPorIdUseCaseAsync(servicoId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().Be(servicoEsperado);

        await servicoUseCases.Received(1).ObterServicoPorIdUseCaseAsync(servicoId);
    }

    [Fact]
    public async Task ObterServicoPorIdUseCaseAsync_ComIdInexistente_DeveLancarDadosNaoEncontradosException()
    {
        // Arrange
        var servicoId = Guid.NewGuid();

        var servicoUseCases = _fixture.CriarServicoUseCases();
        servicoUseCases.ObterServicoPorIdUseCaseAsync(servicoId)
            .Returns<Servico>(x => throw new DadosNaoEncontradosException("Serviço não encontrado"));

        // Act & Assert
        await servicoUseCases.Invoking(x => x.ObterServicoPorIdUseCaseAsync(servicoId))
            .Should().ThrowAsync<DadosNaoEncontradosException>()
            .WithMessage("Serviço não encontrado");
    }

    [Fact]
    public async Task ObterTodosUseCaseAsync_DeveRetornarListaDeServicos()
    {
        // Arrange
        var servicosEsperados = ServicoUseCasesFixture.CriarListaServicosVariados();

        var servicoUseCases = _fixture.CriarServicoUseCases();
        servicoUseCases.ObterTodosUseCaseAsync().Returns(servicosEsperados);

        // Act
        var resultado = await servicoUseCases.ObterTodosUseCaseAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(servicosEsperados.Count);
        resultado.Should().BeEquivalentTo(servicosEsperados);

        await servicoUseCases.Received(1).ObterTodosUseCaseAsync();
    }

    [Fact]
    public async Task DeletarServicoUseCaseAsync_ComIdExistente_DeveExecutarSemErros()
    {
        // Arrange
        var servicoId = Guid.NewGuid();

        var servicoUseCases = _fixture.CriarServicoUseCases();
        servicoUseCases.DeletarServicoUseCaseAsync(servicoId).Returns(Task.FromResult(true));

        // Act
        await servicoUseCases.DeletarServicoUseCaseAsync(servicoId);

        // Assert
        await servicoUseCases.Received(1).DeletarServicoUseCaseAsync(servicoId);
    }

    [Fact]
    public async Task ObterServicoPorNomeUseCaseAsync_ComNomeExistente_DeveRetornarServico()
    {
        // Arrange
        var nomeServico = "Troca de Óleo";
        var servicoEsperado = ServicoUseCasesFixture.CriarServicoValido();

        var servicoUseCases = _fixture.CriarServicoUseCases();
        servicoUseCases.ObterServicoPorNomeUseCaseAsync(nomeServico).Returns(servicoEsperado);

        // Act
        var resultado = await servicoUseCases.ObterServicoPorNomeUseCaseAsync(nomeServico);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().Be(servicoEsperado);

        await servicoUseCases.Received(1).ObterServicoPorNomeUseCaseAsync(nomeServico);
    }

    [Fact]
    public async Task ObterServicoPorNomeUseCaseAsync_ComNomeInexistente_DeveRetornarNull()
    {
        // Arrange
        var nomeServico = "Serviço Inexistente";

        var servicoUseCases = _fixture.CriarServicoUseCases();
        servicoUseCases.ObterServicoPorNomeUseCaseAsync(nomeServico).Returns((Servico?)null);

        // Act
        var resultado = await servicoUseCases.ObterServicoPorNomeUseCaseAsync(nomeServico);

        // Assert
        resultado.Should().BeNull();

        await servicoUseCases.Received(1).ObterServicoPorNomeUseCaseAsync(nomeServico);
    }

    [Fact]
    public async Task ObterServicosDisponiveisUseCaseAsync_DeveRetornarApenasServicosDisponiveis()
    {
        // Arrange
        var servicosDisponiveis = ServicoUseCasesFixture.CriarListaServicosVariados()
            .Where(s => s.Disponivel).ToList();

        var servicoUseCases = _fixture.CriarServicoUseCases();
        servicoUseCases.ObterServicosDisponiveisUseCaseAsync().Returns(servicosDisponiveis);

        // Act
        var resultado = await servicoUseCases.ObterServicosDisponiveisUseCaseAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(servicosDisponiveis.Count);
        resultado.Should().OnlyContain(s => s.Disponivel);
        resultado.Should().BeEquivalentTo(servicosDisponiveis);

        await servicoUseCases.Received(1).ObterServicosDisponiveisUseCaseAsync();
    }
}
