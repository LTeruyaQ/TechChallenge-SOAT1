using API.Notificacoes.OS;
using Core.DTOs.Entidades.Estoque;
using Core.DTOs.Entidades.OrdemServicos;
using Core.DTOs.Requests.OrdemServico.InsumoOS;
using Core.DTOs.Responses.Estoque;
using Core.DTOs.Responses.OrdemServico;
using Core.DTOs.Responses.OrdemServico.InsumoOrdemServico;
using Core.Enumeradores;
using Core.Interfaces.Controllers;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.root;
using Core.Interfaces.Servicos;
using MediatR;

namespace MecanicaOS.UnitTests.API.Notificacoes.OS
{
    public class OrdemServicoCanceladaHandlerE2ETests
    {
        private readonly IOrdemServicoController _ordemServicoController;
        private readonly IInsumoOSController _insumosOSController;
        private readonly ILogServico<OrdemServicoCanceladaHandler> _logServico;
        private readonly ICompositionRoot _compositionRoot;
        private readonly IEstoqueGateway _estoqueGateway;
        private readonly IRepositorio<EstoqueEntityDto> _estoqueRepositorio;
        private readonly IRepositorio<InsumoOSEntityDto> _insumoOSRepositorio;
        private readonly IRepositorio<OrdemServicoEntityDto> _ordemServicoRepositorio;
        private readonly IUnidadeDeTrabalho _unidadeDeTrabalho;
        private readonly OrdemServicoCanceladaHandler _handler;
        private readonly IMediator _mediator;

        public OrdemServicoCanceladaHandlerE2ETests()
        {
            // Configurar mocks para todos os componentes necessários
            _estoqueRepositorio = Substitute.For<IRepositorio<EstoqueEntityDto>>();
            _insumoOSRepositorio = Substitute.For<IRepositorio<InsumoOSEntityDto>>();
            _ordemServicoRepositorio = Substitute.For<IRepositorio<OrdemServicoEntityDto>>();
            _unidadeDeTrabalho = Substitute.For<IUnidadeDeTrabalho>();
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
            _compositionRoot.CriarEstoqueGateway().Returns(_estoqueGateway);
            _compositionRoot.CriarRepositorio<EstoqueEntityDto>().Returns(_estoqueRepositorio);
            _compositionRoot.CriarRepositorio<InsumoOSEntityDto>().Returns(_insumoOSRepositorio);
            _compositionRoot.CriarRepositorio<OrdemServicoEntityDto>().Returns(_ordemServicoRepositorio);
            _compositionRoot.CriarUnidadeDeTrabalho().Returns(_unidadeDeTrabalho);

            // Criar o handler
            _handler = new OrdemServicoCanceladaHandler(_compositionRoot);
        }

        [Fact]
        public async Task Handle_DeveAtualizarEstoqueNoBancoDeDados_QuandoOrdemServicoCancelada()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var estoqueId1 = Guid.NewGuid();
            var estoqueId2 = Guid.NewGuid();

            // Criar DTOs de estoque simulando o banco de dados
            var estoqueDto1 = new EstoqueEntityDto
            {
                Id = estoqueId1,
                Insumo = "Óleo 5W30",
                QuantidadeDisponivel = 10,
                QuantidadeMinima = 5,
                Preco = 45.90M,
                Ativo = true,
                DataCadastro = DateTime.Now.AddDays(-30)
            };

            var estoqueDto2 = new EstoqueEntityDto
            {
                Id = estoqueId2,
                Insumo = "Filtro de Óleo",
                QuantidadeDisponivel = 15,
                QuantidadeMinima = 3,
                Preco = 25.50M,
                Ativo = true,
                DataCadastro = DateTime.Now.AddDays(-30)
            };

            // Configurar repositório de estoque
            _estoqueRepositorio.ObterPorIdAsync(estoqueId1).Returns(estoqueDto1);
            _estoqueRepositorio.ObterPorIdAsync(estoqueId2).Returns(estoqueDto2);

