using Core.DTOs.Entidades.OrdemServicos;
using Core.Enumeradores;
using Core.Especificacoes.OrdemServico;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures;
using Xunit;

namespace MecanicaOS.UnitTests.Core.Especificacoes.OrdemServico;

public class OrdemServicoEspecificacaoTests
{
    private List<OrdemServicoEntityDto> GetOrdensServicoDeTeste()
    {
        var os1 = OrdemServicoFixture.CriarOrdemServicoEntityDtoValido();
        os1.Status = StatusOrdemServico.Recebida;
        os1.Id = Guid.NewGuid();
        os1.DataEnvioOrcamento = null;

        var os2 = OrdemServicoFixture.CriarOrdemServicoEntityDtoValido();
        os2.Status = StatusOrdemServico.EmExecucao;
        os2.Id = Guid.NewGuid();
        os2.DataEnvioOrcamento = null;

        var os3 = OrdemServicoFixture.CriarOrdemServicoEntityDtoValido();
        os3.Status = StatusOrdemServico.Finalizada;
        os3.Id = Guid.NewGuid();
        os3.DataEnvioOrcamento = DateTime.UtcNow.AddDays(-10);

        var os4 = OrdemServicoFixture.CriarOrdemServicoEntityDtoValido();
        os4.Status = StatusOrdemServico.AguardandoAprovação;
        os4.Id = Guid.NewGuid();
        os4.DataEnvioOrcamento = DateTime.UtcNow.AddDays(-2); // Not expired

        var os5 = OrdemServicoFixture.CriarOrdemServicoEntityDtoValido();
        os5.Status = StatusOrdemServico.Cancelada;
        os5.Id = Guid.NewGuid();
        os5.DataEnvioOrcamento = null;

        return new List<OrdemServicoEntityDto> { os1, os2, os3, os4, os5 };
    }

    [Fact]
    public void ObterOrdemServicoPorStatusEspecificacao_DeveRetornarOrdensComStatusCorreto()
    {
        // Arrange
        var ordensServico = GetOrdensServicoDeTeste();
        var status = StatusOrdemServico.EmExecucao;
        var especificacao = new ObterOrdemServicoPorStatusEspecificacao(status);

        // Act
        var resultado = ordensServico.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(1, "deve retornar apenas as ordens de serviço com o status especificado");
        resultado.Should().OnlyContain(os => os.Status == status, 
            "todas as ordens devem ter o status especificado");
    }

    [Theory]
    [InlineData(StatusOrdemServico.Recebida)]
    [InlineData(StatusOrdemServico.EmExecucao)]
    [InlineData(StatusOrdemServico.Finalizada)]
    [InlineData(StatusOrdemServico.AguardandoAprovação)]
    [InlineData(StatusOrdemServico.Cancelada)]
    public void ObterOrdemServicoPorStatusEspecificacao_ComDiferentesStatus_DeveFiltrarCorretamente(StatusOrdemServico status)
    {
        // Arrange
        var ordensServico = GetOrdensServicoDeTeste();
        var especificacao = new ObterOrdemServicoPorStatusEspecificacao(status);

        // Act
        var resultado = ordensServico.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        var esperado = ordensServico.Count(os => os.Status == status);
        resultado.Should().HaveCount(esperado, $"deve retornar {esperado} ordem(ns) com status {status}");
        resultado.Should().OnlyContain(os => os.Status == status, 
            "todas as ordens devem ter o status especificado");
    }

    [Fact]
    public void ObterOrdemServicoPorIdComIncludeEspecificacao_DeveRetornarOrdemCorreta()
    {
        // Arrange
        var ordensServico = GetOrdensServicoDeTeste();
        var ordemId = ordensServico.First().Id;
        var especificacao = new ObterOrdemServicoPorIdComIncludeEspecificacao(ordemId);

        // Act
        var resultado = ordensServico.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(1, "deve retornar apenas a ordem de serviço com o ID especificado");
        resultado.First().Id.Should().Be(ordemId, "deve retornar a ordem correta");
    }

    [Fact]
    public void ObterOrdemServicoPorIdComIncludeEspecificacao_DeveIncluirRelacionamentos()
    {
        // Arrange
        var especificacao = new ObterOrdemServicoPorIdComIncludeEspecificacao(Guid.NewGuid());

        // Assert
        especificacao.Inclusoes.Should().NotBeEmpty("deve ter inclusões definidas");
    }

