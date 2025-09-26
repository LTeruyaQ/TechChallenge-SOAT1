using Core.DTOs.UseCases.Servico;
using Core.Entidades;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Servicos.CadastrarServico
{
    public class CadastrarServicoHandler : UseCasesHandlerAbstrato<CadastrarServicoHandler>, ICadastrarServicoHandler
    {
        private readonly IServicoGateway _servicoGateway;

        public CadastrarServicoHandler(
            IServicoGateway servicoGateway,
            ILogServicoGateway<CadastrarServicoHandler> logServicoGateway,
            IUnidadeDeTrabalhoGateway udtGateway,
            IUsuarioLogadoServicoGateway usuarioLogadoServicoGateway)
            : base(logServicoGateway, udtGateway, usuarioLogadoServicoGateway)
        {
            _servicoGateway = servicoGateway ?? throw new ArgumentNullException(nameof(servicoGateway));
        }

        public async Task<CadastrarServicoResponse> Handle(CadastrarServicoUseCaseDto request)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, request);

                if (string.IsNullOrWhiteSpace(request.Nome))
                    throw new DadosInvalidosException("Nome é obrigatório");
                
                if (string.IsNullOrWhiteSpace(request.Descricao))
                    throw new DadosInvalidosException("Descrição é obrigatória");
                
                if (request.Valor <= 0)
                    throw new DadosInvalidosException("Valor deve ser maior que zero");

                var servicoExistente = await _servicoGateway.ObterServicosDisponiveisPorNomeAsync(request.Nome);
                if (servicoExistente != null)
                    throw new DadosJaCadastradosException("Já existe um serviço cadastrado com este nome");

                Servico servico = new()
                {
                    Nome = request.Nome,
                    Descricao = request.Descricao,
                    Valor = request.Valor,
                    Disponivel = request.Disponivel
                };

                await _servicoGateway.CadastrarAsync(servico);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao cadastrar serviço");

                var response = new CadastrarServicoResponse { Servico = servico };
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
