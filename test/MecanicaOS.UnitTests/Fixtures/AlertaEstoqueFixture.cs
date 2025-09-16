using Core.DTOs.Entidades.Estoque;
using Core.Entidades;

namespace MecanicaOS.UnitTests.Fixtures;

public static class AlertaEstoqueFixture
{
    public static AlertaEstoque CriarAlertaEstoqueValido()
    {
        var estoque = EstoqueFixture.CriarEstoqueComEstoqueBaixo();

        return new AlertaEstoque
        {
            EstoqueId = estoque.Id,
            Estoque = estoque
        };
    }

    public static AlertaEstoque CriarAlertaEstoqueComEstoqueZerado()
    {
        var estoque = EstoqueFixture.CriarEstoqueComEstoqueZerado();

        return new AlertaEstoque
        {
            EstoqueId = estoque.Id,
            Estoque = estoque
        };
    }

    public static AlertaEstoque CriarAlertaEstoqueSemReferencia()
    {
        return new AlertaEstoque
        {
            EstoqueId = Guid.NewGuid()
        };
    }

    public static AlertaEstoque CriarAlertaEstoqueComIdVazio()
    {
        return new AlertaEstoque
        {
            EstoqueId = Guid.Empty
        };
    }

    public static AlertaEstoqueEntityDto CriarAlertaEstoqueEntityDtoValido()
    {
        var estoqueDto = EstoqueFixture.CriarEstoqueEntityDtoValido();

        return new AlertaEstoqueEntityDto
        {
            Id = Guid.NewGuid(),
            EstoqueId = estoqueDto.Id,
            Estoque = estoqueDto,
            Ativo = true,
            DataCadastro = DateTime.Now.AddDays(-5),
            DataAtualizacao = DateTime.Now.AddDays(-1)
        };
    }

    public static AlertaEstoqueEntityDto CriarAlertaEstoqueEntityDtoComValoresPadrao()
    {
        return new AlertaEstoqueEntityDto
        {
            EstoqueId = Guid.NewGuid(),
            Estoque = EstoqueFixture.CriarEstoqueEntityDtoComValoresPadrao()
        };
    }

    public static List<AlertaEstoque> CriarListaAlertasEstoque()
    {
        return new List<AlertaEstoque>
        {
            CriarAlertaEstoqueValido(),
            CriarAlertaEstoqueComEstoqueZerado(),
            CriarAlertaEstoqueSemReferencia()
        };
    }

    public static List<AlertaEstoqueEntityDto> CriarListaAlertaEstoqueEntityDto()
    {
        var estoque1 = EstoqueFixture.CriarEstoqueEntityDtoValido();
        var estoque2 = EstoqueFixture.CriarEstoqueEntityDtoSemDescricao();

        return new List<AlertaEstoqueEntityDto>
        {
            new AlertaEstoqueEntityDto
            {
                Id = Guid.NewGuid(),
                EstoqueId = estoque1.Id,
                Estoque = estoque1,
                Ativo = true,
                DataCadastro = DateTime.Now.AddDays(-3),
                DataAtualizacao = DateTime.Now.AddHours(-2)
            },
            new AlertaEstoqueEntityDto
            {
                Id = Guid.NewGuid(),
                EstoqueId = estoque2.Id,
                Estoque = estoque2,
                Ativo = true,
                DataCadastro = DateTime.Now.AddDays(-1),
                DataAtualizacao = DateTime.Now.AddHours(-1)
            }
        };
    }

    public static AlertaEstoque CriarAlertaParaInsumoEspecifico(string nomeInsumo)
    {
        var estoque = new Estoque
        {
            Insumo = nomeInsumo,
            Descricao = $"Descrição para {nomeInsumo}",
            Preco = 50.00m,
            QuantidadeDisponivel = 1,
            QuantidadeMinima = 10
        };

        return new AlertaEstoque
        {
            EstoqueId = estoque.Id,
            Estoque = estoque
        };
    }
}
