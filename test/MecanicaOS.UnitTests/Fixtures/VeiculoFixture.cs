using Core.DTOs.Entidades.Veiculo;
using Core.Entidades;

namespace MecanicaOS.UnitTests.Fixtures;

public class VeiculoFixture : BaseFixture
{
    public static Veiculo CriarVeiculoValido()
    {
        var veiculo = CriarEntidadeComCamposObrigatorios<Veiculo>();
        veiculo.Placa = "ABC-1234";
        veiculo.Marca = "Toyota";
        veiculo.Modelo = "Corolla";
        veiculo.Ano = "2020";
        veiculo.Cor = "Branco";
        veiculo.ClienteId = ValidGuid;

        return veiculo;
    }

    public static Veiculo CriarVeiculoComDadosInvalidos()
    {
        var veiculo = CriarEntidadeComCamposObrigatorios<Veiculo>();
        veiculo.Placa = ""; // Placa vazia - inválida
        veiculo.Marca = ""; // Marca vazia - inválida
        veiculo.Modelo = ""; // Modelo vazio - inválido
        veiculo.Ano = "1800"; // Ano inválido
        veiculo.Cor = "";
        veiculo.ClienteId = Guid.Empty; // ClienteId inválido

        return veiculo;
    }

    public static VeiculoEntityDto CriarVeiculoEntityDtoValido()
    {
        var dto = CriarRepositoryDtoComCamposObrigatorios<VeiculoEntityDto>();
        dto.Placa = "ABC-1234";
        dto.Marca = "Toyota";
        dto.Modelo = "Corolla";
        dto.Ano = "2020";
        dto.Cor = "Branco";
        dto.ClienteId = ValidGuid;

        return dto;
    }

    public static List<Veiculo> CriarListaVeiculosValidos(int quantidade = 3)
    {
        var veiculos = new List<Veiculo>();

        for (int i = 0; i < quantidade; i++)
        {
            var veiculo = CriarVeiculoValido();
            veiculo.Id = Guid.NewGuid();
            veiculo.Placa = $"ABC-123{i}";
            veiculo.Modelo = $"Modelo {i + 1}";
            veiculos.Add(veiculo);
        }

        return veiculos;
    }
}
