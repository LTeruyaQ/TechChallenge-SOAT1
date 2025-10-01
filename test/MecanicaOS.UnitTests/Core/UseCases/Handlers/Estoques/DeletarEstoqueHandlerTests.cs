using Core.DTOs.Entidades.Estoque;
using Core.Entidades;
using Core.Exceptions;
using MecanicaOS.UnitTests.Fixtures.Handlers;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.Estoques
{
    public class DeletarEstoqueHandlerTests
    {
        private readonly EstoqueHandlerFixture _fixture;

        public DeletarEstoqueHandlerTests()
        {
            _fixture = new EstoqueHandlerFixture();
        }

        [Fact]
        public async Task Handle_ComEstoqueExistente_DeveDeletarERetornarTrue()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            var estoqueExistente = EstoqueHandlerFixture.CriarEstoqueValido();
            estoqueExistente.Id = estoqueId;

            // Configurar o repositório para retornar o estoque existente
            _fixture.ConfigurarMockRepositorioEstoqueParaObterPorId(estoqueId, estoqueExistente);

            // Configurar o repositório para simular a deleção com sucesso
            _fixture.ConfigurarMockRepositorioEstoqueParaDeletar();

            var handler = _fixture.CriarDeletarEstoqueHandler();

            // Act
            var resultado = await handler.Handle(estoqueId);

            // Assert
            resultado.Should().BeTrue();

            // Verificar que o gateway foi chamado com os dados corretos
            await _fixture.RepositorioEstoque.Received(1).ObterPorIdSemRastreamentoAsync(estoqueId);
            await _fixture.RepositorioEstoque.Received(1).DeletarAsync(Arg.Any<EstoqueEntityDto>());

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoDeletar.Received(1).LogInicio(Arg.Any<string>(), estoqueId);
            _fixture.LogServicoDeletar.Received(1).LogFim(Arg.Any<string>(), Arg.Any<bool>());
        }

        [Fact]
        public async Task Handle_ComEstoqueInexistente_DeveLancarDadosNaoEncontradosException()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();

            // Configurar o repositório para retornar null
            _fixture.ConfigurarMockRepositorioEstoqueParaObterPorIdNull(estoqueId);

            var handler = _fixture.CriarDeletarEstoqueHandler();

            // Act & Assert
            var act = async () => await handler.Handle(estoqueId);

            await act.Should().ThrowAsync<DadosNaoEncontradosException>()
                .WithMessage("Estoque não encontrado");

            // Verificar que o gateway foi chamado para obter o estoque
            await _fixture.RepositorioEstoque.Received(1).ObterPorIdSemRastreamentoAsync(estoqueId);

            // Verificar que o gateway NÃO foi chamado para deletar
            await _fixture.RepositorioEstoque.DidNotReceive().DeletarAsync(Arg.Any<EstoqueEntityDto>());

            // Verificar que o commit NÃO foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceive().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoDeletar.Received(1).LogInicio(Arg.Any<string>(), estoqueId);
            _fixture.LogServicoDeletar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosNaoEncontradosException>());
        }

        [Fact]
        public async Task Handle_QuandoCommitFalha_DeveLancarPersistirDadosException()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            var estoqueExistente = EstoqueHandlerFixture.CriarEstoqueValido();
            estoqueExistente.Id = estoqueId;

            // Configurar o repositório para retornar o estoque existente
            _fixture.ConfigurarMockRepositorioEstoqueParaObterPorId(estoqueId, estoqueExistente);

            // Configurar o repositório para simular a deleção
            _fixture.ConfigurarMockRepositorioEstoqueParaDeletar();

            // Configurar o UDT para falhar no commit
            _fixture.ConfigurarMockUdtParaCommitFalha();

            var handler = _fixture.CriarDeletarEstoqueHandler();

            // Act & Assert
            var act = async () => await handler.Handle(estoqueId);

            await act.Should().ThrowAsync<PersistirDadosException>()
                .WithMessage("Erro ao deletar estoque");

            // Verificar que o gateway foi chamado para obter e deletar o estoque
            await _fixture.RepositorioEstoque.Received(1).ObterPorIdSemRastreamentoAsync(estoqueId);
            await _fixture.RepositorioEstoque.Received(1).DeletarAsync(Arg.Any<EstoqueEntityDto>());

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoDeletar.Received(1).LogInicio(Arg.Any<string>(), estoqueId);
            _fixture.LogServicoDeletar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<PersistirDadosException>());
        }

        [Fact]
        public async Task Handle_DevePassarDadosCorretamenteEntreHandlerERepositorio()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();

            // Criar um estoque com valores específicos para identificar no teste
            var estoqueExistente = new Estoque
            {
                Id = estoqueId,
                Insumo = "Produto para Deleção",
                Descricao = "Descrição do produto para teste de deleção",
                QuantidadeDisponivel = 5,
                QuantidadeMinima = 2,
                Preco = 50.00m,
                Ativo = true,
                DataCadastro = new DateTime(2023, 1, 15),
                DataAtualizacao = new DateTime(2023, 6, 30)
            };

            // Capturar o estoque que será passado para o gateway
            Estoque estoqueDeletado = null;

            // Configurar o repositório para retornar o estoque existente
            _fixture.ConfigurarMockRepositorioEstoqueParaObterPorId(estoqueId, estoqueExistente);

            // Configurar o repositório para capturar o objeto passado
            EstoqueEntityDto dtoCaptured = null;
            _fixture.RepositorioEstoque.When(x => x.DeletarAsync(Arg.Any<EstoqueEntityDto>()))
                .Do(callInfo =>
                {
                    dtoCaptured = callInfo.Arg<EstoqueEntityDto>();
                    estoqueDeletado = new Estoque
                    {
                        Id = dtoCaptured.Id,
                        Insumo = dtoCaptured.Insumo,
                        Descricao = dtoCaptured.Descricao,
                        QuantidadeDisponivel = dtoCaptured.QuantidadeDisponivel,
                        QuantidadeMinima = dtoCaptured.QuantidadeMinima,
                        Preco = dtoCaptured.Preco,
                        Ativo = dtoCaptured.Ativo,
                        DataCadastro = dtoCaptured.DataCadastro,
                        DataAtualizacao = dtoCaptured.DataAtualizacao
                    };
                });

            var handler = _fixture.CriarDeletarEstoqueHandler();

            // Act
            var resultado = await handler.Handle(estoqueId);

            // Assert
            // Verificar que o gateway foi chamado
            await _fixture.RepositorioEstoque.Received(1).ObterPorIdSemRastreamentoAsync(estoqueId);
            await _fixture.RepositorioEstoque.Received(1).DeletarAsync(Arg.Any<EstoqueEntityDto>());

            // Verificar que os dados foram passados corretamente para o gateway
            estoqueDeletado.Should().NotBeNull();
            estoqueDeletado.Should().BeEquivalentTo(estoqueExistente);
            estoqueDeletado.Id.Should().Be(estoqueId);
            estoqueDeletado.Insumo.Should().Be("Produto para Deleção");
            estoqueDeletado.Descricao.Should().Be("Descrição do produto para teste de deleção");

            // Verificar que o resultado contém o status correto
            resultado.Should().BeTrue();
        }
    }
}
