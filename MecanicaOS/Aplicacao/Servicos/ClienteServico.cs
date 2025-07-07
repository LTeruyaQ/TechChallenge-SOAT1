using Aplicacao.DTOs.Requests.Veiculo;
using Aplicacao.DTOs.Responses.Estoque;
using Aplicacao.DTOs.Responses.Veiculo;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.Servicos.Abstrato;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Especificacoes;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;

namespace Aplicacao.Servicos
{
    public class ClienteServico : ServicoAbstrato<ClienteServico, Cliente>, IClienteServico
    {
        private readonly IMapper _mapper;

        public ClienteServico(
            ICrudRepositorio<Cliente> repositorio, 
            ILogServico<ClienteServico> logServico, 
            IUnidadeDeTrabalho uot,
            IMapper mapper)
            : base(repositorio, logServico, uot)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ClienteResponse> AtualizarAsync(Guid id, AtualizarClienteRequest request)
        {
            string metodo = nameof(AtualizarAsync);

            try
            {
                LogInicio(metodo, new { id, request });

                var cliente = await _repositorio.ObterPorIdAsync(id) 
                    ?? throw new RegistroNaoEncontradoException("Veículo não encontrado");

                if (request.Nome != null) cliente.Nome = request.Nome;
                if (request.Sexo != null) cliente.Sexo = request.Sexo;
                if (request.TipoCliente != null) cliente.TipoCliente = request.TipoCliente;
                if (request.Documento != null) cliente.Documento = request.Documento;
                if (request.DataNascimento != null) cliente.DataNascimento = request.DataNascimento;               
                
                cliente.DataAtualizacao = DateTime.UtcNow;

                await _repositorio.EditarAsync(cliente);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao atualizar cliente");

                var response = _mapper.Map<ClienteResponse>(cliente);
                LogFim(metodo, response);

                return response;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
        public async Task<ClienteResponse> CadastrarAsync(CadastrarClienteRequest request)
        {
            string metodo = nameof(CadastrarAsync);

            try
            {
                LogInicio(metodo, request);

                var cliente = _mapper.Map<Cliente>(request);

                await _repositorio.CadastrarAsync(cliente);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao cadastrar cliente");

                var response = _mapper.Map<ClienteResponse>(cliente);
                LogFim(metodo, response);

                return response;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<ClienteResponse> ObterPorIdAsync(Guid id)
        {
            string metodo = nameof(ObterPorIdAsync);

            try
            {
                LogInicio(metodo, id);

                var veiculo = await _repositorio.ObterPorIdAsync(id) 
                    ?? throw new RegistroNaoEncontradoException($"Cliente com ID {id} não encontrado.");

                var response = _mapper.Map<ClienteResponse>(veiculo);
                LogFim(metodo, response);

                return response;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<IEnumerable<ClienteResponse>> ObterTodosAsync()
        {
            string metodo = nameof(ObterTodosAsync);

            try
            {
                LogInicio(metodo);

                var veiculos = await _repositorio.ObterTodosAsync();
                var response = _mapper.Map<IEnumerable<ClienteResponse>>(veiculos);

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
                    ?? throw new RegistroNaoEncontradoException("Cliente não encontrado");
                
                await _repositorio.DeletarAsync(veiculo);
                var sucesso = await Commit();

                if (!sucesso)
                    throw new PersistirDadosException("Erro ao remover cliente");

                LogFim(metodo, sucesso);

                return sucesso;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public Task<IEnumerable<ClienteResponse>> ObterPorClienteAsync(Guid clienteId)
        {
            throw new NotImplementedException();
        }
    }
}