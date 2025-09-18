using Core.Entidades;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.Clientes;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Clientes.ObterClientePorDocumento
{
    public class ObterClientePorDocumentoHandler : UseCasesAbstrato<ObterClientePorDocumentoHandler>, IObterClientePorDocumentoHandler
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

        public async Task<ObterClientePorDocumentoResponse> Handle(string documento)
        {
            const string metodo = nameof(Handle);
            LogInicio(metodo, documento);

            try
            {
                if (string.IsNullOrEmpty(documento))
                    throw new DadosInvalidosException("Deve ser informado o documento do usuario do cliente");

                if (await _clienteGateway.ObterClientePorDocumentoAsync(documento) is Cliente cliente)
                {
                    LogFim(metodo, cliente);
                    return new ObterClientePorDocumentoResponse { Cliente = cliente };
                }

                throw new DadosNaoEncontradosException($"Cliente de documento {documento} não encontrado");
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
