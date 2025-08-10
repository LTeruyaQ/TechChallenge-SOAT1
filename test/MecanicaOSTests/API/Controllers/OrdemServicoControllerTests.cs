using API.Controllers;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.DTOs.Requests.OrdemServico;
using Aplicacao.DTOs.Responses.OrdemServico;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using Aplicacao.DTOs.Requests.OrdemServico.InsumoOS;
using Dominio.Enumeradores;
using Aplicacao.DTOs.Responses.OrdemServico.InsumoOrdemServico;

namespace MecanicaOSTests.API.Controllers
{
    public static class OrdemServicoFixture
    {
        public static CadastrarOrdemServicoRequest CriarCadastrarOrdemServicoRequestValido()
        {
            return new CadastrarOrdemServicoRequest
            {
                ClienteId = Guid.NewGuid(),
                VeiculoId = Guid.NewGuid(),
                ServicoId = Guid.NewGuid()
            };
        }

        public static List<CadastrarInsumoOSRequest> CriarListaCadastrarInsumoOSRequestValida()
        {
            return new List<CadastrarInsumoOSRequest>
            {
                new CadastrarInsumoOSRequest { EstoqueId = Guid.NewGuid(), Quantidade = 1 }
            };
        }
    }

    public class OrdemServicoControllerTests
    {
        private readonly Mock<IOrdemServicoServico> _ordemServicoServicoMock;
        private readonly Mock<IInsumoOSServico> _insumoOSServicoMock;
        private readonly OrdemServicoController _controller;

        public OrdemServicoControllerTests()
        {
            _ordemServicoServicoMock = new Mock<IOrdemServicoServico>();
            _insumoOSServicoMock = new Mock<IInsumoOSServico>();
            _controller = new OrdemServicoController(_ordemServicoServicoMock.Object, _insumoOSServicoMock.Object);
        }

        [Fact]
        public async Task ObterPorId_QuandoOrdemServicoExistir_DeveRetornarOkComOrdemServico()
        {
            // Arrange
            var osId = Guid.NewGuid();
            var osResponseDto = new OrdemServicoResponse { Id = osId };
            _ordemServicoServicoMock.Setup(s => s.ObterPorIdAsync(osId)).ReturnsAsync(osResponseDto);

            // Act
            var resultado = await _controller.ObterPorId(osId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var osRetornada = Assert.IsType<OrdemServicoResponse>(okResult.Value);
            Assert.Equal(osResponseDto.Id, osRetornada.Id);
        }

        [Fact]
        public async Task Criar_ComDadosValidos_DeveRetornarCreatedAtAction()
        {
            // Arrange
            var cadastrarOSDto = OrdemServicoFixture.CriarCadastrarOrdemServicoRequestValido();
            var osResponseDto = new OrdemServicoResponse { Id = Guid.NewGuid() };
            _ordemServicoServicoMock.Setup(s => s.CadastrarAsync(cadastrarOSDto)).ReturnsAsync(osResponseDto);

            // Act
            var resultado = await _controller.Criar(cadastrarOSDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(resultado);
            Assert.Equal(nameof(_controller.ObterPorId), createdAtActionResult.ActionName);
            Assert.Equal(osResponseDto.Id, ((OrdemServicoResponse)createdAtActionResult.Value).Id);
        }

        [Fact]
        public async Task AdicionarInsumosOS_DeveRetornarOk()
        {
            // Arrange
            var osId = Guid.NewGuid();
            var cadastrarInsumoDto = OrdemServicoFixture.CriarListaCadastrarInsumoOSRequestValida();
            _insumoOSServicoMock.Setup(s => s.CadastrarInsumosAsync(osId, cadastrarInsumoDto)).ReturnsAsync(new List<InsumoOSResponse>());

            // Act
            var resultado = await _controller.AdicionarInsumosOS(osId, cadastrarInsumoDto);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task AceitarOrcamento_DeveRetornarNoContent()
        {
            // Arrange
            var osId = Guid.NewGuid();
            _ordemServicoServicoMock.Setup(s => s.AceitarOrcamentoAsync(osId)).Returns(Task.CompletedTask);

            // Act
            var resultado = await _controller.AceitarOrcamento(osId);

            // Assert
            Assert.IsType<NoContentResult>(resultado);
        }

        [Fact]
        public async Task RecusarOrcamento_DeveRetornarNoContent()
        {
            // Arrange
            var osId = Guid.NewGuid();
            _ordemServicoServicoMock.Setup(s => s.RecusarOrcamentoAsync(osId)).Returns(Task.CompletedTask);

            // Act
            var resultado = await _controller.RecusarOrcamento(osId);

            // Assert
            Assert.IsType<NoContentResult>(resultado);
        }

        [Fact]
        public async Task ObterTodos_DeveRetornarOkComListaDeOrdensServico()
        {
            // Arrange
            var ordensServico = new List<OrdemServicoResponse> { new OrdemServicoResponse() };
            _ordemServicoServicoMock.Setup(s => s.ObterTodosAsync()).ReturnsAsync(ordensServico);

            // Act
            var resultado = await _controller.ObterTodos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.IsAssignableFrom<IEnumerable<OrdemServicoResponse>>(okResult.Value);
        }

        [Fact]
        public async Task ObterPorStatus_DeveRetornarOkComListaDeOrdensServico()
        {
            // Arrange
            var status = StatusOrdemServico.AguardandoAprovação;
            var ordensServico = new List<OrdemServicoResponse> { new OrdemServicoResponse() };
            _ordemServicoServicoMock.Setup(s => s.ObterPorStatusAsync(status)).ReturnsAsync(ordensServico);

            // Act
            var resultado = await _controller.ObterPorStatus(status);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.IsAssignableFrom<IEnumerable<OrdemServicoResponse>>(okResult.Value);
        }

        [Fact]
        public async Task Atualizar_QuandoOrdemServicoExistir_DeveRetornarOk()
        {
            // Arrange
            var osId = Guid.NewGuid();
            var atualizarOSDto = new AtualizarOrdemServicoRequest();
            var osResponseDto = new OrdemServicoResponse { Id = osId };
            _ordemServicoServicoMock.Setup(s => s.AtualizarAsync(osId, atualizarOSDto)).ReturnsAsync(osResponseDto);

            // Act
            var resultado = await _controller.Atualizar(osId, atualizarOSDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.IsType<OrdemServicoResponse>(okResult.Value);
        }
    }
}
