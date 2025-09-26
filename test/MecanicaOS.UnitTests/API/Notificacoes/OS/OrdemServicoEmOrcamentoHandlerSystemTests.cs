using Core.DTOs.Responses.Estoque;
using Core.DTOs.Responses.OrdemServico.InsumoOrdemServico;
using Core.Interfaces.Controllers;
using Core.Interfaces.root;
using Core.Interfaces.Servicos;
using System.Text;

namespace MecanicaOS.UnitTests.API.Notificacoes.OS
{
    public class OrdemServicoEmOrcamentoHandlerSystemTests
    {
        private readonly OrdemServicoEmOrcamentoHandlerFixture _fixture;
        private readonly IOrdemServicoController _ordemServicoController;
        private readonly IServicoEmail _servicoEmail;
        private readonly ICompositionRoot _compositionRoot;

        public OrdemServicoEmOrcamentoHandlerSystemTests()
        {
            _fixture = new OrdemServicoEmOrcamentoHandlerFixture();
            _ordemServicoController = _fixture.OrdemServicoController;
            _servicoEmail = _fixture.ServicoEmail;

            _compositionRoot = Substitute.For<ICompositionRoot>();
            _compositionRoot.CriarOrdemServicoController().Returns(_ordemServicoController);
            _compositionRoot.CriarServicoEmail().Returns(_servicoEmail);
            _compositionRoot.CriarLogService<OrdemServicoEmOrcamentoHandlerMock>().Returns(_fixture.LogServicoMock);
        }

        [Fact]
        public async Task Handle_DeveProcessarFluxoCompleto_ComTemplateReal()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var evento = _fixture.CriarEvento(ordemServicoId);

            var ordemServico = _fixture.CriarOrdemServicoComOrcamento(ordemServicoId);
            _ordemServicoController.ObterPorId(ordemServicoId).Returns(ordemServico);

            // Criar diretório de templates se não existir
            var templateDir = Path.Combine(AppContext.BaseDirectory, "Templates");
            Directory.CreateDirectory(templateDir);

            // Criar arquivo de template
            var templatePath = Path.Combine(templateDir, "EmailOrcamentoOS.html");
            var templateContent = @"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>Orçamento</title>
                </head>
                <body>
                    <h1>Orçamento para {{NOME_CLIENTE}}</h1>
                    <p>Serviço: {{NOME_SERVICO}} - R$ {{VALOR_SERVICO}}</p>
                    <table>
                        <tr>
                            <th>Insumo</th>
                            <th>Valor</th>
                        </tr>
                        {{#each INSUMOS}}
                        <tr>
                            <td>Insumo (1 und)</td>
                            <td>R$ 0.00</td>
                        </tr>
                        {{/each}}
                    </table>
                    <p>Valor Total: R$ {{VALOR_TOTAL}}</p>
                </body>
                </html>
            ";
            File.WriteAllText(templatePath, templateContent, Encoding.UTF8);

            string emailCapturado = null;

            _servicoEmail.EnviarAsync(
                Arg.Any<string[]>(),
                Arg.Any<string>(),
                Arg.Do<string>(conteudo => emailCapturado = conteudo)
            ).Returns(Task.CompletedTask);

            var handler = new OrdemServicoEmOrcamentoHandlerMock(_compositionRoot);

            // Act
            await handler.Handle(evento, CancellationToken.None);

            // Assert
            // Verificar que o orçamento foi calculado
            await _ordemServicoController.Received(1).CalcularOrcamentoAsync(ordemServicoId);

            // Verificar que os detalhes da OS foram obtidos
            await _ordemServicoController.Received(1).ObterPorId(ordemServicoId);

            // Verificar que o email foi enviado
            await _servicoEmail.Received(1).EnviarAsync(
                Arg.Is<string[]>(emails => emails.Length == 1 && emails[0] == ordemServico.Cliente.Contato.Email),
                Arg.Is<string>(assunto => assunto == "Orçamento de Serviço"),
                Arg.Any<string>()
            );

            // Verificar conteúdo do email
            emailCapturado.Should().NotBeNull();
            emailCapturado.Should().Contain(ordemServico.Cliente.Nome);
            emailCapturado.Should().Contain(ordemServico.Servico.Nome);
            emailCapturado.Should().Contain(ordemServico.Servico.Valor.ToString("N2"));
            emailCapturado.Should().Contain(ordemServico.Orcamento!.Value.ToString("N2"));

            // Verificar que o template foi processado corretamente
            emailCapturado.Should().NotContain("{{NOME_CLIENTE}}");
            emailCapturado.Should().NotContain("{{NOME_SERVICO}}");
            emailCapturado.Should().NotContain("{{VALOR_SERVICO}}");
            emailCapturado.Should().NotContain("{{VALOR_TOTAL}}");
            emailCapturado.Should().NotContain("{{#each INSUMOS}}");
            emailCapturado.Should().NotContain("{{/each}}");

            // Verificar logs
            _fixture.LogServicoMock.Received(1).LogInicio(Arg.Any<string>(), ordemServicoId);
            _fixture.LogServicoMock.Received(1).LogFim(Arg.Any<string>(), Arg.Any<object>());

            // Limpar o arquivo de template após o teste
            File.Delete(templatePath);
        }

        [Fact]
        public async Task Handle_QuandoTemplateNaoExiste_DeveTratarErroCorretamente()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var evento = _fixture.CriarEvento(ordemServicoId);

            var ordemServico = _fixture.CriarOrdemServicoComOrcamento(ordemServicoId);
            _ordemServicoController.ObterPorId(ordemServicoId).Returns(ordemServico);

            // Criar um handler personalizado para este teste que lance uma exceção
            var handler = new OrdemServicoEmOrcamentoHandlerThrowsException(_compositionRoot);

            // Act & Assert
            var act = async () => await handler.Handle(evento, CancellationToken.None);

            // Deve lançar uma exceção porque o template não existe
            await act.Should().ThrowAsync<FileNotFoundException>();

            // Verificar que o orçamento foi calculado
            await _ordemServicoController.Received(1).CalcularOrcamentoAsync(ordemServicoId);

            // Verificar que os detalhes da OS foram obtidos
            await _ordemServicoController.Received(1).ObterPorId(ordemServicoId);

            // Verificar logs
            _fixture.LogServicoMock.Received(1).LogInicio(Arg.Any<string>(), ordemServicoId);
            _fixture.LogServicoMock.Received(1).LogErro(Arg.Any<string>(), Arg.Any<Exception>());
        }

