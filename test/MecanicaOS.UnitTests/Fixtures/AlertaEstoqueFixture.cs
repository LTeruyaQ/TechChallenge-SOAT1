using Core.Entidades;
using Core.DTOs.Repositories.Estoque;

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

    public static AlertaEstoqueRepositoryDto CriarAlertaEstoqueRepositoryDtoValido()
    {
        var estoqueDto = EstoqueFixture.CriarEstoqueRepositoryDtoValido();
        
        return new AlertaEstoqueRepositoryDto
        {
            Id = Guid.NewGuid(),
            EstoqueId = estoqueDto.Id,
            Estoque = estoqueDto,
            Ativo = true,
            DataCadastro = DateTime.Now.AddDays(-5),
            DataAtualizacao = DateTime.Now.AddDays(-1)
        };
    }

    public static AlertaEstoqueRepositoryDto CriarAlertaEstoqueRepositoryDtoComValoresPadrao()
    {
        return new AlertaEstoqueRepositoryDto
        {
            EstoqueId = Guid.NewGuid(),
            Estoque = EstoqueFixture.CriarEstoqueRepositoryDtoComValoresPadrao()
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

    public static List<AlertaEstoqueRepositoryDto> CriarListaAlertaEstoqueRepositoryDto()
    {
        var estoque1 = EstoqueFixture.CriarEstoqueRepositoryDtoValido();
        var estoque2 = EstoqueFixture.CriarEstoqueRepositoryDtoSemDescricao();
        
        return new List<AlertaEstoqueRepositoryDto>
        {
            new AlertaEstoqueRepositoryDto
            {
                Id = Guid.NewGuid(),
                EstoqueId = estoque1.Id,
                Estoque = estoque1,
                Ativo = true,
                DataCadastro = DateTime.Now.AddDays(-3),
                DataAtualizacao = DateTime.Now.AddHours(-2)
            },
            new AlertaEstoqueRepositoryDto
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
