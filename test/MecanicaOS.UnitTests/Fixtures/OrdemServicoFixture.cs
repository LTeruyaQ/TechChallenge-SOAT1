using Core.Entidades;
using Core.DTOs.Entidades.OrdemServicos;
using Core.DTOs.Entidades.Cliente;
using Core.DTOs.Entidades.Veiculo;
using Core.DTOs.Entidades.Servico;
using Core.Enumeradores;

namespace MecanicaOS.UnitTests.Fixtures;

public static class OrdemServicoFixture
{
    public static OrdemServico CriarOrdemServicoValida()
    {
        return new OrdemServico
        {
            ClienteId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ServicoId = Guid.NewGuid(),
            Descricao = "Troca de óleo e filtros",
            Status = StatusOrdemServico.Recebida,
            Orcamento = 150.00m,
            DataEnvioOrcamento = DateTime.Now.AddDays(-2),
            InsumosOS = new List<InsumoOS>()
        };
    }

    public static OrdemServico CriarOrdemServicoEmExecucao()
    {
        return new OrdemServico
        {
            ClienteId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ServicoId = Guid.NewGuid(),
            Descricao = "Revisão completa do veículo",
            Status = StatusOrdemServico.EmExecucao,
            Orcamento = 850.00m,
            DataEnvioOrcamento = DateTime.Now.AddDays(-1),
            InsumosOS = new List<InsumoOS>()
        };
    }

    public static OrdemServico CriarOrdemServicoFinalizada()
    {
        return new OrdemServico
        {
            ClienteId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ServicoId = Guid.NewGuid(),
            Descricao = "Alinhamento e balanceamento",
            Status = StatusOrdemServico.Finalizada,
            Orcamento = 120.00m,
            DataEnvioOrcamento = DateTime.Now.AddDays(-5),
            InsumosOS = new List<InsumoOS>()
        };
    }

    public static OrdemServico CriarOrdemServicoCancelada()
    {
        return new OrdemServico
        {
            ClienteId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ServicoId = Guid.NewGuid(),
            Descricao = "Reparo de transmissão - cancelado pelo cliente",
            Status = StatusOrdemServico.Cancelada,
            Orcamento = 2500.00m,
            DataEnvioOrcamento = DateTime.Now.AddDays(-10),
            InsumosOS = new List<InsumoOS>()
        };
    }

    public static OrdemServico CriarOrdemServicoSemOrcamento()
    {
        return new OrdemServico
        {
            ClienteId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ServicoId = Guid.NewGuid(),
            Descricao = "Diagnóstico inicial",
            Status = StatusOrdemServico.EmDiagnostico,
            InsumosOS = new List<InsumoOS>()
        };
    }

    public static OrdemServico CriarOrdemServicoComInsumos()
    {
        var ordemServico = CriarOrdemServicoValida();
        var insumos = new List<InsumoOS>
        {
            new InsumoOS
            {
                OrdemServicoId = ordemServico.Id,
                EstoqueId = Guid.NewGuid(),
                Quantidade = 1
            },
            new InsumoOS
            {
                OrdemServicoId = ordemServico.Id,
                EstoqueId = Guid.NewGuid(),
                Quantidade = 2
            }
        };
        
        ordemServico.InsumosOS = insumos;
        return ordemServico;
    }

    public static OrdemServicoEntityDto CriarOrdemServicoEntityDtoValido()
    {
        var clienteDto = ClienteFixture.CriarClienteEntityDtoValido();
        var veiculoDto = VeiculoFixture.CriarVeiculoEntityDtoValido();
        var servicoDto = ServicoFixture.CriarServicoEntityDtoValido();
        
        return new OrdemServicoEntityDto
        {
            Id = Guid.NewGuid(),
            ClienteId = clienteDto.Id,
            Cliente = clienteDto,
            VeiculoId = veiculoDto.Id,
            Veiculo = veiculoDto,
            ServicoId = servicoDto.Id,
            Servico = servicoDto,
            Descricao = "Manutenção preventiva completa",
            Status = StatusOrdemServico.AguardandoAprovação,
            Orcamento = 450.00m,
            DataEnvioOrcamento = DateTime.Now.AddDays(-3),
            InsumosOS = new List<InsumoOSEntityDto>(),
            Ativo = true,
            DataCadastro = DateTime.Now.AddDays(-15),
            DataAtualizacao = DateTime.Now.AddDays(-1)
        };
    }

