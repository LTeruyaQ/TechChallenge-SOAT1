using Aplicacao.DTOs.Requests.OrdemServico.InsumoOS;
using Aplicacao.DTOs.Responses.OrdemServico;
using Aplicacao.DTOs.Responses.OrdemServico.InsumoOrdemServico;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.Jobs;
using Aplicacao.Servicos;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using FluentAssertions;
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
        private readonly Mock<IOrdemServicoServico> _ordemServicoServicoMock;
        private readonly Mock<IEstoqueServico> _estoqueServicoMock;
        private readonly Mock<VerificarEstoqueJob> _verificarEstoqueJobMock;
        private readonly Mock<IRepositorio<InsumoOS>> _repositorioMock;
        private readonly Mock<ILogServico<InsumoOSServico>> _logServicoMock;
        private readonly Mock<IUnidadeDeTrabalho> _udtMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUsuarioLogadoServico> _usuarioLogadoServicoMock;
        private readonly InsumoOSServico _insumoOSServico;

        public InsumoOSServicoTests()
        {
            _ordemServicoServicoMock = new Mock<IOrdemServicoServico>();
            _estoqueServicoMock = new Mock<IEstoqueServico>();
            _verificarEstoqueJobMock = new Mock<VerificarEstoqueJob>(null, null, null, null, null, null); // Simplified for this example
            _repositorioMock = new Mock<IRepositorio<InsumoOS>>();
            _logServicoMock = new Mock<ILogServico<InsumoOSServico>>();
            _udtMock = new Mock<IUnidadeDeTrabalho>();
            _mapperMock = new Mock<IMapper>();
            _usuarioLogadoServicoMock = new Mock<IUsuarioLogadoServico>();

            _insumoOSServico = new InsumoOSServico(
                _ordemServicoServicoMock.Object,
                _estoqueServicoMock.Object,
                _verificarEstoqueJobMock.Object,
                _repositorioMock.Object,
                _logServicoMock.Object,
                _udtMock.Object,
                _mapperMock.Object,
                _usuarioLogadoServicoMock.Object
            );
        }

        [Fact]
        public async Task CadastrarInsumosAsync_DeveCadastrarComSucesso()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var requests = new List<CadastrarInsumoOSRequest> { new CadastrarInsumoOSRequest { EstoqueId = Guid.NewGuid(), Quantidade = 1 } };
            var osResponse = new OrdemServicoResponse { Id = ordemServicoId };
            var estoque = new Estoque("Óleo Motor", "Óleo sintético 5W30", (decimal)45.90, 10, 10);

            _ordemServicoServicoMock.Setup(s => s.ObterPorIdAsync(ordemServicoId)).ReturnsAsync(osResponse);
            _mapperMock.Setup(m => m.Map<OrdemServico>(osResponse)).Returns(new OrdemServico { Id = ordemServicoId });
            _repositorioMock.Setup(r => r.ListarAsync(It.IsAny<global::Dominio.Especificacoes.Base.Interfaces.IEspecificacao<InsumoOS>>())).ReturnsAsync(new List<InsumoOS>());
            _estoqueServicoMock.Setup(s => s.ObterPorIdAsync(requests[0].EstoqueId)).ReturnsAsync(new Aplicacao.DTOs.Responses.Estoque.EstoqueResponse());
            _mapperMock.Setup(m => m.Map<Estoque>(It.IsAny<Aplicacao.DTOs.Responses.Estoque.EstoqueResponse>())).Returns(estoque);
            _udtMock.Setup(u => u.Commit()).ReturnsAsync(true);
            _mapperMock.Setup(m => m.Map<InsumoOS>(It.IsAny<CadastrarInsumoOSRequest>())).Returns(new InsumoOS { EstoqueId = requests[0].EstoqueId, Quantidade = 1 });
            _repositorioMock.Setup(r => r.CadastrarVariosAsync(It.IsAny<IEnumerable<InsumoOS>>())).ReturnsAsync((IEnumerable<InsumoOS> insumos) => insumos.ToList());
            _mapperMock.Setup(m => m.Map<List<InsumoOSResponse>>(It.IsAny<List<InsumoOS>>())).Returns(new List<InsumoOSResponse> { new InsumoOSResponse() });

            // Act
            var result = await _insumoOSServico.CadastrarInsumosAsync(ordemServicoId, requests);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task CadastrarInsumosAsync_DeveLancarExcecao_QuandoOrdemDeServicoNaoEncontrada()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var requests = new List<CadastrarInsumoOSRequest>();

            _ordemServicoServicoMock.Setup(s => s.ObterPorIdAsync(ordemServicoId)).ReturnsAsync((OrdemServicoResponse)null);

            // Act & Assert
            await _insumoOSServico.Invoking(s => s.CadastrarInsumosAsync(ordemServicoId, requests))
                .Should().ThrowAsync<DadosNaoEncontradosException>().WithMessage("Ordem de serviço não encontrada");
        }

        [Fact]
        public async Task CadastrarInsumosAsync_DeveLancarExcecao_QuandoInsumoJaCadastrado()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var estoqueId = Guid.NewGuid();
            var requests = new List<CadastrarInsumoOSRequest> { new CadastrarInsumoOSRequest { EstoqueId = estoqueId, Quantidade = 1 } };
            var osResponse = new OrdemServicoResponse { Id = ordemServicoId };
            var insumosExistentes = new List<InsumoOS> { new InsumoOS { EstoqueId = estoqueId } };

            _ordemServicoServicoMock.Setup(s => s.ObterPorIdAsync(ordemServicoId)).ReturnsAsync(osResponse);
            _mapperMock.Setup(m => m.Map<OrdemServico>(osResponse)).Returns(new OrdemServico { Id = ordemServicoId });
            _repositorioMock.Setup(r => r.ListarAsync(It.IsAny<global::Dominio.Especificacoes.Base.Interfaces.IEspecificacao<InsumoOS>>())).ReturnsAsync(insumosExistentes);

            // Act & Assert
            await _insumoOSServico.Invoking(s => s.CadastrarInsumosAsync(ordemServicoId, requests))
                .Should().ThrowAsync<DadosJaCadastradosException>().WithMessage("Os insumos informados já estão cadastrados na Ordem de Serviço");
        }

        [Fact]
        public async Task CadastrarInsumosAsync_DeveLancarExcecao_QuandoEstoqueInsuficiente()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var requests = new List<CadastrarInsumoOSRequest> { new CadastrarInsumoOSRequest { EstoqueId = Guid.NewGuid(), Quantidade = 10 } };
            var osResponse = new OrdemServicoResponse { Id = ordemServicoId };
            var estoque = new Estoque("Óleo Motor", "Óleo sintético 5W30", 50, 5, 5);

            _ordemServicoServicoMock.Setup(s => s.ObterPorIdAsync(ordemServicoId)).ReturnsAsync(osResponse);
            _mapperMock.Setup(m => m.Map<OrdemServico>(osResponse)).Returns(new OrdemServico { Id = ordemServicoId });
            _repositorioMock.Setup(r => r.ListarAsync(It.IsAny<global::Dominio.Especificacoes.Base.Interfaces.IEspecificacao<InsumoOS>>())).ReturnsAsync(new List<InsumoOS>());
            _estoqueServicoMock.Setup(s => s.ObterPorIdAsync(requests[0].EstoqueId)).ReturnsAsync(new Aplicacao.DTOs.Responses.Estoque.EstoqueResponse());
            _mapperMock.Setup(m => m.Map<Estoque>(It.IsAny<Aplicacao.DTOs.Responses.Estoque.EstoqueResponse>())).Returns(estoque);
            _mapperMock.Setup(m => m.Map<InsumoOS>(It.IsAny<CadastrarInsumoOSRequest>())).Returns(new InsumoOS { EstoqueId = requests[0].EstoqueId, Quantidade = 10 });

            // Act & Assert
            await _insumoOSServico.Invoking(s => s.CadastrarInsumosAsync(ordemServicoId, requests))
                .Should().ThrowAsync<InsumosIndisponiveisException>().WithMessage("Insumos insuficientes no estoque para atender ao serviço solicitado.");
        }

        [Fact]
        public async Task DevolverInsumosAoEstoqueAsync_DeveDevolverComSucesso()
        {
            // Arrange
            var insumos = new List<InsumoOS>
            {
                new InsumoOS { EstoqueId = Guid.NewGuid(), Quantidade = 2, Estoque = new Estoque("Óleo Motor", "Óleo sintético 5W30", (decimal)50, 8, 5) }
            };

            // Act
            await _insumoOSServico.DevolverInsumosAoEstoqueAsync(insumos);

            // Assert
            _estoqueServicoMock.Verify(s => s.AtualizarAsync(It.IsAny<Guid>(), It.IsAny<Aplicacao.DTOs.Requests.Estoque.AtualizarEstoqueRequest>()), Times.Once);
            insumos[0].Estoque.QuantidadeDisponivel.Should().Be(10);
        }
    }
}
