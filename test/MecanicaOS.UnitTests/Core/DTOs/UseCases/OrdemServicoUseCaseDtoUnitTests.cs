using Core.DTOs.UseCases.OrdemServico;
using Core.DTOs.UseCases.OrdemServico.InsumoOS;
using Core.Enumeradores;
using MecanicaOS.UnitTests.Fixtures.UseCases;

namespace MecanicaOS.UnitTests.Core.DTOs.UseCases;

public class OrdemServicoUseCaseDtoUnitTests
{
    [Fact]
    public void CadastrarOrdemServicoUseCaseDto_DeveSerInicializadoComPropriedadesCorretas()
    {
        // Arrange & Act
        var dto = OrdemServicoUseCaseFixture.CriarCadastrarOrdemServicoUseCaseDtoValido();

        // Assert
        dto.Should().NotBeNull("o DTO deve ser criado corretamente");
        dto.ClienteId.Should().NotBeEmpty("deve ter um ClienteId válido");
        dto.VeiculoId.Should().NotBeEmpty("deve ter um VeiculoId válido");
        dto.ServicoId.Should().NotBeEmpty("deve ter um ServicoId válido");
        dto.Descricao.Should().Be("Troca de óleo e filtro - manutenção preventiva",
            "a descrição deve ser armazenada corretamente");
    }

    [Fact]
    public void CadastrarOrdemServicoUseCaseDto_DevePermitirDescricaoNula()
    {
        // Arrange & Act
        var dto = OrdemServicoUseCaseFixture.CriarCadastrarOrdemServicoUseCaseDtoSemDescricao();

        // Assert
        dto.Descricao.Should().BeNull("deve permitir descrição nula");
        dto.ClienteId.Should().NotBeEmpty("deve manter IDs obrigatórios");
        dto.VeiculoId.Should().NotBeEmpty("deve manter IDs obrigatórios");
        dto.ServicoId.Should().NotBeEmpty("deve manter IDs obrigatórios");
    }

    [Fact]
    public void CadastrarOrdemServicoUseCaseDto_DevePermitirDescricaoLonga()
    {
        // Arrange & Act
        var dto = OrdemServicoUseCaseFixture.CriarCadastrarOrdemServicoUseCaseDtoDescricaoLonga();

        // Assert
        dto.Descricao.Should().NotBeNullOrEmpty("deve aceitar descrições longas");
        dto.Descricao!.Length.Should().BeGreaterThan(100, "deve ser uma descrição longa");
        dto.Descricao.Should().Contain("freios", "deve conter conteúdo relevante");
        dto.Descricao.Should().Contain("ABS", "deve conter conteúdo técnico");
    }

    [Fact]
    public void CadastrarOrdemServicoUseCaseDto_DeveValidarIdsObrigatorios()
    {
        // Arrange & Act
        var dto = new CadastrarOrdemServicoUseCaseDto
        {
            ClienteId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ServicoId = Guid.NewGuid(),
            Descricao = "Teste"
        };

        // Assert
        dto.ClienteId.Should().NotBeEmpty("ClienteId deve ser válido");
        dto.VeiculoId.Should().NotBeEmpty("VeiculoId deve ser válido");
        dto.ServicoId.Should().NotBeEmpty("ServicoId deve ser válido");
    }

    [Fact]
    public void AtualizarOrdemServicoUseCaseDto_DeveSerInicializadoComPropriedadesCorretas()
    {
        // Arrange & Act
        var dto = OrdemServicoUseCaseFixture.CriarAtualizarOrdemServicoUseCaseDtoValido();

        // Assert
        dto.Should().NotBeNull("o DTO deve ser criado corretamente");
        dto.ClienteId.Should().NotBeEmpty("deve ter um ClienteId válido");
        dto.VeiculoId.Should().NotBeEmpty("deve ter um VeiculoId válido");
        dto.ServicoId.Should().NotBeEmpty("deve ter um ServicoId válido");
        dto.Descricao.Should().Be("Descrição atualizada do serviço",
            "a descrição deve ser armazenada corretamente");
        dto.Status.Should().Be(StatusOrdemServico.EmExecucao, "o status deve ser armazenado corretamente");
    }

    [Fact]
    public void AtualizarOrdemServicoUseCaseDto_DevePermitirCamposNulos()
    {
        // Arrange & Act
        var dto = OrdemServicoUseCaseFixture.CriarAtualizarOrdemServicoUseCaseDtoComCamposNulos();

        // Assert
        dto.ClienteId.Should().BeNull("deve permitir ClienteId nulo");
        dto.VeiculoId.Should().BeNull("deve permitir VeiculoId nulo");
        dto.ServicoId.Should().BeNull("deve permitir ServicoId nulo");
        dto.Descricao.Should().BeNull("deve permitir descrição nula");
        dto.Status.Should().BeNull("deve permitir status nulo");
    }

    [Fact]
    public void AtualizarOrdemServicoUseCaseDto_DevePermitirAtualizacaoParcial()
    {
        // Arrange & Act
        var dto = OrdemServicoUseCaseFixture.CriarAtualizarOrdemServicoUseCaseDtoApenasStatus();

        // Assert
        dto.Status.Should().Be(StatusOrdemServico.Finalizada, "deve permitir atualização apenas do status");
        dto.ClienteId.Should().BeNull("outros campos podem permanecer nulos");
        dto.Descricao.Should().BeNull("outros campos podem permanecer nulos");
    }

