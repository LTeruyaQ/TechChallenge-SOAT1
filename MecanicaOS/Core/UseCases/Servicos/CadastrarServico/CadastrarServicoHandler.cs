using Core.DTOs.UseCases.Servico;
using Core.Entidades;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Servicos.CadastrarServico
{
    public class CadastrarServicoHandler : UseCasesAbstrato<CadastrarServicoHandler, Servico>
    {
        private readonly IServicoGateway _servicoGateway;

        public CadastrarServicoHandler(
            IServicoGateway servicoGateway,
            ILogServico<CadastrarServicoHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _servicoGateway = servicoGateway ?? throw new ArgumentNullException(nameof(servicoGateway));
        }

        public async Task<CadastrarServicoResponse> Handle(CadastrarServicoUseCaseDto request)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, request);

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

                LogFim(metodo, servico);

                return new CadastrarServicoResponse { Servico = servico };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
