using API.Notificacoes.OS;
using Core.DTOs.Responses.OrdemServico;
using Core.DTOs.Responses.OrdemServico.InsumoOrdemServico;
using Core.Interfaces.Controllers;
using Core.Interfaces.root;
using Core.Interfaces.Servicos;
using MediatR;

namespace MecanicaOS.UnitTests.API.Notificacoes.OS
{
    /// <summary>
    /// Versão de teste do OrdemServicoEmOrcamentoHandler que não depende de arquivos de template
    /// </summary>
    public class OrdemServicoEmOrcamentoHandlerMock : INotificationHandler<OrdemServicoEmOrcamentoEvent>
    {
        private readonly IServicoEmail _emailServico;
        private readonly ILogServico<OrdemServicoEmOrcamentoHandlerMock> _logServico;
        private readonly IOrdemServicoController _ordemServicoController;

        public OrdemServicoEmOrcamentoHandlerMock(ICompositionRoot compositionRoot)
        {
            _emailServico = compositionRoot.CriarServicoEmail();
            _logServico = compositionRoot.CriarLogService<OrdemServicoEmOrcamentoHandlerMock>();
            _ordemServicoController = compositionRoot.CriarOrdemServicoController();
        }

        public async Task Handle(OrdemServicoEmOrcamentoEvent notification, CancellationToken cancellationToken)
        {
            var metodo = nameof(Handle);

            try
            {
                _logServico.LogInicio(metodo, notification.OrdemServicoId);

                await _ordemServicoController.CalcularOrcamentoAsync(notification.OrdemServicoId);

                var os = await _ordemServicoController.ObterPorId(notification.OrdemServicoId);

                await EnviarOrcamentoAsync(os);

                _logServico.LogFim(metodo, null);
            }
            catch (Exception e)
            {
                _logServico.LogErro(metodo, e);
                throw;
            }
        }

        private async Task EnviarOrcamentoAsync(OrdemServicoResponse os)
        {
            if (os.Cliente?.Contato?.Email == null)
            {
                throw new Exception("Cliente sem email cadastrado");
            }

            string conteudo = GerarConteudoEmail(os);

            await _emailServico.EnviarAsync(
                new[] { os.Cliente.Contato.Email },
                "Orçamento de Serviço",
                conteudo);
        }

        private string GerarConteudoEmail(OrdemServicoResponse os)
        {
            // Versão simplificada para testes que não depende de arquivos
            return $@"
                <html>
                <body>
                    <h1>Orçamento para {os.Cliente.Nome}</h1>
                    <p>Serviço: {os.Servico.Nome} - R$ {os.Servico.Valor:N2}</p>
                    <table>
                        <tr>
                            <th>Insumo</th>
                            <th>Valor</th>
                        </tr>
                        {GerarHtmlInsumos(os.InsumosOS)}
                    </table>
                    <p>Valor Total: R$ {os.Orcamento:N2}</p>
                </body>
                </html>
            ";
        }

        private string GerarHtmlInsumos(IEnumerable<InsumoOSResponse> insumosOS)
        {
            if (insumosOS == null || !insumosOS.Any())
            {
                return "<tr><td colspan=\"2\">Nenhum insumo</td></tr>";
            }

            return string.Join(Environment.NewLine, insumosOS.Select(i =>
            {
                var descricao = i.Estoque.Insumo;
                var quantidade = i.Quantidade;
                var precoTotal = quantidade * i.Estoque.Preco;
                return $@"
                    <tr>
                        <td>{descricao} ({quantidade} und)</td>
                        <td>R$ {precoTotal:N2}</td>
                    </tr>
                ";
            }));
        }
    }
}
