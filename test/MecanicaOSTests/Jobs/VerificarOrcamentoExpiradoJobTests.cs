using Aplicacao.Interfaces.Servicos;
using Aplicacao.Jobs;
using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOSTests.Jobs
{
    public class VerificarOrcamentoExpiradoJobTests
    {
        private readonly Mock<IRepositorio<OrdemServico>> _ordemServicoRepositorioMock;
        private readonly Mock<IInsumoOSServico> _insumoOSServicoMock;
        private readonly Mock<IUnidadeDeTrabalho> _udtMock;
        private readonly Mock<ILogServico<VerificarOrcamentoExpiradoJob>> _logServicoMock;
        private readonly VerificarOrcamentoExpiradoJob _job;

        public VerificarOrcamentoExpiradoJobTests()
        {
            _ordemServicoRepositorioMock = new Mock<IRepositorio<OrdemServico>>();
            _insumoOSServicoMock = new Mock<IInsumoOSServico>();
            _udtMock = new Mock<IUnidadeDeTrabalho>();
            _logServicoMock = new Mock<ILogServico<VerificarOrcamentoExpiradoJob>>();
            _job = new VerificarOrcamentoExpiradoJob(
                _ordemServicoRepositorioMock.Object,
                _insumoOSServicoMock.Object,
                _udtMock.Object,
                _logServicoMock.Object
            );
        }

        [Fact]
        public async Task ExecutarAsync_DeveAtualizarStatus_QuandoHaOrcamentosExpirados()
        {
            // Arrange
            var ordens = new List<OrdemServico>
            {
                new OrdemServico { Status = StatusOrdemServico.AguardandoAprovação, InsumosOS = new List<InsumoOS>() }
            };
            _ordemServicoRepositorioMock.Setup(r => r.ListarAsync(It.IsAny<global::Dominio.Especificacoes.Base.Interfaces.IEspecificacao<OrdemServico>>())).ReturnsAsync(ordens);

            // Act
            await _job.ExecutarAsync();

            // Assert
            _insumoOSServicoMock.Verify(s => s.DevolverInsumosAoEstoqueAsync(It.IsAny<IEnumerable<InsumoOS>>()), Times.Once);
            _ordemServicoRepositorioMock.Verify(r => r.EditarVariosAsync(ordens), Times.Once);
            _udtMock.Verify(u => u.Commit(), Times.Once);
            ordens.First().Status.Should().Be(StatusOrdemServico.OrcamentoExpirado);
        }

        [Fact]
        public async Task ExecutarAsync_NaoDeveFazerNada_QuandoNaoHaOrcamentosExpirados()
        {
            // Arrange
            _ordemServicoRepositorioMock.Setup(r => r.ListarAsync(It.IsAny<global::Dominio.Especificacoes.Base.Interfaces.IEspecificacao<OrdemServico>>())).ReturnsAsync(new List<OrdemServico>());

            // Act
            await _job.ExecutarAsync();

            // Assert
            _insumoOSServicoMock.Verify(s => s.DevolverInsumosAoEstoqueAsync(It.IsAny<IEnumerable<InsumoOS>>()), Times.Never);
            _ordemServicoRepositorioMock.Verify(r => r.EditarVariosAsync(It.IsAny<IEnumerable<OrdemServico>>()), Times.Never);
            _udtMock.Verify(u => u.Commit(), Times.Never);
        }
    }
}
