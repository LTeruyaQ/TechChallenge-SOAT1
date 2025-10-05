using API.Controllers;
using Core.Entidades;
using Core.Enumeradores;
using Core.Interfaces.root;
using Core.Interfaces.Servicos;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Security.Claims;

namespace MecanicaOS.UnitTests.API.Controllers
{
    /// <summary>
    /// Testes para UsuarioLogadoController (API)
    /// 
    /// IMPORTÂNCIA: Controller crítico para autenticação e autorização.
    /// Gerencia informações do usuário logado e suas permissões.
    /// 
    /// COBERTURA: Valida todos os endpoints relacionados ao usuário autenticado:
    /// - ObterDadosUsuarioLogado: Retorna dados do usuário atual
    /// - VerificarPermissoes: Valida permissões do usuário
    /// - ApenasAdmin: Endpoint restrito a administradores
    /// - ApenasComPermissaoGerenciarEstoque: Endpoint com permissão específica
    /// </summary>
    public class UsuarioLogadoControllerTests
    {
        private readonly ICompositionRoot _compositionRoot;
        private readonly IUsuarioLogadoServico _usuarioLogadoServico;
        private readonly HttpContextAccessor _httpContextAccessor;

        public UsuarioLogadoControllerTests()
        {
            _compositionRoot = Substitute.For<ICompositionRoot>();
            _usuarioLogadoServico = Substitute.For<IUsuarioLogadoServico>();
            _httpContextAccessor = new HttpContextAccessor();
        }

        private UsuarioLogadoController CriarController(bool autenticado = true)
        {
            var httpContext = new DefaultHttpContext();
            
            if (autenticado)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Email, "teste@teste.com"),
                    new Claim(ClaimTypes.Role, "Admin")
                };
                
