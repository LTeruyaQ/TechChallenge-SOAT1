using Dominio.Entidades;
using Dominio.Enumeradores;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace MecanicaOSTests.Entidades
{
    public class ClienteTests
    {
        [Fact]
        public void Dado_NovosValores_Quando_SetarPropriedades_Entao_DeveAtribuirCorretamente()
        {
            // Arrange
            var cliente = new Cliente();
            var nome = "Cliente Teste";
            var sexo = "Masculino";
            var documento = "12345678901";
            var dataNascimento = "2000-01-01";
            var tipoCliente = TipoCliente.PessoaFisica;
            var endereco = new Endereco();
            var contato = new Contato();
            var veiculos = new List<Veiculo>();

            // Act
            cliente.Nome = nome;
            cliente.Sexo = sexo;
            cliente.Documento = documento;
            cliente.DataNascimento = dataNascimento;
            cliente.TipoCliente = tipoCliente;
            cliente.Endereco = endereco;
            cliente.Contato = contato;
            cliente.Veiculos = veiculos;

            // Assert
            cliente.Nome.Should().Be(nome);
            cliente.Sexo.Should().Be(sexo);
            cliente.Documento.Should().Be(documento);
            cliente.DataNascimento.Should().Be(dataNascimento);
            cliente.TipoCliente.Should().Be(tipoCliente);
            cliente.Endereco.Should().Be(endereco);
            cliente.Contato.Should().Be(contato);
            cliente.Veiculos.Should().BeSameAs(veiculos);
        }

        [Fact]
        public void Dado_ClienteComDados_Quando_ChamarAtualizar_Entao_DeveAtualizarPropriedades()
        {
            // Arrange
            var cliente = new Cliente
            {
                Nome = "Nome Antigo",
                Sexo = "Feminino",
                TipoCliente = TipoCliente.PessoaJuridico,
                DataNascimento = "1990-12-31"
            };

            var novoNome = "Novo Nome";
            var novoSexo = "Masculino";
            var novoTipoCliente = TipoCliente.PessoaFisica;
            var novaDataNascimento = "2020-01-01";

            // Act
            cliente.Atualizar(novoNome, novoSexo, novoTipoCliente, novaDataNascimento);

            // Assert
            cliente.Nome.Should().Be(novoNome);
            cliente.Sexo.Should().Be(novoSexo);
            cliente.TipoCliente.Should().Be(novoTipoCliente);
            cliente.DataNascimento.Should().Be(novaDataNascimento);
        }

        [Fact]
        public void Dado_ClienteComDados_Quando_ChamarAtualizarComValoresNulos_Entao_NaoDeveAtualizar()
        {
            // Arrange
            var nomeOriginal = "Nome Original";
            var sexoOriginal = "Feminino";
            var tipoClienteOriginal = TipoCliente.PessoaJuridico;
            var dataNascimentoOriginal = "1999-05-20";

            var cliente = new Cliente
            {
                Nome = nomeOriginal,
                Sexo = sexoOriginal,
                TipoCliente = tipoClienteOriginal,
                DataNascimento = dataNascimentoOriginal
            };

            // Act
            cliente.Atualizar(null, null, null, null);

            // Assert
            cliente.Nome.Should().Be(nomeOriginal);
            cliente.Sexo.Should().Be(sexoOriginal);
            cliente.TipoCliente.Should().Be(tipoClienteOriginal);
            cliente.DataNascimento.Should().Be(dataNascimentoOriginal);
        }
    }
}
