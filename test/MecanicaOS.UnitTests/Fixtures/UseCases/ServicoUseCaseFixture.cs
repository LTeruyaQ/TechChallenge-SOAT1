using Core.DTOs.UseCases.Servico;

namespace MecanicaOS.UnitTests.Fixtures.UseCases;

public static class ServicoUseCaseFixture
{
    public static CadastrarServicoUseCaseDto CriarCadastrarServicoUseCaseDtoValido()
    {
        return new CadastrarServicoUseCaseDto
        {
            Nome = "Troca de Óleo Completa",
            Descricao = "Troca de óleo do motor com filtro e verificação de fluidos",
            Valor = 120.00m,
            Disponivel = true
        };
    }

    public static CadastrarServicoUseCaseDto CriarCadastrarServicoUseCaseDtoCaroIndisponivel()
    {
        return new CadastrarServicoUseCaseDto
        {
            Nome = "Reparo de Transmissão",
            Descricao = "Reparo completo da caixa de transmissão automática",
            Valor = 2500.00m,
            Disponivel = false
        };
    }

    public static CadastrarServicoUseCaseDto CriarCadastrarServicoUseCaseDtoGratuito()
    {
        return new CadastrarServicoUseCaseDto
        {
            Nome = "Diagnóstico Inicial",
            Descricao = "Diagnóstico básico para identificação do problema",
            Valor = 0.00m,
            Disponivel = true
        };
    }

    public static EditarServicoUseCaseDto CriarEditarServicoUseCaseDtoValido()
    {
        return new EditarServicoUseCaseDto
        {
            Nome = "Alinhamento e Balanceamento",
            Descricao = "Alinhamento e balanceamento das quatro rodas",
            Valor = 80.00m,
            Disponivel = true
        };
    }

    public static EditarServicoUseCaseDto CriarEditarServicoUseCaseDtoComValoresNulos()
    {
        return new EditarServicoUseCaseDto
        {
            Nome = "Serviço Teste",
            Descricao = "Descrição Teste",
            Valor = null,
            Disponivel = null
        };
    }

    public static EditarServicoUseCaseDto CriarEditarServicoUseCaseDtoIndisponivel()
    {
        return new EditarServicoUseCaseDto
        {
            Nome = "Serviço Temporariamente Indisponível",
            Descricao = "Este serviço está temporariamente indisponível",
            Valor = 150.00m,
            Disponivel = false
        };
    }

    public static List<CadastrarServicoUseCaseDto> CriarListaCadastrarServicoUseCaseDto()
    {
        return new List<CadastrarServicoUseCaseDto>
        {
            CriarCadastrarServicoUseCaseDtoValido(),
            CriarCadastrarServicoUseCaseDtoCaroIndisponivel(),
            CriarCadastrarServicoUseCaseDtoGratuito()
        };
    }

    public static List<EditarServicoUseCaseDto> CriarListaEditarServicoUseCaseDto()
    {
        return new List<EditarServicoUseCaseDto>
        {
            CriarEditarServicoUseCaseDtoValido(),
            CriarEditarServicoUseCaseDtoComValoresNulos(),
            CriarEditarServicoUseCaseDtoIndisponivel()
        };
    }
}
