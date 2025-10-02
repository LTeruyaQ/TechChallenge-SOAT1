using Core.DTOs.UseCases.OrdemServico.InsumoOS;
using Core.Entidades;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.InsumosOS;
using Core.UseCases.Abstrato;

namespace Core.UseCases.InsumosOS.CadastrarInsumos
{
    public class CadastrarInsumosHandler : UseCasesHandlerAbstrato<CadastrarInsumosHandler>, ICadastrarInsumosHandler
    {
        private readonly IVerificarEstoqueJobGateway _verificarEstoqueJobGateway;

        public CadastrarInsumosHandler(
            ILogGateway<CadastrarInsumosHandler> logServicoGateway,
            IUnidadeDeTrabalhoGateway udtGateway,
            IUsuarioLogadoServicoGateway usuarioLogadoServicoGateway,
            IVerificarEstoqueJobGateway verificarEstoqueJobGateway)
            : base(logServicoGateway, udtGateway, usuarioLogadoServicoGateway)
        {
            _verificarEstoqueJobGateway = verificarEstoqueJobGateway ?? throw new ArgumentNullException(nameof(verificarEstoqueJobGateway));
        }

        public async Task<List<InsumoOS>> Handle(Guid ordemServicoId, List<CadastrarInsumoOSUseCaseDto> request)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, new { ordemServicoId, InsumosCount = request.Count });

                List<InsumoOS> insumosOS = [];

                foreach (var insumoDto in request)
                {
                    var insumoOS = new InsumoOS
                    {
                        OrdemServicoId = ordemServicoId,
                        EstoqueId = insumoDto.EstoqueId,
                        Quantidade = insumoDto.Quantidade,
                        Estoque = insumoDto.Estoque
                    };

                    insumosOS.Add(insumoOS);

                    if (insumoDto.Estoque != null &&
                        (insumoDto.Estoque.QuantidadeDisponivel - insumoDto.Quantidade) <= insumoDto.Estoque.QuantidadeMinima)
                    {
                        await _verificarEstoqueJobGateway.VerificarEstoqueAsync();
                    }
                }

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao cadastrar insumos na ordem de serviço");

                LogFim(metodo, insumosOS);

                return insumosOS.ToList();
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
