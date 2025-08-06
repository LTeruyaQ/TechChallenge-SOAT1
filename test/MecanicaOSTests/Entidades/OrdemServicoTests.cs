using Dominio.Entidades;
using Dominio.Enumeradores;
using FluentAssertions;
using MecanicaOSTests.Fixtures;

namespace MecanicaOSTests.Entidades
{
    public class OrdemServicoTests
    {
        private readonly OrdemServicoFixture _ordemServicoFixture;

        public OrdemServicoTests()
        {
            _ordemServicoFixture = new OrdemServicoFixture();
        }

        [Fact]
        public void Dado_DadosValidos_Quando_CriarOrdemServico_Entao_DeveCriarComSucesso()
        {
            //Arrange
            var ordemServico = _ordemServicoFixture.CriarOrdemServicoValida();

            //Assert
            ordemServico.ClienteId.Should().NotBeEmpty();
            ordemServico.VeiculoId.Should().NotBeEmpty();
            ordemServico.ServicoId.Should().NotBeEmpty();
            ordemServico.Status.Should().Be(StatusOrdemServico.Recebida);
        }

        [Fact]
        public void Dado_DadosValidos_Quando_AtualizarOrdemServico_Entao_DeveAtualizarComSucesso()
        {
            //Arrange
            var ordemServico = _ordemServicoFixture.CriarOrdemServicoValida();
            var novoClienteId = Guid.NewGuid();
            var novoVeiculoId = Guid.NewGuid();
            var novoServicoId = Guid.NewGuid();
            var novaDescricao = "Nova descrição";
            var novoStatus = StatusOrdemServico.EmExecucao;

            //Act
            ordemServico.Atualizar(novoClienteId, novoVeiculoId, novoServicoId, novaDescricao, novoStatus);

            //Assert
            ordemServico.ClienteId.Should().Be(novoClienteId);
            ordemServico.VeiculoId.Should().Be(novoVeiculoId);
            ordemServico.ServicoId.Should().Be(novoServicoId);
            ordemServico.Descricao.Should().Be(novaDescricao);
            ordemServico.Status.Should().Be(novoStatus);
        }

        [Fact]
        public void Dado_DadosNulosOuVazios_Quando_AtualizarOrdemServico_Entao_NaoDeveAtualizar()
        {
            //Arrange
            var ordemServico = _ordemServicoFixture.CriarOrdemServicoValida();
            var clienteIdOriginal = ordemServico.ClienteId;
            var veiculoIdOriginal = ordemServico.VeiculoId;
            var servicoIdOriginal = ordemServico.ServicoId;
            var descricaoOriginal = ordemServico.Descricao;
            var statusOriginal = ordemServico.Status;

            //Act
            ordemServico.Atualizar(null, null, null, null, null);

            //Assert
            ordemServico.ClienteId.Should().Be(clienteIdOriginal);
            ordemServico.VeiculoId.Should().Be(veiculoIdOriginal);
            ordemServico.ServicoId.Should().Be(servicoIdOriginal);
            ordemServico.Descricao.Should().Be(descricaoOriginal);
            ordemServico.Status.Should().Be(statusOriginal);
        }
    }
}
