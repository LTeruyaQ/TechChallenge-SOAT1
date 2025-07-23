using API.Controllers;
using Aplicacao.DTOs.Requests.OrdemServico;
using Aplicacao.Interfaces.Servicos;
using Dominio.Enumeradores;
using Aplicacao.DTOs.Requests.OrdemServico.InsumoOS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Collections.Generic;
using System;

namespace MecanicaOSTests.API
{
    public class OrdemServicoControllerTests
    {
        private readonly Mock<IOrdemServicoServico> _ordemServicoServicoMock = new();
        private readonly Mock<IInsumoOSServico> _insumoOSServicoMock = new();

        private OrdemServicoController CriarController()
        {
            var controller = new OrdemServicoController(_ordemServicoServicoMock.Object, _insumoOSServicoMock.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            return controller;
        }

        [Fact]
        public async Task ObterTodos_QuandoChamado_RetornaOk()
        {
            // Arrange
            var controller = CriarController();

            // Act
            var result = await controller.ObterTodos();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ObterPorId_QuandoChamado_RetornaOk()
        {
            // Arrange
            var controller = CriarController();
            _ordemServicoServicoMock.Setup(x => x.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Aplicacao.DTOs.Responses.OrdemServico.OrdemServicoResponse());

            // Act
            var result = await controller.ObterPorId(Guid.NewGuid());

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ObterPorStatus_QuandoChamado_RetornaOk()
        {
            // Arrange
            var controller = CriarController();

            // Act
            var result = await controller.ObterPorStatus(StatusOrdemServico.Recebida);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Criar_QuandoChamado_RetornaCreatedAtAction()
        {
            // Arrange
            var controller = CriarController();
            var request = new CadastrarOrdemServicoRequest();
            _ordemServicoServicoMock.Setup(x => x.CadastrarAsync(request)).ReturnsAsync(new Aplicacao.DTOs.Responses.OrdemServico.OrdemServicoResponse { Id = Guid.NewGuid() });

            // Act
            var result = await controller.Criar(request);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Atualizar_QuandoChamado_RetornaOk()
        {
            // Arrange
            var controller = CriarController();
            var request = new AtualizarOrdemServicoRequest();

            // Act
            var result = await controller.Atualizar(Guid.NewGuid(), request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task AdicionarInsumosOS_QuandoChamado_RetornaOk()
        {
            // Arrange
            var controller = CriarController();
            var request = new List<CadastrarInsumoOSRequest>();

            // Act
            var result = await controller.AdicionarInsumosOS(Guid.NewGuid(), request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task AceitarOrcamento_QuandoChamado_RetornaNoContent()
        {
            // Arrange
            var controller = CriarController();

            // Act
            var result = await controller.AceitarOrcamento(Guid.NewGuid());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task RecusarOrcamento_QuandoChamado_RetornaNoContent()
        {
            // Arrange
            var controller = CriarController();

            // Act
            var result = await controller.RecusarOrcamento(Guid.NewGuid());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
