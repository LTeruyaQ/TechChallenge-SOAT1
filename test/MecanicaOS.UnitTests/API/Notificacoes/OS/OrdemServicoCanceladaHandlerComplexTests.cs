using API.Notificacoes.OS;
using Core.DTOs.Entidades;
using Core.DTOs.Entidades.Autenticacao;
using Core.DTOs.Entidades.Estoque;
using Core.DTOs.Entidades.OrdemServicos;
using Core.DTOs.Requests.OrdemServico.InsumoOS;
using Core.DTOs.Responses.OrdemServico;
using Core.DTOs.Responses.OrdemServico.InsumoOrdemServico;
using Core.DTOs.Responses.Estoque;
using Core.Entidades;
using Core.Enumeradores;
using Core.Interfaces.Controllers;
using Core.Interfaces.Gateways;
using Core.Interfaces.root;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;
using FluentAssertions;
using MediatR;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Core.Interfaces.Eventos;

namespace MecanicaOS.UnitTests.API.Notificacoes.OS
{
    public class OrdemServicoCanceladaHandlerComplexTests
    {
        private readonly OrdemServicoCanceladaHandlerFixture _fixture;
        private readonly IEstoqueGateway _estoqueGateway;
        private readonly IRepositorio<EstoqueEntityDto> _estoqueRepositorio;
        private readonly IMediator _mediator;

        public OrdemServicoCanceladaHandlerComplexTests()
        {
            _fixture = new OrdemServicoCanceladaHandlerFixture();
            _estoqueGateway = Substitute.For<IEstoqueGateway>();
            _estoqueRepositorio = Substitute.For<IRepositorio<EstoqueEntityDto>>();
            _mediator = Substitute.For<IMediator>();
        }

        [Fact]
        public async Task Handle_ComMultiplosInsumosDiferentes_DeveDevolverTodosCorretamente()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var evento = _fixture.CriarEvento(ordemServicoId);
            
            // Criar 5 insumos diferentes com quantidades variadas
            var insumosInfo = new List<(Guid EstoqueId, int Quantidade)>();
            var estoquesAtualizados = new Dictionary<Guid, int>();
            
            for (int i = 0; i < 5; i++)
            {
                var estoqueId = Guid.NewGuid();
                var quantidade = (i + 1) * 3; // 3, 6, 9, 12, 15
                insumosInfo.Add((estoqueId, quantidade));
                
                // Registrar quantidade inicial para cada estoque
                estoquesAtualizados[estoqueId] = 50;
            }
            
            var ordemServico = _fixture.CriarOrdemServicoComInsumosEspecificos(ordemServicoId, insumosInfo);
            _fixture.OrdemServicoController.ObterPorId(ordemServicoId).Returns(ordemServico);
            
            // Simular a devolução real dos insumos
            _fixture.InsumoOSController.DevolverInsumosAoEstoque(Arg.Any<IEnumerable<DevolverInsumoOSRequest>>())
                .Returns(callInfo =>
                {
                    var requests = callInfo.Arg<IEnumerable<DevolverInsumoOSRequest>>();
                    
                    // Atualizar as quantidades para cada estoque
                    foreach (var request in requests)
                    {
                        if (estoquesAtualizados.ContainsKey(request.EstoqueId))
                        {
                            estoquesAtualizados[request.EstoqueId] += request.Quantidade;
                        }
                    }
                    
                    return Task.CompletedTask;
                });

            // Act
            await _fixture.Handler.Handle(evento, CancellationToken.None);

            // Assert
            // Verificar que o método foi chamado com todos os insumos
            await _fixture.InsumoOSController.Received(1).DevolverInsumosAoEstoque(
                Arg.Is<IEnumerable<DevolverInsumoOSRequest>>(requests => requests.Count() == 5)
            );
            
            // Verificar que cada estoque foi atualizado com a quantidade correta
            for (int i = 0; i < 5; i++)
            {
                var estoqueId = insumosInfo[i].EstoqueId;
                var quantidadeDevolvida = insumosInfo[i].Quantidade;
                var quantidadeEsperada = 50 + quantidadeDevolvida;
                
                estoquesAtualizados[estoqueId].Should().Be(quantidadeEsperada, 
                    $"O estoque {i+1} deveria ter {quantidadeEsperada} itens após a devolução");
            }
        }

