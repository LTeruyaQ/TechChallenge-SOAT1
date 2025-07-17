using Aplicacao.DTOs.Requests.Veiculo;
using Aplicacao.DTOs.Responses.Veiculo;
using Aplicacao.Servicos;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Aplicacao.Servicos.Tests
{
    public class VeiculoServicoTests
    {
        private readonly Mock<IRepositorio<Veiculo>> _repositorioMock;
        private readonly Mock<ILogServico<VeiculoServico>> _logMock;
        private readonly Mock<IUnidadeDeTrabalho> _uotMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly VeiculoServico _servico;

        public VeiculoServicoTests()
        {
            _repositorioMock = new Mock<IRepositorio<Veiculo>>();
            _logMock = new Mock<ILogServico<VeiculoServico>>();
            _uotMock = new Mock<IUnidadeDeTrabalho>();
            _mapperMock = new Mock<IMapper>();
            _servico = new VeiculoServico(
                _repositorioMock.Object, 
                _logMock.Object, 
                _uotMock.Object, 
                _mapperMock.Object);
        }

        private Veiculo CriarVeiculo() => new Veiculo
        {
            Id = Guid.NewGuid(),
            Placa = "ABC1234",
            Marca = "Toyota",
            Modelo = "Corolla",
            Cor = "Preto",
            Ano = "2020",
            ClienteId = Guid.NewGuid()
        };

        [Fact]
        public async Task Dado_RequestValido_Quando_CadastrarAsync_Entao_RetornaVeiculoResponse()
        {
            // Arrange
            var request = new CadastrarVeiculoRequest 
            { 
                Placa = "XYZ1234",
                Marca = "Honda",
                Modelo = "Civic",
                Cor = "Prata",
                Ano = "2021",
                ClienteId = Guid.NewGuid()
            };

            var veiculo = new Veiculo
            {
                Id = Guid.NewGuid(),
                Placa = request.Placa,
                Marca = request.Marca,
                Modelo = request.Modelo,
                Cor = request.Cor,
                Ano = request.Ano,
                ClienteId = request.ClienteId.Value
            };

            var response = new VeiculoResponse
            {
                Id = veiculo.Id,
                Placa = veiculo.Placa,
                Marca = veiculo.Marca,
                Modelo = veiculo.Modelo,
                Cor = veiculo.Cor,
                Ano = veiculo.Ano,
                ClienteId = veiculo.ClienteId
            };

            _repositorioMock.Setup(r => r.CadastrarAsync(It.IsAny<Veiculo>()))
                .ReturnsAsync(veiculo);
                
            _mapperMock.Setup(m => m.Map<Veiculo>(request))
                .Returns(veiculo);
                
            _mapperMock.Setup(m => m.Map<VeiculoResponse>(veiculo))
                .Returns(response);
                
            _uotMock.Setup(u => u.SalvarAlteracoesAsync())
                .ReturnsAsync(true);

            // Act
            var resultado = await _servico.CadastrarAsync(request);

            // Assert
            resultado.Should().NotBeNull("porque o veículo foi cadastrado com sucesso");
            resultado.Id.Should().Be(veiculo.Id, "porque o ID deve ser o mesmo do veículo cadastrado");
            resultado.Placa.Should().Be(request.Placa, "porque a placa deve ser a mesma da requisição");
            resultado.Marca.Should().Be(request.Marca, "porque a marca deve ser a mesma da requisição");
            resultado.Modelo.Should().Be(request.Modelo, "porque o modelo deve ser o mesmo da requisição");
            resultado.Cor.Should().Be(request.Cor, "porque a cor deve ser a mesma da requisição");
            resultado.Ano.Should().Be(request.Ano, "porque o ano deve ser o mesmo da requisição");
            resultado.ClienteId.Should().Be(request.ClienteId.Value, "porque o ClienteId deve ser o mesmo da requisição");
            
            _repositorioMock.Verify(
                r => r.CadastrarAsync(It.IsAny<Veiculo>()), 
                Times.Once, 
                "porque deve chamar o método de cadastro do repositório");
                
            _uotMock.Verify(
                u => u.SalvarAlteracoesAsync(), 
                Times.Once, 
                "porque deve salvar as alterações na unidade de trabalho");
        }

        [Fact]
        public async Task Dado_PlacaExistente_Quando_CadastrarAsync_Entao_LancaExcecaoDadosJaCadastrados()
        {
            // Arrange
            var request = new CadastrarVeiculoRequest 
            { 
                Placa = "XYZ1234",
                Marca = "Honda",
                Modelo = "Civic",
                Cor = "Prata",
                Ano = "2021",
                ClienteId = Guid.NewGuid()
            };

            var veiculoExistente = CriarVeiculo();
            veiculoExistente.Placa = request.Placa;

            _repositorioMock.Setup(r => r.ObterUmSemRastreamentoAsync(It.IsAny<VeiculoPorPlacaEspecificacao>()))
                .ReturnsAsync(veiculoExistente);

            // Act
            Func<Task> act = async () => await _servico.CadastrarAsync(request);

            // Assert
            await act.Should()
                .ThrowAsync<DadosJaCadastradosException>()
                .WithMessage("Já existe um veículo cadastrado com esta placa");
                
            _logMock.Verify(
                x => x.LogAviso("CadastrarAsync", It.IsAny<string>()),
                Times.Once,
                "porque deve registrar um aviso quando tentar cadastrar veículo com placa existente");
        }

        [Fact]
        public async Task Dado_VeiculoExistente_Quando_ObterPorIdAsync_Entao_RetornaVeiculo()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();
            var veiculo = CriarVeiculo();
            veiculo.Id = veiculoId;

            var veiculoResponse = new VeiculoResponse
            {
                Id = veiculo.Id,
                Placa = veiculo.Placa,
                Marca = veiculo.Marca,
                Modelo = veiculo.Modelo,
                Cor = veiculo.Cor,
                Ano = veiculo.Ano,
                ClienteId = veiculo.ClienteId
            };

            _repositorioMock.Setup(r => r.ObterPorIdAsync(veiculoId))
                .ReturnsAsync(veiculo);
                
            _mapperMock.Setup(m => m.Map<VeiculoResponse>(veiculo))
                .Returns(veiculoResponse);

            // Act
            var resultado = await _servico.ObterPorIdAsync(veiculoId);

            // Assert
            resultado.Should().NotBeNull("porque o veículo existe no repositório");
            resultado.Id.Should().Be(veiculoId, "porque deve retornar o veículo com o ID especificado");
            resultado.Placa.Should().Be(veiculo.Placa, "porque a placa deve ser a mesma do veículo cadastrado");
        }

        [Fact]
        public async Task Dado_VeiculoInexistente_Quando_ObterPorIdAsync_Entao_LancaExcecao()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();
            _repositorioMock.Setup(r => r.ObterPorIdAsync(veiculoId))
                .ReturnsAsync((Veiculo)null);

            // Act
            Func<Task> act = async () => await _servico.ObterPorIdAsync(veiculoId);

            // Assert
            await act.Should()
                .ThrowAsync<EntidadeNaoEncontradaException>()
                .WithMessage($"Veículo com ID {veiculoId} não encontrado");
        }

        [Fact]
        public async Task Dado_VeiculosExistentes_Quando_ListarPorClienteAsync_Entao_RetornaListaVeiculos()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var veiculos = new List<Veiculo>
            {
                new Veiculo { Id = Guid.NewGuid(), Placa = "ABC1234", ClienteId = clienteId },
                new Veiculo { Id = Guid.NewGuid(), Placa = "XYZ5678", ClienteId = clienteId }
            };

            var veiculosResponse = veiculos.Select(v => new VeiculoResponse 
            { 
                Id = v.Id, 
                Placa = v.Placa,
                ClienteId = v.ClienteId
            }).ToList();

            _repositorioMock.Setup(r => r.ListarAsync(It.IsAny<VeiculoPorClienteEspecificacao>()))
                .ReturnsAsync(veiculos);
                
            _mapperMock.Setup(m => m.Map<IEnumerable<VeiculoResponse>>(veiculos))
                .Returns(veiculosResponse);

            // Act
            var resultado = await _servico.ListarPorClienteAsync(clienteId);

            // Assert
            resultado.Should().NotBeNull("porque existem veículos cadastrados para o cliente");
            resultado.Should().HaveCount(2, "porque existem dois veículos cadastrados para o cliente");
            resultado.Should().OnlyContain(v => v.ClienteId == clienteId, "porque todos os veículos devem pertencer ao cliente especificado");
            resultado.Should().Contain(v => v.Placa == "ABC1234", "porque o primeiro veículo deve estar na lista");
            resultado.Should().Contain(v => v.Placa == "XYZ5678", "porque o segundo veículo deve estar na lista");
        }
    }
}
