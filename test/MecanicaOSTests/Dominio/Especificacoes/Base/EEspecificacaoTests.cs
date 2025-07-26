using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Especificacoes.Base;
using Infraestrutura.Dados;
using Infraestrutura.Repositorios;
using MecanicaOSTests.Fixtures;
using System.Linq.Expressions;

namespace MecanicaOSTests.Dominio.Especificacoes.Base;

[Collection("Database collection")]
public class EEspecificacaoTests : IDisposable
{
    private readonly MecanicaContexto _context;
    private bool _disposed = false;

    public EEspecificacaoTests(DatabaseFixture fixture)
    {
        _context = fixture.Context;

        // Garante que o banco de dados esteja limpo e com os dados iniciais
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        // Inicializa os dados de teste
        InicializarDadosDeTeste();
    }

    private void InicializarDadosDeTeste()
    {
        // Criar cliente de teste
        var cliente = new Cliente
        {
            Nome = "Cliente Teste",
            Documento = "12345678901",
            DataNascimento = "1990-01-01",
            TipoCliente =TipoCliente.PessoaFisica,
            Contato = new Contato { Email = "cliente@teste.com", Telefone = "11999999999" },
            Endereco = new Endereco { Rua = "Rua Teste", Numero = "123", Bairro = "Centro", Cidade = "São Paulo" }
        };

        // Criar veículo de teste
        var veiculo = new Veiculo
        {
            Placa = "ABC1234",
            Modelo = "Modelo Teste",
            Marca = "Marca Teste",
            Ano = "2020",
            Cor = "Preto",
            Cliente = cliente
        };

        // Criar serviços de teste
        var servico1 = new Servico { Nome = "Troca de Óleo", Descricao = "Troca de óleo completa", Valor = 150m, Disponivel = true };
        var servico2 = new Servico { Nome = "Alinhamento", Descricao = "Alinhamento e balanceamento", Valor = 200m, Disponivel = true };
        var servico3 = new Servico { Nome = "Freios", Descricao = "Troca de pastilhas de freio", Valor = 300m, Disponivel = false };

        // Criar estoque de teste
        var estoque = new Estoque
        {
            Insumo = "Óleo Motor",
            Descricao = "Óleo 5W30 Sintético 1L",
            Preco = 50m,
            QuantidadeDisponivel = 50,
            QuantidadeMinima = 5
        };

        // Criar ordens de serviço de teste
        var ordem1 = new OrdemServico
        {
            Cliente = cliente,
            Veiculo = veiculo,
            Servico = servico1,
            Status = StatusOrdemServico.AguardandoAprovação,
            DataCadastro = DateTime.UtcNow.AddDays(-2),
            Orcamento = 150m
        };

        var ordem2 = new OrdemServico
        {
            Cliente = cliente,
            Veiculo = veiculo,
            Servico = servico2,
            Status = StatusOrdemServico.EmExecucao,
            DataCadastro = DateTime.UtcNow.AddDays(-1),
            Orcamento = 200m
        };

        var ordem3 = new OrdemServico
        {
            Cliente = cliente,
            Veiculo = veiculo,
            Servico = servico3,
            Status = StatusOrdemServico.Cancelada,
            DataCadastro = DateTime.UtcNow,
            Orcamento = 300m
        };

        // Adicionar insumos à ordem de serviço
        ordem1.InsumosOS.Add(new InsumoOS { Estoque = estoque, Quantidade = 2 });
        ordem2.InsumosOS.Add(new InsumoOS { Estoque = estoque, Quantidade = 1 });

        // Adicionar ao contexto
        _context.Clientes.Add(cliente);
        _context.Veiculos.Add(veiculo);
        _context.Servicos.AddRange(servico1, servico2, servico3);
        _context.Estoques.Add(estoque);
        _context.OrdensSevico.AddRange(ordem1, ordem2, ordem3);

        _context.SaveChanges();
    }

    [Fact]
    public async Task Dado_DuasEspecificacoes_Quando_CombinadasComEEspecificacao_Entao_DeveRetornarApenasRegistrosQueAtendemAmbas()
    {
        // Arrange
        var repositorio = new Repositorio<OrdemServico>(_context);

        // Criar especificações individuais
        var especAprovado = new EspecificacaoSimples<OrdemServico>(os => os.Status == StatusOrdemServico.EmExecucao);
        var especOrcamentoAlto = new EspecificacaoSimples<OrdemServico>(os => os.Orcamento >= 200m);

        // Combinar com EEspecificacaoTeste
        var especCombinada = new EEspecificacaoTeste<OrdemServico>(especAprovado, especOrcamentoAlto);

        // Act
        var resultados = await repositorio.ObterPorFiltroAsync(especCombinada);

        // Assert
        Assert.Single(resultados);
        Assert.All(resultados, os =>
        {
            Assert.Equal(StatusOrdemServico.EmExecucao, os.Status);
            Assert.True(os.Orcamento >= 200m);
        });
    }

