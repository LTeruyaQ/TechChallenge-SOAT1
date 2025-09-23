using Adapters.Gateways;
using Core.DTOs.Entidades.Servico;
using Core.Entidades;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.Handlers;
using System;
using System.Reflection;
using Xunit;

namespace MecanicaOS.UnitTests.Core.DTOs.Entidades.Servico
{
    public class ServicoEntityDtoTests
    {
        private readonly ServicoHandlerFixture _fixture;

        public ServicoEntityDtoTests()
        {
            _fixture = new ServicoHandlerFixture();
        }

        [Fact]
        public void ToDto_DeveConverterTodosOsCamposCorretamente()
        {
            // Arrange
            var servico = ServicoHandlerFixture.CriarServicoValido(
                id: Guid.NewGuid(),
                nome: "Serviço de Teste",
                descricao: "Descrição do serviço de teste",
                valor: 199.99m,
                disponivel: true
            );
            
            servico.DataCadastro = DateTime.UtcNow.AddDays(-10);
            servico.DataAtualizacao = DateTime.UtcNow.AddDays(-5);
            servico.Ativo = true;

            // Act
            var dto = InvokeToDto(servico);

            // Assert
            dto.Should().NotBeNull();
            dto.Id.Should().Be(servico.Id);
            dto.Nome.Should().Be(servico.Nome);
            dto.Descricao.Should().Be(servico.Descricao);
            dto.Valor.Should().Be(servico.Valor);
            dto.Disponivel.Should().Be(servico.Disponivel);
            
            // Campos técnicos de auditoria
            dto.Ativo.Should().Be(servico.Ativo);
            dto.DataCadastro.Should().Be(servico.DataCadastro);
            dto.DataAtualizacao.Should().Be(servico.DataAtualizacao);
        }

        [Fact]
        public void FromDto_DeveConverterTodosOsCamposCorretamente()
        {
            // Arrange
            var dto = new ServicoEntityDto
            {
                Id = Guid.NewGuid(),
                Nome = "Serviço de Teste DTO",
                Descricao = "Descrição do serviço de teste DTO",
                Valor = 299.99m,
                Disponivel = true,
                Ativo = true,
                DataCadastro = DateTime.UtcNow.AddDays(-15),
                DataAtualizacao = DateTime.UtcNow.AddDays(-2)
            };

            // Act
            var servico = InvokeFromDto(dto);

            // Assert
            servico.Should().NotBeNull();
            servico.Id.Should().Be(dto.Id);
            servico.Nome.Should().Be(dto.Nome);
            servico.Descricao.Should().Be(dto.Descricao);
            servico.Valor.Should().Be(dto.Valor);
            servico.Disponivel.Should().Be(dto.Disponivel);
            
            // Campos técnicos de auditoria
            servico.Ativo.Should().Be(dto.Ativo);
            servico.DataCadastro.Should().Be(dto.DataCadastro);
            servico.DataAtualizacao.Should().Be(dto.DataAtualizacao);
        }

        [Fact]
        public void ConversaoBidirecional_DevePreservarTodosOsDados()
        {
            // Arrange
            var servicoOriginal = ServicoHandlerFixture.CriarServicoValido(
                id: Guid.NewGuid(),
                nome: "Serviço Bidirecional",
                descricao: "Teste de conversão bidirecional",
                valor: 399.99m,
                disponivel: false
            );
            
            servicoOriginal.DataCadastro = DateTime.UtcNow.AddDays(-30);
            servicoOriginal.DataAtualizacao = DateTime.UtcNow.AddHours(-12);
            servicoOriginal.Ativo = true;

            // Act
            var dto = InvokeToDto(servicoOriginal);
            var servicoConvertido = InvokeFromDto(dto);

            // Assert
            servicoConvertido.Should().NotBeNull();
            servicoConvertido.Id.Should().Be(servicoOriginal.Id);
            servicoConvertido.Nome.Should().Be(servicoOriginal.Nome);
            servicoConvertido.Descricao.Should().Be(servicoOriginal.Descricao);
            servicoConvertido.Valor.Should().Be(servicoOriginal.Valor);
            servicoConvertido.Disponivel.Should().Be(servicoOriginal.Disponivel);
            
            // Campos técnicos de auditoria
            servicoConvertido.Ativo.Should().Be(servicoOriginal.Ativo);
            servicoConvertido.DataCadastro.Should().Be(servicoOriginal.DataCadastro);
            servicoConvertido.DataAtualizacao.Should().Be(servicoOriginal.DataAtualizacao);
        }
        
        // Métodos auxiliares para invocar os métodos privados do ServicoGateway via reflexão
        private ServicoEntityDto InvokeToDto(global::Core.Entidades.Servico servico)
        {
            var type = typeof(ServicoGateway);
            var method = type.GetMethod("ToDto", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
            return (ServicoEntityDto)method.Invoke(null, new object[] { servico });
        }
        
        private global::Core.Entidades.Servico InvokeFromDto(ServicoEntityDto dto)
        {
            var type = typeof(ServicoGateway);
            var method = type.GetMethod("FromDto", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
            return (global::Core.Entidades.Servico)method.Invoke(null, new object[] { dto });
        }
    }
}
