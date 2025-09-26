using Core.DTOs.Entidades.Estoque;
using Core.DTOs.Requests.OrdemServico.InsumoOS;
using Core.Entidades;
using Core.Especificacoes.Base.Interfaces;
using Core.Exceptions;
using MecanicaOS.UnitTests.Fixtures.Handlers;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.InsumosOS
{
    public class DevolverInsumosHandlerTests
    {
        private readonly InsumosOSHandlerFixture _fixture;
        public DevolverInsumosHandlerTests()
        {
            _fixture = new InsumosOSHandlerFixture();
        }

        [Fact]
        public async Task Handle_ComDadosValidos_DeveDevolverInsumos()
        {
            // Arrange
            var estoque = InsumosOSHandlerFixture.CriarEstoqueValido(10, 2);
            var insumos = new List<DevolverInsumoOSRequest>
            {
                new DevolverInsumoOSRequest
                {
                    EstoqueId = estoque.Id,
                    Quantidade = 2
                }
            };

            // Configurar o repositório para retornar o estoque
            _fixture.ConfigurarMockEstoqueRepositorioParaObterPorId(estoque.Id, estoque);

            // Capturar os DTOs que serão enviados ao repositório de estoque para atualização
            var estoqueDtos = _fixture.ConfigurarMockEstoqueRepositorioParaAtualizar(estoque.Id);

            // Configurar o mock do UnidadeDeTrabalho
            _fixture.ConfigurarMockUdtParaCommitSucesso();

            var handler = _fixture.CriarDevolverInsumosHandler();

            // Act
            var resultado = await handler.Handle(insumos);

            // Assert
            resultado.Should().BeTrue();

            // Verificar que o repositório foi chamado para obter o estoque
            await _fixture.RepositorioEstoque.ReceivedWithAnyArgs().ObterPorIdAsync(Arg.Any<Guid>());

            // Verificar que o repositório foi chamado para atualizar o estoque
            await _fixture.RepositorioEstoque.ReceivedWithAnyArgs().EditarAsync(Arg.Any<EstoqueEntityDto>());

            // Verificar que os DTOs capturados estão corretos
            estoqueDtos.Should().NotBeEmpty();
            estoqueDtos[0].Id.Should().Be(estoque.Id);
            estoqueDtos[0].QuantidadeDisponivel.Should().Be(12); // 10 (inicial) + 2 (devolvidos)

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.ReceivedWithAnyArgs().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoDevolverInsumos.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoDevolverInsumos.Received(1).LogFim(Arg.Any<string>(), Arg.Any<bool>());
        }

        [Fact]
        public async Task Handle_ComMultiplosInsumos_DeveDevolverTodosInsumos()
        {
            // Arrange
            var estoque1 = InsumosOSHandlerFixture.CriarEstoqueValido(10, 2);
            var estoque2 = new Estoque
            {
                Id = Guid.NewGuid(),
                Insumo = "Filtro de Óleo",
                Descricao = "Filtro de óleo compatível",
                Preco = 25.00m,
                QuantidadeDisponivel = 5,
                QuantidadeMinima = 1
            };

            var insumos = new List<DevolverInsumoOSRequest>
            {
                new DevolverInsumoOSRequest
                {
                    EstoqueId = estoque1.Id,
                    Quantidade = 2
                },
                new DevolverInsumoOSRequest
                {
                    EstoqueId = estoque2.Id,
                    Quantidade = 1
                }
            };

            // Configurar o repositório para retornar os estoques
            _fixture.ConfigurarMockEstoqueRepositorioParaObterPorId(estoque1.Id, estoque1);
            _fixture.ConfigurarMockEstoqueRepositorioParaObterPorId(estoque2.Id, estoque2);

            // Capturar os DTOs que serão enviados ao repositório de estoque para atualização
            var estoqueDtos1 = _fixture.ConfigurarMockEstoqueRepositorioParaAtualizar(estoque1.Id);
            var estoqueDtos2 = _fixture.ConfigurarMockEstoqueRepositorioParaAtualizar(estoque2.Id);

            // Configurar o mock do UnidadeDeTrabalho
            _fixture.ConfigurarMockUdtParaCommitSucesso();

            var handler = _fixture.CriarDevolverInsumosHandler();

            // Act
            var resultado = await handler.Handle(insumos);

            // Assert
            resultado.Should().BeTrue();

            // Verificar que o repositório foi chamado para obter cada estoque
            await _fixture.RepositorioEstoque.ReceivedWithAnyArgs().ObterPorIdAsync(Arg.Any<Guid>());

            // Verificar que o repositório foi chamado para atualizar os estoques
            await _fixture.RepositorioEstoque.ReceivedWithAnyArgs().EditarAsync(Arg.Any<EstoqueEntityDto>());

            // Verificar que os DTOs capturados estão corretos
            estoqueDtos1.Should().NotBeEmpty();
            estoqueDtos1[0].Id.Should().Be(estoque1.Id);
            estoqueDtos1[0].QuantidadeDisponivel.Should().Be(12); // 10 (inicial) + 2 (devolvidos)

            estoqueDtos2.Should().NotBeEmpty();
            estoqueDtos2[0].QuantidadeDisponivel.Should().Be(6); // 5 (inicial) + 1 (devolvido)

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.ReceivedWithAnyArgs().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoDevolverInsumos.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoDevolverInsumos.Received(1).LogFim(Arg.Any<string>(), Arg.Any<bool>());
        }

        [Fact]
        public async Task Handle_ComEstoqueInexistente_DeveLancarDadosNaoEncontradosException()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            var insumos = new List<DevolverInsumoOSRequest>
            {
                new DevolverInsumoOSRequest
                {
                    EstoqueId = estoqueId,
                    Quantidade = 2
                }
            };

            // Configurar o repositório para retornar null (estoque não encontrado)
            _fixture.RepositorioEstoque
                .ObterPorIdAsync(estoqueId)
                .Returns((EstoqueEntityDto)null);

            _fixture.RepositorioEstoque
                .ObterUmProjetadoAsync<Estoque>(Arg.Any<IEspecificacao<EstoqueEntityDto>>())
                .Returns((Estoque)null);

            var handler = _fixture.CriarDevolverInsumosHandler();

            // Act & Assert
            var act = async () => await handler.Handle(insumos);

            await act.Should().ThrowAsync<DadosNaoEncontradosException>()
                .WithMessage("Estoque não encontrado");

            // Verificar que o repositório foi chamado para obter o estoque
            await _fixture.RepositorioEstoque.ReceivedWithAnyArgs().ObterPorIdAsync(Arg.Any<Guid>());

            // Verificar que o repositório não foi chamado para atualizar o estoque
            await _fixture.RepositorioEstoque.DidNotReceiveWithAnyArgs().EditarAsync(Arg.Any<EstoqueEntityDto>());

            // Verificar que o commit não foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceiveWithAnyArgs().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoDevolverInsumos.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoDevolverInsumos.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosNaoEncontradosException>());
        }

        [Fact]
        public async Task Handle_QuandoCommitFalha_DeveLancarPersistirDadosException()
        {
            // Arrange
            var estoque = InsumosOSHandlerFixture.CriarEstoqueValido(10, 2);
            var insumos = new List<DevolverInsumoOSRequest>
            {
                new DevolverInsumoOSRequest
                {
                    EstoqueId = estoque.Id,
                    Quantidade = 2
                }
            };

            // Configurar o repositório para retornar o estoque
            _fixture.ConfigurarMockEstoqueRepositorioParaObterPorId(estoque.Id, estoque);

            // Capturar os DTOs que serão enviados ao repositório de estoque para atualização
            var estoqueDtos = _fixture.ConfigurarMockEstoqueRepositorioParaAtualizar(estoque.Id);

            // Configurar o mock do UnidadeDeTrabalho para falhar no commit
            _fixture.ConfigurarMockUdtParaCommitFalha();

            var handler = _fixture.CriarDevolverInsumosHandler();

            // Act & Assert
            var act = async () => await handler.Handle(insumos);

            await act.Should().ThrowAsync<PersistirDadosException>()
                .WithMessage("Erro ao atualizar estoque");

            // Verificar que o repositório foi chamado para obter o estoque
            await _fixture.RepositorioEstoque.ReceivedWithAnyArgs().ObterPorIdAsync(Arg.Any<Guid>());

            // Verificar que o repositório foi chamado para atualizar o estoque
            await _fixture.RepositorioEstoque.ReceivedWithAnyArgs().EditarAsync(Arg.Any<EstoqueEntityDto>());

            // Verificar que os DTOs capturados estão corretos
            estoqueDtos.Should().NotBeEmpty();
            estoqueDtos[0].QuantidadeDisponivel.Should().Be(12); // 10 (inicial) + 2 (devolvidos)

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.ReceivedWithAnyArgs().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoDevolverInsumos.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoDevolverInsumos.Received(1).LogErro(Arg.Any<string>(), Arg.Any<PersistirDadosException>());
        }

        [Fact]
        public async Task Handle_DevePassarDadosCorretamenteEntreHandlerERepositorio()
        {
            // Arrange
            var estoque = InsumosOSHandlerFixture.CriarEstoqueValido(10, 2);
            var quantidadeDevolver = 3;
            var insumos = new List<DevolverInsumoOSRequest>
            {
                new DevolverInsumoOSRequest
                {
                    EstoqueId = estoque.Id,
                    Quantidade = quantidadeDevolver
                }
            };

            // Configurar o repositório para retornar o estoque
            _fixture.ConfigurarMockEstoqueRepositorioParaObterPorId(estoque.Id, estoque);

            // Capturar os DTOs que serão enviados ao repositório de estoque para atualização
            var estoqueDtos = _fixture.ConfigurarMockEstoqueRepositorioParaAtualizar(estoque.Id);

            // Configurar o mock do UnidadeDeTrabalho
            _fixture.ConfigurarMockUdtParaCommitSucesso();

            var handler = _fixture.CriarDevolverInsumosHandler();

            // Act
            var resultado = await handler.Handle(insumos);

            // Assert
            resultado.Should().BeTrue();

            // Verificar que os DTOs capturados estão corretos
            estoqueDtos.Should().NotBeEmpty();
            estoqueDtos[0].Id.Should().Be(estoque.Id);
            estoqueDtos[0].QuantidadeDisponivel.Should().Be(13); // 10 (inicial) + 3 (devolvidos)
            estoqueDtos[0].Insumo.Should().Be(estoque.Insumo);
            estoqueDtos[0].Descricao.Should().Be(estoque.Descricao);
            estoqueDtos[0].Preco.Should().Be(estoque.Preco);
            estoqueDtos[0].QuantidadeMinima.Should().Be(estoque.QuantidadeMinima);
            estoqueDtos[0].Ativo.Should().Be(estoque.Ativo);
            estoqueDtos[0].DataCadastro.Should().Be(estoque.DataCadastro);

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.ReceivedWithAnyArgs().Commit();
        }
    }
}
