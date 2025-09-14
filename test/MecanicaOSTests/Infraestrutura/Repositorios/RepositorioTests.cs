using Infraestrutura.Dados;
using Infraestrutura.Repositorios;
using MecanicaOSTests.Fixtures;
using System.Linq.Expressions;

namespace MecanicaOSTests.Infraestrutura.Repositorios
{
    [Collection("Database collection")]
    public class RepositorioTests
    {
        private readonly MecanicaContexto _context;
        private bool _disposed = false;

        public RepositorioTests(DatabaseFixture fixture)
        {
            fixture.ResetDatabase();
            _context = fixture.Context;
        }

        [Fact]
        public async Task CadastrarAsync_DeveAdicionarEntidadeAoContexto()
        {
            // Arrange
            var repositorio = new Repositorio<Cliente>(_context);
            var cliente = new Cliente { Nome = "Teste", Documento = "12345678901", DataNascimento = "1990-01-01" };

            // Act
            await repositorio.CadastrarAsync(cliente);
            await _context.SaveChangesAsync();

            // Assert
            var clienteDoBanco = await _context.Clientes.FindAsync(cliente.Id);
            Assert.NotNull(clienteDoBanco);
            Assert.Equal(cliente.Nome, clienteDoBanco.Nome);
        }

        [Fact]
        public async Task DeletarAsync_DeveRemoverEntidadeDoContexto()
        {
            // Arrange
            var repositorio = new Repositorio<Cliente>(_context);
            var cliente = new Cliente { Nome = "Teste", Documento = "12345678901", DataNascimento = "1990-01-01" };
            await repositorio.CadastrarAsync(cliente);
            await _context.SaveChangesAsync();

            // Act
            await repositorio.DeletarAsync(cliente);
            await _context.SaveChangesAsync();

            // Assert
            var clienteDoBanco = await _context.Clientes.FindAsync(cliente.Id);
            Assert.Null(clienteDoBanco);
        }

        [Fact]
        public async Task CadastrarVariosAsync_DeveAdicionarVariasEntidadesAoContexto()
        {
            // Arrange
            var repositorio = new Repositorio<Cliente>(_context);
            var clientes = new List<Cliente>
            {
                new Cliente { Nome = "Teste 1", Documento = "11111111111", DataNascimento = "1990-01-01" },
                new Cliente { Nome = "Teste 2", Documento = "22222222222", DataNascimento = "1990-01-01" }
            };

            // Act
            await repositorio.CadastrarVariosAsync(clientes);
            await _context.SaveChangesAsync();

            // Assert
            Assert.Equal(2, _context.Clientes.Count());
        }

        [Fact]
        public async Task DeletarVariosAsync_DeveRemoverVariasEntidadesDoContexto()
        {
            // Arrange
            var repositorio = new Repositorio<Cliente>(_context);
            var clientes = new List<Cliente>
            {
                new Cliente { Nome = "Teste 1", Documento = "11111111111", DataNascimento = "1990-01-01" },
                new Cliente { Nome = "Teste 2", Documento = "22222222222", DataNascimento = "1990-01-01" }
            };
            await repositorio.CadastrarVariosAsync(clientes);
            await _context.SaveChangesAsync();

            // Act
            await repositorio.DeletarVariosAsync(clientes);
            await _context.SaveChangesAsync();

            // Assert
            Assert.Empty(_context.Clientes);
        }

        [Fact]
        public async Task DeletarLogicamenteAsync_DeveMarcarEntidadeComoInativa()
        {
            // Arrange
            var repositorio = new Repositorio<Cliente>(_context);
            var cliente = new Cliente { Nome = "Teste", Documento = "12345678901", DataNascimento = "1990-01-01" };
            await repositorio.CadastrarAsync(cliente);
            await _context.SaveChangesAsync();

            // Act
            await repositorio.DeletarLogicamenteAsync(cliente);
            await _context.SaveChangesAsync();

            // Assert
            var clienteDoBanco = await _context.Clientes.FindAsync(cliente.Id);
            Assert.NotNull(clienteDoBanco);
            Assert.False(clienteDoBanco.Ativo);
        }

