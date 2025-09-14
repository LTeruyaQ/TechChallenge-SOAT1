using Core.DTOs.Repositories.Cliente;
using Core.Entidades;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;

namespace Adapters.Gateways
{
    public class EnderecoGateway : IEnderecoGateway
    {
        private readonly IRepositorio<EnderecoRepositoryDto> _repositorioEndereco;

        public EnderecoGateway(IRepositorio<EnderecoRepositoryDto> repositorioEndereco)
        {
            _repositorioEndereco = repositorioEndereco;
        }

        public async Task CadastrarAsync(Endereco endereco)
        {
            await _repositorioEndereco.CadastrarAsync(ToDto(endereco));
        }

        public async Task EditarAsync(Endereco endereco)
        {
            await _repositorioEndereco.EditarAsync(ToDto(endereco));
        }

        public async Task<Endereco?> ObterPorIdAsync(Guid enderecoId)
        {
            var enderecoDto = await _repositorioEndereco.ObterPorIdAsync(enderecoId);
            return enderecoDto != null ? FromDto(enderecoDto) : null;
        }

        private static EnderecoRepositoryDto ToDto(Endereco endereco)
        {
            return new EnderecoRepositoryDto
            {
                Id = endereco.Id,
                Ativo = endereco.Ativo,
                DataCadastro = endereco.DataCadastro,
                DataAtualizacao = endereco.DataAtualizacao,
                Bairro = endereco.Bairro,
                CEP = endereco.CEP,
                Cidade = endereco.Cidade,
                Complemento = endereco.Complemento,
                Numero = endereco.Numero,
                Rua = endereco.Rua,
                IdCliente = endereco.IdCliente
            };
        }

        private static Endereco FromDto(EnderecoRepositoryDto dto)
        {
            return new Endereco
            {
                Id = dto.Id,
                Ativo = dto.Ativo,
                DataCadastro = dto.DataCadastro,
                DataAtualizacao = dto.DataAtualizacao,
                Bairro = dto.Bairro,
                CEP = dto.CEP,
                Cidade = dto.Cidade,
                Complemento = dto.Complemento,
                Numero = dto.Numero,
                Rua = dto.Rua,
                IdCliente = dto.IdCliente
            };
        }
    }
}
