using Dominio.Entidades;
using Dominio.Interfaces.Servicos;

namespace Infraestrutura.Servicos;

public class ServicoNotificacaoEmail : IServicoNotificacaoEmail
{
    public Task EnviarAlertaEstoqueAsync(IEnumerable<Estoque> itensCriticos)
    {
        // TODO: Implementar lógica para formatar e enviar e-mail

        var corpo = string.Join("\n", itensCriticos.Select(i => $"- {i.Insumo} ({i.QuantidadeDisponivel})"));

        Console.WriteLine("Enviar e-mail para mecânica com os seguintes itens em baixa:\n" + corpo);

        return Task.CompletedTask;
    }
}