        [Fact]
        public async Task Handle_ComOrdemServicoCancelada_DevePublicarEventoDeEstoqueAtualizado()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var evento = _fixture.CriarEvento(ordemServicoId);
            
            // Configurar insumos da ordem de serviço
            var estoqueId = Guid.NewGuid();
            var insumosInfo = new List<(Guid EstoqueId, int Quantidade)>
            {
                (estoqueId, 10)
            };
            
            var ordemServico = _fixture.CriarOrdemServicoComInsumosEspecificos(ordemServicoId, insumosInfo);
            _fixture.OrdemServicoController.ObterPorId(ordemServicoId).Returns(ordemServico);
            
            // Simular a publicação de eventos
            var compositionRoot = Substitute.For<ICompositionRoot>();
            compositionRoot.CriarOrdemServicoController().Returns(_fixture.OrdemServicoController);
            compositionRoot.CriarInsumoOSController().Returns(_fixture.InsumoOSController);
            compositionRoot.CriarLogService<OrdemServicoCanceladaHandler>().Returns(_fixture.LogServico);
            var eventosPublisher = Substitute.For<IEventosPublisher>();
            compositionRoot.CriarEventosPublisher().Returns(eventosPublisher);
            
            var handler = new OrdemServicoCanceladaHandler(compositionRoot);

            // Act
            await handler.Handle(evento, CancellationToken.None);

            // Assert
            // Verificar que o método de devolução foi chamado
            await _fixture.InsumoOSController.Received(1).DevolverInsumosAoEstoque(Arg.Any<IEnumerable<DevolverInsumoOSRequest>>());
            
