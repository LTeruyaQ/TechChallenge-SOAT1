using Core.DTOs.UseCases.Autenticacao;
using Core.Entidades;
using Core.Enumeradores;
using Core.Interfaces.Handlers.Autenticacao;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;
using Core.UseCases.Autenticacao.AutenticarUsuario;
using MecanicaOS.UnitTests.Fixtures.UseCases;

namespace MecanicaOS.UnitTests.Fixtures.Handlers;

public class AutenticacaoHandlerFixture : UseCasesFixtureBase
{
    public IUsuarioUseCases UsuarioUseCases { get; }
    public IClienteUseCases ClienteUseCases { get; }
    public IServicoSenha ServicoSenha { get; }
    public IServicoJwt ServicoJwt { get; }
    public ILogServico<AutenticarUsuarioHandler> LogAutenticacao { get; }
    public IUnidadeDeTrabalho UnidadeDeTrabalho { get; }
    public IUsuarioLogadoServico UsuarioLogadoServico { get; }

    public AutenticacaoHandlerFixture()
    {
        UsuarioUseCases = Substitute.For<IUsuarioUseCases>();
        ClienteUseCases = Substitute.For<IClienteUseCases>();
        ServicoSenha = Substitute.For<IServicoSenha>();
        ServicoJwt = Substitute.For<IServicoJwt>();
        LogAutenticacao = Substitute.For<ILogServico<AutenticarUsuarioHandler>>();
        UnidadeDeTrabalho = Substitute.For<IUnidadeDeTrabalho>();
        UsuarioLogadoServico = Substitute.For<IUsuarioLogadoServico>();
    }

    public IAutenticarUsuarioHandler CriarAutenticarUsuarioHandler()
    {
        return new AutenticarUsuarioHandler(
            UsuarioUseCases,
            ServicoSenha,
            ServicoJwt,
            LogAutenticacao,
            ClienteUseCases,
            UnidadeDeTrabalho,
            UsuarioLogadoServico);
    }

    public void ConfigurarMockUsuarioUseCasesParaAutenticacaoValida(Usuario? usuario = null)
    {
        usuario ??= AutenticacaoUseCasesFixture.CriarUsuarioAtivoValido();
        UsuarioUseCases.ObterPorEmailUseCaseAsync(Arg.Any<string>()).Returns(usuario);
    }

    public void ConfigurarMockUsuarioUseCasesParaEmailInexistente()
    {
        UsuarioUseCases.ObterPorEmailUseCaseAsync(Arg.Any<string>()).Returns((Usuario?)null);
    }

    public void ConfigurarMockServicoSenhaParaSenhaValida()
    {
        ServicoSenha.VerificarSenha(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
    }

    public void ConfigurarMockServicoSenhaParaSenhaInvalida()
    {
        ServicoSenha.VerificarSenha(Arg.Any<string>(), Arg.Any<string>()).Returns(false);
    }

    public void ConfigurarMockServicoJwt(string token = "token_jwt_valido")
    {
        ServicoJwt.GerarToken(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IEnumerable<string>>()).Returns(token);
    }

    public void ConfigurarMockClienteUseCasesParaClienteValido(Guid clienteId, string nomeCliente = "Cliente Teste")
    {
        var cliente = new Cliente
        {
            Id = clienteId,
            Nome = nomeCliente,
            Ativo = true,
            DataCadastro = DateTime.UtcNow.AddDays(-10),
            TipoCliente = TipoCliente.PessoaFisica
        };

        ClienteUseCases.ObterPorIdUseCaseAsync(clienteId).Returns(cliente);
    }
}