    [Fact]
    public void ObterOrdemServicoPorIdComInsumosEspecificacao_DeveRetornarOrdemCorreta()
    {
        // Arrange
        var ordensServico = GetOrdensServicoDeTeste();
        var ordemId = ordensServico.First().Id;
        var especificacao = new ObterOrdemServicoPorIdComInsumosEspecificacao(ordemId);

        // Act
        var resultado = ordensServico.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(1, "deve retornar apenas a ordem de serviço com o ID especificado");
        resultado.First().Id.Should().Be(ordemId, "deve retornar a ordem correta");
    }

    [Fact]
    public void ObterOrdemServicoPorIdComInsumosEspecificacao_DeveIncluirInsumos()
    {
        // Arrange
        var especificacao = new ObterOrdemServicoPorIdComInsumosEspecificacao(Guid.NewGuid());

        // Assert
        especificacao.Inclusoes.Should().NotBeEmpty("deve ter inclusões definidas");
        especificacao.Inclusoes.Should().Contain(i => i.ToString().Contains("Insumos"), 
            "deve incluir a propriedade Insumos");
    }

    [Fact]
    public void ObterOSOrcamentoExpiradoEspecificacao_DeveRetornarOrcamentosExpirados()
    {
        // Arrange
        var especificacao = new ObterOSOrcamentoExpiradoEspecificacao();

        // Criar ordens com datas específicas para teste
        var ordensServico = new List<OrdemServicoEntityDto>
        {
            OrdemServicoFixture.CriarOrdemServicoEntityDtoValido(),
            OrdemServicoFixture.CriarOrdemServicoEntityDtoValido()
        };

        ordensServico[0].Status = StatusOrdemServico.AguardandoAprovação;
        ordensServico[0].DataEnvioOrcamento = DateTime.UtcNow.AddDays(-5); // Expirado (mais de 3 dias)

        ordensServico[1].Status = StatusOrdemServico.AguardandoAprovação;
        ordensServico[1].DataEnvioOrcamento = DateTime.UtcNow.AddDays(-2); // Não expirado (menos de 3 dias)

        // Act
        var resultado = ordensServico.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(1, "deve retornar apenas orçamentos expirados");
        resultado.Should().OnlyContain(os => os.Status == StatusOrdemServico.AguardandoAprovação, 
            "deve conter apenas ordens com status AguardandoAprovação");
    }

    [Fact]
    public void ObterOSOrcamentoExpiradoEspecificacao_ComOrcamentosNaoExpirados_DeveRetornarListaVazia()
    {
        // Arrange
        var especificacao = new ObterOSOrcamentoExpiradoEspecificacao();

        var ordensServico = new List<OrdemServicoEntityDto>
        {
            OrdemServicoFixture.CriarOrdemServicoEntityDtoValido()
        };

        ordensServico[0].Status = StatusOrdemServico.AguardandoAprovação;
        ordensServico[0].DataEnvioOrcamento = DateTime.UtcNow.AddDays(-2); // Não expirado (menos de 3 dias)

        // Act
        var resultado = ordensServico.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().BeEmpty("não deve retornar orçamentos não expirados");
    }

    [Fact]
    public void ObterOSOrcamentoExpiradoEspecificacao_ComOrcamentosExpirados_DeveFiltrarCorretamente()
    {
        // Arrange
        var especificacao = new ObterOSOrcamentoExpiradoEspecificacao();

        var ordensServico = new List<OrdemServicoEntityDto>
        {
            OrdemServicoFixture.CriarOrdemServicoEntityDtoValido(),
            OrdemServicoFixture.CriarOrdemServicoEntityDtoValido()
        };

        ordensServico[0].Status = StatusOrdemServico.AguardandoAprovação;
        ordensServico[0].DataEnvioOrcamento = DateTime.UtcNow.AddDays(-4); // Expirado (mais de 3 dias)

        ordensServico[1].Status = StatusOrdemServico.AguardandoAprovação;
        ordensServico[1].DataEnvioOrcamento = DateTime.UtcNow.AddDays(-2); // Não expirado (menos de 3 dias)

        // Act
        var resultado = ordensServico.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(1, "deve retornar 1 orçamento expirado");
        resultado.First().DataEnvioOrcamento.Should().HaveValue("orçamento expirado deve ter data de envio");
    }
}
