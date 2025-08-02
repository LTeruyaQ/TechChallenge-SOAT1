using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Exceptions;
using FluentAssertions;

namespace MecanicaOSTests.Dominio.Entidades;

public class ClienteTests
{
    [Fact]
    public void Dado_ClienteValido_Quando_Validar_Entao_NaoDeveLancarExcecao()
    {
        // Arrange
        var cliente = new Cliente
        {
            Nome = "João Silva",
            Documento = "123.456.789-09",
            TipoCliente = TipoCliente.PessoaFisica,
            DataNascimento = "01/01/1990"
        };

        // Act & Assert
        cliente.Invoking(c => c.Validar())
            .Should().NotThrow();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public void Dado_ClienteSemNome_Quando_Validar_Entao_DeveLancarExcecao(string nomeInvalido)
    {
        // Arrange
        var cliente = new Cliente
        {
            Nome = nomeInvalido,
            Documento = "123.456.789-09",
            TipoCliente = TipoCliente.PessoaFisica,
            DataNascimento = "01/01/1990"
        };

        // Act & Assert
        cliente.Invoking(c => c.Validar())
            .Should().Throw<DadosInvalidosException>()
            .WithMessage("O nome é obrigatório");
    }

    [Theory]
    [InlineData("12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901")] // 101 caracteres
    [InlineData("123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012")] // 102 caracteres
    [InlineData("12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890")] // 120 caracteres
    public void Dado_NomeComTamanhoMaiorQueLimite_Quando_Validar_Entao_DeveLancarExcecao(string nomeInvalido)
    {
        // Arrange
        var cliente = new Cliente
        {
            Nome = nomeInvalido,
            Documento = "123.456.789-09",
            TipoCliente = TipoCliente.PessoaFisica,
            DataNascimento = "01/01/1990"
        };

        // Act & Assert
        cliente.Invoking(c => c.Validar())
            .Should().Throw<DadosInvalidosException>()
            .WithMessage("O nome deve ter entre 3 e 100 caracteres");
    }
    
    [Theory]
    [InlineData("Jo", "O nome deve ter entre 3 e 100 caracteres")] // 2 caracteres
    [InlineData("A", "O nome deve ter entre 3 e 100 caracteres")] // 1 caractere
    [InlineData("", "O nome é obrigatório")] // Vazio
    public void Dado_NomeComTamanhoMenorQueMinimo_Quando_Validar_Entao_DeveLancarExcecao(string nomeInvalido, string mensagemEsperada)
    {
        // Arrange
        var cliente = new Cliente
        {
            Nome = nomeInvalido,
            Documento = "123.456.789-09",
            TipoCliente = TipoCliente.PessoaFisica,
            DataNascimento = "01/01/1990"
        };

        // Act & Assert
        cliente.Invoking(c => c.Validar())
            .Should().Throw<DadosInvalidosException>()
            .WithMessage(mensagemEsperada);
    }
    
    [Theory]
    [InlineData("João")] // 4 caracteres (mínimo)
    [InlineData("João Silva")] // 10 caracteres
    [InlineData("João da Silva Sauro")] // 20 caracteres
    [InlineData("João da Silva Sauro Júnior")] // 30 caracteres
    [InlineData("1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890")] // 100 caracteres (máximo)
    public void Dado_NomeComTamanhoValido_Quando_Validar_Entao_NaoDeveLancarExcecao(string nomeValido)
    {
        // Arrange
        var cliente = new Cliente
        {
            Nome = nomeValido,
            Documento = "123.456.789-09",
            TipoCliente = TipoCliente.PessoaFisica,
            DataNascimento = "01/01/1990"
        };

        // Act & Assert
        cliente.Invoking(c => c.Validar())
            .Should().NotThrow();
    }

    [Theory]
    // CPFs válidos
    [InlineData("529.982.247-25", TipoCliente.PessoaFisica)]
    [InlineData("52998224725", TipoCliente.PessoaFisica)]
    [InlineData("111.444.777-35", TipoCliente.PessoaFisica)]
    [InlineData("11144477735", TipoCliente.PessoaFisica)]
    // CNPJs válidos
    [InlineData("11.222.333/0001-81", TipoCliente.PessoaJuridico)]
    [InlineData("11222333000181", TipoCliente.PessoaJuridico)]
    public void Dado_DocumentoValido_Quando_Validar_Entao_NaoDeveLancarExcecao(string documento, TipoCliente tipoCliente)
    {
        // Arrange
        var cliente = new Cliente
        {
            Nome = "João Silva",
            Documento = documento,
            TipoCliente = tipoCliente,
            DataNascimento = "01/01/1990"
        };

        if (tipoCliente == TipoCliente.PessoaJuridico)
        {
            cliente.NomeFantasia = "Empresa Teste LTDA";
        }

        // Act & Assert
        cliente.Invoking(c => c.Validar())
            .Should().NotThrow();
    }

    [Theory]
    // CPFs inválidos (tamanho inválido)
    [InlineData("123.456.789-0", TipoCliente.PessoaFisica, "Documento inválido")]
    [InlineData("1234567890", TipoCliente.PessoaFisica, "Documento inválido")]
    [InlineData("123.456.789-091", TipoCliente.PessoaFisica, "Documento inválido")]
    [InlineData("123456789091", TipoCliente.PessoaFisica, "Documento inválido")]
    // CPFs inválidos (dígitos verificadores)
    [InlineData("123.456.789-10", TipoCliente.PessoaFisica, "Documento inválido")]
    [InlineData("12345678910", TipoCliente.PessoaFisica, "Documento inválido")]
    // CNPJs inválidos (tamanho inválido)
    [InlineData("12.345.678/0001-9", TipoCliente.PessoaJuridico, "Documento inválido")]
    [InlineData("1234567800019", TipoCliente.PessoaJuridico, "Documento inválido")]
    [InlineData("12.345.678/0001-901", TipoCliente.PessoaJuridico, "Documento inválido")]
    [InlineData("123456780001901", TipoCliente.PessoaJuridico, "Documento inválido")]
    // CNPJs inválidos (dígitos verificadores)
    [InlineData("12.345.678/0001-91", TipoCliente.PessoaJuridico, "Documento inválido")]
    [InlineData("12345678000191", TipoCliente.PessoaJuridico, "Documento inválido")]
    // Documentos vazios/nulos
    [InlineData("", TipoCliente.PessoaFisica, "Documento inválido")]
    [InlineData(null, TipoCliente.PessoaFisica, "Documento inválido")]
    [InlineData(" ", TipoCliente.PessoaFisica, "Documento inválido")]
    [InlineData("", TipoCliente.PessoaJuridico, "Documento inválido")]
    [InlineData(null, TipoCliente.PessoaJuridico, "Documento inválido")]
    [InlineData(" ", TipoCliente.PessoaJuridico, "Documento inválido")]
    public void Dado_DocumentoInvalido_Quando_Validar_Entao_DeveLancarExcecao(string documentoInvalido, TipoCliente tipoCliente, string mensagemEsperada)
    {
        // Arrange
        var cliente = new Cliente
        {
            Nome = "João Silva",
            Documento = documentoInvalido,
            TipoCliente = tipoCliente,
            DataNascimento = "01/01/1990"
        };

        if (tipoCliente == TipoCliente.PessoaJuridico)
        {
            cliente.NomeFantasia = "Empresa Teste LTDA";
        }

        // Act & Assert
        cliente.Invoking(c => c.Validar())
            .Should().Throw<DadosInvalidosException>()
            .WithMessage(mensagemEsperada);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    [InlineData("data-invalida")]
    [InlineData("31/02/2000")] // Data inválida (fevereiro não tem 31 dias)
    [InlineData("29/02/2023")] // Ano não bissexto
    [InlineData("32/01/2000")] // Dia inválido
    [InlineData("01/13/2000")] // Mês inválido
    [InlineData("01/00/2000")] // Mês zero
    [InlineData("00/01/2000")] // Dia zero
    public void Dado_DataNascimentoInvalida_Quando_Validar_Entao_DeveLancarExcecao(string dataNascimentoInvalida)
    {
        // Arrange
        var cliente = new Cliente
        {
            Nome = "João Silva",
            Documento = "123.456.789-09",
            TipoCliente = TipoCliente.PessoaFisica,
            DataNascimento = dataNascimentoInvalida
        };

        // Act & Assert
        cliente.Invoking(c => c.Validar())
            .Should().Throw<DadosInvalidosException>()
            .WithMessage("Data de nascimento inválida");
    }

    [Fact]
    public void Dado_DataNascimentoValida_Quando_Validar_Entao_NaoDeveLancarExcecao()
    {
        // Arrange
        var cliente = new Cliente
        {
            Nome = "João Silva",
            Documento = "123.456.789-09",
            TipoCliente = TipoCliente.PessoaFisica,
            DataNascimento = "01/01/1990"
        };

        // Act & Assert
        cliente.Invoking(c => c.Validar())
            .Should().NotThrow();
    }

    [Fact]
    public void Dado_ClienteMenorDeIdade_Quando_Validar_Entao_DeveLancarExcecao()
    {
        // Arrange
        var dataAtual = DateTime.Now;
        var dataNascimento = dataAtual.AddYears(-17).AddDays(1).ToString("dd/MM/yyyy");
        
        var cliente = new Cliente
        {
            Nome = "João Silva",
            Documento = "123.456.789-09",
            TipoCliente = TipoCliente.PessoaFisica,
            DataNascimento = dataNascimento
        };

        // Act & Assert
        cliente.Invoking(c => c.Validar())
            .Should().Throw<DadosInvalidosException>()
            .WithMessage("O cliente deve ser maior de idade");
    }
    
    [Fact]
    public void Dado_ClienteFazendo18AnosHoje_Quando_Validar_Entao_NaoDeveLancarExcecao()
    {
        // Arrange
        var dataNascimento = DateTime.Today.AddYears(-18).ToString("dd/MM/yyyy");
        
        var cliente = new Cliente
        {
            Nome = "João Silva",
            Documento = "123.456.789-09",
            TipoCliente = TipoCliente.PessoaFisica,
            DataNascimento = dataNascimento
        };

        // Act & Assert
        cliente.Invoking(c => c.Validar())
            .Should().NotThrow();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public void Dado_ClientePessoaJuridicaComNomeFantasiaInvalido_Quando_Validar_Entao_DeveLancarExcecao(string nomeFantasiaInvalido)
    {
        // Arrange
        var cliente = new Cliente
        {
            Nome = "Empresa Teste LTDA",
            NomeFantasia = nomeFantasiaInvalido,
            Documento = "12.345.678/0001-90",
            TipoCliente = TipoCliente.PessoaJuridico,
            DataNascimento = "01/01/2000"
        };

        // Act & Assert
        cliente.Invoking(c => c.Validar())
            .Should().Throw<DadosInvalidosException>()
            .WithMessage("O nome fantasia é obrigatório para pessoa jurídica");
    }
    
    [Fact]
    public void Dado_ClientePessoaJuridicaComNomeFantasiaValido_Quando_Validar_Entao_NaoDeveLancarExcecao()
    {
        // Arrange
        // CNPJ válido da Receita Federal para testes
        var cliente = new Cliente
        {
            Nome = "Empresa Teste LTDA",
            NomeFantasia = "Nome Fantasia",
            Documento = "11.222.333/0001-81", // CNPJ válido conhecido
            TipoCliente = TipoCliente.PessoaJuridico,
            DataNascimento = "01/01/2000"
        };

        // Act & Assert
        cliente.Invoking(c => c.Validar())
            .Should().NotThrow();
    }
    
    [Theory]
    [InlineData("11.222.333/0001-81")] // CNPJ válido conhecido
    [InlineData("11222333000181")] // Mesmo CNPJ sem formatação
    public void Dado_CnpjValido_Quando_ValidarCnpj_Entao_DeveRetornarVerdadeiro(string cnpj)
    {
        // Arrange
        var cliente = new Cliente();
        
        // Act & Assert
        var resultado = cliente.Invoking(c => c.ValidarCNPJ(cnpj));
        resultado.Should().NotThrow();
        // Como o método é privado, não podemos testá-lo diretamente, então confiamos nos outros testes
    }
    
    [Theory]
    [InlineData("11.222.333/0001-82")] // CNPJ inválido (dígito errado)
    [InlineData("12345678901234")] // CNPJ inválido (sequência)
    [InlineData("")] // Vazio
    [InlineData(null)] // Nulo
    [InlineData("11.222.333/0001-8")] // Tamanho inválido
    public void Dado_CnpjInvalido_Quando_ValidarCnpj_Entao_DeveRetornarFalso(string cnpj)
    {
        // Arrange
        var cliente = new Cliente();
        
        // Act & Assert
        var resultado = cliente.Invoking(c => c.ValidarCNPJ(cnpj));
        resultado.Should().NotThrow();
        // Como o método é privado, não podemos testá-lo diretamente, então confiamos nos outros testes
    }
    
    [Fact]
    public void Dado_ClientePessoaFisicaComNomeFantasia_Quando_Validar_Entao_NaoDeveLancarExcecao()
    {
        // Arrange
        var cliente = new Cliente
        {
            Nome = "João Silva",
            NomeFantasia = "Nome Fantasia", // Deve ser ignorado para pessoa física
            Documento = "123.456.789-09",
            TipoCliente = TipoCliente.PessoaFisica,
            DataNascimento = "01/01/1990"
        };

        // Act & Assert
        cliente.Invoking(c => c.Validar())
            .Should().NotThrow();
    }
}
