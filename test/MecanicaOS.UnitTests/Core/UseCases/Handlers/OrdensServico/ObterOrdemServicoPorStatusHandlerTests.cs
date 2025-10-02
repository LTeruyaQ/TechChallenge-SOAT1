using Core.DTOs.Entidades.OrdemServicos;
using Core.Entidades;
using Core.Enumeradores;
using Core.Especificacoes.Base.Interfaces;
using MecanicaOS.UnitTests.Fixtures.Handlers;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.OrdensServico
{
    public class ObterOrdemServicoPorStatusHandlerTests
    {
        private readonly OrdemServicoHandlerFixture _fixture;

        public ObterOrdemServicoPorStatusHandlerTests()
        {
            _fixture = new OrdemServicoHandlerFixture();
        }

        [Theory]
        [InlineData(StatusOrdemServico.Recebida)]
        [InlineData(StatusOrdemServico.EmDiagnostico)]
        [InlineData(StatusOrdemServico.EmExecucao)]
        [InlineData(StatusOrdemServico.Finalizada)]
        [InlineData(StatusOrdemServico.AguardandoAprovacao)]
        [InlineData(StatusOrdemServico.Cancelada)]
        public async Task Handle_ComDiferentesStatus_DeveRetornarApenasDaqueleStatus(StatusOrdemServico status)
        {
            // Arrange
            // Criar ordens com diferentes status
            var ordensServico = new List<OrdemServicoEntityDto>();
            var ordensServicoDoStatus = new List<OrdemServicoEntityDto>();
            
            // Criar 2 ordens para cada status
            foreach (StatusOrdemServico currentStatus in Enum.GetValues(typeof(StatusOrdemServico)))
            {
                for (int i = 0; i < 2; i++)
                {
                    var ordem = new OrdemServicoEntityDto
                    {
                        Id = Guid.NewGuid(),
                        ClienteId = Guid.NewGuid(),
                        VeiculoId = Guid.NewGuid(),
                        ServicoId = Guid.NewGuid(),
                        Descricao = $"Ordem {currentStatus} {i+1}",
                        Status = currentStatus,
                        DataCadastro = DateTime.Now.AddDays(-10),
                        DataAtualizacao = DateTime.Now.AddDays(-1),
                        Ativo = true
                    };
                    
                    ordensServico.Add(ordem);
                    
                    // Guardar ordens do status específico
                    if (currentStatus == status)
                    {
                        ordensServicoDoStatus.Add(ordem);
                    }
                }
            }

            // Capturar ordens retornadas
            List<OrdemServico> ordensRetornadas = null;
            
            // Configurar gateway
            _fixture.RepositorioOrdemServico.ListarProjetadoAsync<OrdemServico>(Arg.Any<IEspecificacao<OrdemServicoEntityDto>>())
                .Returns(callInfo => 
                {
                    // Simular gateway real
                    var filtradas = ordensServicoDoStatus;
                    
                    // Converter para entidades
                    ordensRetornadas = filtradas.Select(dto => new OrdemServico
                    {
                        Id = dto.Id,
                        ClienteId = dto.ClienteId,
                        VeiculoId = dto.VeiculoId,
                        ServicoId = dto.ServicoId,
                        Descricao = dto.Descricao,
                        Status = dto.Status,
                        DataCadastro = dto.DataCadastro,
                        DataAtualizacao = dto.DataAtualizacao,
                        Ativo = dto.Ativo
                    }).ToList();
                    
                    return ordensRetornadas;
                });

            var handler = _fixture.CriarObterOrdemServicoPorStatusHandler();

            // Act
            var resultado = await handler.Handle(status);

            // Assert
            await _fixture.RepositorioOrdemServico.Received(1).ListarProjetadoAsync<OrdemServico>(Arg.Any<IEspecificacao<OrdemServicoEntityDto>>());
            
            // Verificar resultado
            resultado.Should().NotBeNull();
            resultado.Count().Should().Be(2, $"Devem existir 2 ordens com status {status}");
            resultado.Should().OnlyContain(o => o.Status == status);
            
            // Verificar trânsito de dados
            resultado.Should().BeEquivalentTo(ordensRetornadas, 
                options => options.ExcludingMissingMembers());
            
            // Verificar campos técnicos
            foreach (var ordem in resultado)
            {
                ordem.Id.Should().NotBeEmpty();
                ordem.Ativo.Should().BeTrue();
                ordem.DataCadastro.Should().NotBe(default);
                ordem.DataAtualizacao.Should().NotBe(default);
            }
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoObterPorStatus.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<StatusOrdemServico>());
            _fixture.LogServicoObterPorStatus.Received(1).LogFim(Arg.Any<string>(), Arg.Any<IEnumerable<OrdemServico>>());
        }

        [Fact]
        public async Task Handle_ComStatusSemOrdens_DeveRetornarListaVazia()
        {
            var status = StatusOrdemServico.Cancelada;
            var ordensServico = new List<OrdemServico>();

            _fixture.ConfigurarMockRepositorioOrdemServicoParaObterPorStatus(status, ordensServico);

            var handler = _fixture.CriarObterOrdemServicoPorStatusHandler();

            // Act
            var resultado = await handler.Handle(status);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();

            // Verificar que o repositório foi chamado
            await _fixture.RepositorioOrdemServico.Received(1).ListarProjetadoAsync<OrdemServico>(Arg.Any<IEspecificacao<OrdemServicoEntityDto>>());

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterPorStatus.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<StatusOrdemServico>());
            _fixture.LogServicoObterPorStatus.Received(1).LogFim(Arg.Any<string>(), Arg.Any<IEnumerable<OrdemServico>>());
        }

        [Fact]
        public async Task Handle_QuandoRepositorioLancaExcecao_DeveRegistrarLogEPropagar()
        {
            // Arrange
            var status = StatusOrdemServico.Recebida;

            // Configurar o repositório para lançar uma exceção
            _fixture.RepositorioOrdemServico.ListarProjetadoAsync<OrdemServico>(Arg.Any<IEspecificacao<OrdemServicoEntityDto>>())
                .Returns(Task.FromException<IEnumerable<OrdemServico>>(new InvalidOperationException("Erro simulado")));

            var handler = _fixture.CriarObterOrdemServicoPorStatusHandler();

            // Act & Assert
            var act = async () => await handler.Handle(status);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Erro simulado");

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterPorStatus.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<StatusOrdemServico>());
            _fixture.LogServicoObterPorStatus.Received(1).LogErro(Arg.Any<string>(), Arg.Any<InvalidOperationException>());
        }
    }
}
