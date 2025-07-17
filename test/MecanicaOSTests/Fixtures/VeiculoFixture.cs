using Aplicacao.DTOs.Requests.Veiculo;
using Dominio.Entidades;

namespace MecanicaOSTests.Fixtures
{
    public static class VeiculoFixture
    {
        public static CadastrarVeiculoRequest CriarCadastrarVeiculoRequestValido()
        {
            return new CadastrarVeiculoRequest
            {
                Placa = "ABC1D23",
                Marca = "Fiat",
                Modelo = "Uno",
                Ano = "2020",
                ClienteId = Guid.NewGuid()
            };
        }

        public static AtualizarVeiculoRequest CriarAtualizarVeiculoRequestValido()
        {
            return new AtualizarVeiculoRequest
            {
                Placa = "XYZ9Z99",
                Marca = "Volkswagen",
                Modelo = "Gol",
                Cor = "Preto",
                Ano = "2021",
                ClienteId = Guid.NewGuid()
            };
        }

        public static Veiculo CriarVeiculoValido()
        {
            return new Veiculo
            {
                Id = Guid.NewGuid(),
                Placa = "ABC1D23",
                Marca = "Fiat",
                Modelo = "Uno",
                Cor = "Vermelho",
                Ano = "2020",
                ClienteId = Guid.NewGuid()
            };
        }
    }
}
