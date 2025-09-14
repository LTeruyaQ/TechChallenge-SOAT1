using FluentAssertions;
using Moq;

namespace MecanicaOSTests.Entidades
{
    public class ClienteTests
    {
        [Fact]
        public void Dado_DadosValidos_Quando_CriarCliente_Entao_DeveCriarComSucesso()
        {
            // Arrange
            var nome = "Cliente Teste";
            var sexo = "Masculino";
            var documento = "12345678901";
            var dataNascimento = "01/01/1990";
            var tipoCliente = TipoCliente.PessoaFisica;
            var enderecoMock = new Mock<Endereco>();
            var contatoMock = new Mock<Contato>();

            // Act
            var cliente = new Cliente
            {
                Nome = nome,
                Sexo = sexo,
                Documento = documento,
                DataNascimento = dataNascimento,
                TipoCliente = tipoCliente,
                Endereco = enderecoMock.Object,
                Contato = contatoMock.Object
            };

            // Assert
            cliente.Should().NotBeNull();
            cliente.Nome.Should().Be(nome);
            cliente.Sexo.Should().Be(sexo);
            cliente.Documento.Should().Be(documento);
            cliente.DataNascimento.Should().Be(dataNascimento);
            cliente.TipoCliente.Should().Be(tipoCliente);
            cliente.Endereco.Should().BeEquivalentTo(enderecoMock.Object);
            cliente.Contato.Should().BeEquivalentTo(contatoMock.Object);
        }

        [Fact]
        public void Dado_DadosValidos_Quando_Atualizar_Entao_DeveAtualizarComSucesso()
        {
            // Arrange
            var cliente = new Cliente
            {
                Nome = "Nome Antigo",
                Sexo = "Feminino",
                Documento = "12345678901",
                DataNascimento = "01/01/1990",
                TipoCliente = TipoCliente.PessoaFisica,
                Endereco = new Mock<Endereco>().Object,
                Contato = new Mock<Contato>().Object
            };
            var novoNome = "Novo Nome";
            var novoSexo = "Masculino";
            var novoTipoCliente = TipoCliente.PessoaJuridico;
            var novaDataNascimento = "02/02/1992";

            // Act
            cliente.Atualizar(novoNome, novoSexo, novoTipoCliente, novaDataNascimento);

            // Assert
            cliente.Nome.Should().Be(novoNome);
            cliente.Sexo.Should().Be(novoSexo);
            cliente.TipoCliente.Should().Be(novoTipoCliente);
            cliente.DataNascimento.Should().Be(novaDataNascimento);
        }

        [Fact]
        public void DuasEntidadesComMesmoId_DevemSerConsideradasIguais()
        {
            // Arrange
            var id = Guid.NewGuid();
            var cliente1 = new Cliente { Id = id };
            var cliente2 = new Cliente { Id = id };

            // Assert
            (cliente1 == cliente2).Should().BeTrue();
            cliente1.Equals(cliente2).Should().BeTrue();
        }

        [Fact]
        public void DuasEntidadesComIdsDiferentes_NaoDevemSerConsideradasIguais()
        {
            // Arrange
            var cliente1 = new Cliente { Id = Guid.NewGuid() };
            var cliente2 = new Cliente { Id = Guid.NewGuid() };

            // Assert
            (cliente1 != cliente2).Should().BeTrue();
            cliente1.Equals(cliente2).Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_DeveRetornarMesmoValorParaMesmoId()
        {
            // Arrange
            var id = Guid.NewGuid();
            var cliente1 = new Cliente { Id = id };
            var cliente2 = new Cliente { Id = id };

            // Assert
            cliente1.GetHashCode().Should().Be(cliente2.GetHashCode());
        }

        [Fact]
        public void Equals_DeveRetornarFalse_ParaObjetoNulo()
        {
            // Arrange
            var cliente = new Cliente();

            // Assert
            cliente.Equals(null).Should().BeFalse();
        }

        [Fact]
        public void Equals_DeveRetornarFalse_ParaTipoDiferente()
        {
            // Arrange
            var cliente = new Cliente();
            var outroObjeto = new object();

            // Assert
            cliente.Equals(outroObjeto).Should().BeFalse();
        }
    }
}
