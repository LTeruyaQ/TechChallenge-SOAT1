using Aplicacao.DTOs.Requests.Veiculo;
using Aplicacao.DTOs.Responses.Veiculo;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.Servicos.Abstrato;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Especificacoes.Veiculo;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;

namespace Aplicacao.Servicos
{
    public class VeiculoServico : ServicoAbstrato<VeiculoServico, Veiculo>, IVeiculoServico
    {
        public VeiculoServico(
            IRepositorio<Veiculo> repositorio,
            ILogServico<VeiculoServico> logServico,
            IUnidadeDeTrabalho uot,
            IMapper mapper,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(repositorio, logServico, uot, mapper, usuarioLogadoServico)
        {
        }

        public async Task<VeiculoResponse> AtualizarAsync(Guid id, AtualizarVeiculoRequest request)
        {
            string metodo = nameof(AtualizarAsync);

            try
            {
                LogInicio(metodo, new { id, request });

                var veiculo = await _repositorio.ObterPorIdAsync(id)
                    ?? throw new DadosNaoEncontradosException("Veículo não encontrado");

                if (request.Placa != null) veiculo.Placa = request.Placa;
                if (request.Marca != null) veiculo.Marca = request.Marca;
                if (request.Modelo != null) veiculo.Modelo = request.Modelo;
                if (request.Cor != null) veiculo.Cor = request.Cor;
                if (request.Ano != null) veiculo.Ano = request.Ano;
                if (request.Anotacoes != null) veiculo.Anotacoes = request.Anotacoes;
                if (request.ClienteId.HasValue) veiculo.ClienteId = request.ClienteId.Value;

                veiculo.DataAtualizacao = DateTime.UtcNow;

                await _repositorio.EditarAsync(veiculo);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao atualizar veículo");

                var response = _mapper.Map<VeiculoResponse>(veiculo);
                LogFim(metodo, response);

                return response;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
        public async Task<VeiculoResponse> CadastrarAsync(CadastrarVeiculoRequest request)
        {
            string metodo = nameof(CadastrarAsync);

            try
            {
                LogInicio(metodo, request);

                var veiculo = _mapper.Map<Veiculo>(request);

                await _repositorio.CadastrarAsync(veiculo);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao cadastrar veículo");

                var response = _mapper.Map<VeiculoResponse>(veiculo);
                LogFim(metodo, response);

                return response;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<VeiculoResponse> ObterPorIdAsync(Guid id)
        {
            string metodo = nameof(ObterPorIdAsync);

            try
            {
                LogInicio(metodo, id);

                var veiculo = await _repositorio.ObterPorIdAsync(id)
                    ?? throw new DadosNaoEncontradosException($"Veículo com ID {id} não encontrado.");

                var response = _mapper.Map<VeiculoResponse>(veiculo);
                LogFim(metodo, response);

                return response;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<IEnumerable<VeiculoResponse>> ObterPorClienteAsync(Guid clienteId)
        {
            string metodo = nameof(ObterPorClienteAsync);

            try
            {
                LogInicio(metodo, clienteId);

                var filtro = new ObterVeiculoPorClienteEspecificacao(clienteId);
                var veiculos = await _repositorio.ListarAsync(filtro)
                    ?? throw new DadosNaoEncontradosException("Cliente não possui nenhum veículo.");

                var response = _mapper.Map<IEnumerable<VeiculoResponse>>(veiculos);
                LogFim(metodo, response);

                return response;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<VeiculoResponse?> ObterPorPlacaAsync(string placa)
        {
            string metodo = nameof(ObterPorPlacaAsync);

            try
            {
                LogInicio(metodo, placa);

                var filtro = new ObterVeiculoPorPlacaEspecificacao(placa);
                var veiculos = await _repositorio.ListarAsync(filtro)
                    ?? throw new DadosNaoEncontradosException($"Veículo com placa {placa} não encontrado.");

                var veiculo = veiculos.FirstOrDefault();
                var response = veiculo is not null ? _mapper.Map<VeiculoResponse>(veiculo) : null;

                LogFim(metodo, response);

                return response;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<IEnumerable<VeiculoResponse>> ObterTodosAsync()
        {
            string metodo = nameof(ObterTodosAsync);

            try
            {
                LogInicio(metodo);

                var veiculos = await _repositorio.ObterTodosAsync();
                var response = _mapper.Map<IEnumerable<VeiculoResponse>>(veiculos);

                LogFim(metodo, response);

                return response;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
        public async Task<bool> DeletarAsync(Guid id)
        {
            string metodo = nameof(DeletarAsync);

            try
            {
                LogInicio(metodo, id);

                var veiculo = await _repositorio.ObterPorIdAsync(id)
                    ?? throw new DadosNaoEncontradosException("Veículo não encontrado");

                await _repositorio.DeletarAsync(veiculo);
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