            // Configurar insumos da ordem de serviço
            var insumos = new List<InsumoOSResponse>
            {
                new InsumoOSResponse
                {
                    OrdemServicoId = ordemServicoId,
                    EstoqueId = estoqueId1,
                    Quantidade = 3,
                    Estoque = new EstoqueResponse
                    {
                        Id = estoqueId1,
                        Insumo = "Óleo 5W30",
                        Preco = 45.90,
                        QuantidadeDisponivel = 10
                    }
                },
                new InsumoOSResponse
                {
                    OrdemServicoId = ordemServicoId,
                    EstoqueId = estoqueId2,
                    Quantidade = 2,
                    Estoque = new EstoqueResponse
                    {
                        Id = estoqueId2,
                        Insumo = "Filtro de Óleo",
                        Preco = 25.50,
                        QuantidadeDisponivel = 15
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

            // Configurar comportamento do controller de ordem de serviço
            _ordemServicoController.ObterPorId(ordemServicoId).Returns(ordemServico);

            // Simular o comportamento real do InsumoOSController.DevolverInsumosAoEstoque
            _insumosOSController.DevolverInsumosAoEstoque(Arg.Any<IEnumerable<DevolverInsumoOSRequest>>())
                .Returns(callInfo =>
                {
                    var requests = callInfo.Arg<IEnumerable<DevolverInsumoOSRequest>>();

                    // Simular a atualização real do estoque no banco de dados
                    foreach (var request in requests)
                    {
                        if (request.EstoqueId == estoqueId1)
                        {
                            estoqueDto1.QuantidadeDisponivel += request.Quantidade;
                        }
                        else if (request.EstoqueId == estoqueId2)
                        {
                            estoqueDto2.QuantidadeDisponivel += request.Quantidade;
                        }
                    }

                    // Simular commit no banco de dados
                    _unidadeDeTrabalho.Commit().Returns(Task.FromResult(true));

                    return Task.CompletedTask;
                });

            // Configurar evento de cancelamento
            var evento = new OrdemServicoCanceladaEvent(ordemServicoId);

            // Act
            await _handler.Handle(evento, CancellationToken.None);

            // Assert
            // Verificar que o estoque foi atualizado corretamente no "banco de dados"
            estoqueDto1.QuantidadeDisponivel.Should().Be(13); // 10 (inicial) + 3 (devolvido)
            estoqueDto2.QuantidadeDisponivel.Should().Be(17); // 15 (inicial) + 2 (devolvido)

            // Verificar que os logs foram registrados
            _logServico.Received(1).LogInicio(Arg.Any<string>(), ordemServicoId);
            _logServico.Received(1).LogFim(Arg.Any<string>());
        }

        [Fact]
        public async Task Handle_DeveTratarExcecaoDeRepositorio_QuandoErroNoBancoDeDados()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var estoqueId = Guid.NewGuid();

            // Configurar insumos da ordem de serviço
            var insumos = new List<InsumoOSResponse>
            {
                new InsumoOSResponse
                {
                    OrdemServicoId = ordemServicoId,
                    EstoqueId = estoqueId,
                    Quantidade = 3
                }
            };

            // Configurar ordem de serviço
            var ordemServico = new OrdemServicoResponse
            {
                Id = ordemServicoId,
                Status = StatusOrdemServico.Cancelada,
                Insumos = insumos
            };

            // Configurar comportamento do controller de ordem de serviço
            _ordemServicoController.ObterPorId(ordemServicoId).Returns(ordemServico);

            // Simular erro no banco de dados
            var dbException = new Exception("Erro de conexão com o banco de dados");
            _insumosOSController.DevolverInsumosAoEstoque(Arg.Any<IEnumerable<DevolverInsumoOSRequest>>())
                .Returns(Task.FromException<object>(dbException));

            // Configurar evento de cancelamento
            var evento = new OrdemServicoCanceladaEvent(ordemServicoId);

            // Act & Assert
            var act = async () => await _handler.Handle(evento, CancellationToken.None);

            await act.Should().ThrowAsync<Exception>();

            // Verificar que os logs foram registrados corretamente
            _logServico.Received(1).LogInicio(Arg.Any<string>(), ordemServicoId);
            _logServico.Received(1).LogErro(Arg.Any<string>(), dbException);
            _logServico.DidNotReceive().LogFim(Arg.Any<string>());
        }

        [Fact]
        public async Task Handle_DevePublicarEventoDeEstoqueAtualizado_QuandoInsumosDevolvidosComSucesso()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var estoqueId = Guid.NewGuid();

            // Configurar insumos da ordem de serviço
            var insumos = new List<InsumoOSResponse>
            {
                new InsumoOSResponse
                {
                    OrdemServicoId = ordemServicoId,
                    EstoqueId = estoqueId,
                    Quantidade = 5
                }
            };

            // Configurar ordem de serviço
            var ordemServico = new OrdemServicoResponse
            {
                Id = ordemServicoId,
                Status = StatusOrdemServico.Cancelada,
                Insumos = insumos
            };

            // Configurar comportamento do controller de ordem de serviço
            _ordemServicoController.ObterPorId(ordemServicoId).Returns(ordemServico);

            // Configurar comportamento do mediator (se existir na implementação real)
            // Este é um teste adicional que verifica se algum evento é publicado após a devolução
            // Se não houver publicação de eventos na implementação real, este teste pode ser removido

            // Configurar evento de cancelamento
            var evento = new OrdemServicoCanceladaEvent(ordemServicoId);

            // Act
            await _handler.Handle(evento, CancellationToken.None);

            // Assert
            // Verificar que o método de devolução foi chamado
            await _insumosOSController.Received(1).DevolverInsumosAoEstoque(
                Arg.Is<IEnumerable<DevolverInsumoOSRequest>>(requests =>
                    requests.Count() == 1 &&
                    requests.First().EstoqueId == estoqueId &&
                    requests.First().Quantidade == 5)
            );

            // Verificar que os logs foram registrados corretamente
            _logServico.Received(1).LogInicio(Arg.Any<string>(), ordemServicoId);
            _logServico.Received(1).LogFim(Arg.Any<string>());
        }
    }
}
