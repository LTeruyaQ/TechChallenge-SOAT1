using Core.DTOs.Entidades.Estoque;
using Core.DTOs.Entidades.OrdemServicos;
using Core.Entidades;
using Core.Especificacoes.Base.Interfaces;
using Core.Exceptions;
using MecanicaOS.UnitTests.Fixtures.Handlers;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.InsumosOS
{
    public class CadastrarInsumosHandlerTests
    {
        private readonly InsumosOSHandlerFixture _fixture;

        public CadastrarInsumosHandlerTests()
        {
            _fixture = new InsumosOSHandlerFixture();
        }

        [Fact]
        public async Task Handle_DevePassarDadosCorretamenteEntreHandlerEResponse()
        {
            // Arrange
            var ordemServico = InsumosOSHandlerFixture.CriarOrdemServicoValida();
            var estoque = InsumosOSHandlerFixture.CriarEstoqueValido(10, 2);
            var insumos = InsumosOSHandlerFixture.CriarListaInsumosValida(estoque.Id, 2);

            // Configurar o repositório para retornar a ordem de serviço
            _fixture.ConfigurarMockOrdemServicoRepositorioParaObterPorId(ordemServico.Id, ordemServico);

            // Configurar o repositório para retornar o estoque
            _fixture.ConfigurarMockEstoqueRepositorioParaObterPorId(estoque.Id, estoque);

            // Capturar os DTOs que serão enviados ao repositório de insumos
            var insumosOSDtos = _fixture.ConfigurarMockInsumoOSRepositorioParaInserir();

            // Capturar os DTOs que serão enviados ao repositório de estoque para atualização
            var estoqueDtos = _fixture.ConfigurarMockEstoqueRepositorioParaAtualizar(estoque.Id);

            _fixture.ConfigurarMockUdtParaCommitSucesso();

            var handler = _fixture.CriarCadastrarInsumosHandler();

            // Act
            var resultado = await handler.Handle(ordemServico.Id, insumos);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(1);

            var insumoOS = resultado.First();
            insumoOS.OrdemServicoId.Should().Be(ordemServico.Id);
            insumoOS.EstoqueId.Should().Be(estoque.Id);
            insumoOS.Quantidade.Should().Be(2);

            // Verificar que o repositório de estoque foi chamado para atualização
            await _fixture.RepositorioEstoque.ReceivedWithAnyArgs().EditarAsync(Arg.Any<EstoqueEntityDto>());

            // Verificar que os DTOs capturados estão corretos
            estoqueDtos.Should().NotBeEmpty();
            estoqueDtos[0].QuantidadeDisponivel.Should().Be(8);
            estoqueDtos[0].Id.Should().Be(estoque.Id);
            estoqueDtos[0].Insumo.Should().Be(estoque.Insumo);

            // Verificar o resultado retornado pelo handler
            resultado.First().OrdemServicoId.Should().Be(ordemServico.Id);
            resultado.First().EstoqueId.Should().Be(estoque.Id);
            resultado.First().Quantidade.Should().Be(2);

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.ReceivedWithAnyArgs().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoCadastrar.Received(1).LogFim(Arg.Any<string>(), Arg.Any<object>());
        }

        [Fact]
        public async Task Handle_ComOrdemServicoInexistente_DeveLancarDadosNaoEncontradosException()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var estoque = InsumosOSHandlerFixture.CriarEstoqueValido();
            var insumos = InsumosOSHandlerFixture.CriarListaInsumosValida(estoque.Id);

            // Configurar o repositório para retornar null (ordem de serviço não encontrada)
            _fixture.RepositorioOrdemServico
                .ObterPorIdAsync(ordemServicoId)
                .Returns((OrdemServicoEntityDto)null);

            _fixture.RepositorioOrdemServico
                .ObterUmProjetadoAsync<OrdemServico>(Arg.Any<IEspecificacao<OrdemServicoEntityDto>>())
                .Returns((OrdemServico)null);

            var handler = _fixture.CriarCadastrarInsumosHandler();

            // Act & Assert
            var act = async () => await handler.Handle(ordemServicoId, insumos);

            await act.Should().ThrowAsync<DadosNaoEncontradosException>()
                .WithMessage("Ordem de serviço não encontrada");

            // Verificar que o commit não foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceiveWithAnyArgs().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.ReceivedWithAnyArgs().LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoCadastrar.ReceivedWithAnyArgs().LogErro(Arg.Any<string>(), Arg.Any<DadosNaoEncontradosException>());
        }

        [Fact]
        public async Task Handle_ComEstoqueInsuficiente_DeveLancarDadosInvalidosException()
        {
            // Arrange
            var ordemServico = InsumosOSHandlerFixture.CriarOrdemServicoValida();
            var estoque = InsumosOSHandlerFixture.CriarEstoqueValido(5, 2);
            var insumos = InsumosOSHandlerFixture.CriarListaInsumosValida(estoque.Id, 10); // Quantidade maior que disponível

            // Configurar o repositório para retornar a ordem de serviço
            _fixture.ConfigurarMockOrdemServicoRepositorioParaObterPorId(ordemServico.Id, ordemServico);

            // Configurar o repositório para retornar o estoque
            _fixture.ConfigurarMockEstoqueRepositorioParaObterPorId(estoque.Id, estoque);

            var handler = _fixture.CriarCadastrarInsumosHandler();

            // Act & Assert
            var act = async () => await handler.Handle(ordemServico.Id, insumos);

            await act.Should().ThrowAsync<DadosInvalidosException>()
                .WithMessage($"Estoque insuficiente para o insumo {estoque.Insumo}");

            // Verificar que o repositório de estoque não foi chamado para atualização
            await _fixture.RepositorioEstoque.DidNotReceiveWithAnyArgs().EditarAsync(Arg.Any<EstoqueEntityDto>());

            // Verificar que o repositório de insumos não foi chamado para inserção
            await _fixture.RepositorioInsumoOS.DidNotReceiveWithAnyArgs().CadastrarAsync(Arg.Any<InsumoOSEntityDto>());

            // Verificar que o commit não foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceiveWithAnyArgs().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.ReceivedWithAnyArgs().LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoCadastrar.ReceivedWithAnyArgs().LogErro(Arg.Any<string>(), Arg.Any<DadosInvalidosException>());
        }

        [Fact]
        public async Task Handle_QuandoCommitFalha_DeveLancarPersistirDadosException()
        {
            // Arrange
            var ordemServico = InsumosOSHandlerFixture.CriarOrdemServicoValida();
            var estoque = InsumosOSHandlerFixture.CriarEstoqueValido(10, 2);
            var insumos = InsumosOSHandlerFixture.CriarListaInsumosValida(estoque.Id, 2);

            // Configurar o repositório para retornar a ordem de serviço
            _fixture.ConfigurarMockOrdemServicoRepositorioParaObterPorId(ordemServico.Id, ordemServico);

            // Configurar o repositório para retornar o estoque
            _fixture.ConfigurarMockEstoqueRepositorioParaObterPorId(estoque.Id, estoque);

            // Capturar os DTOs que serão enviados ao repositório de insumos
            var insumosOSDtos = _fixture.ConfigurarMockInsumoOSRepositorioParaInserir();

            // Capturar os DTOs que serão enviados ao repositório de estoque para atualização
            var estoqueDtos = _fixture.ConfigurarMockEstoqueRepositorioParaAtualizar(estoque.Id);

            _fixture.ConfigurarMockUdtParaCommitFalha();

            var handler = _fixture.CriarCadastrarInsumosHandler();

            // Act & Assert
            var act = async () => await handler.Handle(ordemServico.Id, insumos);

            await act.Should().ThrowAsync<PersistirDadosException>()
                .WithMessage("Erro ao atualizar estoque");

            // Verificar que o repositório de estoque foi chamado para atualização
            await _fixture.RepositorioEstoque.ReceivedWithAnyArgs().EditarAsync(Arg.Any<EstoqueEntityDto>());
            estoqueDtos.Should().NotBeEmpty();
            estoqueDtos[0].QuantidadeDisponivel.Should().Be(8);

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.ReceivedWithAnyArgs().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.ReceivedWithAnyArgs().LogInicio(Arg.Any<string>(), Arg.Any<object>());
        }

        [Fact]
        public async Task Handle_ComMultiplosInsumos_DeveCadastrarTodosInsumos()
        {
            // Arrange
            var ordemServico = InsumosOSHandlerFixture.CriarOrdemServicoValida();

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

            var estoqueIds = new List<Guid> { estoque1.Id, estoque2.Id };
            var quantidades = new List<int> { 2, 1 };
            var insumos = InsumosOSHandlerFixture.CriarListaInsumosMultiplosValida(estoqueIds, quantidades);

            // Configurar o repositório para retornar a ordem de serviço
            _fixture.ConfigurarMockOrdemServicoRepositorioParaObterPorId(ordemServico.Id, ordemServico);

            // Configurar o repositório para retornar os estoques
            _fixture.ConfigurarMockEstoqueRepositorioParaObterPorId(estoque1.Id, estoque1);
            _fixture.ConfigurarMockEstoqueRepositorioParaObterPorId(estoque2.Id, estoque2);

            // Capturar os DTOs que serão enviados ao repositório de insumos
            var insumosOSDtos = _fixture.ConfigurarMockInsumoOSRepositorioParaInserir();

            // Capturar os DTOs que serão enviados ao repositório de estoque para atualização
            var estoqueDtos = _fixture.ConfigurarMockEstoqueRepositorioParaAtualizar(estoque1.Id);
            _fixture.ConfigurarMockEstoqueRepositorioParaAtualizar(estoque2.Id);
            _fixture.ConfigurarMockUdtParaCommitSucesso();

            var handler = _fixture.CriarCadastrarInsumosHandler();

            // Act
            var resultado = await handler.Handle(ordemServico.Id, insumos);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);

            // Verificar que o repositório de estoque foi chamado para atualização
            await _fixture.RepositorioEstoque.ReceivedWithAnyArgs().EditarAsync(Arg.Any<EstoqueEntityDto>());

            // Verificar o resultado retornado pelo handler
            resultado.Should().HaveCount(2);
            resultado.ElementAt(0).OrdemServicoId.Should().Be(ordemServico.Id);
            resultado.ElementAt(1).OrdemServicoId.Should().Be(ordemServico.Id);

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.ReceivedWithAnyArgs().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.ReceivedWithAnyArgs().LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoCadastrar.ReceivedWithAnyArgs().LogFim(Arg.Any<string>(), Arg.Any<object>());
        }

        [Fact]
        public async Task Handle_ComEstoqueAbaixoDoMinimo_DeveVerificarEstoque()
        {
            // Arrange
            var ordemServico = InsumosOSHandlerFixture.CriarOrdemServicoValida();
            var estoque = InsumosOSHandlerFixture.CriarEstoqueValido(5, 4); // Ficará abaixo do mínimo após retirar 2
            var insumos = InsumosOSHandlerFixture.CriarListaInsumosValida(estoque.Id, 2);

            // Configurar o repositório para retornar a ordem de serviço
            _fixture.ConfigurarMockOrdemServicoRepositorioParaObterPorId(ordemServico.Id, ordemServico);

            // Configurar o repositório para retornar o estoque
            _fixture.ConfigurarMockEstoqueRepositorioParaObterPorId(estoque.Id, estoque);

            // Capturar os DTOs que serão enviados ao repositório de insumos
            var insumosOSDtos = _fixture.ConfigurarMockInsumoOSRepositorioParaInserir();

            // Capturar os DTOs que serão enviados ao repositório de estoque para atualização
            var estoqueDtos = _fixture.ConfigurarMockEstoqueRepositorioParaAtualizar(estoque.Id);

            _fixture.ConfigurarMockUdtParaCommitSucesso();

            var handler = _fixture.CriarCadastrarInsumosHandler();

            // Act
            var resultado = await handler.Handle(ordemServico.Id, insumos);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(1);

            // Verificar que o repositório de estoque foi chamado para atualização
            await _fixture.RepositorioEstoque.ReceivedWithAnyArgs().EditarAsync(Arg.Any<EstoqueEntityDto>());

            // Verificar que os DTOs capturados estão corretos
            estoqueDtos.Should().NotBeEmpty();
            estoqueDtos[0].QuantidadeDisponivel.Should().Be(3);

            // Verificar que o verificador de estoque foi chamado
            await _fixture.VerificarEstoqueJobGateway.ReceivedWithAnyArgs().VerificarEstoqueAsync();

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.ReceivedWithAnyArgs().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoCadastrar.Received(1).LogFim(Arg.Any<string>(), Arg.Any<object>());
        }

    }
}
