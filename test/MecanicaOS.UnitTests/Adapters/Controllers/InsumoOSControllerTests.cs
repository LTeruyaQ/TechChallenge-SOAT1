using Adapters.Controllers;
using Core.DTOs.Requests.OrdemServico.InsumoOS;
using Core.DTOs.Responses.OrdemServico.InsumoOrdemServico;
using Core.DTOs.UseCases.OrdemServico.InsumoOS;
using Core.DTOs.UseCases.Estoque;
using Core.Entidades;
using Core.Interfaces.Presenters;
using Core.Interfaces.root;
using Core.Interfaces.UseCases;

namespace MecanicaOS.UnitTests.Adapters.Controllers
{
    public class InsumoOSControllerTests
    {
        private readonly IInsumoOSUseCases _insumoOSUseCases;
        private readonly IInsumoPresenter _insumoPresenter;
        private readonly InsumoOSController _insumoOSController;
        private readonly ICompositionRoot _compositionRoot;

        public InsumoOSControllerTests()
        {
            _insumoOSUseCases = Substitute.For<IInsumoOSUseCases>();
            _insumoPresenter = Substitute.For<IInsumoPresenter>();
            _compositionRoot = Substitute.For<ICompositionRoot>();

            _compositionRoot.CriarInsumoOSUseCases().Returns(_insumoOSUseCases);
            
            // Configurar mocks padrão para OrdemServicoUseCases e EstoqueUseCases
            var ordemServicoUseCases = Substitute.For<IOrdemServicoUseCases>();
            var estoqueUseCases = Substitute.For<IEstoqueUseCases>();
            _compositionRoot.CriarOrdemServicoUseCases().Returns(ordemServicoUseCases);
            _compositionRoot.CriarEstoqueUseCases().Returns(estoqueUseCases);
            
            _insumoOSController = new InsumoOSController(_compositionRoot);

            // Usar reflexão para injetar o presenter mockado
            var presenterField = typeof(InsumoOSController).GetField("_insumoPresenter",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            presenterField?.SetValue(_insumoOSController, _insumoPresenter);
        }


        [Fact]
        public void MapearParaCadastrarInsumoOSUseCaseDto_ComRequestNulo_DeveRetornarNulo()
        {
            // Act
            var result = _insumoOSController.MapearParaCadastrarInsumoOSUseCaseDto(null);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CadastrarInsumos_DeveUsarMapeamentoEChamarUseCase()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var requests = new List<CadastrarInsumoOSRequest>
            {
                new CadastrarInsumoOSRequest
                {
                    EstoqueId = Guid.NewGuid(),
                    Quantidade = 5
                },
                new CadastrarInsumoOSRequest
                {
                    EstoqueId = Guid.NewGuid(),
                    Quantidade = 10
                }
            };

            // Crie uma lista simples de InsumoOS para o retorno do mock
            var insumosOS = new List<InsumoOS>
            {
                new InsumoOS { Id = Guid.NewGuid() },
                new InsumoOS { Id = Guid.NewGuid() }
            };

            // Obter os mocks já configurados no construtor
            var ordemServicoUseCases = _compositionRoot.CriarOrdemServicoUseCases();
            var estoqueUseCases = _compositionRoot.CriarEstoqueUseCases();
            
            // Configurar comportamentos específicos para este teste
            ordemServicoUseCases.ObterPorIdUseCaseAsync(ordemServicoId)
                .Returns(new OrdemServico { Id = ordemServicoId });

            // Configure os mocks para retornar estoques válidos
            foreach (var request in requests)
            {
                estoqueUseCases.ObterPorIdUseCaseAsync(request.EstoqueId)
                    .Returns(new Estoque
                    {
                        Id = request.EstoqueId,
                        QuantidadeDisponivel = 20,
                        Insumo = "Insumo Teste"
                    });
            }

            // Configure o mock para retornar a lista de insumos
            _insumoOSUseCases.CadastrarInsumosUseCaseAsync(
                Arg.Any<Guid>(),
                Arg.Any<List<CadastrarInsumoOSUseCaseDto>>())
                .Returns(insumosOS);

            // Crie uma lista simples de InsumoOSResponse para o retorno do presenter
            var responses = new List<InsumoOSResponse>
            {
                new InsumoOSResponse(),
                new InsumoOSResponse()
            };

            // Configure o presenter para retornar as respostas
            _insumoPresenter.ToResponse(Arg.Any<IEnumerable<InsumoOS>>()).Returns(responses);

            // Act
            var result = await _insumoOSController.CadastrarInsumos(ordemServicoId, requests);

            // Assert
            // Verifique se o método do usecase foi chamado
            await _insumoOSUseCases.Received(1).CadastrarInsumosUseCaseAsync(
                Arg.Is<Guid>(g => g == ordemServicoId),
                Arg.Any<List<CadastrarInsumoOSUseCaseDto>>());

            // Verifique se o presenter foi chamado
            _insumoPresenter.Received(1).ToResponse(Arg.Any<IEnumerable<InsumoOS>>());

            // Verifique se o resultado é o mesmo que o retornado pelo presenter
            result.Should().BeSameAs(responses);

            // Verifique se os métodos de orquestração foram chamados
            await ordemServicoUseCases.Received(1).ObterPorIdUseCaseAsync(ordemServicoId);
            await estoqueUseCases.Received(requests.Count).ObterPorIdUseCaseAsync(Arg.Any<Guid>());
            await estoqueUseCases.Received(requests.Count).AtualizarUseCaseAsync(
                Arg.Any<Guid>(),
                Arg.Any<AtualizarEstoqueUseCaseDto>());
        }

        [Fact]
        public async Task DevolverInsumosAoEstoque_DeveChamarUseCase()
        {
            // Arrange
            var insumosOS = new List<DevolverInsumoOSRequest>
            {
                new DevolverInsumoOSRequest { EstoqueId = Guid.NewGuid(), Quantidade = 1 },
                new DevolverInsumoOSRequest { EstoqueId = Guid.NewGuid(), Quantidade = 2 }
            };

            // Obter o mock já configurado no construtor
            var estoqueUseCases = _compositionRoot.CriarEstoqueUseCases();

            // Configure os mocks para retornar estoques válidos
            foreach (var insumo in insumosOS)
            {
                estoqueUseCases.ObterPorIdUseCaseAsync(insumo.EstoqueId)
                    .Returns(new Estoque
                    {
                        Id = insumo.EstoqueId,
                        QuantidadeDisponivel = 5,
                        Insumo = "Insumo Teste"
                    });
            }

            // Act
            await _insumoOSController.DevolverInsumosAoEstoque(insumosOS);

            // Assert
            // Verifique se o método do usecase foi chamado
            await _insumoOSUseCases.Received(1).DevolverInsumosAoEstoqueUseCaseAsync(
                Arg.Is<IEnumerable<DevolverInsumoOSRequest>>(i => i == insumosOS));

            // Verifique se os métodos de orquestração foram chamados
            await estoqueUseCases.Received(insumosOS.Count).ObterPorIdUseCaseAsync(Arg.Any<Guid>());
            await estoqueUseCases.Received(insumosOS.Count).AtualizarUseCaseAsync(
                Arg.Any<Guid>(),
                Arg.Any<AtualizarEstoqueUseCaseDto>());
        }
    }
}
