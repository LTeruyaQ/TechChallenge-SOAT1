using Aplicacao.DTOs.Veiculo;
using Aplicacao.Logs.Services;
using Aplicacao.Servicos.Abstrato;
using Dominio.Entidades;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Aplicacao.Servicos
{
    public class VeiculoServico : ServicoAbstratoLog<VeiculoServico>, IVeiculoServico
    {
        private readonly ICrudRepositorio<Veiculo> _repositorio;

        public VeiculoServico(
            ICorrelationIdService correlationIdLog,
            ILogger<VeiculoServico> logger,
            ICrudRepositorio<Veiculo> repositorio) : base(correlationIdLog, logger)
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
                veiculo.Data_Atualizacao = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

                await _repositorio.Editar(veiculo);

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
                    Data_Cadastro = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    Data_Atualizacao = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                };

                var entidade = await _repositorio.CadastrarAsync(veiculo);

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

                var veiculo = await _repositorio.ObterPorIdAsync(id);

                if (veiculo is null)
                    throw new EntidadeNaoEncontradaException($"Veículo com ID {id} não encontrado.");

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

                var veiculos = await _repositorio.ObterPorClienteAsync(clienteId);

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

                var veiculo = await _repositorio.ObterPorPlacaAsync(placa);

                if (veiculo is null)
                    throw new EntidadeNaoEncontradaException($"Veículo com placa {placa} não encontrado.");

                LogFim(metodo, veiculo);

                return veiculo;
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

                var veiculos = await _repositorio.ObterTodos();

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