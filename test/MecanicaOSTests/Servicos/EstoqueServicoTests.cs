using Aplicacao.DTOs.Requests.Estoque;
using Aplicacao.DTOs.Responses.Estoque;
using Aplicacao.Ports;
using Aplicacao.Servicos;
using Aplicacao.UseCases.Estoque.CriarEstoque;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using Moq;

public class EstoqueServicoTests
{
    private readonly Mock<IRepositorio<Estoque>> _repositorioMock;
    private readonly Mock<IEstoqueRepository> _estoqueRepository;
    private readonly Mock<ILogServico<EstoqueServico>> _logServicoMock;
    private readonly Mock<IUnidadeDeTrabalho> _uotMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IUsuarioLogadoServico> _usuarioLogadoServico = new();
    private readonly EstoqueServico _estoqueServico;
    private readonly CriarEstoqueUseCase _criarEstoqueUseCase;

    public EstoqueServicoTests()
    {
        _repositorioMock = new Mock<IRepositorio<Estoque>>();
        _estoqueRepository = new Mock<IEstoqueRepository>();
        _logServicoMock = new Mock<ILogServico<EstoqueServico>>();
        _uotMock = new Mock<IUnidadeDeTrabalho>();
        _mapperMock = new Mock<IMapper>();

        _estoqueServico = new EstoqueServico(
            _repositorioMock.Object,
            _logServicoMock.Object,
            _uotMock.Object,
            _mapperMock.Object,
            _usuarioLogadoServico.Object
        );

        _criarEstoqueUseCase = new CriarEstoqueUseCase(
            _estoqueRepository.Object,
            _uotMock.Object
        );
    }

    [Fact]
    public async Task Dado_RequestValido_Quando_CadastrarAsync_Entao_RetornaResponse()
    {
        var request = new CriarEstoqueRequest();
        var estoque = new Estoque();
        var response = new CriarEstoqueResponse();

        _mapperMock.Setup(m => m.Map<Estoque>(request)).Returns(estoque);
        _uotMock.Setup(m => m.Commit()).ReturnsAsync(true);
        _mapperMock.Setup(m => m.Map<CriarEstoqueResponse>(estoque)).Returns(response);

        var result = await _criarEstoqueUseCase.ExecuteAsync(request);

        Assert.Equal(response, result);
    }

    [Fact]
    public async Task Dado_FalhaNoCommit_Quando_CadastrarAsync_Entao_LancaExcecaoPersistirDados()
    {
        var request = new CriarEstoqueRequest();
        var estoque = new Estoque();

        _mapperMock.Setup(m => m.Map<Estoque>(request)).Returns(estoque);
        _uotMock.Setup(m => m.Commit()).ReturnsAsync(false);

        await Assert.ThrowsAsync<DomainException>(() => _criarEstoqueUseCase.ExecuteAsync(request));
    }

    [Fact]
    public async Task Dado_IdValido_Quando_ObterPorIdAsync_Entao_RetornaResponse()
    {
        var id = Guid.NewGuid();
        var estoque = new Estoque();
        var response = new EstoqueResponse();

        _repositorioMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(estoque);
        _mapperMock.Setup(m => m.Map<EstoqueResponse>(estoque)).Returns(response);

        var result = await _estoqueServico.ObterPorIdAsync(id);

        Assert.Equal(response, result);
    }

    [Fact]
    public async Task Dado_IdInvalido_Quando_ObterPorIdAsync_Entao_LancaExcecao()
    {
        var id = Guid.NewGuid();
        _repositorioMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync((Estoque)null);

        await Assert.ThrowsAsync<DadosNaoEncontradosException>(() => _estoqueServico.ObterPorIdAsync(id));
    }

    [Fact]
    public async Task Dado_IdValido_Quando_DeletarAsync_Entao_RetornaTrue()
    {
        var id = Guid.NewGuid();
        var estoque = new Estoque();

        _repositorioMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(estoque);
        _uotMock.Setup(u => u.Commit()).ReturnsAsync(true);

        var result = await _estoqueServico.DeletarAsync(id);

        Assert.True(result);
    }

    [Fact]
    public async Task Dado_IdInvalido_Quando_DeletarAsync_Entao_LancaExcecao()
    {
        var id = Guid.NewGuid();
        _repositorioMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync((Estoque)null);

        await Assert.ThrowsAsync<DadosNaoEncontradosException>(() => _estoqueServico.DeletarAsync(id));
    }

    [Fact]
    public async Task Quando_ObterTodosAsync_Entao_RetornaLista()
    {
        var estoques = new List<Estoque> { new Estoque() };
        var responses = new List<EstoqueResponse> { new EstoqueResponse() };

        _repositorioMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(estoques);
        _mapperMock.Setup(m => m.Map<IEnumerable<EstoqueResponse>>(estoques)).Returns(responses);

        var result = await _estoqueServico.ObterTodosAsync();

        Assert.Equal(responses, result);
    }

    [Fact]
    public async Task Dado_IdValidoERequest_Quando_AtualizarAsync_Entao_RetornaResponse()
    {
        var id = Guid.NewGuid();
        var request = new AtualizarEstoqueRequest
        {
            Insumo = "NovoInsumo",
            Descricao = "DescricaoAtualizada",
            Preco = 15,
            QuantidadeDisponivel = 5,
            QuantidadeMinima = 2
        };
        var estoque = new Estoque();
        var response = new EstoqueResponse();

        _repositorioMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(estoque);
        _uotMock.Setup(u => u.Commit()).ReturnsAsync(true);
        _mapperMock.Setup(m => m.Map<EstoqueResponse>(estoque)).Returns(response);

        var result = await _estoqueServico.AtualizarAsync(id, request);

        Assert.Equal(response, result);
    }

    [Fact]
    public async Task Dado_IdInvalido_Quando_AtualizarAsync_Entao_LancaExcecao()
    {
        var id = Guid.NewGuid();
        var request = new AtualizarEstoqueRequest();
        _repositorioMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync((Estoque)null);

        await Assert.ThrowsAsync<DadosNaoEncontradosException>(() => _estoqueServico.AtualizarAsync(id, request));
    }

    [Fact]
    public async Task Dado_FalhaNoCommit_Quando_AtualizarAsync_Entao_LancaExcecaoPersistirDados()
    {
        var id = Guid.NewGuid();
        var request = new AtualizarEstoqueRequest();
        var estoque = new Estoque();

        _repositorioMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(estoque);
        _uotMock.Setup(u => u.Commit()).ReturnsAsync(false);

        await Assert.ThrowsAsync<PersistirDadosException>(() => _estoqueServico.AtualizarAsync(id, request));
    }

    [Fact]
    public async Task AtualizarAsync_ComRequestParcial_DeveAtualizarApenasCamposFornecidos()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new AtualizarEstoqueRequest { Preco = 20 };
        var estoque = new Estoque("Óleo Motor", "Óleo sintético 5W30", 10, 10, 10);
        var response = new EstoqueResponse();

        _repositorioMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(estoque);
        _uotMock.Setup(u => u.Commit()).ReturnsAsync(true);
        _mapperMock.Setup(m => m.Map<EstoqueResponse>(estoque)).Returns(response);

        // Act
        await _estoqueServico.AtualizarAsync(id, request);

        // Assert
        Assert.Equal(20, estoque.Preco);
    }
}