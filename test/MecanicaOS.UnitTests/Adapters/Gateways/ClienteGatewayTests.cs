using Adapters.Gateways;
using Core.DTOs.Entidades.Autenticacao;
using Core.DTOs.Entidades.Cliente;
using Core.Entidades;
using Core.Enumeradores;
using Core.Especificacoes.Base.Interfaces;
using Core.Exceptions;
using Core.Interfaces.Repositorios;
using Microsoft.AspNetCore.Mvc;
using MecanicaOS.UnitTests.Fixtures;

namespace MecanicaOS.UnitTests.Adapters.Gateways
{
    /// <summary>
    /// Testes unitários para o ClienteGateway
    /// </summary>
    public class ClienteGatewayTests
    {
        /// <summary>
        /// Verifica se o método CadastrarAsync converte corretamente a entidade para DTO e chama o repositório
        /// </summary>
        [Fact]
        public async Task CadastrarAsync_DeveConverterEntidadeParaDtoEChamarRepositorio()
        {
            // Arrange
            var repositorioMock = Substitute.For<IRepositorio<ClienteEntityDto>>();
            
            var cliente = ClienteFixture.CriarClienteValido();
            cliente.Id = Guid.NewGuid();
            
            ClienteEntityDto? clienteRepositoryDtoCapturado = null;
            
            repositorioMock.CadastrarAsync(Arg.Do<ClienteEntityDto>(dto => clienteRepositoryDtoCapturado = dto))
                .Returns(callInfo => callInfo.Arg<ClienteEntityDto>());
            
            var gateway = new ClienteGateway(repositorioMock);
            
            // Act
            var resultado = await gateway.CadastrarAsync(cliente);
            
            // Assert
            resultado.Should().NotBeNull("o resultado não deve ser nulo");
            resultado.Id.Should().Be(cliente.Id, "o ID deve ser preservado");
            
            clienteRepositoryDtoCapturado.Should().NotBeNull("o DTO capturado não deve ser nulo");
            clienteRepositoryDtoCapturado.Id.Should().Be(cliente.Id, "o ID deve ser preservado no DTO");
            clienteRepositoryDtoCapturado.Nome.Should().Be(cliente.Nome, "o nome deve ser preservado no DTO");
            clienteRepositoryDtoCapturado.TipoCliente.Should().Be(cliente.TipoCliente, "o tipo de cliente deve ser preservado no DTO");
            clienteRepositoryDtoCapturado.Documento.Should().Be(cliente.Documento, "o documento deve ser preservado no DTO");
            clienteRepositoryDtoCapturado.Ativo.Should().Be(cliente.Ativo, "o status ativo deve ser preservado no DTO");
            clienteRepositoryDtoCapturado.DataCadastro.Should().Be(cliente.DataCadastro, "a data de cadastro deve ser preservada no DTO");
            clienteRepositoryDtoCapturado.DataAtualizacao.Should().Be(cliente.DataAtualizacao, "a data de atualização deve ser preservada no DTO");
            
            await repositorioMock.Received(1).CadastrarAsync(Arg.Any<ClienteEntityDto>());
        }

        /// <summary>
        /// Verifica se o método AtualizarAsync converte corretamente a entidade para DTO e chama o repositório
        /// </summary>
        [Fact]
        public async Task EditarAsync_DeveConverterEntidadeParaDtoEChamarRepositorio()
        {
            // Arrange
            var repositorioMock = Substitute.For<IRepositorio<ClienteEntityDto>>();
            
            var cliente = ClienteFixture.CriarClienteValido();
            cliente.Id = Guid.NewGuid();
            cliente.Nome = "Cliente Atualizado";
            
            ClienteEntityDto clienteRepositoryDtoCapturado = null;
            
            repositorioMock.EditarAsync(Arg.Do<ClienteEntityDto>(dto => clienteRepositoryDtoCapturado = dto))
                .Returns(Task.CompletedTask);
            
            var gateway = new ClienteGateway(repositorioMock);
            
            // Act
            await gateway.EditarAsync(cliente);
            
            // Assert
            
            clienteRepositoryDtoCapturado.Should().NotBeNull("o DTO capturado não deve ser nulo");
            clienteRepositoryDtoCapturado.Id.Should().Be(cliente.Id, "o ID deve ser preservado no DTO");
            clienteRepositoryDtoCapturado.Nome.Should().Be("Cliente Atualizado", "o nome deve ser atualizado no DTO");
            clienteRepositoryDtoCapturado.DataAtualizacao.Should().Be(cliente.DataAtualizacao, "a data de atualização deve ser preservada no DTO");
            
            await repositorioMock.Received(1).EditarAsync(Arg.Any<ClienteEntityDto>());
        }

