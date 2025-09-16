using Adapters.Controllers;
using Adapters.DTOs.Requests.Cliente;
using Adapters.Presenters.Interfaces;
using Core.DTOs.UseCases.Cliente;
using Core.Entidades;
using Core.Enumeradores;
using Core.Interfaces.UseCases;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOS.UnitTests.Adapters.Controllers
{
    public class ClienteControllerTests
    {
        private readonly IClienteUseCases _clienteUseCases;
        private readonly IClientePresenter _clientePresenter;
        private readonly ClienteController _clienteController;

        public ClienteControllerTests()
        {
            _clienteUseCases = Substitute.For<IClienteUseCases>();
            _clientePresenter = Substitute.For<IClientePresenter>();
            _clienteController = new ClienteController(_clienteUseCases, _clientePresenter);
        }

        [Fact]
        public void MapearParaCadastrarClienteUseCaseDto_ComRequestValido_DeveMapearCorretamente()
        {
            // Arrange
            var request = new CadastrarClienteRequest
            {
                Nome = "Cliente Teste",
                Sexo = "Masculino",
                Documento = "12345678900",
                DataNascimento = "1990-01-01",
                TipoCliente = TipoCliente.PessoaFisica,
                Rua = "Rua Teste",
                Bairro = "Bairro Teste",
                Cidade = "Cidade Teste",
                Numero = "123",
                CEP = "12345-678",
                Complemento = "Complemento Teste",
                Email = "teste@email.com",
                Telefone = "11987654321"
            };

            // Act
            var result = _clienteController.MapearParaCadastrarClienteUseCaseDto(request);

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be(request.Nome);
            result.Sexo.Should().Be(request.Sexo);
            result.Documento.Should().Be(request.Documento);
            result.DataNascimento.Should().Be(request.DataNascimento);
            result.TipoCliente.Should().Be(request.TipoCliente);
            result.Rua.Should().Be(request.Rua);
            result.Bairro.Should().Be(request.Bairro);
            result.Cidade.Should().Be(request.Cidade);
            result.Numero.Should().Be(request.Numero);
            result.CEP.Should().Be(request.CEP);
            result.Complemento.Should().Be(request.Complemento);
            result.Email.Should().Be(request.Email);
            result.Telefone.Should().Be(request.Telefone);
        }

        [Fact]
        public void MapearParaCadastrarClienteUseCaseDto_ComRequestNulo_DeveRetornarNulo()
        {
            // Act
            var result = _clienteController.MapearParaCadastrarClienteUseCaseDto(null);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void MapearParaAtualizarClienteUseCaseDto_ComRequestValido_DeveMapearCorretamente()
        {
            // Arrange
            var request = new AtualizarClienteRequest
            {
                Id = Guid.NewGuid(),
                Nome = "Cliente Atualizado",
                Sexo = "Feminino",
                Documento = "98765432100",
                DataNascimento = "1995-05-05",
                TipoCliente = TipoCliente.PessoaJuridico,
                EnderecoId = Guid.NewGuid(),
                Rua = "Rua Atualizada",
                Bairro = "Bairro Atualizado",
                Cidade = "Cidade Atualizada",
                Numero = "456",
                CEP = "98765-432",
                Complemento = "Complemento Atualizado",
                ContatoId = Guid.NewGuid(),
                Email = "atualizado@email.com",
                Telefone = "11912345678"
            };

            // Act
            var result = _clienteController.MapearParaAtualizarClienteUseCaseDto(request);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(request.Id);
            result.Nome.Should().Be(request.Nome);
            result.Sexo.Should().Be(request.Sexo);
            result.Documento.Should().Be(request.Documento);
            result.DataNascimento.Should().Be(request.DataNascimento);
            result.TipoCliente.Should().Be(request.TipoCliente);
            result.EnderecoId.Should().Be(request.EnderecoId);
            result.Rua.Should().Be(request.Rua);
            result.Bairro.Should().Be(request.Bairro);
            result.Cidade.Should().Be(request.Cidade);
            result.Numero.Should().Be(request.Numero);
            result.CEP.Should().Be(request.CEP);
            result.Complemento.Should().Be(request.Complemento);
            result.ContatoId.Should().Be(request.ContatoId);
            result.Email.Should().Be(request.Email);
            result.Telefone.Should().Be(request.Telefone);
        }

        [Fact]
        public void MapearParaAtualizarClienteUseCaseDto_ComRequestNulo_DeveRetornarNulo()
        {
            // Act
            var result = _clienteController.MapearParaAtualizarClienteUseCaseDto(null);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Cadastrar_DeveUsarMapeamentoEChamarUseCase()
        {
            // Arrange
            var request = new CadastrarClienteRequest
            {
                Nome = "Cliente Teste",
                Sexo = "Masculino",
                Documento = "12345678900",
                DataNascimento = "1990-01-01",
                TipoCliente = TipoCliente.PessoaFisica,
                Rua = "Rua Teste",
                Bairro = "Bairro Teste",
                Cidade = "Cidade Teste",
                Numero = "123",
                CEP = "12345-678",
                Complemento = "Complemento Teste",
                Email = "teste@email.com",
                Telefone = "11987654321"
            };

            var cliente = new Cliente();
            _clienteUseCases.CadastrarUseCaseAsync(Arg.Any<CadastrarClienteUseCaseDto>())
                .Returns(cliente);

            // Act
            await _clienteController.Cadastrar(request);

            // Assert
            await _clienteUseCases.Received(1).CadastrarUseCaseAsync(Arg.Is<CadastrarClienteUseCaseDto>(
                dto => dto.Nome == request.Nome &&
                      dto.Documento == request.Documento &&
                      dto.Email == request.Email));
            
            _clientePresenter.Received(1).ParaResponse(cliente);
        }

        [Fact]
        public async Task Atualizar_DeveUsarMapeamentoEChamarUseCase()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new AtualizarClienteRequest
            {
                Id = Guid.NewGuid(),
                Nome = "Cliente Atualizado",
                Sexo = "Feminino",
                Documento = "98765432100",
                DataNascimento = "1995-05-05",
                TipoCliente = TipoCliente.PessoaJuridico
            };

            var cliente = new Cliente();
            _clienteUseCases.AtualizarUseCaseAsync(id, Arg.Any<AtualizarClienteUseCaseDto>())
                .Returns(cliente);

            // Act
            await _clienteController.Atualizar(id, request);

            // Assert
            await _clienteUseCases.Received(1).AtualizarUseCaseAsync(
                Arg.Is<Guid>(g => g == id),
                Arg.Is<AtualizarClienteUseCaseDto>(
                    dto => dto.Nome == request.Nome &&
                          dto.Documento == request.Documento &&
                          dto.TipoCliente == request.TipoCliente));
            
            _clientePresenter.Received(1).ParaResponse(cliente);
        }
    }
}
