using Aplicacao.DTOs.Requests.Estoque;
using Aplicacao.DTOs.Requests.OrdemServico.InsumoOS;
using Aplicacao.DTOs.Responses.Estoque;
using Aplicacao.DTOs.Responses.OrdemServico;
using Aplicacao.DTOs.Responses.OrdemServico.InsumoOrdemServico;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.Jobs;
using Aplicacao.Servicos;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Especificacoes.Insumo;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using FluentAssertions;
using Moq;

namespace MecanicaOSTests.Servicos;

public class InsumoOSServicoTests
{
    private readonly Mock<IOrdemServicoServico> _osServicoMock;
    private readonly Mock<IEstoqueServico> _estoqueServicoMock;
    private readonly Mock<VerificarEstoqueJob> _verificarEstoqueJobMock;
    private readonly Mock<IRepositorio<InsumoOS>> _insumoOSRepoMock;
    private readonly Mock<ILogServico<InsumoOSServico>> _logMock;
    private readonly Mock<IUnidadeDeTrabalho> _uotMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IUsuarioLogadoServico> _usuarioLogadoServicoMock;
    private readonly InsumoOSServico _insumoOSServico;

    public InsumoOSServicoTests()
    {
        _osServicoMock = new Mock<IOrdemServicoServico>();
        _estoqueServicoMock = new Mock<IEstoqueServico>();
        
        // Criando mocks para as dependências do VerificarEstoqueJob
        var estoqueRepoMock = new Mock<IRepositorio<Estoque>>();
        var usuarioRepoMock = new Mock<IRepositorio<Usuario>>();
        var alertaEstoqueRepoMock = new Mock<IRepositorio<AlertaEstoque>>();
        var notificacaoEmailMock = new Mock<IServicoEmail>();
        var logVerificarEstoqueJobMock = new Mock<ILogServico<VerificarEstoqueJob>>();
        var uotVerificarEstoqueJobMock = new Mock<IUnidadeDeTrabalho>();
        
        _verificarEstoqueJobMock = new Mock<VerificarEstoqueJob>(
            estoqueRepoMock.Object,
            usuarioRepoMock.Object,
            alertaEstoqueRepoMock.Object,
            notificacaoEmailMock.Object,
            logVerificarEstoqueJobMock.Object,
            uotVerificarEstoqueJobMock.Object);
            
        _insumoOSRepoMock = new Mock<IRepositorio<InsumoOS>>();
        _logMock = new Mock<ILogServico<InsumoOSServico>>();
        _uotMock = new Mock<IUnidadeDeTrabalho>();
        _mapperMock = new Mock<IMapper>();
        _usuarioLogadoServicoMock = new Mock<IUsuarioLogadoServico>();

        _insumoOSServico = new InsumoOSServico(
            _osServicoMock.Object,
            _estoqueServicoMock.Object,
            _verificarEstoqueJobMock.Object,
            _insumoOSRepoMock.Object,
            _logMock.Object,
            _uotMock.Object,
            _mapperMock.Object,
            _usuarioLogadoServicoMock.Object);
    }

    [Fact]
    public async Task Dado_OrdemServicoNaoEncontrada_Quando_CadastrarInsumosAsync_Entao_DeveLancarExcecao()
    {
        // Arrange
        var ordemServicoId = Guid.NewGuid();
        var request = new List<CadastrarInsumoOSRequest>
        {
            new() { EstoqueId = Guid.NewGuid(), Quantidade = 2 }
        };

        _osServicoMock.Setup(x => x.ObterPorIdAsync(ordemServicoId))
            .Returns(Task.FromResult<OrdemServicoResponse>(null!));

        // Act & Assert
        await _insumoOSServico.Invoking(x => x.CadastrarInsumosAsync(ordemServicoId, request))
            .Should().ThrowAsync<DadosNaoEncontradosException>()
            .WithMessage("Ordem de serviço não encontrada");
    }

    [Fact]
    public async Task Dado_InsumosJaCadastrados_Quando_CadastrarInsumosAsync_Entao_DeveLancarExcecao()
    {
        // Arrange
        var ordemServicoId = Guid.NewGuid();
        var estoqueId = Guid.NewGuid();
        
        var request = new List<CadastrarInsumoOSRequest>
        {
            new() { EstoqueId = estoqueId, Quantidade = 1 }
        };

        var ordemServicoResponse = new OrdemServicoResponse { Id = ordemServicoId };
        _osServicoMock.Setup(x => x.ObterPorIdAsync(ordemServicoId))
            .ReturnsAsync(ordemServicoResponse);
            
        _mapperMock.Setup(m => m.Map<OrdemServico>(It.IsAny<OrdemServicoResponse>()))
            .Returns(new OrdemServico { Id = ordemServicoId });

        var insumosExistentes = new List<InsumoOS>
        {
            new() { EstoqueId = estoqueId, OrdemServicoId = ordemServicoId, Quantidade = 1 }
        };

        _insumoOSRepoMock.Setup(x => x.ListarAsync(It.IsAny<ObterInsumosOSPorOSEspecificacao>()))
            .ReturnsAsync(insumosExistentes);

        // Act & Assert
        await _insumoOSServico.Invoking(x => x.CadastrarInsumosAsync(ordemServicoId, request))
            .Should().ThrowAsync<DadosJaCadastradosException>()
            .WithMessage("Os insumos informados já estão cadastrados na Ordem de Serviço");
    }

