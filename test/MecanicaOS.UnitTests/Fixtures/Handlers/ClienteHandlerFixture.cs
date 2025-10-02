using Adapters.Gateways;
using Core.DTOs.Entidades.Cliente;
using Core.DTOs.UseCases.Cliente;
using Core.Entidades;
using Core.Enumeradores;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.UseCases.Clientes.AtualizarCliente;
using Core.UseCases.Clientes.CadastrarCliente;
using Core.UseCases.Clientes.ObterCliente;
using Core.UseCases.Clientes.ObterClientePorDocumento;
using Core.UseCases.Clientes.ObterTodosClientes;
using Core.UseCases.Clientes.RemoverCliente;

namespace MecanicaOS.UnitTests.Fixtures.Handlers
{
    public class ClienteHandlerFixture
    {
        // Repositórios mockados
        public IRepositorio<ClienteEntityDto> RepositorioCliente { get; }
        public IRepositorio<EnderecoEntityDto> RepositorioEndereco { get; }
        public IRepositorio<ContatoEntityDto> RepositorioContato { get; }

        // Gateways reais
        public IClienteGateway ClienteGateway { get; }
        public IEnderecoGateway EnderecoGateway { get; }
        public IContatoGateway ContatoGateway { get; }

        // Serviços mockados
        public ILogGateway<CadastrarClienteHandler> LogServicoCadastrar { get; }
        public ILogGateway<AtualizarClienteHandler> LogServicoAtualizar { get; }
        public ILogGateway<ObterClienteHandler> LogServicoObter { get; }
        public ILogGateway<ObterTodosClientesHandler> LogServicoObterTodos { get; }
        public ILogGateway<ObterClientePorDocumentoHandler> LogServicoObterPorDocumento { get; }
        public ILogGateway<RemoverClienteHandler> LogServicoRemover { get; }
        public IUnidadeDeTrabalhoGateway UnidadeDeTrabalho { get; }
        public IUsuarioLogadoServicoGateway UsuarioLogadoServico { get; }

        public ClienteHandlerFixture()
        {
            // Inicializar repositórios mockados
            RepositorioCliente = Substitute.For<IRepositorio<ClienteEntityDto>>();
            RepositorioEndereco = Substitute.For<IRepositorio<EnderecoEntityDto>>();
            RepositorioContato = Substitute.For<IRepositorio<ContatoEntityDto>>();

            // Inicializar gateways reais usando os repositórios mockados
            ClienteGateway = new ClienteGateway(RepositorioCliente);
            EnderecoGateway = new EnderecoGateway(RepositorioEndereco);
            ContatoGateway = new ContatoGateway(RepositorioContato);

            // Inicializar serviços mockados
            LogServicoCadastrar = Substitute.For<ILogGateway<CadastrarClienteHandler>>();
            LogServicoAtualizar = Substitute.For<ILogGateway<AtualizarClienteHandler>>();
            LogServicoObter = Substitute.For<ILogGateway<ObterClienteHandler>>();
            LogServicoObterTodos = Substitute.For<ILogGateway<ObterTodosClientesHandler>>();
            LogServicoObterPorDocumento = Substitute.For<ILogGateway<ObterClientePorDocumentoHandler>>();
            LogServicoRemover = Substitute.For<ILogGateway<RemoverClienteHandler>>();
            UnidadeDeTrabalho = Substitute.For<IUnidadeDeTrabalhoGateway>();
            UsuarioLogadoServico = Substitute.For<IUsuarioLogadoServicoGateway>();

            // Configuração padrão para o UDT
            UnidadeDeTrabalho.Commit().Returns(Task.FromResult(true));
        }

        #region Criação de Handlers

        public CadastrarClienteHandler CriarCadastrarClienteHandler()
        {
            return new CadastrarClienteHandler(
                ClienteGateway,
                EnderecoGateway,
                ContatoGateway,
                LogServicoCadastrar,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);
        }

        public AtualizarClienteHandler CriarAtualizarClienteHandler()
        {
            return new AtualizarClienteHandler(
                ClienteGateway,
                EnderecoGateway,
                ContatoGateway,
                LogServicoAtualizar,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);
        }

