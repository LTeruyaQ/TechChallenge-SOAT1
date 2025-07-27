using Aplicacao.DTOs.Requests.Veiculo;
using Aplicacao.DTOs.Responses.Veiculo;
using Aplicacao.Servicos;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Especificacoes.Veiculo;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using Infraestrutura.Autenticacao;
using Moq;

public class VeiculoServicoTests
{
    private readonly Mock<IRepositorio<Veiculo>> _repositorioMock;
    private readonly Mock<ILogServico<VeiculoServico>> _logMock;
    private readonly Mock<IUnidadeDeTrabalho> _uotMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly VeiculoServico _servico;
    private readonly Mock<IUsuarioLogadoServico> _usuarioLogadoServico;

    public VeiculoServicoTests()
    {
        _repositorioMock = new Mock<IRepositorio<Veiculo>>();
        _logMock = new Mock<ILogServico<VeiculoServico>>();
        _uotMock = new Mock<IUnidadeDeTrabalho>();
        _mapperMock = new Mock<IMapper>();
        _usuarioLogadoServico = new Mock<IUsuarioLogadoServico>();
        _servico = new VeiculoServico(_repositorioMock.Object, _logMock.Object, _uotMock.Object, _mapperMock.Object, _usuarioLogadoServico.Object);
    }

    private Veiculo CriarVeiculo() => new Veiculo
    {
        Id = Guid.NewGuid(),
        Placa = "ABC1234",
        Marca = "Toyota",
        Modelo = "Corolla",
        Cor = "Preto",
        Ano = "2020",
        ClienteId = Guid.NewGuid()
    };


    [Fact]
    public async Task Dado_IdValidoERequest_Quando_AtualizarAsync_Entao_AtualizaERetornaResposta()
    {
        var id = Guid.NewGuid();
        var veiculo = CriarVeiculo();
        var request = new AtualizarVeiculoRequest { Cor = "Azul" };

        _repositorioMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(veiculo);
        _repositorioMock.Setup(r => r.EditarAsync(veiculo)).Returns(Task.CompletedTask);
        _uotMock.Setup(u => u.Commit()).ReturnsAsync(true);
        _mapperMock.Setup(m => m.Map<VeiculoResponse>(veiculo)).Returns(new VeiculoResponse());

        var result = await _servico.AtualizarAsync(id, request);

        Assert.NotNull(result);
        Assert.Equal("Azul", veiculo.Cor);
    }

