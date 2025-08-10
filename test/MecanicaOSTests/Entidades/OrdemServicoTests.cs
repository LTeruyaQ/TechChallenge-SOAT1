using Dominio.Entidades;
using Dominio.Enumeradores;
using FluentAssertions;
using Moq;

namespace MecanicaOSTests.Entidades
{
    public class OrdemServicoTests
    {
        [Fact]
        public void Dado_DadosValidos_Quando_CriarOrdemServico_Entao_DeveCriarComSucesso()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var clienteMock = new Mock<Cliente>();
            var veiculoId = Guid.NewGuid();
            var veiculoMock = new Mock<Veiculo>();
            var servicoId = Guid.NewGuid();
            var servicoMock = new Mock<Servico>();
            var orcamento = 250.00m;
            var dataEnvioOrcamento = DateTime.UtcNow;
            var descricao = "Troca de óleo";
            var status = StatusOrdemServico.Recebida;

            // Act
            var ordemServico = new OrdemServico
            {
                ClienteId = clienteId,
                Cliente = clienteMock.Object,
                VeiculoId = veiculoId,
                Veiculo = veiculoMock.Object,
                ServicoId = servicoId,
                Servico = servicoMock.Object,
                Orcamento = orcamento,
                DataEnvioOrcamento = dataEnvioOrcamento,
                Descricao = descricao,
                Status = status
            };

            // Assert
            ordemServico.Should().NotBeNull();
            ordemServico.ClienteId.Should().Be(clienteId);
            ordemServico.Cliente.Should().BeEquivalentTo(clienteMock.Object);
            ordemServico.VeiculoId.Should().Be(veiculoId);
            ordemServico.Veiculo.Should().BeEquivalentTo(veiculoMock.Object);
            ordemServico.ServicoId.Should().Be(servicoId);
            ordemServico.Servico.Should().BeEquivalentTo(servicoMock.Object);
            ordemServico.Orcamento.Should().Be(orcamento);
            ordemServico.DataEnvioOrcamento.Should().Be(dataEnvioOrcamento);
            ordemServico.Descricao.Should().Be(descricao);
            ordemServico.Status.Should().Be(status);
        }

        [Fact]
        public void Dado_DadosValidos_Quando_Atualizar_Entao_DeveAtualizarComSucesso()
        {
            // Arrange
            var ordemServico = new OrdemServico
            {
                ClienteId = Guid.NewGuid(),
                VeiculoId = Guid.NewGuid(),
                ServicoId = Guid.NewGuid(),
                Descricao = "Descrição Antiga",
                Status = StatusOrdemServico.Recebida
            };
            var novoClienteId = Guid.NewGuid();
            var novoVeiculoId = Guid.NewGuid();
            var novoServicoId = Guid.NewGuid();
            var novaDescricao = "Nova Descrição";
            var novoStatus = StatusOrdemServico.Finalizada;

            // Act
            ordemServico.Atualizar(novoClienteId, novoVeiculoId, novoServicoId, novaDescricao, novoStatus);

            // Assert
            ordemServico.ClienteId.Should().Be(novoClienteId);
            ordemServico.VeiculoId.Should().Be(novoVeiculoId);
            ordemServico.ServicoId.Should().Be(novoServicoId);
            ordemServico.Descricao.Should().Be(novaDescricao);
            ordemServico.Status.Should().Be(novoStatus);
        }
    }
}
