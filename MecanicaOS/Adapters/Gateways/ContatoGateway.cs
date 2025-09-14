using Core.DTOs.Repositories.Cliente;
using Core.Entidades;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;

namespace Adapters.Gateways
{
    public class ContatoGateway : IContatoGateway
    {
        private readonly IRepositorio<ContatoRepositoryDTO> _repositorioContato;

        public ContatoGateway(IRepositorio<ContatoRepositoryDTO> repositorioContato)
        {
            _repositorioContato = repositorioContato;
        }

        public async Task CadastrarAsync(Contato contato)
        {
            await _repositorioContato.CadastrarAsync(ToDto(contato));
        }

        public async Task EditarAsync(Contato contato)
        {
            await _repositorioContato.EditarAsync(ToDto(contato));
        }

        public async Task<Contato?> ObterPorIdAsync(Guid contatoId)
        {
            var contatoDto = await _repositorioContato.ObterPorIdAsync(contatoId);
            return contatoDto != null ? FromDto(contatoDto) : null;
        }

        private static ContatoRepositoryDTO ToDto(Contato contato)
        {
            return new ContatoRepositoryDTO
            {
                Id = contato.Id,
                Ativo = contato.Ativo,
                DataCadastro = contato.DataCadastro,
                DataAtualizacao = contato.DataAtualizacao,
                Email = contato.Email,
                Telefone = contato.Telefone,
                IdCliente = contato.IdCliente
            };
        }

        private static Contato FromDto(ContatoRepositoryDTO dto)
        {
            return new Contato
            {
                Id = dto.Id,
                Ativo = dto.Ativo,
                DataCadastro = dto.DataCadastro,
                DataAtualizacao = dto.DataAtualizacao,
                Email = dto.Email,
                Telefone = dto.Telefone,
                IdCliente = dto.IdCliente
            };
        }
    }
}
