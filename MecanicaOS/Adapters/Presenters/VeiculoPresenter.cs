using Adapters.Presenters.Interfaces;
using Aplicacao.DTOs.Requests.Veiculo;
using Core.DTOs.Veiculo;

namespace Adapters.Presenters
{
    public class VeiculoPresenter : IVeiculoPresenter
    {
        public CadastrarVeiculoUseCaseDto ParaUseCaseDto(CadastrarVeiculoRequest request)
        {
            if (request == null)
                return null;

            return new CadastrarVeiculoUseCaseDto
            {
                Placa = request.Placa,
                Marca = request.Marca,
                Modelo = request.Modelo,
                Cor = request.Cor,
                Ano = request.Ano,
                Anotacoes = request.Anotacoes,
                ClienteId = request.ClienteId
            };
        }

        public AtualizarVeiculoUseCaseDto ParaUseCaseDto(AtualizarVeiculoRequest request)
        {
            if (request == null)
                return null;

            return new AtualizarVeiculoUseCaseDto
            {
                Placa = request.Placa,
                Marca = request.Marca,
                Modelo = request.Modelo,
                Cor = request.Cor,
                Ano = request.Ano,
                Anotacoes = request.Anotacoes,
                ClienteId = request.ClienteId
            };
        }
    }
}
