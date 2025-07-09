using Dominio.Especificacoes.Base;
using Dominio.Especificacoes.Base.Extensoes;
using System.Linq.Expressions;
using Testes.Especificacoes.Base.Modelos;

namespace Testes.Especificacoes;
public class OrdemServicoEspecificacaoTests
{
    [Fact]
    public void Deve_FiltrarOrdensPorCliente_Quando_EspecificacaoAplicada()
    {
        // Arrange
        var clienteId = 1;
        var espec = new OrdemServicoPorClienteEspecificacao(clienteId);

        // Adiciona as inclusões antes de usar a especificação
        espec = (OrdemServicoPorClienteEspecificacao)espec
            .Incluir(o => o.Cliente)
            .Incluir("Itens.Produto");

        var ordens = CriarOrdensParaTeste();

        // Act
        var resultado = ordens.Where(espec.Expressao.Compile()).ToList();

        // Assert
        Assert.All(resultado, o => Assert.Equal(clienteId, o.ClienteId));
        Assert.All(resultado, o => Assert.NotNull(o.Cliente));
        Assert.All(resultado, o => Assert.All(o.Itens, i => Assert.NotNull(i.Produto)));
    }

    [Fact]
    public void Deve_FiltrarOrdensEmAberto_Quando_EspecificacaoCombinada()
    {
        // Arrange
        var especStatus = new OrdemServicoStatusEspecificacao(StatusOrdemServico.Aberta);
        var especData = new OrdemServicoDataMaiorQueEspecificacao(DateTime.Now.AddDays(-7));

        // Adiciona as inclusões antes de combinar
        especStatus = (OrdemServicoStatusEspecificacao)especStatus
            .Incluir(o => o.Cliente)
            .Incluir(o => o.Tecnico);

        especData = (OrdemServicoDataMaiorQueEspecificacao)especData
            .Incluir(o => o.Cliente)
            .Incluir(o => o.Tecnico);

        var especCombinada = especStatus.E(especData);

        var ordens = CriarOrdensParaTeste();

        // Act
        var resultado = ordens.Where(especCombinada.Expressao.Compile()).ToList();

        // Assert
        Assert.All(resultado, o =>
        {
            Assert.Equal(StatusOrdemServico.Aberta, o.Status);
            Assert.True(o.DataAbertura >= DateTime.Now.AddDays(-7));
            Assert.NotNull(o.Cliente);
            Assert.NotNull(o.Tecnico);
        });
    }

    [Fact]
    public void Deve_ManterInclusoesAposCombinacao_Quando_EspecificacoesComInclusoesDiferentes()
    {
        // Arrange
        var espec1 = new OrdemServicoPorClienteEspecificacao(1);
        var espec2 = new OrdemServicoStatusEspecificacao(StatusOrdemServico.Aberta);

        // Adiciona as inclusões antes de combinar
        espec1 = (OrdemServicoPorClienteEspecificacao)espec1
            .Incluir(o => o.Cliente);

        espec2 = (OrdemServicoStatusEspecificacao)espec2
            .Incluir(o => o.Tecnico)
            .Incluir("Itens.Produto");

        // Act
        var combinada = espec1.E(espec2);

        // Assert
        Assert.Contains(combinada.Inclusoes, e => e.ToString().Contains("Cliente"));
        Assert.Contains(combinada.Inclusoes, e => e.ToString().Contains("Tecnico"));
        Assert.Contains(combinada.InclusoesPorString, s => s.Contains("Itens.Produto"));
    }

    [Fact]
    public void Deve_RemoverInclusoesDuplicadas_Quando_CombinarEspecificacoesComInclusoesIguais()
    {
        // Arrange
        var espec1 = new OrdemServicoPorClienteEspecificacao(1);
        var espec2 = new OrdemServicoStatusEspecificacao(StatusOrdemServico.Aberta);

        // Adiciona as inclusões antes de combinar
        espec1 = (OrdemServicoPorClienteEspecificacao)espec1
            .Incluir(o => o.Cliente);

        espec2 = (OrdemServicoStatusEspecificacao)espec2
            .Incluir(o => o.Cliente);

        // Act
        var combinada = espec1.E(espec2);

        // Assert
        Assert.Single(combinada.Inclusoes);
    }

