using Aplicacao.Servicos;
using Dominio.Entidades;
using Dominio.Interfaces.Servicos;
using MecanicaOSTests.Fixtures;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace MecanicaOSTests.Servicos
{
    public class OrcamentoServicoTests
    {
        private readonly Mock<ILogServico<OrcamentoServico>> _logMock;
        private readonly OrcamentoServico _orcamentoServico;

        public OrcamentoServicoTests()
        {
            _logMock = new Mock<ILogServico<OrcamentoServico>>();
            _orcamentoServico = new OrcamentoServico(_logMock.Object);
        }

        [Fact]
        public void Dado_OrdemServicoValidaComInsumos_Quando_GerarOrcamento_Entao_RetornaSomaDoServicoEInsumos()
        {
            // Arrange
            var ordemServico = OrdemServicoFixture.CriarOrdemServicoValida();
            ordemServico.Servico = ServicoFixture.CriarServico(100m);
            ordemServico.InsumosOS = new List<InsumoOS>
            {
                InsumoOSFixture.CriarInsumoOS(2, 50m), // 2 * 50 = 100
                InsumoOSFixture.CriarInsumoOS(1, 75m)  // 1 * 75 = 75
            };
            var valorEsperado = 275m;

            // Act
            var orcamento = _orcamentoServico.GerarOrcamento(ordemServico);

            // Assert
            Assert.Equal(valorEsperado, orcamento);
        }

        [Fact]
        public void Dado_OrdemServicoSemInsumos_Quando_GerarOrcamento_Entao_RetornaApenasValorDoServico()
        {
            // Arrange
            var ordemServico = OrdemServicoFixture.CriarOrdemServicoValida();
            ordemServico.Servico = ServicoFixture.CriarServico(100m);
            ordemServico.InsumosOS = new List<InsumoOS>();
            var valorEsperado = 100m;

            // Act
            var orcamento = _orcamentoServico.GerarOrcamento(ordemServico);

            // Assert
            Assert.Equal(valorEsperado, orcamento);
        }

        [Fact]
        public void Dado_OrdemDeServicoNula_Quando_GerarOrcamento_Entao_DeveLancarExcecao()
        {
            // Arrange
            OrdemServico ordemServico = null;

            // Act & Assert
            var exception = Assert.Throws<NullReferenceException>(() => _orcamentoServico.GerarOrcamento(ordemServico));
            _logMock.Verify(log => log.LogErro(It.IsAny<string>(), exception), Times.Once);
        }

        [Fact]
        public void Dado_OrdemServicoSemServico_Quando_GerarOrcamento_Entao_DeveLancarExcecao()
        {
            // Arrange
            var ordemServico = OrdemServicoFixture.CriarOrdemServicoValida();
            ordemServico.Servico = null;
            ordemServico.InsumosOS = new List<InsumoOS>();

            // Act & Assert
            var exception = Assert.Throws<NullReferenceException>(() => _orcamentoServico.GerarOrcamento(ordemServico));
            _logMock.Verify(log => log.LogErro(It.IsAny<string>(), exception), Times.Once);
        }

        [Fact]
        public void Dado_OrdemServicoComListaDeInsumosNula_Quando_GerarOrcamento_Entao_DeveLancarExcecao()
        {
            // Arrange
            var ordemServico = OrdemServicoFixture.CriarOrdemServicoValida();
            ordemServico.Servico = ServicoFixture.CriarServicoValido();
            ordemServico.InsumosOS = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _orcamentoServico.GerarOrcamento(ordemServico));
            _logMock.Verify(log => log.LogErro(It.IsAny<string>(), exception), Times.Once);
        }

        [Fact]
        public void Dado_InsumoComQuantidadeZero_Quando_GerarOrcamento_Entao_CalculaCorretamente()
        {
            // Arrange
            var ordemServico = OrdemServicoFixture.CriarOrdemServicoValida();
            ordemServico.Servico = ServicoFixture.CriarServico(100m);
            ordemServico.InsumosOS = new List<InsumoOS>
            {
                InsumoOSFixture.CriarInsumoOS(0, 50m),   // 0 * 50 = 0
                InsumoOSFixture.CriarInsumoOS(1, 75m)   // 1 * 75 = 75
            };
            var valorEsperado = 175m; // 100 (serviço) + 0 + 75

            // Act
            var orcamento = _orcamentoServico.GerarOrcamento(ordemServico);

            // Assert
            Assert.Equal(valorEsperado, orcamento);
        }

        [Fact]
        public void Dado_InsumoComPrecoZero_Quando_GerarOrcamento_Entao_CalculaCorretamente()
        {
            // Arrange
            var ordemServico = OrdemServicoFixture.CriarOrdemServicoValida();
            ordemServico.Servico = ServicoFixture.CriarServico(100m);
            ordemServico.InsumosOS = new List<InsumoOS>
            {
                InsumoOSFixture.CriarInsumoOS(2, 0m),    // 2 * 0 = 0
                InsumoOSFixture.CriarInsumoOS(1, 75m)   // 1 * 75 = 75
            };
            var valorEsperado = 175m; // 100 (serviço) + 0 + 75

            // Act
            var orcamento = _orcamentoServico.GerarOrcamento(ordemServico);

            // Assert
            Assert.Equal(valorEsperado, orcamento);
        }

        [Fact]
        public void Dado_ServicoComValorZero_Quando_GerarOrcamento_Entao_CalculaApenasInsumos()
        {
            // Arrange
            var ordemServico = OrdemServicoFixture.CriarOrdemServicoValida();
            ordemServico.Servico = ServicoFixture.CriarServico(0m); // Valor do serviço = 0
            ordemServico.InsumosOS = new List<InsumoOS>
            {
                InsumoOSFixture.CriarInsumoOS(2, 50m),   // 2 * 50 = 100
                InsumoOSFixture.CriarInsumoOS(1, 75m)   // 1 * 75 = 75
            };
            var valorEsperado = 175m; // 0 (serviço) + 100 + 75

            // Act
            var orcamento = _orcamentoServico.GerarOrcamento(ordemServico);

            // Assert
            Assert.Equal(valorEsperado, orcamento);
        }

        [Fact]
        public void Dado_ServicoComValorZeroESemInsumos_Quando_GerarOrcamento_Entao_RetornaZero()
        {
            // Arrange
            var ordemServico = OrdemServicoFixture.CriarOrdemServicoValida();
            ordemServico.Servico = ServicoFixture.CriarServico(0m);
            ordemServico.InsumosOS = new List<InsumoOS>();
            var valorEsperado = 0m;

            // Act
            var orcamento = _orcamentoServico.GerarOrcamento(ordemServico);

            // Assert
            Assert.Equal(valorEsperado, orcamento);
        }
    }
}
