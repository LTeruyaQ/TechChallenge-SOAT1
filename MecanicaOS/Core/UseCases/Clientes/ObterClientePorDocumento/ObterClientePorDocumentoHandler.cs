using Core.Entidades;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Clientes.ObterClientePorDocumento
{
    public class ObterClientePorDocumentoHandler : UseCasesAbstrato<ObterClientePorDocumentoHandler, Cliente>
    {
        private readonly IClienteGateway _clienteGateway;

        public ObterClientePorDocumentoHandler(
            IClienteGateway clienteGateway,
            ILogServico<ObterClientePorDocumentoHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _clienteGateway = clienteGateway ?? throw new ArgumentNullException(nameof(clienteGateway));
        }

        public async Task<ObterClientePorDocumentoResponse> Handle(ObterClientePorDocumentoUseCase query)
        {
            const string metodo = nameof(Handle);
            LogInicio(metodo, query.Documento);

            try
            {
                if (string.IsNullOrEmpty(query.Documento))
                    throw new DadosInvalidosException("Deve ser informado o documento do usuario do cliente");

                if (await _clienteGateway.ObterClientePorDocumentoAsync(query.Documento) is Cliente cliente)
                {
                    LogFim(metodo, cliente);
                    return new ObterClientePorDocumentoResponse { Cliente = cliente };
                }

                throw new DadosNaoEncontradosException($"Cliente de documento {query.Documento} não encontrado");
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
