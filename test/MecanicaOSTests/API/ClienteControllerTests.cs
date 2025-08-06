using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aplicacao.DTOs.Requests;
using Aplicacao.DTOs.Requests.Cliente;
using Aplicacao.DTOs.Responses;
using Aplicacao.DTOs.Responses.Cliente;
using Aplicacao.Interfaces.Servicos;
using API.Controllers;
using AutoMapper;
using Dominio.Entidades;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using MecanicaOSTests.Fixtures;

namespace MecanicaOSTests.API
{
    [Collection(nameof(ClienteCollection))]
    public class ClienteControllerTests
    {
        private readonly ClienteFixture _clienteFixture;
        private readonly Mock<IClienteServico> _clienteServicoMock;
        private readonly ClienteController _clienteController;

        public ClienteControllerTests(ClienteFixture clienteFixture)
        {
            _clienteFixture = clienteFixture;
            _clienteServicoMock = new Mock<IClienteServico>();
            _clienteController = new ClienteController(_clienteServicoMock.Object);
        }

        [Fact]
        public async Task ObterTodos_DeveRetornarOkComClientes()
        {
            // Arrange
            var clientes = _clienteFixture.GerarClientes(5);
            var clientesResponse = new List<ClienteResponse>();
            foreach (var cliente in clientes)
            {
                clientesResponse.Add(new ClienteResponse { Id = cliente.Id, Nome = cliente.Nome });
            }

            _clienteServicoMock.Setup(s => s.ObterTodosAsync()).ReturnsAsync(clientesResponse);

            // Act
            var resultado = await _clienteController.ObterTodos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var clientesRetornados = Assert.IsAssignableFrom<IEnumerable<ClienteResponse>>(okResult.Value);
            clientesRetornados.Should().HaveCount(5);
        }

        [Fact]
        public async Task ObterPorId_DeveRetornarOkComCliente()
        {
            // Arrange
            var cliente = _clienteFixture.GerarCliente();
            var clienteResponse = new ClienteResponse { Id = cliente.Id, Nome = cliente.Nome };
            _clienteServicoMock.Setup(s => s.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(clienteResponse);

            // Act
            var resultado = await _clienteController.ObterPorId(Guid.NewGuid());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var clienteRetornado = Assert.IsAssignableFrom<ClienteResponse>(okResult.Value);
            clienteRetornado.Should().NotBeNull();
        }

        [Fact]
        public async Task Criar_DeveRetornarCreatedAtActionComCliente()
        {
            // Arrange
            var request = _clienteFixture.GerarCadastroClienteRequest();
            var cliente = _clienteFixture.GerarCliente();
            var clienteResponse = new ClienteResponse { Id = cliente.Id, Nome = cliente.Nome };
            _clienteServicoMock.Setup(s => s.CadastrarAsync(It.IsAny<CadastrarClienteRequest>())).ReturnsAsync(clienteResponse);

            // Act
            var resultado = await _clienteController.Criar(request);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(resultado);
            var clienteRetornado = Assert.IsAssignableFrom<ClienteResponse>(createdAtActionResult.Value);
            clienteRetornado.Should().NotBeNull();
        }

        [Fact]
        public async Task Atualizar_DeveRetornarOkComCliente()
        {
            // Arrange
            var request = _clienteFixture.GerarAtualizarClienteRequest();
            var cliente = _clienteFixture.GerarCliente();
            var clienteResponse = new ClienteResponse { Id = cliente.Id, Nome = cliente.Nome };
            _clienteServicoMock.Setup(s => s.AtualizarAsync(It.IsAny<Guid>(), It.IsAny<AtualizarClienteRequest>())).ReturnsAsync(clienteResponse);

            // Act
            var resultado = await _clienteController.Atualizar(Guid.NewGuid(), request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var clienteRetornado = Assert.IsAssignableFrom<ClienteResponse>(okResult.Value);
            clienteRetornado.Should().NotBeNull();
        }

        [Fact]
        public async Task Remover_DeveRetornarNoContent()
        {
            // Arrange
            _clienteServicoMock.Setup(s => s.RemoverAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            // Act
            var resultado = await _clienteController.Remover(Guid.NewGuid());

            // Assert
            Assert.IsType<NoContentResult>(resultado);
        }
    }
}
