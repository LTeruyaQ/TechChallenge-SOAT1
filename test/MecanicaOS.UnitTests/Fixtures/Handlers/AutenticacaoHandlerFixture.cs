using Core.Entidades;
using Core.Enumeradores;
using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.Autenticacao;
using Core.Interfaces.UseCases;
using Core.UseCases.Autenticacao.AutenticarUsuario;
using MecanicaOS.UnitTests.Fixtures.UseCases;

namespace MecanicaOS.UnitTests.Fixtures.Handlers;

public class AutenticacaoHandlerFixture : UseCasesFixtureBase
{
    public IUsuarioUseCases UsuarioUseCases { get; }
    public IClienteUseCases ClienteUseCases { get; }
    public ISegurancaGateway SegurancaGateway { get; }
    public ILogGateway<AutenticarUsuarioHandler> LogAutenticacao { get; }
    public IUnidadeDeTrabalhoGateway UnidadeDeTrabalho { get; }
    public IUsuarioLogadoServicoGateway UsuarioLogadoServico { get; }

    public AutenticacaoHandlerFixture()
    {
        UsuarioUseCases = Substitute.For<IUsuarioUseCases>();
        ClienteUseCases = Substitute.For<IClienteUseCases>();
        SegurancaGateway = Substitute.For<ISegurancaGateway>();
        LogAutenticacao = Substitute.For<ILogGateway<AutenticarUsuarioHandler>>();
        UnidadeDeTrabalho = Substitute.For<IUnidadeDeTrabalhoGateway>();
        UsuarioLogadoServico = Substitute.For<IUsuarioLogadoServicoGateway>();
    }

    public IAutenticarUsuarioHandler CriarAutenticarUsuarioHandler()
    {
        return new AutenticarUsuarioHandler(
            UsuarioUseCases,
            SegurancaGateway,
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

    public void ConfigurarMockSegurancaGatewayParaSenhaValida()
    {
        SegurancaGateway.VerificarSenha(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
    }

    public void ConfigurarMockSegurancaGatewayParaSenhaInvalida()
    {
        SegurancaGateway.VerificarSenha(Arg.Any<string>(), Arg.Any<string>()).Returns(false);
    }

    public void ConfigurarMockSegurancaGatewayParaGerarToken(string token = "token_jwt_valido")
    {
        SegurancaGateway.GerarToken(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Guid?>(), Arg.Any<IEnumerable<string>>()).Returns(token);
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
