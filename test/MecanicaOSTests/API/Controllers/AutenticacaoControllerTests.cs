using API.Controllers;
using Aplicacao.DTOs.Requests.Autenticacao;
using Aplicacao.DTOs.Requests.Usuario;
using Aplicacao.DTOs.Responses.Autenticacao;
using Aplicacao.DTOs.Responses.Usuario;
using Aplicacao.Interfaces.Servicos;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOSTests.API.Controllers
{
    public class AutenticacaoControllerTests
    {
        private readonly Mock<IAutenticacaoServico> _autenticacaoServicoMock;
        private readonly Mock<IUsuarioServico> _usuarioServicoMock;
        private readonly AutenticacaoController _controller;

        public AutenticacaoControllerTests()
        {
            _autenticacaoServicoMock = new Mock<IAutenticacaoServico>();
            _usuarioServicoMock = new Mock<IUsuarioServico>();
            _controller = new AutenticacaoController(_autenticacaoServicoMock.Object, _usuarioServicoMock.Object);
        }

        [Fact]
        public async Task Login_DeveRetornarOk_ComToken()
        {
            // Arrange
            var request = new AutenticacaoRequest { Email = "teste@teste.com", Senha = "123" };
            var authResponse = new AutenticacaoResponse { Token = "meu.token.jwt" };
            _autenticacaoServicoMock.Setup(s => s.AutenticarAsync(request)).ReturnsAsync(authResponse);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(authResponse);
        }

        [Fact]
        public async Task Registrar_DeveRetornarCreatedAtAction()
        {
            // Arrange
            var request = new CadastrarUsuarioRequest { Email = "novo@teste.com" };
            var userResponse = new UsuarioResponse { Email = "novo@teste.com" };
            _usuarioServicoMock.Setup(s => s.CadastrarAsync(request)).ReturnsAsync(userResponse);

            // Act
            var result = await _controller.Registrar(request);

            // Assert
            var createdAtActionResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdAtActionResult.ActionName.Should().Be(nameof(AutenticacaoController.Login));
            createdAtActionResult.Value.Should().Be(userResponse);
        }

        [Fact]
        public void ValidarToken_DeveRetornarOk()
        {
            // Act
            var result = _controller.ValidarToken();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().NotBeNull();
        }
    }
}
