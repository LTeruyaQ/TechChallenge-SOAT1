using API.Notificacoes.OS;
using Core.DTOs.Requests.OrdemServico.InsumoOS;
using Core.DTOs.Responses.Estoque;
using Core.DTOs.Responses.OrdemServico;
using Core.DTOs.Responses.OrdemServico.InsumoOrdemServico;
using Core.Entidades;
using Core.Enumeradores;
using Core.Interfaces.Controllers;
using Core.Interfaces.Gateways;
using Core.Interfaces.root;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;
using MediatR;

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

                    return Task.FromResult(true);
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

                    return Task.FromResult(true);
                });

            // Configurar evento de cancelamento
            var evento = new OrdemServicoCanceladaEvent(ordemServicoId);
            await _handler.Handle(evento, CancellationToken.None);

            // Assert
            // Verificar que os insumos foram devolvidos ao estoque
            await _insumosOSController.Received(1).DevolverInsumosAoEstoque(Arg.Any<IEnumerable<DevolverInsumoOSRequest>>());

            // Verificar que o estoque foi atualizado corretamente
            estoque.QuantidadeDisponivel.Should().Be(27); // 20 (inicial) + 7 (devolvido)
        }

    }
}
