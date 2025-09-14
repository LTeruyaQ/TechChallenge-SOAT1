using Core.DTOs.Repositories.Veiculo;
using Core.Entidades;
using Core.Especificacoes.Veiculo;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;

namespace Adapters.Gateways
{
    public class VeiculoGateway : IVeiculoGateway
    {
        private readonly IRepositorio<VeiculoRepositoryDto> _repositorioVeiculo;

        public VeiculoGateway(IRepositorio<VeiculoRepositoryDto> repositorioVeiculo)
        {
            _repositorioVeiculo = repositorioVeiculo;
        }

        public async Task CadastrarAsync(object veiculo)
        {
            await _repositorioVeiculo.CadastrarAsync(ToDto((Veiculo)veiculo));
        }

        public async Task DeletarAsync(Veiculo veiculo)
        {
            await _repositorioVeiculo.DeletarAsync(ToDto(veiculo));
        }

        public async Task EditarAsync(object veiculo)
        {
            await _repositorioVeiculo.EditarAsync(ToDto((Veiculo)veiculo));
        }

        public async Task<Veiculo?> ObterPorIdAsync(Guid id)
        {
            var dto = await _repositorioVeiculo.ObterPorIdAsync(id);
            return dto != null ? FromDto(dto) : null;
        }

        public async Task<IEnumerable<Veiculo>> ObterTodosAsync()
        {
            var dtos = await _repositorioVeiculo.ObterTodosAsync();
            return dtos.Select(FromDto);
        }

        public async Task<IEnumerable<Veiculo>> ObterVeiculoPorClienteAsync(Guid clienteId)
        {
            var especificacao = new ObterVeiculoPorClienteEspecificacao(clienteId);
            var dtos = await _repositorioVeiculo.ListarAsync(especificacao);
            return dtos.Select(FromDto);
        }

        public async Task<IEnumerable<Veiculo>> ObterVeiculoPorPlacaAsync(string placa)
        {
            var especificacao = new ObterVeiculoPorPlacaEspecificacao(placa);
            var dtos = await _repositorioVeiculo.ListarAsync(especificacao);
            return dtos.Select(FromDto);
        }

        private static VeiculoRepositoryDto ToDto(Veiculo veiculo)
        {
            return new VeiculoRepositoryDto
            {
                Id = veiculo.Id,
                Ativo = veiculo.Ativo,
                DataCadastro = veiculo.DataCadastro,
                DataAtualizacao = veiculo.DataAtualizacao,
                Placa = veiculo.Placa,
                Marca = veiculo.Marca,
                Modelo = veiculo.Modelo,
                Cor = veiculo.Cor,
                Ano = veiculo.Ano,
                Anotacoes = veiculo.Anotacoes,
                ClienteId = veiculo.ClienteId
            };
        }

        private static Veiculo FromDto(VeiculoRepositoryDto dto)
        {
            return new Veiculo
            {
                Id = dto.Id,
                Ativo = dto.Ativo,
                DataCadastro = dto.DataCadastro,
                DataAtualizacao = dto.DataAtualizacao,
                Placa = dto.Placa,
                Marca = dto.Marca,
                Modelo = dto.Modelo,
                Cor = dto.Cor,
                Ano = dto.Ano,
                Anotacoes = dto.Anotacoes,
                ClienteId = dto.ClienteId
            };
        }
    }
}
