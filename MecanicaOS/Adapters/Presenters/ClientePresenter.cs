using Adapters.DTOs.Requests.Cliente;
using Adapters.DTOs.Responses.Cliente;
using Adapters.DTOs.Responses.Veiculo;
using Adapters.Presenters.Interfaces;
using Core.DTOs.UseCases.Cliente;
using Core.Entidades;

namespace Adapters.Presenters
{
    public class ClientePresenter : IClientePresenter
    {
        public CadastrarClienteUseCaseDto? ParaUseCaseDto(CadastrarClienteRequest request)
        {
            if (request is null)
                return null;

            return new CadastrarClienteUseCaseDto
            {
                Nome = request.Nome,
                Sexo = request.Sexo,
                Documento = request.Documento,
                DataNascimento = request.DataNascimento,
                TipoCliente = request.TipoCliente,
                Rua = request.Rua,
                Bairro = request.Bairro,
                Cidade = request.Cidade,
                Numero = request.Numero,
                CEP = request.CEP,
                Complemento = request.Complemento,
                Email = request.Email,
                Telefone = request.Telefone
            };
        }

        public AtualizarClienteUseCaseDto? ParaUseCaseDto(AtualizarClienteRequest request)
        {
            if (request is null)
                return null;

            return new AtualizarClienteUseCaseDto
            {
                Id = request.Id,
                Nome = request.Nome,
                Sexo = request.Sexo,
                Documento = request.Documento,
                DataNascimento = request.DataNascimento,
                TipoCliente = request.TipoCliente,
                EnderecoId = request.EnderecoId,
                Rua = request.Rua,
                Bairro = request.Bairro,
                Cidade = request.Cidade,
                Numero = request.Numero,
                CEP = request.CEP,
                Complemento = request.Complemento,
                ContatoId = request.ContatoId,
                Email = request.Email,
                Telefone = request.Telefone
            };
        }

        public ClienteResponse? ParaResponse(Cliente cliente)
        {
            if (cliente is null)
                return null;

            return new ClienteResponse
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                Sexo = cliente.Sexo,
                Documento = cliente.Documento,
                DataNascimento = cliente.DataNascimento,
                TipoCliente = cliente.TipoCliente.ToString(),
                EnderecoId = cliente.Endereco.Id,
                ContatoId = cliente.Contato.Id,
                DataCadastro = cliente.DataCadastro.ToString("yyyy-MM-dd HH:mm:ss"),
                DataAtualizacao = cliente.DataAtualizacao?.ToString("yyyy-MM-dd HH:mm:ss"),
                Veiculos = cliente.Veiculos?.Select(v => new VeiculoResponse
                {
                    Id = v.Id,
                    Placa = v.Placa,
                    Marca = v.Marca,
                    Modelo = v.Modelo,
                    Cor = v.Cor,
                    Ano = v.Ano,
                    Anotacoes = v.Anotacoes,
                    ClienteId = v.ClienteId.Value,
                    DataCadastro = v.DataCadastro,
                    DataAtualizacao = v.DataAtualizacao
                })
            };
        }

        public IEnumerable<ClienteResponse?> ParaResponse(IEnumerable<Cliente> clientes)
        {
            if (clientes == null)
                return [];

            return clientes.Select(ParaResponse);
        }
    }
}
