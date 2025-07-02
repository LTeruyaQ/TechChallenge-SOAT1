using Aplicacao.DTOs.Servico;
using Aplicacao.Logs.Services;
using Aplicacao.Servicos.Abstrato;
using Dominio.DTOs;
using Dominio.Entidades;
using Dominio.Especificacoes;
using Dominio.Especificacoes.Base.Interfaces;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Aplicacao.Servicos
{
    public class ServicoServico : ServicoAbstratoLog<ServicoServico>, IServicoServico
    {
        private readonly ICrudRepositorio<Servico> _repositorio;

        public ServicoServico(ICorrelationIdService correlationIdLog,
            ILogger<ServicoServico> logger, ICrudRepositorio<Servico> repositorio) : base(correlationIdLog, logger)
        {
            _repositorio = repositorio;
        }

        public async Task<Servico> CadastrarServico(CadastrarServicoDto cadastrarServico)
        {
            var metodo = nameof(CadastrarServico);
            try
            {
                LogInicio(metodo, cadastrarServico);

                Servico servico = new Servico()
                {
                    Descricao = cadastrarServico.Descricao,
                    Nome = cadastrarServico.Nome,
                    Disponivel = cadastrarServico.Disponivel,
                    DataCadastro = DateTime.Now
                };

                var entidade = await _repositorio.CadastrarAsync(servico);

                LogFim(metodo);

                return entidade;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task DeletarServico(Guid id)
        {
            var metodo = nameof(DeletarServico);
            try
            {
                LogInicio(metodo, id);

                var servico = await ObterServicoPorId(id);
                await _repositorio.DeletarAsync(servico);

                LogFim(metodo);
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task EditarServico(Guid id, EditarServicoDto novoServico)
        {
            var metodo = nameof(EditarServico);

            try
            {
                LogInicio(metodo, new { id, novoServico });
                var servico = await ObterServicoPorId(id);

                if (servico.Nome != novoServico.Nome)
                    servico.Nome = novoServico.Nome;

                if (servico.Descricao != novoServico.Descricao)
                    servico.Descricao = novoServico.Descricao;

                if (servico.Valor != novoServico.Valor)
                    servico.Valor = novoServico.Valor;

                if (servico.Disponivel != novoServico.Disponivel)
                    servico.Disponivel = novoServico.Disponivel;

                servico.DataAtualizacao = DateTime.Now;

                await _repositorio.Editar(servico);

                LogFim(metodo);
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<Servico> ObterServicoPorId(Guid id)
        {
            var metodo = nameof(ObterServicoPorId);
            try
            {
                LogInicio(metodo);
                var servico = await _repositorio.ObterPorIdAsync(id);

                if (servico is null) throw new EntidadeNaoEncontradaException($"Não foi encontrado o serviço de id: {id}");

                LogFim(metodo, servico);

                return servico;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<IEnumerable<Servico>> ObterServicosPorFiltro(FiltrarServicoDto filtroDto)
        {
            var metodo = nameof(ObterServicosPorFiltro);
            try
            {
                LogInicio(metodo, filtroDto);

                IEspecificacao<Servico> filtro = new ServicoDisponivelEspecificacao();

                var result = await _repositorio.ObterPorFiltro(filtro);

                LogFim(metodo, result);

                return result;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<IEnumerable<Servico>> ObterTodos()
        {
            var metodo = nameof(ObterTodos);
            try
            {
                LogInicio(metodo);

                var result = await _repositorio.ObterTodos();

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