        [Fact]
        public async Task Handle_DeveCalcularValorTotalCorretamente()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var evento = _fixture.CriarEvento(ordemServicoId);

            // Criar ordem de serviço com valores específicos
            var valorServico = 150.75M;
            var valorOrcamento = 325.50M; // Valor do serviço + valor dos insumos
            var ordemServico = _fixture.CriarOrdemServicoComOrcamento(ordemServicoId, valorServico, valorOrcamento);

            // Configurar insumos com valores específicos
            ordemServico.InsumosOS = new List<InsumoOSResponse>
            {
                new InsumoOSResponse
                {
                    OrdemServicoId = ordemServicoId,
                    EstoqueId = Guid.NewGuid(),
                    Quantidade = 2,
                    Estoque = new EstoqueResponse
                    {
                        Id = Guid.NewGuid(),
                        Insumo = "Óleo Sintético",
                        Preco = 45.50 // Total: 91.00
                    }
                },
                new InsumoOSResponse
                {
                    OrdemServicoId = ordemServicoId,
                    EstoqueId = Guid.NewGuid(),
                    Quantidade = 1,
                    Estoque = new EstoqueResponse
                    {
                        Id = Guid.NewGuid(),
                        Insumo = "Filtro de Óleo",
                        Preco = 83.75 // Total: 83.75
                    }
                }
            };
            // Valor total dos insumos: 91.00 + 83.75 = 174.75
            // Valor total do orçamento: 150.75 (serviço) + 174.75 (insumos) = 325.50

            _ordemServicoController.ObterPorId(ordemServicoId).Returns(ordemServico);

            // Criar diretório de templates se não existir
            var templateDir = Path.Combine(AppContext.BaseDirectory, "Templates");
            Directory.CreateDirectory(templateDir);

            // Criar arquivo de template
            var templatePath = Path.Combine(templateDir, "EmailOrcamentoOS.html");
            var templateContent = @"<p>Valor Total: R$ {{VALOR_TOTAL}}</p>";
            File.WriteAllText(templatePath, templateContent, Encoding.UTF8);

            string emailCapturado = null;

            _servicoEmail.EnviarAsync(
                Arg.Any<string[]>(),
                Arg.Any<string>(),
                Arg.Do<string>(conteudo => emailCapturado = conteudo)
            ).Returns(Task.CompletedTask);

            var handler = new OrdemServicoEmOrcamentoHandlerMock(_compositionRoot);

            // Act
            await handler.Handle(evento, CancellationToken.None);

            // Assert
            // Verificar que o email foi enviado
            await _servicoEmail.Received(1).EnviarAsync(
                Arg.Any<string[]>(),
                Arg.Any<string>(),
                Arg.Any<string>()
            );

            // Verificar que o valor total no email está correto
            emailCapturado.Should().NotBeNull();
            emailCapturado.Should().Contain("325,50"); // Formatado como 325,50 (pt-BR)

            // Limpar o arquivo de template após o teste
            File.Delete(templatePath);
        }
    }
}
