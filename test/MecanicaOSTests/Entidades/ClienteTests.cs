using Bogus;
using Dominio.Entidades;
using FluentAssertions;
using MecanicaOSTests.Fixtures;
using Xunit;

namespace MecanicaOSTests.Entidades
{
    [Collection(nameof(ClienteCollection))]
    public class ClienteTests
    {
        private readonly ClienteFixture _clienteFixture;

        public ClienteTests(ClienteFixture clienteFixture)
        {
            _clienteFixture = clienteFixture;
        }

        [Fact]
        public void Dado_DadosValidos_Quando_CriarCliente_Entao_DeveCriarComSucesso()
        {
            //Arrange
            var cliente = _clienteFixture.GerarCliente();

            //Act

            //Assert
            cliente.Should().NotBeNull();
            cliente.Nome.Should().NotBeNullOrEmpty();
            cliente.Documento.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Dado_DadosValidos_Quando_AtualizarCliente_Entao_DeveAtualizarComSucesso()
        {
            //Arrange
            var cliente = _clienteFixture.GerarCliente();
            var novoNome = new Faker().Person.FullName;
            var novoSexo = "Feminino";
            var novoTipoCliente = Dominio.Enumeradores.TipoCliente.PessoaJuridica;
            var novaDataNascimento = "2000-01-01";

            //Act
            cliente.Atualizar(novoNome, novoSexo, novoTipoCliente, novaDataNascimento);

            //Assert
            cliente.Nome.Should().Be(novoNome);
            cliente.Sexo.Should().Be(novoSexo);
            cliente.TipoCliente.Should().Be(novoTipoCliente);
            cliente.DataNascimento.Should().Be(novaDataNascimento);
        }

        [Fact]
        public void Dado_DadosNulosOuVazios_Quando_AtualizarCliente_Entao_NaoDeveAtualizar()
        {
            //Arrange
            var cliente = _clienteFixture.GerarCliente();
            var nomeOriginal = cliente.Nome;
            var sexoOriginal = cliente.Sexo;
            var tipoClienteOriginal = cliente.TipoCliente;
            var dataNascimentoOriginal = cliente.DataNascimento;

            //Act
            cliente.Atualizar(null, null, null, null);

            //Assert
            cliente.Nome.Should().Be(nomeOriginal);
            cliente.Sexo.Should().Be(sexoOriginal);
            cliente.TipoCliente.Should().Be(tipoClienteOriginal);
            cliente.DataNascimento.Should().Be(dataNascimentoOriginal);
        }
    }
}
