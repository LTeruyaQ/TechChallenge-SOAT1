using Core.DTOs.UseCases.Eventos;
using Core.DTOs.UseCases.OrdemServico;
using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;
using Core.UseCases;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.UseCases;
using NSubstitute;
using Xunit;

namespace MecanicaOS.UnitTests.Core.UseCases;

public class OrdemServicoUseCasesUnitTests
{
    private readonly OrdemServicoUseCasesFixture _fixture;

    public OrdemServicoUseCasesUnitTests()
    {
        _fixture = new OrdemServicoUseCasesFixture();
    }

    [Fact]
    public async Task CadastrarUseCaseAsync_ComDadosValidos_DeveRetornarOrdemServicoCadastrada()
    {
        // Arrange
        var mockOrdemServicoGateway = _fixture.CriarMockOrdemServicoGateway();
        var mockClienteGateway = _fixture.CriarMockClienteGateway();
        var mockServicoUseCases = _fixture.CriarMockServicoUseCases();
        var mockUdt = _fixture.CriarMockUnidadeDeTrabalho();

        var request = OrdemServicoUseCasesFixture.CriarCadastrarOrdemServicoUseCaseDtoValido();
        var clienteComVeiculo = OrdemServicoUseCasesFixture.CriarClienteComVeiculo();
        var servicoDisponivel = OrdemServicoUseCasesFixture.CriarServicoDisponivel();
        var ordemServicoEsperada = OrdemServicoUseCasesFixture.CriarOrdemServicoValida();

        // Ajustar IDs para corresponder ao request
        clienteComVeiculo.Id = request.ClienteId;
        clienteComVeiculo.Veiculos.First().Id = request.VeiculoId;
        servicoDisponivel.Id = request.ServicoId;

        _fixture.ConfigurarMockClienteGatewayParaVerificacao(mockClienteGateway, request.ClienteId, clienteComVeiculo);
        _fixture.ConfigurarMockServicoUseCasesParaVerificacao(mockServicoUseCases, request.ServicoId, servicoDisponivel);
        _fixture.ConfigurarMockOrdemServicoGatewayParaCadastro(mockOrdemServicoGateway, ordemServicoEsperada);

        var ordemServicoUseCases = _fixture.CriarOrdemServicoUseCases(
            mockOrdemServicoGateway, mockClienteGateway, mockServicoUseCases, null, mockUdt);

        // Act
        var resultado = await ordemServicoUseCases.CadastrarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.ClienteId.Should().NotBeEmpty();
        resultado.VeiculoId.Should().NotBeEmpty();
        resultado.ServicoId.Should().NotBeEmpty();
        resultado.Descricao.Should().Be(request.Descricao);
        resultado.Status.Should().Be(StatusOrdemServico.Recebida);

        mockClienteGateway.Received(1).ObterClienteComVeiculoPorIdAsync(request.ClienteId);
        mockServicoUseCases.Received(1).ObterServicoPorIdUseCaseAsync(request.ServicoId);
        mockOrdemServicoGateway.Received(1).CadastrarAsync(Arg.Any<OrdemServico>());
        mockUdt.Received(1).Commit();
    }

    [Fact]
    public async Task CadastrarUseCaseAsync_ComClienteInexistente_DeveLancarDadosNaoEncontradosException()
    {
        // Arrange
        var mockClienteGateway = _fixture.CriarMockClienteGateway();
        var request = OrdemServicoUseCasesFixture.CriarCadastrarOrdemServicoUseCaseDtoValido();

        _fixture.ConfigurarMockClienteGatewayParaClienteNaoEncontrado(mockClienteGateway, request.ClienteId);

        var ordemServicoUseCases = _fixture.CriarOrdemServicoUseCases(
            null, mockClienteGateway);

        // Act & Assert
        await ordemServicoUseCases
            .Invoking(x => x.CadastrarUseCaseAsync(request))
            .Should()
            .ThrowAsync<DadosNaoEncontradosException>()
            .WithMessage("O cliente não foi encontrado");

        mockClienteGateway.Received(1).ObterClienteComVeiculoPorIdAsync(request.ClienteId);
    }

