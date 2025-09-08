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
            IUnidadeDeTrabalho udt,
            IMapper mapper,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(repositorio, logServico, udt, mapper, usuarioLogadoServico)
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

                _mapper.Map(request, veiculo);
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

                var veiculoExistente = await ObterPorPlacaAsync(request.Placa);
                if (veiculoExistente != null)
                    throw new DadosJaCadastradosException("Veículo com a placa informada já cadastrado.");

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

        public async Task<IEnumerable<VeiculoResponse>> ObterTodosPorClienteAsync(Guid clienteId)
        {
            string metodo = nameof(ObterTodosPorClienteAsync);

            try
            {
                LogInicio(metodo, clienteId);

                var filtro = new ObterTodosVeiculosPorClienteEspecificacao(clienteId);
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

                var filtro = new ObterVeiculosAtivosEspecificacao();
                var veiculos = await _repositorio.ListarAsync(filtro);
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

                veiculo.Ativo = false;
                veiculo.DataAtualizacao = DateTime.UtcNow;

                await _repositorio.EditarAsync(veiculo);
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