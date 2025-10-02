using Adapters.Controllers;
using Core.DTOs.Requests.Cliente;
using Core.DTOs.UseCases.Cliente;
using Core.Entidades;
using Core.Enumeradores;
using Core.Interfaces.Presenters;
using Core.Interfaces.root;
using Core.Interfaces.UseCases;

namespace MecanicaOS.UnitTests.Adapters.Controllers
{
    public class ClienteControllerTests
    {
        private readonly IClienteUseCases _clienteUseCases;
        private readonly IClientePresenter _clientePresenter;
        private readonly ClienteController _clienteController;
        private readonly ICompositionRoot _compositionRoot;

        public ClienteControllerTests()
        {
            _clienteUseCases = Substitute.For<IClienteUseCases>();
            _clientePresenter = Substitute.For<IClientePresenter>();
            _compositionRoot = Substitute.For<ICompositionRoot>();

            _compositionRoot.CriarClienteUseCases().Returns(_clienteUseCases);
            _clienteController = new ClienteController(_compositionRoot);

            // Usar reflexão para injetar o presenter mockado
            var presenterField = typeof(ClienteController).GetField("_clientePresenter",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            presenterField?.SetValue(_clienteController, _clientePresenter);
        }


        [Fact]
        public void MapearParaCadastrarClienteUseCaseDto_ComRequestNulo_DeveRetornarNulo()
        {
            // Act
            var result = _clienteController.MapearParaCadastrarClienteUseCaseDto(null);

            // Assert
            result.Should().BeNull();
        }


        [Fact]
        public void MapearParaAtualizarClienteUseCaseDto_ComRequestNulo_DeveRetornarNulo()
        {
            // Act
            var result = _clienteController.MapearParaAtualizarClienteUseCaseDto(null);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Cadastrar_DeveUsarMapeamentoEChamarUseCase()
        {
            // Arrange
            var request = new CadastrarClienteRequest
            {
                Nome = "Cliente Teste",
                Sexo = "Masculino",
                Documento = "12345678900",
                DataNascimento = "1990-01-01",
                TipoCliente = TipoCliente.PessoaFisica,
                Rua = "Rua Teste",
                Bairro = "Bairro Teste",
                Cidade = "Cidade Teste",
                Numero = "123",
                CEP = "12345-678",
                Complemento = "Complemento Teste",
                Email = "teste@email.com",
                Telefone = "11987654321"
            };

            var cliente = new Cliente();
            _clienteUseCases.CadastrarUseCaseAsync(Arg.Any<CadastrarClienteUseCaseDto>())
                .Returns(cliente);

            // Act
            await _clienteController.Cadastrar(request);

            // Assert
            await _clienteUseCases.Received(1).CadastrarUseCaseAsync(Arg.Is<CadastrarClienteUseCaseDto>(
                dto => dto.Nome == request.Nome &&
                      dto.Documento == request.Documento &&
                      dto.Email == request.Email));

            _clientePresenter.Received(1).ParaResponse(cliente);
        }

        [Fact]
        public async Task Atualizar_DeveUsarMapeamentoEChamarUseCase()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new AtualizarClienteRequest
            {
                Id = Guid.NewGuid(),
                Nome = "Cliente Atualizado",
                Sexo = "Feminino",
                Documento = "98765432100",
                DataNascimento = "1995-05-05",
                TipoCliente = TipoCliente.PessoaJuridico
            };

            var cliente = new Cliente();
            _clienteUseCases.AtualizarUseCaseAsync(id, Arg.Any<AtualizarClienteUseCaseDto>())
                .Returns(cliente);

            // Act
            await _clienteController.Atualizar(id, request);

            // Assert
            await _clienteUseCases.Received(1).AtualizarUseCaseAsync(
                Arg.Is<Guid>(g => g == id),
                Arg.Is<AtualizarClienteUseCaseDto>(
                    dto => dto.Nome == request.Nome &&
                          dto.Documento == request.Documento &&
                          dto.TipoCliente == request.TipoCliente));

            _clientePresenter.Received(1).ParaResponse(cliente);
        }
    }
}
