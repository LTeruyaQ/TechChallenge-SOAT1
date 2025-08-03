using Aplicacao.DTOs.Requests.Estoque;
using Aplicacao.DTOs.Requests.OrdemServico.InsumoOS;
using Aplicacao.DTOs.Responses.OrdemServico;
using Aplicacao.DTOs.Responses.OrdemServico.InsumoOrdemServico;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.Jobs;
using Aplicacao.Servicos;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Especificacoes.Insumo;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using MecanicaOSTests.Fixtures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOSTests.Servicos
{
    public class InsumoOSServicoTests
    {
        private readonly Mock<IOrdemServicoServico> _oSServicoMock;
        private readonly Mock<IEstoqueServico> _estoqueServicoMock;
        private readonly Mock<VerificarEstoqueJob> _verificarEstoqueJobMock;
        private readonly Mock<IRepositorio<InsumoOS>> _repositorioMock;
        private readonly Mock<ILogServico<InsumoOSServico>> _logMock;
        private readonly Mock<IUnidadeDeTrabalho> _udtMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUsuarioLogadoServico> _usuarioLogadoServicoMock;

        private readonly InsumoOSServico _insumoOSServico;

        public InsumoOSServicoTests()
        {
            _oSServicoMock = new Mock<IOrdemServicoServico>();
            _estoqueServicoMock = new Mock<IEstoqueServico>();

            var estoqueRepoMock = new Mock<IRepositorio<Estoque>>();
            var usuarioRepoMock = new Mock<IRepositorio<Usuario>>();
            var alertaEstoqueRepoMock = new Mock<IRepositorio<AlertaEstoque>>();
            var servicoEmailMock = new Mock<IServicoEmail>();
            var logJobMock = new Mock<ILogServico<VerificarEstoqueJob>>();
            var udtJobMock = new Mock<IUnidadeDeTrabalho>();

            _verificarEstoqueJobMock = new Mock<VerificarEstoqueJob>(
                estoqueRepoMock.Object,
                usuarioRepoMock.Object,
                alertaEstoqueRepoMock.Object,
                servicoEmailMock.Object,
                logJobMock.Object,
                udtJobMock.Object
            );

            _repositorioMock = new Mock<IRepositorio<InsumoOS>>();
            _logMock = new Mock<ILogServico<InsumoOSServico>>();
            _udtMock = new Mock<IUnidadeDeTrabalho>();
            _mapperMock = new Mock<IMapper>();
            _usuarioLogadoServicoMock = new Mock<IUsuarioLogadoServico>();

            _mapperMock.Setup(m => m.Map<InsumoOS>(It.IsAny<CadastrarInsumoOSRequest>()))
                       .Returns((CadastrarInsumoOSRequest req) => new InsumoOS { EstoqueId = req.EstoqueId, Quantidade = req.Quantidade });

            _mapperMock.Setup(m => m.Map<List<InsumoOSResponse>>(It.IsAny<List<InsumoOS>>()))
                       .Returns((List<InsumoOS> insumos) => insumos.Select(i => new InsumoOSResponse { OrdemServicoId = i.OrdemServicoId, EstoqueId = i.EstoqueId, Quantidade = i.Quantidade }).ToList());

            _insumoOSServico = new InsumoOSServico(
                _oSServicoMock.Object,
                _estoqueServicoMock.Object,
                _verificarEstoqueJobMock.Object,
                _repositorioMock.Object,
                _logMock.Object,
                _udtMock.Object,
                _mapperMock.Object,
                _usuarioLogadoServicoMock.Object
            );
        }

        [Fact]
        public async Task Dado_RequestValido_Quando_CadastrarInsumosAsync_Entao_RetornaListaResponse()
        {
            // Arrange
            var osId = Guid.NewGuid();
            var request = InsumoOSFixture.CriarListaCadastrarInsumoOSRequestValida();
            var osResponse = new OrdemServicoResponse { Id = osId };
            var os = OrdemServicoFixture.CriarOrdemServicoValida();
            var estoque = EstoqueFixture.CriarEstoqueValido();
            estoque.QuantidadeDisponivel = 10;

            _oSServicoMock.Setup(s => s.ObterPorIdAsync(osId)).ReturnsAsync(osResponse);
            _mapperMock.Setup(m => m.Map<OrdemServico>(osResponse)).Returns(os);
            _repositorioMock.Setup(r => r.ListarAsync(It.IsAny<ObterInsumosOSPorOSEspecificacao>())).ReturnsAsync(new List<InsumoOS>());
            _estoqueServicoMock.Setup(s => s.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(EstoqueFixture.CriarEstoqueResponseValido(estoque.Id, 10));
            _mapperMock.Setup(m => m.Map<Estoque>(It.IsAny<Aplicacao.DTOs.Responses.Estoque.EstoqueResponse>())).Returns(estoque);
            _repositorioMock.Setup(r => r.CadastrarVariosAsync(It.IsAny<List<InsumoOS>>()))
                            .ReturnsAsync((List<InsumoOS> insumos) => insumos);
            _udtMock.Setup(u => u.Commit()).ReturnsAsync(true);

            // Act
            var result = await _insumoOSServico.CadastrarInsumosAsync(osId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Count, result.Count);
        }

        [Fact]
        public async Task Dado_EstoqueInsuficiente_Quando_CadastrarInsumosAsync_Entao_LancaInsumosIndisponiveisException()
        {
            // Arrange
            var osId = Guid.NewGuid();
            var request = InsumoOSFixture.CriarListaCadastrarInsumoOSRequestValida();
            var osResponse = new OrdemServicoResponse { Id = osId };
            var os = OrdemServicoFixture.CriarOrdemServicoValida();
            var estoque = EstoqueFixture.CriarEstoqueValido();
            estoque.QuantidadeDisponivel = 0; // Insufficient stock

            _oSServicoMock.Setup(s => s.ObterPorIdAsync(osId)).ReturnsAsync(osResponse);
            _mapperMock.Setup(m => m.Map<OrdemServico>(osResponse)).Returns(os);
            _repositorioMock.Setup(r => r.ListarAsync(It.IsAny<ObterInsumosOSPorOSEspecificacao>())).ReturnsAsync(new List<InsumoOS>());
            _estoqueServicoMock.Setup(s => s.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(EstoqueFixture.CriarEstoqueResponseValido(estoque.Id, 0));
            _mapperMock.Setup(m => m.Map<Estoque>(It.IsAny<Aplicacao.DTOs.Responses.Estoque.EstoqueResponse>())).Returns(estoque);

            // Act & Assert
            await Assert.ThrowsAsync<InsumosIndisponiveisException>(() => _insumoOSServico.CadastrarInsumosAsync(osId, request));
        }

        [Fact]
        public async Task Dado_InsumosValidos_Quando_DevolverInsumosAoEstoqueAsync_Entao_AtualizaEstoque()
        {
            // Arrange
            var insumos = InsumoOSFixture.CriarListaInsumoOSValida();
            var estoqueServiceMock = new Mock<IEstoqueServico>();

            // Act
            await _insumoOSServico.DevolverInsumosAoEstoqueAsync(insumos);

            // Assert
            foreach (var insumo in insumos)
            {
                _estoqueServicoMock.Verify(s => s.AtualizarAsync(insumo.EstoqueId, It.IsAny<AtualizarEstoqueRequest>()), Times.Once);
            }
        }
    }
}
