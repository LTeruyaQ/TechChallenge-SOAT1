using Core.DTOs.UseCases.Cliente;
using Core.Entidades;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.Clientes;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Clientes.CadastrarCliente
{
    public class CadastrarClienteHandler : UseCasesAbstrato<CadastrarClienteHandler>, ICadastrarClienteHandler
    {
        private readonly IClienteGateway _clienteGateway;
        private readonly IEnderecoGateway _enderecoGateway;
        private readonly IContatoGateway _contatoGateway;

        public CadastrarClienteHandler(
            IClienteGateway clienteGateway,
            IEnderecoGateway enderecoGateway,
            IContatoGateway contatoGateway,
            ILogServico<CadastrarClienteHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _clienteGateway = clienteGateway ?? throw new ArgumentNullException(nameof(clienteGateway));
            _contatoGateway = contatoGateway ?? throw new ArgumentNullException(nameof(contatoGateway));
            _enderecoGateway = enderecoGateway ?? throw new ArgumentNullException(nameof(enderecoGateway));
        }

        public async Task<CadastrarClienteResponse> Handle(CadastrarClienteUseCaseDto request)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, request);

                // Verificar se o cliente já existe com o documento informado
                var clienteExistente = await _clienteGateway.ObterClientePorDocumentoAsync(request.Documento);
                if (clienteExistente != null)
                {
                    throw new DadosJaCadastradosException("Cliente já cadastrado");
                }

                Cliente cliente = new()
                {
                    Nome = request.Nome,
                    Sexo = request.Sexo ?? (request.TipoCliente == Core.Enumeradores.TipoCliente.PessoaFisica ? "Não informado" : null),
                    Documento = request.Documento,
                    DataNascimento = request.DataNascimento,
                    TipoCliente = request.TipoCliente
                };

                var entityCliente = await _clienteGateway.CadastrarAsync(cliente);

                if (!entityCliente.Id.Equals(Guid.Empty))
                {
                    await CadastrarEnderecoCliente(entityCliente.Id, request);
                    await CadastrarContatoCliente(entityCliente.Id, request);
                }

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao cadastrar cliente");

                LogFim(metodo, cliente);

                return new CadastrarClienteResponse { Cliente = cliente };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        private async Task CadastrarEnderecoCliente(Guid clienteId, CadastrarClienteUseCaseDto enderecoCliente)
        {
            Endereco endereco = new()
            {
                Bairro = enderecoCliente.Bairro,
                Numero = enderecoCliente.Numero,
                CEP = enderecoCliente.CEP,
                Cidade = enderecoCliente.Cidade,
                Complemento = enderecoCliente.Complemento,
                Rua = enderecoCliente.Rua,
                IdCliente = clienteId
            };

            await _enderecoGateway.CadastrarAsync(endereco);
        }

        private async Task CadastrarContatoCliente(Guid clienteId, CadastrarClienteUseCaseDto contatoCliente)
        {
            Contato contato = new()
            {
                IdCliente = clienteId,
                Email = contatoCliente.Email,
                Telefone = contatoCliente.Telefone
            };

            await _contatoGateway.CadastrarAsync(contato);
        }
    }
}
