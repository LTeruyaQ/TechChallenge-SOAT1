using Core.DTOs.UseCases.Estoque;
using Core.Entidades;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;

namespace MecanicaOS.UnitTests.Fixtures.UseCases;

public class EstoqueUseCasesFixture : UseCasesFixtureBase
{
    public IEstoqueUseCases CriarEstoqueUseCases(IEstoqueGateway? mockEstoqueGateway = null)
    {
        // Para os testes, vamos criar um mock da interface IEstoqueUseCases
        // Os testes devem focar no comportamento da interface, não na implementação interna
        return Substitute.For<IEstoqueUseCases>();
    }

    public IEstoqueUseCases CriarEstoqueUseCases(
        IEstoqueGateway? mockEstoqueGateway,
        ILogServico<IEstoqueUseCases>? mockLogServico,
        IUnidadeDeTrabalho? mockUdt = null)
    {
        // Para os testes, vamos criar um mock da interface IEstoqueUseCases
        return Substitute.For<IEstoqueUseCases>();
    }

    public static Estoque CriarEstoqueValido()
    {
        return new Estoque
        {
            Id = Guid.NewGuid(),
            Insumo = "Óleo de Motor",
            Descricao = "Óleo sintético 5W30",
            QuantidadeDisponivel = 10,
            QuantidadeMinima = 5,
            Preco = 25.99m,
            Ativo = true,
            DataCadastro = DateTime.UtcNow.AddDays(-30),
            DataAtualizacao = DateTime.UtcNow.AddDays(-5)
        };
    }

    public static Estoque CriarEstoqueCritico()
    {
        return new Estoque
        {
            Id = Guid.NewGuid(),
            Insumo = "Filtro de Ar",
            Descricao = "Filtro de ar para motor",
            QuantidadeDisponivel = 8,
            QuantidadeMinima = 10,
            Preco = 45.00m,
            Ativo = true,
            DataCadastro = DateTime.UtcNow.AddDays(-15),
            DataAtualizacao = DateTime.UtcNow.AddDays(-2)
        };
    }

    public static Estoque CriarEstoqueZerado()
    {
        return new Estoque
        {
            Id = Guid.NewGuid(),
            Insumo = "Pastilha de Freio",
            Descricao = "Pastilha de freio dianteira",
            QuantidadeDisponivel = 0,
            QuantidadeMinima = 6,
            Preco = 89.90m,
            Ativo = true,
            DataCadastro = DateTime.UtcNow.AddDays(-10),
            DataAtualizacao = DateTime.UtcNow.AddHours(-6)
        };
    }

    public static Estoque CriarEstoqueInativo()
    {
        return new Estoque
        {
            Id = Guid.NewGuid(),
            Insumo = "Produto Descontinuado",
            Descricao = "Produto que não é mais vendido",
            QuantidadeDisponivel = 15,
            QuantidadeMinima = 12,
            Preco = 18.90m,
            Ativo = false,
            DataCadastro = DateTime.UtcNow.AddDays(-180),
            DataAtualizacao = DateTime.UtcNow.AddDays(-90)
        };
    }

    public static CadastrarEstoqueUseCaseDto CriarCadastrarEstoqueUseCaseDtoValido()
    {
        return new CadastrarEstoqueUseCaseDto
        {
            Insumo = "Vela de Ignição",
            Descricao = "Vela de ignição NGK padrão",
            QuantidadeDisponivel = 25,
            QuantidadeMinima = 12,
            Preco = 18.90m
        };
    }

    public static CadastrarEstoqueUseCaseDto CriarCadastrarEstoqueUseCaseDtoComQuantidadeZero()
    {
        return new CadastrarEstoqueUseCaseDto
        {
            Insumo = "Amortecedor Traseiro",
            Descricao = "Amortecedor traseiro para veículos compactos",
            QuantidadeDisponivel = 0,
            QuantidadeMinima = 4,
            Preco = 180.00m
        };
    }

