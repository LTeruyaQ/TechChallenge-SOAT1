using Aplicacao.DTOs.Requests.Servico;
using Aplicacao.DTOs.Responses.Servico;
using Aplicacao.Servicos;
using AutoMapper;
using Dominio.Entidades;
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
    public class ServicoServicoTests
    {
        private readonly Mock<IRepositorio<Servico>> _repositorioMock = new();
        private readonly Mock<ILogServico<ServicoServico>> _logServicoMock = new();
        private readonly Mock<IUnidadeDeTrabalho> _uotMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly ServicoServico _servicoServico;

        public ServicoServicoTests()
        {
            _servicoServico = new ServicoServico(
                _repositorioMock.Object,
                _logServicoMock.Object,
                _uotMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Dado_NomeExistente_Quando_CadastrarServicoAsync_Entao_LancaExcecaoDadosJaCadastrados()
        {
            // Arrange
            var request = new CadastrarServicoRequest 
            { 
                Nome = "Serviço X", 
                Descricao = "Descrição detalhada do serviço", 
                Valor = 150.50m, 
                Disponivel = true  
            };

            var servicoExistente = new Servico 
            { 
                Id = Guid.NewGuid(),
                Nome = request.Nome,
                Descricao = "Serviço já existente",
                Valor = 100.00m,
                Disponivel = true
            };

            _repositorioMock.Setup(r => r.ObterPorEspecificacaoAsync(It.IsAny<IServicoEspecificacao>()))
                .ReturnsAsync(new List<Servico> { servicoExistente });

            // Act
            Func<Task> act = async () => await _servicoServico.CadastrarAsync(request);

            // Assert
            await act.Should()
                .ThrowAsync<DadosJaCadastradosException>()
                .WithMessage("Já existe um serviço cadastrado com este nome");
                
            _logServicoMock.Verify(
                x => x.LogAviso("CadastrarAsync", It.IsAny<string>()),
                Times.Once,
                "porque deve registrar um aviso quando tentar cadastrar serviço com nome existente");
        }

        [Fact]
        public async Task Dado_RequestValido_Quando_CadastrarServicoAsync_Entao_RetornaServicoResponse()
        {
            // Arrange
            var request = new CadastrarServicoRequest 
            { 
                Nome = "Serviço Y", 
                Descricao = "Novo serviço", 
                Valor = 200.00m, 
                Disponivel = true  
            };

            var servico = new Servico 
            { 
                Id = Guid.NewGuid(),
                Nome = request.Nome,
                Descricao = request.Descricao,
                Valor = request.Valor,
                Disponivel = request.Disponivel
            };

            var response = new ServicoResponse 
            { 
                Id = servico.Id,
                Nome = servico.Nome,
                Descricao = servico.Descricao,
                Valor = servico.Valor,
                Disponivel = servico.Disponivel
            };

            _repositorioMock.Setup(r => r.ObterPorEspecificacaoAsync(It.IsAny<IServicoEspecificacao>()))
                .ReturnsAsync(new List<Servico>());
                
            _mapperMock.Setup(m => m.Map<Servico>(request)).Returns(servico);
            _repositorioMock.Setup(r => r.CadastrarAsync(servico)).ReturnsAsync(servico);
            _uotMock.Setup(u => u.SalvarAlteracoesAsync()).ReturnsAsync(true);
            _mapperMock.Setup(m => m.Map<ServicoResponse>(servico)).Returns(response);

            // Act
            var resultado = await _servicoServico.CadastrarAsync(request);

            // Assert
            resultado.Should().NotBeNull("porque o serviço foi cadastrado com sucesso");
            resultado.Id.Should().Be(servico.Id, "porque o ID deve ser o mesmo do serviço cadastrado");
            resultado.Nome.Should().Be(request.Nome, "porque o nome deve ser o mesmo da requisição");
            resultado.Descricao.Should().Be(request.Descricao, "porque a descrição deve ser a mesma da requisição");
            resultado.Valor.Should().Be(request.Valor, "porque o valor deve ser o mesmo da requisição");
            resultado.Disponivel.Should().BeTrue("porque o serviço foi cadastrado como disponível");
            
            _repositorioMock.Verify(
                r => r.CadastrarAsync(It.IsAny<Servico>()), 
                Times.Once, 
                "porque deve chamar o método de cadastro do repositório");
                
            _uotMock.Verify(
                u => u.SalvarAlteracoesAsync(), 
                Times.Once, 
                "porque deve salvar as alterações na unidade de trabalho");
        }

        [Fact]
        public async Task Dado_ServicoExistente_Quando_ObterPorIdAsync_Entao_RetornaServico()
        {
            // Arrange
            var servicoId = Guid.NewGuid();
            var servico = new Servico 
            { 
                Id = servicoId,
                Nome = "Serviço Z",
                Descricao = "Descrição do serviço Z",
                Valor = 150.00m,
                Disponivel = true
            };

            var servicoResponse = new ServicoResponse 
            { 
                Id = servico.Id,
                Nome = servico.Nome,
                Descricao = servico.Descricao,
                Valor = servico.Valor,
                Disponivel = servico.Disponivel
            };

            _repositorioMock.Setup(r => r.ObterPorIdAsync(servicoId))
                .ReturnsAsync(servico);
            _mapperMock.Setup(m => m.Map<ServicoResponse>(servico))
                .Returns(servicoResponse);

            // Act
            var resultado = await _servicoServico.ObterPorIdAsync(servicoId);

            // Assert
            resultado.Should().NotBeNull("porque o serviço existe no repositório");
            resultado.Id.Should().Be(servicoId, "porque deve retornar o serviço com o ID especificado");
            resultado.Nome.Should().Be(servico.Nome, "porque deve retornar o nome correto do serviço");
        }

        [Fact]
        public async Task Dado_ServicoInexistente_Quando_ObterPorIdAsync_Entao_LancaExcecao()
        {
            // Arrange
            var servicoId = Guid.NewGuid();
            _repositorioMock.Setup(r => r.ObterPorIdAsync(servicoId))
                .ReturnsAsync((Servico)null);

            // Act
            Func<Task> act = async () => await _servicoServico.ObterPorIdAsync(servicoId);

            // Assert
            await act.Should()
                .ThrowAsync<EntidadeNaoEncontradaException>()
                .WithMessage($"Serviço com ID {servicoId} não encontrado");
        }

        [Fact]
        public async Task Dado_ServicosExistentes_Quando_ListarTodosAsync_Entao_RetornaListaServicos()
        {
            // Arrange
            var servicos = new List<Servico>
            {
                new Servico { Id = Guid.NewGuid(), Nome = "Serviço A", Valor = 100.00m, Disponivel = true },
                new Servico { Id = Guid.NewGuid(), Nome = "Serviço B", Valor = 200.00m, Disponivel = true }
            };

            var servicosResponse = servicos.Select(s => new ServicoResponse 
            { 
                Id = s.Id, 
                Nome = s.Nome,
                Valor = s.Valor,
                Disponivel = s.Disponivel
            }).ToList();

            _repositorioMock.Setup(r => r.ListarTodosAsync())
                .ReturnsAsync(servicos);
            _mapperMock.Setup(m => m.Map<IEnumerable<ServicoResponse>>(servicos))
                .Returns(servicosResponse);

            // Act
            var resultado = await _servicoServico.ListarTodosAsync();

            // Assert
            resultado.Should().NotBeNull("porque existem serviços cadastrados");
            resultado.Should().HaveCount(2, "porque existem dois serviços cadastrados");
            resultado.Should().Contain(s => s.Nome == "Serviço A", "porque o primeiro serviço deve estar na lista");
            resultado.Should().Contain(s => s.Nome == "Serviço B", "porque o segundo serviço deve estar na lista");
        }
    }
}
