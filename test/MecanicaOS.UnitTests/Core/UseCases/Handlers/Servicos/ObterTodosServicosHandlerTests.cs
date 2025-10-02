using Core.DTOs.Entidades.Servico;
using Core.Entidades;
using MecanicaOS.UnitTests.Fixtures.Handlers;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.Servicos
{
    public class ObterTodosServicosHandlerTests
    {
        private readonly ServicoHandlerFixture _fixture;

        public ObterTodosServicosHandlerTests()
        {
            _fixture = new ServicoHandlerFixture();
        }

        [Fact]
        public async Task Handle_DevePassarDadosCorretamenteEntreHandlerERepositorio()
        {
            // Arrange
            // Criar uma lista de serviços com valores específicos para identificar no teste
            var servicos = new List<Servico>
            {
                new Servico
                {
                    Id = Guid.NewGuid(),
                    Nome = "Serviço Específico 1",
                    Descricao = "Descrição específica 1",
                    Valor = 100.00m,
                    Disponivel = true,
                    Ativo = true,
                    DataCadastro = new DateTime(2023, 1, 15),
                    DataAtualizacao = new DateTime(2023, 6, 30)
                },
                new Servico
                {
                    Id = Guid.NewGuid(),
                    Nome = "Serviço Específico 2",
                    Descricao = "Descrição específica 2",
                    Valor = 200.00m,
                    Disponivel = false,
                    Ativo = true,
                    DataCadastro = new DateTime(2023, 2, 20),
                    DataAtualizacao = new DateTime(2023, 7, 15)
                }
            };

            // Configurar o repositório para retornar a lista específica
            var dtos = servicos.Select(s => new ServicoEntityDto
            {
                Id = s.Id,
                Nome = s.Nome,
                Descricao = s.Descricao,
                Valor = s.Valor,
                Disponivel = s.Disponivel,
                Ativo = s.Ativo,
                DataCadastro = s.DataCadastro,
                DataAtualizacao = s.DataAtualizacao
            }).ToList();
            _fixture.RepositorioServico.ObterTodosAsync().Returns(dtos);

            var handler = _fixture.CriarObterTodosServicosHandler();

            // Act
            var resultado = await handler.Handle();

            // Assert
            // Verificar que o repositório foi chamado
            await _fixture.RepositorioServico.Received(1).ObterTodosAsync();

            // Verificar que o resultado contém exatamente os mesmos dados retornados pelo repositório
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.Should().BeEquivalentTo(servicos);

            // Verificar os dados do primeiro serviço
            var primeiroServico = resultado.First(s => s.Nome == "Serviço Específico 1");
            primeiroServico.Descricao.Should().Be("Descrição específica 1");
            primeiroServico.Valor.Should().Be(100.00m);
            primeiroServico.Disponivel.Should().BeTrue();

            // Verificar os dados do segundo serviço
            var segundoServico = resultado.First(s => s.Nome == "Serviço Específico 2");
            segundoServico.Descricao.Should().Be("Descrição específica 2");
            segundoServico.Valor.Should().Be(200.00m);
            segundoServico.Disponivel.Should().BeFalse();

            // Verificar que os campos técnicos foram preservados
            primeiroServico.Ativo.Should().BeTrue();
            primeiroServico.DataCadastro.Should().Be(new DateTime(2023, 1, 15));
            primeiroServico.DataAtualizacao.Should().Be(new DateTime(2023, 6, 30));
            segundoServico.Ativo.Should().BeTrue();
            segundoServico.DataCadastro.Should().Be(new DateTime(2023, 2, 20));
            segundoServico.DataAtualizacao.Should().Be(new DateTime(2023, 7, 15));

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterTodos.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoObterTodos.Received(1).LogFim(Arg.Any<string>(), Arg.Any<object>());
        }

        [Fact]
        public async Task Handle_ComListaVazia_DeveRetornarListaVazia()
        {
            // Arrange
            var servicos = new List<Servico>();

            _fixture.ConfigurarMockServicoGatewayParaObterTodos(servicos);

            var handler = _fixture.CriarObterTodosServicosHandler();

            // Act
            var resultado = await handler.Handle();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();

            // Verificar que o gateway foi chamado
            await _fixture.RepositorioServico.Received(1).ObterTodosAsync();

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterTodos.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoObterTodos.Received(1).LogFim(Arg.Any<string>(), Arg.Any<object>());
        }

        [Fact]
        public async Task Handle_ComExcecaoInesperada_DevePropagaExcecao()
        {
            // Arrange
            // Configurar o repositório para lançar uma exceção
            _fixture.RepositorioServico.ObterTodosAsync()
                .Returns(Task.FromException<IEnumerable<ServicoEntityDto>>(new InvalidOperationException("Erro simulado")));

            var handler = _fixture.CriarObterTodosServicosHandler();

            // Act & Assert
            var act = async () => await handler.Handle();

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Erro simulado");

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterTodos.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoObterTodos.Received(1).LogErro(Arg.Any<string>(), Arg.Any<InvalidOperationException>());
        }
    }
}
