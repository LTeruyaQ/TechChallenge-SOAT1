using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;

namespace MecanicaOS.UnitTests.Fixtures.UseCases;

public abstract class UseCasesFixtureBase
{
    // Mocks de Serviços Comuns
    public ILogServico<T> CriarMockLogServico<T>() => Substitute.For<ILogServico<T>>();
    public IUnidadeDeTrabalho CriarMockUnidadeDeTrabalho() => Substitute.For<IUnidadeDeTrabalho>();
    public IUsuarioLogadoServico CriarMockUsuarioLogadoServico() => Substitute.For<IUsuarioLogadoServico>();
    public IServicoSenha CriarMockServicoSenha() => Substitute.For<IServicoSenha>();
    public IServicoJwt CriarMockServicoJwt() => Substitute.For<IServicoJwt>();

    // Mocks de Gateways
    public IClienteGateway CriarMockClienteGateway() => Substitute.For<IClienteGateway>();
    public IUsuarioGateway CriarMockUsuarioGateway() => Substitute.For<IUsuarioGateway>();
    public IServicoGateway CriarMockServicoGateway() => Substitute.For<IServicoGateway>();
    public IEstoqueGateway CriarMockEstoqueGateway() => Substitute.For<IEstoqueGateway>();
    public IOrdemServicoGateway CriarMockOrdemServicoGateway() => Substitute.For<IOrdemServicoGateway>();
    public IVeiculoGateway CriarMockVeiculoGateway() => Substitute.For<IVeiculoGateway>();
    public IEnderecoGateway CriarMockEnderecoGateway() => Substitute.For<IEnderecoGateway>();
    public IContatoGateway CriarMockContatoGateway() => Substitute.For<IContatoGateway>();
    public IEventosGateway CriarMockEventosGateway() => Substitute.For<IEventosGateway>();

    // Mocks de UseCases
    public IUsuarioUseCases CriarMockUsuarioUseCases() => Substitute.For<IUsuarioUseCases>();
    public IClienteUseCases CriarMockClienteUseCases() => Substitute.For<IClienteUseCases>();
    public IServicoUseCases CriarMockServicoUseCases() => Substitute.For<IServicoUseCases>();
    public IEstoqueUseCases CriarMockEstoqueUseCases() => Substitute.For<IEstoqueUseCases>();

    protected void ConfigurarMocksBasicos(
        IUnidadeDeTrabalho mockUdt,
        IUsuarioLogadoServico mockUsuarioLogado)
    {
        mockUdt.Commit().Returns(true);
        mockUsuarioLogado.ObterUsuarioLogado().Returns(UsuarioFixture.CriarUsuarioValido());
    }
}
