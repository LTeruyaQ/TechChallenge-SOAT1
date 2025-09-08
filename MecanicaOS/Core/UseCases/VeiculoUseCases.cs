using Core.DTOs.Veiculo;
using Core.Entidades;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;
using Core.UseCases.Abstrato;

namespace Core.UseCases
{
    public class VeiculoUseCases : UseCasesAbstrato<VeiculoUseCases, Veiculo>, IVeiculoUseCases
    {
        private readonly IVeiculoGateway _veiculoGateway;
        public VeiculoUseCases(
            ILogServico<VeiculoUseCases> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico,
            IVeiculoGateway veiculoGateway)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _veiculoGateway = veiculoGateway;
        }

        public async Task<Veiculo> AtualizarUseCaseAsync(Guid id, AtualizarVeiculoUseCaseDto request)
        {
            string metodo = nameof(AtualizarUseCaseAsync);

            try
            {
                LogInicio(metodo, new { id, request });

                var veiculo = await _veiculoGateway.ObterPorIdAsync(id)
                    ?? throw new DadosNaoEncontradosException("Veículo não encontrado");

                if (request.Placa != null) veiculo.Placa = request.Placa;
                if (request.Marca != null) veiculo.Marca = request.Marca;
                if (request.Modelo != null) veiculo.Modelo = request.Modelo;
                if (request.Cor != null) veiculo.Cor = request.Cor;
                if (request.Ano != null) veiculo.Ano = request.Ano;
                if (request.Anotacoes != null) veiculo.Anotacoes = request.Anotacoes;
                if (request.ClienteId.HasValue) veiculo.ClienteId = request.ClienteId.Value;

                veiculo.DataAtualizacao = DateTime.UtcNow;

                await _veiculoGateway.EditarAsync(veiculo);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao atualizar veículo");

                LogFim(metodo, veiculo);

                return veiculo;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
        public async Task<Veiculo> CadastrarUseCaseAsync(CadastrarVeiculoUseCaseDto request)
        {
            string metodo = nameof(CadastrarUseCaseAsync);

            try
            {
                LogInicio(metodo, request);

                Veiculo veiculo = new ()
                {
                    Ano = request.Ano,
                    Cor = request.Cor,
                    Marca = request.Marca,
                    Modelo = request.Modelo,
                    Placa = request.Placa,
                    Anotacoes = request.Anotacoes,
                    ClienteId = request.ClienteId,
                };

                await _veiculoGateway.CadastrarAsync(veiculo);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao cadastrar veículo");

                LogFim(metodo, veiculo);

                return veiculo;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<Veiculo> ObterPorIdUseCaseAsync(Guid id)
        {
            string metodo = nameof(ObterPorIdUseCaseAsync);

            try
            {
                LogInicio(metodo, id);

                var veiculo = await _veiculoGateway.ObterPorIdAsync(id)
                    ?? throw new DadosNaoEncontradosException($"Veículo com ID {id} não encontrado.");

                LogFim(metodo, veiculo);

                return veiculo;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<IEnumerable<Veiculo>> ObterPorClienteUseCaseAsync(Guid clienteId)
        {
            string metodo = nameof(ObterPorClienteUseCaseAsync);

            try
            {
                LogInicio(metodo, clienteId);

                var veiculos = await _veiculoGateway.ObterVeiculoPorClienteAsync(clienteId);
                LogFim(metodo, veiculos);

                return veiculos;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<Veiculo?> ObterPorPlacaUseCaseAsync(string placa)
        {
            string metodo = nameof(ObterPorPlacaUseCaseAsync);

            try
            {
                LogInicio(metodo, placa);

                var veiculos = await _veiculoGateway.ObterVeiculoPorPlacaAsync(placa);

                var veiculo = veiculos.FirstOrDefault();

                LogFim(metodo, veiculo);

                return veiculo;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<IEnumerable<Veiculo>> ObterTodosUseCaseAsync()
        {
            string metodo = nameof(ObterTodosUseCaseAsync);

            try
            {
                LogInicio(metodo);

                var veiculos = await _veiculoGateway.ObterTodosAsync();

                LogFim(metodo, veiculos);

                return veiculos;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
        public async Task<bool> DeletarUseCaseAsync(Guid id)
        {
            string metodo = nameof(DeletarUseCaseAsync);

            try
            {
                LogInicio(metodo, id);

                var veiculo = await _veiculoGateway.ObterPorIdAsync(id)
                    ?? throw new DadosNaoEncontradosException("Veículo não encontrado");

                await _veiculoGateway.DeletarAsync(veiculo);
                var sucesso = await Commit();

                if (!sucesso)
                    throw new PersistirDadosException("Erro ao remover veículo");

                LogFim(metodo, sucesso);

                return sucesso;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}