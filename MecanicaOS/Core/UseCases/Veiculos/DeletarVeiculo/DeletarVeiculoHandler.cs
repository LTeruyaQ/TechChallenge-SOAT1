using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.Veiculos;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Veiculos.DeletarVeiculo
{
    public class DeletarVeiculoHandler : UseCasesAbstrato<DeletarVeiculoHandler>, IDeletarVeiculoHandler
    {
        private readonly IVeiculoGateway _veiculoGateway;

        public DeletarVeiculoHandler(
            IVeiculoGateway veiculoGateway,
            ILogServico<DeletarVeiculoHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _veiculoGateway = veiculoGateway ?? throw new ArgumentNullException(nameof(veiculoGateway));
        }

        public async Task<DeletarVeiculoResponse> Handle(Guid id)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, id);

                var veiculo = await _veiculoGateway.ObterPorIdAsync(id)
                    ?? throw new DadosNaoEncontradosException("Veículo não encontrado");

                await _veiculoGateway.DeletarAsync(veiculo);
                var sucesso = await Commit();

                if (!sucesso)
                    throw new PersistirDadosException("Erro ao remover veículo");

                LogFim(metodo, sucesso);

                return new DeletarVeiculoResponse { Sucesso = sucesso };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
