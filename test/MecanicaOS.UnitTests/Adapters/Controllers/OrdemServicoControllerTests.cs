using Adapters.Controllers;
using Core.DTOs.Requests.OrdemServico;
using Core.Entidades;
using Core.Enumeradores;
using Core.Interfaces.UseCases;
using Core.Interfaces.root;

namespace MecanicaOS.UnitTests.Adapters.Controllers
{
    /// <summary>
    /// Testes para OrdemServicoController (Adapter)
    /// Cobertura: 60.5%
    /// 
    /// IMPORTÂNCIA: Controller central do sistema - gerencia todo o fluxo de ordens de serviço.
    /// Orquestra validações cross-domain (Cliente, Serviço) antes de chamar UseCases.
    /// Crítico para integridade do negócio.
    /// </summary>
    public class OrdemServicoControllerTests
    {
        [Fact]
        public void Construtor_DeveCriarInstancia()
        {
            // Arrange
            var compositionRoot = Substitute.For<ICompositionRoot>();
            var ordemServicoUseCases = Substitute.For<IOrdemServicoUseCases>();
            var clienteUseCases = Substitute.For<IClienteUseCases>();
            var servicoUseCases = Substitute.For<IServicoUseCases>();

            compositionRoot.CriarOrdemServicoUseCases().Returns(ordemServicoUseCases);
            compositionRoot.CriarClienteUseCases().Returns(clienteUseCases);
            compositionRoot.CriarServicoUseCases().Returns(servicoUseCases);

            // Act
            var controller = new OrdemServicoController(compositionRoot);

            // Assert
            controller.Should().NotBeNull();
        }

        [Fact]
        public async Task ObterTodos_DeveChamarUseCases()
        {
            // Arrange
            var compositionRoot = Substitute.For<ICompositionRoot>();
            var ordemServicoUseCases = Substitute.For<IOrdemServicoUseCases>();
            var clienteUseCases = Substitute.For<IClienteUseCases>();
            var servicoUseCases = Substitute.For<IServicoUseCases>();
            var ordens = new List<OrdemServico> 
            { 
                new OrdemServico 
                { 
                    Id = Guid.NewGuid(), 
                    ClienteId = Guid.NewGuid(), 
                    ServicoId = Guid.NewGuid(), 
                    Status = StatusOrdemServico.Recebida 
                } 
            };

            compositionRoot.CriarOrdemServicoUseCases().Returns(ordemServicoUseCases);
            compositionRoot.CriarClienteUseCases().Returns(clienteUseCases);
            compositionRoot.CriarServicoUseCases().Returns(servicoUseCases);
            ordemServicoUseCases.ObterTodosUseCaseAsync().Returns(Task.FromResult<IEnumerable<OrdemServico>>(ordens));

            var controller = new OrdemServicoController(compositionRoot);

            // Act
            var resultado = await controller.ObterTodos();

            // Assert
            await ordemServicoUseCases.Received(1).ObterTodosUseCaseAsync();
        }

        [Fact]
        public async Task ObterPorId_DeveChamarUseCases()
        {
            // Arrange
            var compositionRoot = Substitute.For<ICompositionRoot>();
            var ordemServicoUseCases = Substitute.For<IOrdemServicoUseCases>();
            var clienteUseCases = Substitute.For<IClienteUseCases>();
            var servicoUseCases = Substitute.For<IServicoUseCases>();
            var id = Guid.NewGuid();
            var ordem = new OrdemServico 
            { 
                Id = id, 
                ClienteId = Guid.NewGuid(), 
                ServicoId = Guid.NewGuid(), 
                Status = StatusOrdemServico.Recebida 
            };

            compositionRoot.CriarOrdemServicoUseCases().Returns(ordemServicoUseCases);
            compositionRoot.CriarClienteUseCases().Returns(clienteUseCases);
            compositionRoot.CriarServicoUseCases().Returns(servicoUseCases);
            ordemServicoUseCases.ObterPorIdUseCaseAsync(id).Returns(Task.FromResult<OrdemServico?>(ordem));

            var controller = new OrdemServicoController(compositionRoot);

            // Act
            var resultado = await controller.ObterPorId(id);

            // Assert
            await ordemServicoUseCases.Received(1).ObterPorIdUseCaseAsync(id);
        }

        [Fact]
        public async Task ObterPorStatus_DeveChamarUseCases()
        {
            // Arrange
            var compositionRoot = Substitute.For<ICompositionRoot>();
            var ordemServicoUseCases = Substitute.For<IOrdemServicoUseCases>();
            var clienteUseCases = Substitute.For<IClienteUseCases>();
            var servicoUseCases = Substitute.For<IServicoUseCases>();
            var status = StatusOrdemServico.EmExecucao;
            var ordens = new List<OrdemServico> 
            { 
                new OrdemServico 
                { 
                    Id = Guid.NewGuid(), 
                    ClienteId = Guid.NewGuid(), 
                    ServicoId = Guid.NewGuid(), 
                    Status = status 
                } 
            };

            compositionRoot.CriarOrdemServicoUseCases().Returns(ordemServicoUseCases);
            compositionRoot.CriarClienteUseCases().Returns(clienteUseCases);
            compositionRoot.CriarServicoUseCases().Returns(servicoUseCases);
            ordemServicoUseCases.ObterPorStatusUseCaseAsync(status).Returns(Task.FromResult<IEnumerable<OrdemServico>>(ordens));

            var controller = new OrdemServicoController(compositionRoot);

            // Act
            var resultado = await controller.ObterPorStatus(status);

            // Assert
            await ordemServicoUseCases.Received(1).ObterPorStatusUseCaseAsync(status);
        }

