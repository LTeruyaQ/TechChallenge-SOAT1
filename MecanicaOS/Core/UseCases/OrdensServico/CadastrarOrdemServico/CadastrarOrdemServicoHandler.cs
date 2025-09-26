using Core.DTOs.UseCases.OrdemServico;
using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.OrdensServico;
using Core.Interfaces.UseCases;
using Core.UseCases.Abstrato;

namespace Core.UseCases.OrdensServico.CadastrarOrdemServico
{
    public class CadastrarOrdemServicoHandler : UseCasesHandlerAbstrato<CadastrarOrdemServicoHandler>, ICadastrarOrdemServicoHandler
    {
        private readonly IOrdemServicoGateway _ordemServicoGateway;
        private readonly IClienteUseCases _clienteUseCases;
        private readonly IServicoUseCases _servicoUseCases;

        public CadastrarOrdemServicoHandler(
            IOrdemServicoGateway ordemServicoGateway,
            IClienteUseCases clienteUseCases,
            IServicoUseCases servicoUseCases,
            ILogGateway<CadastrarOrdemServicoHandler> logServicoGateway,
            IUnidadeDeTrabalhoGateway udtGateway,
            IUsuarioLogadoServicoGateway usuarioLogadoServicoGateway)
            : base(logServicoGateway, udtGateway, usuarioLogadoServicoGateway)
        {
            _ordemServicoGateway = ordemServicoGateway ?? throw new ArgumentNullException(nameof(ordemServicoGateway));
            _clienteUseCases = clienteUseCases ?? throw new ArgumentNullException(nameof(clienteUseCases));
            _servicoUseCases = servicoUseCases ?? throw new ArgumentNullException(nameof(servicoUseCases));
        }

        public async Task<OrdemServico> Handle(CadastrarOrdemServicoUseCaseDto request)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, request);

                var cliente = await _clienteUseCases.ObterPorIdUseCaseAsync(request.ClienteId)
                    ?? throw new DadosNaoEncontradosException("Cliente não encontrado");

                var servico = await _servicoUseCases.ObterServicoPorIdUseCaseAsync(request.ServicoId)
                    ?? throw new DadosNaoEncontradosException("Serviço não encontrado");

                var ordemServico = new OrdemServico
                {
                    ClienteId = request.ClienteId,
                    VeiculoId = request.VeiculoId,
                    ServicoId = request.ServicoId,
                    Descricao = request.Descricao,
                    Status = StatusOrdemServico.Recebida,
                    DataCadastro = DateTime.UtcNow,
                    Cliente = cliente,
                    Servico = servico
                };

                await _ordemServicoGateway.CadastrarAsync(ordemServico);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao cadastrar ordem de serviço");

                LogFim(metodo, ordemServico);

                return ordemServico;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
