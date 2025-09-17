using Core.UseCases.Veiculos.DeletarVeiculo;

namespace Core.Interfaces.Handlers.Veiculos
{
    /// <summary>
    /// Interface para o handler de deleção de veículos
    /// </summary>
    public interface IDeletarVeiculoHandler
    {
        /// <summary>
        /// Manipula a operação de deleção de veículo
        /// </summary>
        /// <param name="id">ID do veículo a ser deletado</param>
        /// <returns>Resposta indicando o sucesso da operação</returns>
        Task<DeletarVeiculoResponse> Handle(Guid id);
    }
}
