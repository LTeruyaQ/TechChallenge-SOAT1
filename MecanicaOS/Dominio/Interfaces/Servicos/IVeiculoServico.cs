using Dominio.DTOs.Veiculo;
using Dominio.Entidades;

namespace Dominio.Interfaces.Servicos
{
    public interface IVeiculoServico
    {
        Task AtualizarAsync(Guid id, EditarVeiculoDto veiculoDto);
        Task<Veiculo> CadastrarAsync(CadastrarVeiculoDto veiculoDto);
        Task RemoverAsync(Guid id);
        Task<IEnumerable<Veiculo>> ObterPorClienteAsync(Guid clienteId);
        Task<Veiculo> ObterPorIdAsync(Guid id);
        Task<Veiculo> ObterPorPlacaAsync(string placa);
        Task<IEnumerable<Veiculo>> ObterTodosAsync();
    }
}