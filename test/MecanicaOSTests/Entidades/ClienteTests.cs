using Dominio.Entidades;
using Dominio.Enumeradores;
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
            cliente.Endereco.Should().Be(enderecoMock.Object);
            cliente.Contato.Should().Be(contatoMock.Object);
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
            var novoTipoCliente = TipoCliente.PessoaJuridica;
            var novaDataNascimento = "02/02/1992";

            // Act
            cliente.Atualizar(novoNome, novoSexo, novoTipoCliente, novaDataNascimento);

            // Assert
            cliente.Nome.Should().Be(novoNome);
            cliente.Sexo.Should().Be(novoSexo);
            cliente.TipoCliente.Should().Be(novoTipoCliente);
            cliente.DataNascimento.Should().Be(novaDataNascimento);
        }
    }
}