    [Fact]
    public void Deve_CombinarEspecificacoesDeClassesDiferentes_ComSucesso()
    {
        // Arrange
        // Especificação para OrdemServico: Ordens abertas
        var ordemEspec = new OrdemServicoStatusEspecificacao(StatusOrdemServico.Aberta);

        // Especificação para ItemOrdemServico: Itens com valor unitário maior que 150
        var itemEspec = new ItemValorMaiorQueEspecificacao(150);

        // Adiciona as inclusões antes de usar as especificações
        ordemEspec = (OrdemServicoStatusEspecificacao)ordemEspec
            .Incluir(o => o.Cliente);

        itemEspec = (ItemValorMaiorQueEspecificacao)itemEspec
            .Incluir(i => i.Produto);

        // Criar dados de teste
        var ordens = CriarOrdensParaTeste();

        // Act
        // 1. Primeiro filtramos as ordens abertas
        var ordensAbertas = ordens.Where(ordemEspec.Expressao.Compile()).ToList();

        // 2. Para cada ordem aberta, filtramos os itens que atendem ao critério
        var resultado = ordensAbertas
            .Where(ordem => ordem.Itens.Any(item => itemEspec.Expressao.Compile()(item)))
            .ToList();

        // Assert
        // Deve retornar apenas a ordem 1 que tem um item com valor 200 (> 150)
        Assert.Single(resultado);
        Assert.Equal(1, resultado[0].Id);

        // Verifica se as inclusões foram mantidas
        Assert.NotNull(resultado[0].Cliente);
        Assert.All(resultado[0].Itens, item =>
        {
            if (item.ValorUnitario > 150)
            {
                Assert.NotNull(item.Produto);
            }
        });
    }

    private IQueryable<OrdemServico> CriarOrdensParaTeste()
    {
        var categoria1 = new Categoria { Id = 1, Nome = "Categoria 1" };
        var categoria2 = new Categoria { Id = 2, Nome = "Categoria 2" };

        var cliente1 = new Cliente { Id = 1, Nome = "Cliente 1" };
        var cliente2 = new Cliente { Id = 2, Nome = "Cliente 2" };
        var tecnico1 = new Tecnico { Id = 1, Nome = "Técnico 1" };
        var produto1 = new Produto { Id = 1, Nome = "Produto 1", Preco = 100, Ativo = true, Categoria = categoria1, CategoriaId = 1 };
        var produto2 = new Produto { Id = 2, Nome = "Produto 2", Preco = 200, Ativo = true, Categoria = categoria2, CategoriaId = 2 };

        var ordens = new List<OrdemServico>
        {
            new()
            {
                Id = 1,
                Cliente = cliente1,
                ClienteId = 1,
                Tecnico = tecnico1,
                Status = StatusOrdemServico.Aberta,
                DataAbertura = DateTime.Now.AddDays(-5),
                Itens = new List<ItemOrdemServico>
                {
                    new() { Id = 1, Produto = produto1, Quantidade = 1, ValorUnitario = 100 },
                    new() { Id = 3, Produto = produto2, Quantidade = 1, ValorUnitario = 200 }
                }
            },
            new()
            {
                Id = 2,
                Cliente = cliente2,
                ClienteId = 2,
                Status = StatusOrdemServico.Fechada,
                DataAbertura = DateTime.Now.AddDays(-10),
                DataFechamento = DateTime.Now.AddDays(-1),
                Itens = new List<ItemOrdemServico>
                {
                    new() { Id = 2, Produto = produto2, Quantidade = 2, ValorUnitario = 200 }
                }
            },
            new()
            {
                Id = 3,
                Cliente = cliente1,
                ClienteId = 1,
                Tecnico = tecnico1,
                Status = StatusOrdemServico.Aberta,
                DataAbertura = DateTime.Now.AddDays(-1),
                Itens = new List<ItemOrdemServico>()
            }
        };

        return ordens.AsQueryable();
    }

