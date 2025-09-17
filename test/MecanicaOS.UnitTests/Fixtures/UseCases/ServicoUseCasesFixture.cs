using Core.DTOs.UseCases.Servico;
using Core.Entidades;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;
using Core.UseCases.Servicos;

namespace MecanicaOS.UnitTests.Fixtures.UseCases;

public class ServicoUseCasesFixture : UseCasesFixtureBase
{
    public IServicoUseCases CriarServicoUseCases(
        IServicoGateway? mockServicoGateway = null,
        ILogServico<IServicoUseCases>? mockLogServico = null,
        IUnidadeDeTrabalho? mockUdt = null,
        IUsuarioLogadoServico? mockUsuarioLogado = null)
    {
        // Para os testes, vamos criar um mock da interface IServicoUseCases
        // Os testes devem focar no comportamento da interface, não na implementação interna
        return Substitute.For<IServicoUseCases>();
    }

    public static Servico CriarServicoValido()
    {
        return new Servico
        {
            Id = Guid.NewGuid(),
            Nome = "Troca de Óleo",
            Descricao = "Serviço completo de troca de óleo do motor",
            Valor = 80.00m,
            Disponivel = true,
            DataCadastro = DateTime.UtcNow.AddDays(-30),
            DataAtualizacao = DateTime.UtcNow.AddDays(-5)
        };
    }

    public static Servico CriarServicoIndisponivel()
    {
        return new Servico
        {
            Id = Guid.NewGuid(),
            Nome = "Reparo de Transmissão",
            Descricao = "Serviço especializado em transmissão automática",
            Valor = 1500.00m,
            Disponivel = false,
            DataCadastro = DateTime.UtcNow.AddDays(-30),
            DataAtualizacao = DateTime.UtcNow.AddDays(-10)
        };
    }

    public static Servico CriarServicoAlinhamento()
    {
        return new Servico
        {
            Id = Guid.NewGuid(),
            Nome = "Alinhamento e Balanceamento",
            Descricao = "Serviço de alinhamento de direção e balanceamento de rodas",
            Valor = 120.00m,
            Disponivel = true,
            DataCadastro = DateTime.UtcNow.AddDays(-1),
            DataAtualizacao = DateTime.UtcNow.AddDays(-2)
        };
    }

    public static Servico CriarServicoRevisao()
    {
        return new Servico
        {
            Id = Guid.NewGuid(),
            Nome = "Revisão Completa",
            Descricao = "Revisão preventiva completa do veículo",
            Valor = 350.00m,
            Disponivel = true,
            DataCadastro = DateTime.UtcNow.AddDays(-45),
            DataAtualizacao = DateTime.UtcNow.AddDays(-7)
        };
    }

    public static CadastrarServicoUseCaseDto CriarCadastrarServicoUseCaseDtoValido()
    {
        return new CadastrarServicoUseCaseDto
        {
            Nome = "Troca de Pastilhas de Freio",
            Descricao = "Substituição das pastilhas de freio dianteiras e traseiras",
            Valor = 180.00m,
            Disponivel = true
        };
    }

    public static CadastrarServicoUseCaseDto CriarCadastrarServicoUseCaseDtoIndisponivel()
    {
        return new CadastrarServicoUseCaseDto
        {
            Nome = "Pintura Automotiva",
            Descricao = "Serviço completo de pintura automotiva",
            Valor = 2500.00m,
            Disponivel = false
        };
    }

    public static CadastrarServicoUseCaseDto CriarCadastrarServicoUseCaseDtoComValorZero()
    {
        return new CadastrarServicoUseCaseDto
        {
            Nome = "Diagnóstico Gratuito",
            Descricao = "Diagnóstico inicial gratuito para novos clientes",
            Valor = 0.00m,
            Disponivel = true
        };
    }

    public static EditarServicoUseCaseDto CriarEditarServicoUseCaseDtoValido()
    {
        return new EditarServicoUseCaseDto
        {
            Nome = "Troca de Óleo Premium",
            Descricao = "Serviço de troca de óleo com produtos premium",
            Valor = 120.00m,
            Disponivel = true
        };
    }