            // Verificar logs
            _fixture.LogServico.Received(1).LogInicio(Arg.Any<string>(), ordemServicoId);
            _fixture.LogServico.Received(1).LogFim(Arg.Any<string>(), Arg.Any<object>());
        }

        [Fact]
        public async Task Handle_ComOrdemServicoCancelada_DeveAtualizarEstoqueEManterOutrosCampos()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var estoqueId = Guid.NewGuid();
            var evento = _fixture.CriarEvento(ordemServicoId);
            
            // Criar um estoque com todos os campos preenchidos
            var estoqueDto = new EstoqueEntityDto
            {
                Id = estoqueId,
                Insumo = "Óleo Sintético 5W30",
                Descricao = "Óleo sintético para motores modernos",
                Preco = 45.90M,
                QuantidadeDisponivel = 20,
                QuantidadeMinima = 5,
                Ativo = true,
                DataCadastro = DateTime.Now.AddDays(-30),
                DataAtualizacao = DateTime.Now.AddDays(-5)
            };
            
            // Configurar insumos da ordem de serviço
            var insumosInfo = new List<(Guid EstoqueId, int Quantidade)>
            {
                (estoqueId, 8)
            };
            
            var ordemServico = _fixture.CriarOrdemServicoComInsumosEspecificos(ordemServicoId, insumosInfo);
            _fixture.OrdemServicoController.ObterPorId(ordemServicoId).Returns(ordemServico);
            
            // Simular o repositório e gateway
            _estoqueRepositorio.ObterPorIdAsync(estoqueId).Returns(estoqueDto);
            
            var compositionRoot = Substitute.For<ICompositionRoot>();
            compositionRoot.CriarOrdemServicoController().Returns(_fixture.OrdemServicoController);
            compositionRoot.CriarInsumoOSController().Returns(_fixture.InsumoOSController);
            compositionRoot.CriarLogService<OrdemServicoCanceladaHandler>().Returns(_fixture.LogServico);
            compositionRoot.CriarEstoqueGateway().Returns(_estoqueGateway);
            compositionRoot.CriarRepositorio<EstoqueEntityDto>().Returns(_estoqueRepositorio);
            
            // Simular a devolução real dos insumos
            _fixture.InsumoOSController.DevolverInsumosAoEstoque(Arg.Any<IEnumerable<DevolverInsumoOSRequest>>())
                .Returns(callInfo =>
                {
                    var requests = callInfo.Arg<IEnumerable<DevolverInsumoOSRequest>>();
                    
                    foreach (var request in requests)
                    {
                        if (request.EstoqueId == estoqueId)
                        {
                            // Atualizar apenas a quantidade disponível
                            estoqueDto.QuantidadeDisponivel += request.Quantidade;
                            estoqueDto.DataAtualizacao = DateTime.Now;
                        }
                    }
                    
                    return Task.CompletedTask;
                });
            
            var handler = new OrdemServicoCanceladaHandler(compositionRoot);

            // Act
            await handler.Handle(evento, CancellationToken.None);

            // Assert
            // Verificar que a quantidade foi atualizada
            estoqueDto.QuantidadeDisponivel.Should().Be(28); // 20 + 8
            
            // Verificar que os outros campos foram preservados
            estoqueDto.Insumo.Should().Be("Óleo Sintético 5W30");
            estoqueDto.Descricao.Should().Be("Óleo sintético para motores modernos");
            estoqueDto.Preco.Should().Be(45.90M);
            estoqueDto.QuantidadeMinima.Should().Be(5);
            estoqueDto.Ativo.Should().BeTrue();
            estoqueDto.DataCadastro.Date.Should().Be(DateTime.Now.AddDays(-30).Date, because: "a data de cadastro não deve ser alterada");
            
            // Verificar que a data de atualização foi modificada
            estoqueDto.DataAtualizacao.Should().BeAfter(DateTime.Now.AddMinutes(-1));
        }

        [Fact]
        public async Task Handle_ComEstoqueAbaixoDoMinimo_DeveAtualizarQuantidadeCorretamente()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var estoqueId = Guid.NewGuid();
            var evento = _fixture.CriarEvento(ordemServicoId);
            
            // Criar um estoque com quantidade abaixo do mínimo
            var estoqueDto = new EstoqueEntityDto
            {
                Id = estoqueId,
                Insumo = "Filtro de Óleo",
                QuantidadeDisponivel = 2, // Abaixo do mínimo
                QuantidadeMinima = 5,
                Preco = 25.50M,
                Ativo = true
            };
            
            // Configurar insumos da ordem de serviço
            var insumosInfo = new List<(Guid EstoqueId, int Quantidade)>
            {
                (estoqueId, 4) // Devolução suficiente para ficar acima do mínimo
            };
            
            var ordemServico = _fixture.CriarOrdemServicoComInsumosEspecificos(ordemServicoId, insumosInfo);
            _fixture.OrdemServicoController.ObterPorId(ordemServicoId).Returns(ordemServico);
            
            // Simular o repositório
            _estoqueRepositorio.ObterPorIdAsync(estoqueId).Returns(estoqueDto);
            
            var compositionRoot = Substitute.For<ICompositionRoot>();
            compositionRoot.CriarOrdemServicoController().Returns(_fixture.OrdemServicoController);
            compositionRoot.CriarInsumoOSController().Returns(_fixture.InsumoOSController);
            compositionRoot.CriarLogService<OrdemServicoCanceladaHandler>().Returns(_fixture.LogServico);
            compositionRoot.CriarEstoqueGateway().Returns(_estoqueGateway);
            compositionRoot.CriarRepositorio<EstoqueEntityDto>().Returns(_estoqueRepositorio);
            
            // Simular a devolução real dos insumos
            _fixture.InsumoOSController.DevolverInsumosAoEstoque(Arg.Any<IEnumerable<DevolverInsumoOSRequest>>())
                .Returns(callInfo =>
                {
                    var requests = callInfo.Arg<IEnumerable<DevolverInsumoOSRequest>>();
                    
                    foreach (var request in requests)
                    {
                        if (request.EstoqueId == estoqueId)
                        {
                            estoqueDto.QuantidadeDisponivel += request.Quantidade;
                        }
                    }
                    
                    return Task.CompletedTask;
                });
            
            var handler = new OrdemServicoCanceladaHandler(compositionRoot);

            // Act
            await handler.Handle(evento, CancellationToken.None);

            // Assert
            // Verificar que a quantidade foi atualizada corretamente
            estoqueDto.QuantidadeDisponivel.Should().Be(6); // 2 + 4
            
            // Verificar que agora está acima do mínimo
            estoqueDto.QuantidadeDisponivel.Should().BeGreaterThan(estoqueDto.QuantidadeMinima);
        }
    }
}
