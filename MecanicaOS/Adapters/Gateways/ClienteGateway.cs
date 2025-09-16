using Core.DTOs.Entidades.Cliente;
using Core.Entidades;
using Core.Especificacoes.Cliente;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;

namespace Adapters.Gateways
{
    public class ClienteGateway : IClienteGateway
    {
        private readonly IRepositorio<ClienteEntityDto> _repositorioCliente;

        public ClienteGateway(IRepositorio<ClienteEntityDto> repositorioCliente)
        {
            _repositorioCliente = repositorioCliente;
        }

        public async Task<Cliente> CadastrarAsync(Cliente cliente)
        {
            var response = await _repositorioCliente.CadastrarAsync(ToDto(cliente));
            return FromDto(response);
        }

        public async Task DeletarAsync(Cliente cliente)
        {
            await _repositorioCliente.DeletarAsync(ToDto(cliente));
        }

        public async Task EditarAsync(Cliente cliente)
        {
            await _repositorioCliente.EditarAsync(ToDto(cliente));
        }

        public async Task<Cliente?> ObterClienteComVeiculoPorIdAsync(Guid clienteId)
        {
            var especificacao = new ObterClienteComVeiculoPorIdEspecificacao(clienteId);
            return await _repositorioCliente.ObterUmProjetadoSemRastreamentoAsync<Cliente>(especificacao);
        }

        public async Task<Cliente?> ObterClientePorDocumentoAsync(string documento)
        {
            var especificacao = new ObterClientePorDocumento(documento);
            return await _repositorioCliente.ObterUmProjetadoSemRastreamentoAsync<Cliente>(especificacao);
        }

        public async Task<Cliente?> ObterPorIdAsync(Guid id)
        {
            var clienteDto = await _repositorioCliente.ObterPorIdAsync(id);
            return clienteDto != null ? FromDto(clienteDto) : null;
        }

        public async Task<IEnumerable<Cliente>> ObterTodosClienteComVeiculoAsync()
        {
            var especificacao = new ObterTodosClienteComVeiculoEspecificacao();
            return await _repositorioCliente.ListarProjetadoAsync<Cliente>(especificacao);
        }

        private static ClienteEntityDto ToDto(Cliente cliente)
        {
            return new ClienteEntityDto
            {
                Id = cliente.Id,
                Ativo = cliente.Ativo,
                DataCadastro = cliente.DataCadastro,
                DataAtualizacao = cliente.DataAtualizacao,
                Nome = cliente.Nome,
                Documento = cliente.Documento,
                Sexo = cliente.Sexo,
                DataNascimento = cliente.DataNascimento,
                TipoCliente = cliente.TipoCliente,
                Contato = new ContatoEntityDto
                {
                    Id = cliente.Contato.Id,
                    Ativo = cliente.Contato.Ativo,
                    DataCadastro = cliente.Contato.DataCadastro,
                    DataAtualizacao = cliente.Contato.DataAtualizacao,
                    Email = cliente.Contato.Email,
                    Telefone = cliente.Contato.Telefone,
                    IdCliente = cliente.Id
                },
                Endereco = new EnderecoEntityDto
                {
                    Id = cliente.Endereco.Id,
                    Ativo = cliente.Endereco.Ativo,
                    DataCadastro = cliente.Endereco.DataCadastro,
                    DataAtualizacao = cliente.Endereco.DataAtualizacao,
                    Rua = cliente.Endereco.Rua,
                    Numero = cliente.Endereco.Numero,
                    Complemento = cliente.Endereco.Complemento,
                    Bairro = cliente.Endereco.Bairro,
                    Cidade = cliente.Endereco.Cidade,
                    CEP = cliente.Endereco.CEP,
                    IdCliente = cliente.Id
                }
            };
        }

        private static Cliente FromDto(ClienteEntityDto dto)
        {
            return new Cliente
            {
                Id = dto.Id,
                Ativo = dto.Ativo,
                DataCadastro = dto.DataCadastro,
                DataAtualizacao = dto.DataAtualizacao,
                Nome = dto.Nome,
                Documento = dto.Documento,
                Sexo = dto.Sexo,
                DataNascimento = dto.DataNascimento,
                TipoCliente = dto.TipoCliente,
                Contato = new Contato
                {
                    Id = dto.Contato.Id,
                    Ativo = dto.Contato.Ativo,
                    DataCadastro = dto.Contato.DataCadastro,
                    DataAtualizacao = dto.Contato.DataAtualizacao,
                    Email = dto.Contato.Email,
                    Telefone = dto.Contato.Telefone,
                    IdCliente = dto.Id
                },
                Endereco = new Endereco
                {
                    Id = dto.Endereco.Id,
                    Ativo = dto.Endereco.Ativo,
                    DataCadastro = dto.Endereco.DataCadastro,
                    DataAtualizacao = dto.Endereco.DataAtualizacao,
                    Rua = dto.Endereco.Rua,
                    Numero = dto.Endereco.Numero,
                    Complemento = dto.Endereco.Complemento,
                    Bairro = dto.Endereco.Bairro,
                    Cidade = dto.Endereco.Cidade,
                    CEP = dto.Endereco.CEP,
                    IdCliente = dto.Id
                },
                Veiculos = dto.Veiculos?.Select(v => new Veiculo
                {
                    Id = v.Id,
                    Ativo = v.Ativo,
                    DataCadastro = v.DataCadastro,
                    DataAtualizacao = v.DataAtualizacao,
                    Marca = v.Marca,
                    Modelo = v.Modelo,
                    Ano = v.Ano,
                    Placa = v.Placa,
                    ClienteId = v.ClienteId
                }) ?? Enumerable.Empty<Veiculo>()
            };
        }
    }
}