    [Theory]
    [InlineData(StatusOrdemServico.Recebida)]
    [InlineData(StatusOrdemServico.EmExecucao)]
    [InlineData(StatusOrdemServico.Finalizada)]
    [InlineData(StatusOrdemServico.Cancelada)]
    public void AtualizarOrdemServicoUseCaseDto_DeveAceitarTodosStatus(StatusOrdemServico status)
    {
        // Arrange & Act
        var dto = new AtualizarOrdemServicoUseCaseDto
        {
            Status = status
        };

        // Assert
        dto.Status.Should().Be(status, "deve aceitar o status especificado");
    }

    [Fact]
    public void CadastrarInsumoOSUseCaseDto_DeveSerInicializadoComPropriedadesCorretas()
    {
        // Arrange & Act
        var dto = OrdemServicoUseCaseFixture.CriarCadastrarInsumoOSUseCaseDtoValido();

        // Assert
        dto.Should().NotBeNull("o DTO deve ser criado corretamente");
        dto.EstoqueId.Should().NotBeEmpty("deve ter um EstoqueId válido");
        dto.Quantidade.Should().Be(2, "a quantidade deve ser armazenada corretamente");
    }

    [Fact]
    public void CadastrarInsumoOSUseCaseDto_DevePermitirQuantidadeAlta()
    {
        // Arrange & Act
        var dto = OrdemServicoUseCaseFixture.CriarCadastrarInsumoOSUseCaseDtoQuantidadeAlta();

        // Assert
        dto.Quantidade.Should().Be(50, "deve aceitar quantidades altas");
        dto.EstoqueId.Should().NotBeEmpty("deve manter EstoqueId válido");
    }

    [Fact]
    public void CadastrarInsumoOSUseCaseDto_DevePermitirQuantidadeMinima()
    {
        // Arrange & Act
        var dto = OrdemServicoUseCaseFixture.CriarCadastrarInsumoOSUseCaseDtoQuantidadeMinima();

        // Assert
        dto.Quantidade.Should().Be(1, "deve aceitar quantidade mínima");
        dto.EstoqueId.Should().NotBeEmpty("deve manter EstoqueId válido");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public void CadastrarInsumoOSUseCaseDto_DeveAceitarDiferentesQuantidades(int quantidade)
    {
        // Arrange & Act
        var dto = new CadastrarInsumoOSUseCaseDto
        {
            EstoqueId = Guid.NewGuid(),
            Quantidade = quantidade
        };

        // Assert
        dto.Quantidade.Should().Be(quantidade, "deve aceitar a quantidade especificada");
    }

    [Fact]
    public void CadastrarOrdemServicoUseCaseDto_ListaFixture_DeveConterTodosOsItens()
    {
        // Arrange & Act
        var lista = OrdemServicoUseCaseFixture.CriarListaCadastrarOrdemServicoUseCaseDto();

        // Assert
        lista.Should().NotBeNull("a lista deve ser criada");
        lista.Should().HaveCount(3, "deve conter todos os DTOs da fixture");
        lista.Should().OnlyContain(dto => dto.ClienteId != Guid.Empty,
            "todos os DTOs devem ter ClienteId válido");
        lista.Should().OnlyContain(dto => dto.VeiculoId != Guid.Empty,
            "todos os DTOs devem ter VeiculoId válido");
        lista.Should().OnlyContain(dto => dto.ServicoId != Guid.Empty,
            "todos os DTOs devem ter ServicoId válido");
    }

    [Fact]
    public void AtualizarOrdemServicoUseCaseDto_ListaFixture_DeveConterTodosOsItens()
    {
        // Arrange & Act
        var lista = OrdemServicoUseCaseFixture.CriarListaAtualizarOrdemServicoUseCaseDto();

        // Assert
        lista.Should().NotBeNull("a lista deve ser criada");
        lista.Should().HaveCount(3, "deve conter todos os DTOs da fixture");
    }

    [Fact]
    public void CadastrarInsumoOSUseCaseDto_ListaFixture_DeveConterTodosOsItens()
    {
        // Arrange & Act
        var lista = OrdemServicoUseCaseFixture.CriarListaCadastrarInsumoOSUseCaseDto();

        // Assert
        lista.Should().NotBeNull("a lista deve ser criada");
        lista.Should().HaveCount(3, "deve conter todos os DTOs da fixture");
        lista.Should().OnlyContain(dto => dto.EstoqueId != Guid.Empty,
            "todos os DTOs devem ter EstoqueId válido");
        lista.Should().OnlyContain(dto => dto.Quantidade > 0,
            "todos os DTOs devem ter quantidade positiva");
    }

    [Fact]
    public void OrdemServicoUseCaseDto_DevePermitirAlteracaoDePropriedades()
    {
        // Arrange
        var dto = OrdemServicoUseCaseFixture.CriarCadastrarOrdemServicoUseCaseDtoValido();
        var novaDescricao = "Nova descrição do serviço";

        // Act
        dto.Descricao = novaDescricao;

        // Assert
        dto.Descricao.Should().Be(novaDescricao, "deve permitir alteração da descrição");
    }

    [Fact]
    public void OrdemServicoUseCaseDto_DeveSerDistintoEntreInstancias()
    {
        // Arrange & Act
        var dto1 = OrdemServicoUseCaseFixture.CriarCadastrarOrdemServicoUseCaseDtoValido();
        var dto2 = OrdemServicoUseCaseFixture.CriarCadastrarOrdemServicoUseCaseDtoSemDescricao();

        // Assert
        dto1.Should().NotBeSameAs(dto2, "devem ser instâncias diferentes");
        dto1.ClienteId.Should().NotBe(dto2.ClienteId, "devem ter ClienteIds diferentes");
        dto1.Descricao.Should().NotBe(dto2.Descricao, "devem ter descrições diferentes");
    }
}
