using API.Controllers;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.DTOs.Requests.Servico;
using Aplicacao.DTOs.Responses.Servico;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;

namespace MecanicaOSTests.API.Controllers
{
    public static class ServicosFixture
    {
        public static CadastrarServicoRequest CriarCadastrarServicoRequestValido()
        {
            return new CadastrarServicoRequest
            {
                Nome = "Troca de Oleo",
                Descricao = "Troca de oleo do motor",
                Valor = 100,
                Disponivel = true
            };
        }

        public static EditarServicoRequest CriarEditarServicoRequestValido()
        {
            return new EditarServicoRequest
            {
                Nome = "Troca de Oleo Super",
                Descricao = "Troca de oleo do motor super",
                Valor = 120,
                Disponivel = false
            };
        }
    }

    public class ServicosControllerTests
    {
        private readonly Mock<IServicoServico> _servicoServicoMock;
        private readonly ServicosController _controller;

        public ServicosControllerTests()
        {
            _servicoServicoMock = new Mock<IServicoServico>();
            _controller = new ServicosController(_servicoServicoMock.Object);
        }

        [Fact]
        public async Task ObterPorId_QuandoServicoExistir_DeveRetornarOkComServico()
        {
            // Arrange
            var servicoId = Guid.NewGuid();
            var servicoResponseDto = new ServicoResponse { Id = servicoId };
            _servicoServicoMock.Setup(s => s.ObterServicoPorIdAsync(servicoId)).ReturnsAsync(servicoResponseDto);

            // Act
            var resultado = await _controller.ObterPorId(servicoId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var servicoRetornado = Assert.IsType<ServicoResponse>(okResult.Value);
            Assert.Equal(servicoResponseDto.Id, servicoRetornado.Id);
        }

        [Fact]
        public async Task Criar_ComDadosValidos_DeveRetornarCreatedAtAction()
        {
            // Arrange
            var cadastrarServicoDto = ServicosFixture.CriarCadastrarServicoRequestValido();
            var servicoResponseDto = new ServicoResponse { Id = Guid.NewGuid() };
            _servicoServicoMock.Setup(s => s.CadastrarServicoAsync(cadastrarServicoDto)).ReturnsAsync(servicoResponseDto);

            // Act
            var resultado = await _controller.Criar(cadastrarServicoDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(resultado);
            Assert.Equal(nameof(_controller.ObterPorId), createdAtActionResult.ActionName);
            Assert.Equal(servicoResponseDto.Id, ((ServicoResponse)createdAtActionResult.Value).Id);
        }

        [Fact]
        public async Task Atualizar_QuandoServicoExistir_DeveRetornarOk()
        {
            // Arrange
            var servicoId = Guid.NewGuid();
            var editarServicoDto = ServicosFixture.CriarEditarServicoRequestValido();
            var servicoResponseDto = new ServicoResponse { Id = servicoId };
            _servicoServicoMock.Setup(s => s.EditarServicoAsync(servicoId, editarServicoDto)).ReturnsAsync(servicoResponseDto);

            // Act
            var resultado = await _controller.Atualizar(servicoId, editarServicoDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.IsType<ServicoResponse>(okResult.Value);
        }

        [Fact]
        public async Task Deletar_QuandoServicoExistir_DeveRetornarNoContent()
        {
            // Arrange
            var servicoId = Guid.NewGuid();
            _servicoServicoMock.Setup(s => s.DeletarServicoAsync(servicoId)).Returns(Task.CompletedTask);

            // Act
            var resultado = await _controller.Deletar(servicoId);

            // Assert
            Assert.IsType<NoContentResult>(resultado);
        }

        [Fact]
        public async Task ObterTodos_DeveRetornarOkComListaDeServicos()
        {
            // Arrange
            var servicos = new List<ServicoResponse> { new ServicoResponse() };
            _servicoServicoMock.Setup(s => s.ObterTodosAsync()).ReturnsAsync(servicos);

            // Act
            var resultado = await _controller.ObterTodos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.IsAssignableFrom<IEnumerable<ServicoResponse>>(okResult.Value);
        }

        [Fact]
        public async Task ObterServicosDisponiveis_DeveRetornarOkComListaDeServicos()
        {
            // Arrange
            var servicos = new List<ServicoResponse> { new ServicoResponse() };
            _servicoServicoMock.Setup(s => s.ObterServicosDisponiveisAsync()).ReturnsAsync(servicos);

            // Act
            var resultado = await _controller.ObterServicosDisponiveis();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.IsAssignableFrom<IEnumerable<ServicoResponse>>(okResult.Value);
        }
    }
}
