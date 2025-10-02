using Core.DTOs.Entidades.Estoque;
using Core.DTOs.UseCases.Estoque;
using Core.Entidades;
using Core.Exceptions;
using MecanicaOS.UnitTests.Fixtures.Handlers;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.Estoques
{
    public class CadastrarEstoqueHandlerTests
    {
        private readonly EstoqueHandlerFixture _fixture;

        public CadastrarEstoqueHandlerTests()
        {
            _fixture = new EstoqueHandlerFixture();
        }

        [Fact]
        public async Task Handle_ComDadosValidos_DeveCadastrarEstoqueCorretamente()
        {
            // Arrange
            var request = EstoqueHandlerFixture.CriarCadastrarEstoqueUseCaseDtoValido();
            var estoqueEsperado = new Estoque
            {
                Id = Guid.NewGuid(),
                Insumo = request.Insumo,
                Descricao = request.Descricao,
                QuantidadeDisponivel = request.QuantidadeDisponivel,
                QuantidadeMinima = request.QuantidadeMinima,
                Preco = request.Preco,
                Ativo = true,
                DataCadastro = DateTime.UtcNow,
                DataAtualizacao = DateTime.UtcNow
            };

            // Configurar o repositório para simular o cadastro
            _fixture.ConfigurarMockRepositorioEstoqueParaCadastrar(estoqueEsperado);
            _fixture.RepositorioEstoque.ObterTodosAsync().Returns(new List<EstoqueEntityDto>());

            var handler = _fixture.CriarCadastrarEstoqueHandler();

            // Act
            var resultado = await handler.Handle(request);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().NotBeNull();

            // Verificar que o gateway foi chamado com os dados corretos
            await _fixture.RepositorioEstoque.Received(1).CadastrarAsync(Arg.Any<EstoqueEntityDto>());

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<CadastrarEstoqueUseCaseDto>());
            _fixture.LogServicoCadastrar.Received(1).LogFim(Arg.Any<string>(), Arg.Any<Estoque>());
        }

        [Fact]
        public async Task Handle_ComInsumoJaCadastrado_DeveLancarDadosJaCadastradosException()
        {
            // Arrange
            var request = EstoqueHandlerFixture.CriarCadastrarEstoqueUseCaseDtoValido();
            var estoqueExistente = new Estoque
            {
                Id = Guid.NewGuid(),
                Insumo = request.Insumo, // Mesmo nome
                Descricao = "Descrição existente",
                QuantidadeDisponivel = 5,
                QuantidadeMinima = 2,
                Preco = 10.00m,
                Ativo = true
            };

            // Configurar o repositório para retornar um estoque com o mesmo nome
            var estoquesDto = new List<EstoqueEntityDto>
            {
                new EstoqueEntityDto
                {
                    Id = estoqueExistente.Id,
                    Insumo = estoqueExistente.Insumo,
                    Descricao = estoqueExistente.Descricao,
                    QuantidadeDisponivel = estoqueExistente.QuantidadeDisponivel,
                    QuantidadeMinima = estoqueExistente.QuantidadeMinima,
                    Preco = estoqueExistente.Preco,
                    Ativo = estoqueExistente.Ativo
                }
            };
            _fixture.RepositorioEstoque.ObterTodosAsync().Returns(estoquesDto);

            var handler = _fixture.CriarCadastrarEstoqueHandler();

            // Act & Assert
            var act = async () => await handler.Handle(request);

            await act.Should().ThrowAsync<DadosJaCadastradosException>()
                .WithMessage("Produto já cadastrado");

            // Verificar que o gateway foi chamado para obter todos os estoques
            await _fixture.RepositorioEstoque.Received(1).ObterTodosAsync();

            // Verificar que o gateway NÃO foi chamado para cadastrar
            await _fixture.RepositorioEstoque.DidNotReceive().CadastrarAsync(Arg.Any<EstoqueEntityDto>());

            // Verificar que o commit NÃO foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceive().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<CadastrarEstoqueUseCaseDto>());
            _fixture.LogServicoCadastrar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosJaCadastradosException>());
        }

        [Fact]
        public async Task Handle_QuandoCommitFalha_DeveLancarPersistirDadosException()
        {
            // Arrange
            var request = EstoqueHandlerFixture.CriarCadastrarEstoqueUseCaseDtoValido();

            // Configurar o repositório para simular o cadastro
            _fixture.RepositorioEstoque.ObterTodosAsync().Returns(new List<EstoqueEntityDto>());

            // Configurar o UDT para falhar no commit
            _fixture.ConfigurarMockUdtParaCommitFalha();

            var handler = _fixture.CriarCadastrarEstoqueHandler();

            // Act & Assert
            var act = async () => await handler.Handle(request);

            await act.Should().ThrowAsync<PersistirDadosException>()
                .WithMessage("Erro ao cadastrar estoque");

            // Verificar que o gateway foi chamado para cadastrar
            await _fixture.RepositorioEstoque.Received(1).CadastrarAsync(Arg.Any<EstoqueEntityDto>());

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();

            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<CadastrarEstoqueUseCaseDto>());
        }

        [Fact]
        public async Task Handle_DevePreservarCamposTecnicosDeAuditoriaAoCadastrar()
        {
            // Arrange
            // Criar um DTO com valores específicos para identificar no teste
            var request = new CadastrarEstoqueUseCaseDto
            {
                Insumo = "Produto Específico para Teste",
                Descricao = "Descrição específica para teste de trânsito de dados",
                QuantidadeDisponivel = 42,
                QuantidadeMinima = 10,
                Preco = 123.45m
            };

            // Capturar o DTO que será enviado para o repositório
            EstoqueEntityDto dtoCadastrado = null;
            var dataCadastro = DateTime.UtcNow;

            _fixture.RepositorioEstoque.CadastrarAsync(Arg.Any<EstoqueEntityDto>())
                .Returns(callInfo =>
                {
                    dtoCadastrado = callInfo.Arg<EstoqueEntityDto>();
                    dtoCadastrado.Id = Guid.NewGuid();
                    dtoCadastrado.Ativo = true;
                    dtoCadastrado.DataCadastro = dataCadastro;
                    dtoCadastrado.DataAtualizacao = dataCadastro;
                    return Task.FromResult(dtoCadastrado);
                });

            _fixture.RepositorioEstoque.ObterTodosAsync().Returns(new List<EstoqueEntityDto>());
            var handler = _fixture.CriarCadastrarEstoqueHandler();

            // Act
            var resultado = await handler.Handle(request);

            // Assert
            // Verificar que o gateway foi chamado
            await _fixture.RepositorioEstoque.Received(1).CadastrarAsync(Arg.Any<EstoqueEntityDto>());

            // Verificar que os dados foram passados corretamente para o gateway
            dtoCadastrado.Should().NotBeNull();
            dtoCadastrado.Insumo.Should().Be("Produto Específico para Teste");
            dtoCadastrado.Descricao.Should().Be("Descrição específica para teste de trânsito de dados");
            dtoCadastrado.QuantidadeDisponivel.Should().Be(42);
            dtoCadastrado.QuantidadeMinima.Should().Be(10);
            dtoCadastrado.Preco.Should().Be(123.45m);
            
            // Verificar campos técnicos de auditoria no DTO
            dtoCadastrado.Id.Should().NotBeEmpty();
            dtoCadastrado.Ativo.Should().BeTrue();
            dtoCadastrado.DataCadastro.Should().BeCloseTo(dataCadastro, TimeSpan.FromSeconds(5));
            dtoCadastrado.DataAtualizacao.Should().BeCloseTo(dataCadastro, TimeSpan.FromSeconds(5));

            // Verificar que o resultado contém os mesmos dados e campos técnicos
            resultado.Should().NotBeNull();
            resultado.Id.Should().NotBeEmpty();
            resultado.Insumo.Should().Be("Produto Específico para Teste");
            resultado.Descricao.Should().Be("Descrição específica para teste de trânsito de dados");
            resultado.QuantidadeDisponivel.Should().Be(42);
            resultado.QuantidadeMinima.Should().Be(10);
            resultado.Preco.Should().Be(123.45m);
            resultado.Ativo.Should().BeTrue();
            resultado.DataCadastro.Should().BeCloseTo(dataCadastro, TimeSpan.FromSeconds(5));
            // DataAtualizacao não é preenchido durante o cadastro, apenas em atualizações
        }
    }
}
