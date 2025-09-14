using Core.DTOs.UseCases.Cliente;
using Core.Enumeradores;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.UseCases;
using Xunit;

namespace MecanicaOS.UnitTests.Core.DTOs.UseCases;

public class ClienteUseCaseDtoUnitTests
{
    [Fact]
    public void CadastrarClienteUseCaseDto_DeveSerInicializadoComPropriedadesCorretas()
    {
        // Arrange & Act
        var dto = ClienteUseCaseFixture.CriarCadastrarClienteUseCaseDtoValido();

        // Assert
        dto.Should().NotBeNull("o DTO deve ser criado corretamente");
        dto.Nome.Should().Be("João Silva Santos", "o nome deve ser armazenado corretamente");
        dto.Sexo.Should().Be("M", "o sexo deve ser armazenado corretamente");
        dto.Documento.Should().Be("12345678901", "o documento deve ser armazenado corretamente");
        dto.DataNascimento.Should().Be("1985-03-15", "a data de nascimento deve ser armazenada corretamente");
        dto.TipoCliente.Should().Be(TipoCliente.PessoaFisica, "o tipo de cliente deve ser armazenado corretamente");
        dto.Email.Should().Be("joao.silva@email.com", "o email deve ser armazenado corretamente");
        dto.Telefone.Should().Be("(11) 99999-8888", "o telefone deve ser armazenado corretamente");
    }

    [Fact]
    public void CadastrarClienteUseCaseDto_DeveArmazenarEnderecoCorretamente()
    {
        // Arrange & Act
        var dto = ClienteUseCaseFixture.CriarCadastrarClienteUseCaseDtoValido();

        // Assert
        dto.Rua.Should().Be("Rua das Flores", "a rua deve ser armazenada corretamente");
        dto.Bairro.Should().Be("Centro", "o bairro deve ser armazenado corretamente");
        dto.Cidade.Should().Be("São Paulo", "a cidade deve ser armazenada corretamente");
        dto.Numero.Should().Be("123", "o número deve ser armazenado corretamente");
        dto.CEP.Should().Be("01234-567", "o CEP deve ser armazenado corretamente");
        dto.Complemento.Should().Be("Apto 45", "o complemento deve ser armazenado corretamente");
    }

    [Fact]
    public void CadastrarClienteUseCaseDto_DevePermitirPessoaJuridica()
    {
        // Arrange & Act
        var dto = ClienteUseCaseFixture.CriarCadastrarClienteUseCaseDtoPessoaJuridica();

        // Assert
        dto.Nome.Should().Be("Empresa XYZ Ltda", "deve aceitar nome de empresa");
        dto.TipoCliente.Should().Be(TipoCliente.PessoaJuridico, "deve ser pessoa jurídica");
        dto.Documento.Should().Be("12345678000195", "deve aceitar CNPJ");
        dto.Email.Should().Be("contato@empresaxyz.com.br", "deve aceitar email corporativo");
    }

    [Fact]
    public void CadastrarClienteUseCaseDto_DevePermitirComplementoNulo()
    {
        // Arrange & Act
        var dto = ClienteUseCaseFixture.CriarCadastrarClienteUseCaseDtoSemComplemento();

        // Assert
        dto.Complemento.Should().BeNull("deve permitir complemento nulo");
        dto.Nome.Should().NotBeNullOrEmpty("outros campos obrigatórios devem estar preenchidos");
        dto.Email.Should().NotBeNullOrEmpty("email deve estar preenchido");
    }

    [Theory]
    [InlineData(TipoCliente.PessoaFisica)]
    [InlineData(TipoCliente.PessoaJuridico)]
    public void CadastrarClienteUseCaseDto_DeveAceitarTodosTiposCliente(TipoCliente tipoCliente)
    {
        // Arrange & Act
        var dto = new CadastrarClienteUseCaseDto
        {
            Nome = "Teste",
            Sexo = "M",
            Documento = "12345678901",
            DataNascimento = "1990-01-01",
            TipoCliente = tipoCliente,
            Rua = "Rua Teste",
            Bairro = "Bairro Teste",
            Cidade = "Cidade Teste",
            Numero = "123",
            CEP = "12345-678",
            Email = "teste@email.com",
            Telefone = "(11) 99999-9999"
        };

        // Assert
        dto.TipoCliente.Should().Be(tipoCliente, "deve aceitar o tipo de cliente especificado");
    }