        public ObterClienteHandler CriarObterClienteHandler()
        {
            return new ObterClienteHandler(
                ClienteGateway,
                LogServicoObter,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);
        }

        public ObterTodosClientesHandler CriarObterTodosClientesHandler()
        {
            return new ObterTodosClientesHandler(
                ClienteGateway,
                LogServicoObterTodos,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);
        }

        public ObterClientePorDocumentoHandler CriarObterClientePorDocumentoHandler()
        {
            return new ObterClientePorDocumentoHandler(
                ClienteGateway,
                LogServicoObterPorDocumento,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);
        }

        public RemoverClienteHandler CriarRemoverClienteHandler()
        {
            return new RemoverClienteHandler(
                ClienteGateway,
                LogServicoRemover,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);
        }

        #endregion

        #region Dados de Teste

        public static Cliente CriarClientePessoaFisicaValido()
        {
            return new Cliente
            {
                Id = Guid.NewGuid(),
                Nome = "João da Silva",
                Documento = "123.456.789-00",
                DataNascimento = "01/01/1980",
                Sexo = "M",
                TipoCliente = TipoCliente.PessoaFisica,
                Contato = new Contato
                {
                    Email = "joao@example.com",
                    Telefone = "(11) 98765-4321"
                },
                Endereco = new Endereco
                {
                    Rua = "Rua das Flores",
                    Numero = "123",
                    Complemento = "Apto 45",
                    Bairro = "Centro",
                    Cidade = "São Paulo",
                    CEP = "01234-567"
                },
                Ativo = true,
                DataCadastro = DateTime.UtcNow.AddDays(-30),
                DataAtualizacao = DateTime.UtcNow.AddDays(-5)
            };
        }

        public static Cliente CriarClientePessoaJuridicaValido()
        {
            return new Cliente
            {
                Id = Guid.NewGuid(),
                Nome = "Empresa XYZ Ltda",
                Documento = "12.345.678/0001-90",
                DataNascimento = "01/01/2000",
                TipoCliente = TipoCliente.PessoaJuridico,
                Contato = new Contato
                {
                    Email = "contato@empresaxyz.com",
                    Telefone = "(11) 3456-7890"
                },
                Endereco = new Endereco
                {
                    Rua = "Av. Paulista",
                    Numero = "1000",
                    Complemento = "Sala 123",
                    Bairro = "Bela Vista",
                    Cidade = "São Paulo",
                    CEP = "01310-100"
                },
                Ativo = true,
                DataCadastro = DateTime.UtcNow.AddDays(-60),
                DataAtualizacao = DateTime.UtcNow.AddDays(-10)
            };
        }

        public static CadastrarClienteUseCaseDto CriarCadastrarClientePessoaFisicaDtoValido()
        {
            return new CadastrarClienteUseCaseDto
            {
                Nome = "Maria Oliveira",
                Documento = "987.654.321-00",
                Sexo = "F",
                DataNascimento = "15/05/1985",
                TipoCliente = TipoCliente.PessoaFisica,
                Email = "maria@example.com",
                Telefone = "(11) 91234-5678",
                Rua = "Rua dos Pinheiros",
                Numero = "456",
                Complemento = "Casa 2",
                Bairro = "Pinheiros",
                Cidade = "São Paulo",
                CEP = "05422-010"
            };
        }

        public static CadastrarClienteUseCaseDto CriarCadastrarClientePessoaJuridicaDtoValido()
        {
            return new CadastrarClienteUseCaseDto
            {
                Nome = "Empresa ABC S.A.",
                Documento = "98.765.432/0001-10",
                Sexo = "",
                DataNascimento = "01/01/2010",
                TipoCliente = TipoCliente.PessoaJuridico,
                Email = "contato@empresaabc.com",
                Telefone = "(11) 2345-6789",
                Rua = "Av. Brigadeiro Faria Lima",
                Numero = "3000",
                Complemento = "Andar 15",
                Bairro = "Itaim Bibi",
                Cidade = "São Paulo",
                CEP = "04538-132"
            };
        }