    public static AtualizarEstoqueUseCaseDto CriarAtualizarEstoqueUseCaseDtoValido()
    {
        return new AtualizarEstoqueUseCaseDto
        {
            Insumo = "Óleo Motor 5W30 Atualizado",
            Descricao = "Óleo lubrificante sintético premium",
            QuantidadeDisponivel = 75,
            QuantidadeMinima = 15,
            Preco = 52.90m
        };
    }

    public static AtualizarEstoqueUseCaseDto CriarAtualizarEstoqueUseCaseDtoComQuantidadeCritica()
    {
        return new AtualizarEstoqueUseCaseDto
        {
            Insumo = "Filtro de Combustível",
            Descricao = "Filtro de combustível universal",
            QuantidadeDisponivel = 3,
            QuantidadeMinima = 8,
            Preco = 35.50m
        };
    }

    public static List<Estoque> CriarListaEstoquesVariados()
    {
        return new List<Estoque>
        {
            CriarEstoqueValido(),
            CriarEstoqueCritico(),
            CriarEstoqueZerado(),
            new Estoque
            {
                Id = Guid.NewGuid(),
                Insumo = "Filtro de Ar",
                Descricao = "Filtro de ar para motor",
                QuantidadeDisponivel = 8,
                QuantidadeMinima = 10,
                Preco = 45.00m,
                Ativo = true,
                DataCadastro = DateTime.UtcNow.AddDays(-15),
                DataAtualizacao = DateTime.UtcNow.AddDays(-2)
            }
        };
    }

    public void ConfigurarMockEstoqueGatewayParaCadastro(
        IEstoqueGateway mockEstoqueGateway,
        Estoque? estoqueRetorno = null)
    {
        mockEstoqueGateway.CadastrarAsync(Arg.Any<Estoque>()).Returns(Task.FromResult(estoqueRetorno));
        mockEstoqueGateway.EditarAsync(Arg.Any<Estoque>()).Returns(Task.FromResult(estoqueRetorno));
        mockEstoqueGateway.ObterPorIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult(estoqueRetorno));
    }

    public void ConfigurarMockEstoqueGatewayParaAtualizacao(
        IEstoqueGateway mockEstoqueGateway,
        Estoque estoqueExistente)
    {
        mockEstoqueGateway.ObterPorIdAsync(estoqueExistente.Id).Returns(estoqueExistente);
        mockEstoqueGateway.EditarAsync(Arg.Any<Estoque>()).Returns(Task.CompletedTask);
    }

    public void ConfigurarMockEstoqueGatewayParaEstoqueNaoEncontrado(
        IEstoqueGateway mockEstoqueGateway,
        Guid estoqueId)
    {
        mockEstoqueGateway.ObterPorIdAsync(estoqueId).Returns((Estoque?)null);
    }

    public void ConfigurarMockEstoqueGatewayParaNomeJaCadastrado(
        IEstoqueGateway mockEstoqueGateway,
        string nome)
    {
        var estoqueExistente = CriarEstoqueValido();
        estoqueExistente.Insumo = nome;
        mockEstoqueGateway.ObterTodosAsync().Returns(new List<Estoque> { estoqueExistente });
    }

    public void ConfigurarMockEstoqueGatewayParaListagem(
        IEstoqueGateway mockEstoqueGateway,
        List<Estoque>? estoques = null)
    {
        estoques ??= CriarListaEstoquesVariados();
        mockEstoqueGateway.ObterTodosAsync().Returns(estoques);
        mockEstoqueGateway.ObterEstoqueCriticoAsync().Returns(estoques.Where(e => e.QuantidadeDisponivel <= e.QuantidadeMinima).ToList());
    }

    public void ConfigurarMockEstoqueGatewayParaMovimentacao(
        IEstoqueGateway mockEstoqueGateway,
        Estoque estoque,
        int novaQuantidade)
    {
        mockEstoqueGateway.ObterPorIdAsync(estoque.Id).Returns(estoque);
        mockEstoqueGateway.EditarAsync(estoque).Returns(Task.CompletedTask);
    }
}
