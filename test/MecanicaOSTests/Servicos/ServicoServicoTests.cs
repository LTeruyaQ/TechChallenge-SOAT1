using Aplicacao.DTOs.Requests.Servico;
using Aplicacao.DTOs.Responses.Servico;
using Aplicacao.Servicos;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Especificacoes.Base.Interfaces;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using FluentAssertions;
using Moq;

public class ServicoServicoTests
{
    private readonly Mock<IRepositorio<Servico>> _repositorioMock = new();
    private readonly Mock<ILogServico<ServicoServico>> _logServicoMock = new();
    private readonly Mock<IUnidadeDeTrabalho> _uotMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IUsuarioLogadoServico> _usuarioLogadoServico = new();

    private readonly ServicoServico _servicoServico;

    public ServicoServicoTests()
    {
        _servicoServico = new ServicoServico(
            _repositorioMock.Object,
            _logServicoMock.Object,
            _uotMock.Object,
            _mapperMock.Object,
            _usuarioLogadoServico.Object);
    }

    [Fact]
    public async Task Dado_NomeExistente_Quando_CadastrarServicoAsync_Entao_LancaExcecaoDadosJaCadastrados()
    {
        // Arrange
        var request = new CadastrarServicoRequest
        {
            Nome = "Serviço X",
            Descricao = "Descrição detalhada do serviço",
            Valor = 150.50m,
            Disponivel = true
        };

        var servicoExistente = new Servico
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome,
            Descricao = "Serviço já existente",
            Valor = 100.00m,
            Disponivel = true
        };

        var servicoResponse = new ServicoResponse
        {
            Id = servicoExistente.Id,
            Nome = servicoExistente.Nome,
            Descricao = servicoExistente.Descricao,
            Valor = servicoExistente.Valor,
            Disponivel = servicoExistente.Disponivel
        };

        _repositorioMock
            .Setup(r => r.ObterUmSemRastreamentoAsync(It.IsAny<IEspecificacao<Servico>>()))
            .ReturnsAsync(servicoExistente);

        _mapperMock
            .Setup(m => m.Map<ServicoResponse>(servicoExistente))
            .Returns(servicoResponse);

        // Act & Assert
        var excecao = await Assert.ThrowsAsync<DadosJaCadastradosException>(
            () => _servicoServico.CadastrarServicoAsync(request));

        excecao.Message.Should().Be("Serviço já cadastrado");

        _repositorioMock.Verify(
            r => r.ObterUmSemRastreamentoAsync(It.IsAny<IEspecificacao<Servico>>()),
            Times.Once,
            "ObterUmSemRastreamentoAsync deveria ter sido chamado uma vez");

        _repositorioMock.Verify(
            r => r.CadastrarAsync(It.IsAny<Servico>()),
            Times.Never,
            "CadastrarAsync não deveria ter sido chamado");