                var identity = new ClaimsIdentity(claims, "TestAuth");
                httpContext.User = new ClaimsPrincipal(identity);
            }

            _httpContextAccessor.HttpContext = httpContext;
            _compositionRoot.CriarUsuarioLogadoServico().Returns(_usuarioLogadoServico);

            return new UsuarioLogadoController(_httpContextAccessor, _compositionRoot);
        }

        [Fact]
        public void Construtor_ComUsuarioAutenticado_DeveCriarServicoUsuarioLogado()
        {
            // Arrange & Act
            var controller = CriarController(autenticado: true);

            // Assert
            controller.Should().NotBeNull();
            _compositionRoot.Received(1).CriarUsuarioLogadoServico();
        }

        [Fact]
        public void Construtor_ComUsuarioNaoAutenticado_NaoDeveCriarServicoUsuarioLogado()
        {
            // Arrange & Act
            var controller = CriarController(autenticado: false);

            // Assert
            controller.Should().NotBeNull();
            _compositionRoot.DidNotReceive().CriarUsuarioLogadoServico();
        }

        [Fact]
        public void ObterDadosUsuarioLogado_ComUsuarioAutenticado_DeveRetornarDadosDoUsuario()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var usuario = new Usuario
            {
                Id = usuarioId,
                Email = "admin@teste.com",
                TipoUsuario = TipoUsuario.Admin
            };

            var claims = new List<Claim>
            {
                new Claim("permissao", "administrador"),
                new Claim(ClaimTypes.Email, "admin@teste.com")
            };

            _usuarioLogadoServico.EstaAutenticado.Returns(true);
            _usuarioLogadoServico.ObterUsuarioLogado().Returns(usuario);
            _usuarioLogadoServico.ObterTodasClaims().Returns(claims);

            var controller = CriarController();

            // Act
            var resultado = controller.ObterDadosUsuarioLogado();

            // Assert
            resultado.Should().BeOfType<OkObjectResult>();
            var okResult = resultado as OkObjectResult;
            okResult!.Value.Should().NotBeNull();

            // Verifica propriedades do objeto retornado
            var valorRetornado = okResult.Value;
            var propriedadeId = valorRetornado!.GetType().GetProperty("Id");
            var propriedadeEmail = valorRetornado.GetType().GetProperty("Email");
            var propriedadeTipoUsuario = valorRetornado.GetType().GetProperty("TipoUsuario");
            var propriedadeEstaAutenticado = valorRetornado.GetType().GetProperty("EstaAutenticado");
            var propriedadeClaims = valorRetornado.GetType().GetProperty("Claims");

            propriedadeId!.GetValue(valorRetornado).Should().Be(usuarioId);
            propriedadeEmail!.GetValue(valorRetornado).Should().Be("admin@teste.com");
            propriedadeTipoUsuario!.GetValue(valorRetornado).Should().Be(TipoUsuario.Admin);
            propriedadeEstaAutenticado!.GetValue(valorRetornado).Should().Be(true);
            propriedadeClaims!.GetValue(valorRetornado).Should().NotBeNull();
        }

        [Fact]
        public void ObterDadosUsuarioLogado_ComUsuarioNaoAutenticado_DeveRetornarUnauthorized()
        {
            // Arrange
            _usuarioLogadoServico.EstaAutenticado.Returns(false);
            var controller = CriarController();

            // Act
            var resultado = controller.ObterDadosUsuarioLogado();

            // Assert
            resultado.Should().BeOfType<UnauthorizedObjectResult>();
            var unauthorizedResult = resultado as UnauthorizedObjectResult;
            unauthorizedResult!.Value.Should().Be("Usuário não autenticado");
        }

        [Fact]
        public void ObterDadosUsuarioLogado_DeveChamarObterTodasClaimsEConverterParaDictionary()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Email = "teste@teste.com",
                TipoUsuario = TipoUsuario.Cliente
            };

            var claims = new List<Claim>
            {
                new Claim("permissao1", "valor1"),
                new Claim("permissao2", "valor2")
            };

            _usuarioLogadoServico.EstaAutenticado.Returns(true);
            _usuarioLogadoServico.ObterUsuarioLogado().Returns(usuario);
            _usuarioLogadoServico.ObterTodasClaims().Returns(claims);

            var controller = CriarController();

            // Act
            var resultado = controller.ObterDadosUsuarioLogado();

            // Assert
            _usuarioLogadoServico.Received(1).ObterTodasClaims();
            
            var okResult = resultado as OkObjectResult;
            var valorRetornado = okResult!.Value;
            var propriedadeClaims = valorRetornado!.GetType().GetProperty("Claims");
            var claimsRetornadas = propriedadeClaims!.GetValue(valorRetornado) as Dictionary<string, string>;
            
            claimsRetornadas.Should().NotBeNull();
            claimsRetornadas.Should().ContainKey("permissao1");
            claimsRetornadas.Should().ContainKey("permissao2");
            claimsRetornadas!["permissao1"].Should().Be("valor1");
            claimsRetornadas["permissao2"].Should().Be("valor2");
        }

        [Fact]
        public void VerificarPermissoes_ComUsuarioAutenticado_DeveRetornarPermissoes()
        {
            // Arrange
            _usuarioLogadoServico.EstaAutenticado.Returns(true);
            _usuarioLogadoServico.PossuiPermissao("cliente").Returns(true);
            _usuarioLogadoServico.PossuiPermissao("administrador").Returns(false);

            var controller = CriarController();

            // Act
            var resultado = controller.VerificarPermissoes();

            // Assert
            resultado.Should().BeOfType<OkObjectResult>();
            var okResult = resultado as OkObjectResult;
            
            var valorRetornado = okResult!.Value;
            var propriedadeCliente = valorRetornado!.GetType().GetProperty("Cliente");
            var propriedadeAdministrador = valorRetornado.GetType().GetProperty("Administrador");
            
            propriedadeCliente!.GetValue(valorRetornado).Should().Be(true);
            propriedadeAdministrador!.GetValue(valorRetornado).Should().Be(false);
            
            _usuarioLogadoServico.Received(1).PossuiPermissao("cliente");
            _usuarioLogadoServico.Received(1).PossuiPermissao("administrador");
        }

        [Fact]
        public void VerificarPermissoes_ComUsuarioNaoAutenticado_DeveRetornarUnauthorized()
        {
            // Arrange
            _usuarioLogadoServico.EstaAutenticado.Returns(false);
            var controller = CriarController();

            // Act
            var resultado = controller.VerificarPermissoes();

            // Assert
            resultado.Should().BeOfType<UnauthorizedObjectResult>();
            var unauthorizedResult = resultado as UnauthorizedObjectResult;
            unauthorizedResult!.Value.Should().Be("Usuário não autenticado");
        }

        [Fact]
        public void VerificarPermissoes_DeveVerificarAmbasPermissoes()
        {
            // Arrange
            _usuarioLogadoServico.EstaAutenticado.Returns(true);
            _usuarioLogadoServico.PossuiPermissao(Arg.Any<string>()).Returns(true);

            var controller = CriarController();

            // Act
            var resultado = controller.VerificarPermissoes();

            // Assert
            _usuarioLogadoServico.Received(1).PossuiPermissao("cliente");
            _usuarioLogadoServico.Received(1).PossuiPermissao("administrador");
            _usuarioLogadoServico.Received(2).PossuiPermissao(Arg.Any<string>());
        }

        [Fact]
        public void ApenasAdmin_DeveRetornarMensagemDeAcessoPermitido()
        {
            // Arrange
            var controller = CriarController();

            // Act
            var resultado = controller.ApenasAdmin();

            // Assert
            resultado.Should().BeOfType<OkObjectResult>();
            var okResult = resultado as OkObjectResult;
            
            var valorRetornado = okResult!.Value;
            var propriedadeMensagem = valorRetornado!.GetType().GetProperty("Mensagem");
            var mensagem = propriedadeMensagem!.GetValue(valorRetornado) as string;
            
            mensagem.Should().Be("Acesso permitido apenas para administradores");
        }

        [Fact]
        public void ApenasComPermissaoGerenciarEstoque_DeveRetornarMensagemDeAcessoPermitido()
        {
            // Arrange
            var controller = CriarController();

            // Act
            var resultado = controller.ApenasComPermissaoGerenciarEstoque();

            // Assert
            resultado.Should().BeOfType<OkObjectResult>();
            var okResult = resultado as OkObjectResult;
            
            var valorRetornado = okResult!.Value;
            var propriedadeMensagem = valorRetornado!.GetType().GetProperty("Mensagem");
            var mensagem = propriedadeMensagem!.GetValue(valorRetornado) as string;
            
            mensagem.Should().Be("Acesso permitido apenas para usuários com permissão de gerenciar estoque");
        }

        [Fact]
        public void ObterDadosUsuarioLogado_ComUsuarioCliente_DeveRetornarTipoCliente()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Email = "cliente@teste.com",
                TipoUsuario = TipoUsuario.Cliente
            };

            _usuarioLogadoServico.EstaAutenticado.Returns(true);
            _usuarioLogadoServico.ObterUsuarioLogado().Returns(usuario);
            _usuarioLogadoServico.ObterTodasClaims().Returns(new List<Claim>());

            var controller = CriarController();

            // Act
            var resultado = controller.ObterDadosUsuarioLogado();

            // Assert
            var okResult = resultado as OkObjectResult;
            var valorRetornado = okResult!.Value;
            var propriedadeTipoUsuario = valorRetornado!.GetType().GetProperty("TipoUsuario");
            
            propriedadeTipoUsuario!.GetValue(valorRetornado).Should().Be(TipoUsuario.Cliente);
        }
    }
}
