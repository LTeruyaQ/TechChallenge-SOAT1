using Core.DTOs.UseCases.Orcamento;
using Core.Entidades;
using MecanicaOS.UnitTests.Fixtures.Handlers;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.Orcamentos
{
    public class GerarOrcamentoHandlerTests
    {
        private readonly OrcamentoHandlerFixture _fixture;

        public GerarOrcamentoHandlerTests()
        {
            _fixture = new OrcamentoHandlerFixture();
        }

        [Fact]
        public void Handle_ComOrdemServicoComInsumos_DeveCalcularValorCorretamente()
        {
            // Arrange
            var ordemServico = OrcamentoHandlerFixture.CriarOrdemServicoComInsumosValida();
            var useCase = new GerarOrcamentoUseCase(ordemServico);

            // Calcular o valor esperado manualmente
            decimal valorServicoEsperado = ordemServico.Servico.Valor;
            decimal valorInsumosEsperado = ordemServico.InsumosOS.Sum(i => i.Quantidade * i.Estoque.Preco);
            decimal valorTotalEsperado = valorServicoEsperado + valorInsumosEsperado;

            var handler = _fixture.CriarGerarOrcamentoHandler();

            // Act
            var resultado = handler.Handle(useCase);

            // Assert
            resultado.Should().Be(valorTotalEsperado);

            // Verificar os valores específicos
            valorServicoEsperado.Should().Be(100.00m);
            valorInsumosEsperado.Should().Be((50.00m * 2) + (25.00m * 1)); // 2 óleos + 1 filtro
            resultado.Should().Be(225.00m); // 100 + 100 + 25

            // Não verificamos logs porque o método não está chamando LogInicio no handler
        }

        [Fact]
        public void Handle_ComOrdemServicoSemInsumos_DeveCalcularApenasValorDoServico()
        {
            // Arrange
            var ordemServico = OrcamentoHandlerFixture.CriarOrdemServicoSemInsumosValida();
            var useCase = new GerarOrcamentoUseCase(ordemServico);

            // Calcular o valor esperado manualmente
            decimal valorServicoEsperado = ordemServico.Servico.Valor;
            decimal valorInsumosEsperado = 0m; // Sem insumos
            decimal valorTotalEsperado = valorServicoEsperado + valorInsumosEsperado;

            var handler = _fixture.CriarGerarOrcamentoHandler();

            // Act
            var resultado = handler.Handle(useCase);

            // Assert
            resultado.Should().Be(valorTotalEsperado);

            // Verificar os valores específicos
            valorServicoEsperado.Should().Be(80.00m);
            valorInsumosEsperado.Should().Be(0m);
            resultado.Should().Be(80.00m);

            // Não verificamos logs porque o método não está chamando LogInicio no handler
        }

        [Fact]
        public void Handle_ComOrdemServicoComInsumosVariados_DeveCalcularValorCorretamente()
        {
            // Arrange
            var ordemServico = OrcamentoHandlerFixture.CriarOrdemServicoComInsumosValida();

            // Adicionar mais um insumo para testar
            var estoque3 = new Estoque
            {
                Id = Guid.NewGuid(),
                Insumo = "Fluido de Freio",
                Descricao = "Fluido de freio DOT 4",
                Preco = 30.00m,
                QuantidadeDisponivel = 8,
                QuantidadeMinima = 2
            };

            var insumoOS3 = new InsumoOS
            {
                Id = Guid.NewGuid(),
                EstoqueId = estoque3.Id,
                Estoque = estoque3,
                Quantidade = 3
            };

            // Adicionar o novo insumo à lista
            var insumosAtualizados = new List<InsumoOS>(ordemServico.InsumosOS)
            {
                insumoOS3
            };

            ordemServico.InsumosOS = insumosAtualizados;

            var useCase = new GerarOrcamentoUseCase(ordemServico);

            // Calcular o valor esperado manualmente
            decimal valorServicoEsperado = ordemServico.Servico.Valor;
            decimal valorInsumosEsperado = ordemServico.InsumosOS.Sum(i => i.Quantidade * i.Estoque.Preco);
            decimal valorTotalEsperado = valorServicoEsperado + valorInsumosEsperado;

            var handler = _fixture.CriarGerarOrcamentoHandler();

            // Act
            var resultado = handler.Handle(useCase);

            // Assert
            resultado.Should().Be(valorTotalEsperado);

            // Verificar os valores específicos
            valorServicoEsperado.Should().Be(100.00m);
            valorInsumosEsperado.Should().Be((50.00m * 2) + (25.00m * 1) + (30.00m * 3)); // 2 óleos + 1 filtro + 3 fluidos
            resultado.Should().Be(315.00m); // 100 + 100 + 25 + 90

            // Não verificamos logs porque o método não está chamando LogInicio no handler
        }

        [Fact]
        public void Handle_ComOrdemServicoNula_DeveLancarArgumentNullException()
        {
            // Arrange
            OrdemServico ordemServico = null;

            var handler = _fixture.CriarGerarOrcamentoHandler();

            // Act & Assert
            Action act = () => new GerarOrcamentoUseCase(ordemServico);

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("ordemServico");
        }

        [Fact]
        public void Handle_ComServicoNulo_DeveLancarNullReferenceException()
        {
            // Arrange
            var ordemServico = OrcamentoHandlerFixture.CriarOrdemServicoComInsumosValida();
            ordemServico.Servico = null;

            var useCase = new GerarOrcamentoUseCase(ordemServico);
            var handler = _fixture.CriarGerarOrcamentoHandler();

            // Act & Assert
            Action act = () => handler.Handle(useCase);

            act.Should().Throw<NullReferenceException>();
        }

        [Fact]
        public void Handle_ComInsumosComEstoqueNulo_DeveLancarNullReferenceException()
        {
            // Arrange
            var ordemServico = OrcamentoHandlerFixture.CriarOrdemServicoComInsumosValida();

            // Criar um insumo com estoque nulo
            var insumoComEstoqueNulo = new InsumoOS
            {
                Id = Guid.NewGuid(),
                EstoqueId = Guid.NewGuid(),
                Estoque = null,
                Quantidade = 1
            };

            // Adicionar o insumo com estoque nulo à lista
            var insumosAtualizados = new List<InsumoOS>(ordemServico.InsumosOS)
            {
                insumoComEstoqueNulo
            };

            ordemServico.InsumosOS = insumosAtualizados;

            var useCase = new GerarOrcamentoUseCase(ordemServico);
            var handler = _fixture.CriarGerarOrcamentoHandler();

            // Act & Assert
            Action act = () => handler.Handle(useCase);

            act.Should().Throw<NullReferenceException>();
        }

        [Fact]
        public void Handle_DevePassarDadosCorretamenteEntreHandlerEResponse()
        {
            // Arrange
            var ordemServico = OrcamentoHandlerFixture.CriarOrdemServicoComInsumosValida();
            var useCase = new GerarOrcamentoUseCase(ordemServico);

            // Calcular o valor esperado manualmente
            decimal valorServicoEsperado = 100.00m;
            decimal valorInsumosEsperado = (50.00m * 2) + (25.00m * 1); // 2 óleos + 1 filtro
            decimal valorTotalEsperado = 225.00m; // 100 + 100 + 25

            var handler = _fixture.CriarGerarOrcamentoHandler();

            // Act
            var resultado = handler.Handle(useCase);

            // Assert
            resultado.Should().Be(valorTotalEsperado);

            // Verificar que o valor do orçamento é exatamente o esperado
            resultado.Should().Be(225.00m);

            // Verificar que o valor do orçamento é a soma do valor do serviço e dos insumos
            resultado.Should().Be(valorServicoEsperado + valorInsumosEsperado);

        }
    }
}
