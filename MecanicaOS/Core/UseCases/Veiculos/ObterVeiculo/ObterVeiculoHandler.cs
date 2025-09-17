using Core.Entidades;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Veiculos.ObterVeiculo
{
    public class ObterVeiculoHandler : UseCasesAbstrato<ObterVeiculoHandler, Veiculo>
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

        public async Task<ObterVeiculoResponse> Handle(ObterVeiculoUseCase query)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, query.Id);

                var veiculo = await _veiculoGateway.ObterPorIdAsync(query.Id)
                    ?? throw new DadosNaoEncontradosException($"Veículo com ID {query.Id} não encontrado.");

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