    [Fact]
    public async Task Dado_EspecificacaoComInclusoes_Quando_CombinadaComEEspecificacao_Entao_DeveManterTodasAsInclusoes()
    {
        // Arrange
        var repositorio = new Repositorio<OrdemServico>(_context);

        // Criar especificações com inclusões
        var especComServico = new EspecificacaoSimples<OrdemServico>(os => os.Status == StatusOrdemServico.EmExecucao);
        especComServico.AdicionarInclusao(os => os.Servico);

        var especComCliente = new EspecificacaoSimples<OrdemServico>(os => os.Orcamento > 100m);
        especComCliente.AdicionarInclusao(os => os.Cliente);

        // Combinar com EEspecificacaoTeste
        var especCombinada = new EEspecificacaoTeste<OrdemServico>(especComServico, especComCliente);

        // Act
        var resultados = await repositorio.ObterPorFiltroAsync(especCombinada);

        // Assert
        Assert.NotEmpty(resultados);
        Assert.All(resultados, os =>
        {
            Assert.NotNull(os.Servico);
            Assert.NotNull(os.Cliente);
            Assert.Equal(StatusOrdemServico.EmExecucao, os.Status);
            Assert.True(os.Orcamento > 100m);
        });
    }

    [Fact]
    public async Task Dado_EspecificacaoComInclusaoDeColecao_Quando_CombinadaComEEspecificacao_Entao_DeveCarregarAColecao()
    {
        // Arrange
        var repositorio = new Repositorio<OrdemServico>(_context);

        // Criar especificações
        var especComInsumos = new EspecificacaoSimples<OrdemServico>(os => os.InsumosOS.Any());
        especComInsumos.AdicionarInclusao(os => os.InsumosOS);
        especComInsumos.AdicionarInclusao(os => os.InsumosOS, io => io.Estoque);

        var especAprovado = new EspecificacaoSimples<OrdemServico>(os => os.Status == StatusOrdemServico.EmExecucao);

        // Combinar com EEspecificacaoTeste
        var especCombinada = new EEspecificacaoTeste<OrdemServico>(especComInsumos, especAprovado);

        // Act
        var resultados = await repositorio.ObterPorFiltroAsync(especCombinada);

        // Assert
        Assert.NotEmpty(resultados);
        Assert.All(resultados, os =>
        {
            Assert.NotEmpty(os.InsumosOS);
            Assert.NotNull(os.InsumosOS.First().Estoque);
            Assert.Equal(StatusOrdemServico.EmExecucao, os.Status);
        });
    }

    [Fact]
    public async Task Dado_EspecificacaoCombinada_Quando_UsadaComRepositorio_Entao_DeveAplicarTodasAsCondicoes()
    {
        // Arrange
        var repositorio = new Repositorio<OrdemServico>(_context);

        // Criar especificações
        var especStatus = new EspecificacaoSimples<OrdemServico>(
            os => os.Status == StatusOrdemServico.AguardandoAprovação ||
                  os.Status == StatusOrdemServico.EmExecucao);

        var especData = new EspecificacaoSimples<OrdemServico>(
            os => os.DataCadastro >= DateTime.UtcNow.AddDays(-2.5));

        // Combinar com EEspecificacaoTeste
        var especCombinada = new EEspecificacaoTeste<OrdemServico>(especStatus, especData);

        // Adicionar inclusões
        especCombinada.AdicionarInclusao(os => os.Servico);
        especCombinada.AdicionarInclusao(os => os.Cliente);

        // Act
        var resultados = await repositorio.ObterPorFiltroAsync(especCombinada);

        // Assert
        Assert.Equal(2, resultados.Count());
        Assert.Contains(resultados, os => os.Status == StatusOrdemServico.AguardandoAprovação);
        Assert.Contains(resultados, os => os.Status == StatusOrdemServico.EmExecucao);
        Assert.All(resultados, os =>
        {
            Assert.NotNull(os.Servico);
            Assert.NotNull(os.Cliente);
            Assert.True(os.DataCadastro >= DateTime.UtcNow.AddDays(-2.5));
        });
    }

