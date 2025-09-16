using Core.Entidades;
using Core.DTOs.Entidades.Servico;

namespace MecanicaOS.UnitTests.Fixtures;

public static class ServicoFixture
{
    public static Servico CriarServicoValido()
    {
        return new Servico
        {
            Nome = "Troca de Óleo",
            Descricao = "Troca completa do óleo do motor com filtro",
            Valor = 120.00m,
            Disponivel = true
        };
    }

    public static Servico CriarServicoComValorAlto()
    {
        return new Servico
        {
            Nome = "Revisão Completa",
            Descricao = "Revisão geral do veículo com todos os itens",
            Valor = 850.00m,
            Disponivel = true
        };
    }

    public static Servico CriarServicoIndisponivel()
    {
        return new Servico
        {
            Nome = "Reparo de Transmissão",
            Descricao = "Reparo completo da caixa de transmissão",
            Valor = 2500.00m,
            Disponivel = false
        };
    }

    public static Servico CriarServicoGratuito()
    {
        return new Servico
        {
            Nome = "Diagnóstico Básico",
            Descricao = "Diagnóstico inicial do problema",
            Valor = 0.00m,
            Disponivel = true
        };
    }

    public static ServicoEntityDto CriarServicoEntityDtoValido()
    {
        return new ServicoEntityDto
        {
            Id = Guid.NewGuid(),
            Nome = "Alinhamento e Balanceamento",
            Descricao = "Alinhamento e balanceamento das rodas",
            Valor = 80.00m,
            Disponivel = true,
            Ativo = true,
            DataCadastro = DateTime.Now.AddDays(-10),
            DataAtualizacao = DateTime.Now.AddDays(-1)
        };
    }

    public static ServicoEntityDto CriarServicoEntityDtoComValoresPadrao()
    {
        return new ServicoEntityDto
        {
            Nome = "Serviço Teste",
            Descricao = "Descrição de teste"
        };
    }

    public static List<Servico> CriarListaServicos()
    {
        return new List<Servico>
        {
            CriarServicoValido(),
            CriarServicoComValorAlto(),
            CriarServicoIndisponivel(),
            CriarServicoGratuito()
        };
    }

    public static List<ServicoEntityDto> CriarListaServicoEntityDto()
    {
        return new List<ServicoEntityDto>
        {
            CriarServicoEntityDtoValido(),
            new ServicoEntityDto
            {
                Id = Guid.NewGuid(),
                Nome = "Troca de Pneus",
                Descricao = "Troca dos quatro pneus do veículo",
                Valor = 400.00m,
                Disponivel = true,
                Ativo = true,
                DataCadastro = DateTime.Now.AddDays(-5),
                DataAtualizacao = DateTime.Now
            }
        };
    }
}
