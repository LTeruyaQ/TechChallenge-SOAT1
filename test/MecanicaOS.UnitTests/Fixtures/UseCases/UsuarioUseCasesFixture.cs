using Core.DTOs.UseCases.Usuario;
using Core.Entidades;
using Core.Enumeradores;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases;
using NSubstitute;
using Core.Interfaces.UseCases;

namespace MecanicaOS.UnitTests.Fixtures.UseCases;

public class UsuarioUseCasesFixture
{

    public UsuarioUseCases CriarUsuarioUseCases(
        IUsuarioGateway? mockUsuarioGateway = null,
        IClienteUseCases? mockClienteUseCases = null,
        IServicoSenha? mockServicoSenha = null,
        IUnidadeDeTrabalho? mockUdt = null)
    {
        // Se mockUsuarioGateway for explicitamente nulo, não substitua para permitir teste do construtor
        if (mockUsuarioGateway == null && mockClienteUseCases == null && 
            mockServicoSenha == null && mockUdt == null)
        {
            return new UsuarioUseCases(
                CriarMockLogServico<UsuarioUseCases>(),
                CriarMockUnidadeDeTrabalho(),
                CriarMockClienteUseCases(),
                CriarMockServicoSenha(),
                CriarMockUsuarioLogadoServico(),
                null!);
        }
        
        return new UsuarioUseCases(
            CriarMockLogServico<UsuarioUseCases>(),
            mockUdt ?? CriarMockUnidadeDeTrabalho(),
            mockClienteUseCases ?? CriarMockClienteUseCases(),
            mockServicoSenha ?? CriarMockServicoSenha(),
            CriarMockUsuarioLogadoServico(),
            mockUsuarioGateway ?? CriarMockUsuarioGateway());
    }

    public static IUsuarioGateway CriarMockUsuarioGateway() => Substitute.For<IUsuarioGateway>();
    public static IClienteUseCases CriarMockClienteUseCases() => Substitute.For<IClienteUseCases>();
    public static IServicoSenha CriarMockServicoSenha() => Substitute.For<IServicoSenha>();
    public static IUnidadeDeTrabalho CriarMockUnidadeDeTrabalho() => Substitute.For<IUnidadeDeTrabalho>();
    public static ILogServico<T> CriarMockLogServico<T>() => Substitute.For<ILogServico<T>>();
    public static IUsuarioLogadoServico CriarMockUsuarioLogadoServico() => Substitute.For<IUsuarioLogadoServico>();

    public static Usuario CriarUsuarioAdministradorValido()
    {
        return new Usuario
        {
            Id = Guid.NewGuid(),
            Email = "admin@mecanicaos.com",
            Senha = "senhaHasheada123",
            TipoUsuario = TipoUsuario.Admin,
            RecebeAlertaEstoque = true,
            DataUltimoAcesso = DateTime.UtcNow.AddHours(-2),
            DataCadastro = DateTime.UtcNow.AddDays(-30),
            DataAtualizacao = DateTime.UtcNow.AddDays(-1),
            Ativo = true
        };
    }

    public static Usuario CriarUsuarioFuncionarioValido()
    {
        return new Usuario
        {
            Id = Guid.NewGuid(),
            Email = "funcionario@mecanicaos.com",
            Senha = "senhaHasheada456",
            TipoUsuario = TipoUsuario.Cliente,
            RecebeAlertaEstoque = false,
            DataUltimoAcesso = DateTime.UtcNow.AddHours(-1),
            DataCadastro = DateTime.UtcNow.AddDays(-15),
            DataAtualizacao = DateTime.UtcNow.AddDays(-1),
            Ativo = true
        };
    }

    public static Usuario CriarUsuarioClienteValido()
    {
        return new Usuario
        {
            Id = Guid.NewGuid(),
            Email = "cliente@email.com",
            Senha = "senhaHasheada789",
            TipoUsuario = TipoUsuario.Cliente,
            ClienteId = Guid.NewGuid(),
            RecebeAlertaEstoque = false,
            DataUltimoAcesso = DateTime.UtcNow.AddMinutes(-30),
            DataCadastro = DateTime.UtcNow.AddDays(-7),
            DataAtualizacao = DateTime.UtcNow.AddDays(-1),
            Ativo = true
        };
    }

    public static Usuario CriarUsuarioInativo()
    {
        return new Usuario
        {
            Id = Guid.NewGuid(),
            Email = "inativo@mecanicaos.com",
            Senha = "senhaHasheada000",
            TipoUsuario = TipoUsuario.Cliente,
            RecebeAlertaEstoque = false,
            DataUltimoAcesso = DateTime.UtcNow.AddDays(-30),
            DataCadastro = DateTime.UtcNow.AddDays(-60),
            DataAtualizacao = DateTime.UtcNow.AddDays(-30),
            Ativo = false
        };
    }

    public static CadastrarUsuarioUseCaseDto CriarCadastrarUsuarioAdministradorDto()
    {
        return new CadastrarUsuarioUseCaseDto
        {
            Email = "novo.admin@mecanicaos.com",
            Senha = "MinhaSenh@123",
            TipoUsuario = TipoUsuario.Admin,
            RecebeAlertaEstoque = true,
            Documento = null
        };
    }

    public static CadastrarUsuarioUseCaseDto CriarCadastrarUsuarioFuncionarioDto()
    {
        return new CadastrarUsuarioUseCaseDto
        {
            Email = "novo.funcionario@mecanicaos.com",
            Senha = "MinhaSenh@456",
            TipoUsuario = TipoUsuario.Cliente,
            RecebeAlertaEstoque = false,
            Documento = null
        };
    }

