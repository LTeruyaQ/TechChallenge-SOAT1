using Xunit;
using FluentAssertions;
using Core.DTOs.Entidades.Cliente;
using Core.DTOs.Entidades.Autenticacao;
using Core.DTOs.Entidades.Veiculo;
using Core.Enumeradores;

namespace MecanicaOS.UnitTests.Core.DTOs.Entidades;

public class ClienteEntityDtoUnitTests
{
    [Fact]
    public void ClienteEntityDto_QuandoCriado_DeveHerdarDeRepositoryDto()
    {
        // Arrange & Act
        var dto = new ClienteEntityDto();

        // Assert
        dto.Should().BeAssignableTo<EntityDto>("ClienteEntityDto deve herdar de EntityDto");
        dto.Id.Should().Be(Guid.Empty, "Id deve ser vazio por padrão no DTO");
        dto.Ativo.Should().BeFalse("Ativo deve ser false por padrão no DTO");
    }

    [Fact]
    public void ClienteEntityDto_QuandoDefinidoCamposTecnicos_DevePreservarAuditoria()
    {
        // Arrange
        var dto = new ClienteEntityDto();
        var id = Guid.NewGuid();
        var dataCadastro = DateTime.Now;
        var dataAtualizacao = DateTime.Now.AddMinutes(5);

        // Act
        dto.Id = id;
        dto.DataCadastro = dataCadastro;
        dto.DataAtualizacao = dataAtualizacao;
        dto.Ativo = true;

        // Assert
        dto.Id.Should().Be(id, "o ID deve ser preservado corretamente");
        dto.DataCadastro.Should().Be(dataCadastro, "a data de cadastro deve ser preservada");
        dto.DataAtualizacao.Should().Be(dataAtualizacao, "a data de atualização deve ser preservada");
        dto.Ativo.Should().BeTrue("o status ativo deve ser preservado");
    }

    [Fact]
    public void ClienteEntityDto_QuandoDefinidoNome_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new ClienteEntityDto();
        var nomeEsperado = "João Silva";

        // Act
        dto.Nome = nomeEsperado;

        // Assert
        dto.Nome.Should().Be(nomeEsperado, "o nome deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void ClienteEntityDto_QuandoDefinidoDocumento_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new ClienteEntityDto();
        var documentoEsperado = "12345678901";

        // Act
        dto.Documento = documentoEsperado;

        // Assert
        dto.Documento.Should().Be(documentoEsperado, "o documento deve ser armazenado corretamente no DTO");
    }

    [Theory]
    [InlineData(TipoCliente.PessoaFisica)]
    [InlineData(TipoCliente.PessoaJuridico)]
    public void ClienteEntityDto_QuandoDefinidoTipoCliente_DeveArmazenarCorretamente(TipoCliente tipo)
    {
        // Arrange
        var dto = new ClienteEntityDto();

        // Act
        dto.TipoCliente = tipo;

        // Assert
        dto.TipoCliente.Should().Be(tipo, "o tipo de cliente deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void ClienteEntityDto_QuandoDefinidaDataNascimento_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new ClienteEntityDto();
        var dataNascimento = "1990-01-01";

        // Act
        dto.DataNascimento = dataNascimento;

        // Assert
        dto.DataNascimento.Should().Be(dataNascimento, "a data de nascimento deve ser armazenada corretamente no DTO");
    }

    [Theory]
    [InlineData("M")]
    [InlineData("F")]
    [InlineData(null)]
    public void ClienteEntityDto_QuandoDefinidoSexo_DeveArmazenarCorretamente(string? sexo)
    {
        // Arrange
        var dto = new ClienteEntityDto();

        // Act
        dto.Sexo = sexo;

        // Assert
        dto.Sexo.Should().Be(sexo, "o sexo deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void ClienteEntityDto_QuandoDefinidoEnderecoEContato_DeveArmazenarReferencias()
    {
        // Arrange
        var dto = new ClienteEntityDto();
        var endereco = new EnderecoEntityDto();
        var contato = new ContatoEntityDto();

        // Act
        dto.Endereco = endereco;
        dto.Contato = contato;

        // Assert
        dto.Endereco.Should().Be(endereco, "a referência do endereço deve ser armazenada corretamente");
        dto.Contato.Should().Be(contato, "a referência do contato deve ser armazenada corretamente");
    }

    [Fact]
    public void ClienteEntityDto_QuandoDefinidaListaVeiculos_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new ClienteEntityDto();
        var veiculos = new List<VeiculoEntityDto>
        {
            new VeiculoEntityDto { Placa = "ABC-1234" },
            new VeiculoEntityDto { Placa = "XYZ-5678" }
        };

        // Act
        dto.Veiculos = veiculos;

        // Assert
        dto.Veiculos.Should().HaveCount(2, "a lista de veículos deve conter 2 itens");
        dto.Veiculos.Should().Contain(v => v.Placa == "ABC-1234", "deve conter o primeiro veículo");
        dto.Veiculos.Should().Contain(v => v.Placa == "XYZ-5678", "deve conter o segundo veículo");
    }
}
