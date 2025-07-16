using Aplicacao.DTOs.Requests.Estoque;
using Aplicacao.DTOs.Responses.Estoque;
using Aplicacao.Servicos;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using Moq;

public class EstoqueServicoTests
{
    private readonly Mock<IRepositorio<Estoque>> _repositorioMock;
    private readonly Mock<ILogServico<EstoqueServico>> _logServicoMock;
    private readonly Mock<IUnidadeDeTrabalho> _uotMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly EstoqueServico _estoqueServico;

    public EstoqueServicoTests()
    {
        _repositorioMock = new Mock<IRepositorio<Estoque>>();
        _logServicoMock = new Mock<ILogServico<EstoqueServico>>();
        _uotMock = new Mock<IUnidadeDeTrabalho>();
        _mapperMock = new Mock<IMapper>();

        _estoqueServico = new EstoqueServico(
            _repositorioMock.Object,
            _logServicoMock.Object,
            _uotMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Given_ValidRequest_When_CadastrarAsync_Then_ReturnsResponse()
    {
        var request = new CadastrarEstoqueRequest();
        var estoque = new Estoque();
        var response = new EstoqueResponse();

        _mapperMock.Setup(m => m.Map<Estoque>(request)).Returns(estoque);
        _uotMock.Setup(m => m.Commit()).ReturnsAsync(true);
        _mapperMock.Setup(m => m.Map<EstoqueResponse>(estoque)).Returns(response);

        var result = await _estoqueServico.CadastrarAsync(request);

        Assert.Equal(response, result);
    }

    [Fact]
    public async Task Given_CommitFails_When_CadastrarAsync_Then_ThrowsPersistirDadosException()
    {
        var request = new CadastrarEstoqueRequest();
        var estoque = new Estoque();

        _mapperMock.Setup(m => m.Map<Estoque>(request)).Returns(estoque);
        _uotMock.Setup(m => m.Commit()).ReturnsAsync(false);

        await Assert.ThrowsAsync<PersistirDadosException>(() => _estoqueServico.CadastrarAsync(request));
    }

    [Fact]
    public async Task Given_IdValido_When_ObterPorIdAsync_Then_ReturnsResponse()
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
    public async Task Given_IdInvalido_When_ObterPorIdAsync_Then_ThrowsException()
    {
        var id = Guid.NewGuid();
        _repositorioMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync((Estoque)null);

        await Assert.ThrowsAsync<DadosNaoEncontradosException>(() => _estoqueServico.ObterPorIdAsync(id));
    }

    [Fact]
    public async Task Given_IdValido_When_DeletarAsync_Then_ReturnsTrue()
    {
        var id = Guid.NewGuid();
        var estoque = new Estoque();

        _repositorioMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(estoque);
        _uotMock.Setup(u => u.Commit()).ReturnsAsync(true);

        var result = await _estoqueServico.DeletarAsync(id);

        Assert.True(result);
    }

    [Fact]
    public async Task Given_IdInvalido_When_DeletarAsync_Then_ThrowsException()
    {
        var id = Guid.NewGuid();
        _repositorioMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync((Estoque)null);

        await Assert.ThrowsAsync<DadosNaoEncontradosException>(() => _estoqueServico.DeletarAsync(id));
    }

    [Fact]
    public async Task Given_EstoquesExistem_When_ObterTodosAsync_Then_ReturnsList()
    {
        var estoques = new List<Estoque> { new Estoque() };
        var responses = new List<EstoqueResponse> { new EstoqueResponse() };

        _repositorioMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(estoques);
        _mapperMock.Setup(m => m.Map<IEnumerable<EstoqueResponse>>(estoques)).Returns(responses);

        var result = await _estoqueServico.ObterTodosAsync();

        Assert.Equal(responses, result);
    }

    [Fact]
    public async Task Given_ValidUpdateRequest_When_AtualizarAsync_Then_ReturnsUpdatedResponse()
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
    public async Task Given_InvalidId_When_AtualizarAsync_Then_ThrowsException()
    {
        var id = Guid.NewGuid();
        var request = new AtualizarEstoqueRequest();
        _repositorioMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync((Estoque)null);

        await Assert.ThrowsAsync<DadosNaoEncontradosException>(() => _estoqueServico.AtualizarAsync(id, request));
    }

    [Fact]
    public async Task Given_CommitFails_When_AtualizarAsync_Then_ThrowsException()
    {
        var id = Guid.NewGuid();
        var request = new AtualizarEstoqueRequest();
        var estoque = new Estoque();

        _repositorioMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(estoque);
        _uotMock.Setup(u => u.Commit()).ReturnsAsync(false);

        await Assert.ThrowsAsync<PersistirDadosException>(() => _estoqueServico.AtualizarAsync(id, request));
    }
}