    [Fact]
    public void Deve_CombinarMultiplasEspecificacoesComMultiplosNiveis_ComSucesso()
    {
        // Arrange
        // Especificação para OrdemServico: Ordens abertas
        var ordemAbertaEspec = new OrdemServicoStatusEspecificacao(StatusOrdemServico.Aberta);

        // Especificação para OrdemServico: Ordens dos últimos 7 dias
        var ordemDataEspec = new OrdemServicoDataMaiorQueEspecificacao(DateTime.Now.AddDays(-7));

        // Especificação para ItemOrdemServico: Itens com valor unitário maior que 50
        var itemValorEspec = new ItemValorMaiorQueEspecificacao(50);

        // Especificação para Produto: Apenas produtos ativos
        var produtoAtivoEspec = new ProdutoAtivoEspecificacao();

        // Adiciona as inclusões antes de combinar as especificações
        ordemAbertaEspec = (OrdemServicoStatusEspecificacao)ordemAbertaEspec
            .Incluir(o => o.Cliente)
            .Incluir(o => o.Tecnico);

        itemValorEspec = (ItemValorMaiorQueEspecificacao)itemValorEspec
            .Incluir(i => i.Produto);

        // Criar dados de teste
        var ordens = CriarOrdensParaTeste();

        // Act
        // 1. Filtra as ordens que atendem aos critérios
        var resultado = ordens
            .Where(ordemAbertaEspec.Expressao.Compile())
            .Where(ordemDataEspec.Expressao.Compile())
            .Where(ordem =>
                // Verifica se a ordem tem itens que atendem aos critérios
                ordem.Itens.Any(item =>
                    itemValorEspec.Expressao.Compile()(item) &&
                    produtoAtivoEspec.Expressao.Compile()(item.Produto)))
            .ToList();

        // Assert
        // Deve retornar apenas as ordens que têm itens com valor > 50 e produto ativo
        Assert.NotEmpty(resultado);

        // Verifica se as inclusões foram mantidas
        Assert.All(resultado, ordem =>
        {
            Assert.NotNull(ordem.Cliente);
            Assert.NotNull(ordem.Tecnico);

            // Verifica os itens que atenderam aos critérios
            var itensFiltrados = ordem.Itens
                .Where(item => item.ValorUnitario > 50 && item.Produto?.Ativo == true)
                .ToList();

            Assert.NotEmpty(itensFiltrados);
            Assert.All(itensFiltrados, item =>
            {
                Assert.NotNull(item.Produto);
                Assert.True(item.ValorUnitario > 50);
            });
        });
    }
}


public enum StatusOrdemServico
{
    Aberta = 1,
    EmAndamento = 2,
    AguardandoPecas = 3,
    Fechada = 4,
    Cancelada = 5
}

public class OrdemServico
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public virtual Cliente Cliente { get; set; }
    public int? TecnicoId { get; set; }
    public virtual Tecnico Tecnico { get; set; }
    public StatusOrdemServico Status { get; set; }
    public DateTime DataAbertura { get; set; }
    public DateTime? DataFechamento { get; set; }
    public virtual ICollection<ItemOrdemServico> Itens { get; set; } = new List<ItemOrdemServico>();
}

// Nova classe de especificação para itens com valor maior que um valor mínimo
public class ItemValorMaiorQueEspecificacao : Especificacao<ItemOrdemServico>
{
    private readonly decimal _valorMinimo;

    public ItemValorMaiorQueEspecificacao(decimal valorMinimo)
    {
        _valorMinimo = valorMinimo;
    }

    public override Expression<Func<ItemOrdemServico, bool>> Expressao =>
        item => item.ValorUnitario > _valorMinimo;
}

public class ProdutoAtivoEspecificacao : Especificacao<Produto>
{
    public override Expression<Func<Produto, bool>> Expressao =>
        p => p.Ativo;
}

public class ItemOrdemServico
{
    public int Id { get; set; }
    public int OrdemServicoId { get; set; }
    public int ProdutoId { get; set; }
    public virtual Produto Produto { get; set; }
    public int Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
}

public class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public virtual ICollection<OrdemServico> OrdensServico { get; set; }
}

// Classes de suporte para os testes
public class OrdemServicoPorClienteEspecificacao : Especificacao<OrdemServico>
{
    private readonly int _clienteId;

    public OrdemServicoPorClienteEspecificacao(int clienteId)
    {
        _clienteId = clienteId;
    }

    public override Expression<Func<OrdemServico, bool>> Expressao =>
        o => o.ClienteId == _clienteId;
}

public class OrdemServicoStatusEspecificacao : Especificacao<OrdemServico>
{
    private readonly StatusOrdemServico _status;

    public OrdemServicoStatusEspecificacao(StatusOrdemServico status)
    {
        _status = status;
    }

    public override Expression<Func<OrdemServico, bool>> Expressao =>
        o => o.Status == _status;
}

public class OrdemServicoDataMaiorQueEspecificacao : Especificacao<OrdemServico>
{
    private readonly DateTime _data;

    public OrdemServicoDataMaiorQueEspecificacao(DateTime data)
    {
        _data = data;
    }

    public override Expression<Func<OrdemServico, bool>> Expressao =>
        o => o.DataAbertura >= _data;
}

public class Tecnico
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public virtual ICollection<OrdemServico> OrdensServico { get; set; }
}

public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public decimal Preco { get; set; }
    public bool Ativo { get; set; }
    public int CategoriaId { get; set; }
    public virtual Categoria Categoria { get; set; }
    public virtual ICollection<ItemOrdemServico> ItensOrdemServico { get; set; } = new List<ItemOrdemServico>();
    public Fornecedor Fornecedor { get; internal set; }
}

public class Categoria
{
    public int Id { get; set; }
    public string Nome { get; set; }
}

