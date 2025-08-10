using API.Controllers;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.DTOs.Requests.Usuario;
using Aplicacao.DTOs.Responses.Usuario;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using Dominio.Enumeradores;

namespace MecanicaOSTests.API.Controllers
{
    public static class UsuarioFixture
    {
        public static CadastrarUsuarioRequest CriarCadastrarUsuarioRequestValido()
        {
            return new CadastrarUsuarioRequest
            {
                Email = "teste@email.com",
                Senha = "password",
                TipoUsuario = TipoUsuario.Admin
            };
        }

        public static AtualizarUsuarioRequest CriarAtualizarUsuarioRequestValido()
        {
            return new AtualizarUsuarioRequest
            {
                Email = "teste.editado@email.com"
            };
        }
    }

    public class UsuarioControllerTests
    {
        private readonly Mock<IUsuarioServico> _usuarioServicoMock;
        private readonly UsuarioController _controller;

        public UsuarioControllerTests()
        {
            _usuarioServicoMock = new Mock<IUsuarioServico>();
            _controller = new UsuarioController(_usuarioServicoMock.Object);
        }

        [Fact]
        public async Task ObterPorId_QuandoUsuarioExistir_DeveRetornarOkComUsuario()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var usuarioResponseDto = new UsuarioResponse { Id = usuarioId };
            _usuarioServicoMock.Setup(s => s.ObterPorIdAsync(usuarioId)).ReturnsAsync(usuarioResponseDto);

            // Act
            var resultado = await _controller.ObterPorId(usuarioId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var usuarioRetornado = Assert.IsType<UsuarioResponse>(okResult.Value);
            Assert.Equal(usuarioResponseDto.Id, usuarioRetornado.Id);
        }

        [Fact]
        public async Task Criar_ComDadosValidos_DeveRetornarCreatedAtAction()
        {
            // Arrange
            var cadastrarUsuarioDto = UsuarioFixture.CriarCadastrarUsuarioRequestValido();
            var usuarioResponseDto = new UsuarioResponse { Id = Guid.NewGuid() };
            _usuarioServicoMock.Setup(s => s.CadastrarAsync(cadastrarUsuarioDto)).ReturnsAsync(usuarioResponseDto);

            // Act
            var resultado = await _controller.Criar(cadastrarUsuarioDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(resultado);
            Assert.Equal(nameof(_controller.ObterPorId), createdAtActionResult.ActionName);
            Assert.Equal(usuarioResponseDto.Id, ((UsuarioResponse)createdAtActionResult.Value).Id);
        }

        [Fact]
        public async Task Atualizar_QuandoUsuarioExistir_DeveRetornarOk()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var atualizarUsuarioDto = UsuarioFixture.CriarAtualizarUsuarioRequestValido();
            var usuarioResponseDto = new UsuarioResponse { Id = usuarioId };
            _usuarioServicoMock.Setup(s => s.AtualizarAsync(usuarioId, atualizarUsuarioDto)).ReturnsAsync(usuarioResponseDto);

            // Act
            var resultado = await _controller.Atualizar(usuarioId, atualizarUsuarioDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.IsType<UsuarioResponse>(okResult.Value);
        }

        [Fact]
        public async Task Deletar_QuandoUsuarioExistir_DeveRetornarNoContent()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            _usuarioServicoMock.Setup(s => s.DeletarAsync(usuarioId)).ReturnsAsync(true);

            // Act
            var resultado = await _controller.Deletar(usuarioId);

            // Assert
            Assert.IsType<NoContentResult>(resultado);
        }

        [Fact]
        public async Task ObterTodos_DeveRetornarOkComListaDeUsuarios()
        {
            // Arrange
            var usuarios = new List<UsuarioResponse> { new UsuarioResponse() };
            _usuarioServicoMock.Setup(s => s.ObterTodosAsync()).ReturnsAsync(usuarios);

            // Act
            var resultado = await _controller.ObterTodos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.IsAssignableFrom<IEnumerable<UsuarioResponse>>(okResult.Value);
        }
    }
}