    public static OrdemServicoEntityDto CriarOrdemServicoEntityDtoComValoresPadrao()
    {
        return new OrdemServicoEntityDto
        {
            ClienteId = Guid.NewGuid(),
            Cliente = ClienteFixture.CriarClienteEntityDtoValido(),
            VeiculoId = Guid.NewGuid(),
            Veiculo = VeiculoFixture.CriarVeiculoEntityDtoValido(),
            ServicoId = Guid.NewGuid(),
            Servico = ServicoFixture.CriarServicoEntityDtoComValoresPadrao(),
            Status = StatusOrdemServico.Recebida,
            InsumosOS = new List<InsumoOSEntityDto>()
        };
    }

    public static OrdemServicoEntityDto CriarOrdemServicoEntityDtoComInsumos()
    {
        var ordemServicoDto = CriarOrdemServicoEntityDtoValido();
        var insumosDto = new List<InsumoOSEntityDto>
        {
            new InsumoOSEntityDto
            {
                Id = Guid.NewGuid(),
                OrdemServicoId = ordemServicoDto.Id,
                EstoqueId = Guid.NewGuid(),
                Estoque = EstoqueFixture.CriarEstoqueEntityDtoValido(),
                Quantidade = 2,
                Ativo = true,
                DataCadastro = DateTime.Now.AddDays(-5),
                DataAtualizacao = DateTime.Now.AddDays(-1)
            }
        };
        
        ordemServicoDto.InsumosOS = insumosDto;
        return ordemServicoDto;
    }

    public static List<OrdemServico> CriarListaOrdensServico()
    {
        return new List<OrdemServico>
        {
            CriarOrdemServicoValida(),
            CriarOrdemServicoEmExecucao(),
            CriarOrdemServicoFinalizada(),
            CriarOrdemServicoCancelada(),
            CriarOrdemServicoSemOrcamento()
        };
    }

    public static List<OrdemServicoEntityDto> CriarListaOrdemServicoEntityDto()
    {
        return new List<OrdemServicoEntityDto>
        {
            CriarOrdemServicoEntityDtoValido(),
            CriarOrdemServicoEntityDtoComInsumos(),
            new OrdemServicoEntityDto
            {
                Id = Guid.NewGuid(),
                ClienteId = Guid.NewGuid(),
                Cliente = ClienteFixture.CriarClienteEntityDtoValido(),
                VeiculoId = Guid.NewGuid(),
                Veiculo = VeiculoFixture.CriarVeiculoEntityDtoValido(),
                ServicoId = Guid.NewGuid(),
                Servico = ServicoFixture.CriarServicoEntityDtoValido(),
                Descricao = "Troca de pneus",
                Status = StatusOrdemServico.Entregue,
                Orcamento = 400.00m,
                DataEnvioOrcamento = DateTime.Now.AddDays(-7),
                InsumosOS = new List<InsumoOSEntityDto>(),
                Ativo = true,
                DataCadastro = DateTime.Now.AddDays(-20),
                DataAtualizacao = DateTime.Now.AddDays(-3)
            }
        };
    }

    public static OrdemServico CriarOrdemServicoParaAtualizacao()
    {
        return new OrdemServico
        {
            ClienteId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ServicoId = Guid.NewGuid(),
            Descricao = "Descrição original",
            Status = StatusOrdemServico.Recebida,
            Orcamento = 100.00m,
            InsumosOS = new List<InsumoOS>()
        };
    }

    public static List<OrdemServico> CriarOrdensServicoParaCliente(Guid clienteId)
    {
        return new List<OrdemServico>
        {
            new OrdemServico
            {
                ClienteId = clienteId,
                VeiculoId = Guid.NewGuid(),
                ServicoId = Guid.NewGuid(),
                Descricao = "Primeira ordem de serviço",
                Status = StatusOrdemServico.Recebida,
                InsumosOS = new List<InsumoOS>()
            },
            new OrdemServico
            {
                ClienteId = clienteId,
                VeiculoId = Guid.NewGuid(),
                ServicoId = Guid.NewGuid(),
                Descricao = "Segunda ordem de serviço",
                Status = StatusOrdemServico.EmExecucao,
                InsumosOS = new List<InsumoOS>()
            }
        };
    }
}