        /// <summary>
        /// Verifica se o método ObterPorIdAsync usa especificação correta e retorna cliente
        /// </summary>
        [Fact]
        public async Task ObterPorIdAsync_DeveUsarEspecificacaoERetornarCliente()
        {
            // Arrange
            var repositorioMock = Substitute.For<IRepositorio<ClienteEntityDto>>();
            
            var cliente = ClienteFixture.CriarClienteValido();
            cliente.Id = Guid.NewGuid();
            
            repositorioMock.ObterUmProjetadoSemRastreamentoAsync<Cliente>(Arg.Any<IEspecificacao<ClienteEntityDto>>())
                .Returns(cliente);
            
            var gateway = new ClienteGateway(repositorioMock);
            
            // Act
            var resultado = await gateway.ObterPorIdAsync(cliente.Id);
            
            // Assert
            resultado.Should().NotBeNull("o resultado não deve ser nulo");
            resultado!.Id.Should().Be(cliente.Id, "o ID deve ser preservado");
            resultado.Nome.Should().Be(cliente.Nome, "o nome deve ser preservado");
            resultado.TipoCliente.Should().Be(cliente.TipoCliente, "o tipo de cliente deve ser preservado");
            resultado.Documento.Should().Be(cliente.Documento, "o documento deve ser preservado");
            resultado.Ativo.Should().BeTrue("o status ativo deve ser preservado");
            
            resultado.Endereco.Should().NotBeNull("o endereço não deve ser nulo");
            resultado.Contato.Should().NotBeNull("o contato não deve ser nulo");
            
            await repositorioMock.Received(1).ObterUmProjetadoSemRastreamentoAsync<Cliente>(Arg.Any<IEspecificacao<ClienteEntityDto>>());
        }

        /// <summary>
        /// Verifica se o método ObterClientePorDocumentoAsync usa especificação correta e retorna cliente
        /// </summary>
        [Fact]
        public async Task ObterClientePorDocumentoAsync_DeveUsarEspecificacaoERetornarCliente()
        {
            // Arrange
            var repositorioMock = Substitute.For<IRepositorio<ClienteEntityDto>>();
            
            var cliente = ClienteFixture.CriarClienteValido();
            var documento = "12345678900";
            cliente.Documento = documento;
            
            repositorioMock.ObterUmProjetadoSemRastreamentoAsync<Cliente>(Arg.Any<IEspecificacao<ClienteEntityDto>>())
                .Returns(cliente);
            
            var gateway = new ClienteGateway(repositorioMock);
            
            // Act
            var resultado = await gateway.ObterClientePorDocumentoAsync(documento);
            
            // Assert
            resultado.Should().NotBeNull("o resultado não deve ser nulo");
            resultado!.Id.Should().Be(cliente.Id, "o ID deve ser preservado");
            resultado.Documento.Should().Be(documento, "o documento deve ser preservado");
            
            await repositorioMock.Received(1).ObterUmProjetadoSemRastreamentoAsync<Cliente>(Arg.Any<IEspecificacao<ClienteEntityDto>>());
        }

        /// <summary>
        /// Verifica se o método ObterTodosAsync converte corretamente os DTOs para entidades
        /// </summary>
        [Fact]
        public async Task ObterTodosClientesAsync_DeveUsarEspecificacaoERetornarClientes()
        {
            // Arrange
            var repositorioMock = Substitute.For<IRepositorio<ClienteEntityDto>>();
            
            var clientes = ClienteFixture.CriarListaClientes(2);
            
            repositorioMock.ListarProjetadoAsync<Cliente>(Arg.Any<IEspecificacao<ClienteEntityDto>>())
                .Returns(clientes);
            
            var gateway = new ClienteGateway(repositorioMock);
            
            // Act
            var resultado = await gateway.ObterTodosClientesAsync();
            
            // Assert
            resultado.Should().NotBeNull("o resultado não deve ser nulo");
            resultado.Should().HaveCount(2, "devem ser retornados 2 clientes");
            
            await repositorioMock.Received(1).ListarProjetadoAsync<Cliente>(Arg.Any<IEspecificacao<ClienteEntityDto>>());
        }
    }
}
