using Core.DTOs.Entidades.Estoque;
using Core.DTOs.UseCases.Estoque;
using Core.Entidades;
using Core.Exceptions;
using MecanicaOS.UnitTests.Fixtures.Handlers;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.Estoques
{
    public class AtualizarEstoqueHandlerTests
    {
        private readonly EstoqueHandlerFixture _fixture;

        public AtualizarEstoqueHandlerTests()
        {
            _fixture = new EstoqueHandlerFixture();
        }

        [Fact]
        public async Task Handle_ComDadosValidos_DeveAtualizarEstoqueCorretamente()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            var request = EstoqueHandlerFixture.CriarAtualizarEstoqueUseCaseDtoValido();
            var estoqueExistente = EstoqueHandlerFixture.CriarEstoqueValido();
            estoqueExistente.Id = estoqueId;

            // Configurar o repositório para retornar o estoque existente
            var dto = new EstoqueEntityDto
            {
                Id = estoqueExistente.Id,
                Insumo = estoqueExistente.Insumo,
                Descricao = estoqueExistente.Descricao,
                QuantidadeDisponivel = estoqueExistente.QuantidadeDisponivel,
                QuantidadeMinima = estoqueExistente.QuantidadeMinima,
                Preco = estoqueExistente.Preco,
                Ativo = estoqueExistente.Ativo,
                DataCadastro = estoqueExistente.DataCadastro,
                DataAtualizacao = estoqueExistente.DataAtualizacao
            };
            _fixture.RepositorioEstoque.ObterPorIdSemRastreamentoAsync(estoqueId).Returns(dto);

            // Configurar o gateway para simular a atualização
            _fixture.ConfigurarMockEstoqueGatewayParaAtualizar(estoqueExistente);

            var handler = _fixture.CriarAtualizarEstoqueHandler();

            // Act
            var resultado = await handler.Handle(estoqueId, request);

            // Assert
            resultado.Should().NotBeNull();

            // Verificar que o gateway foi chamado com os dados corretos
            await _fixture.RepositorioEstoque.Received(1).ObterPorIdSemRastreamentoAsync(estoqueId);
            await _fixture.RepositorioEstoque.Received(1).EditarAsync(Arg.Any<EstoqueEntityDto>());

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoAtualizar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoAtualizar.Received(1).LogFim(Arg.Any<string>(), Arg.Any<Estoque>());
        }

        [Fact]
        public async Task Handle_ComEstoqueInexistente_DeveLancarDadosNaoEncontradosException()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            var request = EstoqueHandlerFixture.CriarAtualizarEstoqueUseCaseDtoValido();

            // Configurar o repositório para retornar null
            _fixture.RepositorioEstoque.ObterPorIdSemRastreamentoAsync(estoqueId).Returns((EstoqueEntityDto)null);

            var handler = _fixture.CriarAtualizarEstoqueHandler();

            // Act & Assert
            var act = async () => await handler.Handle(estoqueId, request);

            await act.Should().ThrowAsync<DadosNaoEncontradosException>()
                .WithMessage("Estoque não encontrado");

            // Verificar que o gateway foi chamado para obter o estoque
            await _fixture.RepositorioEstoque.Received(1).ObterPorIdSemRastreamentoAsync(estoqueId);

            // Verificar que o gateway NÃO foi chamado para editar
            await _fixture.RepositorioEstoque.DidNotReceive().EditarAsync(Arg.Any<EstoqueEntityDto>());

            // Verificar que o commit NÃO foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceive().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoAtualizar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoAtualizar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosNaoEncontradosException>());
        }

        [Fact]
        public async Task Handle_QuandoCommitFalha_DeveLancarPersistirDadosException()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            var request = EstoqueHandlerFixture.CriarAtualizarEstoqueUseCaseDtoValido();
            var estoqueExistente = EstoqueHandlerFixture.CriarEstoqueValido();
            estoqueExistente.Id = estoqueId;

            // Configurar o repositório para retornar o estoque existente
            var dto = new EstoqueEntityDto
            {
                Id = estoqueExistente.Id,
                Insumo = estoqueExistente.Insumo,
                Descricao = estoqueExistente.Descricao,
                QuantidadeDisponivel = estoqueExistente.QuantidadeDisponivel,
                QuantidadeMinima = estoqueExistente.QuantidadeMinima,
                Preco = estoqueExistente.Preco,
                Ativo = estoqueExistente.Ativo,
                DataCadastro = estoqueExistente.DataCadastro,
                DataAtualizacao = estoqueExistente.DataAtualizacao
            };
            _fixture.RepositorioEstoque.ObterPorIdSemRastreamentoAsync(estoqueId).Returns(dto);

            // Configurar o gateway para simular a atualização
            _fixture.ConfigurarMockEstoqueGatewayParaAtualizar(estoqueExistente);

            // Configurar o UDT para falhar no commit
            _fixture.ConfigurarMockUdtParaCommitFalha();

            var handler = _fixture.CriarAtualizarEstoqueHandler();

            // Act & Assert
            var act = async () => await handler.Handle(estoqueId, request);

            await act.Should().ThrowAsync<PersistirDadosException>()
                .WithMessage("Erro ao atualizar estoque");

            // Verificar que o gateway foi chamado para obter e editar o estoque
            await _fixture.RepositorioEstoque.Received(1).ObterPorIdSemRastreamentoAsync(estoqueId);
            await _fixture.RepositorioEstoque.Received(1).EditarAsync(Arg.Any<EstoqueEntityDto>());

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoAtualizar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoAtualizar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<PersistirDadosException>());
        }

        [Fact]
        public async Task Handle_DevePassarDadosCorretamenteEntreHandlerERepositorio()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();

            // Criar um DTO com valores específicos para identificar no teste
            var request = new AtualizarEstoqueUseCaseDto
            {
                Insumo = "Produto Específico para Teste de Atualização",
                Descricao = "Descrição específica para teste de trânsito de dados na atualização",
                QuantidadeDisponivel = 99,
                QuantidadeMinima = 20,
                Preco = 456.78m
            };

            // Criar um estoque existente com valores diferentes
            var estoqueExistente = new Estoque
            {
                Id = estoqueId,
                Insumo = "Produto Original",
                Descricao = "Descrição original",
                QuantidadeDisponivel = 10,
                QuantidadeMinima = 5,
                Preco = 123.45m,
                Ativo = true,
                DataCadastro = new DateTime(2023, 1, 15),
                DataAtualizacao = new DateTime(2023, 6, 30)
            };

            // Capturar o estoque que será passado para o gateway
            Estoque estoqueAtualizado = null;

            // Configurar o repositório para retornar o estoque existente
            var dto = new EstoqueEntityDto
            {
                Id = estoqueExistente.Id,
                Insumo = estoqueExistente.Insumo,
                Descricao = estoqueExistente.Descricao,
                QuantidadeDisponivel = estoqueExistente.QuantidadeDisponivel,
                QuantidadeMinima = estoqueExistente.QuantidadeMinima,
                Preco = estoqueExistente.Preco,
                Ativo = estoqueExistente.Ativo,
                DataCadastro = estoqueExistente.DataCadastro,
                DataAtualizacao = estoqueExistente.DataAtualizacao
            };
            _fixture.RepositorioEstoque.ObterPorIdSemRastreamentoAsync(estoqueId).Returns(dto);

            // Configurar o repositório para capturar o objeto passado
            EstoqueEntityDto dtoCaptured = null;
            _fixture.RepositorioEstoque.When(x => x.EditarAsync(Arg.Any<EstoqueEntityDto>()))
                .Do(callInfo =>
                {
                    dtoCaptured = callInfo.Arg<EstoqueEntityDto>();
                    estoqueAtualizado = new Estoque
                    {
                        Id = dtoCaptured.Id,
                        Insumo = dtoCaptured.Insumo,
                        Descricao = dtoCaptured.Descricao,
                        QuantidadeDisponivel = dtoCaptured.QuantidadeDisponivel,
                        QuantidadeMinima = dtoCaptured.QuantidadeMinima,
                        Preco = dtoCaptured.Preco,
                        Ativo = dtoCaptured.Ativo,
                        DataCadastro = dtoCaptured.DataCadastro,
                        DataAtualizacao = DateTime.UtcNow
                    };
                });

            var handler = _fixture.CriarAtualizarEstoqueHandler();

            // Act
            var resultado = await handler.Handle(estoqueId, request);

            // Assert
            // Verificar que o gateway foi chamado
            await _fixture.RepositorioEstoque.Received(1).ObterPorIdSemRastreamentoAsync(estoqueId);
            await _fixture.RepositorioEstoque.Received(1).EditarAsync(Arg.Any<EstoqueEntityDto>());

            // Verificar que os dados foram passados corretamente para o gateway
            estoqueAtualizado.Should().NotBeNull();
            estoqueAtualizado.Id.Should().Be(estoqueId);
            estoqueAtualizado.Insumo.Should().Be("Produto Específico para Teste de Atualização");
            estoqueAtualizado.Descricao.Should().Be("Descrição específica para teste de trânsito de dados na atualização");
            estoqueAtualizado.QuantidadeDisponivel.Should().Be(99);
            estoqueAtualizado.QuantidadeMinima.Should().Be(20);
            estoqueAtualizado.Preco.Should().Be(456.78m);

            // Verificar que os campos técnicos foram preservados
            estoqueAtualizado.Ativo.Should().BeTrue(); // Mantido do original
            estoqueAtualizado.DataCadastro.Should().Be(new DateTime(2023, 1, 15)); // Mantido do original
            estoqueAtualizado.DataAtualizacao.Should().NotBe(new DateTime(2023, 6, 30)); // Atualizado

            // Verificar que o resultado contém os mesmos dados
            resultado.Should().NotBeNull();
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(estoqueAtualizado, options =>
                options.Excluding(e => e.DataAtualizacao));
        }
    }
}
