using Adapters.Controllers;
using Core.DTOs.Requests.OrdemServico;
using Core.DTOs.Responses.OrdemServico;
using Core.DTOs.UseCases.OrdemServico;
using Core.Entidades;
using Core.Enumeradores;
using Core.Interfaces.Controllers;
using Core.Interfaces.Presenters;
using Core.Interfaces.root;
using Core.Interfaces.UseCases;
using NSubstitute;
using FluentAssertions;
using Xunit;

namespace MecanicaOS.UnitTests.Adapters.Controllers
{
    public class OrdemServicoControllerTests
    {
        private readonly IOrdemServicoUseCases _ordemServicoUseCases;
        private readonly IOrdemServicoPresenter _ordemServicoPresenter;
        private readonly OrdemServicoController _ordemServicoController;
        private readonly ICompositionRoot _compositionRoot;

        public OrdemServicoControllerTests()
        {
            _ordemServicoUseCases = Substitute.For<IOrdemServicoUseCases>();
            _ordemServicoPresenter = Substitute.For<IOrdemServicoPresenter>();
            _compositionRoot = Substitute.For<ICompositionRoot>();
            
            _compositionRoot.CriarOrdemServicoUseCases().Returns(_ordemServicoUseCases);
            _ordemServicoController = new OrdemServicoController(_compositionRoot);
            
            // Usar reflexão para injetar o presenter mockado
            var presenterField = typeof(OrdemServicoController).GetField("_ordemServicoPresenter", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            presenterField?.SetValue(_ordemServicoController, _ordemServicoPresenter);
        }

        [Fact]
        public void MapearParaCadastrarOrdemServicoUseCaseDto_ComRequestValido_DeveMapearCorretamente()
        {
            // Arrange
            var request = new CadastrarOrdemServicoRequest
            {
                ClienteId = Guid.NewGuid(),
                VeiculoId = Guid.NewGuid(),
                ServicoId = Guid.NewGuid(),
                Descricao = "Descrição da Ordem de Serviço Teste"
            };

            // Act
            var result = _ordemServicoController.MapearParaCadastrarOrdemServicoUseCaseDto(request);

            // Assert
            result.Should().NotBeNull();
            result.ClienteId.Should().Be(request.ClienteId);
            result.VeiculoId.Should().Be(request.VeiculoId);
            result.ServicoId.Should().Be(request.ServicoId);
            result.Descricao.Should().Be(request.Descricao);
        }

        [Fact]
        public void MapearParaCadastrarOrdemServicoUseCaseDto_ComRequestNulo_DeveRetornarNulo()
        {
            // Act
            var result = _ordemServicoController.MapearParaCadastrarOrdemServicoUseCaseDto(null);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void MapearParaAtualizarOrdemServicoUseCaseDto_ComRequestValido_DeveMapearCorretamente()
        {
            // Arrange
            var request = new AtualizarOrdemServicoRequest
            {
                ClienteId = Guid.NewGuid(),
                VeiculoId = Guid.NewGuid(),
                ServicoId = Guid.NewGuid(),
                Descricao = "Descrição da Ordem de Serviço Atualizada",
                Status = StatusOrdemServico.EmExecucao
            };

            // Act
            var result = _ordemServicoController.MapearParaAtualizarOrdemServicoUseCaseDto(request);

            // Assert
            result.Should().NotBeNull();
            result.ClienteId.Should().Be(request.ClienteId);
            result.VeiculoId.Should().Be(request.VeiculoId);
            result.ServicoId.Should().Be(request.ServicoId);
            result.Descricao.Should().Be(request.Descricao);
            result.Status.Should().Be(request.Status);
        }

        [Fact]
        public void MapearParaAtualizarOrdemServicoUseCaseDto_ComRequestNulo_DeveRetornarNulo()
        {
            // Act
            var result = _ordemServicoController.MapearParaAtualizarOrdemServicoUseCaseDto(null);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Cadastrar_DeveUsarMapeamentoEChamarUseCase()
        {
            // Arrange
            var request = new CadastrarOrdemServicoRequest
            {
                ClienteId = Guid.NewGuid(),
                VeiculoId = Guid.NewGuid(),
                ServicoId = Guid.NewGuid(),
                Descricao = "Descrição da Ordem de Serviço Teste"
            };

            var ordemServico = new OrdemServico();
            _ordemServicoUseCases.CadastrarUseCaseAsync(Arg.Any<CadastrarOrdemServicoUseCaseDto>())
                .Returns(ordemServico);

            // Act
            await _ordemServicoController.Cadastrar(request);

            // Assert
            await _ordemServicoUseCases.Received(1).CadastrarUseCaseAsync(Arg.Is<CadastrarOrdemServicoUseCaseDto>(
                dto => dto.ClienteId == request.ClienteId &&
                      dto.VeiculoId == request.VeiculoId &&
                      dto.ServicoId == request.ServicoId &&
                      dto.Descricao == request.Descricao));

            _ordemServicoPresenter.Received(1).ParaResponse(ordemServico);
        }

        [Fact]
        public async Task Atualizar_DeveUsarMapeamentoEChamarUseCase()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new AtualizarOrdemServicoRequest
            {
                ClienteId = Guid.NewGuid(),
                VeiculoId = Guid.NewGuid(),
                ServicoId = Guid.NewGuid(),
                Descricao = "Descrição da Ordem de Serviço Atualizada",
                Status = StatusOrdemServico.EmExecucao
            };

            var ordemServico = new OrdemServico();
            _ordemServicoUseCases.AtualizarUseCaseAsync(id, Arg.Any<AtualizarOrdemServicoUseCaseDto>())
                .Returns(ordemServico);

            // Act
            await _ordemServicoController.Atualizar(id, request);

            // Assert
            await _ordemServicoUseCases.Received(1).AtualizarUseCaseAsync(
                Arg.Is<Guid>(g => g == id),
                Arg.Is<AtualizarOrdemServicoUseCaseDto>(
                    dto => dto.ClienteId == request.ClienteId &&
                          dto.VeiculoId == request.VeiculoId &&
                          dto.ServicoId == request.ServicoId &&
                          dto.Descricao == request.Descricao &&
                          dto.Status == request.Status));

            _ordemServicoPresenter.Received(1).ParaResponse(ordemServico);
        }
    }
}
