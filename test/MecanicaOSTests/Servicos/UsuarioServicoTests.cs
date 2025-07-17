using Aplicacao.DTOs.Requests.Usuario;
using Aplicacao.DTOs.Responses.Usuario;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.Servicos;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using FluentAssertions;
using MecanicaOSTests.Fixtures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Aplicacao.Servicos.Tests
{
    public class UsuarioServicoTests
    {
        private readonly Mock<IRepositorio<Usuario>> _repositorioMock;
        private readonly Mock<IUnidadeDeTrabalho> _uotMock;
        private readonly Mock<ILogServico<UsuarioServico>> _logMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IClienteServico> _clienteServicoMock;
        private readonly Mock<IServicoSenha> _servicoSenhaMock;
        private readonly UsuarioServico _servico;

        public UsuarioServicoTests()
        {
            _repositorioMock = new Mock<IRepositorio<Usuario>>();
            _uotMock = new Mock<IUnidadeDeTrabalho>();
            _logMock = new Mock<ILogServico<UsuarioServico>>();
            _mapperMock = new Mock<IMapper>();
            _clienteServicoMock = new Mock<IClienteServico>();
            _servicoSenhaMock = new Mock<IServicoSenha>();

            _servico = new UsuarioServico(
                _repositorioMock.Object,
                _logMock.Object,
                _uotMock.Object,
                _mapperMock.Object,
                _clienteServicoMock.Object,
                _servicoSenhaMock.Object);
        }

        private Usuario CriarUsuario() => UsuarioBogusFixture.CriarUsuarioValido();

        [Fact]
        public async Task Dado_NovoUsuarioClienteValido_Quando_CadastrarAsync_Entao_UsuarioCriadoComSucesso()
        {
            // Arrange
            var request = UsuarioBogusFixture.CriarCadastrarUsuarioRequestValido();
            request.TipoUsuario = TipoUsuario.Cliente;

            var usuario = UsuarioBogusFixture.CriarUsuarioValido();
            usuario.Email = request.Email;
            usuario.TipoUsuario = request.TipoUsuario;
            usuario.Senha = "senha_criptografada";

            var response = new UsuarioResponse
            {
                Id = usuario.Id,
                Email = usuario.Email,
                TipoUsuario = usuario.TipoUsuario,
                ClienteId = usuario.ClienteId
            };

            _repositorioMock.Setup(r => r.ObterUmSemRastreamentoAsync(It.IsAny<UsuarioPorEmailEspecificacao>()))
                .ReturnsAsync((Usuario)null);
                
            _servicoSenhaMock.Setup(s => s.CriptografarSenha(request.Senha))
                .Returns("senha_criptografada");
                
            _clienteServicoMock.Setup(c => c.ObterIdPorDocumentoAsync(request.Documento))
                .ReturnsAsync(usuario.ClienteId);
                
            _repositorioMock.Setup(r => r.CadastrarAsync(It.IsAny<Usuario>()))
                .ReturnsAsync(usuario);
                
            _uotMock.Setup(u => u.SalvarAlteracoesAsync())
                .ReturnsAsync(true);
                
            _mapperMock.Setup(m => m.Map<UsuarioResponse>(usuario))
                .Returns(response);

            // Act
            var resultado = await _servico.CadastrarAsync(request);

            // Assert
            resultado.Should().NotBeNull("porque o usuário foi cadastrado com sucesso");
            resultado.Id.Should().Be(usuario.Id, "porque o ID deve ser o mesmo do usuário cadastrado");
            resultado.Email.Should().Be(request.Email, "porque o email deve ser o mesmo da requisição");
            resultado.TipoUsuario.Should().Be(request.TipoUsuario, "porque o tipo de usuário deve ser o mesmo da requisição");
            resultado.ClienteId.Should().Be(usuario.ClienteId, "porque o ClienteId deve ser o mesmo do usuário cadastrado");
            
            _repositorioMock.Verify(
                r => r.CadastrarAsync(It.Is<Usuario>(u => 
                    u.Email == request.Email && 
                    u.Senha == "senha_criptografada" &&
                    u.TipoUsuario == request.TipoUsuario)),
                Times.Once,
                "porque deve cadastrar o usuário com os dados corretos");
                
            _uotMock.Verify(
                u => u.SalvarAlteracoesAsync(),
                Times.Once,
                "porque deve salvar as alterações no banco de dados");
        }

        [Fact]
        public async Task Dado_EmailJaCadastrado_Quando_CadastrarAsync_Entao_LancaExcecaoDadosJaCadastrados()
        {
            // Arrange
            var request = UsuarioBogusFixture.CriarCadastrarUsuarioRequestValido();
            request.TipoUsuario = TipoUsuario.Cliente;

            var usuarioExistente = UsuarioBogusFixture.CriarUsuarioValido();
            usuarioExistente.Email = request.Email;

            _repositorioMock.Setup(r => r.ObterUmSemRastreamentoAsync(It.IsAny<UsuarioPorEmailEspecificacao>()))
                .ReturnsAsync(usuarioExistente);

            // Act
            Func<Task> act = async () => await _servico.CadastrarAsync(request);

            // Assert
            await act.Should()
                .ThrowAsync<DadosJaCadastradosException>()
                .WithMessage("Já existe um usuário cadastrado com este e-mail");
                
            _logMock.Verify(
                x => x.LogAviso("CadastrarAsync", It.IsAny<string>()),
                Times.Once,
                "porque deve registrar um aviso quando tentar cadastrar usuário com e-mail existente");
        }

        [Fact]
        public async Task Dado_UsuarioExistente_Quando_ObterPorIdAsync_Entao_RetornaUsuario()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var usuario = CriarUsuario();
            usuario.Id = usuarioId;

            var usuarioResponse = new UsuarioResponse
            {
                Id = usuario.Id,
                Email = usuario.Email,
                TipoUsuario = usuario.TipoUsuario,
                ClienteId = usuario.ClienteId
            };

            _repositorioMock.Setup(r => r.ObterPorIdAsync(usuarioId))
                .ReturnsAsync(usuario);
                
            _mapperMock.Setup(m => m.Map<UsuarioResponse>(usuario))
                .Returns(usuarioResponse);

            // Act
            var resultado = await _servico.ObterPorIdAsync(usuarioId);

            // Assert
            resultado.Should().NotBeNull("porque o usuário existe no repositório");
            resultado.Id.Should().Be(usuarioId, "porque deve retornar o usuário com o ID especificado");
            resultado.Email.Should().Be(usuario.Email, "porque o email deve ser o mesmo do usuário cadastrado");
        }

        [Fact]
        public async Task Dado_UsuarioInexistente_Quando_ObterPorIdAsync_Entao_LancaExcecao()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            _repositorioMock.Setup(r => r.ObterPorIdAsync(usuarioId))
                .ReturnsAsync((Usuario)null);

            // Act
            Func<Task> act = async () => await _servico.ObterPorIdAsync(usuarioId);

            // Assert
            await act.Should()
                .ThrowAsync<EntidadeNaoEncontradaException>()
                .WithMessage($"Usuário com ID {usuarioId} não encontrado");
        }

        [Fact]
        public async Task Dado_EmailExistente_Quando_ObterPorEmailAsync_Entao_RetornaUsuario()
        {
            // Arrange
            var email = "usuario@teste.com";
            var usuario = CriarUsuario();
            usuario.Email = email;

            _repositorioMock.Setup(r => r.ObterUmSemRastreamentoAsync(It.IsAny<UsuarioPorEmailEspecificacao>()))
                .ReturnsAsync(usuario);

            // Act
            var resultado = await _servico.ObterPorEmailAsync(email);

            // Assert
            resultado.Should().NotBeNull("porque o usuário existe no repositório");
            resultado.Email.Should().Be(email, "porque deve retornar o usuário com o e-mail especificado");
        }

        [Fact]
        public async Task Dado_EmailInexistente_Quando_ObterPorEmailAsync_Entao_RetornaNulo()
        {
            // Arrange
            var email = "inexistente@teste.com";
            _repositorioMock.Setup(r => r.ObterUmSemRastreamentoAsync(It.IsAny<UsuarioPorEmailEspecificacao>()))
                .ReturnsAsync((Usuario)null);

            // Act
            var resultado = await _servico.ObterPorEmailAsync(email);

            // Assert
            resultado.Should().BeNull("porque não existe usuário com o e-mail especificado");
        }
    }
}