        [Fact]
        public async Task EditarAsync_DeveAtualizarEntidadeNoContexto()
        {
            // Arrange
            var repositorio = new Repositorio<Cliente>(_context);
            var cliente = new Cliente { Nome = "Teste", Documento = "12345678901", DataNascimento = "1990-01-01" };
            await repositorio.CadastrarAsync(cliente);
            await _context.SaveChangesAsync();

            // Act
            cliente.Nome = "Teste Editado";
            await repositorio.EditarAsync(cliente);
            await _context.SaveChangesAsync();

            // Assert
            var clienteDoBanco = await _context.Clientes.FindAsync(cliente.Id);
            Assert.NotNull(clienteDoBanco);
            Assert.Equal("Teste Editado", clienteDoBanco.Nome);
        }

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarEntidadeCorreta()
        {
            // Arrange
            var repositorio = new Repositorio<Cliente>(_context);
            var cliente = new Cliente { Nome = "Teste", Documento = "12345678901", DataNascimento = "1990-01-01" };
            await repositorio.CadastrarAsync(cliente);
            await _context.SaveChangesAsync();

            // Act
            var resultado = await repositorio.ObterPorIdAsync(cliente.Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(cliente.Id, resultado.Id);
        }

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarTodasEntidades()
        {
            // Arrange
            var repositorio = new Repositorio<Cliente>(_context);
            var clientes = new List<Cliente>
            {
                new Cliente { Nome = "Teste 1", Documento = "11111111111", DataNascimento = "1990-01-01" },
                new Cliente { Nome = "Teste 2", Documento = "22222222222", DataNascimento = "1990-01-01" }
            };
            await repositorio.CadastrarVariosAsync(clientes);
            await _context.SaveChangesAsync();

            // Act
            var resultado = await repositorio.ObterTodosAsync();

            // Assert
            Assert.Equal(2, resultado.Count());
        }

        [Fact]
        public async Task EditarVariosAsync_DeveAtualizarVariasEntidades()
        {
            // Arrange
            var repositorio = new Repositorio<Cliente>(_context);
            var clientes = new List<Cliente>
            {
                new Cliente { Nome = "Teste 1", Documento = "11111111111", DataNascimento = "1990-01-01" },
                new Cliente { Nome = "Teste 2", Documento = "22222222222", DataNascimento = "1990-01-01" }
            };
            await repositorio.CadastrarVariosAsync(clientes);
            await _context.SaveChangesAsync();

            // Act
            clientes[0].Nome = "Editado 1";
            clientes[1].Nome = "Editado 2";
            await repositorio.EditarVariosAsync(clientes);
            await _context.SaveChangesAsync();

            // Assert
            var resultados = await repositorio.ObterTodosAsync();
            Assert.Equal(2, resultados.Count());
            Assert.Contains(resultados, c => c.Nome == "Editado 1");
            Assert.Contains(resultados, c => c.Nome == "Editado 2");
        }

        [Fact]
        public async Task CadastrarVariosAsync_ComListaNulaOuVazia_DeveRetornarEnumerableVazio()
        {
            // Arrange
            var repositorio = new Repositorio<Cliente>(_context);

            // Act
            var resultadoNulo = await repositorio.CadastrarVariosAsync(null);
            var resultadoVazio = await repositorio.CadastrarVariosAsync(new List<Cliente>());

            // Assert
            Assert.Empty(resultadoNulo);
            Assert.Empty(resultadoVazio);
        }

        public class ClientePorNomeEspecificacao : EspecificacaoBase<Cliente>
        {
            private readonly string _nome;

            public ClientePorNomeEspecificacao(string nome)
            {
                _nome = nome;
            }

            public override Expression<Func<Cliente, bool>> Expressao => c => c.Nome == _nome;
        }

        [Fact]
        public async Task ListarAsync_ComEspecificacao_DeveRetornarEntidadesFiltradas()
        {
            // Arrange
            var repositorio = new Repositorio<Cliente>(_context);
            var clientes = new List<Cliente>
            {
                new Cliente { Nome = "Filtro", Documento = "111", DataNascimento = "1990-01-01" },
                new Cliente { Nome = "NaoFiltro", Documento = "222", DataNascimento = "1990-01-01" },
                new Cliente { Nome = "Filtro", Documento = "333", DataNascimento = "1990-01-01" }
            };
            await repositorio.CadastrarVariosAsync(clientes);
            await _context.SaveChangesAsync();
            var especificacao = new ClientePorNomeEspecificacao("Filtro");

            // Act
            var resultado = await repositorio.ListarAsync(especificacao);

            // Assert
            Assert.Equal(2, resultado.Count());
            Assert.All(resultado, c => Assert.Equal("Filtro", c.Nome));
        }

        [Fact]
        public async Task ObterUmAsync_ComEspecificacao_DeveRetornarEntidadeUnica()
        {
            // Arrange
            var repositorio = new Repositorio<Cliente>(_context);
            var cliente = new Cliente { Nome = "Unico", Documento = "444", DataNascimento = "1990-01-01" };
            await repositorio.CadastrarAsync(cliente);
            await _context.SaveChangesAsync();
            var especificacao = new ClientePorNomeEspecificacao("Unico");

            // Act
            var resultado = await repositorio.ObterUmAsync(especificacao);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Unico", resultado.Nome);
        }

        [Fact]
        public async Task ListarSemRastreamentoAsync_DeveRetornarEntidadesNaoRastreadas()
        {
            // Arrange
            var repositorio = new Repositorio<Cliente>(_context);
            var cliente = new Cliente { Nome = "NaoRastreado", Documento = "555", DataNascimento = "1990-01-01" };
            await repositorio.CadastrarAsync(cliente);
            await _context.SaveChangesAsync();
            var especificacao = new ClientePorNomeEspecificacao("NaoRastreado");
            _context.ChangeTracker.Clear();

            // Act
            var resultado = await repositorio.ListarSemRastreamentoAsync(especificacao);
            var entidade = resultado.First();
            entidade.Nome = "Modificado";

            // Assert
            Assert.Single(resultado);
            Assert.Empty(_context.ChangeTracker.Entries());
        }

        [Fact]
        public async Task ObterUmSemRastreamentoAsync_DeveRetornarEntidadeNaoRastreada()
        {
            // Arrange
            var repositorio = new Repositorio<Cliente>(_context);
            var cliente = new Cliente { Nome = "UnicoNaoRastreado", Documento = "666", DataNascimento = "1990-01-01" };
            await repositorio.CadastrarAsync(cliente);
            await _context.SaveChangesAsync();
            var especificacao = new ClientePorNomeEspecificacao("UnicoNaoRastreado");
            _context.ChangeTracker.Clear();

            // Act
            var resultado = await repositorio.ObterUmSemRastreamentoAsync(especificacao);
            Assert.NotNull(resultado);
            resultado.Nome = "Modificado";

            // Assert
            Assert.Empty(_context.ChangeTracker.Entries());
        }

        public class ClienteDto
        {
            public string Nome { get; set; } = string.Empty;
        }

        public class ClienteParaDtoEspecificacao : EspecificacaoBase<Cliente>
        {
            private readonly Expression<Func<Cliente, bool>> _expression;

            public ClienteParaDtoEspecificacao(Expression<Func<Cliente, bool>> expression)
            {
                _expression = expression;
                DefinirProjecao(c => new ClienteDto { Nome = c.Nome });
            }

            public override Expression<Func<Cliente, bool>> Expressao => _expression;
        }

        [Fact]
        public async Task ListarProjetadoAsync_DeveRetornarDtoCorretamente()
        {
            // Arrange
            var repositorio = new Repositorio<Cliente>(_context);
            var cliente = new Cliente { Nome = "Projecao", Documento = "777", DataNascimento = "1990-01-01" };
            await repositorio.CadastrarAsync(cliente);
            await _context.SaveChangesAsync();
            var especificacao = new ClienteParaDtoEspecificacao(c => c.Nome == "Projecao");

            // Act
            var resultado = await repositorio.ListarProjetadoAsync<ClienteDto>(especificacao);

            // Assert
            Assert.Single(resultado);
            Assert.Equal("Projecao", resultado.First().Nome);
        }

        [Fact]
        public async Task ObterUmProjetadoAsync_DeveRetornarDtoUnico()
        {
            // Arrange
            var repositorio = new Repositorio<Cliente>(_context);
            var cliente = new Cliente { Nome = "ProjecaoUnica", Documento = "888", DataNascimento = "1990-01-01" };
            await repositorio.CadastrarAsync(cliente);
            await _context.SaveChangesAsync();
            var especificacao = new ClienteParaDtoEspecificacao(c => c.Nome == "ProjecaoUnica");

            // Act
            var resultado = await repositorio.ObterUmProjetadoAsync<ClienteDto>(especificacao);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("ProjecaoUnica", resultado.Nome);
        }

        [Fact]
        public async Task ListarProjetadoSemRastreamentoAsync_DeveRetornarDtoCorretamente()
        {
            // Arrange
            var repositorio = new Repositorio<Cliente>(_context);
            var cliente = new Cliente { Nome = "ProjecaoSemRastreio", Documento = "999", DataNascimento = "1990-01-01" };
            await repositorio.CadastrarAsync(cliente);
            await _context.SaveChangesAsync();
            var especificacao = new ClienteParaDtoEspecificacao(c => c.Nome == "ProjecaoSemRastreio");

            // Act
            var resultado = await repositorio.ListarProjetadoSemRastreamentoAsync<ClienteDto>(especificacao);

            // Assert
            Assert.Single(resultado);
            Assert.Equal("ProjecaoSemRastreio", resultado.First().Nome);
        }

        [Fact]
        public async Task ObterUmProjetadoSemRastreamentoAsync_DeveRetornarDtoUnico()
        {
            // Arrange
            var repositorio = new Repositorio<Cliente>(_context);
            var cliente = new Cliente { Nome = "ProjecaoUnicaSemRastreio", Documento = "101010", DataNascimento = "1990-01-01" };
            await repositorio.CadastrarAsync(cliente);
            await _context.SaveChangesAsync();
            var especificacao = new ClienteParaDtoEspecificacao(c => c.Nome == "ProjecaoUnicaSemRastreio");

            // Act
            var resultado = await repositorio.ObterUmProjetadoSemRastreamentoAsync<ClienteDto>(especificacao);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("ProjecaoUnicaSemRastreio", resultado.Nome);
        }
    }
}