        public static AtualizarClienteUseCaseDto CriarAtualizarClienteDtoValido()
        {
            return new AtualizarClienteUseCaseDto
            {
                Nome = "João da Silva Atualizado",
                Sexo = "M",
                DataNascimento = "02/02/1980",
                Email = "joao.novo@example.com",
                Telefone = "(11) 99999-8888",
                Rua = "Rua das Flores Atualizada",
                Numero = "456",
                Complemento = "Apto 50",
                Bairro = "Centro Novo",
                Cidade = "São Paulo",
                CEP = "01234-999"
            };
        }

        public static List<Cliente> CriarListaClientesVariados()
        {
            return new List<Cliente>
            {
                CriarClientePessoaFisicaValido(),
                CriarClientePessoaJuridicaValido(),
                new Cliente
                {
                    Id = Guid.NewGuid(),
                    Nome = "Pedro Santos",
                    Documento = "111.222.333-44",
                    DataNascimento = "10/10/1990",
                    Sexo = "M",
                    TipoCliente = TipoCliente.PessoaFisica,
                    Contato = new Contato
                    {
                        Email = "pedro@example.com",
                        Telefone = "(11) 97777-6666"
                    },
                    Endereco = new Endereco
                    {
                        Rua = "Rua Augusta",
                        Numero = "789",
                        Complemento = "Apto 10",
                        Bairro = "Consolação",
                        Cidade = "São Paulo",
                        CEP = "01305-000"
                    },
                    Ativo = true,
                    DataCadastro = DateTime.UtcNow.AddDays(-15),
                    DataAtualizacao = DateTime.UtcNow.AddDays(-2)
                }
            };
        }

        #endregion

        #region Configuração de Mocks dos Repositórios

        public void ConfigurarMockRepositorioClienteParaObterPorId(Guid id, Cliente cliente)
        {
            var dto = cliente != null ? ToClienteDto(cliente) : null;
            RepositorioCliente.ObterPorIdAsync(id).Returns(dto);

            // Configurar também o método ObterUmProjetadoSemRastreamentoAsync que é usado pelo gateway
            RepositorioCliente
                .ObterUmProjetadoSemRastreamentoAsync<Cliente>(Arg.Any<global::Core.Especificacoes.Base.Interfaces.IEspecificacao<ClienteEntityDto>>())
                .Returns(cliente);
        }

        public void ConfigurarMockRepositorioClienteParaObterPorIdNull(Guid id)
        {
            RepositorioCliente.ObterPorIdAsync(id).Returns((ClienteEntityDto)null);

            // Configurar também o método ObterUmProjetadoSemRastreamentoAsync que é usado pelo gateway
            RepositorioCliente
                .ObterUmProjetadoSemRastreamentoAsync<Cliente>(Arg.Any<global::Core.Especificacoes.Base.Interfaces.IEspecificacao<ClienteEntityDto>>())
                .Returns((Cliente)null);
        }

        public void ConfigurarMockRepositorioClienteParaObterTodos(List<Cliente> clientes)
        {
            // Para consultas com especificação (ObterTodosClienteComVeiculoAsync)
            RepositorioCliente.ListarProjetadoAsync<Cliente>(Arg.Any<global::Core.Especificacoes.Base.Interfaces.IEspecificacao<ClienteEntityDto>>())
                .Returns(clientes);
        }

        public void ConfigurarMockRepositorioClienteParaObterPorDocumento(string documento, Cliente cliente)
        {
            // Para consultas com especificação (ObterClientePorDocumentoAsync)
            RepositorioCliente.ObterUmProjetadoSemRastreamentoAsync<Cliente>(Arg.Any<global::Core.Especificacoes.Base.Interfaces.IEspecificacao<ClienteEntityDto>>())
                .Returns(cliente);
        }

        public void ConfigurarMockRepositorioClienteParaCadastrar(Cliente cliente)
        {
            var dto = ToClienteDto(cliente);
            RepositorioCliente.CadastrarAsync(Arg.Any<ClienteEntityDto>()).Returns(dto);
        }

        public void ConfigurarMockRepositorioClienteParaEditar()
        {
            RepositorioCliente.EditarAsync(Arg.Any<ClienteEntityDto>()).Returns(Task.CompletedTask);
        }

