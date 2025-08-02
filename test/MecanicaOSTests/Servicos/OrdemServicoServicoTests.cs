using Aplicacao.DTOs.Requests.OrdemServico;
using Aplicacao.DTOs.Responses.OrdemServico;
using Aplicacao.DTOs.Responses.Servico;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.Notificacoes.OS;
using Aplicacao.Servicos;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Especificacoes.Cliente;
using Dominio.Especificacoes.OrdemServico;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using Xunit;

namespace MecanicaOSTests.Servicos;

public class OrdemServicoServicoTests
{
    private readonly Mock<IRepositorio<OrdemServico>> _repositorioMock;
    private readonly Mock<ILogServico<OrdemServicoServico>> _logServicoMock;
    private readonly Mock<IUnidadeDeTrabalho> _udtMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IRepositorio<Cliente>> _clienteRepositorioMock;
    private readonly Mock<IServicoServico> _servicoServicoMock;
    private readonly Mock<IUsuarioLogadoServico> _usuarioLogadoServicoMock;
    private readonly OrdemServicoServico _ordemServicoServico;
    
    private readonly Guid _clienteId = Guid.NewGuid();
    private readonly Guid _veiculoId = Guid.NewGuid();
    private readonly Guid _servicoId = Guid.NewGuid();
    private readonly Guid _ordemServicoId = Guid.NewGuid();

    public OrdemServicoServicoTests()
    {
        _repositorioMock = new Mock<IRepositorio<OrdemServico>>();
        _logServicoMock = new Mock<ILogServico<OrdemServicoServico>>();
        _udtMock = new Mock<IUnidadeDeTrabalho>();
        _mapperMock = new Mock<IMapper>();
        _mediatorMock = new Mock<IMediator>();
        _clienteRepositorioMock = new Mock<IRepositorio<Cliente>>();
        _servicoServicoMock = new Mock<IServicoServico>();
        _usuarioLogadoServicoMock = new Mock<IUsuarioLogadoServico>();

        _ordemServicoServico = new OrdemServicoServico(
            _repositorioMock.Object,
            _logServicoMock.Object,
            _udtMock.Object,
            _mapperMock.Object,
            _mediatorMock.Object,
            _clienteRepositorioMock.Object,
            _servicoServicoMock.Object,
            _usuarioLogadoServicoMock.Object
        );
    }

