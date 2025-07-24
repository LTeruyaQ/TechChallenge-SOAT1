using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Especificacoes;
using Dominio.Especificacoes.Base;
using Infraestrutura.Dados;
using Infraestrutura.Repositorios;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MecanicaOSTests.Infraestrutura;

public class OrdemServicoRepositorioTests
{
    [Fact]
    public async Task Dado_OrdemServicoComNavegacoes_Quando_ObterUmAsync_Entao_DeveIncluirTodasNavegacoes()
    {
        // Arrange – criar BD em memória
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<MecanicaContexto>()
            .UseSqlite(connection)
            .EnableSensitiveDataLogging()
            .Options;

        Guid osId;

        using (var contexto = new MecanicaContexto(options))
        {
            contexto.Database.EnsureCreated();

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
            var estoque = new Estoque
            {
                Insumo = "Filtro de Óleo",
                Descricao = "Filtro de óleo para motor",
                Preco = 25m,
                QuantidadeDisponivel = 100,
                QuantidadeMinima = 10
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
            };

            ordemServico.InsumosOS.Add(new InsumoOS
            {
                Estoque = estoque
            });

            contexto.Add(ordemServico);
            await contexto.SaveChangesAsync();
            osId = ordemServico.Id;
        }

        // Act
        using (var contexto = new MecanicaContexto(options))
        {
            var repositorio = new Repositorio<OrdemServico>(contexto);
            var especificacao = new ObterOrdemServicoPorIdComIncludeEspecificacao(osId);

            var resultado = await repositorio.ObterUmAsync(especificacao);

            // Assert – todas as navegações carregadas
            Assert.NotNull(resultado);
            Assert.NotNull(resultado!.Servico);
            Assert.NotNull(resultado.Cliente);
            Assert.NotNull(resultado.Cliente.Contato);
            Assert.NotEmpty(resultado.InsumosOS);
            Assert.NotNull(resultado.InsumosOS.First().Estoque);
        }
    }

    [Fact]
    public async Task Dado_OrdemServicoComMultiplosNiveis_Quando_ObterUmAsync_Entao_DeveIncluirTodasNavegacoes()
    {
        // Arrange – criar BD em memória
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<MecanicaContexto>()
            .UseSqlite(connection)
            .EnableSensitiveDataLogging()
            .Options;

        Guid osId;

        using (var contexto = new MecanicaContexto(options))
        {
            await contexto.Database.EnsureCreatedAsync();

            // Criar dados de teste com múltiplos níveis
            var endereco = new Endereco
            {
                Rua = "Rua dos Testes",
                Numero = "123",
                Bairro = "Centro",
                Cidade = "São Paulo"
            };

            var contato = new Contato
            {
                Email = "multinivel@teste.com",
                Telefone = "11999998888"
            };

            var cliente = new Cliente
            {
                Nome = "Cliente Multinível",
                Documento = "98765432100",
                DataNascimento = "1985-05-15",
                TipoCliente = TipoCliente.PessoaFisica,
                Contato = contato,
                Endereco = endereco
            };

            var veiculo = new Veiculo
            {
                Placa = "XYZ9876",
                Modelo = "Civic",
                Marca = "Honda",
                Cor = "Prata",
                Ano = "2020",
                Cliente = cliente
            };

            var servico = new Servico
            {
                Nome = "Revisão Completa",
                Descricao = "Revisão completa do veículo",
                Valor = 500m,
                Disponivel = true
            };

            var estoque1 = new Estoque
            {
                Insumo = "Filtro de Ar",
                Descricao = "Filtro de ar do motor",
                Preco = 80m,
                QuantidadeDisponivel = 50,
                QuantidadeMinima = 5
            };

            var estoque2 = new Estoque
            {
                Insumo = "Óleo 5W30",
                Descricao = "Óleo sintético 5W30 5L",
                Preco = 120m,
                QuantidadeDisponivel = 100,
                QuantidadeMinima = 10
            };

            var ordemServico = new OrdemServico
            {
                Cliente = cliente,
                Veiculo = veiculo,
                Servico = servico,
                Status = StatusOrdemServico.AguardandoAprovação,
                DataCadastro = DateTime.UtcNow
            };

            // Adiciona múltiplos insumos
            ordemServico.InsumosOS.Add(new InsumoOS { Estoque = estoque1, Quantidade = 1 });
            ordemServico.InsumosOS.Add(new InsumoOS { Estoque = estoque2, Quantidade = 2 });

            await contexto.AddAsync(ordemServico);
            await contexto.SaveChangesAsync();
            osId = ordemServico.Id;
        }

        // Act
        using (var contexto = new MecanicaContexto(options))
        {
            var repositorio = new Repositorio<OrdemServico>(contexto);

            // Cria uma especificação que usa os múltiplos níveis de navegação
            var especificacao = new ObterOrdemServicoComMultiplosNiveisEspecificacao(osId);

            var resultado = await repositorio.ObterUmAsync(especificacao);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(osId, resultado.Id);

            // Verifica os includes de primeiro nível
            Assert.NotNull(resultado.Cliente);
            Assert.NotNull(resultado.Veiculo);
            Assert.NotNull(resultado.Servico);
            Assert.NotEmpty(resultado.InsumosOS);

            // Verifica os includes de segundo nível
            Assert.NotNull(resultado.Cliente.Contato);
            Assert.NotNull(resultado.Cliente.Endereco);

            // Verifica os includes de terceiro nível (veículo -> cliente)
            Assert.NotNull(resultado.Veiculo.Cliente);

            // Verifica os includes profundos (insumos -> estoque)
            foreach (var insumo in resultado.InsumosOS)
            {
                Assert.NotNull(insumo.Estoque);
            }

            // Verifica se a navegação de volta está funcionando
            Assert.Equal(resultado.Cliente.Id, resultado.Veiculo.Cliente.Id);
        }
    }
}

// Nova classe de especificação para testar múltiplos níveis
public class ObterOrdemServicoComMultiplosNiveisEspecificacao : EspecificacaoBase<OrdemServico>
{
    private readonly Guid _id;

    public ObterOrdemServicoComMultiplosNiveisEspecificacao(Guid id)
    {
        _id = id;

        // Inclui propriedades de primeiro nível
        AdicionarInclusao(os => os.Servico);

        // Inclui Cliente com suas propriedades
        AdicionarInclusao(os => os.Cliente);
        AdicionarInclusao(os => os.Cliente.Contato);
        AdicionarInclusao(os => os.Cliente.Endereco);

        // Inclui Veículo e suas propriedades
        AdicionarInclusao(os => os.Veiculo);

        // Inclui o Cliente do Veículo
        AdicionarInclusao(os => os.Veiculo.Cliente);
        AdicionarInclusao(os => os.Veiculo.Cliente.Contato);

        // Inclui coleção de InsumosOS com navegação para Estoque
        AdicionarInclusao(os => os.InsumosOS);
        AdicionarInclusao(os => os.InsumosOS, io => io.Estoque);
    }

    public override Expression<Func<OrdemServico, bool>> Expressao => os => os.Id == _id;
}