    [Fact]
    public void Dado_EspecificacaoComInclusoesENula_Quando_CombinadaComEEspecificacao_Entao_DeveManterInclusoesExistentes()
    {
        // Arrange
        var especComInclusoes = new EspecificacaoSimples<OrdemServico>(os => os.Orcamento > 100);
        especComInclusoes.AdicionarInclusao(os => os.Cliente);
        especComInclusoes.AdicionarInclusao(os => os.Servico);

        var especSemInclusoes = new EspecificacaoSimples<OrdemServico>(os => os.Status != StatusOrdemServico.Cancelada);

        // Act - Testa em ambas as ordens
        var especCombinada1 = new EEspecificacaoTeste<OrdemServico>(especComInclusoes, especSemInclusoes);
        var especCombinada2 = new EEspecificacaoTeste<OrdemServico>(especSemInclusoes, especComInclusoes);

        // Assert - Em ambos os casos, as inclusões devem ser mantidas
        // Verifica se as inclusões foram combinadas corretamente
        Assert.Equal(2, especCombinada1.InclusoesPublicas.Count);
        Assert.Equal(2, especCombinada2.InclusoesPublicas.Count);

        Assert.All(new[] { especCombinada1, especCombinada2 }, espec =>
        {
            // Verifica se os caminhos das propriedades estão nas inclusões
            Assert.Contains(espec.InclusoesPublicas, i => i.Contains("Cliente"));
            Assert.Contains(espec.InclusoesPublicas, i => i.Contains("Servico"));
        });
    }

    [Fact]
    public void Dado_EspecificacoesComInclusoes_Quando_CombinadasComEEspecificacao_Entao_DeveCombinarTodasAsInclusoes()
    {
        // Arrange
        var especEsquerda = new EspecificacaoSimples<OrdemServico>(os => os.Orcamento > 100);
        especEsquerda.AdicionarInclusao(os => os.Cliente);
        especEsquerda.AdicionarInclusao(os => os.Veiculo);

        var especDireita = new EspecificacaoSimples<OrdemServico>(os => os.Status != StatusOrdemServico.Cancelada);
        especDireita.AdicionarInclusao(os => os.Servico);
        especDireita.AdicionarInclusao(os => os.Veiculo);

        // Act
        var especCombinada = new EEspecificacaoTeste<OrdemServico>(especEsquerda, especDireita);

        // Assert
        // Verifica se todas as inclusões foram combinadas (inclusive duplicadas)
        Assert.Equal(3, especCombinada.InclusoesPublicas.Count);

        // Verifica se as inclusões específicas estão presentes
        Assert.Contains(especCombinada.InclusoesPublicas, i => i.Contains("Cliente"));
        Assert.Contains(especCombinada.InclusoesPublicas, i => i.Contains("Veiculo"));
        Assert.Contains(especCombinada.InclusoesPublicas, i => i.Contains("Servico"));
    }

    [Fact]
    public async Task Dado_EspecificacaoCombinadaComTresCondicoes_Quando_UsadaComRepositorio_Entao_DeveAplicarTodasAsCondicoes()
    {
        // Arrange
        var repositorio = new Repositorio<OrdemServico>(_context);

        // Criar especificações
        var espec1 = new EspecificacaoSimples<OrdemServico>(os => os.Orcamento > 100m);
        var espec2 = new EspecificacaoSimples<OrdemServico>(os => os.Orcamento < 250m);
        var espec3 = new EspecificacaoSimples<OrdemServico>(
            os => os.Status != StatusOrdemServico.Cancelada);

        // Combinar com EEspecificacaoTeste aninhado
        var especCombinada1e2 = new EEspecificacaoTeste<OrdemServico>(espec1, espec2);
        var especCombinadaFinal = new EEspecificacaoTeste<OrdemServico>(especCombinada1e2, espec3);

        // Act
        var resultados = await repositorio.ObterPorFiltroAsync(especCombinadaFinal);

        // Assert
        Assert.NotEmpty(resultados);
        Assert.All(resultados, os =>
        {
            Assert.True(os.Orcamento > 100m && os.Orcamento < 250m);
            Assert.NotEqual(StatusOrdemServico.Cancelada, os.Status);
        });
    }

    // Classe auxiliar para criar especificações simples
    public class EspecificacaoSimples<T> : EspecificacaoBase<T> where T : class
    {
        private readonly Expression<Func<T, bool>> _expressao;

        public EspecificacaoSimples(Expression<Func<T, bool>> expressao)
        {
            _expressao = expressao ?? throw new ArgumentNullException(nameof(expressao));
        }

        public override Expression<Func<T, bool>> Expressao => _expressao;

        // Métodos públicos para adicionar inclusões que delegam para os métodos protegidos da classe base
        public new void AdicionarInclusao<TProp>(Expression<Func<T, TProp>> navegacao)
        {
            base.AdicionarInclusao(navegacao);
        }

        public new void AdicionarInclusao<TColecao, TProp>(
            Expression<Func<T, IEnumerable<TColecao>>> colecao,
            Expression<Func<TColecao, TProp>> navegacao)
        {
            base.AdicionarInclusao(colecao, navegacao);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // O contexto é gerenciado pelo DatabaseFixture
                // Não precisamos descartá-lo aqui
            }
            _disposed = true;
        }
    }
}
