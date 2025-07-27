using Aplicacao.DTOs.Requests.Cliente;
using Aplicacao.DTOs.Responses.Cliente;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.Servicos.Abstrato;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Especificacoes.Cliente;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;

namespace Aplicacao.Servicos
{
    public class ClienteServico : ServicoAbstrato<ClienteServico, Cliente>, IClienteServico
    {
        private readonly IRepositorio<Endereco> _repositoryEndereco;
        private readonly IRepositorio<Contato> _repositoryContato;

        public ClienteServico(
            IRepositorio<Cliente> repositorio,
            IRepositorio<Endereco> repositoryEndereco,
            IRepositorio<Contato> repositoryContato,
            ILogServico<ClienteServico> logServico,
            IUnidadeDeTrabalho uot,
            IMapper mapper, 
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(repositorio, logServico, uot, mapper, usuarioLogadoServico)
        {
            _repositoryContato = repositoryContato ?? throw new ArgumentNullException(nameof(repositoryContato));
            _repositoryEndereco = repositoryEndereco ?? throw new ArgumentNullException(nameof(repositoryEndereco));
        }

        public async Task<ClienteResponse> AtualizarAsync(Guid id, AtualizarClienteRequest request)
        {
            string metodo = nameof(AtualizarAsync);

            try
            {
                LogInicio(metodo, new { id, request });

                var cliente = await _repositorio.ObterPorIdAsync(id)
                    ?? throw new DadosNaoEncontradosException("cliente não encontrado");

                cliente.Atualizar(request.Nome, request.Sexo, request.TipoCliente, request.DataNascimento);

                await _repositorio.EditarAsync(cliente);

                if (!request.EnderecoId.Equals(Guid.Empty))
                    await AtualizarEnderecoCliente(request);

                if (!request.ContatoId.Equals(Guid.Empty))
                    await AtualizarContatoCliente(request);

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

        private async Task AtualizarEnderecoCliente(AtualizarClienteRequest enderecoCliente)
        {
            if (enderecoCliente.EnderecoId.Equals(Guid.Empty))
                throw new DadosInvalidosException("Deve ser informado o id do endereço a ser editado");

            if (await _repositoryEndereco.ObterPorIdAsync(enderecoCliente.EnderecoId) is Endereco endereco)
            {
                endereco.Bairro = enderecoCliente.Bairro;
                endereco.Numero = enderecoCliente.Numero;
                endereco.CEP = enderecoCliente.CEP;
                endereco.Cidade = enderecoCliente.Cidade;
                endereco.Complemento = enderecoCliente.Complemento;
                endereco.Rua = enderecoCliente.Rua;
                endereco.DataAtualizacao = DateTime.UtcNow;

                await _repositoryEndereco.EditarAsync(endereco);
            }

            throw new DadosNaoEncontradosException("Endereço inexistente");
        }

        private async Task AtualizarContatoCliente(AtualizarClienteRequest contatoCliente)
        {
            if (!contatoCliente.ContatoId.Equals(Guid.Empty))
                throw new Exception("Endereço inexistente");

            if (await _repositoryContato.ObterPorIdAsync(contatoCliente.ContatoId) is Contato contato)
            {
                contato.Telefone = contatoCliente.Telefone;
                contato.IdCliente = contatoCliente.Id.Value;
                contato.Email = contatoCliente.Email;
                contato.DataAtualizacao = DateTime.UtcNow;

                await _repositoryContato.EditarAsync(contato);
            }
        }

        private async Task CadastrarEnderecoCliente(Guid clienteId, CadastrarClienteRequest enderecoCliente)
        {
            Endereco endereco = new()
            {
                Bairro = enderecoCliente.Bairro,
                Numero = enderecoCliente.Numero,
                CEP = enderecoCliente.CEP,
                Cidade = enderecoCliente.Cidade,
                Complemento = enderecoCliente.Complemento,
                Rua = enderecoCliente.Rua,
                IdCliente = clienteId
            };

            await _repositoryEndereco.CadastrarAsync(endereco);
        }

        private async Task CadastrarContatoCliente(Guid clienteId, CadastrarClienteRequest contatoCliente)
        {
            Contato contato = new()
            {
                DataCadastro = DateTime.UtcNow,
                IdCliente = clienteId,
                Email = contatoCliente.Email,
                Telefone = contatoCliente.Telefone
            };

            await _repositoryContato.CadastrarAsync(contato);
        }

        public async Task<ClienteResponse> CadastrarAsync(CadastrarClienteRequest request)
        {
            string metodo = nameof(CadastrarAsync);

            try
            {
                LogInicio(metodo, request);

                var cliente = _mapper.Map<Cliente>(request);

                var entityCliente = await _repositorio.CadastrarAsync(cliente);

                if (!entityCliente.Id.Equals(Guid.Empty))
                {
                    await CadastrarEnderecoCliente(entityCliente.Id, request);
                    await CadastrarContatoCliente(entityCliente.Id, request);
                }

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
                    ?? throw new DadosNaoEncontradosException($"Cliente com ID {id} não encontrado.");

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

        public async Task<bool> RemoverAsync(Guid id)
        {
            string metodo = nameof(RemoverAsync);

            try
            {
                LogInicio(metodo, id);

                var veiculo = await _repositorio.ObterPorIdAsync(id)
                    ?? throw new DadosNaoEncontradosException("Cliente não encontrado");

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

        public async Task<Cliente> ObterPorDocumento(string documento)
        {
            const string metodo = nameof(ObterPorDocumento);
            LogInicio(metodo, documento);

            try
            {
                if (string.IsNullOrEmpty(documento))
                    throw new DadosInvalidosException("Deve ser informado o documento do usuario do cliente");

                if (await _repositorio.ObterUmPorFiltroAsync(new ObterClientePorDocumento(documento)) is Cliente cliente)
                {
                    LogFim(metodo, cliente);
                    return cliente;
                }

                throw new DadosNaoEncontradosException($"Cliente de documento {documento} não encontrado");
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}