using Core.DTOs.UseCases.Cliente;
using Core.Entidades;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;
using Core.UseCases.Abstrato;

namespace Core.UseCases
{
    public class ClienteUseCases : UseCasesAbstrato<ClienteUseCases, Cliente>, IClienteUseCases
    {
        private readonly IClienteGateway _clienteGateway;
        private readonly IEnderecoGateway _enderecoGateway;
        private readonly IContatoGateway _contatoGateway;

        public ClienteUseCases(
            IClienteGateway clienteGateway,
            IEnderecoGateway enderecoGateway,
            IContatoGateway contatoGateway,
            ILogServico<ClienteUseCases> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _clienteGateway = clienteGateway ?? throw new ArgumentNullException(nameof(clienteGateway));
            _contatoGateway = contatoGateway ?? throw new ArgumentNullException(nameof(contatoGateway));
            _enderecoGateway = enderecoGateway ?? throw new ArgumentNullException(nameof(enderecoGateway));
        }

        public async Task<Cliente> AtualizarUseCaseAsync(Guid id, AtualizarClienteUseCaseDto request)
        {
            string metodo = nameof(AtualizarUseCaseAsync);

            try
            {
                LogInicio(metodo, new { id, request });

                var cliente = await _clienteGateway.ObterPorIdAsync(id)
                    ?? throw new DadosNaoEncontradosException("cliente não encontrado");

                cliente.Atualizar(request.Nome, request.Sexo, request.TipoCliente, request.DataNascimento);

                await _clienteGateway.EditarAsync(cliente);

                if (!request.EnderecoId.Equals(Guid.Empty))
                    await AtualizarEnderecoCliente(request);

                if (!request.ContatoId.Equals(Guid.Empty))
                    await AtualizarContatoCliente(request);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao atualizar cliente");

                LogFim(metodo, cliente);

                return cliente;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        private async Task AtualizarEnderecoCliente(AtualizarClienteUseCaseDto enderecoCliente)
        {
            if (enderecoCliente.EnderecoId.Equals(Guid.Empty))
                throw new DadosInvalidosException("Deve ser informado o id do endereço a ser editado");

            if (await _enderecoGateway.ObterPorIdAsync(enderecoCliente.EnderecoId) is Endereco endereco)
            {
                endereco.Bairro = enderecoCliente.Bairro;
                endereco.Numero = enderecoCliente.Numero;
                endereco.CEP = enderecoCliente.CEP;
                endereco.Cidade = enderecoCliente.Cidade;
                endereco.Complemento = enderecoCliente.Complemento;
                endereco.Rua = enderecoCliente.Rua;
                endereco.MarcarComoAtualizada();

                await _enderecoGateway.EditarAsync(endereco);
            }
            else
            {
                throw new DadosNaoEncontradosException("Endereço inexistente");
            }
        }

        private async Task AtualizarContatoCliente(AtualizarClienteUseCaseDto contatoCliente)
        {
            if (contatoCliente.ContatoId.Equals(Guid.Empty))
                throw new Exception("Contato inexistente");

            if (await _contatoGateway.ObterPorIdAsync(contatoCliente.ContatoId) is Contato contato)
            {
                contato.Telefone = contatoCliente.Telefone;
                contato.IdCliente = contatoCliente.Id.Value;
                contato.Email = contatoCliente.Email;
                contato.MarcarComoAtualizada();

                await _contatoGateway.EditarAsync(contato);
            }
        }

        private async Task CadastrarEnderecoCliente(Guid clienteId, CadastrarClienteUseCaseDto enderecoCliente)
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

            await _enderecoGateway.CadastrarAsync(endereco);
        }

        private async Task CadastrarContatoCliente(Guid clienteId, CadastrarClienteUseCaseDto contatoCliente)
        {
            Contato contato = new()
            {
                IdCliente = clienteId,
                Email = contatoCliente.Email,
                Telefone = contatoCliente.Telefone
            };

            await _contatoGateway.CadastrarAsync(contato);
        }

        public async Task<Cliente> CadastrarUseCaseAsync(CadastrarClienteUseCaseDto request)
        {
            string metodo = nameof(CadastrarUseCaseAsync);

            try
            {
                LogInicio(metodo, request);

                Cliente cliente = new()
                {
                    Nome = request.Nome,
                    Sexo = request.Sexo,
                    Documento = request.Documento,
                    DataNascimento = request.DataNascimento,
                    TipoCliente = request.TipoCliente
                };

                var entityCliente = await _clienteGateway.CadastrarAsync(cliente);

                if (!entityCliente.Id.Equals(Guid.Empty))
                {
                    await CadastrarEnderecoCliente(entityCliente.Id, request);
                    await CadastrarContatoCliente(entityCliente.Id, request);
                }

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao cadastrar cliente");

                LogFim(metodo, cliente);

                return cliente;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<Cliente> ObterPorIdUseCaseAsync(Guid id)
        {
            string metodo = nameof(ObterPorIdUseCaseAsync);

            try
            {
                LogInicio(metodo, id);

                var clienteComVeiculo = await _clienteGateway.ObterPorIdAsync(id);

                if (clienteComVeiculo == null)
                {
                    // Tenta buscar o cliente com veículo
                    clienteComVeiculo = await _clienteGateway.ObterClienteComVeiculoPorIdAsync(id);
                }

                LogFim(metodo, clienteComVeiculo);

                return clienteComVeiculo;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<IEnumerable<Cliente>> ObterTodosUseCaseAsync()
        {
            string metodo = nameof(ObterTodosUseCaseAsync);

            try
            {
                LogInicio(metodo);

                var clientesComVeiculos = await _clienteGateway.ObterTodosClienteComVeiculoAsync();

                LogFim(metodo, clientesComVeiculos);

                return clientesComVeiculos;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<bool> RemoverUseCaseAsync(Guid id)
        {
            string metodo = nameof(RemoverUseCaseAsync);

            try
            {
                LogInicio(metodo, id);

                var veiculo = await _clienteGateway.ObterPorIdAsync(id)
                    ?? throw new DadosNaoEncontradosException("Cliente não encontrado");

                await _clienteGateway.DeletarAsync(veiculo);
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

        public async Task<Cliente> ObterPorDocumentoUseCaseAsync(string documento)
        {
            const string metodo = nameof(ObterPorDocumentoUseCaseAsync);
            LogInicio(metodo, documento);

            try
            {
                if (string.IsNullOrEmpty(documento))
                    throw new DadosInvalidosException("Deve ser informado o documento do usuario do cliente");

                if (await _clienteGateway.ObterClientePorDocumentoAsync(documento) is Cliente cliente)
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