using API.Notificacoes.OS;
using Core.DTOs.Requests.OrdemServico.InsumoOS;
using Core.DTOs.Responses.OrdemServico;
using Core.DTOs.Responses.OrdemServico.InsumoOrdemServico;
using Core.DTOs.Responses.Estoque;
using Core.Entidades;
using Core.Enumeradores;
using Core.Interfaces.Controllers;
using Core.Interfaces.Gateways;
using Core.Interfaces.root;
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

namespace MecanicaOS.UnitTests.API.Notificacoes.OS
{
    public class OrdemServicoCanceladaHandlerIntegrationTests
    {
        private readonly IOrdemServicoController _ordemServicoController;
        private readonly IInsumoOSController _insumosOSController;
        private readonly ILogServico<OrdemServicoCanceladaHandler> _logServico;
        private readonly ICompositionRoot _compositionRoot;
        private readonly IOrdemServicoUseCases _ordemServicoUseCases;
        private readonly IInsumoOSUseCases _insumoOSUseCases;
        private readonly IEstoqueGateway _estoqueGateway;
        private readonly OrdemServicoCanceladaHandler _handler;
        private readonly IMediator _mediator;

        public OrdemServicoCanceladaHandlerIntegrationTests()
        {
            // Configurar mocks para todos os componentes necessários
            _ordemServicoUseCases = Substitute.For<IOrdemServicoUseCases>();
            _insumoOSUseCases = Substitute.For<IInsumoOSUseCases>();
            _estoqueGateway = Substitute.For<IEstoqueGateway>();
            _logServico = Substitute.For<ILogServico<OrdemServicoCanceladaHandler>>();
            _mediator = Substitute.For<IMediator>();
            
            // Configurar controllers
            _ordemServicoController = Substitute.For<IOrdemServicoController>();
            _insumosOSController = Substitute.For<IInsumoOSController>();
            
            // Configurar CompositionRoot
            _compositionRoot = Substitute.For<ICompositionRoot>();
            _compositionRoot.CriarOrdemServicoController().Returns(_ordemServicoController);
            _compositionRoot.CriarInsumoOSController().Returns(_insumosOSController);
            _compositionRoot.CriarLogService<OrdemServicoCanceladaHandler>().Returns(_logServico);
            
            // Criar o handler
            _handler = new OrdemServicoCanceladaHandler(_compositionRoot);
        }

        [Fact]
        public async Task Handle_DeveProcessarCancelamentoEDevolverInsumos_EndToEnd()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var clienteId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var servicoId = Guid.NewGuid();
            var estoqueId1 = Guid.NewGuid();
            var estoqueId2 = Guid.NewGuid();
            
            // Configurar insumos da ordem de serviço
            var insumos = new List<InsumoOSResponse>
            {
                new InsumoOSResponse 
                { 
                    OrdemServicoId = ordemServicoId, 
                    EstoqueId = estoqueId1, 
                    Quantidade = 5,
                    Estoque = new EstoqueResponse
                    {
                        Id = estoqueId1,
                        Insumo = "Óleo de Motor",
                        Preco = 25.0,
                        QuantidadeDisponivel = 15
                    }
                },
                new InsumoOSResponse 
                { 
                    OrdemServicoId = ordemServicoId, 
                    EstoqueId = estoqueId2, 
                    Quantidade = 3,
                    Estoque = new EstoqueResponse
                    {
                        Id = estoqueId2,
                        Insumo = "Filtro de Ar",
                        Preco = 45.0,
                        QuantidadeDisponivel = 8
                    }
                }
            };
            
            // Configurar ordem de serviço completa
            var ordemServico = new OrdemServicoResponse
            {
                Id = ordemServicoId,
                ClienteId = clienteId,
                VeiculoId = veiculoId,
                ServicoId = servicoId,
                Status = StatusOrdemServico.Cancelada,
                Descricao = "Troca de óleo e filtros",
                Insumos = insumos
            };
            
            // Configurar comportamento do controller de ordem de serviço
            _ordemServicoController.ObterPorId(ordemServicoId).Returns(ordemServico);
            
            // Configurar evento de cancelamento
            var evento = new OrdemServicoCanceladaEvent(ordemServicoId);

            // Act
            await _handler.Handle(evento, CancellationToken.None);

            // Assert
            // Verificar que o log foi registrado corretamente
            _logServico.Received(1).LogInicio("Handle", ordemServicoId);
            _logServico.Received(1).LogFim("Handle", Arg.Any<object>());
            
            // Verificar que a ordem de serviço foi obtida
            await _ordemServicoController.Received(1).ObterPorId(ordemServicoId);
            
            // Verificar que os insumos foram devolvidos ao estoque com os valores corretos
            await _insumosOSController.Received(1).DevolverInsumosAoEstoque(
                Arg.Is<IEnumerable<DevolverInsumoOSRequest>>(reqs => reqs.Count() == 2 && 
                    reqs.Any(r => r.EstoqueId == estoqueId1 && r.Quantidade == 5) && 
                    reqs.Any(r => r.EstoqueId == estoqueId2 && r.Quantidade == 3))
            );
        }

        [Fact]
        public async Task Handle_DeveProcessarFluxoCompleto_ComEstoqueRealAtualizado()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var estoqueId = Guid.NewGuid();
            
            // Configurar estoque inicial
            var estoque = new Estoque
            {
                Id = estoqueId,
                Insumo = "Óleo Sintético",
                QuantidadeDisponivel = 20,
                QuantidadeMinima = 5,
                Preco = 35.90M
            };
            
