using Core.Entidades;
using Core.DTOs.Repositories.OrdemServicos;
using Core.DTOs.Repositories.Estoque;

namespace MecanicaOS.UnitTests.Fixtures;

public static class InsumoOSFixture
{
    public static InsumoOS CriarInsumoOSValido()
    {
        var ordemServico = OrdemServicoFixture.CriarOrdemServicoValida();
        var estoque = EstoqueFixture.CriarEstoqueValido();
        
        return new InsumoOS
        {
            OrdemServicoId = ordemServico.Id,
            OrdemServico = ordemServico,
            EstoqueId = estoque.Id,
            Estoque = estoque,
            Quantidade = 2
        };
    }

    public static InsumoOS CriarInsumoOSComQuantidadeAlta()
    {
        var ordemServico = OrdemServicoFixture.CriarOrdemServicoValida();
        var estoque = EstoqueFixture.CriarEstoqueValido();
        
        return new InsumoOS
        {
            OrdemServicoId = ordemServico.Id,
            OrdemServico = ordemServico,
            EstoqueId = estoque.Id,
            Estoque = estoque,
            Quantidade = 10
        };
    }

    public static InsumoOS CriarInsumoOSQuantidadeUnitaria()
    {
        var ordemServico = OrdemServicoFixture.CriarOrdemServicoValida();
        var estoque = EstoqueFixture.CriarEstoqueCaroPremium();
        
        return new InsumoOS
        {
            OrdemServicoId = ordemServico.Id,
            OrdemServico = ordemServico,
            EstoqueId = estoque.Id,
            Estoque = estoque,
            Quantidade = 1
        };
    }

    public static InsumoOS CriarInsumoOSSemReferencias()
    {
        return new InsumoOS
        {
            OrdemServicoId = Guid.NewGuid(),
            EstoqueId = Guid.NewGuid(),
            Quantidade = 3
        };
    }

    public static InsumoOSRepositoryDto CriarInsumoOSRepositoryDtoValido()
    {
        var ordemServicoDto = OrdemServicoFixture.CriarOrdemServicoRepositoryDtoValido();
        var estoqueDto = EstoqueFixture.CriarEstoqueRepositoryDtoValido();
        
        return new InsumoOSRepositoryDto
        {
            Id = Guid.NewGuid(),
            OrdemServicoId = ordemServicoDto.Id,
            OrdemServico = ordemServicoDto,
            EstoqueId = estoqueDto.Id,
            Estoque = estoqueDto,
            Quantidade = 4,
            Ativo = true,
            DataCadastro = DateTime.Now.AddDays(-7),
            DataAtualizacao = DateTime.Now.AddDays(-1)
        };
    }

    public static InsumoOSRepositoryDto CriarInsumoOSRepositoryDtoComValoresPadrao()
    {
        return new InsumoOSRepositoryDto
        {
            OrdemServicoId = Guid.NewGuid(),
            OrdemServico = OrdemServicoFixture.CriarOrdemServicoRepositoryDtoComValoresPadrao(),
            EstoqueId = Guid.NewGuid(),
            Estoque = EstoqueFixture.CriarEstoqueRepositoryDtoComValoresPadrao(),
            Quantidade = 1
        };
    }

    public static List<InsumoOS> CriarListaInsumosOS()
    {
        return new List<InsumoOS>
        {
            CriarInsumoOSValido(),
            CriarInsumoOSComQuantidadeAlta(),
            CriarInsumoOSQuantidadeUnitaria()
        };
    }

    public static List<InsumoOSRepositoryDto> CriarListaInsumoOSRepositoryDto()
    {
        var ordemServico1 = OrdemServicoFixture.CriarOrdemServicoRepositoryDtoValido();
        var ordemServico2 = OrdemServicoFixture.CriarOrdemServicoRepositoryDtoComValoresPadrao();
        var estoque1 = EstoqueFixture.CriarEstoqueRepositoryDtoValido();
        var estoque2 = EstoqueFixture.CriarEstoqueRepositoryDtoSemDescricao();
        
        return new List<InsumoOSRepositoryDto>
        {
            new InsumoOSRepositoryDto
            {
                Id = Guid.NewGuid(),
                OrdemServicoId = ordemServico1.Id,
                OrdemServico = ordemServico1,
                EstoqueId = estoque1.Id,
                Estoque = estoque1,
                Quantidade = 2,
                Ativo = true,
                DataCadastro = DateTime.Now.AddDays(-5),
                DataAtualizacao = DateTime.Now.AddDays(-2)
            },
            new InsumoOSRepositoryDto
            {
                Id = Guid.NewGuid(),
                OrdemServicoId = ordemServico2.Id,
                OrdemServico = ordemServico2,
                EstoqueId = estoque2.Id,
                Estoque = estoque2,
                Quantidade = 1,
                Ativo = true,
                DataCadastro = DateTime.Now.AddDays(-3),
                DataAtualizacao = DateTime.Now.AddHours(-6)
            }
        };
    }

    public static InsumoOS CriarInsumoParaInsumoEspecifico(string nomeInsumo, int quantidade)
    {
        var ordemServico = OrdemServicoFixture.CriarOrdemServicoValida();
        var estoque = new Estoque
        {
            Insumo = nomeInsumo,
            Descricao = $"Descrição para {nomeInsumo}",
            Preco = 25.00m,
            QuantidadeDisponivel = 20,
            QuantidadeMinima = 5
        };
        
        return new InsumoOS
        {
            OrdemServicoId = ordemServico.Id,
            OrdemServico = ordemServico,
            EstoqueId = estoque.Id,
            Estoque = estoque,
            Quantidade = quantidade
        };
    }

    public static List<InsumoOS> CriarInsumosParaOrdemServico(Guid ordemServicoId)
    {
        var estoque1 = EstoqueFixture.CriarEstoqueValido();
        var estoque2 = EstoqueFixture.CriarEstoqueComEstoqueBaixo();
        
        return new List<InsumoOS>
        {
            new InsumoOS
            {
                OrdemServicoId = ordemServicoId,
                EstoqueId = estoque1.Id,
                Estoque = estoque1,
                Quantidade = 1
            },
            new InsumoOS
            {
                OrdemServicoId = ordemServicoId,
                EstoqueId = estoque2.Id,
                Estoque = estoque2,
                Quantidade = 2
            }
        };
    }
}
