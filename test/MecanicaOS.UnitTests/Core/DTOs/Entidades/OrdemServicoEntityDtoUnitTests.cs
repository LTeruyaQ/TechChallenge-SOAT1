using Core.DTOs.Entidades.Autenticacao;
using Core.DTOs.Entidades.Cliente;
using Core.DTOs.Entidades.OrdemServicos;
using Core.DTOs.Entidades.Servico;
using Core.DTOs.Entidades.Veiculo;
using Core.Enumeradores;

namespace MecanicaOS.UnitTests.Core.DTOs.Entidades;

public class OrdemServicoEntityDtoUnitTests
{
    [Fact]
    public void OrdemServicoEntityDto_QuandoCriado_DeveHerdarDeEntityDto()
    {
        // Arrange & Act
        var dto = new OrdemServicoEntityDto();

        // Assert
        dto.Should().BeAssignableTo<EntityDto>("OrdemServicoEntityDto deve herdar de EntityDto");
        dto.Id.Should().Be(Guid.Empty, "Id deve ser vazio por padrão no DTO");
        dto.Ativo.Should().BeFalse("Ativo deve ser false por padrão no DTO");
        dto.InsumosOS.Should().NotBeNull("InsumosOS deve ser inicializada");
        dto.InsumosOS.Should().BeEmpty("InsumosOS deve estar vazia inicialmente");
    }

    [Fact]
    public void OrdemServicoEntityDto_QuandoDefinidoCamposTecnicos_DevePreservarAuditoria()
    {
        // Arrange
        var dto = new OrdemServicoEntityDto();
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
    public void OrdemServicoEntityDto_QuandoDefinidoClienteId_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new OrdemServicoEntityDto();
        var clienteIdEsperado = Guid.NewGuid();

        // Act
        dto.ClienteId = clienteIdEsperado;

        // Assert
        dto.ClienteId.Should().Be(clienteIdEsperado, "o ClienteId deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void OrdemServicoEntityDto_QuandoDefinidoVeiculoId_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new OrdemServicoEntityDto();
        var veiculoIdEsperado = Guid.NewGuid();

        // Act
        dto.VeiculoId = veiculoIdEsperado;

        // Assert
        dto.VeiculoId.Should().Be(veiculoIdEsperado, "o VeiculoId deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void OrdemServicoEntityDto_QuandoDefinidoServicoId_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new OrdemServicoEntityDto();
        var servicoIdEsperado = Guid.NewGuid();

        // Act
        dto.ServicoId = servicoIdEsperado;

        // Assert
        dto.ServicoId.Should().Be(servicoIdEsperado, "o ServicoId deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void OrdemServicoEntityDto_QuandoDefinidoOrcamento_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new OrdemServicoEntityDto();
        var orcamentoEsperado = 1500.50m;

        // Act
        dto.Orcamento = orcamentoEsperado;

        // Assert
        dto.Orcamento.Should().Be(orcamentoEsperado, "o orçamento deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void OrdemServicoEntityDto_QuandoDefinidaDescricao_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new OrdemServicoEntityDto();
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
    public void OrdemServicoEntityDto_QuandoDefinidoStatus_DeveArmazenarCorretamente(StatusOrdemServico status)
    {
        // Arrange
        var dto = new OrdemServicoEntityDto();

        // Act
        dto.Status = status;

        // Assert
        dto.Status.Should().Be(status, "o status deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void OrdemServicoEntityDto_QuandoDefinidaDataEnvioOrcamento_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new OrdemServicoEntityDto();
        var dataEsperada = DateTime.Now;

        // Act
        dto.DataEnvioOrcamento = dataEsperada;

        // Assert
        dto.DataEnvioOrcamento.Should().Be(dataEsperada, "a data de envio do orçamento deve ser armazenada corretamente no DTO");
    }

    [Fact]
    public void OrdemServicoEntityDto_QuandoDefinidoCliente_DeveArmazenarReferencia()
    {
        // Arrange
        var dto = new OrdemServicoEntityDto();
        var clienteDto = new ClienteEntityDto { Nome = "João Silva" };

        // Act
        dto.Cliente = clienteDto;

        // Assert
        dto.Cliente.Should().Be(clienteDto, "a referência do cliente deve ser armazenada corretamente no DTO");
        dto.Cliente.Nome.Should().Be("João Silva", "o nome do cliente deve ser preservado na referência");
    }

    [Fact]
    public void OrdemServicoEntityDto_QuandoDefinidoVeiculo_DeveArmazenarReferencia()
    {
        // Arrange
        var dto = new OrdemServicoEntityDto();
        var veiculoDto = new VeiculoEntityDto { Placa = "ABC-1234" };

        // Act
        dto.Veiculo = veiculoDto;

        // Assert
        dto.Veiculo.Should().Be(veiculoDto, "a referência do veículo deve ser armazenada corretamente no DTO");
        dto.Veiculo.Placa.Should().Be("ABC-1234", "a placa do veículo deve ser preservada na referência");
    }

    [Fact]
    public void OrdemServicoEntityDto_QuandoDefinidoServico_DeveArmazenarReferencia()
    {
        // Arrange
        var dto = new OrdemServicoEntityDto();
        var servicoDto = new ServicoEntityDto { Nome = "Troca de Óleo", Descricao = "Serviço de troca de óleo" };

        // Act
        dto.Servico = servicoDto;

        // Assert
        dto.Servico.Should().Be(servicoDto, "a referência do serviço deve ser armazenada corretamente no DTO");
        dto.Servico.Nome.Should().Be("Troca de Óleo", "o nome do serviço deve ser preservado na referência");
    }

    [Fact]
    public void OrdemServicoEntityDto_QuandoDefinidaListaInsumosOS_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new OrdemServicoEntityDto();
        var insumosOS = new List<InsumoOSEntityDto>
        {
            new InsumoOSEntityDto { Quantidade = 2 },
            new InsumoOSEntityDto { Quantidade = 1 }
        };

        // Act
        dto.InsumosOS = insumosOS;

        // Assert
        dto.InsumosOS.Should().HaveCount(2, "a lista de insumos OS deve conter 2 itens");
        dto.InsumosOS.Should().Contain(i => i.Quantidade == 2, "deve conter o primeiro insumo");
        dto.InsumosOS.Should().Contain(i => i.Quantidade == 1, "deve conter o segundo insumo");
    }
}
