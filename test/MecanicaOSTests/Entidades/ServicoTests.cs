using FluentAssertions;

namespace MecanicaOSTests.Entidades
{
    public class ServicoTests
    {
        [Fact]
        public void Dado_DadosValidos_Quando_CriarServico_Entao_DeveCriarComSucesso()
        {
            // Arrange
            var nome = "Serviço Teste";
            var descricao = "Descrição do Serviço";
            var valor = 150.00m;
            var disponivel = true;

            // Act
            var servico = new Servico
            {
                Nome = nome,
                Descricao = descricao,
                Valor = valor,
                Disponivel = disponivel
            };

            // Assert
            servico.Should().NotBeNull();
            servico.Nome.Should().Be(nome);
            servico.Descricao.Should().Be(descricao);
            servico.Valor.Should().Be(valor);
            servico.Disponivel.Should().Be(disponivel);
        }

        [Fact]
        public void Dado_DadosValidos_Quando_Atualizar_Entao_DeveAtualizarComSucesso()
        {
            // Arrange
            var servico = new Servico
            {
                Nome = "Nome Antigo",
                Descricao = "Descrição Antiga",
                Valor = 100.00m,
                Disponivel = true
            };
            var novoNome = "Novo Nome";
            var novaDescricao = "Nova Descrição";
            var novoValor = 200.00m;
            var novoDisponivel = false;

            // Act
            servico.Atualizar(novoNome, novaDescricao, novoValor, novoDisponivel);

            // Assert
            servico.Nome.Should().Be(novoNome);
            servico.Descricao.Should().Be(novaDescricao);
            servico.Valor.Should().Be(novoValor);
            servico.Disponivel.Should().Be(novoDisponivel);
        }
    }
}
