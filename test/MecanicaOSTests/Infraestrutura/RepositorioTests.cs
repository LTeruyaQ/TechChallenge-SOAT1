using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Especificacoes.Base;
using Infraestrutura.Dados;
using Infraestrutura.Repositorios;
using MecanicaOSTests.Infraestrutura.Projecoes;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq.Expressions;

namespace MecanicaOSTests.Infraestrutura;

public class RepositorioTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<MecanicaContexto> _options;
    private readonly MecanicaContexto _contexto;
    private readonly Repositorio<OrdemServico> _repositorio;
    private readonly Mock<IMediator> _mediatR;

    public RepositorioTests()
    {
        _mediatR = new Mock<IMediator>();
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<MecanicaContexto>()
            .UseSqlite(_connection)
            .EnableSensitiveDataLogging()
            .Options;

        _contexto = new MecanicaContexto(_options, _mediatR.Object);
        _contexto.Database.EnsureCreated();

        _repositorio = new Repositorio<OrdemServico>(_contexto);
    }

    private async Task<Guid> CriarDadosTeste()
    {
        var cliente = new Cliente
        {
            Nome = "Cliente Teste",
            Documento = "12345678901",
            DataNascimento = "1990-01-01",
            TipoCliente = TipoCliente.PessoaFisica,
            Contato = new Contato
            {
                Email = "cliente@teste.com",
                Telefone = "11999999999"
            },
            Endereco = new Endereco
            {
                Rua = "Rua A",
                Numero = "123",
                Bairro = "Centro",
                Cidade = "São Paulo"
            }
        };

        var servico = new Servico
        {
            Nome = "Troca de Óleo",
            Descricao = "Troca completa de óleo do motor",
            Valor = 150m,
            Disponivel = true
        };

        var veiculo = new Veiculo
        {
            Placa = "ABC1234",
            Modelo = "Civic",
            Cor = "Prata",
            Marca = "Honda",
            Ano = "2020",
            Cliente = cliente
        };

        var ordemServico = new OrdemServico
        {
            Cliente = cliente,
            Servico = servico,
            Veiculo = veiculo,
            Status = StatusOrdemServico.AguardandoAprovação,
            DataCadastro = DateTime.UtcNow
        };

        await _contexto.AddAsync(ordemServico);
        await _contexto.SaveChangesAsync();

        return ordemServico.Id;
    }



    [Fact]
    public async Task Dado_OrdemServico_Quando_ObterPorFiltroPaginadoSemRastreamento_Entao_RetornaSemRastreamento()
    {
        var id = await CriarDadosTeste();

        var especificacao = new OrdemServicoPaginadaEspecificacao(id, pagina: 0, tamanho: 10);

        var resultado = await _repositorio.ListarSemRastreamentoAsync(especificacao);

        Assert.Single(resultado);
        var ordemServico = resultado.First();
        Assert.Equal(id, ordemServico.Id);
    }

    [Fact]
    public async Task Dado_OrdemServico_Quando_ObterPorFiltroPaginado_Entao_RetornaComRastreamento()
    {
        var id = await CriarDadosTeste();

        var especificacao = new OrdemServicoPaginadaEspecificacao(id, pagina: 0, tamanho: 10);

        var resultado = await _repositorio.ListarAsync(especificacao);

        Assert.Single(resultado);
        var ordemServico = resultado.First();
        Assert.Equal(id, ordemServico.Id);
        Assert.True(_contexto.Entry(ordemServico).IsKeySet);
    }

    private class OrdemServicoProjetadaEspecificacao : EspecificacaoBase<OrdemServico>
    {
        private readonly Guid _id;

        public OrdemServicoProjetadaEspecificacao(Guid id)
        {
            _id = id;
            AdicionarInclusao(os => os.Cliente);
            AdicionarInclusao(os => os.Veiculo);
            AdicionarInclusao(os => os.Servico);

            DefinirProjecao(os => new OrdemServicoProjecao
            {
                Id = os.Id,
                ClienteNome = os.Cliente.Nome,
                VeiculoModelo = os.Veiculo.Modelo,
                ServicoNome = os.Servico.Nome,
                Status = os.Status.ToString(),
                DataCadastro = os.DataCadastro
            });
        }

        public override Expression<Func<OrdemServico, bool>> Expressao =>
            os => os.Id == _id;
    }

    [Fact]
    public async Task Dado_OrdemServico_Quando_ObterProjetado_Entao_RetornaProjecao()
    {
        var id = await CriarDadosTeste();

        var especificacao = new OrdemServicoProjetadaEspecificacao(id);
        var resultado = await _repositorio.ObterUmProjetadoAsync<OrdemServicoProjecao>(especificacao);

        Assert.NotNull(resultado);
        Assert.Equal(id, resultado.Id);
        Assert.Equal("Cliente Teste", resultado.ClienteNome);
        Assert.Equal("Civic", resultado.VeiculoModelo);
        Assert.Equal("Troca de Óleo", resultado.ServicoNome);
    }

    [Fact]
    public async Task Dado_OrdemServico_Quando_ObterProjetadoSemRastreamento_Entao_RetornaProjecaoSemRastreamento()
    {
        var id = await CriarDadosTeste();

        var especificacao = new OrdemServicoProjetadaEspecificacao(id);
        var resultado = await _repositorio.ObterUmProjetadoSemRastreamentoAsync<OrdemServicoProjecao>(especificacao);

        Assert.NotNull(resultado);
        Assert.Equal(id, resultado.Id);
        Assert.Equal("Cliente Teste", resultado.ClienteNome);
    }

    private class ListarOrdensServicoProjetadasEspecificacao : EspecificacaoBase<OrdemServico>
    {
        private readonly Guid _id;

        public ListarOrdensServicoProjetadasEspecificacao(Guid id)
        {
            _id = id;
            AdicionarInclusao(os => os.Cliente);

            DefinirProjecao(os => new OrdemServicoProjecao
            {
                Id = os.Id,
                ClienteNome = os.Cliente.Nome,
                Status = os.Status.ToString()
            });
        }

        public override Expression<Func<OrdemServico, bool>> Expressao =>
            os => os.Id == _id;
    }

    [Fact]
    public async Task Dado_OrdemServico_Quando_ListarProjetado_Entao_RetornaListaProjecao()
    {
        var id = await CriarDadosTeste();

        var especificacao = new ListarOrdensServicoProjetadasEspecificacao(id);
        var resultado = await _repositorio.ListarProjetadoAsync<OrdemServicoProjecao>(especificacao);

        Assert.Single(resultado);
        var projecao = resultado.First();
        Assert.Equal(id, projecao.Id);
        Assert.Equal("Cliente Teste", projecao.ClienteNome);
    }

    [Fact]
    public async Task Dado_OrdemServico_Quando_ListarProjetadoSemRastreamento_Entao_RetornaListaProjecaoSemRastreamento()
    {
        var id = await CriarDadosTeste();

        var especificacao = new ListarOrdensServicoProjetadasEspecificacao(id);
        var resultado = await _repositorio.ListarProjetadoSemRastreamentoAsync<OrdemServicoProjecao>(especificacao);

        Assert.Single(resultado);
        var projecao = resultado.First();
        Assert.Equal(id, projecao.Id);
        Assert.Equal("Cliente Teste", projecao.ClienteNome);
    }

    private class ListarOrdensServicoPaginadasEspecificacao : EspecificacaoBase<OrdemServico>
    {
        private readonly Guid _id;

        public ListarOrdensServicoPaginadasEspecificacao(Guid id, int pagina = 0, int tamanho = 10)
        {
            _id = id;
            AdicionarPaginacao(pagina, tamanho);
            AdicionarInclusao(os => os.Cliente);

            DefinirProjecao(os => new OrdemServicoProjecao
            {
                Id = os.Id,
                ClienteNome = os.Cliente.Nome,
                Status = os.Status.ToString()
            });
        }

        public override Expression<Func<OrdemServico, bool>> Expressao =>
            os => os.Id == _id;
    }

    private class OrdemServicoPaginadaEspecificacao : EspecificacaoBase<OrdemServico>
    {
        private readonly Guid? _id;

        public OrdemServicoPaginadaEspecificacao(Guid? id = null, int pagina = 0, int tamanho = 10)
        {
            _id = id;
            AdicionarPaginacao(pagina, tamanho);
        }

        public override Expression<Func<OrdemServico, bool>> Expressao =>
            os => _id == null || os.Id == _id;
    }

    [Fact]
    public async Task Dado_OrdemServico_Quando_ListarProjetadoComPaginacao_Entao_RetornaListaPaginada()
    {
        var id = await CriarDadosTeste();

        var especificacao = new ListarOrdensServicoPaginadasEspecificacao(id);
        var resultado = await _repositorio.ListarProjetadoAsync<OrdemServicoProjecao>(especificacao);

        Assert.Single(resultado);
        var projecao = resultado.First();
        Assert.Equal(id, projecao.Id);
        Assert.Equal("Cliente Teste", projecao.ClienteNome);
    }

    [Fact]
    public async Task Dado_OrdemServico_Quando_ListarProjetadoComPaginacaoSemRastreamento_Entao_RetornaListaPaginadaSemRastreamento()
    {
        var id = await CriarDadosTeste();

        var especificacao = new ListarOrdensServicoPaginadasEspecificacao(id);
        var resultado = await _repositorio.ListarProjetadoSemRastreamentoAsync<OrdemServicoProjecao>(especificacao);

        Assert.Single(resultado);
        var projecao = resultado.First();
        Assert.Equal(id, projecao.Id);
        Assert.Equal("Cliente Teste", projecao.ClienteNome);
    }

    public void Dispose()
    {
        _contexto.Database.EnsureDeleted();
        _contexto.Dispose();
        _connection.Dispose();
    }
}
