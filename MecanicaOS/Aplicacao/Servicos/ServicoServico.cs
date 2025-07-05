using Aplicacao.Servicos.Abstrato;
using Dominio.DTOs.Servico;
using Dominio.Entidades;
using Dominio.Especificacoes;
using Dominio.Especificacoes.Base.Extensoes;
using Dominio.Especificacoes.Base.Interfaces;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;

namespace Aplicacao.Servicos
{
    public class ServicoServico : ServicoAbstrato<ServicoServico, Servico>, IServicoServico
    {
        public ServicoServico(ICrudRepositorio<Servico> repositorio, ILogServico<ServicoServico> logServico, IUnidadeDeTrabalho uot)
            : base(repositorio, logServico, uot)
        {
        }

        public async Task<Servico> CadastrarServicoAsync(CadastrarServicoDto cadastrarServico)
        {
            var metodo = nameof(CadastrarServicoAsync);
            try
            {
                LogInicio(metodo, cadastrarServico);

                if (await ObterServicoPorNomeAsync(cadastrarServico.Nome) is Servico servicoJaCadastrado)
                    throw new RegistroJaCadastradoException("Serviço já cadastrado");

                Servico servico = new Servico()
                {
                    Descricao = cadastrarServico.Descricao,
                    Nome = cadastrarServico.Nome,
                    Disponivel = cadastrarServico.Disponivel,
                    Valor = cadastrarServico.Valor
                };

                var entidade = await _repositorio.CadastrarAsync(servico);

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

        public async Task<Servico> ObterServicoPorNomeAsync(string nome)
        {
            var metodo = nameof(ObterServicosDisponiveisAsync);
            try
            {
                LogInicio(metodo);

                IEspecificacao<Servico> filtro = new ObterServicoPorNomeEspecificacao(nome);
                filtro.E(new ObterServicoDisponivelEspecificacao());

                var result = await _repositorio.ObterPorFiltroAsync(filtro);

                LogFim(metodo, result);

                return result.SingleOrDefault();
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task DeletarServicoAsync(Guid id)
        {
            var metodo = nameof(DeletarServicoAsync);
            try
            {
                LogInicio(metodo, id);

                var servico = await ObterServicoPorIdAsync(id);
                await _repositorio.DeletarAsync(servico);

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

        public async Task EditarServicoAsync(Guid id, EditarServicoDto novoServico)
        {
            var metodo = nameof(EditarServicoAsync);

            try
            {
                LogInicio(metodo, new { id, novoServico });
                var servico = await ObterServicoPorIdAsync(id);

                if (servico.Nome != novoServico.Nome)
                    servico.Nome = novoServico.Nome;

                if (servico.Descricao != novoServico.Descricao)
                    servico.Descricao = novoServico.Descricao;

                if (servico.Valor != novoServico.Valor)
                    servico.Valor = novoServico.Valor;

                if (servico.Disponivel != novoServico.Disponivel)
                    servico.Disponivel = novoServico.Disponivel;

                servico.DataAtualizacao = DateTime.UtcNow;

                await _repositorio.EditarAsync(servico);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao atualizar serviço");

                LogFim(metodo);
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<Servico> ObterServicoPorIdAsync(Guid id)
        {
            var metodo = nameof(ObterServicoPorIdAsync);
            try
            {
                LogInicio(metodo);

                var servico = await _repositorio.ObterPorIdAsync(id) ?? throw new EntidadeNaoEncontradaException($"Não foi encontrado o serviço de id: {id}");

                LogFim(metodo, servico);

                return servico;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<IEnumerable<Servico>> ObterServicosDisponiveisAsync()
        {
            var metodo = nameof(ObterServicosDisponiveisAsync);
            try
            {
                LogInicio(metodo);

                IEspecificacao<Servico> filtro = new ObterServicoDisponivelEspecificacao();

                var result = await _repositorio.ObterPorFiltroAsync(filtro);

                LogFim(metodo, result);

                return result;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<IEnumerable<Servico>> ObterTodosAsync()
        {
            var metodo = nameof(ObterTodosAsync);
            try
            {
                LogInicio(metodo);

                var result = await _repositorio.ObterTodosAsync();

                LogFim(metodo, result);

                return result;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
