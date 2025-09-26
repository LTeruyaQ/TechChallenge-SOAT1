using Core.DTOs.UseCases.Servico;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Servicos.EditarServico
{
    public class EditarServicoHandler : UseCasesHandlerAbstrato<EditarServicoHandler>, IEditarServicoHandler
    {
        private readonly IServicoGateway _servicoGateway;

        public EditarServicoHandler(
            IServicoGateway servicoGateway,
            ILogServicoGateway<EditarServicoHandler> logServicoGateway,
            IUnidadeDeTrabalhoGateway udtGateway,
            IUsuarioLogadoServicoGateway usuarioLogadoServicoGateway)
            : base(logServicoGateway, udtGateway, usuarioLogadoServicoGateway)
        {
            _servicoGateway = servicoGateway ?? throw new ArgumentNullException(nameof(servicoGateway));
        }

        public async Task<EditarServicoResponse> Handle(Guid id, EditarServicoUseCaseDto request)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, new { id, request });

                // Validar dados
                if (string.IsNullOrWhiteSpace(request.Nome))
                    throw new DadosInvalidosException("Nome é obrigatório");
                
                if (string.IsNullOrWhiteSpace(request.Descricao))
                    throw new DadosInvalidosException("Descrição é obrigatória");
                
                if (request.Valor <= 0)
                    throw new DadosInvalidosException("Valor deve ser maior que zero");

                var servico = await _servicoGateway.ObterPorIdAsync(id)
                    ?? throw new DadosNaoEncontradosException("Serviço não encontrado");

                servico.Nome = request.Nome;
                servico.Descricao = request.Descricao;
                servico.Valor = request.Valor.Value;
                servico.Disponivel = request.Disponivel.Value;
                servico.DataAtualizacao = DateTime.UtcNow;

                await _servicoGateway.EditarAsync(servico);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao editar serviço");

                var response = new EditarServicoResponse { Servico = servico };
                LogFim(metodo, response);
                return response;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
