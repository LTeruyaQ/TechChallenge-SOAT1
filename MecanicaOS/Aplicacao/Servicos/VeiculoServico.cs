using Aplicacao.DTOs.Veiculo;
using Aplicacao.Interfaces;
using Aplicacao.Servicos.Abstrato;
using Dominio.Entidades;
using Dominio.Especificacoes;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Services;
using System.Numerics;

namespace Aplicacao.Servicos
{
    public class VeiculoServico : ServicoAbstrato<VeiculoServico>, IVeiculoServico
    {
        private readonly ICrudRepositorio<Veiculo> _repositorio;

        public VeiculoServico(ILogServico<VeiculoServico> logServico, ICrudRepositorio<Veiculo> repositorio, IUnidadeDeTrabalho uot) : base(logServico, uot)
        {
            _repositorio = repositorio;
        }

        public async Task AtualizarAsync(Guid id, EditarVeiculoDto veiculoDto)
        {
            string metodo = nameof(AtualizarAsync);

            try
            {
                LogInicio(metodo, new { id, veiculoDto });

                var veiculo = await ObterPorIdAsync(id);

                veiculo.Placa = veiculoDto.Placa ?? veiculo.Placa;
                veiculo.Marca = veiculoDto.Marca ?? veiculo.Marca;
                veiculo.Modelo = veiculoDto.Modelo ?? veiculo.Modelo;
                veiculo.Cor = veiculoDto.Cor ?? veiculo.Cor;
                veiculo.Ano = veiculoDto.Ano ?? veiculo.Ano;
                veiculo.Anotacoes = veiculoDto.Anotacoes ?? veiculo.Anotacoes;
                veiculo.ClienteId = veiculoDto.ClienteId ?? veiculo.ClienteId;
                veiculo.DataAtualizacao = DateTime.UtcNow;

                await _repositorio.EditarAsync(veiculo);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao atualizar veiculo");

                LogFim(metodo);
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
        public async Task<Veiculo> CadastrarAsync(CadastrarVeiculoDto veiculoDto)
        {
            string metodo = nameof(CadastrarAsync);

            try
            {
                LogInicio(metodo, veiculoDto);

                var veiculo = new Veiculo
                {
                    Placa = veiculoDto.Placa,
                    Marca = veiculoDto.Marca,
                    Modelo = veiculoDto.Modelo,
                    Cor = veiculoDto.Cor,
                    Ano = veiculoDto.Ano,
                    Anotacoes = veiculoDto.Anotacoes,
                    ClienteId = veiculoDto.ClienteId,
                };

                var entidade = await _repositorio.CadastrarAsync(veiculo);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao cadastrar veiculo");

                LogFim(metodo, entidade);

                return entidade;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<Veiculo> ObterPorIdAsync(Guid id)
        {
            string metodo = nameof(ObterPorIdAsync);

            try
            {
                LogInicio(metodo, id);

                var veiculo = await _repositorio.ObterPorIdAsync(id) ?? throw new EntidadeNaoEncontradaException($"Veículo com ID {id} não encontrado.");

                LogFim(metodo, veiculo);

                return veiculo;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<IEnumerable<Veiculo>> ObterPorClienteAsync(Guid clienteId)
        {
            string metodo = nameof(ObterPorClienteAsync);

            try
            {
                LogInicio(metodo, clienteId);

                ObterVeiculoPorClienteEspecificacao filtro = new(clienteId);

                var veiculos = await _repositorio.ObterPorFiltroAsync(filtro) ?? throw new EntidadeNaoEncontradaException($"Cliente não possui nenhum veículo.");

                LogFim(metodo, veiculos);

                return veiculos;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<Veiculo> ObterPorPlacaAsync(string placa)
        {
            string metodo = nameof(ObterPorPlacaAsync);

            try
            {
                LogInicio(metodo, placa);

                ObterVeiculoPorPlacaEspecificacao filtro = new(placa);
                var veiculo = await _repositorio.ObterPorFiltroAsync(filtro) ?? throw new EntidadeNaoEncontradaException($"Veículo com placa {placa} não encontrado.");                    

                LogFim(metodo, veiculo);

                return veiculo.FirstOrDefault();
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<IEnumerable<Veiculo>> ObterTodosAsync()
        {
            string metodo = nameof(ObterTodosAsync);

            try
            {
                LogInicio(metodo);

                var veiculos = await _repositorio.ObterTodosAsync();

                LogFim(metodo, veiculos);

                return veiculos;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
        public async Task RemoverAsync(Guid id)
        {
            string metodo = nameof(RemoverAsync);

            try
            {
                LogInicio(metodo, id);

                var veiculo = await ObterPorIdAsync(id);
                await _repositorio.DeletarAsync(veiculo);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao remover veiculo");

                LogFim(metodo);
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}