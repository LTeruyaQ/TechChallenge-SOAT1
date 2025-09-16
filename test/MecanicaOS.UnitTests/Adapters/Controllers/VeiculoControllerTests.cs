using Adapters.Controllers;
using Adapters.DTOs.Requests.Veiculo;
using Adapters.DTOs.Responses.Veiculo;
using Adapters.Presenters.Interfaces;
using Core.DTOs.UseCases.Veiculo;
using Core.Entidades;
using Core.Interfaces.UseCases;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOS.UnitTests.Adapters.Controllers
{
    public class VeiculoControllerTests
    {
        private readonly IVeiculoUseCases _veiculoUseCases;
        private readonly IVeiculoPresenter _veiculoPresenter;
        private readonly VeiculoController _veiculoController;

        public VeiculoControllerTests()
        {
            _veiculoUseCases = Substitute.For<IVeiculoUseCases>();
            _veiculoPresenter = Substitute.For<IVeiculoPresenter>();
            _veiculoController = new VeiculoController(_veiculoUseCases, _veiculoPresenter);
        }

        [Fact]
        public void MapearParaCadastrarVeiculoUseCaseDto_ComRequestValido_DeveMapearCorretamente()
        {
            // Arrange
            var request = new CadastrarVeiculoRequest
            {
                Placa = "ABC1234",
                Marca = "Toyota",
                Modelo = "Corolla",
                Cor = "Preto",
                Ano = "2022",
                Anotacoes = "Veículo em bom estado",
                ClienteId = Guid.NewGuid()
            };

            // Act
            var result = _veiculoController.MapearParaCadastrarVeiculoUseCaseDto(request);

            // Assert
            result.Should().NotBeNull();
            result.Placa.Should().Be(request.Placa);
            result.Marca.Should().Be(request.Marca);
            result.Modelo.Should().Be(request.Modelo);
            result.Cor.Should().Be(request.Cor);
            result.Ano.Should().Be(request.Ano);
            result.Anotacoes.Should().Be(request.Anotacoes);
            result.ClienteId.Should().Be(request.ClienteId);
        }

        [Fact]
        public void MapearParaCadastrarVeiculoUseCaseDto_ComRequestNulo_DeveRetornarNulo()
        {
            // Act
            var result = _veiculoController.MapearParaCadastrarVeiculoUseCaseDto(null);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void MapearParaAtualizarVeiculoUseCaseDto_ComRequestValido_DeveMapearCorretamente()
        {
            // Arrange
            var request = new AtualizarVeiculoRequest
            {
                Placa = "XYZ9876",
                Marca = "Honda",
                Modelo = "Civic",
                Cor = "Branco",
                Ano = "2021",
                Anotacoes = "Veículo atualizado",
                ClienteId = Guid.NewGuid()
            };

            // Act
            var result = _veiculoController.MapearParaAtualizarVeiculoUseCaseDto(request);

            // Assert
            result.Should().NotBeNull();
            result.Placa.Should().Be(request.Placa);
            result.Marca.Should().Be(request.Marca);
            result.Modelo.Should().Be(request.Modelo);
            result.Cor.Should().Be(request.Cor);
            result.Ano.Should().Be(request.Ano);
            result.Anotacoes.Should().Be(request.Anotacoes);
            result.ClienteId.Should().Be(request.ClienteId);
        }

        [Fact]
        public void MapearParaAtualizarVeiculoUseCaseDto_ComRequestNulo_DeveRetornarNulo()
        {
            // Act
            var result = _veiculoController.MapearParaAtualizarVeiculoUseCaseDto(null);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Cadastrar_DeveUsarMapeamentoEChamarUseCase()
        {
            // Arrange
            var request = new CadastrarVeiculoRequest
            {
                Placa = "ABC1234",
                Marca = "Toyota",
                Modelo = "Corolla",
                Cor = "Preto",
                Ano = "2022",
                Anotacoes = "Veículo em bom estado",
                ClienteId = Guid.NewGuid()
            };

            var veiculo = new Veiculo();
            var veiculoResponse = new VeiculoResponse();

            _veiculoUseCases.CadastrarUseCaseAsync(Arg.Any<CadastrarVeiculoUseCaseDto>())
                .Returns(veiculo);

            _veiculoPresenter.ParaResponse(veiculo)
                .Returns(veiculoResponse);

            // Act
            var result = await _veiculoController.Cadastrar(request);

            // Assert
            await _veiculoUseCases.Received(1).CadastrarUseCaseAsync(Arg.Is<CadastrarVeiculoUseCaseDto>(
                dto => dto.Placa == request.Placa &&
                      dto.Marca == request.Marca &&
                      dto.Modelo == request.Modelo &&
                      dto.Cor == request.Cor &&
                      dto.Ano == request.Ano &&
                      dto.Anotacoes == request.Anotacoes &&
                      dto.ClienteId == request.ClienteId));
            
            _veiculoPresenter.Received(1).ParaResponse(veiculo);
            
            result.Should().Be(veiculoResponse);
        }

        [Fact]
        public async Task Atualizar_DeveUsarMapeamentoEChamarUseCase()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new AtualizarVeiculoRequest
            {
                Placa = "XYZ9876",
                Marca = "Honda",
                Modelo = "Civic",
                Cor = "Branco",
                Ano = "2021",
                Anotacoes = "Veículo atualizado",
                ClienteId = Guid.NewGuid()
            };

            var veiculo = new Veiculo();
            var veiculoResponse = new VeiculoResponse();

            _veiculoUseCases.AtualizarUseCaseAsync(id, Arg.Any<AtualizarVeiculoUseCaseDto>())
                .Returns(veiculo);

            _veiculoPresenter.ParaResponse(veiculo)
                .Returns(veiculoResponse);

            // Act
            var result = await _veiculoController.Atualizar(id, request);

            // Assert
            await _veiculoUseCases.Received(1).AtualizarUseCaseAsync(
                Arg.Is<Guid>(g => g == id),
                Arg.Is<AtualizarVeiculoUseCaseDto>(
                    dto => dto.Placa == request.Placa &&
                          dto.Marca == request.Marca &&
                          dto.Modelo == request.Modelo &&
                          dto.Cor == request.Cor &&
                          dto.Ano == request.Ano &&
                          dto.Anotacoes == request.Anotacoes &&
                          dto.ClienteId == request.ClienteId));
            
            _veiculoPresenter.Received(1).ParaResponse(veiculo);
            
            result.Should().Be(veiculoResponse);
        }
    }
}
