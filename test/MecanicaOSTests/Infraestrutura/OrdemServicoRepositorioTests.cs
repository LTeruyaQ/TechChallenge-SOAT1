using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Especificacoes;
using Infraestrutura.Dados;
using Infraestrutura.Repositorios;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MecanicaOSTests.Infraestrutura;

public class OrdemServicoRepositorioTests
{
    [Fact]
    public async Task ObterUmAsync_Deve_Incluir_Todas_Navegacoes()
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
                TipoCliente = "PessoaFisica",
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
}
