using Core.DTOs.Entidades.Estoque;
using Core.Entidades;
using Core.Exceptions;
using MecanicaOS.UnitTests.Fixtures.Handlers;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.Estoques
{
    public class ObterEstoqueHandlerTests
    {
        private readonly EstoqueHandlerFixture _fixture;

        public ObterEstoqueHandlerTests()
        {
            _fixture = new EstoqueHandlerFixture();
        }

        [Fact]
        public async Task Handle_ComIdExistente_DeveRetornarEstoqueCorreto()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            var estoqueEsperado = EstoqueHandlerFixture.CriarEstoqueValido();
            estoqueEsperado.Id = estoqueId;

            // Configurar o repositório para retornar o estoque esperado
            _fixture.ConfigurarMockRepositorioEstoqueParaObterPorId(estoqueId, estoqueEsperado);

            var handler = _fixture.CriarObterEstoqueHandler();

            // Act
            var resultado = await handler.Handle(estoqueId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(estoqueEsperado);

            // Verificar que o repositório foi chamado com o ID correto
            await _fixture.RepositorioEstoque.Received(1).ObterPorIdSemRastreamentoAsync(estoqueId);

            // Verificar que os logs foram registrados
            _fixture.LogServicoObter.Received(1).LogInicio(Arg.Any<string>(), estoqueId);
            _fixture.LogServicoObter.Received(1).LogFim(Arg.Any<string>(), Arg.Any<object>());
        }

        [Fact]
        public async Task Handle_ComIdInexistente_DeveLancarDadosNaoEncontradosException()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();

            // Configurar o repositório para retornar null
            _fixture.ConfigurarMockRepositorioEstoqueParaObterPorId(estoqueId, null);

            var handler = _fixture.CriarObterEstoqueHandler();

            // Act & Assert
            var act = async () => await handler.Handle(estoqueId);

            await act.Should().ThrowAsync<DadosNaoEncontradosException>()
                .WithMessage("Estoque não encontrado");

            // Verificar que o repositório foi chamado com o ID correto
            await _fixture.RepositorioEstoque.Received(1).ObterPorIdSemRastreamentoAsync(estoqueId);

            // Verificar que os logs foram registrados
            _fixture.LogServicoObter.Received(1).LogInicio(Arg.Any<string>(), estoqueId);
            _fixture.LogServicoObter.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosNaoEncontradosException>());
        }

        [Fact]
        public async Task Handle_QuandoGatewayLancaExcecao_DeveRegistrarLogEPropagar()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            var excecaoEsperada = new Exception("Erro no banco de dados");

            // Configurar o repositório para lançar uma exceção
            _fixture.RepositorioEstoque.ObterPorIdSemRastreamentoAsync(Arg.Any<Guid>())
                .Returns(Task.FromException<EstoqueEntityDto>(excecaoEsperada));

            var handler = _fixture.CriarObterEstoqueHandler();

            // Act & Assert
            var act = async () => await handler.Handle(estoqueId);

            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Erro no banco de dados");

            // Verificar que o repositório foi chamado com o ID correto
            await _fixture.RepositorioEstoque.Received(1).ObterPorIdSemRastreamentoAsync(estoqueId);

            // Verificar que os logs foram registrados
            _fixture.LogServicoObter.Received(1).LogInicio(Arg.Any<string>(), estoqueId);
            _fixture.LogServicoObter.Received(1).LogErro(Arg.Any<string>(), excecaoEsperada);
        }

        [Fact]
        public async Task Handle_DevePassarDadosCorretamenteEntreHandlerERepositorio()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();

            // Criar um estoque com valores específicos para identificar no teste
            var estoqueEsperado = new Estoque
            {
                Id = estoqueId,
                Insumo = "Produto Específico de Teste",
                Descricao = "Descrição específica para teste de trânsito de dados",
                QuantidadeDisponivel = 42,
                QuantidadeMinima = 10,
                Preco = 123.45m,
                Ativo = true,
                DataCadastro = new DateTime(2023, 1, 15),
                DataAtualizacao = new DateTime(2023, 6, 30)
            };

            // Configurar o repositório para retornar o estoque específico
            _fixture.ConfigurarMockRepositorioEstoqueParaObterPorId(estoqueId, estoqueEsperado);

            var handler = _fixture.CriarObterEstoqueHandler();

            // Act
            var resultado = await handler.Handle(estoqueId);

            // Assert
            // Verificar que o repositório foi chamado com o ID correto
            await _fixture.RepositorioEstoque.Received(1).ObterPorIdSemRastreamentoAsync(estoqueId);

            // Verificar que o resultado contém exatamente os mesmos dados retornados pelo gateway
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(estoqueEsperado);

            // Verificar cada propriedade individualmente para garantir que não houve alteração
            resultado.Id.Should().Be(estoqueId);
            resultado.Insumo.Should().Be("Produto Específico de Teste");
            resultado.Descricao.Should().Be("Descrição específica para teste de trânsito de dados");
            resultado.QuantidadeDisponivel.Should().Be(42);
            resultado.QuantidadeMinima.Should().Be(10);
            resultado.Preco.Should().Be(123.45m);
            resultado.Ativo.Should().BeTrue();
            resultado.DataCadastro.Should().Be(new DateTime(2023, 1, 15));
            resultado.DataAtualizacao.Should().Be(new DateTime(2023, 6, 30));
        }
    }
}