    [Fact]
    public async Task Dado_EstoqueInsuiciente_Quando_CadastrarInsumosAsync_Entao_DeveLancarExcecao()
    {
        // Arrange
        var ordemServicoId = Guid.NewGuid();
        var estoqueId = Guid.NewGuid();
        
        var request = new List<CadastrarInsumoOSRequest>
        {
            new() { EstoqueId = estoqueId, Quantidade = 10 }
        };

        var ordemServicoResponse = new OrdemServicoResponse { Id = ordemServicoId };
        _osServicoMock.Setup(x => x.ObterPorIdAsync(ordemServicoId))
            .ReturnsAsync(ordemServicoResponse);
            
        _mapperMock.Setup(m => m.Map<OrdemServico>(It.IsAny<OrdemServicoResponse>()))
            .Returns(new OrdemServico { Id = ordemServicoId });

        _insumoOSRepoMock.Setup(x => x.ListarAsync(It.IsAny<ObterInsumosOSPorOSEspecificacao>()))
            .ReturnsAsync(new List<InsumoOS>());

        var estoque = new Estoque { Id = estoqueId, QuantidadeDisponivel = 5 };
        var estoqueResponse = new EstoqueResponse { Id = estoqueId, QuantidadeDisponivel = 5 };
        _estoqueServicoMock.Setup(x => x.ObterPorIdAsync(estoqueId))
            .ReturnsAsync(estoqueResponse);
            
        _mapperMock.Setup(m => m.Map<Estoque>(It.IsAny<EstoqueResponse>()))
            .Returns(estoque);

        _mapperMock.Setup(m => m.Map<Estoque>(It.IsAny<ObterInsumosOSPorOSEspecificacao>()))
            .Returns(estoque);
            
        _mapperMock.Setup(m => m.Map<InsumoOS>(It.IsAny<CadastrarInsumoOSRequest>()))
            .Returns(new InsumoOS { EstoqueId = estoqueId, Quantidade = 10 });

        // Act & Assert
        await _insumoOSServico.Invoking(x => x.CadastrarInsumosAsync(ordemServicoId, request))
            .Should().ThrowAsync<InsumosIndisponiveisException>()
            .WithMessage("Insumos insuficientes no estoque para atender ao serviço solicitado.");
    }

