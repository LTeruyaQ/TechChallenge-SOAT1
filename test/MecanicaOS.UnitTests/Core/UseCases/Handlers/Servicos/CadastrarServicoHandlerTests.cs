using Core.DTOs.Entidades.Servico;
using Core.DTOs.UseCases.Servico;
using Core.Entidades;
using Core.Exceptions;
using Core.UseCases.Servicos.CadastrarServico;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.Handlers;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.Servicos
{
    public class CadastrarServicoHandlerTests
    {
        private readonly ServicoHandlerFixture _fixture;

        public CadastrarServicoHandlerTests()
        {
            _fixture = new ServicoHandlerFixture();
        }

        [Fact]
        public async Task Handle_ComDadosValidos_DeveRetornarServicoCadastrado()
        {
            // Arrange
            var dtoRequest = ServicoHandlerFixture.CriarCadastrarServicoDto();
            var servicoRetornado = ServicoHandlerFixture.CriarServicoValido(
                nome: dtoRequest.Nome,
                descricao: dtoRequest.Descricao,
                valor: dtoRequest.Valor,
                disponivel: dtoRequest.Disponivel);

            // Configurar o repositório para retornar o DTO quando chamado com CadastrarAsync
            var dtoResponse = new ServicoEntityDto
            {
                Id = servicoRetornado.Id,
                Nome = servicoRetornado.Nome,
                Descricao = servicoRetornado.Descricao,
                Valor = servicoRetornado.Valor,
                Disponivel = servicoRetornado.Disponivel,
                Ativo = servicoRetornado.Ativo,
                DataCadastro = servicoRetornado.DataCadastro,
                DataAtualizacao = servicoRetornado.DataAtualizacao
            };
            _fixture.RepositorioServico.CadastrarAsync(Arg.Any<ServicoEntityDto>()).Returns(dtoResponse);
            _fixture.ConfigurarMockUdtParaCommitSucesso();
            // Não precisamos configurar ObterPorNomeAsync, pois não é usado no handler

            var handler = _fixture.CriarCadastrarServicoHandler();

            // Act
            var resultado = await handler.Handle(dtoRequest);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Servico.Should().NotBeNull();
            resultado.Servico.Nome.Should().Be(dtoRequest.Nome);
            resultado.Servico.Descricao.Should().Be(dtoRequest.Descricao);
            resultado.Servico.Valor.Should().Be(dtoRequest.Valor);
            resultado.Servico.Disponivel.Should().Be(dtoRequest.Disponivel);

            // Verificar que o repositório foi chamado para cadastrar (através do gateway real)
            await _fixture.RepositorioServico.Received(1).CadastrarAsync(Arg.Any<ServicoEntityDto>());

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<CadastrarServicoUseCaseDto>());
            _fixture.LogServicoCadastrar.Received(1).LogFim(Arg.Any<string>(), Arg.Any<CadastrarServicoResponse>());
        }

        [Fact]
        public async Task Handle_ComNomeJaCadastrado_DeveLancarDadosJaCadastradosException()
        {
            // Arrange
            var dtoRequest = ServicoHandlerFixture.CriarCadastrarServicoDto();
            var servicoExistente = ServicoHandlerFixture.CriarServicoValido(nome: dtoRequest.Nome);

            // Configurar o repositório para retornar o serviço quando chamado com a especificação correta
            _fixture.RepositorioServico
                .ObterUmProjetadoSemRastreamentoAsync<Servico>(Arg.Any<global::Core.Especificacoes.Base.Interfaces.IEspecificacao<ServicoEntityDto>>())
                .Returns(servicoExistente);

            var handler = _fixture.CriarCadastrarServicoHandler();

            // Act & Assert
            var act = async () => await handler.Handle(dtoRequest);

            await act.Should().ThrowAsync<DadosJaCadastradosException>()
                .WithMessage("Já existe um serviço cadastrado com este nome");

            // Verificar que o repositório não foi chamado para cadastrar
            await _fixture.RepositorioServico.DidNotReceive().CadastrarAsync(Arg.Any<ServicoEntityDto>());

            // Verificar que o commit não foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceive().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<CadastrarServicoUseCaseDto>());
            _fixture.LogServicoCadastrar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosJaCadastradosException>());
        }

        [Fact]
        public async Task Handle_QuandoCommitFalha_DeveLancarPersistirDadosException()
        {
            // Arrange
            var dtoRequest = ServicoHandlerFixture.CriarCadastrarServicoDto();
            var servicoRetornado = ServicoHandlerFixture.CriarServicoValido(
                nome: dtoRequest.Nome,
                descricao: dtoRequest.Descricao,
                valor: dtoRequest.Valor,
                disponivel: dtoRequest.Disponivel);

            // Configurar o repositório para retornar o DTO quando chamado com CadastrarAsync
            var dtoResponse = new ServicoEntityDto
            {
                Id = servicoRetornado.Id,
                Nome = servicoRetornado.Nome,
                Descricao = servicoRetornado.Descricao,
                Valor = servicoRetornado.Valor,
                Disponivel = servicoRetornado.Disponivel,
                Ativo = servicoRetornado.Ativo,
                DataCadastro = servicoRetornado.DataCadastro,
                DataAtualizacao = servicoRetornado.DataAtualizacao
            };
            _fixture.RepositorioServico.CadastrarAsync(Arg.Any<ServicoEntityDto>()).Returns(dtoResponse);
            _fixture.ConfigurarMockUdtParaCommitFalha();
            // Não precisamos configurar ObterPorNomeAsync, pois não é usado no handler

            var handler = _fixture.CriarCadastrarServicoHandler();

            // Act & Assert
            var act = async () => await handler.Handle(dtoRequest);

            await act.Should().ThrowAsync<PersistirDadosException>()
                .WithMessage("Erro ao cadastrar serviço");

            // Verificar que o repositório foi chamado para cadastrar (através do gateway real)
            await _fixture.RepositorioServico.Received(1).CadastrarAsync(Arg.Any<ServicoEntityDto>());

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<CadastrarServicoUseCaseDto>());
            _fixture.LogServicoCadastrar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<PersistirDadosException>());
        }

        [Theory]
        [InlineData(null, "Descrição", 100)]
        [InlineData("", "Descrição", 100)]
        [InlineData("Nome", null, 100)]
        [InlineData("Nome", "", 100)]
        [InlineData("Nome", "Descrição", 0)]
        [InlineData("Nome", "Descrição", -1)]
        public async Task Handle_ComDadosInvalidos_DeveLancarDadosInvalidosException(string nome, string descricao, decimal valor)
        {
            // Arrange
            var dtoRequest = ServicoHandlerFixture.CriarCadastrarServicoDto(
                nome: nome,
                descricao: descricao,
                valor: valor);

            var handler = _fixture.CriarCadastrarServicoHandler();

            // Act & Assert
            var act = async () => await handler.Handle(dtoRequest);

            await act.Should().ThrowAsync<DadosInvalidosException>();

            // Verificar que o repositório não foi chamado para cadastrar
            await _fixture.RepositorioServico.DidNotReceive().CadastrarAsync(Arg.Any<ServicoEntityDto>());

            // Verificar que o commit não foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceive().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<CadastrarServicoUseCaseDto>());
            _fixture.LogServicoCadastrar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosInvalidosException>());
        }
    }
}