        [Fact]
        public async Task AceitarOrcamento_DeveChamarUseCases()
        {
            // Arrange
            var compositionRoot = Substitute.For<ICompositionRoot>();
            var ordemServicoUseCases = Substitute.For<IOrdemServicoUseCases>();
            var clienteUseCases = Substitute.For<IClienteUseCases>();
            var servicoUseCases = Substitute.For<IServicoUseCases>();
            var id = Guid.NewGuid();

            compositionRoot.CriarOrdemServicoUseCases().Returns(ordemServicoUseCases);
            compositionRoot.CriarClienteUseCases().Returns(clienteUseCases);
            compositionRoot.CriarServicoUseCases().Returns(servicoUseCases);

            var controller = new OrdemServicoController(compositionRoot);

            // Act
            await controller.AceitarOrcamento(id);

            // Assert
            await ordemServicoUseCases.Received(1).AceitarOrcamentoUseCaseAsync(id);
        }

        [Fact]
        public async Task RecusarOrcamento_DeveChamarUseCases()
        {
            // Arrange
            var compositionRoot = Substitute.For<ICompositionRoot>();
            var ordemServicoUseCases = Substitute.For<IOrdemServicoUseCases>();
            var clienteUseCases = Substitute.For<IClienteUseCases>();
            var servicoUseCases = Substitute.For<IServicoUseCases>();
            var id = Guid.NewGuid();

            compositionRoot.CriarOrdemServicoUseCases().Returns(ordemServicoUseCases);
            compositionRoot.CriarClienteUseCases().Returns(clienteUseCases);
            compositionRoot.CriarServicoUseCases().Returns(servicoUseCases);

            var controller = new OrdemServicoController(compositionRoot);

            // Act
            await controller.RecusarOrcamento(id);

            // Assert
            await ordemServicoUseCases.Received(1).RecusarOrcamentoUseCaseAsync(id);
        }

        [Fact]
        public void MapearParaCadastrarOrdemServicoUseCaseDto_DeveMapearCorretamente()
        {
            // Arrange
            var compositionRoot = Substitute.For<ICompositionRoot>();
            var ordemServicoUseCases = Substitute.For<IOrdemServicoUseCases>();
            var clienteUseCases = Substitute.For<IClienteUseCases>();
            var servicoUseCases = Substitute.For<IServicoUseCases>();
            compositionRoot.CriarOrdemServicoUseCases().Returns(ordemServicoUseCases);
            compositionRoot.CriarClienteUseCases().Returns(clienteUseCases);
            compositionRoot.CriarServicoUseCases().Returns(servicoUseCases);

            var controller = new OrdemServicoController(compositionRoot);
            var request = new CadastrarOrdemServicoRequest
            {
                ClienteId = Guid.NewGuid(),
                ServicoId = Guid.NewGuid(),
                VeiculoId = Guid.NewGuid(),
                Descricao = "Ordem de serviço teste"
            };

            // Act
            var resultado = controller.MapearParaCadastrarOrdemServicoUseCaseDto(request);

            // Assert
            resultado.Should().NotBeNull();
            resultado.VeiculoId.Should().Be(request.VeiculoId);
            resultado.Descricao.Should().Be("Ordem de serviço teste");
        }

        [Fact]
        public void MapearParaAtualizarOrdemServicoUseCaseDto_DeveMapearCorretamente()
        {
            // Arrange
            var compositionRoot = Substitute.For<ICompositionRoot>();
            var ordemServicoUseCases = Substitute.For<IOrdemServicoUseCases>();
            var clienteUseCases = Substitute.For<IClienteUseCases>();
            var servicoUseCases = Substitute.For<IServicoUseCases>();
            compositionRoot.CriarOrdemServicoUseCases().Returns(ordemServicoUseCases);
            compositionRoot.CriarClienteUseCases().Returns(clienteUseCases);
            compositionRoot.CriarServicoUseCases().Returns(servicoUseCases);

            var controller = new OrdemServicoController(compositionRoot);
            var request = new AtualizarOrdemServicoRequest
            {
                Descricao = "Descrição atualizada",
                Status = StatusOrdemServico.EmExecucao
            };

            // Act
            var resultado = controller.MapearParaAtualizarOrdemServicoUseCaseDto(request);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Descricao.Should().Be("Descrição atualizada");
            resultado.Status.Should().Be(StatusOrdemServico.EmExecucao);
        }
    }
}