    [Fact]
    public void AtualizarClienteUseCaseDto_DeveSerInicializadoComPropriedadesCorretas()
    {
        // Arrange & Act
        var dto = ClienteUseCaseFixture.CriarAtualizarClienteUseCaseDtoValido();

        // Assert
        dto.Should().NotBeNull("o DTO deve ser criado corretamente");
        dto.Id.Should().NotBeEmpty("deve ter um ID válido");
        dto.Nome.Should().Be("João Silva Santos Atualizado", "o nome deve ser armazenado corretamente");
        dto.EnderecoId.Should().NotBeEmpty("deve ter um EnderecoId válido");
        dto.ContatoId.Should().NotBeEmpty("deve ter um ContatoId válido");
    }

    [Fact]
    public void AtualizarClienteUseCaseDto_DevePermitirCamposNulos()
    {
        // Arrange & Act
        var dto = ClienteUseCaseFixture.CriarAtualizarClienteUseCaseDtoComCamposNulos();

        // Assert
        dto.Nome.Should().BeNull("deve permitir nome nulo");
        dto.Sexo.Should().BeNull("deve permitir sexo nulo");
        dto.Documento.Should().BeNull("deve permitir documento nulo");
        dto.DataNascimento.Should().BeNull("deve permitir data de nascimento nula");
        dto.TipoCliente.Should().BeNull("deve permitir tipo de cliente nulo");
        dto.Rua.Should().BeNull("deve permitir rua nula");
        dto.Email.Should().BeNull("deve permitir email nulo");
        dto.Telefone.Should().BeNull("deve permitir telefone nulo");
    }

    [Fact]
    public void AtualizarClienteUseCaseDto_DevePermitirAtualizacaoParcial()
    {
        // Arrange & Act
        var dto = ClienteUseCaseFixture.CriarAtualizarClienteUseCaseDtoApenasNome();

        // Assert
        dto.Nome.Should().Be("Nome Atualizado", "deve permitir atualização apenas do nome");
        dto.Sexo.Should().BeNull("outros campos podem permanecer nulos");
        dto.Email.Should().BeNull("outros campos podem permanecer nulos");
    }

    [Fact]
    public void CadastrarClienteUseCaseDto_ListaFixture_DeveConterTodosOsItens()
    {
        // Arrange & Act
        var lista = ClienteUseCaseFixture.CriarListaCadastrarClienteUseCaseDto();

        // Assert
        lista.Should().NotBeNull("a lista deve ser criada");
        lista.Should().HaveCount(3, "deve conter todos os DTOs da fixture");
        lista.Should().OnlyContain(dto => !string.IsNullOrEmpty(dto.Nome), 
            "todos os DTOs devem ter nome");
        lista.Should().OnlyContain(dto => !string.IsNullOrEmpty(dto.Email), 
            "todos os DTOs devem ter email");
    }

    [Fact]
    public void AtualizarClienteUseCaseDto_ListaFixture_DeveConterTodosOsItens()
    {
        // Arrange & Act
        var lista = ClienteUseCaseFixture.CriarListaAtualizarClienteUseCaseDto();

        // Assert
        lista.Should().NotBeNull("a lista deve ser criada");
        lista.Should().HaveCount(3, "deve conter todos os DTOs da fixture");
        lista.Should().OnlyContain(dto => dto.Id != Guid.Empty, 
            "todos os DTOs devem ter ID válido");
        lista.Should().OnlyContain(dto => dto.EnderecoId != Guid.Empty, 
            "todos os DTOs devem ter EnderecoId válido");
        lista.Should().OnlyContain(dto => dto.ContatoId != Guid.Empty, 
            "todos os DTOs devem ter ContatoId válido");
    }

    [Fact]
    public void ClienteUseCaseDto_DevePermitirAlteracaoDePropriedades()
    {
        // Arrange
        var dto = ClienteUseCaseFixture.CriarCadastrarClienteUseCaseDtoValido();
        var novoNome = "Nome Alterado";
        var novoEmail = "novo@email.com";

        // Act
        dto.Nome = novoNome;
        dto.Email = novoEmail;

        // Assert
        dto.Nome.Should().Be(novoNome, "deve permitir alteração do nome");
        dto.Email.Should().Be(novoEmail, "deve permitir alteração do email");
    }
}
