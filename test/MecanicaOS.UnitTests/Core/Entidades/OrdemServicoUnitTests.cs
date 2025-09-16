using Core.Entidades;
using Core.Entidades.Abstratos;
using Core.Enumeradores;

namespace MecanicaOS.UnitTests.Core.Entidades;

public class OrdemServicoUnitTests
{
    [Fact]
    public void OrdemServico_QuandoCriada_DeveInicializarComValoresPadrao()
    {
        // Arrange & Act
        var ordemServico = new OrdemServico();

        // Assert
        ordemServico.Should().BeAssignableTo<Entidade>("OrdemServico deve herdar de Entidade");
        ordemServico.Id.Should().NotBeEmpty("Id deve ser gerado automaticamente");
        ordemServico.Ativo.Should().BeTrue("OrdemServico deve estar ativa por padrão");
        ordemServico.DataCadastro.Should().NotBe(default(DateTime), "DataCadastro deve ser definida");
        ordemServico.DataAtualizacao.Should().NotBe(default(DateTime), "DataAtualizacao deve ser definida");
        ordemServico.InsumosOS.Should().NotBeNull("InsumosOS deve ser inicializada");
        ordemServico.InsumosOS.Should().BeEmpty("InsumosOS deve estar vazia inicialmente");
    }

    [Fact]
    public void OrdemServico_QuandoDefinidoClienteId_DeveArmazenarCorretamente()
    {
        // Arrange
        var ordemServico = new OrdemServico();
        var clienteIdEsperado = Guid.NewGuid();

        // Act
        ordemServico.ClienteId = clienteIdEsperado;

        // Assert
        ordemServico.ClienteId.Should().Be(clienteIdEsperado, "o ClienteId deve ser armazenado corretamente");
    }

    [Fact]
    public void OrdemServico_QuandoDefinidoVeiculoId_DeveArmazenarCorretamente()
    {
        // Arrange
        var ordemServico = new OrdemServico();
        var veiculoIdEsperado = Guid.NewGuid();

        // Act
        ordemServico.VeiculoId = veiculoIdEsperado;

        // Assert
        ordemServico.VeiculoId.Should().Be(veiculoIdEsperado, "o VeiculoId deve ser armazenado corretamente");
    }

    [Fact]
    public void OrdemServico_QuandoDefinidoServicoId_DeveArmazenarCorretamente()
    {
        // Arrange
        var ordemServico = new OrdemServico();
        var servicoIdEsperado = Guid.NewGuid();

        // Act
        ordemServico.ServicoId = servicoIdEsperado;

        // Assert
        ordemServico.ServicoId.Should().Be(servicoIdEsperado, "o ServicoId deve ser armazenado corretamente");
    }

    [Fact]
    public void OrdemServico_QuandoDefinidoOrcamento_DeveArmazenarCorretamente()
    {
        // Arrange
        var ordemServico = new OrdemServico();
        var orcamentoEsperado = 1500.50m;

        // Act
        ordemServico.Orcamento = orcamentoEsperado;

        // Assert
        ordemServico.Orcamento.Should().Be(orcamentoEsperado, "o orçamento deve ser armazenado corretamente");
    }

    [Fact]
    public void OrdemServico_QuandoDefinidaDescricao_DeveArmazenarCorretamente()
    {
        // Arrange
        var ordemServico = new OrdemServico();
        var descricaoEsperada = "Troca de óleo e filtros";

        // Act
        ordemServico.Descricao = descricaoEsperada;

        // Assert
        ordemServico.Descricao.Should().Be(descricaoEsperada, "a descrição deve ser armazenada corretamente");
    }

    [Theory]
    [InlineData(StatusOrdemServico.Recebida)]
    [InlineData(StatusOrdemServico.EmExecucao)]
    [InlineData(StatusOrdemServico.Finalizada)]
    [InlineData(StatusOrdemServico.Cancelada)]
    public void OrdemServico_QuandoDefinidoStatus_DeveArmazenarCorretamente(StatusOrdemServico status)
    {
        // Arrange
        var ordemServico = new OrdemServico();

        // Act
        ordemServico.Status = status;

        // Assert
        ordemServico.Status.Should().Be(status, "o status deve ser armazenado corretamente");
    }