        public void ConfigurarMockRepositorioClienteParaDeletar()
        {
            RepositorioCliente.DeletarAsync(Arg.Any<ClienteEntityDto>()).Returns(Task.CompletedTask);
        }

        public void ConfigurarMockRepositorioEnderecoParaCadastrar(Endereco endereco)
        {
            var dto = ToEnderecoDto(endereco);
            RepositorioEndereco.CadastrarAsync(Arg.Any<EnderecoEntityDto>()).Returns(dto);
        }

        public void ConfigurarMockRepositorioEnderecoParaObterPorId(Guid id, Endereco endereco)
        {
            var dto = endereco != null ? ToEnderecoDto(endereco) : null;
            RepositorioEndereco.ObterPorIdAsync(id).Returns(dto);
        }

        public void ConfigurarMockRepositorioEnderecoParaEditar()
        {
            RepositorioEndereco.EditarAsync(Arg.Any<EnderecoEntityDto>()).Returns(Task.CompletedTask);
        }

        public void ConfigurarMockRepositorioContatoParaCadastrar(Contato contato)
        {
            var dto = ToContatoDto(contato);
            RepositorioContato.CadastrarAsync(Arg.Any<ContatoEntityDto>()).Returns(dto);
        }

        public void ConfigurarMockRepositorioContatoParaObterPorId(Guid id, Contato contato)
        {
            var dto = contato != null ? ToContatoDto(contato) : null;
            RepositorioContato.ObterPorIdAsync(id).Returns(dto);
        }

        public void ConfigurarMockRepositorioContatoParaEditar()
        {
            RepositorioContato.EditarAsync(Arg.Any<ContatoEntityDto>()).Returns(Task.CompletedTask);
        }

        public void ConfigurarMockUdtParaCommitFalha()
        {
            UnidadeDeTrabalho.Commit().Returns(Task.FromResult(false));
        }

        #endregion

        // Método para configurar endereço e contato para atualização
        public void ConfigurarMockEnderecoeContatoParaAtualizar(Guid enderecoId, Guid contatoId, Guid clienteId)
        {
            var endereco = new Endereco { Id = enderecoId, IdCliente = clienteId };
            ConfigurarMockRepositorioEnderecoParaObterPorId(enderecoId, endereco);

            var contato = new Contato { Id = contatoId, IdCliente = clienteId };
            ConfigurarMockRepositorioContatoParaObterPorId(contatoId, contato);
        }

        #region Métodos de Conversão para DTOs

        private static ClienteEntityDto ToClienteDto(Cliente cliente)
        {
            return new ClienteEntityDto
            {
                Id = cliente.Id,
                Ativo = cliente.Ativo,
                DataCadastro = cliente.DataCadastro,
                DataAtualizacao = cliente.DataAtualizacao,
                Nome = cliente.Nome,
                Documento = cliente.Documento,
                Sexo = cliente.Sexo,
                DataNascimento = cliente.DataNascimento,
                TipoCliente = cliente.TipoCliente,
                Contato = cliente.Contato != null ? ToContatoDto(cliente.Contato) : null,
                Endereco = cliente.Endereco != null ? ToEnderecoDto(cliente.Endereco) : null
            };
        }

        private static EnderecoEntityDto ToEnderecoDto(Endereco endereco)
        {
            return new EnderecoEntityDto
            {
                Id = endereco.Id,
                Ativo = endereco.Ativo,
                DataCadastro = endereco.DataCadastro,
                DataAtualizacao = endereco.DataAtualizacao,
                Rua = endereco.Rua,
                Numero = endereco.Numero,
                Complemento = endereco.Complemento,
                Bairro = endereco.Bairro,
                Cidade = endereco.Cidade,
                CEP = endereco.CEP,
                IdCliente = endereco.IdCliente
            };
        }

        private static ContatoEntityDto ToContatoDto(Contato contato)
        {
            return new ContatoEntityDto
            {
                Id = contato.Id,
                Ativo = contato.Ativo,
                DataCadastro = contato.DataCadastro,
                DataAtualizacao = contato.DataAtualizacao,
                Email = contato.Email,
                Telefone = contato.Telefone,
                IdCliente = contato.IdCliente
            };
        }

        #endregion
    }
}
