using Xunit;
using FluentAssertions;
using Core.DTOs.Repositories.OrdemServicos;
using Core.DTOs.Repositories.Autenticacao;
using Core.DTOs.Repositories.Cliente;
using Core.DTOs.Repositories.Servico;
using Core.DTOs.Repositories.Veiculo;
using Core.Enumeradores;

namespace MecanicaOS.UnitTests.Core.DTOs.Repositories;

public class OrdemServicoRepositoryDtoUnitTests
{
    [Fact]
    public void OrdemServicoRepositoryDto_QuandoCriado_DeveHerdarDeRepositoryDto()
    {
        // Arrange & Act
        var dto = new OrdemServicoRepositoryDto();

        // Assert
        dto.Should().BeAssignableTo<RepositoryDto>("OrdemServicoRepositoryDto deve herdar de RepositoryDto");
        dto.Id.Should().Be(Guid.Empty, "Id deve ser vazio por padrão no DTO");
        dto.Ativo.Should().BeFalse("Ativo deve ser false por padrão no DTO");
        dto.InsumosOS.Should().NotBeNull("InsumosOS deve ser inicializada");
        dto.InsumosOS.Should().BeEmpty("InsumosOS deve estar vazia inicialmente");
    }

    [Fact]
    public void OrdemServicoRepositoryDto_QuandoDefinidoCamposTecnicos_DevePreservarAuditoria()
    {
        // Arrange
        var dto = new OrdemServicoRepositoryDto();
        var id = Guid.NewGuid();
        var dataCadastro = DateTime.Now;
        var dataAtualizacao = DateTime.Now.AddMinutes(5);

        // Act
        dto.Id = id;
        dto.DataCadastro = dataCadastro;
        dto.DataAtualizacao = dataAtualizacao;
        dto.Ativo = true;

        // Assert
        dto.Id.Should().Be(id, "o ID deve ser preservado corretamente");
        dto.DataCadastro.Should().Be(dataCadastro, "a data de cadastro deve ser preservada");
        dto.DataAtualizacao.Should().Be(dataAtualizacao, "a data de atualização deve ser preservada");
        dto.Ativo.Should().BeTrue("o status ativo deve ser preservado");
    }

    [Fact]
    public void OrdemServicoRepositoryDto_QuandoDefinidoClienteId_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new OrdemServicoRepositoryDto();
        var clienteIdEsperado = Guid.NewGuid();

        // Act
        dto.ClienteId = clienteIdEsperado;

        // Assert
        dto.ClienteId.Should().Be(clienteIdEsperado, "o ClienteId deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void OrdemServicoRepositoryDto_QuandoDefinidoVeiculoId_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new OrdemServicoRepositoryDto();
        var veiculoIdEsperado = Guid.NewGuid();

        // Act
        dto.VeiculoId = veiculoIdEsperado;

        // Assert
        dto.VeiculoId.Should().Be(veiculoIdEsperado, "o VeiculoId deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void OrdemServicoRepositoryDto_QuandoDefinidoServicoId_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new OrdemServicoRepositoryDto();
        var servicoIdEsperado = Guid.NewGuid();

        // Act
        dto.ServicoId = servicoIdEsperado;

        // Assert
        dto.ServicoId.Should().Be(servicoIdEsperado, "o ServicoId deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void OrdemServicoRepositoryDto_QuandoDefinidoOrcamento_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new OrdemServicoRepositoryDto();
        var orcamentoEsperado = 1500.50m;

        // Act
        dto.Orcamento = orcamentoEsperado;

        // Assert
        dto.Orcamento.Should().Be(orcamentoEsperado, "o orçamento deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void OrdemServicoRepositoryDto_QuandoDefinidaDescricao_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new OrdemServicoRepositoryDto();
        var descricaoEsperada = "Troca de óleo e filtros";

        // Act
        dto.Descricao = descricaoEsperada;

        // Assert
        dto.Descricao.Should().Be(descricaoEsperada, "a descrição deve ser armazenada corretamente no DTO");
    }

    [Theory]
    [InlineData(StatusOrdemServico.Recebida)]
    [InlineData(StatusOrdemServico.EmExecucao)]
    [InlineData(StatusOrdemServico.Finalizada)]
    [InlineData(StatusOrdemServico.Cancelada)]
    public void OrdemServicoRepositoryDto_QuandoDefinidoStatus_DeveArmazenarCorretamente(StatusOrdemServico status)
    {
        // Arrange
        var dto = new OrdemServicoRepositoryDto();

        // Act
        dto.Status = status;

        // Assert
        dto.Status.Should().Be(status, "o status deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void OrdemServicoRepositoryDto_QuandoDefinidaDataEnvioOrcamento_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new OrdemServicoRepositoryDto();
        var dataEsperada = DateTime.Now;

        // Act
        dto.DataEnvioOrcamento = dataEsperada;

        // Assert
        dto.DataEnvioOrcamento.Should().Be(dataEsperada, "a data de envio do orçamento deve ser armazenada corretamente no DTO");
    }

    [Fact]
    public void OrdemServicoRepositoryDto_QuandoDefinidoCliente_DeveArmazenarReferencia()
    {
        // Arrange
        var dto = new OrdemServicoRepositoryDto();
        var clienteDto = new ClienteRepositoryDTO { Nome = "João Silva" };

        // Act
        dto.Cliente = clienteDto;

        // Assert
        dto.Cliente.Should().Be(clienteDto, "a referência do cliente deve ser armazenada corretamente no DTO");
        dto.Cliente.Nome.Should().Be("João Silva", "o nome do cliente deve ser preservado na referência");
    }

    [Fact]
    public void OrdemServicoRepositoryDto_QuandoDefinidoVeiculo_DeveArmazenarReferencia()
    {
        // Arrange
        var dto = new OrdemServicoRepositoryDto();
        var veiculoDto = new VeiculoRepositoryDto { Placa = "ABC-1234" };

        // Act
        dto.Veiculo = veiculoDto;

        // Assert
        dto.Veiculo.Should().Be(veiculoDto, "a referência do veículo deve ser armazenada corretamente no DTO");
        dto.Veiculo.Placa.Should().Be("ABC-1234", "a placa do veículo deve ser preservada na referência");
    }

    [Fact]
    public void OrdemServicoRepositoryDto_QuandoDefinidoServico_DeveArmazenarReferencia()
    {
        // Arrange
        var dto = new OrdemServicoRepositoryDto();
        var servicoDto = new ServicoRepositoryDto { Nome = "Troca de Óleo", Descricao = "Serviço de troca de óleo" };

        // Act
        dto.Servico = servicoDto;

        // Assert
        dto.Servico.Should().Be(servicoDto, "a referência do serviço deve ser armazenada corretamente no DTO");
        dto.Servico.Nome.Should().Be("Troca de Óleo", "o nome do serviço deve ser preservado na referência");
    }

    [Fact]
    public void OrdemServicoRepositoryDto_QuandoDefinidaListaInsumosOS_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new OrdemServicoRepositoryDto();
        var insumosOS = new List<InsumoOSRepositoryDto>
        {
            new InsumoOSRepositoryDto { Quantidade = 2 },
            new InsumoOSRepositoryDto { Quantidade = 1 }
        };

        // Act
        dto.InsumosOS = insumosOS;

        // Assert
        dto.InsumosOS.Should().HaveCount(2, "a lista de insumos OS deve conter 2 itens");
        dto.InsumosOS.Should().Contain(i => i.Quantidade == 2, "deve conter o primeiro insumo");
        dto.InsumosOS.Should().Contain(i => i.Quantidade == 1, "deve conter o segundo insumo");
    }
}
