using Adapters.Controllers;
using Core.DTOs.Requests.OrdemServico;
using Core.DTOs.Responses.OrdemServico;
using Core.DTOs.UseCases.OrdemServico;
using Core.Entidades;
using Core.Enumeradores;
using Core.Interfaces.Presenters;
using Core.Interfaces.root;
using Core.Interfaces.UseCases;

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
            
            // Configurar mocks padrão para ClienteUseCases e ServicoUseCases
            var clienteUseCases = Substitute.For<IClienteUseCases>();
            var servicoUseCases = Substitute.For<IServicoUseCases>();
            _compositionRoot.CriarClienteUseCases().Returns(clienteUseCases);
            _compositionRoot.CriarServicoUseCases().Returns(servicoUseCases);
            
            _ordemServicoController = new OrdemServicoController(_compositionRoot);

            // Usar reflexão para injetar o presenter mockado
            var presenterField = typeof(OrdemServicoController).GetField("_ordemServicoPresenter",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            presenterField?.SetValue(_ordemServicoController, _ordemServicoPresenter);
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
                Descricao = "Descrição da Ordem de Serviço Teste",
                Status = StatusOrdemServico.EmExecucao
            };

            // Act
            var result = _ordemServicoController.MapearParaAtualizarOrdemServicoUseCaseDto(request);

            // Assert
            result.Should().NotBeNull();
            // Apenas os campos que são mapeados no método atual
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
        public async Task Cadastrar_DeveValidarDependenciasEChamarUseCase()
        {
            // Arrange
            var request = new CadastrarOrdemServicoRequest
            {
                ClienteId = Guid.NewGuid(),
                VeiculoId = Guid.NewGuid(),
                ServicoId = Guid.NewGuid(),
                Descricao = "Descrição da Ordem de Serviço Teste"
            };

            var ordemServico = new OrdemServico
            {
                Id = Guid.NewGuid(),
                ClienteId = request.ClienteId,
                VeiculoId = request.VeiculoId,
                ServicoId = request.ServicoId,
                Descricao = request.Descricao,
                Status = StatusOrdemServico.Recebida,
                DataCadastro = DateTime.Now,
                Ativo = true
            };
            
            var ordemServicoResponse = new OrdemServicoResponse
            {
                Id = ordemServico.Id,
                ClienteId = ordemServico.ClienteId,
                VeiculoId = ordemServico.VeiculoId,
                ServicoId = ordemServico.ServicoId,
                Descricao = ordemServico.Descricao,
                Status = ordemServico.Status
            };
            
            // Criar objetos Cliente e Servico com todas as propriedades obrigatórias
            var cliente = new Cliente { 
                Id = request.ClienteId,
                Nome = "Cliente Teste",
                TipoCliente = TipoCliente.PessoaFisica,
                Documento = "12345678900",
                Ativo = true,
                DataCadastro = DateTime.Now.AddDays(-30)
            };
            
            var servico = new Servico { 
                Id = request.ServicoId,
                Nome = "Serviço Teste",
                Descricao = "Descrição do serviço teste",
                Valor = 100.0m,
                Disponivel = true,
                Ativo = true,
                DataCadastro = DateTime.Now.AddDays(-15)
            };

            // Obter os mocks já configurados no construtor
            var clienteUseCases = _compositionRoot.CriarClienteUseCases();
            var servicoUseCases = _compositionRoot.CriarServicoUseCases();
            
            // Configurar comportamentos específicos para este teste
            clienteUseCases.ObterPorIdUseCaseAsync(request.ClienteId).Returns(cliente);
            servicoUseCases.ObterServicoPorIdUseCaseAsync(request.ServicoId).Returns(servico);

            // Configure o mock para retornar a ordem de serviço
            _ordemServicoUseCases.CadastrarUseCaseAsync(Arg.Any<CadastrarOrdemServicoUseCaseDto>())
                .Returns(ordemServico);
            
            // Configure o presenter para retornar a resposta
            _ordemServicoPresenter.ParaResponse(Arg.Any<OrdemServico>())
                .Returns(ordemServicoResponse);

            // Act
            var result = await _ordemServicoController.Cadastrar(request);

            // Assert
            // Verificar validações de dependências
            await clienteUseCases.Received(1).ObterPorIdUseCaseAsync(request.ClienteId);
            await servicoUseCases.Received(1).ObterServicoPorIdUseCaseAsync(request.ServicoId);

            // Verificar que o método do usecase foi chamado com os parâmetros corretos
            await _ordemServicoUseCases.Received(1).CadastrarUseCaseAsync(Arg.Is<CadastrarOrdemServicoUseCaseDto>(
                dto => dto.ClienteId == request.ClienteId &&
                      dto.VeiculoId == request.VeiculoId &&
                      dto.ServicoId == request.ServicoId &&
                      dto.Descricao == request.Descricao &&
                      dto.Cliente == cliente &&
                      dto.Servico == servico));

            // Verificar que o presenter foi chamado com a entidade retornada pelo usecase
            _ordemServicoPresenter.Received(1).ParaResponse(ordemServico);
            
            // Verificar que o resultado é a resposta esperada com todos os campos
            result.Should().BeEquivalentTo(ordemServicoResponse);
            
            // Verificar campos principais
            result.Id.Should().Be(ordemServico.Id);
            result.Status.Should().Be(StatusOrdemServico.Recebida);
        }

        [Fact]
        public async Task Atualizar_DeveUsarMapeamentoEChamarUseCase()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new AtualizarOrdemServicoRequest
            {
                Descricao = "Descrição da Ordem de Serviço Atualizada",
                Status = StatusOrdemServico.EmExecucao
            };

            var ordemServico = new OrdemServico();
            var ordemServicoResponse = new OrdemServicoResponse();

            _ordemServicoUseCases.AtualizarUseCaseAsync(id, Arg.Any<AtualizarOrdemServicoUseCaseDto>())
                .Returns(ordemServico);
            _ordemServicoPresenter.ParaResponse(Arg.Any<OrdemServico>())
                .Returns(ordemServicoResponse);

            // Act
            var result = await _ordemServicoController.Atualizar(id, request);

            // Assert
            await _ordemServicoUseCases.Received(1).AtualizarUseCaseAsync(
                Arg.Is<Guid>(g => g == id),
                Arg.Is<AtualizarOrdemServicoUseCaseDto>(
                    dto => dto.Descricao == request.Descricao &&
                          dto.Status == request.Status));

            _ordemServicoPresenter.Received(1).ParaResponse(ordemServico);
            result.Should().Be(ordemServicoResponse);
        }
    }
}