        _uotMock.Verify(
            u => u.Commit(),
            Times.Never,
            "Commit não deveria ter sido chamado");
    }

    [Fact]
    public async Task Dado_RequestValido_Quando_CadastrarServicoAsync_Entao_RetornaServicoResponse()
    {
        var request = new CadastrarServicoRequest
        {
            Nome = "Novo Serviço",
            Descricao = "Descrição",
            Valor = 100,
            Disponivel = true
        };

        var entidade = new Servico { Descricao = "teste", Nome = "teste" };
        var response = new ServicoResponse();

        _servicoServicoTestSetupObterServicoPorNome(request.Nome, null);
        _mapperMock.Setup(m => m.Map<Servico>(request)).Returns(entidade);
        _repositorioMock.Setup(r => r.CadastrarAsync(entidade)).ReturnsAsync(entidade);
        _uotMock.Setup(u => u.Commit()).ReturnsAsync(true);
        _mapperMock.Setup(m => m.Map<ServicoResponse>(entidade)).Returns(response);

        var result = await _servicoServico.CadastrarServicoAsync(request);

        Assert.Equal(response, result);
    }

    [Fact]
    public async Task Dado_FalhaNoCommit_Quando_CadastrarServicoAsync_Entao_LancaExcecaoPersistirDados()
    {
        var request = new CadastrarServicoRequest { Nome = "Novo", Descricao = "descricao", Valor = 20, Disponivel = true };
        var entidade = new Servico { Descricao = "teste", Nome = "teste" };

        _servicoServicoTestSetupObterServicoPorNome(request.Nome, null);
        _mapperMock.Setup(m => m.Map<Servico>(request)).Returns(entidade);
        _repositorioMock.Setup(r => r.CadastrarAsync(entidade)).ReturnsAsync(entidade);
        _uotMock.Setup(u => u.Commit()).ReturnsAsync(false);

        await Assert.ThrowsAsync<PersistirDadosException>(() => _servicoServico.CadastrarServicoAsync(request));
    }

    [Fact]
    public async Task Dado_IdInvalido_Quando_DeletarServicoAsync_Entao_LancaExcecaoDadosNaoEncontrados()
    {
        var id = Guid.NewGuid();
        _repositorioMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync((Servico?)null);

        await Assert.ThrowsAsync<DadosNaoEncontradosException>(() => _servicoServico.DeletarServicoAsync(id));
    }

    [Fact]
    public async Task Dado_FalhaNoCommit_Quando_DeletarServicoAsync_Entao_LancaExcecaoPersistirDados()
    {
        var id = Guid.NewGuid();
        var servico = new Servico { Descricao = "teste", Nome = "teste" };

        _repositorioMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(servico);
        _repositorioMock.Setup(r => r.DeletarAsync(servico)).Returns(Task.CompletedTask);
        _uotMock.Setup(u => u.Commit()).ReturnsAsync(false);

        await Assert.ThrowsAsync<PersistirDadosException>(() => _servicoServico.DeletarServicoAsync(id));
    }

    [Fact]
    public async Task Dado_IdValido_Quando_EditarServicoAsync_Entao_RetornaServicoResponse()
    {
        var id = Guid.NewGuid();
        var request = new EditarServicoRequest
        {
            Nome = "Atualizado",
            Descricao = "Desc",
            Valor = 150,
            Disponivel = false
        };

        var servico = new Mock<Servico>();
        var response = new ServicoResponse();

        _repositorioMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(servico.Object);
        _repositorioMock.Setup(r => r.EditarAsync(servico.Object)).Returns(Task.CompletedTask);
        _uotMock.Setup(u => u.Commit()).ReturnsAsync(true);
        _mapperMock.Setup(m => m.Map<ServicoResponse>(servico.Object)).Returns(response);

        var result = await _servicoServico.EditarServicoAsync(id, request);

        Assert.Equal(response, result);
    }

    [Fact]
    public async Task Dado_FalhaNoCommit_Quando_EditarServicoAsync_Entao_LancaExcecaoPersistirDados()
    {
        var id = Guid.NewGuid();
        var request = new EditarServicoRequest() { Descricao = "descricao", Disponivel = true, Nome = "Nome Servico", Valor = 30 };
        var servico = new Servico { Descricao = "teste", Nome = "teste" };

        _repositorioMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(servico);
        _repositorioMock.Setup(r => r.EditarAsync(servico)).Returns(Task.CompletedTask);
        _uotMock.Setup(u => u.Commit()).ReturnsAsync(false);

        await Assert.ThrowsAsync<PersistirDadosException>(() => _servicoServico.EditarServicoAsync(id, request));
    }

    [Fact]
    public async Task Dado_IdInvalido_Quando_ObterServicoPorIdAsync_Entao_LancaExcecaoDadosNaoEncontrados()
    {
        _repositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Servico?)null);

        await Assert.ThrowsAsync<DadosNaoEncontradosException>(() => _servicoServico.ObterServicoPorIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task Dado_IdValido_Quando_ObterServicoPorIdAsync_Entao_RetornaServicoResponse()
    {
        var servico = new Servico { Descricao = "teste", Nome = "teste" };
        var response = new ServicoResponse();

        _repositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(servico);
        _mapperMock.Setup(m => m.Map<ServicoResponse>(servico)).Returns(response);

        var result = await _servicoServico.ObterServicoPorIdAsync(Guid.NewGuid());

        Assert.Equal(response, result);
    }

    [Fact]
    public async Task Dado_RepositorioVazio_Quando_ObterTodosAsync_Entao_RetornaListaVazia()
    {
        _repositorioMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(new List<Servico>());
        _mapperMock.Setup(m => m.Map<IEnumerable<ServicoResponse>>(It.IsAny<IEnumerable<Servico>>()))
                  .Returns(Array.Empty<ServicoResponse>());

        var resultado = await _servicoServico.ObterTodosAsync();

        Assert.Empty(resultado);
    }

    [Fact]
    public async Task Dado_ServicosDisponiveis_Quando_ObterServicosDisponiveisAsync_Entao_RetornaListaDeServicos()
    {
        var lista = new List<Servico>();
        var listaResponse = new List<ServicoResponse> { new ServicoResponse() };

        _repositorioMock.Setup(r => r.ObterPorFiltroAsync(It.IsAny<IEspecificacao<Servico>>()))
                        .ReturnsAsync(lista);
        _mapperMock.Setup(m => m.Map<IEnumerable<ServicoResponse>>(lista))
                   .Returns(listaResponse);

        var resultado = await _servicoServico.ObterServicosDisponiveisAsync();

        Assert.Single(resultado);
    }

    [Fact]
    public async Task Dado_NomeInexistente_Quando_ObterServicoPorNomeAsync_Entao_RetornaNulo()
    {
        _repositorioMock.Setup(r => r.ObterUmSemRastreamentoAsync(It.IsAny<IEspecificacao<Servico>>()))
                        .ReturnsAsync((Servico?)null);

        var resultado = await _servicoServico.ObterServicoPorNomeAsync("Inexistente");

        Assert.Null(resultado);
    }

    private void _servicoServicoTestSetupObterServicoPorNome(string nome, Servico? servico)
    {
        _repositorioMock
            .Setup(r => r.ObterUmSemRastreamentoAsync(It.IsAny<IEspecificacao<Servico>>()))
            .ReturnsAsync(servico);
    }
}