    [Fact]
    public async Task Dado_IdInexistente_Quando_AtualizarAsync_Entao_LancaExcecaoDadosNaoEncontrados()
    {
        _repositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Veiculo)null!);

        await Assert.ThrowsAsync<DadosNaoEncontradosException>(() => _servico.AtualizarAsync(Guid.NewGuid(), new AtualizarVeiculoRequest()));
    }

    [Fact]
    public async Task Dado_FalhaNoCommit_Quando_AtualizarAsync_Entao_LancaExcecaoPersistirDados()
    {
        var id = Guid.NewGuid();
        var veiculo = CriarVeiculo();
        var request = new AtualizarVeiculoRequest { Modelo = "Novo Modelo" };

        _repositorioMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(veiculo);
        _repositorioMock.Setup(r => r.EditarAsync(veiculo)).Returns(Task.CompletedTask);
        _uotMock.Setup(u => u.Commit()).ReturnsAsync(false);

        await Assert.ThrowsAsync<PersistirDadosException>(() => _servico.AtualizarAsync(id, request));
    }
    [Fact]
    public async Task Dado_IdExistente_Quando_ObterPorIdAsync_Entao_RetornaVeiculoResponse()
    {
        var veiculo = CriarVeiculo();
        _repositorioMock.Setup(r => r.ObterPorIdAsync(veiculo.Id)).ReturnsAsync(veiculo);
        _mapperMock.Setup(m => m.Map<VeiculoResponse>(veiculo)).Returns(new VeiculoResponse());

        var result = await _servico.ObterPorIdAsync(veiculo.Id);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task Dado_IdInvalido_Quando_ObterPorIdAsync_Entao_LancaExcecaoDadosNaoEncontrados()
    {
        _repositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Veiculo)null!);

        await Assert.ThrowsAsync<DadosNaoEncontradosException>(() => _servico.ObterPorIdAsync(Guid.NewGuid()));
    }
    [Fact]
    public async Task Dado_ClienteComVeiculos_Quando_ObterPorClienteAsync_Entao_RetornaLista()
    {
        var clienteId = Guid.NewGuid();
        var veiculos = new List<Veiculo> { CriarVeiculo() };

        _repositorioMock.Setup(r => r.ObterPorFiltroAsync(It.IsAny<ObterVeiculoPorClienteEspecificacao>()))
            .ReturnsAsync(veiculos);
        _mapperMock.Setup(m => m.Map<IEnumerable<VeiculoResponse>>(veiculos))
            .Returns(new List<VeiculoResponse> { new VeiculoResponse() });

        var result = await _servico.ObterPorClienteAsync(clienteId);

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task Dado_ClienteSemVeiculos_Quando_ObterPorClienteAsync_Entao_LancaExcecaoDadosNaoEncontrados()
    {
        _repositorioMock.Setup(r => r.ObterPorFiltroAsync(It.IsAny<ObterVeiculoPorClienteEspecificacao>()))
            .ReturnsAsync((IEnumerable<Veiculo>)null!);

        await Assert.ThrowsAsync<DadosNaoEncontradosException>(() => _servico.ObterPorClienteAsync(Guid.NewGuid()));
    }
    [Fact]
    public async Task Dado_PlacaExistente_Quando_ObterPorPlacaAsync_Entao_RetornaVeiculoResponse()
    {
        var veiculo = CriarVeiculo();
        _repositorioMock.Setup(r => r.ObterPorFiltroAsync(It.IsAny<ObterVeiculoPorPlacaEspecificacao>()))
            .ReturnsAsync(new List<Veiculo> { veiculo });
        _mapperMock.Setup(m => m.Map<VeiculoResponse>(veiculo)).Returns(new VeiculoResponse());

        var result = await _servico.ObterPorPlacaAsync("ABC1234");

        Assert.NotNull(result);
    }

    [Fact]
    public async Task Dado_PlacaInexistente_Quando_ObterPorPlacaAsync_Entao_LancaExcecaoDadosNaoEncontrados()
    {
        _repositorioMock.Setup(r => r.ObterPorFiltroAsync(It.IsAny<ObterVeiculoPorPlacaEspecificacao>()))
            .ReturnsAsync((IEnumerable<Veiculo>)null!);

        await Assert.ThrowsAsync<DadosNaoEncontradosException>(() => _servico.ObterPorPlacaAsync("XXXX9999"));
    }
    [Fact]
    public async Task Quando_ObterTodosAsync_Entao_RetornaListaVeiculoResponse()
    {
        var veiculos = new List<Veiculo> { CriarVeiculo() };

        _repositorioMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(veiculos);
        _mapperMock.Setup(m => m.Map<IEnumerable<VeiculoResponse>>(veiculos))
            .Returns(new List<VeiculoResponse> { new VeiculoResponse() });

        var result = await _servico.ObterTodosAsync();

        Assert.NotNull(result);
        Assert.Single(result);
    }
    [Fact]
    public async Task Dado_IdExistente_Quando_DeletarAsync_Entao_RetornaTrue()
    {
        var veiculo = CriarVeiculo();
        _repositorioMock.Setup(r => r.ObterPorIdAsync(veiculo.Id)).ReturnsAsync(veiculo);
        _repositorioMock.Setup(r => r.DeletarAsync(veiculo)).Returns(Task.CompletedTask);
        _uotMock.Setup(u => u.Commit()).ReturnsAsync(true);

        var result = await _servico.DeletarAsync(veiculo.Id);

        Assert.True(result);
    }

    [Fact]
    public async Task Dado_IdInexistente_Quando_DeletarAsync_Entao_LancaExcecaoDadosNaoEncontrados()
    {
        _repositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Veiculo)null!);

        await Assert.ThrowsAsync<DadosNaoEncontradosException>(() => _servico.DeletarAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task Dado_FalhaNoCommit_Quando_DeletarAsync_Entao_LancaExcecaoPersistirDados()
    {
        var veiculo = CriarVeiculo();
        _repositorioMock.Setup(r => r.ObterPorIdAsync(veiculo.Id)).ReturnsAsync(veiculo);
        _repositorioMock.Setup(r => r.DeletarAsync(veiculo)).Returns(Task.CompletedTask);
        _uotMock.Setup(u => u.Commit()).ReturnsAsync(false);

        await Assert.ThrowsAsync<PersistirDadosException>(() => _servico.DeletarAsync(veiculo.Id));
    }
}
