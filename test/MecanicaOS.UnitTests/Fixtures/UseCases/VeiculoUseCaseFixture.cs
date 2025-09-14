using Core.DTOs.UseCases.Veiculo;

namespace MecanicaOS.UnitTests.Fixtures.UseCases;

public static class VeiculoUseCaseFixture
{
    public static CadastrarVeiculoUseCaseDto CriarCadastrarVeiculoUseCaseDtoValido()
    {
        return new CadastrarVeiculoUseCaseDto
        {
            Placa = "ABC-1234",
            Marca = "Toyota",
            Modelo = "Corolla",
            Cor = "Prata",
            Ano = "2020",
            Anotacoes = "Veículo em bom estado geral",
            ClienteId = Guid.NewGuid()
        };
    }

    public static CadastrarVeiculoUseCaseDto CriarCadastrarVeiculoUseCaseDtoSemAnotacoes()
    {
        return new CadastrarVeiculoUseCaseDto
        {
            Placa = "XYZ-9876",
            Marca = "Honda",
            Modelo = "Civic",
            Cor = "Branco",
            Ano = "2019",
            Anotacoes = null,
            ClienteId = Guid.NewGuid()
        };
    }

    public static CadastrarVeiculoUseCaseDto CriarCadastrarVeiculoUseCaseDtoVeiculoAntigo()
    {
        return new CadastrarVeiculoUseCaseDto
        {
            Placa = "OLD-1990",
            Marca = "Volkswagen",
            Modelo = "Fusca",
            Cor = "Azul",
            Ano = "1990",
            Anotacoes = "Veículo clássico - necessita cuidados especiais",
            ClienteId = Guid.NewGuid()
        };
    }

    public static CadastrarVeiculoUseCaseDto CriarCadastrarVeiculoUseCaseDtoVeiculoNovo()
    {
        return new CadastrarVeiculoUseCaseDto
        {
            Placa = "NEW-2024",
            Marca = "BMW",
            Modelo = "X3",
            Cor = "Preto",
            Ano = "2024",
            Anotacoes = "Veículo zero quilômetro - ainda na garantia",
            ClienteId = Guid.NewGuid()
        };
    }

    public static AtualizarVeiculoUseCaseDto CriarAtualizarVeiculoUseCaseDtoValido()
    {
        return new AtualizarVeiculoUseCaseDto
        {
            Placa = "UPD-5678",
            Marca = "Ford",
            Modelo = "Focus",
            Cor = "Vermelho",
            Ano = "2021",
            Anotacoes = "Anotações atualizadas do veículo",
            ClienteId = Guid.NewGuid()
        };
    }

    public static AtualizarVeiculoUseCaseDto CriarAtualizarVeiculoUseCaseDtoComCamposNulos()
    {
        return new AtualizarVeiculoUseCaseDto
        {
            Placa = null,
            Marca = null,
            Modelo = null,
            Cor = null,
            Ano = null,
            Anotacoes = null,
            ClienteId = null
        };
    }

    public static AtualizarVeiculoUseCaseDto CriarAtualizarVeiculoUseCaseDtoApenasCor()
    {
        return new AtualizarVeiculoUseCaseDto
        {
            Cor = "Verde"
        };
    }

    public static AtualizarVeiculoUseCaseDto CriarAtualizarVeiculoUseCaseDtoApenasAnotacoes()
    {
        return new AtualizarVeiculoUseCaseDto
        {
            Anotacoes = "Novas anotações importantes sobre o veículo"
        };
    }

    public static List<CadastrarVeiculoUseCaseDto> CriarListaCadastrarVeiculoUseCaseDto()
    {
        return new List<CadastrarVeiculoUseCaseDto>
        {
            CriarCadastrarVeiculoUseCaseDtoValido(),
            CriarCadastrarVeiculoUseCaseDtoSemAnotacoes(),
            CriarCadastrarVeiculoUseCaseDtoVeiculoAntigo(),
            CriarCadastrarVeiculoUseCaseDtoVeiculoNovo()
        };
    }

    public static List<AtualizarVeiculoUseCaseDto> CriarListaAtualizarVeiculoUseCaseDto()
    {
        return new List<AtualizarVeiculoUseCaseDto>
        {
            CriarAtualizarVeiculoUseCaseDtoValido(),
            CriarAtualizarVeiculoUseCaseDtoComCamposNulos(),
            CriarAtualizarVeiculoUseCaseDtoApenasCor(),
            CriarAtualizarVeiculoUseCaseDtoApenasAnotacoes()
        };
    }
}
