using API.Controllers;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.DTOs.Requests.Veiculo;
using Aplicacao.DTOs.Responses.Veiculo;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace MecanicaOSTests.API.Controllers
{
    public static class VeiculoFixture
    {
        public static CadastrarVeiculoRequest CriarCadastrarVeiculoRequestValido()
        {
            return new CadastrarVeiculoRequest
            {
                Placa = "ABC1234",
                Modelo = "Onix",
                Marca = "Chevrolet",
                Ano = "2020",
                Cor = "Preto",
                ClienteId = Guid.NewGuid()
            };
        }

        public static AtualizarVeiculoRequest CriarAtualizarVeiculoRequestValido()
        {
            return new AtualizarVeiculoRequest
            {
                Modelo = "Onix Plus",
                Marca = "Chevrolet",
                Ano = "2021",
                Cor = "Branco"
            };
        }
    }

    public class VeiculoControllerTests
    {
        private readonly Mock<IVeiculoServico> _veiculoServicoMock;
        private readonly Mock<ILogger<VeiculoController>> _loggerMock;
        private readonly VeiculoController _controller;

        public VeiculoControllerTests()
        {
            _veiculoServicoMock = new Mock<IVeiculoServico>();
            _loggerMock = new Mock<ILogger<VeiculoController>>();
            _controller = new VeiculoController(_veiculoServicoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ObterPorId_QuandoVeiculoExistir_DeveRetornarOkComVeiculo()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();
            var veiculoResponseDto = new VeiculoResponse { Id = veiculoId };
            _veiculoServicoMock.Setup(s => s.ObterPorIdAsync(veiculoId)).ReturnsAsync(veiculoResponseDto);

            // Act
            var resultado = await _controller.ObterPorId(veiculoId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var veiculoRetornado = Assert.IsType<VeiculoResponse>(okResult.Value);
            Assert.Equal(veiculoResponseDto.Id, veiculoRetornado.Id);
        }

        [Fact]
        public async Task Cadastrar_ComDadosValidos_DeveRetornarCreatedAtAction()
        {
            // Arrange
            var cadastrarVeiculoDto = VeiculoFixture.CriarCadastrarVeiculoRequestValido();
            var veiculoResponseDto = new VeiculoResponse { Id = Guid.NewGuid() };
            _veiculoServicoMock.Setup(s => s.CadastrarAsync(cadastrarVeiculoDto)).ReturnsAsync(veiculoResponseDto);

            // Act
            var resultado = await _controller.Cadastrar(cadastrarVeiculoDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(resultado);
            Assert.Equal(nameof(_controller.ObterPorId), createdAtActionResult.ActionName);
            Assert.Equal(veiculoResponseDto.Id, ((VeiculoResponse)createdAtActionResult.Value).Id);
        }

        [Fact]
        public async Task Editar_QuandoVeiculoExistir_DeveRetornarOk()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();
            var atualizarVeiculoDto = VeiculoFixture.CriarAtualizarVeiculoRequestValido();
            var veiculoResponseDto = new VeiculoResponse { Id = veiculoId };
            _veiculoServicoMock.Setup(s => s.AtualizarAsync(veiculoId, atualizarVeiculoDto)).ReturnsAsync(veiculoResponseDto);

            // Act
            var resultado = await _controller.Editar(veiculoId, atualizarVeiculoDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.IsType<VeiculoResponse>(okResult.Value);
        }

        [Fact]
        public async Task Deletar_QuandoVeiculoExistir_DeveRetornarNoContent()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();
            _veiculoServicoMock.Setup(s => s.DeletarAsync(veiculoId)).ReturnsAsync(true);

            // Act
            var resultado = await _controller.Deletar(veiculoId);

            // Assert
            Assert.IsType<NoContentResult>(resultado);
        }

        [Fact]
        public async Task ObterTodos_DeveRetornarOkComListaDeVeiculos()
        {
            // Arrange
            var veiculos = new List<VeiculoResponse> { new VeiculoResponse() };
            _veiculoServicoMock.Setup(s => s.ObterTodosAsync()).ReturnsAsync(veiculos);

            // Act
            var resultado = await _controller.ObterTodos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.IsAssignableFrom<IEnumerable<VeiculoResponse>>(okResult.Value);
        }

        [Fact]
        public async Task ObterPorCliente_DeveRetornarOkComListaDeVeiculos()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var veiculos = new List<VeiculoResponse> { new VeiculoResponse() };
            _veiculoServicoMock.Setup(s => s.ObterPorClienteAsync(clienteId)).ReturnsAsync(veiculos);

            // Act
            var resultado = await _controller.ObterPorCliente(clienteId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.IsAssignableFrom<IEnumerable<VeiculoResponse>>(okResult.Value);
        }
    }
}
