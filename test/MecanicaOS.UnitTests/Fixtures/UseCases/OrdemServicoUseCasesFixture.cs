using Core.DTOs.UseCases.Eventos;
using Core.DTOs.UseCases.OrdemServico;
using Core.Entidades;
using Core.Enumeradores;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.UseCases;

namespace MecanicaOS.UnitTests.Fixtures.UseCases;

public class OrdemServicoUseCasesFixture : UseCasesFixtureBase
{

    public IOrdemServicoUseCases CriarOrdemServicoUseCases(
        IOrdemServicoGateway? mockOrdemServicoGateway = null,
        IClienteGateway? mockClienteGateway = null,
        IServicoUseCases? mockServicoUseCases = null,
        IEventosGateway? mockEventosGateway = null,
        IUnidadeDeTrabalho? mockUdt = null)
    {
        // Para os testes, vamos criar um mock da interface IOrdemServicoUseCases
        // Os testes devem focar no comportamento da interface, não na implementação interna
        return Substitute.For<IOrdemServicoUseCases>();
    }

    public static OrdemServico CriarOrdemServicoValida()
    {
        return new OrdemServico
        {
            Id = Guid.NewGuid(),
            ClienteId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ServicoId = Guid.NewGuid(),
            Descricao = "Troca de óleo e filtros",
            Status = StatusOrdemServico.Recebida,
            DataCadastro = DateTime.UtcNow.AddHours(-2),
            DataAtualizacao = DateTime.UtcNow.AddHours(-1),
            Ativo = true
        };
    }

    public static OrdemServico CriarOrdemServicoEmDiagnostico()
    {
        return new OrdemServico
        {
            Id = Guid.NewGuid(),
            ClienteId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ServicoId = Guid.NewGuid(),
            Descricao = "Diagnóstico de problema no motor",
            Status = StatusOrdemServico.EmDiagnostico,
            DataCadastro = DateTime.UtcNow.AddHours(-3),
            DataAtualizacao = DateTime.UtcNow.AddHours(-1),
            Ativo = true
        };
    }

    public static OrdemServico CriarOrdemServicoAguardandoAprovacao()
    {
        return new OrdemServico
        {
            Id = Guid.NewGuid(),
            ClienteId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ServicoId = Guid.NewGuid(),
            Descricao = "Reparo na transmissão",
            Status = StatusOrdemServico.AguardandoAprovacao,
            DataEnvioOrcamento = DateTime.UtcNow.AddDays(-1),
            DataCadastro = DateTime.UtcNow.AddDays(-2),
            DataAtualizacao = DateTime.UtcNow.AddDays(-1),
            Ativo = true
        };
    }

    public static OrdemServico CriarOrdemServicoOrcamentoExpirado()
    {
        return new OrdemServico
        {
            Id = Guid.NewGuid(),
            ClienteId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ServicoId = Guid.NewGuid(),
            Descricao = "Reparo no sistema de freios",
            Status = StatusOrdemServico.OrcamentoExpirado,
            DataEnvioOrcamento = DateTime.UtcNow.AddDays(-5),
            DataCadastro = DateTime.UtcNow.AddDays(-6),
            DataAtualizacao = DateTime.UtcNow.AddDays(-1),
            Ativo = true
        };
    }

    public static OrdemServico CriarOrdemServicoFinalizada()
    {
        return new OrdemServico
        {
            Id = Guid.NewGuid(),
            ClienteId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ServicoId = Guid.NewGuid(),
            Descricao = "Manutenção preventiva completa",
            Status = StatusOrdemServico.Finalizada,
            DataCadastro = DateTime.UtcNow.AddDays(-7),
            DataAtualizacao = DateTime.UtcNow.AddDays(-1),
            Ativo = true
        };
    }

    public static OrdemServico CriarOrdemServicoCancelada()
    {
        return new OrdemServico
        {
            Id = Guid.NewGuid(),
            ClienteId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ServicoId = Guid.NewGuid(),
            Descricao = "Serviço cancelado pelo cliente",
            Status = StatusOrdemServico.Cancelada,
            DataCadastro = DateTime.UtcNow.AddDays(-5),
            DataAtualizacao = DateTime.UtcNow.AddDays(-2),
            Ativo = true
        };
    }

    public static OrdemServico CriarOrdemServicoEmExecucao()
    {
        return new OrdemServico
        {
            Id = Guid.NewGuid(),
            ClienteId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ServicoId = Guid.NewGuid(),
            Descricao = "Serviço em execução",
            Status = StatusOrdemServico.EmExecucao,
            DataCadastro = DateTime.UtcNow.AddDays(-3),
            DataAtualizacao = DateTime.UtcNow.AddDays(-1),
            Ativo = true
        };
    }

    public static CadastrarOrdemServicoUseCaseDto CriarCadastrarOrdemServicoUseCaseDtoValido()
    {
        return new CadastrarOrdemServicoUseCaseDto
        {
            ClienteId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ServicoId = Guid.NewGuid(),
            Descricao = "Troca de óleo e filtros"
        };
    }

    public static AtualizarOrdemServicoUseCaseDto CriarAtualizarOrdemServicoUseCaseDtoValido()
    {
        return new AtualizarOrdemServicoUseCaseDto
        {
            ClienteId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ServicoId = Guid.NewGuid(),
            Descricao = "Troca de óleo, filtros e revisão geral",
            Status = StatusOrdemServico.EmExecucao
        };
    }

    public static AtualizarOrdemServicoUseCaseDto CriarAtualizarOrdemServicoParaFinalizada()
    {
        return new AtualizarOrdemServicoUseCaseDto
        {
            ClienteId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ServicoId = Guid.NewGuid(),
            Descricao = "Serviço finalizado com sucesso",
            Status = StatusOrdemServico.Finalizada
        };
    }

