using Core.DTOs.UseCases.OrdemServico;
using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
using Core.Interfaces.UseCases;
using MecanicaOS.UnitTests.Fixtures.UseCases;

namespace MecanicaOS.UnitTests.Core.UseCases;

/// <summary>
/// Testes de integração para OrdemServicoUseCases focando no comportamento da interface
/// após a migração para handlers individuais
/// </summary>
public class OrdemServicoUseCasesIntegrationTests
{
    private readonly OrdemServicoUseCasesFixture _fixture;

    public OrdemServicoUseCasesIntegrationTests()
    {
        _fixture = new OrdemServicoUseCasesFixture();
    }

    [Fact]
    public async Task CadastrarUseCaseAsync_ComDadosValidos_DeveRetornarOrdemServicoCadastrada()
    {
        // Arrange
        var request = OrdemServicoUseCasesFixture.CriarCadastrarOrdemServicoUseCaseDtoValido();
        var ordemServicoEsperada = OrdemServicoUseCasesFixture.CriarOrdemServicoValida();

        var ordemServicoUseCases = _fixture.CriarOrdemServicoUseCases();
        ordemServicoUseCases.CadastrarUseCaseAsync(request).Returns(ordemServicoEsperada);

        // Act
        var resultado = await ordemServicoUseCases.CadastrarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.ClienteId.Should().Be(ordemServicoEsperada.ClienteId);
        resultado.VeiculoId.Should().Be(ordemServicoEsperada.VeiculoId);
        resultado.ServicoId.Should().Be(ordemServicoEsperada.ServicoId);
        resultado.Status.Should().Be(ordemServicoEsperada.Status);

        await ordemServicoUseCases.Received(1).CadastrarUseCaseAsync(request);
    }

    [Fact]
    public async Task CadastrarUseCaseAsync_ComClienteInexistente_DeveLancarDadosNaoEncontradosException()
    {
        // Arrange
        var request = OrdemServicoUseCasesFixture.CriarCadastrarOrdemServicoUseCaseDtoValido();

        var ordemServicoUseCases = _fixture.CriarOrdemServicoUseCases();
        ordemServicoUseCases.CadastrarUseCaseAsync(request)
            .Returns<OrdemServico>(x => throw new DadosNaoEncontradosException("Cliente não encontrado"));

        // Act & Assert
        await ordemServicoUseCases.Invoking(x => x.CadastrarUseCaseAsync(request))
            .Should().ThrowAsync<DadosNaoEncontradosException>()
            .WithMessage("Cliente não encontrado");
    }

