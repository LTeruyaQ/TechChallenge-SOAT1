using Core.DTOs.UseCases.Veiculo;
using Core.UseCases.Veiculos.AtualizarVeiculo;

namespace Core.Interfaces.Handlers.Veiculos
{
    /// <summary>
    /// Interface para o handler de atualização de veículos
    /// </summary>
    public interface IAtualizarVeiculoHandler
    {
        /// <summary>
        /// Manipula a operação de atualização de veículo
        /// </summary>
        /// <param name="id">ID do veículo a ser atualizado</param>
        /// <param name="request">DTO com os dados atualizados do veículo</param>
        /// <returns>Resposta contendo o veículo atualizado</returns>
        Task<AtualizarVeiculoResponse> Handle(Guid id, AtualizarVeiculoUseCaseDto request);
    }
}
