using Core.DTOs.UseCases.Cliente;
using Core.Entidades;
using Core.Enumeradores;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Clientes.AtualizarCliente;
using Core.UseCases.Clientes.CadastrarCliente;
using Core.UseCases.Clientes.ObterCliente;
using Core.UseCases.Clientes.ObterClientePorDocumento;
using Core.UseCases.Clientes.ObterTodosClientes;
using Core.UseCases.Clientes.RemoverCliente;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MecanicaOS.UnitTests.Fixtures.Handlers
{
    public class ClienteHandlerFixture
    {
        public IClienteGateway ClienteGateway { get; }
        public IEnderecoGateway EnderecoGateway { get; }
        public IContatoGateway ContatoGateway { get; }
        public ILogServico<CadastrarClienteHandler> LogServicoCadastrar { get; }
        public ILogServico<AtualizarClienteHandler> LogServicoAtualizar { get; }
        public ILogServico<ObterClienteHandler> LogServicoObter { get; }
        public ILogServico<ObterTodosClientesHandler> LogServicoObterTodos { get; }
        public ILogServico<ObterClientePorDocumentoHandler> LogServicoObterPorDocumento { get; }
        public ILogServico<RemoverClienteHandler> LogServicoRemover { get; }
        public IUnidadeDeTrabalho UnidadeDeTrabalho { get; }
        public IUsuarioLogadoServico UsuarioLogadoServico { get; }

        public ClienteHandlerFixture()
        {
            ClienteGateway = Substitute.For<IClienteGateway>();
            EnderecoGateway = Substitute.For<IEnderecoGateway>();
            ContatoGateway = Substitute.For<IContatoGateway>();
            LogServicoCadastrar = Substitute.For<ILogServico<CadastrarClienteHandler>>();
            LogServicoAtualizar = Substitute.For<ILogServico<AtualizarClienteHandler>>();
            LogServicoObter = Substitute.For<ILogServico<ObterClienteHandler>>();
            LogServicoObterTodos = Substitute.For<ILogServico<ObterTodosClientesHandler>>();
            LogServicoObterPorDocumento = Substitute.For<ILogServico<ObterClientePorDocumentoHandler>>();
            LogServicoRemover = Substitute.For<ILogServico<RemoverClienteHandler>>();
            UnidadeDeTrabalho = Substitute.For<IUnidadeDeTrabalho>();
            UsuarioLogadoServico = Substitute.For<IUsuarioLogadoServico>();
            
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

        #region Configuração de Mocks

        public void ConfigurarMockClienteGatewayParaObterPorId(Guid id, Cliente cliente)
        {
            ClienteGateway.ObterPorIdAsync(id).Returns(cliente);
        }

        public void ConfigurarMockClienteGatewayParaObterTodos(List<Cliente> clientes)
        {
            ClienteGateway.ObterTodosClienteComVeiculoAsync().Returns(clientes);
        }

        public void ConfigurarMockClienteGatewayParaObterPorDocumento(string documento, Cliente cliente)
        {
            ClienteGateway.ObterClientePorDocumentoAsync(documento).Returns(cliente);
        }

        public void ConfigurarMockClienteGatewayParaCadastrar(Cliente cliente)
        {
            ClienteGateway.CadastrarAsync(Arg.Any<Cliente>()).Returns(Task.FromResult(cliente));
            ClienteGateway.When(x => x.CadastrarAsync(Arg.Any<Cliente>()))
                .Do(callInfo => 
                {
                    var clienteArg = callInfo.Arg<Cliente>();
                    clienteArg.Id = cliente.Id;
                    clienteArg.DataCadastro = cliente.DataCadastro;
                    clienteArg.DataAtualizacao = cliente.DataAtualizacao;
                    clienteArg.Ativo = cliente.Ativo;
                });
        }

        public void ConfigurarMockClienteGatewayParaAtualizar(Cliente cliente)
        {
            ClienteGateway.EditarAsync(Arg.Any<Cliente>()).Returns(Task.CompletedTask);
            ClienteGateway.When(x => x.EditarAsync(Arg.Any<Cliente>()))
                .Do(callInfo => 
                {
                    var clienteArg = callInfo.Arg<Cliente>();
                    clienteArg.DataAtualizacao = DateTime.UtcNow;
                });
        }

        public void ConfigurarMockClienteGatewayParaRemover(Cliente cliente)
        {
            ClienteGateway.DeletarAsync(cliente).Returns(Task.CompletedTask);
        }

        public void ConfigurarMockUdtParaCommitFalha()
        {
            UnidadeDeTrabalho.Commit().Returns(Task.FromResult(false));
        }

        public void ConfigurarMockEnderecoeContatoParaAtualizar(Guid enderecoId, Guid contatoId, Guid clienteId)
        {
            var endereco = new Endereco { Id = enderecoId, IdCliente = clienteId };
            EnderecoGateway.ObterPorIdAsync(enderecoId).Returns(endereco);
            
            var contato = new Contato { Id = contatoId, IdCliente = clienteId };
            ContatoGateway.ObterPorIdAsync(contatoId).Returns(contato);
        }

        #endregion
    }
}