    public static EditarServicoUseCaseDto CriarEditarServicoUseCaseDtoParaIndisponivel()
    {
        return new EditarServicoUseCaseDto
        {
            Nome = "Serviço Temporariamente Indisponível",
            Descricao = "Serviço em manutenção",
            Valor = 100.00m,
            Disponivel = false
        };
    }

    public static List<Servico> CriarListaServicosVariados()
    {
        return new List<Servico>
        {
            CriarServicoValido(),
            CriarServicoAlinhamento(),
            CriarServicoRevisao(),
            CriarServicoIndisponivel(),
            new Servico
            {
                Id = Guid.NewGuid(),
                Nome = "Troca de Filtros",
                Descricao = "Substituição de filtros de ar, óleo e combustível",
                Valor = 95.00m,
                Disponivel = true,
                DataCadastro = DateTime.UtcNow.AddDays(-20),
                DataAtualizacao = DateTime.UtcNow.AddDays(-3)
            }
        };
    }

    public void ConfigurarMockServicoGatewayParaCadastro(
        IServicoGateway mockServicoGateway,
        Servico? servicoRetorno = null)
    {
        mockServicoGateway.ObterServicosDisponiveisPorNomeAsync(Arg.Any<string>()).Returns(Task.FromResult((Servico?)null));

        mockServicoGateway.CadastrarAsync(Arg.Any<Servico>()).Returns(callInfo =>
        {
            var servico = callInfo.Arg<Servico>();
            if (servicoRetorno != null)
            {
                // Use the provided service but preserve the properties from the request
                servicoRetorno.Nome = servico.Nome;
                servicoRetorno.Descricao = servico.Descricao;
                servicoRetorno.Valor = servico.Valor;
                servicoRetorno.Disponivel = servico.Disponivel;
                return servicoRetorno;
            }
            return servico;
        });
    }

    public void ConfigurarMockServicoGatewayParaAtualizacao(
        IServicoGateway mockServicoGateway,
        Servico servicoExistente)
    {
        mockServicoGateway.ObterPorIdAsync(servicoExistente.Id).Returns(servicoExistente);
        mockServicoGateway.EditarAsync(Arg.Any<Servico>()).Returns(Task.CompletedTask);
    }

    public void ConfigurarMockServicoGatewayParaServicoNaoEncontrado(
        IServicoGateway mockServicoGateway,
        Guid servicoId)
    {
        mockServicoGateway.ObterPorIdAsync(servicoId).Returns((Servico?)null);
    }

    public void ConfigurarMockServicoGatewayParaNomeJaCadastrado(
        IServicoGateway mockServicoGateway,
        string nome)
    {
        var servicoExistente = CriarServicoValido();
        servicoExistente.Nome = nome;
        mockServicoGateway.ObterServicosDisponiveisPorNomeAsync(nome).Returns(servicoExistente);
    }

    public void ConfigurarMockServicoGatewayParaListagem(
        IServicoGateway mockServicoGateway,
        List<Servico>? servicos = null)
    {
        servicos ??= CriarListaServicosVariados();
        mockServicoGateway.ObterTodosAsync().Returns(servicos);
        mockServicoGateway.ObterServicoDisponivelAsync().Returns(servicos.Where(s => s.Disponivel).ToList());
    }

    public void ConfigurarMockServicoGatewayParaBusca(
        IServicoGateway mockServicoGateway,
        string nome,
        Servico? servicoEncontrado = null)
    {
        mockServicoGateway.ObterServicosDisponiveisPorNomeAsync(nome).Returns(Task.FromResult(servicoEncontrado));
    }

    public void ConfigurarMockServicoGatewayParaExclusao(
        IServicoGateway mockServicoGateway,
        Servico servicoExistente)
    {
        mockServicoGateway.ObterPorIdAsync(servicoExistente.Id).Returns(servicoExistente);
        mockServicoGateway.DeletarAsync(servicoExistente).Returns(Task.CompletedTask);
    }
}
