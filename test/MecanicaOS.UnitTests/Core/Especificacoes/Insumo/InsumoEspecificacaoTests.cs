using Core.DTOs.Repositories.Estoque;
using Core.DTOs.Repositories.OrdemServicos;
using Core.Especificacoes.Insumo;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures;
using Xunit;

namespace MecanicaOS.UnitTests.Core.Especificacoes.Insumo;

public class InsumoEspecificacaoTests
{
    private List<InsumoOSRepositoryDto> GetInsumosOSDeTeste()
    {
        var osId1 = Guid.NewGuid();
        var osId2 = Guid.NewGuid();
        var estoque1 = EstoqueFixture.CriarEstoqueRepositoryDtoValido();
        var estoque2 = EstoqueFixture.CriarEstoqueRepositoryDtoValido();
        var estoque3 = EstoqueFixture.CriarEstoqueRepositoryDtoSemDescricao();

        return new List<InsumoOSRepositoryDto>
        {
            new InsumoOSRepositoryDto
            {
                Id = Guid.NewGuid(),
                OrdemServicoId = osId1,
                EstoqueId = estoque1.Id,
                Quantidade = 2,
                Ativo = true,
                DataCadastro = DateTime.Now.AddDays(-1),
                DataAtualizacao = DateTime.Now,
                Estoque = estoque1
            },
            new InsumoOSRepositoryDto
            {
                Id = Guid.NewGuid(),
                OrdemServicoId = osId1,
                EstoqueId = estoque2.Id,
                Quantidade = 1,
                Ativo = true,
                DataCadastro = DateTime.Now.AddDays(-1),
                DataAtualizacao = DateTime.Now,
                Estoque = estoque2
            },
            new InsumoOSRepositoryDto
            {
                Id = Guid.NewGuid(),
                OrdemServicoId = osId2,
                EstoqueId = estoque3.Id,
                Quantidade = 3,
                Ativo = true,
                DataCadastro = DateTime.Now.AddDays(-2),
                DataAtualizacao = DateTime.Now,
                Estoque = estoque3
            }
        };
    }

    [Fact]
    public void ObterInsumosPorIdsEOSEspecificacao_DeveRetornarInsumosCorretos()
    {
        // Arrange
        var insumos = GetInsumosOSDeTeste();
        var osId = insumos.First().OrdemServicoId;
        var idsInsumos = insumos.Where(i => i.OrdemServicoId == osId).Select(i => i.Id).ToList();
        var especificacao = new ObterInsumosPorIdsEOSEspecificacao(osId, idsInsumos);

        // Act
        var resultado = insumos.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(2, "deve retornar apenas os insumos da OS especificada com os IDs corretos");
        resultado.Should().OnlyContain(i => i.OrdemServicoId == osId, 
            "todos os insumos devem pertencer à ordem de serviço especificada");
        resultado.Should().OnlyContain(i => idsInsumos.Contains(i.Id), 
            "todos os insumos devem ter IDs que estão na lista especificada");
    }

    [Fact]
    public void ObterInsumosPorIdsEOSEspecificacao_QuandoNenhumInsumoCorresponde_DeveRetornarListaVazia()
    {
        // Arrange
        var insumos = GetInsumosOSDeTeste();
        var osIdInexistente = Guid.NewGuid();
        var idsInexistentes = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var especificacao = new ObterInsumosPorIdsEOSEspecificacao(osIdInexistente, idsInexistentes);

        // Act
        var resultado = insumos.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().BeEmpty("não deve retornar nenhum insumo quando OS e IDs não existem");
    }

    [Fact]
    public void ObterInsumosPorIdsEOSEspecificacao_QuandoOSCorretoMasIdsIncorretos_DeveRetornarListaVazia()
    {
        // Arrange
        var insumos = GetInsumosOSDeTeste();
        var osId = insumos.First().OrdemServicoId;
        var idsIncorretos = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var especificacao = new ObterInsumosPorIdsEOSEspecificacao(osId, idsIncorretos);

        // Act
        var resultado = insumos.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().BeEmpty("não deve retornar nenhum insumo quando IDs não correspondem");
    }

    [Fact]
    public void ObterInsumosOSPorOSEspecificacao_DeveRetornarTodosInsumosDeUmaOS()
    {
        // Arrange
        var insumos = GetInsumosOSDeTeste();
        var osId = insumos.First().OrdemServicoId;
        var especificacao = new ObterInsumosOSPorOSEspecificacao(osId);

        // Act
        var resultado = insumos.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(2, "deve retornar todos os insumos da OS especificada");
        resultado.Should().OnlyContain(i => i.OrdemServicoId == osId, 
            "todos os insumos devem pertencer à ordem de serviço especificada");
    }

    [Fact]
    public void ObterInsumosOSPorOSEspecificacao_QuandoOSNaoExiste_DeveRetornarListaVazia()
    {
        // Arrange
        var insumos = GetInsumosOSDeTeste();
        var osIdInexistente = Guid.NewGuid();
        var especificacao = new ObterInsumosOSPorOSEspecificacao(osIdInexistente);

        // Act
        var resultado = insumos.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().BeEmpty("não deve retornar nenhum insumo quando OS não existe");
    }

    [Fact]
    public void ObterInsumosOSPorOSEspecificacao_DeveIncluirEstoque()
    {
        // Arrange
        var especificacao = new ObterInsumosOSPorOSEspecificacao(Guid.NewGuid());

        // Assert
        especificacao.Inclusoes.Should().NotBeEmpty("deve ter inclusões definidas");
        especificacao.Inclusoes.Should().Contain(i => i.ToString().Contains("Estoque"), 
            "deve incluir a propriedade Estoque");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void ObterInsumosPorIdsEOSEspecificacao_ComDiferentesQuantidadesDeIds_DeveFuncionar(int quantidadeIds)
    {
        // Arrange
        var insumos = GetInsumosOSDeTeste();
        var osId = insumos.First().OrdemServicoId;
        var idsInsumos = insumos.Where(i => i.OrdemServicoId == osId)
                               .Select(i => i.Id)
                               .Take(quantidadeIds)
                               .ToList();
        var especificacao = new ObterInsumosPorIdsEOSEspecificacao(osId, idsInsumos);

        // Act
        var resultado = insumos.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(Math.Min(quantidadeIds, 2), 
            "deve retornar a quantidade correta de insumos baseada nos IDs fornecidos");
    }

    [Fact]
    public void ObterInsumosPorIdsEOSEspecificacao_ComListaVaziaDeIds_DeveRetornarListaVazia()
    {
        // Arrange
        var insumos = GetInsumosOSDeTeste();
        var osId = insumos.First().OrdemServicoId;
        var idsVazios = new List<Guid>();
        var especificacao = new ObterInsumosPorIdsEOSEspecificacao(osId, idsVazios);

        // Act
        var resultado = insumos.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().BeEmpty("não deve retornar nenhum insumo quando lista de IDs está vazia");
    }
}
