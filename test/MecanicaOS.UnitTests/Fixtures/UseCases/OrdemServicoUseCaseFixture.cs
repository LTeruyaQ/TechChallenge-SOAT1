using Core.DTOs.UseCases.OrdemServico;
using Core.DTOs.UseCases.OrdemServico.InsumoOS;
using Core.Enumeradores;

namespace MecanicaOS.UnitTests.Fixtures.UseCases;

public static class OrdemServicoUseCaseFixture
{
    public static CadastrarOrdemServicoUseCaseDto CriarCadastrarOrdemServicoUseCaseDtoValido()
    {
        return new CadastrarOrdemServicoUseCaseDto
        {
            ClienteId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ServicoId = Guid.NewGuid(),
            Descricao = "Troca de óleo e filtro - manutenção preventiva"
        };
    }

    public static CadastrarOrdemServicoUseCaseDto CriarCadastrarOrdemServicoUseCaseDtoSemDescricao()
    {
        return new CadastrarOrdemServicoUseCaseDto
        {
            ClienteId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ServicoId = Guid.NewGuid(),
            Descricao = null
        };
    }

    public static CadastrarOrdemServicoUseCaseDto CriarCadastrarOrdemServicoUseCaseDtoDescricaoLonga()
    {
        return new CadastrarOrdemServicoUseCaseDto
        {
            ClienteId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ServicoId = Guid.NewGuid(),
            Descricao = "Reparo completo do sistema de freios incluindo troca de pastilhas, discos, fluido de freio, revisão das mangueiras e calibração do sistema ABS. Também será realizada a verificação do sistema de direção hidráulica e alinhamento das rodas."
        };
    }

    public static AtualizarOrdemServicoUseCaseDto CriarAtualizarOrdemServicoUseCaseDtoValido()
    {
        return new AtualizarOrdemServicoUseCaseDto
        {
            ClienteId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ServicoId = Guid.NewGuid(),
            Descricao = "Descrição atualizada do serviço",
            Status = StatusOrdemServico.EmExecucao
        };
    }

    public static AtualizarOrdemServicoUseCaseDto CriarAtualizarOrdemServicoUseCaseDtoComCamposNulos()
    {
        return new AtualizarOrdemServicoUseCaseDto
        {
            ClienteId = null,
            VeiculoId = null,
            ServicoId = null,
            Descricao = null,
            Status = null
        };
    }

    public static AtualizarOrdemServicoUseCaseDto CriarAtualizarOrdemServicoUseCaseDtoApenasStatus()
    {
        return new AtualizarOrdemServicoUseCaseDto
        {
            Status = StatusOrdemServico.Finalizada
        };
    }

    public static CadastrarInsumoOSUseCaseDto CriarCadastrarInsumoOSUseCaseDtoValido()
    {
        return new CadastrarInsumoOSUseCaseDto
        {
            EstoqueId = Guid.NewGuid(),
            Quantidade = 2
        };
    }

    public static CadastrarInsumoOSUseCaseDto CriarCadastrarInsumoOSUseCaseDtoQuantidadeAlta()
    {
        return new CadastrarInsumoOSUseCaseDto
        {
            EstoqueId = Guid.NewGuid(),
            Quantidade = 50
        };
    }

    public static CadastrarInsumoOSUseCaseDto CriarCadastrarInsumoOSUseCaseDtoQuantidadeMinima()
    {
        return new CadastrarInsumoOSUseCaseDto
        {
            EstoqueId = Guid.NewGuid(),
            Quantidade = 1
        };
    }

    public static List<CadastrarOrdemServicoUseCaseDto> CriarListaCadastrarOrdemServicoUseCaseDto()
    {
        return new List<CadastrarOrdemServicoUseCaseDto>
        {
            CriarCadastrarOrdemServicoUseCaseDtoValido(),
            CriarCadastrarOrdemServicoUseCaseDtoSemDescricao(),
            CriarCadastrarOrdemServicoUseCaseDtoDescricaoLonga()
        };
    }

    public static List<AtualizarOrdemServicoUseCaseDto> CriarListaAtualizarOrdemServicoUseCaseDto()
    {
        return new List<AtualizarOrdemServicoUseCaseDto>
        {
            CriarAtualizarOrdemServicoUseCaseDtoValido(),
            CriarAtualizarOrdemServicoUseCaseDtoComCamposNulos(),
            CriarAtualizarOrdemServicoUseCaseDtoApenasStatus()
        };
    }

    public static List<CadastrarInsumoOSUseCaseDto> CriarListaCadastrarInsumoOSUseCaseDto()
    {
        return new List<CadastrarInsumoOSUseCaseDto>
        {
            CriarCadastrarInsumoOSUseCaseDtoValido(),
            CriarCadastrarInsumoOSUseCaseDtoQuantidadeAlta(),
            CriarCadastrarInsumoOSUseCaseDtoQuantidadeMinima()
        };
    }
}
