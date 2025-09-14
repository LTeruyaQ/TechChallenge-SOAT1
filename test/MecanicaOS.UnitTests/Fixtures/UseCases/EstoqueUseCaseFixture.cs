using Core.DTOs.UseCases.Estoque;

namespace MecanicaOS.UnitTests.Fixtures.UseCases;

public static class EstoqueUseCaseFixture
{
    public static CadastrarEstoqueUseCaseDto CriarCadastrarEstoqueUseCaseDtoValido()
    {
        return new CadastrarEstoqueUseCaseDto
        {
            Insumo = "Óleo Motor 5W30",
            Descricao = "Óleo sintético para motor 5W30 - 1 litro",
            Preco = 45.90m,
            QuantidadeDisponivel = 50,
            QuantidadeMinima = 10
        };
    }

    public static CadastrarEstoqueUseCaseDto CriarCadastrarEstoqueUseCaseDtoSemDescricao()
    {
        return new CadastrarEstoqueUseCaseDto
        {
            Insumo = "Filtro de Ar",
            Descricao = null,
            Preco = 25.00m,
            QuantidadeDisponivel = 30,
            QuantidadeMinima = 5
        };
    }

    public static CadastrarEstoqueUseCaseDto CriarCadastrarEstoqueUseCaseDtoEstoqueBaixo()
    {
        return new CadastrarEstoqueUseCaseDto
        {
            Insumo = "Pastilha de Freio",
            Descricao = "Pastilha de freio dianteira - par",
            Preco = 120.00m,
            QuantidadeDisponivel = 3,
            QuantidadeMinima = 5
        };
    }

    public static CadastrarEstoqueUseCaseDto CriarCadastrarEstoqueUseCaseDtoGratuito()
    {
        return new CadastrarEstoqueUseCaseDto
        {
            Insumo = "Amostra Grátis",
            Descricao = "Produto promocional sem custo",
            Preco = 0.00m,
            QuantidadeDisponivel = 100,
            QuantidadeMinima = 0
        };
    }

    public static AtualizarEstoqueUseCaseDto CriarAtualizarEstoqueUseCaseDtoValido()
    {
        return new AtualizarEstoqueUseCaseDto
        {
            Insumo = "Óleo Motor 10W40 Atualizado",
            Descricao = "Óleo mineral para motor 10W40 - 1 litro",
            Preco = 35.50m,
            QuantidadeDisponivel = 75,
            QuantidadeMinima = 15
        };
    }

    public static AtualizarEstoqueUseCaseDto CriarAtualizarEstoqueUseCaseDtoComCamposNulos()
    {
        return new AtualizarEstoqueUseCaseDto
        {
            Insumo = null,
            Descricao = null,
            Preco = null,
            QuantidadeDisponivel = null,
            QuantidadeMinima = null
        };
    }

    public static AtualizarEstoqueUseCaseDto CriarAtualizarEstoqueUseCaseDtoApenasPreco()
    {
        return new AtualizarEstoqueUseCaseDto
        {
            Preco = 55.00m
        };
    }

    public static AtualizarEstoqueUseCaseDto CriarAtualizarEstoqueUseCaseDtoApenasQuantidade()
    {
        return new AtualizarEstoqueUseCaseDto
        {
            QuantidadeDisponivel = 200
        };
    }

    public static List<CadastrarEstoqueUseCaseDto> CriarListaCadastrarEstoqueUseCaseDto()
    {
        return new List<CadastrarEstoqueUseCaseDto>
        {
            CriarCadastrarEstoqueUseCaseDtoValido(),
            CriarCadastrarEstoqueUseCaseDtoSemDescricao(),
            CriarCadastrarEstoqueUseCaseDtoEstoqueBaixo(),
            CriarCadastrarEstoqueUseCaseDtoGratuito()
        };
    }

    public static List<AtualizarEstoqueUseCaseDto> CriarListaAtualizarEstoqueUseCaseDto()
    {
        return new List<AtualizarEstoqueUseCaseDto>
        {
            CriarAtualizarEstoqueUseCaseDtoValido(),
            CriarAtualizarEstoqueUseCaseDtoComCamposNulos(),
            CriarAtualizarEstoqueUseCaseDtoApenasPreco(),
            CriarAtualizarEstoqueUseCaseDtoApenasQuantidade()
        };
    }
}