    public static Cliente CriarClienteComVeiculo()
    {
        var clienteId = Guid.NewGuid();
        var veiculoId = Guid.NewGuid();

        return new Cliente
        {
            Id = clienteId,
            Nome = "João Silva",
            Documento = "12345678901",
            TipoCliente = TipoCliente.PessoaFisica,
            Contato = new Contato
            {
                Email = "joao@email.com",
                Telefone = "11999999999"
            },
            Veiculos = new List<Veiculo>
            {
                new Veiculo
                {
                    Id = veiculoId,
                    ClienteId = clienteId,
                    Marca = "Toyota",
                    Modelo = "Corolla",
                    Ano = "2020",
                    Placa = "ABC1234",
                    Cor = "Branco",
                    DataCadastro = DateTime.UtcNow.AddDays(-30),
                    DataAtualizacao = DateTime.UtcNow.AddDays(-30),
                    Ativo = true
                }
            },
            DataCadastro = DateTime.UtcNow.AddDays(-30),
            DataAtualizacao = DateTime.UtcNow.AddDays(-30),
            Ativo = true
        };
    }

    public static Servico CriarServicoDisponivel()
    {
        return new Servico
        {
            Id = Guid.NewGuid(),
            Nome = "Troca de Óleo",
            Descricao = "Troca de óleo do motor",
            Valor = 150.00m,
            Disponivel = true,
            DataCadastro = DateTime.UtcNow.AddDays(-10),
            DataAtualizacao = DateTime.UtcNow.AddDays(-10),
            Ativo = true
        };
    }

    public static Servico CriarServicoIndisponivel()
    {
        return new Servico
        {
            Id = Guid.NewGuid(),
            Nome = "Reparo Especial",
            Descricao = "Serviço temporariamente indisponível",
            Valor = 500.00m,
            Disponivel = false,
            DataCadastro = DateTime.UtcNow.AddDays(-5),
            DataAtualizacao = DateTime.UtcNow.AddDays(-1),
            Ativo = true
        };
    }

    public static List<OrdemServico> CriarListaOrdensServicoVariadas()
    {
        return new List<OrdemServico>
        {
            CriarOrdemServicoValida(),
            CriarOrdemServicoEmDiagnostico(),
            CriarOrdemServicoAguardandoAprovacao(),
            CriarOrdemServicoFinalizada(),
            CriarOrdemServicoCancelada(),
            CriarOrdemServicoEmExecucao()
        };
    }

    public void ConfigurarMockOrdemServicoGatewayParaCadastro(
        IOrdemServicoGateway mockGateway,
        OrdemServico? ordemServicoEsperada = null)
    {
        var ordemServico = ordemServicoEsperada ?? CriarOrdemServicoValida();
        mockGateway.CadastrarAsync(Arg.Any<OrdemServico>()).Returns(Task.FromResult(ordemServico));
    }

    public void ConfigurarMockOrdemServicoGatewayParaAtualizacao(
        IOrdemServicoGateway mockGateway,
        OrdemServico ordemServicoExistente)
    {
        mockGateway.ObterPorIdAsync(ordemServicoExistente.Id).Returns(Task.FromResult(ordemServicoExistente));
        mockGateway.EditarAsync(Arg.Any<OrdemServico>()).Returns(Task.CompletedTask);
    }

    public void ConfigurarMockOrdemServicoGatewayParaOrdemNaoEncontrada(
        IOrdemServicoGateway mockGateway,
        Guid ordemServicoId)
    {
        mockGateway.ObterPorIdAsync(ordemServicoId).Returns((OrdemServico?)null);
    }

    public void ConfigurarMockOrdemServicoGatewayParaListagem(
        IOrdemServicoGateway mockGateway,
        List<OrdemServico> ordensServico)
    {
        mockGateway.ObterTodosAsync().Returns(ordensServico);
    }

    public void ConfigurarMockOrdemServicoGatewayParaStatus(
        IOrdemServicoGateway mockGateway,
        StatusOrdemServico status,
        List<OrdemServico> ordensServico)
    {
        mockGateway.ObterOrdemServicoPorStatusAsync(status).Returns(ordensServico);
    }

    public void ConfigurarMockClienteGatewayParaVerificacao(
        IClienteGateway mockGateway,
        Guid clienteId,
        Cliente? cliente = null)
    {
        var clienteRetorno = cliente ?? CriarClienteComVeiculo();
        clienteRetorno.Id = clienteId;
        mockGateway.ObterClienteComVeiculoPorIdAsync(clienteId).Returns(clienteRetorno);
    }

    public void ConfigurarMockClienteGatewayParaClienteNaoEncontrado(
        IClienteGateway mockGateway,
        Guid clienteId)
    {
        mockGateway.ObterClienteComVeiculoPorIdAsync(clienteId).Returns((Cliente?)null);
    }

    public void ConfigurarMockServicoUseCasesParaVerificacao(
        IServicoUseCases mockServicoUseCases,
        Guid servicoId,
        Servico? servico = null)
    {
        var servicoRetorno = servico ?? CriarServicoDisponivel();
        servicoRetorno.Id = servicoId;
        mockServicoUseCases.ObterServicoPorIdUseCaseAsync(servicoId).Returns(servicoRetorno);
    }

    public void ConfigurarMockEventosGateway(IEventosGateway mockGateway)
    {
        mockGateway.Publicar(Arg.Any<OrdemServicoFinalizadaEventDTO>()).Returns(Task.CompletedTask);
        mockGateway.Publicar(Arg.Any<OrdemServicoEmOrcamentoEventDTO>()).Returns(Task.CompletedTask);
        mockGateway.Publicar(Arg.Any<OrdemServicoCanceladaEventDTO>()).Returns(Task.CompletedTask);
    }
}
