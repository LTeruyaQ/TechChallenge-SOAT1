using Core.DTOs.Entidades.Cliente;
using Core.DTOs.UseCases.Cliente;
using Core.Entidades;
using Core.Enumeradores;
using Core.Especificacoes.Base.Interfaces;
using Core.Exceptions;
using Core.UseCases.Clientes.AtualizarCliente;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.Handlers;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.Clientes
{
    public class AtualizarClienteHandlerTests
    {
        private readonly ClienteHandlerFixture _fixture;

        public AtualizarClienteHandlerTests()
        {
            _fixture = new ClienteHandlerFixture();
        }

        [Fact]
        public async Task Handle_ComDadosValidos_DeveAtualizarCliente()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var clienteExistente = ClienteHandlerFixture.CriarClientePessoaFisicaValido();
            clienteExistente.Id = clienteId;
            
            var dto = ClienteHandlerFixture.CriarAtualizarClienteDtoValido();
            
            // Configurar o repositório para retornar o cliente existente
            _fixture.ConfigurarMockRepositorioClienteParaObterPorId(clienteId, clienteExistente);
            
            // Configurar o repositório para simular a atualização
            _fixture.ConfigurarMockRepositorioClienteParaEditar();
            
            var handler = _fixture.CriarAtualizarClienteHandler();

            // Act
            var resultado = await handler.Handle(clienteId, dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Cliente.Should().NotBeNull();
            resultado.Cliente.Should().BeEquivalentTo(clienteExistente);
            
            // Verificar que o repositório foi chamado para obter e atualizar o cliente
            await _fixture.RepositorioCliente.Received(1).ObterPorIdAsync(clienteId);
            await _fixture.RepositorioCliente.Received(1).EditarAsync(Arg.Any<ClienteEntityDto>());
            
            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoAtualizar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoAtualizar.Received(1).LogFim(Arg.Any<string>(), clienteExistente);
        }

        [Fact]
        public async Task Handle_ComClienteInexistente_DeveLancarDadosNaoEncontradosException()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var dto = ClienteHandlerFixture.CriarAtualizarClienteDtoValido();
            
            // Configurar o repositório para retornar null (cliente não encontrado)
            _fixture.RepositorioCliente.ObterPorIdAsync(clienteId).Returns(Task.FromResult<ClienteEntityDto>(null));
            
            var handler = _fixture.CriarAtualizarClienteHandler();

            // Act & Assert
            var act = async () => await handler.Handle(clienteId, dto);
            
            await act.Should().ThrowAsync<DadosNaoEncontradosException>()
                .WithMessage("Cliente não encontrado");
            
            // Verificar que o repositório foi chamado para obter o cliente
            await _fixture.RepositorioCliente.Received(1).ObterPorIdAsync(clienteId);
            
            // Verificar que o repositório NÃO foi chamado para atualizar o cliente
            await _fixture.RepositorioCliente.DidNotReceive().EditarAsync(Arg.Any<ClienteEntityDto>());
            
            // Verificar que o commit NÃO foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceive().Commit();
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoAtualizar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoAtualizar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosNaoEncontradosException>());
        }

        [Fact]
        public async Task Handle_QuandoCommitFalha_DeveLancarPersistirDadosException()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var clienteExistente = ClienteHandlerFixture.CriarClientePessoaFisicaValido();
            clienteExistente.Id = clienteId;
            
            var dto = ClienteHandlerFixture.CriarAtualizarClienteDtoValido();
            
            // Configurar o repositório para retornar o cliente existente
            _fixture.ConfigurarMockRepositorioClienteParaObterPorId(clienteId, clienteExistente);
            
            // Configurar o repositório para simular a atualização
            _fixture.ConfigurarMockRepositorioClienteParaEditar();
            
            // Configurar o UDT para falhar no commit
            _fixture.ConfigurarMockUdtParaCommitFalha();
            
            var handler = _fixture.CriarAtualizarClienteHandler();

            // Act & Assert
            var act = async () => await handler.Handle(clienteId, dto);
            
            await act.Should().ThrowAsync<PersistirDadosException>()
                .WithMessage("Erro ao atualizar cliente");
            
            // Verificar que o repositório foi chamado para obter e atualizar o cliente
            await _fixture.RepositorioCliente.Received(1).ObterPorIdAsync(clienteId);
            await _fixture.RepositorioCliente.Received(1).EditarAsync(Arg.Any<ClienteEntityDto>());
            
            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoAtualizar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoAtualizar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<PersistirDadosException>());
        }

        [Fact]
        public async Task Handle_DeveAtualizarDadosCorretamente()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var enderecoId = Guid.NewGuid();
            var contatoId = Guid.NewGuid();
            
            var clienteExistente = ClienteHandlerFixture.CriarClientePessoaFisicaValido();
            clienteExistente.Id = clienteId;
            clienteExistente.Nome = "Nome Original";
            clienteExistente.Sexo = "M";
            clienteExistente.DataNascimento = "01/01/1980";
            clienteExistente.Endereco.Id = enderecoId;
            clienteExistente.Contato.Id = contatoId;
            
            var dto = new AtualizarClienteUseCaseDto
            {
                Id = clienteId,
                Nome = "Nome Atualizado",
                Sexo = "F",
                DataNascimento = "02/02/1990",
                Email = "email.atualizado@example.com",
                Telefone = "(11) 99999-8888",
                Rua = "Rua Atualizada",
                Numero = "999",
                Complemento = "Apto 999",
                Bairro = "Bairro Atualizado",
                Cidade = "Cidade Atualizada",
                CEP = "99999-999",
                EnderecoId = enderecoId,
                ContatoId = contatoId
            };
            
            // Configurar o repositório para retornar o cliente existente
            _fixture.ConfigurarMockRepositorioClienteParaObterPorId(clienteId, clienteExistente);
            
            // Configurar o repositório para simular a atualização
            _fixture.ConfigurarMockRepositorioClienteParaEditar();
            
            // Configurar os repositórios de endereço e contato
            _fixture.ConfigurarMockEnderecoeContatoParaAtualizar(enderecoId, contatoId, clienteId);
            
            var handler = _fixture.CriarAtualizarClienteHandler();

            // Act
            var resultado = await handler.Handle(clienteId, dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Cliente.Should().NotBeNull();
            
            // Verificar que os dados do cliente foram atualizados corretamente
            resultado.Cliente.Nome.Should().Be("Nome Atualizado");
            resultado.Cliente.Sexo.Should().Be("F");
            resultado.Cliente.DataNascimento.Should().Be("02/02/1990");
            
            // Verificar que o repositório foi chamado para obter e atualizar o cliente
            await _fixture.RepositorioCliente.Received(1).ObterPorIdAsync(clienteId);
            await _fixture.RepositorioCliente.Received(1).EditarAsync(Arg.Any<ClienteEntityDto>());
            
            // Verificar que o repositório de endereço foi chamado para atualizar o endereço
            await _fixture.RepositorioEndereco.Received(1).EditarAsync(Arg.Any<EnderecoEntityDto>());
            
            // Verificar que o repositório de contato foi chamado para atualizar o contato
            await _fixture.RepositorioContato.Received(1).EditarAsync(Arg.Any<ContatoEntityDto>());
            
            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();
        }
    }
}