    [Fact]
    public void OrdemServico_QuandoDefinidaDataEnvioOrcamento_DeveArmazenarCorretamente()
    {
        // Arrange
        var ordemServico = new OrdemServico();
        var dataEsperada = DateTime.Now;

        // Act
        ordemServico.DataEnvioOrcamento = dataEsperada;

        // Assert
        ordemServico.DataEnvioOrcamento.Should().Be(dataEsperada, "a data de envio do orçamento deve ser armazenada corretamente");
    }

    [Fact]
    public void OrdemServico_QuandoAtualizadaComTodosParametros_DeveAtualizarCorretamente()
    {
        // Arrange
        var ordemServico = new OrdemServico();
        var novoClienteId = Guid.NewGuid();
        var novoVeiculoId = Guid.NewGuid();
        var novoServicoId = Guid.NewGuid();
        var novaDescricao = "Nova descrição do serviço";
        var novoStatus = StatusOrdemServico.EmExecucao;

        // Act
        ordemServico.Atualizar(novoClienteId, novoVeiculoId, novoServicoId, novaDescricao, novoStatus);

        // Assert
        ordemServico.ClienteId.Should().Be(novoClienteId, "o ClienteId deve ser atualizado");
        ordemServico.VeiculoId.Should().Be(novoVeiculoId, "o VeiculoId deve ser atualizado");
        ordemServico.ServicoId.Should().Be(novoServicoId, "o ServicoId deve ser atualizado");
        ordemServico.Descricao.Should().Be(novaDescricao, "a descrição deve ser atualizada");
        ordemServico.Status.Should().Be(novoStatus, "o status deve ser atualizado");
    }

    [Fact]
    public void OrdemServico_QuandoAtualizadaComParametrosNulos_NaoDeveAlterarValoresExistentes()
    {
        // Arrange
        var ordemServico = new OrdemServico
        {
            ClienteId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ServicoId = Guid.NewGuid(),
            Descricao = "Descrição original",
            Status = StatusOrdemServico.Recebida
        };

        var valoresOriginais = new
        {
            ClienteId = ordemServico.ClienteId,
            VeiculoId = ordemServico.VeiculoId,
            ServicoId = ordemServico.ServicoId,
            Descricao = ordemServico.Descricao,
            Status = ordemServico.Status
        };

        // Act
        ordemServico.Atualizar(null, null, null, null, null);

        // Assert
        ordemServico.ClienteId.Should().Be(valoresOriginais.ClienteId, "o ClienteId não deve ser alterado");
        ordemServico.VeiculoId.Should().Be(valoresOriginais.VeiculoId, "o VeiculoId não deve ser alterado");
        ordemServico.ServicoId.Should().Be(valoresOriginais.ServicoId, "o ServicoId não deve ser alterado");
        ordemServico.Descricao.Should().Be(valoresOriginais.Descricao, "a descrição não deve ser alterada");
        ordemServico.Status.Should().Be(valoresOriginais.Status, "o status não deve ser alterado");
    }

    [Fact]
    public void OrdemServico_QuandoComparadaComOutraOrdemServicoComMesmoId_DeveSerIgual()
    {
        // Arrange
        var id = Guid.NewGuid();
        var ordemServico1 = new OrdemServico { Id = id, Descricao = "Descrição 1" };
        var ordemServico2 = new OrdemServico { Id = id, Descricao = "Descrição 2" };

        // Act & Assert
        ordemServico1.Should().Be(ordemServico2, "ordens de serviço com mesmo Id devem ser consideradas iguais");
        ordemServico1.GetHashCode().Should().Be(ordemServico2.GetHashCode(), "hash codes devem ser iguais para objetos iguais");
    }
}
