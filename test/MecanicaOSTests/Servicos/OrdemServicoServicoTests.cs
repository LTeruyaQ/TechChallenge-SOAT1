using Aplicacao.DTOs.Requests.OrdemServico;
using Aplicacao.DTOs.Responses.Servico;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.Notificacoes.OS;
using Aplicacao.Servicos;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using FluentAssertions;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Dominio.Especificacoes.Base.Interfaces;

namespace MecanicaOSTests.Servicos
{
    public class OrdemServicoServicoTests
    {
        private readonly Mock<IRepositorio<OrdemServico>> _repositorioMock;
        private readonly Mock<ILogServico<OrdemServicoServico>> _logServicoMock;
        private readonly Mock<IUnidadeDeTrabalho> _udtMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IRepositorio<Cliente>> _clienteRepositorioMock;
        private readonly Mock<IServicoServico> _servicoServicoMock;
        private readonly Mock<IUsuarioLogadoServico> _usuarioLogadoServicoMock;
        private readonly OrdemServicoServico _ordemServicoServico;

        public OrdemServicoServicoTests()
        {
            _repositorioMock = new Mock<IRepositorio<OrdemServico>>();
            _logServicoMock = new Mock<ILogServico<OrdemServicoServico>>();
            _udtMock = new Mock<IUnidadeDeTrabalho>();
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>();
            _clienteRepositorioMock = new Mock<IRepositorio<Cliente>>();
            _servicoServicoMock = new Mock<IServicoServico>();
            _usuarioLogadoServicoMock = new Mock<IUsuarioLogadoServico>();

            _ordemServicoServico = new OrdemServicoServico(
                _repositorioMock.Object,
                _logServicoMock.Object,
                _udtMock.Object,
                _mapperMock.Object,
                _mediatorMock.Object,
                _clienteRepositorioMock.Object,
                _servicoServicoMock.Object,
                _usuarioLogadoServicoMock.Object
            );
        }

        [Fact]
        public async Task CadastrarAsync_DeveCadastrarComSucesso()
        {
            // Arrange
            var request = new CadastrarOrdemServicoRequest { ClienteId = Guid.NewGuid(), VeiculoId = Guid.NewGuid(), ServicoId = Guid.NewGuid() };
            var cliente = new Cliente { Id = request.ClienteId, Veiculos = new List<Veiculo> { new Veiculo { Id = request.VeiculoId } } };
            var servico = new ServicoResponse { Disponivel = true };
            var os = new OrdemServico();

            _clienteRepositorioMock.Setup(r => r.ObterUmAsync(It.IsAny<Dominio.Especificacoes.Base.Interfaces.IEspecificacao<Cliente>>())).ReturnsAsync(cliente);
            _servicoServicoMock.Setup(s => s.ObterServicoPorIdAsync(request.ServicoId)).ReturnsAsync(servico);
            _mapperMock.Setup(m => m.Map<OrdemServico>(request)).Returns(os);
            _repositorioMock.Setup(r => r.CadastrarAsync(os)).ReturnsAsync(os);
            _udtMock.Setup(u => u.Commit()).ReturnsAsync(true);

            // Act
            var result = await _ordemServicoServico.CadastrarAsync(request);

            // Assert
            result.Should().NotBeNull();
            _repositorioMock.Verify(r => r.CadastrarAsync(os), Times.Once);
        }

        [Fact]
        public async Task AtualizarAsync_DevePublicarEvento_QuandoStatusMuda()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new AtualizarOrdemServicoRequest { Status = StatusOrdemServico.EmDiagnostico };
            var os = new OrdemServico { Id = id };

            _repositorioMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(os);
            _udtMock.Setup(u => u.Commit()).ReturnsAsync(true);

            // Act
            await _ordemServicoServico.AtualizarAsync(id, request);

            // Assert
            _mediatorMock.Verify(m => m.Publish(It.IsAny<OrdemServicoEmOrcamentoEvent>(), default), Times.Once);
        }

        [Fact]
        public async Task AceitarOrcamentoAsync_DeveAtualizarStatusParaEmExecucao()
        {
            // Arrange
            var id = Guid.NewGuid();
            var os = new OrdemServico { Id = id, Status = StatusOrdemServico.AguardandoAprovação, DataEnvioOrcamento = DateTime.UtcNow };

            _repositorioMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(os);
            _udtMock.Setup(u => u.Commit()).ReturnsAsync(true);

            // Act
            await _ordemServicoServico.AceitarOrcamentoAsync(id);

            // Assert
            os.Status.Should().Be(StatusOrdemServico.EmExecucao);
            _repositorioMock.Verify(r => r.EditarAsync(os), Times.Once);
        }

        [Fact]
        public async Task RecusarOrcamentoAsync_DeveAtualizarStatusParaCancelada()
        {
            // Arrange
            var id = Guid.NewGuid();
            var os = new OrdemServico { Id = id, Status = StatusOrdemServico.AguardandoAprovação, DataEnvioOrcamento = DateTime.UtcNow };

            _repositorioMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(os);
            _udtMock.Setup(u => u.Commit()).ReturnsAsync(true);

            // Act
            await _ordemServicoServico.RecusarOrcamentoAsync(id);

            // Assert
            os.Status.Should().Be(StatusOrdemServico.Cancelada);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<OrdemServicoCanceladaEvent>(), default), Times.Once);
        }

        [Fact]
        public async Task ProcessarRespostaOrcamentoAsync_DeveLancarExcecao_QuandoOrcamentoExpirado()
        {
            // Arrange
            var id = Guid.NewGuid();
            var os = new OrdemServico { Id = id, Status = StatusOrdemServico.OrcamentoExpirado };

            _repositorioMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(os);

            // Act & Assert
            await _ordemServicoServico.Invoking(s => s.AceitarOrcamentoAsync(id))
                .Should().ThrowAsync<OrcamentoExpiradoException>().WithMessage("Orçamento expirado");
        }
    }
}