    [Fact]
    public async Task AtualizarUseCaseAsync_ComDadosValidos_DeveRetornarOrdemServicoAtualizada()
    {
        // Arrange
        var ordemServicoId = Guid.NewGuid();
        var request = OrdemServicoUseCasesFixture.CriarAtualizarOrdemServicoUseCaseDtoValido();
        var ordemServicoEsperada = OrdemServicoUseCasesFixture.CriarOrdemServicoValida();

        var ordemServicoUseCases = _fixture.CriarOrdemServicoUseCases();
        ordemServicoUseCases.AtualizarUseCaseAsync(ordemServicoId, request).Returns(ordemServicoEsperada);

        // Act
        var resultado = await ordemServicoUseCases.AtualizarUseCaseAsync(ordemServicoId, request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().Be(ordemServicoEsperada);

        await ordemServicoUseCases.Received(1).AtualizarUseCaseAsync(ordemServicoId, request);
    }

    [Fact]
    public async Task AtualizarUseCaseAsync_ComOrdemServicoInexistente_DeveLancarDadosNaoEncontradosException()
    {
        // Arrange
        var ordemServicoId = Guid.NewGuid();
        var request = OrdemServicoUseCasesFixture.CriarAtualizarOrdemServicoUseCaseDtoValido();

        var ordemServicoUseCases = _fixture.CriarOrdemServicoUseCases();
        ordemServicoUseCases.AtualizarUseCaseAsync(ordemServicoId, request)
            .Returns<OrdemServico>(x => throw new DadosNaoEncontradosException("Ordem de serviço não encontrada"));

        // Act & Assert
        await ordemServicoUseCases.Invoking(x => x.AtualizarUseCaseAsync(ordemServicoId, request))
            .Should().ThrowAsync<DadosNaoEncontradosException>()
            .WithMessage("Ordem de serviço não encontrada");
    }

    [Fact]
    public async Task ObterPorIdUseCaseAsync_ComIdExistente_DeveRetornarOrdemServico()
    {
        // Arrange
        var ordemServicoId = Guid.NewGuid();
        var ordemServicoEsperada = OrdemServicoUseCasesFixture.CriarOrdemServicoValida();

        var ordemServicoUseCases = _fixture.CriarOrdemServicoUseCases();
        ordemServicoUseCases.ObterPorIdUseCaseAsync(ordemServicoId).Returns(ordemServicoEsperada);

        // Act
        var resultado = await ordemServicoUseCases.ObterPorIdUseCaseAsync(ordemServicoId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().Be(ordemServicoEsperada);

        await ordemServicoUseCases.Received(1).ObterPorIdUseCaseAsync(ordemServicoId);
    }

    [Fact]
    public async Task ObterTodosUseCaseAsync_DeveRetornarListaDeOrdensServico()
    {
        // Arrange
        var ordensServicoEsperadas = OrdemServicoUseCasesFixture.CriarListaOrdensServicoVariadas();

        var ordemServicoUseCases = _fixture.CriarOrdemServicoUseCases();
        ordemServicoUseCases.ObterTodosUseCaseAsync().Returns(ordensServicoEsperadas);

        // Act
        var resultado = await ordemServicoUseCases.ObterTodosUseCaseAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(ordensServicoEsperadas.Count);
        resultado.Should().BeEquivalentTo(ordensServicoEsperadas);

        await ordemServicoUseCases.Received(1).ObterTodosUseCaseAsync();
    }

    [Theory]
    [InlineData(StatusOrdemServico.Recebida)]
    [InlineData(StatusOrdemServico.EmDiagnostico)]
    [InlineData(StatusOrdemServico.AguardandoAprovação)]
    [InlineData(StatusOrdemServico.EmExecucao)]
    [InlineData(StatusOrdemServico.Finalizada)]
    [InlineData(StatusOrdemServico.Cancelada)]
    public async Task ObterPorStatusUseCaseAsync_ComStatusValido_DeveRetornarOrdensComStatus(StatusOrdemServico status)
    {
        // Arrange
        var ordensServicoEsperadas = OrdemServicoUseCasesFixture.CriarListaOrdensServicoVariadas()
            .Where(os => os.Status == status).ToList();

        var ordemServicoUseCases = _fixture.CriarOrdemServicoUseCases();
        ordemServicoUseCases.ObterPorStatusUseCaseAsync(status).Returns(ordensServicoEsperadas);

        // Act
        var resultado = await ordemServicoUseCases.ObterPorStatusUseCaseAsync(status);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().OnlyContain(os => os.Status == status);
        resultado.Should().BeEquivalentTo(ordensServicoEsperadas);

        await ordemServicoUseCases.Received(1).ObterPorStatusUseCaseAsync(status);
    }

    [Fact]
    public async Task AceitarOrcamentoUseCaseAsync_ComOrdemServicoValida_DeveAtualizarStatusParaEmExecucao()
    {
        // Arrange
        var ordemServicoId = Guid.NewGuid();

        var ordemServicoUseCases = _fixture.CriarOrdemServicoUseCases();
        ordemServicoUseCases.AceitarOrcamentoUseCaseAsync(ordemServicoId).Returns(Task.CompletedTask);

        // Act
        await ordemServicoUseCases.AceitarOrcamentoUseCaseAsync(ordemServicoId);

        // Assert
        await ordemServicoUseCases.Received(1).AceitarOrcamentoUseCaseAsync(ordemServicoId);
    }

    [Fact]
    public async Task RecusarOrcamentoUseCaseAsync_ComOrdemServicoValida_DeveAtualizarStatusParaCancelada()
    {
        // Arrange
        var ordemServicoId = Guid.NewGuid();

        var ordemServicoUseCases = _fixture.CriarOrdemServicoUseCases();
        ordemServicoUseCases.RecusarOrcamentoUseCaseAsync(ordemServicoId).Returns(Task.CompletedTask);

        // Act
        await ordemServicoUseCases.RecusarOrcamentoUseCaseAsync(ordemServicoId);

        // Assert
        await ordemServicoUseCases.Received(1).RecusarOrcamentoUseCaseAsync(ordemServicoId);
    }

    [Fact]
    public async Task AceitarOrcamentoUseCaseAsync_ComOrcamentoExpirado_DeveLancarOrcamentoExpiradoException()
    {
        // Arrange
        var ordemServicoId = Guid.NewGuid();

        var ordemServicoUseCases = _fixture.CriarOrdemServicoUseCases();
        ordemServicoUseCases.AceitarOrcamentoUseCaseAsync(ordemServicoId)
            .Returns(Task.FromException(new OrcamentoExpiradoException("Orçamento expirado")));

        // Act & Assert
        await ordemServicoUseCases.Invoking(x => x.AceitarOrcamentoUseCaseAsync(ordemServicoId))
            .Should().ThrowAsync<OrcamentoExpiradoException>()
            .WithMessage("Orçamento expirado");
    }
}
