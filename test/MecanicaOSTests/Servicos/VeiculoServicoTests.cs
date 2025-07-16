using Aplicacao.DTOs.Requests.Veiculo;
using Aplicacao.DTOs.Responses.Veiculo;
using Aplicacao.Servicos;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Especificacoes;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class VeiculoServicoTests
{
    private readonly Mock<IRepositorio<Veiculo>> _repositorioMock;
    private readonly Mock<ILogServico<VeiculoServico>> _logMock;
    private readonly Mock<IUnidadeDeTrabalho> _uotMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly VeiculoServico _servico;

    public VeiculoServicoTests()
    {
        _repositorioMock = new Mock<IRepositorio<Veiculo>>();
        _logMock = new Mock<ILogServico<VeiculoServico>>();
        _uotMock = new Mock<IUnidadeDeTrabalho>();
        _mapperMock = new Mock<IMapper>();
        _servico = new VeiculoServico(_repositorioMock.Object, _logMock.Object, _uotMock.Object, _mapperMock.Object);
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
    //[Fact]
    //public async Task Given_ValidRequest_When_CadastrarAsync_Then_ReturnVeiculoResponse()
    //{
    //    // Arrange
    //    var request = new CadastrarVeiculoRequest { Placa = "XYZ1234" };
    //    var veiculo = CriarVeiculo();

    //    _mapperMock.Setup(m => m.Map<Veiculo>(request)).Returns(veiculo);
    //    _repositorioMock.Setup(r => r.CadastrarAsync(veiculo)).Returns(Task.CompletedTask);
    //    _uotMock.Setup(u => u.Commit()).ReturnsAsync(true);
    //    _mapperMock.Setup(m => m.Map<VeiculoResponse>(veiculo)).Returns(new VeiculoResponse());

    //    // Act
    //    var result = await _servico.CadastrarAsync(request);

    //    // Assert
    //    Assert.NotNull(result);
    //}

    //[Fact]
    //public async Task Given_CommitFails_When_CadastrarAsync_Then_ThrowPersistirDadosException()
    //{
    //    var request = new CadastrarVeiculoRequest { Placa = "XYZ1234" };
    //    var veiculo = CriarVeiculo();

    //    _mapperMock.Setup(m => m.Map<Veiculo>(request)).Returns(veiculo);
    //    _repositorioMock.Setup(r => r.CadastrarAsync(It.IsAny<Veiculo>())).Returns(Task.CompletedTask);
    //    _uotMock.Setup(u => u.Commit()).ReturnsAsync(false);

    //    await Assert.ThrowsAsync<PersistirDadosException>(() => _servico.CadastrarAsync(request));
    //}
    [Fact]
    public async Task Given_ValidIdAndRequest_When_AtualizarAsync_Then_UpdateAndReturnResponse()
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
    public async Task Given_NonexistentId_When_AtualizarAsync_Then_ThrowDadosNaoEncontradosException()
    {
        _repositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Veiculo)null!);

        await Assert.ThrowsAsync<DadosNaoEncontradosException>(() => _servico.AtualizarAsync(Guid.NewGuid(), new AtualizarVeiculoRequest()));
    }

    [Fact]
    public async Task Given_CommitFails_When_AtualizarAsync_Then_ThrowPersistirDadosException()
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
    public async Task Given_ExistingId_When_ObterPorIdAsync_Then_ReturnVeiculoResponse()
    {
        var veiculo = CriarVeiculo();
        _repositorioMock.Setup(r => r.ObterPorIdAsync(veiculo.Id)).ReturnsAsync(veiculo);
        _mapperMock.Setup(m => m.Map<VeiculoResponse>(veiculo)).Returns(new VeiculoResponse());

        var result = await _servico.ObterPorIdAsync(veiculo.Id);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task Given_InvalidId_When_ObterPorIdAsync_Then_ThrowDadosNaoEncontradosException()
    {
        _repositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Veiculo)null!);

        await Assert.ThrowsAsync<DadosNaoEncontradosException>(() => _servico.ObterPorIdAsync(Guid.NewGuid()));
    }
    [Fact]
    public async Task Given_ClienteComVeiculos_When_ObterPorClienteAsync_Then_ReturnLista()
    {
        var clienteId = Guid.NewGuid();
        var veiculos = new List<Veiculo> { CriarVeiculo() };

        _repositorioMock.Setup(r => r.ObterPorFiltroAsync(It.IsAny<ObterVeiculoPorClienteEspecificacao>()))
            .ReturnsAsync(veiculos);
        _mapperMock.Setup(m => m.Map<IEnumerable<VeiculoResponse>>(veiculos)).Returns(new List<VeiculoResponse>());

        var result = await _servico.ObterPorClienteAsync(clienteId);

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task Given_ClienteSemVeiculos_When_ObterPorClienteAsync_Then_ThrowDadosNaoEncontradosException()
    {
        _repositorioMock.Setup(r => r.ObterPorFiltroAsync(It.IsAny<ObterVeiculoPorClienteEspecificacao>()))
            .ReturnsAsync((IEnumerable<Veiculo>)null!);

        await Assert.ThrowsAsync<DadosNaoEncontradosException>(() => _servico.ObterPorClienteAsync(Guid.NewGuid()));
    }
    [Fact]
    public async Task Given_PlacaExistente_When_ObterPorPlacaAsync_Then_ReturnVeiculoResponse()
    {
        var veiculo = CriarVeiculo();
        _repositorioMock.Setup(r => r.ObterPorFiltroAsync(It.IsAny<ObterVeiculoPorPlacaEspecificacao>()))
            .ReturnsAsync(new List<Veiculo> { veiculo });
        _mapperMock.Setup(m => m.Map<VeiculoResponse>(veiculo)).Returns(new VeiculoResponse());

        var result = await _servico.ObterPorPlacaAsync("ABC1234");

        Assert.NotNull(result);
    }

    [Fact]
    public async Task Given_PlacaInexistente_When_ObterPorPlacaAsync_Then_ThrowDadosNaoEncontradosException()
    {
        _repositorioMock.Setup(r => r.ObterPorFiltroAsync(It.IsAny<ObterVeiculoPorPlacaEspecificacao>()))
            .ReturnsAsync((IEnumerable<Veiculo>)null!);

        await Assert.ThrowsAsync<DadosNaoEncontradosException>(() => _servico.ObterPorPlacaAsync("XXXX9999"));
    }
    [Fact]
    public async Task When_ObterTodosAsync_Then_ReturnListaVeiculoResponse()
    {
        var veiculos = new List<Veiculo> { CriarVeiculo() };

        _repositorioMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(veiculos);
        _mapperMock.Setup(m => m.Map<IEnumerable<VeiculoResponse>>(veiculos)).Returns(new List<VeiculoResponse>());

        var result = await _servico.ObterTodosAsync();

        Assert.NotNull(result);
        Assert.Single(result);
    }
    [Fact]
    public async Task Given_IdExistente_When_DeletarAsync_Then_ReturnTrue()
    {
        var veiculo = CriarVeiculo();
        _repositorioMock.Setup(r => r.ObterPorIdAsync(veiculo.Id)).ReturnsAsync(veiculo);
        _repositorioMock.Setup(r => r.DeletarAsync(veiculo)).Returns(Task.CompletedTask);
        _uotMock.Setup(u => u.Commit()).ReturnsAsync(true);

        var result = await _servico.DeletarAsync(veiculo.Id);

        Assert.True(result);
    }

    [Fact]
    public async Task Given_IdInexistente_When_DeletarAsync_Then_ThrowDadosNaoEncontradosException()
    {
        _repositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Veiculo)null!);

        await Assert.ThrowsAsync<DadosNaoEncontradosException>(() => _servico.DeletarAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task Given_CommitFails_When_DeletarAsync_Then_ThrowPersistirDadosException()
    {
        var veiculo = CriarVeiculo();
        _repositorioMock.Setup(r => r.ObterPorIdAsync(veiculo.Id)).ReturnsAsync(veiculo);
        _repositorioMock.Setup(r => r.DeletarAsync(veiculo)).Returns(Task.CompletedTask);
        _uotMock.Setup(u => u.Commit()).ReturnsAsync(false);

        await Assert.ThrowsAsync<PersistirDadosException>(() => _servico.DeletarAsync(veiculo.Id));
    }
}
