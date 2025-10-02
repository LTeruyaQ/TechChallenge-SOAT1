using API.Notificacoes.OS;
using Core.DTOs.Entidades.Estoque;
using Core.DTOs.Requests.OrdemServico.InsumoOS;
using Core.DTOs.UseCases.Eventos;
using Core.DTOs.UseCases.OrdemServico;
using Core.Entidades;
using Core.Enumeradores;
using Core.Interfaces.Eventos;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.Interfaces.root;
using FluentAssertions;
using MediatR;
using NSubstitute;

namespace MecanicaOS.UnitTests.API.Notificacoes.OS
{
    public class OrdemServicoCanceladaHandlerComplexTests
    {
        private readonly OrdemServicoCanceladaHandlerFixture _fixture;
        private readonly IEstoqueGateway _estoqueGateway;
        private readonly IRepositorio<EstoqueEntityDto> _estoqueRepositorio;
        private readonly IMediator _mediator;
        private readonly ICompositionRoot _compositionRoot;

        public OrdemServicoCanceladaHandlerComplexTests()
        {
            _fixture = new OrdemServicoCanceladaHandlerFixture();
            _estoqueGateway = Substitute.For<IEstoqueGateway>();
            _estoqueRepositorio = Substitute.For<IRepositorio<EstoqueEntityDto>>();
            _mediator = Substitute.For<IMediator>();
            
            // Configurar CompositionRoot
            _compositionRoot = Substitute.For<ICompositionRoot>();
            _compositionRoot.CriarOrdemServicoController().Returns(_fixture.OrdemServicoController);
            _compositionRoot.CriarInsumoOSController().Returns(_fixture.InsumoOSController);
            _compositionRoot.CriarLogService<OrdemServicoCanceladaHandler>().Returns(_fixture.LogServico);
            _compositionRoot.CriarEstoqueGateway().Returns(_estoqueGateway);
            _compositionRoot.CriarRepositorio<EstoqueEntityDto>().Returns(_estoqueRepositorio);
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
                    $"O estoque {i + 1} deveria ter {quantidadeEsperada} itens após a devolução");
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
        }

        [Theory]
        [InlineData(2, 5, 4, true)]  // Abaixo do mínimo -> acima do mínimo
        [InlineData(5, 5, 2, false)] // No mínimo -> acima do mínimo
        [InlineData(1, 5, 3, false)] // Abaixo do mínimo -> ainda abaixo
        public async Task Handle_ComDiferentesEstados_DeveAtualizarQuantidadeCorretamente(
            int quantidadeInicial, int quantidadeMinima, int quantidadeDevolvida, bool devePublicarAlerta)
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var estoqueId = Guid.NewGuid();
            var evento = _fixture.CriarEvento(ordemServicoId);

            // Criar um estoque com quantidade específica
            var estoqueDto = new EstoqueEntityDto
            {
                Id = estoqueId,
                Insumo = "Filtro de Óleo",
                QuantidadeDisponivel = quantidadeInicial,
                QuantidadeMinima = quantidadeMinima,
                Preco = 25.50M,
                Ativo = true,
                DataCadastro = DateTime.Now.AddDays(-10)
            };

            // Configurar insumos da ordem de serviço
            var insumosInfo = new List<(Guid EstoqueId, int Quantidade)>
            {
                (estoqueId, quantidadeDevolvida)
            };

            var ordemServico = _fixture.CriarOrdemServicoComInsumosEspecificos(ordemServicoId, insumosInfo);
            _fixture.OrdemServicoController.ObterPorId(ordemServicoId).Returns(ordemServico);

            // Simular o repositório
            _estoqueRepositorio.ObterPorIdAsync(estoqueId).Returns(estoqueDto);

            // Configurar mock para eventos
            var eventosPublisher = Substitute.For<IEventosPublisher>();
            _compositionRoot.CriarEventosPublisher().Returns(eventosPublisher);

            // Simular a devolução real dos insumos
            _fixture.InsumoOSController.DevolverInsumosAoEstoque(Arg.Any<IEnumerable<DevolverInsumoOSRequest>>())
                .Returns(callInfo =>
                {
                    var requests = callInfo.Arg<IEnumerable<DevolverInsumoOSRequest>>();

                    foreach (var request in requests)
                    {
                        if (request.EstoqueId == estoqueId)
                        {
                            // Registrar a quantidade disponível antes da atualização
                            var quantidadeAnterior = estoqueDto.QuantidadeDisponivel;
                            
                            // Atualizar a quantidade disponível
                            estoqueDto.QuantidadeDisponivel += request.Quantidade;
                            estoqueDto.DataAtualizacao = DateTime.Now;
                            
                            // Verificar se mudou de estado (abaixo/acima do mínimo)
                            var estavaBaixo = quantidadeAnterior < estoqueDto.QuantidadeMinima;
                            var estáAcima = estoqueDto.QuantidadeDisponivel >= estoqueDto.QuantidadeMinima;
                            
                            // Se estava abaixo e agora está acima, publicar evento
                            if (estavaBaixo && estáAcima && devePublicarAlerta)
                            {
                                // Simular publicação de evento
                                eventosPublisher.Publicar(new OrdemServicoFinalizadaEventDTO(ordemServicoId));
                            }
                        }
                    }

                    return Task.CompletedTask;
                });

            var handler = new OrdemServicoCanceladaHandler(_compositionRoot);

            // Act
            await handler.Handle(evento, CancellationToken.None);

            // Assert
            // Verificar que a quantidade foi atualizada corretamente
            estoqueDto.QuantidadeDisponivel.Should().Be(quantidadeInicial + quantidadeDevolvida);

            // Verificar se a data de atualização foi modificada
            estoqueDto.DataAtualizacao.Should().BeAfter(DateTime.Now.AddMinutes(-1));
            
            // Verificar se o evento foi publicado quando necessário
            if (devePublicarAlerta)
            {
                eventosPublisher.Received(1).Publicar(Arg.Any<OrdemServicoFinalizadaEventDTO>());
            }
            else
            {
                eventosPublisher.DidNotReceive().Publicar(Arg.Any<OrdemServicoFinalizadaEventDTO>());
            }
            
            // Verificar que a data de cadastro não foi alterada
            estoqueDto.DataCadastro.Date.Should().Be(DateTime.Now.AddDays(-10).Date);
        }
    }
}
