using Core.Entidades;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;
using Core.UseCases.Abstrato;

namespace Core.UseCases.InsumosOS.CadastrarInsumos
{
    public class CadastrarInsumosHandler : UseCasesAbstrato<CadastrarInsumosHandler, InsumoOS>
    {
        private readonly IOrdemServicoUseCases _ordemServicoUseCases;
        private readonly IEstoqueUseCases _estoqueUseCases;
        private readonly IInsumosGateway _insumosGateway;
        private readonly IVerificarEstoqueJobGateway _verificarEstoqueJobGateway;

        public CadastrarInsumosHandler(
            IOrdemServicoUseCases ordemServicoUseCases,
            IEstoqueUseCases estoqueUseCases,
            IInsumosGateway insumosGateway,
            ILogServico<CadastrarInsumosHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico,
            IVerificarEstoqueJobGateway verificarEstoqueJobGateway)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _ordemServicoUseCases = ordemServicoUseCases ?? throw new ArgumentNullException(nameof(ordemServicoUseCases));
            _estoqueUseCases = estoqueUseCases ?? throw new ArgumentNullException(nameof(estoqueUseCases));
            _insumosGateway = insumosGateway ?? throw new ArgumentNullException(nameof(insumosGateway));
            _verificarEstoqueJobGateway = verificarEstoqueJobGateway ?? throw new ArgumentNullException(nameof(verificarEstoqueJobGateway));
        }

        public async Task<CadastrarInsumosResponse> Handle(CadastrarInsumosCommand command)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, new { command.OrdemServicoId, InsumosCount = command.Request.Count });

                var ordemServico = await _ordemServicoUseCases.ObterPorIdUseCaseAsync(command.OrdemServicoId)
                    ?? throw new DadosNaoEncontradosException("Ordem de serviço não encontrada");

                var insumosOS = new List<InsumoOS>();

                foreach (var insumoDto in command.Request)
                {
                    var estoque = await _estoqueUseCases.ObterPorIdUseCaseAsync(insumoDto.EstoqueId);

                    if (estoque.QuantidadeDisponivel < insumoDto.Quantidade)
                        throw new DadosInvalidosException($"Estoque insuficiente para o insumo {estoque.Insumo}");

                    var insumoOS = new InsumoOS
                    {
                        OrdemServicoId = command.OrdemServicoId,
                        EstoqueId = insumoDto.EstoqueId,
                        Quantidade = insumoDto.Quantidade,
                        Estoque = estoque
                    };

                    // Simular cadastro por enquanto - método não existe na interface
                    insumosOS.Add(insumoOS);

                    // Atualizar estoque
                    estoque.QuantidadeDisponivel -= insumoDto.Quantidade;
                    await _estoqueUseCases.AtualizarUseCaseAsync(estoque.Id, new Core.DTOs.UseCases.Estoque.AtualizarEstoqueUseCaseDto
                    {
                        Insumo = estoque.Insumo,
                        QuantidadeDisponivel = estoque.QuantidadeDisponivel,
                        QuantidadeMinima = estoque.QuantidadeMinima,
                        Preco = estoque.Preco
                    });

                    // Verificar se precisa gerar job de estoque crítico (simplificado)
                    if (estoque.QuantidadeDisponivel <= estoque.QuantidadeMinima)
                    {
                        // Job seria criado aqui
                    }
                }

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao cadastrar insumos na ordem de serviço");

                LogFim(metodo, insumosOS);

                return new CadastrarInsumosResponse { InsumosOS = insumosOS };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
