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
            var ordemServicoId = Guid.NewGuid();
            var estoque = InsumosOSHandlerFixture.CriarEstoqueValido(10, 2);
            var insumos = InsumosOSHandlerFixture.CriarListaInsumosValida(estoque.Id, 2);
            insumos[0].Estoque = estoque;

            _fixture.ConfigurarMockUdtParaCommitSucesso();

            var handler = _fixture.CriarCadastrarInsumosHandler();

            // Act
            var resultado = await handler.Handle(ordemServicoId, insumos);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(1);

            var insumoOS = resultado.First();
            insumoOS.OrdemServicoId.Should().Be(ordemServicoId);
            insumoOS.EstoqueId.Should().Be(estoque.Id);
            insumoOS.Quantidade.Should().Be(2);

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.ReceivedWithAnyArgs().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoCadastrar.Received(1).LogFim(Arg.Any<string>(), Arg.Any<object>());
        }

        // Testes de validação cross-domain removidos - agora são responsabilidade do controller

        [Fact]
        public async Task Handle_QuandoCommitFalha_DeveLancarPersistirDadosException()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var estoque = InsumosOSHandlerFixture.CriarEstoqueValido(10, 2);
            var insumos = InsumosOSHandlerFixture.CriarListaInsumosValida(estoque.Id, 2);
            insumos[0].Estoque = estoque;

            _fixture.ConfigurarMockUdtParaCommitFalha();

            var handler = _fixture.CriarCadastrarInsumosHandler();

            // Act & Assert
            var act = async () => await handler.Handle(ordemServicoId, insumos);

            await act.Should().ThrowAsync<PersistirDadosException>()
                .WithMessage("Erro ao atualizar estoque");

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.ReceivedWithAnyArgs().Commit();
        }

        [Fact]
        public async Task Handle_ComMultiplosInsumos_DeveCadastrarTodosInsumos()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var estoque1 = InsumosOSHandlerFixture.CriarEstoqueValido(10, 2);
            var estoque2 = InsumosOSHandlerFixture.CriarEstoqueValido(5, 1);

            var insumos = InsumosOSHandlerFixture.CriarListaInsumosValida(estoque1.Id, 2);
            insumos[0].Estoque = estoque1;

            var insumo2 = InsumosOSHandlerFixture.CriarListaInsumosValida(estoque2.Id, 1)[0];
            insumo2.Estoque = estoque2;
            insumos.Add(insumo2);

            _fixture.ConfigurarMockUdtParaCommitSucesso();

            var handler = _fixture.CriarCadastrarInsumosHandler();

            // Act
            var resultado = await handler.Handle(ordemServicoId, insumos);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.ElementAt(0).OrdemServicoId.Should().Be(ordemServicoId);
            resultado.ElementAt(1).OrdemServicoId.Should().Be(ordemServicoId);

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.ReceivedWithAnyArgs().Commit();
        }

        [Fact]
        public async Task Handle_ComEstoqueAbaixoDoMinimo_DeveVerificarEstoque()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var estoque = InsumosOSHandlerFixture.CriarEstoqueValido(5, 4); // Ficará abaixo do mínimo após retirar 2
            var insumos = InsumosOSHandlerFixture.CriarListaInsumosValida(estoque.Id, 2);
            insumos[0].Estoque = estoque;

            _fixture.ConfigurarMockUdtParaCommitSucesso();

            var handler = _fixture.CriarCadastrarInsumosHandler();

            // Act
            var resultado = await handler.Handle(ordemServicoId, insumos);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(1);

            // Verificar que o verificador de estoque foi chamado
            await _fixture.VerificarEstoqueJobGateway.ReceivedWithAnyArgs().VerificarEstoqueAsync();

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.ReceivedWithAnyArgs().Commit();
        }
    }
}
