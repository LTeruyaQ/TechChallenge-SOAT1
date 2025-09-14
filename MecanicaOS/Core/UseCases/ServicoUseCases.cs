using Core.DTOs.UseCases.Servico;
using Core.Entidades;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;
using Core.UseCases.Abstrato;

namespace Core.UseCases
{
    public class ServicoUseCases : UseCasesAbstrato<ServicoUseCases, Servico>, IServicoUseCases
    {
        private readonly IServicoGateway _servicoGateway;
        public ServicoUseCases(
            ILogServico<ServicoUseCases> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico,
            IServicoGateway servicoGateway)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _servicoGateway = servicoGateway;
        }

        public async Task<Servico> CadastrarServicoUseCaseAsync(CadastrarServicoUseCaseDto request)
        {
            var metodo = nameof(CadastrarServicoUseCaseAsync);
            try
            {
                LogInicio(metodo, request);

                if (await ObterServicoPorNomeUseCaseAsync(request.Nome) != null)
                    throw new DadosJaCadastradosException("Serviço já cadastrado");

                Servico servico = new()
                {
                    Nome = request.Nome,
                    Valor = request.Valor,
                    Disponivel = request.Disponivel,
                    Descricao = request.Descricao
                };

                var entidade = await _servicoGateway.CadastrarAsync(servico);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao cadastrar serviço");

                LogFim(metodo, entidade);
                return entidade;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<Servico?> ObterServicoPorNomeUseCaseAsync(string nome)
        {
            var metodo = nameof(ObterServicoPorNomeUseCaseAsync);
            try
            {
                LogInicio(metodo);

                var servicosDisponiveis = await _servicoGateway.ObterServicosDisponiveisPorNomeAsync(nome);

                LogFim(metodo, servicosDisponiveis);

                return servicosDisponiveis;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task DeletarServicoUseCaseAsync(Guid id)
        {
            var metodo = nameof(DeletarServicoUseCaseAsync);
            try
            {
                LogInicio(metodo, id);

                var servico = await _servicoGateway.ObterPorIdAsync(id) ?? throw new DadosNaoEncontradosException("Serviço não encontrado");
                await _servicoGateway.DeletarAsync(servico);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao deletar serviço");

                LogFim(metodo);
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<Servico> EditarServicoUseCaseAsync(Guid id, EditarServicoUseCaseDto request)
        {
            var metodo = nameof(EditarServicoUseCaseAsync);

            try
            {
                LogInicio(metodo, new { id, request });

                var servico = await _servicoGateway.ObterPorIdAsync(id);

                servico.Atualizar(request.Nome, request.Descricao, request.Valor, request.Disponivel);

                await _servicoGateway.EditarAsync(servico);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao atualizar serviço");

                LogFim(metodo, servico);

                return servico;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<Servico> ObterServicoPorIdUseCaseAsync(Guid id)
        {
            var metodo = nameof(ObterServicoPorIdUseCaseAsync);
            try
            {
                LogInicio(metodo);

                var servico = await _servicoGateway.ObterPorIdAsync(id) ?? throw new DadosNaoEncontradosException("Serviço não encontrado");

                LogFim(metodo, servico);

                return servico;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<IEnumerable<Servico>> ObterServicosDisponiveisUseCaseAsync()
        {
            var metodo = nameof(ObterServicosDisponiveisUseCaseAsync);
            try
            {
                LogInicio(metodo);

                var servicosDisponiveis = await _servicoGateway.ObterServicoDisponivelAsync();

                LogFim(metodo, servicosDisponiveis);

                return servicosDisponiveis;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<IEnumerable<Servico>> ObterTodosUseCaseAsync()
        {
            var metodo = nameof(ObterTodosUseCaseAsync);
            try
            {
                LogInicio(metodo);

                var servicos = await _servicoGateway.ObterTodosAsync();

                LogFim(metodo, servicos);

                return servicos;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