    public static CadastrarUsuarioUseCaseDto CriarCadastrarUsuarioClienteDto()
    {
        return new CadastrarUsuarioUseCaseDto
        {
            Email = "novo.cliente@email.com",
            Senha = "MinhaSenh@789",
            TipoUsuario = TipoUsuario.Cliente,
            RecebeAlertaEstoque = false,
            Documento = "12345678901"
        };
    }

    public static AtualizarUsuarioUseCaseDto CriarAtualizarUsuarioUseCaseDtoValido()
    {
        return new AtualizarUsuarioUseCaseDto
        {
            Email = "usuario.atualizado@mecanicaos.com",
            Senha = "NovaSenha@123",
            TipoUsuario = TipoUsuario.Cliente,
            RecebeAlertaEstoque = true,
            DataUltimoAcesso = DateTime.UtcNow
        };
    }

    public static AtualizarUsuarioUseCaseDto CriarAtualizarUsuarioSemSenhaDto()
    {
        return new AtualizarUsuarioUseCaseDto
        {
            Email = "usuario.sem.senha@mecanicaos.com",
            Senha = null,
            TipoUsuario = TipoUsuario.Admin,
            RecebeAlertaEstoque = false,
            DataUltimoAcesso = DateTime.UtcNow.AddHours(-1)
        };
    }

    public static Cliente CriarClienteParaUsuario()
    {
        return new Cliente
        {
            Id = Guid.NewGuid(),
            Nome = "Cliente Teste",
            Documento = "12345678901",
            TipoCliente = TipoCliente.PessoaFisica,
            DataNascimento = "1990-01-01",
            Contato = new Contato
            {
                Email = "cliente@email.com",
                Telefone = "(11) 99999-9999"
            },
            Endereco = new Endereco
            {
                Rua = "Rua Teste",
                Numero = "123",
                Bairro = "Centro",
                Cidade = "São Paulo",
                CEP = "01000-000"
            },
            DataCadastro = DateTime.UtcNow.AddDays(-10),
            DataAtualizacao = DateTime.UtcNow.AddDays(-1),
            Ativo = true
        };
    }

    public static void ConfigurarMockUsuarioGatewayParaEmailEncontrado(IUsuarioGateway mock, Usuario usuario)
    {
        mock.ObterPorEmailAsync(usuario.Email).Returns(usuario);
    }


    public static List<Usuario> CriarListaUsuariosVariados()
    {
        return new List<Usuario>
        {
            CriarUsuarioAdministradorValido(),
            CriarUsuarioFuncionarioValido(),
            CriarUsuarioClienteValido(),
            CriarUsuarioInativo()
        };
    }

    public void ConfigurarMockUsuarioGatewayParaCadastro(
        IUsuarioGateway mockGateway,
        Usuario? usuarioEsperado = null)
    {
        var usuario = usuarioEsperado ?? CriarUsuarioAdministradorValido();
        mockGateway.CadastrarAsync(Arg.Any<Usuario>()).Returns(usuario);
    }

    public void ConfigurarMockUsuarioGatewayParaAtualizacao(
        IUsuarioGateway mockGateway,
        Usuario usuarioExistente)
    {
        mockGateway.ObterPorIdAsync(usuarioExistente.Id).Returns(usuarioExistente);
        mockGateway.EditarAsync(Arg.Any<Usuario>()).Returns(Task.CompletedTask);
    }

    public void ConfigurarMockUsuarioGatewayParaUsuarioNaoEncontrado(
        IUsuarioGateway mockGateway,
        Guid usuarioId)
    {
        mockGateway.ObterPorIdAsync(usuarioId).Returns((Usuario?)null);
    }

    public void ConfigurarMockUsuarioGatewayParaEmailJaCadastrado(
        IUsuarioGateway mockGateway,
        string email)
    {
        var usuarioExistente = CriarUsuarioAdministradorValido();
        usuarioExistente.Email = email;
        mockGateway.ObterPorEmailAsync(email).Returns(usuarioExistente);
    }

    public void ConfigurarMockUsuarioGatewayParaEmailNaoEncontrado(
        IUsuarioGateway mockGateway,
        string email)
    {
        mockGateway.ObterPorEmailAsync(email).Returns((Usuario?)null);
    }

    public void ConfigurarMockUsuarioGatewayParaListagem(
        IUsuarioGateway mockGateway,
        List<Usuario> usuarios)
    {
        mockGateway.ObterTodosAsync().Returns(usuarios);
    }

    public void ConfigurarMockUsuarioGatewayParaDelecao(
        IUsuarioGateway mockGateway,
        Usuario usuario)
    {
        mockGateway.ObterPorIdAsync(usuario.Id).Returns(Task.FromResult(usuario));
        mockGateway.DeletarAsync(usuario).Returns(Task.FromResult(true));
    }

    public void ConfigurarMockServicoSenha(
        IServicoSenha mockServicoSenha,
        string senhaOriginal = "MinhaSenh@123",
        string senhaCriptografada = "senhaHasheada123")
    {
        mockServicoSenha.CriptografarSenha(senhaOriginal).Returns(senhaCriptografada);
    }

    public void ConfigurarMockClienteUseCasesParaDocumento(
        IClienteUseCases mockClienteUseCases,
        string documento,
        Cliente? cliente = null)
    {
        var clienteRetorno = cliente ?? CriarClienteParaUsuario();
        clienteRetorno.Documento = documento;
        mockClienteUseCases.ObterPorDocumentoUseCaseAsync(documento).Returns(clienteRetorno);
    }
}