    [Fact]
    public async Task Dado_DadosValidos_Quando_CadastrarInsumosAsync_Entao_DeveCadastrarInsumos()
    {
        // Arrange
        var ordemServicoId = Guid.NewGuid();
        var estoqueId = Guid.NewGuid();
        
        var request = new List<CadastrarInsumoOSRequest>
        {
            new() { EstoqueId = estoqueId, Quantidade = 3 }
        };

        var ordemServicoResponse = new OrdemServicoResponse { Id = ordemServicoId };
        _osServicoMock.Setup(x => x.ObterPorIdAsync(ordemServicoId))
            .ReturnsAsync(ordemServicoResponse);
            
        _mapperMock.Setup(m => m.Map<OrdemServico>(It.IsAny<OrdemServicoResponse>()))
            .Returns(new OrdemServico { Id = ordemServicoId });

        _insumoOSRepoMock.Setup(x => x.ListarAsync(It.IsAny<ObterInsumosOSPorOSEspecificacao>()))
            .ReturnsAsync(new List<InsumoOS>());

        var estoque = new Estoque { Id = estoqueId, QuantidadeDisponivel = 5 };
        var estoqueResponse = new EstoqueResponse { Id = estoqueId, QuantidadeDisponivel = 5 };
        _estoqueServicoMock.Setup(x => x.ObterPorIdAsync(estoqueId))
            .ReturnsAsync(estoqueResponse);

        _mapperMock.Setup(m => m.Map<Estoque>(It.IsAny<EstoqueResponse>()))
            .Returns(estoque);
            
        _mapperMock.Setup(m => m.Map<Estoque>(It.IsAny<ObterInsumosOSPorOSEspecificacao>()))
            .Returns(estoque);

        var insumoOS = new InsumoOS { Id = Guid.NewGuid(), EstoqueId = estoqueId, Quantidade = 3, OrdemServicoId = ordemServicoId };
        _mapperMock.Setup(m => m.Map<InsumoOS>(It.IsAny<CadastrarInsumoOSRequest>()))
            .Returns(new InsumoOS { EstoqueId = estoqueId, Quantidade = 3 });

        var insumosCadastrados = new List<InsumoOS> { insumoOS };
        _insumoOSRepoMock.Setup(x => x.CadastrarVariosAsync(It.IsAny<IEnumerable<InsumoOS>>()))
            .ReturnsAsync(insumosCadastrados);

        _uotMock.Setup(x => x.Commit())
            .ReturnsAsync(true);
            
        _mapperMock.Setup(m => m.Map<List<InsumoOSResponse>>(It.IsAny<List<InsumoOS>>()))
            .Returns(insumosCadastrados.Select(i => new InsumoOSResponse { EstoqueId = i.EstoqueId, Quantidade = i.Quantidade }).ToList());

        // Act
        var result = await _insumoOSServico.CadastrarInsumosAsync(ordemServicoId, request);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        _osServicoMock.Verify(x => x.ObterPorIdAsync(ordemServicoId), Times.Once);
        _insumoOSRepoMock.Verify(x => x.CadastrarVariosAsync(It.IsAny<IEnumerable<InsumoOS>>()), Times.Once);
        _uotMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task Dado_ErroAoSalvar_Quando_CadastrarInsumosAsync_Entao_DeveLancarExcecao()
    {
        // Arrange
        var ordemServicoId = Guid.NewGuid();
        var estoqueId = Guid.NewGuid();
        
        var request = new List<CadastrarInsumoOSRequest>
        {
            new() { EstoqueId = estoqueId, Quantidade = 1 }
        };

        var ordemServicoResponse = new OrdemServicoResponse { Id = ordemServicoId };
        _osServicoMock.Setup(x => x.ObterPorIdAsync(ordemServicoId))
            .ReturnsAsync(ordemServicoResponse);
            
        _mapperMock.Setup(m => m.Map<OrdemServico>(It.IsAny<OrdemServicoResponse>()))
            .Returns(new OrdemServico { Id = ordemServicoId });

        _insumoOSRepoMock.Setup(x => x.ListarAsync(It.IsAny<ObterInsumosOSPorOSEspecificacao>()))
            .ReturnsAsync(new List<InsumoOS>());

        var estoque = new Estoque { Id = estoqueId, QuantidadeDisponivel = 10 };
        var estoqueResponse = new EstoqueResponse { Id = estoqueId, QuantidadeDisponivel = 10 };
        _estoqueServicoMock.Setup(x => x.ObterPorIdAsync(estoqueId))
            .ReturnsAsync(estoqueResponse);

        _mapperMock.Setup(m => m.Map<Estoque>(It.IsAny<EstoqueResponse>()))
            .Returns(estoque);
            
        _mapperMock.Setup(m => m.Map<Estoque>(It.IsAny<ObterInsumosOSPorOSEspecificacao>()))
            .Returns(estoque);

        _mapperMock.Setup(m => m.Map<InsumoOS>(It.IsAny<CadastrarInsumoOSRequest>()))
            .Returns(new InsumoOS { EstoqueId = estoqueId, Quantidade = 1 });

        var insumosCadastrados = new List<InsumoOS> { new() };
        _insumoOSRepoMock.Setup(x => x.CadastrarVariosAsync(It.IsAny<IEnumerable<InsumoOS>>()))
            .ReturnsAsync(insumosCadastrados);
            
        _mapperMock.Setup(m => m.Map<List<InsumoOSResponse>>(It.IsAny<List<InsumoOS>>()))
            .Returns(insumosCadastrados.Select(i => new InsumoOSResponse { EstoqueId = i.EstoqueId, Quantidade = i.Quantidade }).ToList());

        _uotMock.Setup(x => x.Commit())
            .ReturnsAsync(false);

        // Act & Assert
        await _insumoOSServico.Invoking(x => x.CadastrarInsumosAsync(ordemServicoId, request))
            .Should().ThrowAsync<PersistirDadosException>()
            .WithMessage("Erro ao adicionar os insumos na ordem de serviço");
    }

    [Fact]
    public async Task Dado_InsumosValidos_Quando_DevolverInsumosAoEstoqueAsync_Entao_DeveAtualizarEstoque()
    {
        // Arrange
        var estoqueId = Guid.NewGuid();
        var insumos = new List<InsumoOS>
        {
            new()
            {
                Id = Guid.NewGuid(),
                EstoqueId = estoqueId,
                Quantidade = 2,
                Estoque = new Estoque { Id = estoqueId, QuantidadeDisponivel = 5 }
            }
        };

        _mapperMock.Setup(m => m.Map<AtualizarEstoqueRequest>(It.IsAny<Estoque>()))
            .Returns<Estoque>(e => new AtualizarEstoqueRequest 
            { 
                QuantidadeDisponivel = e.QuantidadeDisponivel 
            });

        // Act
        await _insumoOSServico.DevolverInsumosAoEstoqueAsync(insumos);

        // Assert
        _estoqueServicoMock.Verify(x => x.AtualizarAsync(
            estoqueId,
            It.Is<AtualizarEstoqueRequest>(r => r.QuantidadeDisponivel == 7)),
            Times.Once);
    }
}
