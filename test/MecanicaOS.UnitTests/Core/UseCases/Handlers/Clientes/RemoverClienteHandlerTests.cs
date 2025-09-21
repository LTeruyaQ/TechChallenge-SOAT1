using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
using Core.UseCases.Clientes.RemoverCliente;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.Handlers;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.Clientes
{
    public class RemoverClienteHandlerTests
    {
        private readonly ClienteHandlerFixture _fixture;

        public RemoverClienteHandlerTests()
        {
            _fixture = new ClienteHandlerFixture();
        }

        [Fact]
        public async Task Handle_ComClienteExistente_DeveRemoverCliente()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var clienteExistente = ClienteHandlerFixture.CriarClientePessoaFisicaValido();
            clienteExistente.Id = clienteId;
            
            // Configurar o gateway para retornar o cliente existente
            _fixture.ConfigurarMockClienteGatewayParaObterPorId(clienteId, clienteExistente);
            
            // Configurar o gateway para simular a remoção
            _fixture.ConfigurarMockClienteGatewayParaRemover(clienteExistente);
            
            var handler = _fixture.CriarRemoverClienteHandler();

            // Act
            var resultado = await handler.Handle(clienteId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            
            // Verificar que o gateway foi chamado para obter e remover o cliente
            await _fixture.ClienteGateway.Received(1).ObterPorIdAsync(clienteId);
            await _fixture.ClienteGateway.Received(1).DeletarAsync(clienteExistente);
            
            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoRemover.Received(1).LogInicio(Arg.Any<string>(), clienteId);
            _fixture.LogServicoRemover.Received(1).LogFim(Arg.Any<string>(), true);
        }

        [Fact]
        public async Task Handle_ComClienteInexistente_DeveLancarDadosNaoEncontradosException()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            
            // Configurar o gateway para retornar null (cliente não encontrado)
            _fixture.ClienteGateway.ObterPorIdAsync(clienteId).Returns((Cliente)null);
            
            var handler = _fixture.CriarRemoverClienteHandler();

            // Act & Assert
            var act = async () => await handler.Handle(clienteId);
            
            await act.Should().ThrowAsync<DadosNaoEncontradosException>()
                .WithMessage("Cliente não encontrado");
            
            // Verificar que o gateway foi chamado para obter o cliente
            await _fixture.ClienteGateway.Received(1).ObterPorIdAsync(clienteId);
            
            // Verificar que o gateway NÃO foi chamado para remover o cliente
            await _fixture.ClienteGateway.DidNotReceive().DeletarAsync(Arg.Any<Cliente>());
            
            // Verificar que o commit NÃO foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceive().Commit();
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoRemover.Received(1).LogInicio(Arg.Any<string>(), clienteId);
            _fixture.LogServicoRemover.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosNaoEncontradosException>());
        }

        [Fact]
        public async Task Handle_QuandoCommitFalha_DeveLancarPersistirDadosException()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var clienteExistente = ClienteHandlerFixture.CriarClientePessoaFisicaValido();
            clienteExistente.Id = clienteId;
            
            // Configurar o gateway para retornar o cliente existente
            _fixture.ConfigurarMockClienteGatewayParaObterPorId(clienteId, clienteExistente);
            
            // Configurar o gateway para simular a remoção
            _fixture.ConfigurarMockClienteGatewayParaRemover(clienteExistente);
            
            // Configurar o UDT para falhar no commit
            _fixture.ConfigurarMockUdtParaCommitFalha();
            
            var handler = _fixture.CriarRemoverClienteHandler();

            // Act & Assert
            var act = async () => await handler.Handle(clienteId);
            
            await act.Should().ThrowAsync<PersistirDadosException>()
                .WithMessage("Erro ao remover cliente");
            
            // Verificar que o gateway foi chamado para obter e remover o cliente
            await _fixture.ClienteGateway.Received(1).ObterPorIdAsync(clienteId);
            await _fixture.ClienteGateway.Received(1).DeletarAsync(clienteExistente);
            
            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoRemover.Received(1).LogInicio(Arg.Any<string>(), clienteId);
            _fixture.LogServicoRemover.Received(1).LogErro(Arg.Any<string>(), Arg.Any<PersistirDadosException>());
        }

        [Fact]
        public async Task Handle_QuandoGatewayLancaExcecao_DeveRegistrarLogEPropagar()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var excecaoEsperada = new Exception("Erro no banco de dados");
            
            // Configurar o gateway para lançar uma exceção
            _fixture.ClienteGateway.When(x => x.ObterPorIdAsync(Arg.Any<Guid>()))
                .Do(x => { throw excecaoEsperada; });
            
            var handler = _fixture.CriarRemoverClienteHandler();

            // Act & Assert
            var act = async () => await handler.Handle(clienteId);
            
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Erro no banco de dados");
            
            // Verificar que o gateway foi chamado para obter o cliente
            await _fixture.ClienteGateway.Received(1).ObterPorIdAsync(clienteId);
            
            // Verificar que o gateway NÃO foi chamado para remover o cliente
            await _fixture.ClienteGateway.DidNotReceive().DeletarAsync(Arg.Any<Cliente>());
            
            // Verificar que o commit NÃO foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceive().Commit();
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoRemover.Received(1).LogInicio(Arg.Any<string>(), clienteId);
            _fixture.LogServicoRemover.Received(1).LogErro(Arg.Any<string>(), excecaoEsperada);
        }

        [Fact]
        public async Task Handle_DevePassarDadosCorretamenteEntreHandlerERepositorio()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            
            // Criar um cliente com valores específicos para identificar no teste
            var clienteEspecifico = new Cliente
            {
                Id = clienteId,
                Nome = "Cliente Para Remover",
                Documento = "111.222.333-44",
                DataNascimento = "01/01/1990",
                Sexo = "M",
                TipoCliente = TipoCliente.PessoaFisica,
                Contato = new Contato
                {
                    Email = "remover@example.com",
                    Telefone = "(11) 91234-5678"
                },
                Endereco = new Endereco
                {
                    Rua = "Rua da Remoção",
                    Numero = "123",
                    Complemento = "Apto 42",
                    Bairro = "Bairro Teste",
                    Cidade = "Cidade Teste",
                    CEP = "12345-678"
                },
                Ativo = true,
                DataCadastro = new DateTime(2023, 1, 15),
                DataAtualizacao = new DateTime(2023, 6, 30)
            };
            
            // Configurar o gateway para retornar o cliente específico
            _fixture.ConfigurarMockClienteGatewayParaObterPorId(clienteId, clienteEspecifico);
            
            // Configurar o gateway para simular a remoção
            _fixture.ConfigurarMockClienteGatewayParaRemover(clienteEspecifico);
            
            var handler = _fixture.CriarRemoverClienteHandler();

            // Act
            var resultado = await handler.Handle(clienteId);

            // Assert
            // Verificar que o gateway foi chamado com o ID correto
            await _fixture.ClienteGateway.Received(1).ObterPorIdAsync(clienteId);
            
            // Verificar que o gateway foi chamado para remover o cliente específico
            await _fixture.ClienteGateway.Received(1).DeletarAsync(clienteEspecifico);
            
            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();
            
            // Verificar o resultado
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
        }
    }
}
