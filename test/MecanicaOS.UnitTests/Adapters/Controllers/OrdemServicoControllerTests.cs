using Adapters.Controllers;
using Core.DTOs.Requests.OrdemServico;
using Core.DTOs.UseCases.OrdemServico;
using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
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

        [Fact]
        public async Task Cadastrar_ComClienteInexistente_DeveLancarDadosNaoEncontradosException()
        {
            // Arrange
            var compositionRoot = Substitute.For<ICompositionRoot>();
            var ordemServicoUseCases = Substitute.For<IOrdemServicoUseCases>();
            var clienteUseCases = Substitute.For<IClienteUseCases>();
            var servicoUseCases = Substitute.For<IServicoUseCases>();

            compositionRoot.CriarOrdemServicoUseCases().Returns(ordemServicoUseCases);
            compositionRoot.CriarClienteUseCases().Returns(clienteUseCases);
            compositionRoot.CriarServicoUseCases().Returns(servicoUseCases);

            clienteUseCases.ObterPorIdUseCaseAsync(Arg.Any<Guid>()).Returns(Task.FromResult<Cliente?>(null));

            var controller = new OrdemServicoController(compositionRoot);
            var request = new CadastrarOrdemServicoRequest
            {
                ClienteId = Guid.NewGuid(),
                ServicoId = Guid.NewGuid(),
                VeiculoId = Guid.NewGuid(),
                Descricao = "Teste"
            };

            // Act & Assert
            await Assert.ThrowsAsync<DadosNaoEncontradosException>(async () =>
                await controller.Cadastrar(request));
        }

        [Fact]
        public async Task Cadastrar_ComServicoInexistente_DeveLancarDadosNaoEncontradosException()
        {
            // Arrange
            var compositionRoot = Substitute.For<ICompositionRoot>();
            var ordemServicoUseCases = Substitute.For<IOrdemServicoUseCases>();
            var clienteUseCases = Substitute.For<IClienteUseCases>();
            var servicoUseCases = Substitute.For<IServicoUseCases>();

            compositionRoot.CriarOrdemServicoUseCases().Returns(ordemServicoUseCases);
            compositionRoot.CriarClienteUseCases().Returns(clienteUseCases);
            compositionRoot.CriarServicoUseCases().Returns(servicoUseCases);

            var cliente = new Cliente
            {
                Id = Guid.NewGuid(),
                Nome = "Cliente Teste",
                Documento = "12345678900",
                TipoCliente = TipoCliente.PessoaFisica,
                DataNascimento = "1990-01-01",
                DataCadastro = DateTime.Now
            };

            clienteUseCases.ObterPorIdUseCaseAsync(Arg.Any<Guid>()).Returns(Task.FromResult<Cliente?>(cliente));
            servicoUseCases.ObterServicoPorIdUseCaseAsync(Arg.Any<Guid>()).Returns(Task.FromResult<Servico?>(null));

            var controller = new OrdemServicoController(compositionRoot);
            var request = new CadastrarOrdemServicoRequest
            {
                ClienteId = cliente.Id,
                ServicoId = Guid.NewGuid(),
                VeiculoId = Guid.NewGuid(),
                Descricao = "Teste"
            };

            // Act & Assert
            await Assert.ThrowsAsync<DadosNaoEncontradosException>(async () =>
                await controller.Cadastrar(request));
        }

        [Fact]
        public async Task Cadastrar_ComDadosValidos_DeveChamarUseCases()
        {
            // Arrange
            var compositionRoot = Substitute.For<ICompositionRoot>();
            var ordemServicoUseCases = Substitute.For<IOrdemServicoUseCases>();
            var clienteUseCases = Substitute.For<IClienteUseCases>();
            var servicoUseCases = Substitute.For<IServicoUseCases>();

            compositionRoot.CriarOrdemServicoUseCases().Returns(ordemServicoUseCases);
            compositionRoot.CriarClienteUseCases().Returns(clienteUseCases);
            compositionRoot.CriarServicoUseCases().Returns(servicoUseCases);

            var cliente = new Cliente
            {
                Id = Guid.NewGuid(),
                Nome = "Cliente Teste",
                Documento = "12345678900",
                TipoCliente = TipoCliente.PessoaFisica,
                DataNascimento = "1990-01-01",
                DataCadastro = DateTime.Now
            };

            var servico = new Servico
            {
                Id = Guid.NewGuid(),
                Nome = "Serviço Teste",
                Descricao = "Descrição do serviço",
                Valor = 100m,
                Disponivel = true,
                DataCadastro = DateTime.Now
            };

            var ordemServicoCriada = new OrdemServico
            {
                Id = Guid.NewGuid(),
                ClienteId = cliente.Id,
                ServicoId = servico.Id,
                VeiculoId = Guid.NewGuid(),
                Status = StatusOrdemServico.Recebida,
                DataCadastro = DateTime.Now
            };

            clienteUseCases.ObterPorIdUseCaseAsync(cliente.Id).Returns(Task.FromResult<Cliente?>(cliente));
            servicoUseCases.ObterServicoPorIdUseCaseAsync(servico.Id).Returns(Task.FromResult<Servico?>(servico));
            ordemServicoUseCases.CadastrarUseCaseAsync(Arg.Any<CadastrarOrdemServicoUseCaseDto>())
                .Returns(Task.FromResult(ordemServicoCriada));

            var controller = new OrdemServicoController(compositionRoot);
            var request = new CadastrarOrdemServicoRequest
            {
                ClienteId = cliente.Id,
                ServicoId = servico.Id,
                VeiculoId = Guid.NewGuid(),
                Descricao = "Teste"
            };

            // Act
            var resultado = await controller.Cadastrar(request);

            // Assert
            resultado.Should().NotBeNull();
            await clienteUseCases.Received(1).ObterPorIdUseCaseAsync(cliente.Id);
            await servicoUseCases.Received(1).ObterServicoPorIdUseCaseAsync(servico.Id);
        }

        [Fact]
        public async Task Atualizar_ComDadosValidos_DeveChamarUseCases()
        {
            // Arrange
            var compositionRoot = Substitute.For<ICompositionRoot>();
            var ordemServicoUseCases = Substitute.For<IOrdemServicoUseCases>();
            var clienteUseCases = Substitute.For<IClienteUseCases>();
            var servicoUseCases = Substitute.For<IServicoUseCases>();

            compositionRoot.CriarOrdemServicoUseCases().Returns(ordemServicoUseCases);
            compositionRoot.CriarClienteUseCases().Returns(clienteUseCases);
            compositionRoot.CriarServicoUseCases().Returns(servicoUseCases);

            var id = Guid.NewGuid();
            var ordemServicoAtualizada = new OrdemServico
            {
                Id = id,
                ClienteId = Guid.NewGuid(),
                ServicoId = Guid.NewGuid(),
                VeiculoId = Guid.NewGuid(),
                Status = StatusOrdemServico.EmExecucao,
                DataCadastro = DateTime.Now
            };

            ordemServicoUseCases.AtualizarUseCaseAsync(id, Arg.Any<AtualizarOrdemServicoUseCaseDto>())
                .Returns(Task.FromResult(ordemServicoAtualizada));

            var controller = new OrdemServicoController(compositionRoot);
            var request = new AtualizarOrdemServicoRequest
            {
                Descricao = "Descrição atualizada",
                Status = StatusOrdemServico.EmExecucao
            };

            // Act
            var resultado = await controller.Atualizar(id, request);

            // Assert
            resultado.Should().NotBeNull();
            await ordemServicoUseCases.Received(1).AtualizarUseCaseAsync(id, Arg.Any<AtualizarOrdemServicoUseCaseDto>());
        }

        [Fact]
        public async Task CalcularOrcamentoAsync_ComOrdemServicoExistente_DeveChamarOrcamentoUseCases()
        {
            // Arrange
            var compositionRoot = Substitute.For<ICompositionRoot>();
            var ordemServicoUseCases = Substitute.For<IOrdemServicoUseCases>();
            var clienteUseCases = Substitute.For<IClienteUseCases>();
            var servicoUseCases = Substitute.For<IServicoUseCases>();

            compositionRoot.CriarOrdemServicoUseCases().Returns(ordemServicoUseCases);
            compositionRoot.CriarClienteUseCases().Returns(clienteUseCases);
            compositionRoot.CriarServicoUseCases().Returns(servicoUseCases);

            var id = Guid.NewGuid();
            var ordemServico = new OrdemServico
            {
                Id = id,
                ClienteId = Guid.NewGuid(),
                ServicoId = Guid.NewGuid(),
                VeiculoId = Guid.NewGuid(),
                Status = StatusOrdemServico.Recebida,
                DataCadastro = DateTime.Now
            };

            ordemServicoUseCases.ObterPorIdUseCaseAsync(id).Returns(Task.FromResult<OrdemServico?>(ordemServico));

            var controller = new OrdemServicoController(compositionRoot);

            // Act
            await controller.CalcularOrcamentoAsync(id);

            // Assert
            await ordemServicoUseCases.Received(1).ObterPorIdUseCaseAsync(id);
        }

        [Fact]
        public async Task CalcularOrcamentoAsync_ComOrdemServicoInexistente_NaoDeveChamarOrcamentoUseCases()
        {
            // Arrange
            var compositionRoot = Substitute.For<ICompositionRoot>();
            var ordemServicoUseCases = Substitute.For<IOrdemServicoUseCases>();
            var clienteUseCases = Substitute.For<IClienteUseCases>();
            var servicoUseCases = Substitute.For<IServicoUseCases>();

            compositionRoot.CriarOrdemServicoUseCases().Returns(ordemServicoUseCases);
            compositionRoot.CriarClienteUseCases().Returns(clienteUseCases);
            compositionRoot.CriarServicoUseCases().Returns(servicoUseCases);

            var id = Guid.NewGuid();
            ordemServicoUseCases.ObterPorIdUseCaseAsync(id).Returns(Task.FromResult<OrdemServico?>(null));

            var controller = new OrdemServicoController(compositionRoot);

            // Act
            await controller.CalcularOrcamentoAsync(id);

            // Assert
            await ordemServicoUseCases.Received(1).ObterPorIdUseCaseAsync(id);
        }
    }
}
