using Core.UseCases.Veiculos.ObterVeiculoPorPlaca;

namespace Core.Interfaces.Handlers.Veiculos
{
    /// <summary>
    /// Interface para o handler de obtenção de veículo por placa
    /// </summary>
    public interface IObterVeiculoPorPlacaHandler
    {
        /// <summary>
        /// Manipula a operação de obtenção de veículo por placa
        /// </summary>
        /// <param name="placa">Placa do veículo</param>
        /// <returns>Resposta contendo o veículo encontrado</returns>
        Task<ObterVeiculoPorPlacaResponse> Handle(string placa);
    }
}