    [Fact]
    public async Task CadastrarUseCaseAsync_ComVeiculoInexistente_DeveLancarDadosNaoEncontradosException()
    {
        // Arrange
        var mockClienteGateway = _fixture.CriarMockClienteGateway();
        var request = OrdemServicoUseCasesFixture.CriarCadastrarOrdemServicoUseCaseDtoValido();
        var clienteComVeiculo = OrdemServicoUseCasesFixture.CriarClienteComVeiculo();

        // Cliente existe mas veículo tem ID diferente
        clienteComVeiculo.Id = request.ClienteId;
        clienteComVeiculo.Veiculos.First().Id = Guid.NewGuid(); // ID diferente do request

        _fixture.ConfigurarMockClienteGatewayParaVerificacao(mockClienteGateway, request.ClienteId, clienteComVeiculo);

        var ordemServicoUseCases = _fixture.CriarOrdemServicoUseCases(
            null, mockClienteGateway);

        // Act & Assert
        await ordemServicoUseCases
            .Invoking(x => x.CadastrarUseCaseAsync(request))
            .Should()
            .ThrowAsync<DadosNaoEncontradosException>()
            .WithMessage("O veículo não foi encontrado");

        mockClienteGateway.Received(1).ObterClienteComVeiculoPorIdAsync(request.ClienteId);
    }

    [Fact]
    public async Task CadastrarUseCaseAsync_ComServicoIndisponivel_DeveLancarServicoIndisponivelException()
    {
        // Arrange
        var mockClienteGateway = _fixture.CriarMockClienteGateway();
        var mockServicoUseCases = _fixture.CriarMockServicoUseCases();
        var request = OrdemServicoUseCasesFixture.CriarCadastrarOrdemServicoUseCaseDtoValido();
        var clienteComVeiculo = OrdemServicoUseCasesFixture.CriarClienteComVeiculo();
        var servicoIndisponivel = OrdemServicoUseCasesFixture.CriarServicoIndisponivel();

        clienteComVeiculo.Id = request.ClienteId;
        clienteComVeiculo.Veiculos.First().Id = request.VeiculoId;
        servicoIndisponivel.Id = request.ServicoId;

        _fixture.ConfigurarMockClienteGatewayParaVerificacao(mockClienteGateway, request.ClienteId, clienteComVeiculo);
        _fixture.ConfigurarMockServicoUseCasesParaVerificacao(mockServicoUseCases, request.ServicoId, servicoIndisponivel);

        var ordemServicoUseCases = _fixture.CriarOrdemServicoUseCases(
            null, mockClienteGateway, mockServicoUseCases);

        // Act & Assert
        await ordemServicoUseCases
            .Invoking(x => x.CadastrarUseCaseAsync(request))
            .Should()
            .ThrowAsync<ServicoIndisponivelException>()
            .WithMessage("Serviço indisponível");

        mockServicoUseCases.Received(1).ObterServicoPorIdUseCaseAsync(request.ServicoId);
    }

