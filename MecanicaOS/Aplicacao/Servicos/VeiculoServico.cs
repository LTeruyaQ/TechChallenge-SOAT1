using Aplicacao.DTOs.Veiculo;
using Aplicacao.Logs.Services;
using Aplicacao.Veiculos.Abstrato;
using Dominio.DTOs;
using Dominio.Entidades;
using Dominio.Especificacoes;
using Dominio.Especificacoes.Base.Interfaces;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Aplicacao.Veiculos
{
    public class VeiculoVeiculo : VeiculoAbstratoLog<VeiculoVeiculo>, IVeiculoVeiculo
    {
        private readonly ICrudRepositorio<Veiculo> _repositorio;

        public VeiculoVeiculo(ICorrelationIdService correlationIdLog,
            ILogger<VeiculoVeiculo> logger, ICrudRepositorio<Veiculo> repositorio) : base(correlationIdLog, logger)
        {
            _repositorio = repositorio;
        }

        public async Task<Veiculo> CadastrarVeiculo(CadastrarVeiculoDto cadastrarVeiculo)
        {
            var metodo = nameof(CadastrarVeiculo);
            try
            {
                LogInicio(metodo, cadastrarVeiculo);

                Veiculo Veiculo = new Veiculo()
                {
                    Descricao = cadastrarVeiculo.Descricao,
                    Nome = cadastrarVeiculo.Nome,
                    Disponivel = cadastrarVeiculo.Disponivel,
                    Valor = cadastrarVeiculo.Valor
                };

                var entidade = await _repositorio.CadastrarAsync(Veiculo);

                LogFim(metodo);

                return entidade;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task DeletarVeiculo(Guid id)
        {
            var metodo = nameof(DeletarVeiculo);
            try
            {
                LogInicio(metodo, id);

                var Veiculo = await ObterVeiculoPorId(id);
                await _repositorio.DeletarAsync(Veiculo);

                LogFim(metodo);
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task EditarVeiculo(Guid id, EditarVeiculoDto novoVeiculo)
        {
            var metodo = nameof(EditarVeiculo);

            try
            {
                LogInicio(metodo, new { id, novoVeiculo });
                var Veiculo = await ObterVeiculoPorId(id);

                if (Veiculo.Nome != novoVeiculo.Nome)
                    Veiculo.Nome = novoVeiculo.Nome;

                if (Veiculo.Descricao != novoVeiculo.Descricao)
                    Veiculo.Descricao = novoVeiculo.Descricao;

                if (Veiculo.Valor != novoVeiculo.Valor)
                    Veiculo.Valor = novoVeiculo.Valor;

                if (Veiculo.Disponivel != novoVeiculo.Disponivel)
                    Veiculo.Disponivel = novoVeiculo.Disponivel;

                Veiculo.DataAtualizacao = DateTime.UtcNow;

                await _repositorio.Editar(Veiculo);

                LogFim(metodo);
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<Veiculo> ObterVeiculoPorId(Guid id)
        {
            var metodo = nameof(ObterVeiculoPorId);
            try
            {
                LogInicio(metodo);
                var Veiculo = await _repositorio.ObterPorIdAsync(id);

                if (Veiculo is null) throw new EntidadeNaoEncontradaException($"Não foi encontrado o serviço de id: {id}");

                LogFim(metodo, Veiculo);

                return Veiculo;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<IEnumerable<Veiculo>> ObterVeiculosPorFiltro(FiltrarVeiculoDto filtroDto)
        {
            var metodo = nameof(ObterVeiculosPorFiltro);
            try
            {
                LogInicio(metodo, filtroDto);

                IEspecificacao<Veiculo> filtro = new VeiculoDisponivelEspecificacao();

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

        public async Task<IEnumerable<Veiculo>> ObterTodos()
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
