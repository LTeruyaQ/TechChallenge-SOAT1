using Adapters.Presenters.Interfaces;
using Aplicacao.DTOs.Requests.Cliente;
using Core.DTOs.Cliente;

namespace Adapters.Presenters
{
    public class ClientePresenter : IClientePresenter
    {
        public CadastrarClienteUseCaseDto ParaUseCaseDto(CadastrarClienteRequest request)
        {
            if (request == null)
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

        public AtualizarClienteUseCaseDto ParaUseCaseDto(AtualizarClienteRequest request)
        {
            if (request == null)
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
    }
}
