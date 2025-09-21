using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.Veiculos;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Veiculos.ObterVeiculo
{
    public class ObterVeiculoHandler : UseCasesHandlerAbstrato<ObterVeiculoHandler>, IObterVeiculoHandler
    {
        private readonly IVeiculoGateway _veiculoGateway;

        public ObterVeiculoHandler(
            IVeiculoGateway veiculoGateway,
            ILogServico<ObterVeiculoHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _veiculoGateway = veiculoGateway ?? throw new ArgumentNullException(nameof(veiculoGateway));
        }

        public async Task<ObterVeiculoResponse> Handle(Guid id)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, id);

                var veiculo = await _veiculoGateway.ObterPorIdAsync(id)
                    ?? throw new DadosNaoEncontradosException($"Veículo com ID {id} não encontrado.");

                LogFim(metodo, veiculo);

                return new ObterVeiculoResponse { Veiculo = veiculo };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
