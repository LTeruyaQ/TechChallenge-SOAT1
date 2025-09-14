using Core.Entidades;
using Core.DTOs.Repositories.Estoque;

namespace MecanicaOS.UnitTests.Fixtures;

public static class EstoqueFixture
{
    public static Estoque CriarEstoqueValido()
    {
        return new Estoque
        {
            Insumo = "Óleo Motor 5W30",
            Descricao = "Óleo sintético para motores a gasolina",
            Preco = 45.90m,
            QuantidadeDisponivel = 25,
            QuantidadeMinima = 5
        };
    }

    public static Estoque CriarEstoqueComEstoqueBaixo()
    {
        return new Estoque
        {
            Insumo = "Filtro de Ar",
            Descricao = "Filtro de ar original do fabricante",
            Preco = 35.00m,
            QuantidadeDisponivel = 3,
            QuantidadeMinima = 10
        };
    }

    public static Estoque CriarEstoqueComEstoqueZerado()
    {
        return new Estoque
        {
            Insumo = "Pastilha de Freio",
            Descricao = "Pastilha de freio cerâmica premium",
            Preco = 120.00m,
            QuantidadeDisponivel = 0,
            QuantidadeMinima = 4
        };
    }

    public static Estoque CriarEstoqueSemDescricao()
    {
        return new Estoque
        {
            Insumo = "Parafuso M8",
            Descricao = null,
            Preco = 2.50m,
            QuantidadeDisponivel = 100,
            QuantidadeMinima = 20
        };
    }

    public static Estoque CriarEstoqueCaroPremium()
    {
        return new Estoque
        {
            Insumo = "Kit Embreagem Completo",
            Descricao = "Kit completo de embreagem importado",
            Preco = 850.00m,
            QuantidadeDisponivel = 2,
            QuantidadeMinima = 1
        };
    }

    public static EstoqueRepositoryDto CriarEstoqueRepositoryDtoValido()
    {
        return new EstoqueRepositoryDto
        {
            Id = Guid.NewGuid(),
            Insumo = "Vela de Ignição",
            Descricao = "Vela de ignição iridium",
            Preco = 25.00m,
            QuantidadeDisponivel = 40,
            QuantidadeMinima = 8,
            Ativo = true,
            DataCadastro = DateTime.Now.AddDays(-20),
            DataAtualizacao = DateTime.Now.AddDays(-2)
        };
    }

    public static EstoqueRepositoryDto CriarEstoqueRepositoryDtoSemDescricao()
    {
        return new EstoqueRepositoryDto
        {
            Id = Guid.NewGuid(),
            Insumo = "Abraçadeira Universal",
            Descricao = null,
            Preco = 8.50m,
            QuantidadeDisponivel = 50,
            QuantidadeMinima = 15,
            Ativo = true,
            DataCadastro = DateTime.Now.AddDays(-10),
            DataAtualizacao = DateTime.Now.AddDays(-1)
        };
    }

    public static EstoqueRepositoryDto CriarEstoqueRepositoryDtoComValoresPadrao()
    {
        return new EstoqueRepositoryDto
        {
            Insumo = "Insumo Teste"
        };
    }

    public static List<Estoque> CriarListaEstoques()
    {
        return new List<Estoque>
        {
            CriarEstoqueValido(),
            CriarEstoqueComEstoqueBaixo(),
            CriarEstoqueComEstoqueZerado(),
            CriarEstoqueSemDescricao(),
            CriarEstoqueCaroPremium()
        };
    }

    public static List<EstoqueRepositoryDto> CriarListaEstoqueRepositoryDto()
    {
        return new List<EstoqueRepositoryDto>
        {
            CriarEstoqueRepositoryDtoValido(),
            CriarEstoqueRepositoryDtoSemDescricao(),
            new EstoqueRepositoryDto
            {
                Id = Guid.NewGuid(),
                Insumo = "Correia Dentada",
                Descricao = "Correia dentada para motor 1.0",
                Preco = 65.00m,
                QuantidadeDisponivel = 12,
                QuantidadeMinima = 3,
                Ativo = true,
                DataCadastro = DateTime.Now.AddDays(-15),
                DataAtualizacao = DateTime.Now.AddDays(-3)
            }
        };
    }

    public static List<Estoque> CriarEstoquesComAlerta()
    {
        return new List<Estoque>
        {
            CriarEstoqueComEstoqueBaixo(),
            CriarEstoqueComEstoqueZerado()
        };
    }

    public static Estoque CriarEstoqueParaAlerta()
    {
        return new Estoque
        {
            Insumo = "Fluido de Freio",
            Descricao = "Fluido de freio DOT 4",
            Preco = 18.00m,
            QuantidadeDisponivel = 2,
            QuantidadeMinima = 8
        };
    }
}
