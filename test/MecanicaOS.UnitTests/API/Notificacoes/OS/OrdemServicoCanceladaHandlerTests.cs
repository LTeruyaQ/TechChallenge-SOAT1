using API.Notificacoes.OS;
using Core.DTOs.Requests.OrdemServico.InsumoOS;
using Core.DTOs.Responses.OrdemServico;
using Core.DTOs.Responses.OrdemServico.InsumoOrdemServico;
using Core.Interfaces.Controllers;
using Core.Interfaces.root;
using Core.Interfaces.Servicos;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOS.UnitTests.API.Notificacoes.OS
{
    public class OrdemServicoCanceladaHandlerTests
    {
        private readonly OrdemServicoCanceladaHandlerFixture _fixture;

        public OrdemServicoCanceladaHandlerTests()
        {
            _fixture = new OrdemServicoCanceladaHandlerFixture();
        }

        [Fact]
        public async Task Handle_ComOrdemServicoSemInsumos_NaoDeveDevolverInsumos()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var evento = _fixture.CriarEvento(ordemServicoId);
            
            // Configurar ordem de serviço sem insumos
            var ordemServico = _fixture.CriarOrdemServicoSemInsumos(ordemServicoId);
            
            _fixture.OrdemServicoController.ObterPorId(ordemServicoId).Returns(ordemServico);

            // Act
            await _fixture.Handler.Handle(evento, CancellationToken.None);

            // Assert
            await _fixture.OrdemServicoController.Received(1).ObterPorId(ordemServicoId);
            _fixture.LogServico.Received(1).LogInicio(Arg.Any<string>(), ordemServicoId);
            _fixture.LogServico.Received(1).LogFim(Arg.Any<string>(), Arg.Any<object>());
            
            // Verificar que o método de devolução de insumos não foi chamado
            await _fixture.InsumoOSController.DidNotReceive().DevolverInsumosAoEstoque(Arg.Any<IEnumerable<DevolverInsumoOSRequest>>());
        }

        [Fact]
        public async Task Handle_ComOrdemServicoComInsumos_DeveDevolverInsumos()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var evento = _fixture.CriarEvento(ordemServicoId);
            
            // Configurar ordem de serviço com insumos
            var ordemServico = _fixture.CriarOrdemServicoComInsumos(ordemServicoId, 2);
            var insumos = ordemServico.Insumos.ToList();
            
            _fixture.OrdemServicoController.ObterPorId(ordemServicoId).Returns(ordemServico);

            // Act
            await _fixture.Handler.Handle(evento, CancellationToken.None);

            // Assert
            await _fixture.OrdemServicoController.Received(1).ObterPorId(ordemServicoId);
            _fixture.LogServico.Received(1).LogInicio(Arg.Any<string>(), ordemServicoId);
            _fixture.LogServico.Received(1).LogFim(Arg.Any<string>(), Arg.Any<object>());
            
            // Verificar que o método de devolução de insumos foi chamado com os parâmetros corretos
            await _fixture.InsumoOSController.Received(1).DevolverInsumosAoEstoque(
                Arg.Is<IEnumerable<DevolverInsumoOSRequest>>(x => 
                    x.Count() == 2 &&
                    x.All(r => insumos.Any(i => i.EstoqueId == r.EstoqueId && i.Quantidade == r.Quantidade))
                )
            );
        }

        [Fact]
        public async Task Handle_QuandoOrdemServicoControllerLancaExcecao_DeveLogarErroEPropagar()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var evento = _fixture.CriarEvento(ordemServicoId);
            var exception = new Exception("Erro ao obter ordem de serviço");
            
            _fixture.OrdemServicoController.ObterPorId(ordemServicoId).Throws(exception);

            // Act & Assert
            var act = async () => await _fixture.Handler.Handle(evento, CancellationToken.None);
            
            await act.Should().ThrowAsync<Exception>().WithMessage("Erro ao obter ordem de serviço");
            
            _fixture.LogServico.Received(1).LogInicio(Arg.Any<string>(), ordemServicoId);
            _fixture.LogServico.Received(1).LogErro(Arg.Any<string>(), exception);
            _fixture.LogServico.DidNotReceive().LogFim(Arg.Any<string>());
        }

        [Fact]
        public async Task Handle_QuandoInsumoOSControllerLancaExcecao_DeveLogarErroEPropagar()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var evento = _fixture.CriarEvento(ordemServicoId);
            var exception = new Exception("Erro ao devolver insumos");
            
            // Configurar ordem de serviço com insumos
            var ordemServico = _fixture.CriarOrdemServicoComInsumos(ordemServicoId, 1);
            
            _fixture.OrdemServicoController.ObterPorId(ordemServicoId).Returns(ordemServico);
            _fixture.InsumoOSController.DevolverInsumosAoEstoque(Arg.Any<IEnumerable<DevolverInsumoOSRequest>>()).Throws(exception);

            // Act & Assert
            var act = async () => await _fixture.Handler.Handle(evento, CancellationToken.None);
            
            await act.Should().ThrowAsync<Exception>().WithMessage("Erro ao devolver insumos");
            
            _fixture.LogServico.Received(1).LogInicio(Arg.Any<string>(), ordemServicoId);
            _fixture.LogServico.Received(1).LogErro(Arg.Any<string>(), exception);
            _fixture.LogServico.DidNotReceive().LogFim(Arg.Any<string>());
        }

        [Fact]
        public async Task Handle_ComInsumosNulos_NaoDeveDevolverInsumos()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var evento = _fixture.CriarEvento(ordemServicoId);
            
            // Configurar ordem de serviço com insumos nulos
            var ordemServico = new OrdemServicoResponse
            {
                Id = ordemServicoId,
                Insumos = null
            };
            
            _fixture.OrdemServicoController.ObterPorId(ordemServicoId).Returns(ordemServico);

            // Act
            await _fixture.Handler.Handle(evento, CancellationToken.None);

            // Assert
            await _fixture.OrdemServicoController.Received(1).ObterPorId(ordemServicoId);
            _fixture.LogServico.Received(1).LogInicio(Arg.Any<string>(), ordemServicoId);
            _fixture.LogServico.Received(1).LogFim(Arg.Any<string>(), Arg.Any<object>());
            
            // Verificar que o método de devolução de insumos não foi chamado
            await _fixture.InsumoOSController.DidNotReceive().DevolverInsumosAoEstoque(Arg.Any<IEnumerable<DevolverInsumoOSRequest>>());
        }

        [Fact]
        public async Task Handle_DeveMapearCorretamenteOsInsumos()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var estoqueId1 = Guid.NewGuid();
            var estoqueId2 = Guid.NewGuid();
            var evento = _fixture.CriarEvento(ordemServicoId);
            
            // Configurar ordem de serviço com insumos específicos
            var insumosInfo = new List<(Guid EstoqueId, int Quantidade)>
            {
                (estoqueId1, 5),
                (estoqueId2, 10)
            };
            
            var ordemServico = _fixture.CriarOrdemServicoComInsumosEspecificos(ordemServicoId, insumosInfo);
            
            _fixture.OrdemServicoController.ObterPorId(ordemServicoId).Returns(ordemServico);

            // Act
            await _fixture.Handler.Handle(evento, CancellationToken.None);

            // Assert
            await _fixture.InsumoOSController.Received(1).DevolverInsumosAoEstoque(
                Arg.Is<IEnumerable<DevolverInsumoOSRequest>>(reqs => reqs.Count() == 2 && 
                    reqs.Any(r => r.EstoqueId == estoqueId1 && r.Quantidade == 5) && 
                    reqs.Any(r => r.EstoqueId == estoqueId2 && r.Quantidade == 10))
            );
        }
    }
}