    [Fact]
    public async Task AtualizarUseCaseAsync_ComDadosValidos_DeveRetornarOrdemServicoAtualizada()
    {
        // Arrange
        var mockOrdemServicoGateway = _fixture.CriarMockOrdemServicoGateway();
        var mockUdt = _fixture.CriarMockUnidadeDeTrabalho();

        var ordemServicoExistente = OrdemServicoUseCasesFixture.CriarOrdemServicoValida();
        var request = OrdemServicoUseCasesFixture.CriarAtualizarOrdemServicoUseCaseDtoValido();

        _fixture.ConfigurarMockOrdemServicoGatewayParaAtualizacao(mockOrdemServicoGateway, ordemServicoExistente);

        var ordemServicoUseCases = _fixture.CriarOrdemServicoUseCases(
            mockOrdemServicoGateway, null, null, null, mockUdt);

        // Act
        var resultado = await ordemServicoUseCases.AtualizarUseCaseAsync(ordemServicoExistente.Id, request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(ordemServicoExistente.Id);
        resultado.ClienteId.Should().Be(request.ClienteId.ToString());
        resultado.VeiculoId.Should().Be(request.VeiculoId.ToString());
        resultado.ServicoId.Should().Be(request.ServicoId.ToString());
        resultado.Descricao.Should().Be(request.Descricao);
        resultado.Status.Should().Be(request.Status);

        await mockOrdemServicoGateway.Received(1).ObterPorIdAsync(ordemServicoExistente.Id);
        await mockOrdemServicoGateway.Received(1).EditarAsync(Arg.Any<OrdemServico>());
        mockUdt.Received(1).Commit();
    }

    [Fact]
    public async Task AtualizarUseCaseAsync_ComOrdemServicoInexistente_DeveLancarDadosNaoEncontradosException()
    {
        // Arrange
        var mockOrdemServicoGateway = _fixture.CriarMockOrdemServicoGateway();
        var ordemServicoId = Guid.NewGuid();
        var request = OrdemServicoUseCasesFixture.CriarAtualizarOrdemServicoUseCaseDtoValido();

        _fixture.ConfigurarMockOrdemServicoGatewayParaOrdemNaoEncontrada(mockOrdemServicoGateway, ordemServicoId);

        var ordemServicoUseCases = _fixture.CriarOrdemServicoUseCases(mockOrdemServicoGateway);

        // Act & Assert
        await ordemServicoUseCases
            .Invoking(x => x.AtualizarUseCaseAsync(ordemServicoId, request))
            .Should()
            .ThrowAsync<DadosNaoEncontradosException>()
            .WithMessage("Ordem de serviço não encontrada");

        mockOrdemServicoGateway.Received(1).ObterPorIdAsync(ordemServicoId);
        mockOrdemServicoGateway.DidNotReceive().EditarAsync(Arg.Any<OrdemServico>());
    }

    [Fact]
    public async Task AtualizarUseCaseAsync_ComStatusEmDiagnostico_DevePublicarEventoOrcamento()
    {
        // Arrange
        var mockOrdemServicoGateway = _fixture.CriarMockOrdemServicoGateway();
        var mockEventosGateway = _fixture.CriarMockEventosGateway();
        var mockUdt = _fixture.CriarMockUnidadeDeTrabalho();

        var ordemServicoExistente = OrdemServicoUseCasesFixture.CriarOrdemServicoValida();
        var request = OrdemServicoUseCasesFixture.CriarAtualizarOrdemServicoUseCaseDtoValido();
        request.Status = StatusOrdemServico.EmDiagnostico;

        _fixture.ConfigurarMockOrdemServicoGatewayParaAtualizacao(mockOrdemServicoGateway, ordemServicoExistente);
        _fixture.ConfigurarMockEventosGateway(mockEventosGateway);

        var ordemServicoUseCases = _fixture.CriarOrdemServicoUseCases(
            mockOrdemServicoGateway, null, null, mockEventosGateway, mockUdt);

        // Act
        var resultado = await ordemServicoUseCases.AtualizarUseCaseAsync(ordemServicoExistente.Id, request);

        // Assert
        resultado.Status.Should().Be(StatusOrdemServico.EmDiagnostico);
        mockEventosGateway.Received(1).Publicar(Arg.Any<OrdemServicoEmOrcamentoEventDTO>());
    }

    [Fact]
    public async Task ObterPorIdUseCaseAsync_ComIdValido_DeveRetornarOrdemServico()
    {
        // Arrange
        var mockOrdemServicoGateway = _fixture.CriarMockOrdemServicoGateway();
        var ordemServicoExistente = OrdemServicoUseCasesFixture.CriarOrdemServicoValida();

        mockOrdemServicoGateway
            .ObterOrdemServicoPorIdComInsumos(ordemServicoExistente.Id)
            .Returns(Task.FromResult(ordemServicoExistente));

        var ordemServicoUseCases = _fixture.CriarOrdemServicoUseCases(mockOrdemServicoGateway);

        // Act
        var resultado = await ordemServicoUseCases.ObterPorIdUseCaseAsync(ordemServicoExistente.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(ordemServicoExistente.Id);
        resultado.Descricao.Should().Be(ordemServicoExistente.Descricao);

        mockOrdemServicoGateway.Received(1).ObterOrdemServicoPorIdComInsumos(ordemServicoExistente.Id);
    }

    [Fact]
    public async Task ObterTodosUseCaseAsync_DeveRetornarListaDeOrdensServico()
    {
        // Arrange
        var mockOrdemServicoGateway = _fixture.CriarMockOrdemServicoGateway();
        var ordensServicoEsperadas = OrdemServicoUseCasesFixture.CriarListaOrdensServicoVariadas();

        _fixture.ConfigurarMockOrdemServicoGatewayParaListagem(mockOrdemServicoGateway, ordensServicoEsperadas);

        var ordemServicoUseCases = _fixture.CriarOrdemServicoUseCases(mockOrdemServicoGateway);

        // Act
        var resultado = await ordemServicoUseCases.ObterTodosUseCaseAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(ordensServicoEsperadas.Count);
        resultado.Should().BeEquivalentTo(ordensServicoEsperadas);

        mockOrdemServicoGateway.Received(1).ObterTodosAsync();
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
        var mockOrdemServicoGateway = _fixture.CriarMockOrdemServicoGateway();
        var ordensServico = OrdemServicoUseCasesFixture.CriarListaOrdensServicoVariadas()
            .Where(os => os.Status == status).ToList();

        _fixture.ConfigurarMockOrdemServicoGatewayParaStatus(mockOrdemServicoGateway, status, ordensServico);

        var ordemServicoUseCases = _fixture.CriarOrdemServicoUseCases(mockOrdemServicoGateway);

        // Act
        var resultado = await ordemServicoUseCases.ObterPorStatusUseCaseAsync(status);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().OnlyContain(os => os.Status == status);

        mockOrdemServicoGateway.Received(1).ObterOrdemServicoPorStatusAsync(status);
    }

    [Fact]
    public async Task AceitarOrcamentoUseCaseAsync_ComOrdemServicoValida_DeveAtualizarStatusParaEmExecucao()
    {
        // Arrange
        var mockOrdemServicoGateway = _fixture.CriarMockOrdemServicoGateway();
        var mockUdt = _fixture.CriarMockUnidadeDeTrabalho();
        var ordemServicoAguardando = OrdemServicoUseCasesFixture.CriarOrdemServicoAguardandoAprovacao();

        _fixture.ConfigurarMockOrdemServicoGatewayParaAtualizacao(mockOrdemServicoGateway, ordemServicoAguardando);

        var ordemServicoUseCases = _fixture.CriarOrdemServicoUseCases(
            mockOrdemServicoGateway, null, null, null, mockUdt);

        // Act
        await ordemServicoUseCases.AceitarOrcamentoUseCaseAsync(ordemServicoAguardando.Id);

        // Assert
        mockOrdemServicoGateway.Received(1).ObterPorIdAsync(ordemServicoAguardando.Id);
        mockOrdemServicoGateway.Received(1).EditarAsync(Arg.Is<OrdemServico>(os => os.Status == StatusOrdemServico.EmExecucao));
        mockUdt.Received(1).Commit();
    }

    [Fact]
    public async Task RecusarOrcamentoUseCaseAsync_ComOrdemServicoValida_DeveAtualizarStatusParaCancelada()
    {
        // Arrange
        var mockOrdemServicoGateway = _fixture.CriarMockOrdemServicoGateway();
        var mockEventosGateway = _fixture.CriarMockEventosGateway();
        var mockUdt = _fixture.CriarMockUnidadeDeTrabalho();
        var ordemServicoAguardando = OrdemServicoUseCasesFixture.CriarOrdemServicoAguardandoAprovacao();

        _fixture.ConfigurarMockOrdemServicoGatewayParaAtualizacao(mockOrdemServicoGateway, ordemServicoAguardando);
        _fixture.ConfigurarMockEventosGateway(mockEventosGateway);

        var ordemServicoUseCases = _fixture.CriarOrdemServicoUseCases(
            mockOrdemServicoGateway, null, null, mockEventosGateway, mockUdt);

        // Act
        await ordemServicoUseCases.RecusarOrcamentoUseCaseAsync(ordemServicoAguardando.Id);

        // Assert
        await mockOrdemServicoGateway.Received(1).ObterPorIdAsync(ordemServicoAguardando.Id);
        await mockOrdemServicoGateway.Received(1).EditarAsync(Arg.Any<OrdemServico>());
        await mockEventosGateway.Received(1).Publicar(Arg.Any<OrdemServicoCanceladaEventDTO>());
        mockUdt.Received(1).Commit();
    }

    [Fact]
    public async Task AceitarOrcamentoUseCaseAsync_ComOrdemServicoInexistente_DeveLancarDadosNaoEncontradosException()
    {
        // Arrange
        var mockOrdemServicoGateway = _fixture.CriarMockOrdemServicoGateway();
        var ordemServicoId = Guid.NewGuid();

        _fixture.ConfigurarMockOrdemServicoGatewayParaOrdemNaoEncontrada(mockOrdemServicoGateway, ordemServicoId);

        var ordemServicoUseCases = _fixture.CriarOrdemServicoUseCases(mockOrdemServicoGateway);

        // Act & Assert
        await ordemServicoUseCases
            .Invoking(x => x.AceitarOrcamentoUseCaseAsync(ordemServicoId))
            .Should()
            .ThrowAsync<DadosNaoEncontradosException>()
            .WithMessage("Ordem de serviço não encontrada");
    }

    [Fact]
    public async Task AceitarOrcamentoUseCaseAsync_ComOrcamentoExpirado_DeveLancarOrcamentoExpiradoException()
    {
        // Arrange
        var mockOrdemServicoGateway = _fixture.CriarMockOrdemServicoGateway();
        var ordemServicoExpirada = OrdemServicoUseCasesFixture.CriarOrdemServicoOrcamentoExpirado();

        _fixture.ConfigurarMockOrdemServicoGatewayParaAtualizacao(mockOrdemServicoGateway, ordemServicoExpirada);

        var ordemServicoUseCases = _fixture.CriarOrdemServicoUseCases(mockOrdemServicoGateway);

        // Act & Assert
        await ordemServicoUseCases
            .Invoking(x => x.AceitarOrcamentoUseCaseAsync(ordemServicoExpirada.Id))
            .Should()
            .ThrowAsync<OrcamentoExpiradoException>()
            .WithMessage("Orçamento expirado");
    }

    [Fact]
    public async Task AceitarOrcamentoUseCaseAsync_ComOrcamentoQueDeveExpirar_DeveLancarOrcamentoExpiradoException()
    {
        // Arrange
        var mockOrdemServicoGateway = _fixture.CriarMockOrdemServicoGateway();
        var mockUdt = _fixture.CriarMockUnidadeDeTrabalho();
        
        var ordemServicoParaExpirar = new OrdemServico
        {
            Id = Guid.NewGuid(),
            Status = StatusOrdemServico.AguardandoAprovação,
            DataEnvioOrcamento = DateTime.UtcNow.AddDays(-4), // Expirado (mais de 3 dias)
            DataCadastro = DateTime.UtcNow.AddDays(-5),
            DataAtualizacao = DateTime.UtcNow.AddDays(-4),
            Ativo = true
        };

        _fixture.ConfigurarMockOrdemServicoGatewayParaAtualizacao(mockOrdemServicoGateway, ordemServicoParaExpirar);

        var ordemServicoUseCases = _fixture.CriarOrdemServicoUseCases(
            mockOrdemServicoGateway, null, null, null, mockUdt);

        // Act & Assert
        await ordemServicoUseCases
            .Invoking(x => x.AceitarOrcamentoUseCaseAsync(ordemServicoParaExpirar.Id))
            .Should()
            .ThrowAsync<OrcamentoExpiradoException>()
            .WithMessage("Orçamento expirado");

        // Deve ter atualizado o status para expirado antes de lançar a exceção
        mockOrdemServicoGateway.Received(1).EditarAsync(Arg.Is<OrdemServico>(os => os.Status == StatusOrdemServico.OrcamentoExpirado));
        mockUdt.Received(1).Commit();
    }

    [Fact]
    public void Constructor_ComParametrosNulos_DeveLancarArgumentNullException()
    {
        // Arrange
        var mockUdt = _fixture.CriarMockUnidadeDeTrabalho();
        var mockClienteGateway = _fixture.CriarMockClienteGateway();
        var mockServicoUseCases = _fixture.CriarMockServicoUseCases();
        var mockUsuarioLogado = _fixture.CriarMockUsuarioLogadoServico();
        var mockOrdemServicoGateway = _fixture.CriarMockOrdemServicoGateway();
        var mockEventosGateway = _fixture.CriarMockEventosGateway();
        var mockLogServico = _fixture.CriarMockLogServico<OrdemServicoUseCases>();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => 
            new OrdemServicoUseCases(
                null!,
                mockUdt,
                mockClienteGateway,
                mockServicoUseCases,
                mockUsuarioLogado,
                mockOrdemServicoGateway,
                mockEventosGateway));
                
        Assert.Equal("logServico", exception.ParamName);
    }
}