            // Configurar insumos da ordem de serviço
            var insumoQuantidade = 7;
            var insumos = new List<InsumoOSResponse>
            {
                new InsumoOSResponse 
                { 
                    OrdemServicoId = ordemServicoId, 
                    EstoqueId = estoqueId, 
                    Quantidade = insumoQuantidade,
                    Estoque = new EstoqueResponse
                    {
                        Id = estoqueId,
                        Insumo = "Óleo Sintético",
                        Preco = 35.90,
                        QuantidadeDisponivel = 20
                    }
                }
            };
            
            // Configurar ordem de serviço
            var ordemServico = new OrdemServicoResponse
            {
                Id = ordemServicoId,
                Status = StatusOrdemServico.Cancelada,
                Insumos = insumos
            };
            
            // Configurar comportamento dos mocks
            _ordemServicoController.ObterPorId(ordemServicoId).Returns(ordemServico);
            
            // Simular o comportamento real do InsumoOSController.DevolverInsumosAoEstoque
            _insumosOSController.DevolverInsumosAoEstoque(Arg.Any<IEnumerable<DevolverInsumoOSRequest>>())
                .Returns(callInfo =>
                {
                    // Capturar os argumentos passados
                    var requests = callInfo.Arg<IEnumerable<DevolverInsumoOSRequest>>();
                    
                    // Simular a chamada para o use case
                    _insumoOSUseCases.DevolverInsumosAoEstoqueUseCaseAsync(requests);
                    
                    return Task.CompletedTask;
                });
            
            // Configurar o comportamento do use case para simular a devolução real
            _insumoOSUseCases.DevolverInsumosAoEstoqueUseCaseAsync(Arg.Any<IEnumerable<DevolverInsumoOSRequest>>())
                .Returns(callInfo =>
                {
                    // Capturar os argumentos passados
                    var requests = callInfo.Arg<IEnumerable<DevolverInsumoOSRequest>>();
                    
                    // Simular a atualização do estoque
                    foreach (var request in requests)
                    {
                        if (request.EstoqueId == estoqueId)
                        {
                            // Atualizar a quantidade disponível no estoque
                            estoque.QuantidadeDisponivel += request.Quantidade;
                        }
                    }
                    
                    return Task.CompletedTask;
                });
            
            // Configurar evento de cancelamento
            var evento = new OrdemServicoCanceladaEvent(ordemServicoId);

            // Act
            await _handler.Handle(evento, CancellationToken.None);

            // Assert
            // Verificar que os insumos foram devolvidos ao estoque
            await _insumosOSController.Received(1).DevolverInsumosAoEstoque(Arg.Any<IEnumerable<DevolverInsumoOSRequest>>());
            
            // Verificar que o estoque foi atualizado corretamente
            estoque.QuantidadeDisponivel.Should().Be(27); // 20 (inicial) + 7 (devolvido)
        }

        [Fact]
        public async Task Handle_ComMultiplosInsumos_DeveDevolverTodosCorretamente()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            
            // Criar vários insumos com diferentes quantidades
            var insumos = new List<InsumoOSResponse>();
            var estoquesAtualizados = new Dictionary<Guid, int>();
            
            // Adicionar 5 insumos diferentes
            for (int i = 0; i < 5; i++)
            {
                var estoqueId = Guid.NewGuid();
                var quantidade = (i + 1) * 2; // 2, 4, 6, 8, 10
                
                insumos.Add(new InsumoOSResponse
                {
                    OrdemServicoId = ordemServicoId,
                    EstoqueId = estoqueId,
                    Quantidade = quantidade,
                    Estoque = new EstoqueResponse
                    {
                        Id = estoqueId,
                        Insumo = $"Insumo {i+1}",
                        Preco = 10.0 * (i + 1),
                        QuantidadeDisponivel = 50
                    }
                });
                
                // Registrar a quantidade inicial para cada estoque
                estoquesAtualizados[estoqueId] = 50;
            }
            
            // Configurar ordem de serviço
            var ordemServico = new OrdemServicoResponse
            {
                Id = ordemServicoId,
                Status = StatusOrdemServico.Cancelada,
                Insumos = insumos
            };
            
            // Configurar comportamento dos mocks
            _ordemServicoController.ObterPorId(ordemServicoId).Returns(ordemServico);
            
            // Simular a devolução real dos insumos
            _insumosOSController.DevolverInsumosAoEstoque(Arg.Any<IEnumerable<DevolverInsumoOSRequest>>())
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
            
            // Configurar evento de cancelamento
            var evento = new OrdemServicoCanceladaEvent(ordemServicoId);

            // Act
            await _handler.Handle(evento, CancellationToken.None);

            // Assert
            // Verificar que o método foi chamado com todos os insumos
            await _insumosOSController.Received(1).DevolverInsumosAoEstoque(
                Arg.Is<IEnumerable<DevolverInsumoOSRequest>>(x => x.Count() == 5)
            );
            
            // Verificar que cada estoque foi atualizado com a quantidade correta
            for (int i = 0; i < 5; i++)
            {
                var estoqueId = insumos[i].EstoqueId;
                var quantidadeDevolvida = insumos[i].Quantidade;
                var quantidadeEsperada = 50 + quantidadeDevolvida;
                
                estoquesAtualizados[estoqueId].Should().Be(quantidadeEsperada, 
                    $"O estoque {i+1} deveria ter {quantidadeEsperada} itens após a devolução");
            }
        }
    }
}