    [Fact]
    public async Task CadastrarAsync_QuandoDadosValidos_DeveCadastrarComSucesso()
    {
        // Arrange
        var request = new CadastrarOrdemServicoRequest
        {
            ClienteId = _clienteId,
            VeiculoId = _veiculoId,
            ServicoId = _servicoId,
            Descricao = "Troca de óleo e filtro"
        };

        var cliente = new Cliente
        {
            Id = _clienteId,
            Nome = "Cliente Teste",
            Documento = "12345678901",
            Veiculos = new List<Veiculo>
            {
                new()
                {
                    Id = _veiculoId,
                    Placa = "ABC1234",
                    Marca = "Fiat",
                    Modelo = "Uno",
                    Ano = "2020"
                }
            }
        };

        var servico = new Servico
        {
            Id = _servicoId,
            Nome = "Troca de Óleo",
            Descricao = "Troca de óleo do motor",
            Valor = 150.00m
        };

        var ordemServico = new OrdemServico
        {
            Id = _ordemServicoId,
            ClienteId = _clienteId,
            VeiculoId = _veiculoId,
            ServicoId = _servicoId,
            Descricao = "Troca de óleo e filtro",
            Status = StatusOrdemServico.EmDiagnostico
        };

        _clienteRepositorioMock.Setup(x => x.ObterUmAsync(It.IsAny<ObterClienteComVeiculoPorIdEspecificacao>()))
            .ReturnsAsync(cliente);
            
        _servicoServicoMock.Setup(x => x.ObterServicoPorIdAsync(_servicoId))
            .ReturnsAsync(new ServicoResponse { Id = _servicoId, Nome = "Serviço Teste", Valor = 100.00m });
            
        _mapperMock.Setup(m => m.Map<OrdemServico>(request))
            .Returns(ordemServico);
            
        _repositorioMock.Setup(x => x.CadastrarAsync(ordemServico))
            .ReturnsAsync(ordemServico);
            
        _udtMock.Setup(x => x.Commit())
            .ReturnsAsync(true);
            
        _mapperMock.Setup(m => m.Map<OrdemServicoResponse>(It.IsAny<OrdemServico>()))
            .Returns(new OrdemServicoResponse { Id = _ordemServicoId });

        // Act
        var resultado = await _ordemServicoServico.CadastrarAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(_ordemServicoId);
        _repositorioMock.Verify(x => x.CadastrarAsync(It.IsAny<OrdemServico>()), Times.Once);
        _udtMock.Verify(x => x.Commit(), Times.Once);
        _logServicoMock.Verify(l => l.LogInicio(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
        _logServicoMock.Verify(l => l.LogFim(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task CadastrarAsync_QuandoClienteNaoEncontrado_DeveLancarExcecao()
    {
        // Arrange
        var request = new CadastrarOrdemServicoRequest
        {
            ClienteId = _clienteId,
            VeiculoId = _veiculoId,
            ServicoId = _servicoId,
            Descricao = "Troca de óleo e filtro"
        };

        _clienteRepositorioMock.Setup(x => x.ObterUmAsync(It.IsAny<ObterClienteComVeiculoPorIdEspecificacao>()))
            .ReturnsAsync((Cliente)null);

        // Act & Assert
        await Assert.ThrowsAsync<DadosNaoEncontradosException>(() => 
            _ordemServicoServico.CadastrarAsync(request));
    }

    [Fact]
    public async Task AtualizarAsync_QuandoDadosValidos_DeveAtualizarComSucesso()
    {
        // Arrange
        var request = new AtualizarOrdemServicoRequest
        {
            ClienteId = _clienteId,
            VeiculoId = _veiculoId,
            ServicoId = _servicoId,
            Descricao = "Descrição atualizada",
            Status = StatusOrdemServico.EmDiagnostico
        };

        var ordemServico = new OrdemServico
        {
            Id = _ordemServicoId,
            ClienteId = _clienteId,
            VeiculoId = _veiculoId,
            ServicoId = _servicoId,
            Descricao = "Descrição antiga",
            Status = StatusOrdemServico.EmDiagnostico
        };

        _repositorioMock.Setup(x => x.ObterPorIdAsync(_ordemServicoId))
            .ReturnsAsync(ordemServico);
            
        _udtMock.Setup(x => x.Commit())
            .ReturnsAsync(true);
            
        _mapperMock.Setup(m => m.Map<OrdemServicoResponse>(It.IsAny<OrdemServico>()))
            .Returns(new OrdemServicoResponse { Id = _ordemServicoId });

        // Act
        var resultado = await _ordemServicoServico.AtualizarAsync(_ordemServicoId, request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(_ordemServicoId);
        ordemServico.Descricao.Should().Be(request.Descricao);
        ordemServico.Status.Should().Be(request.Status);
        _repositorioMock.Verify(x => x.EditarAsync(It.IsAny<OrdemServico>()), Times.Once);
        _udtMock.Verify(x => x.Commit(), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<OrdemServicoEmOrcamentoEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoEncontrado_DeveRetornarOrdemServico()
    {
        // Arrange
        var ordemServico = new OrdemServico
        {
            Id = _ordemServicoId,
            ClienteId = _clienteId,
            VeiculoId = _veiculoId,
            ServicoId = _servicoId,
            Descricao = "Ordem de serviço teste",
            Status = StatusOrdemServico.EmDiagnostico
        };

        _repositorioMock.Setup(x => x.ObterUmSemRastreamentoAsync(It.IsAny<ObterOrdemServicoPorIdComInsumosEspecificacao>()))
            .ReturnsAsync(ordemServico);
            
        _mapperMock.Setup(m => m.Map<OrdemServicoResponse>(ordemServico))
            .Returns(new OrdemServicoResponse { Id = _ordemServicoId });

        // Act
        var resultado = await _ordemServicoServico.ObterPorIdAsync(_ordemServicoId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(_ordemServicoId);
        _repositorioMock.Verify(x => x.ObterUmSemRastreamentoAsync(It.Is<ObterOrdemServicoPorIdComInsumosEspecificacao>(s => s != null)), Times.Once);
    }

    [Fact]
    public async Task ObterPorStatusAsync_QuandoEncontrado_DeveRetornarLista()
    {
        // Arrange
        var status = StatusOrdemServico.EmDiagnostico;
        var ordens = new List<OrdemServico>
        {
            new OrdemServico { Id = _ordemServicoId, Status = status }
        };

        _repositorioMock.Setup(x => x.ListarAsync(It.IsAny<ObterOrdemServicoPorStatusEspecificacao>()))
            .ReturnsAsync(ordens);
            
        _mapperMock.Setup(m => m.Map<IEnumerable<OrdemServicoResponse>>(It.IsAny<IEnumerable<OrdemServico>>()))
            .Returns(new List<OrdemServicoResponse> { new OrdemServicoResponse { Id = _ordemServicoId } });

        // Act
        var resultado = await _ordemServicoServico.ObterPorStatusAsync(status);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(1);
        resultado.First().Id.Should().Be(_ordemServicoId);
    }

    [Fact]
    public async Task AceitarOrcamentoAsync_QuandoOrcamentoValido_DeveAtualizarStatus()
    {
        // Arrange
        var ordemServico = new OrdemServico
        {
            Id = _ordemServicoId,
            Status = StatusOrdemServico.AguardandoAprovacao,
            DataEnvioOrcamento = DateTime.UtcNow
        };

        _repositorioMock.Setup(x => x.ObterPorIdAsync(_ordemServicoId))
            .ReturnsAsync(ordemServico);
            
        _udtMock.Setup(x => x.Commit())
            .ReturnsAsync(true);

        // Act
        await _ordemServicoServico.AceitarOrcamentoAsync(_ordemServicoId);

        // Assert
        ordemServico.Status.Should().Be(StatusOrdemServico.EmExecucao);
        _repositorioMock.Verify(x => x.EditarAsync(It.IsAny<OrdemServico>()), Times.Once);
        _udtMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task RecusarOrcamentoAsync_QuandoOrcamentoValido_DeveAtualizarStatusECancelar()
    {
        // Arrange
        var ordemServico = new OrdemServico
        {
            Id = _ordemServicoId,
            Status = StatusOrdemServico.AguardandoAprovacao,
            DataEnvioOrcamento = DateTime.UtcNow
        };

        _repositorioMock.Setup(x => x.ObterPorIdAsync(_ordemServicoId))
            .ReturnsAsync(ordemServico);
            
        _udtMock.Setup(x => x.Commit())
            .ReturnsAsync(true);

        // Act
        await _ordemServicoServico.RecusarOrcamentoAsync(_ordemServicoId);

        // Assert
        ordemServico.Status.Should().Be(StatusOrdemServico.Cancelada);
        _repositorioMock.Verify(x => x.EditarAsync(It.IsAny<OrdemServico>()), Times.Once);
        _udtMock.Verify(x => x.Commit(), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<OrdemServicoCanceladaEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
