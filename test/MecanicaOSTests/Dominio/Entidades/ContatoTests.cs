using Dominio.Entidades;
using FluentAssertions;

namespace MecanicaOSTests.Dominio.Entidades;

public class ContatoTests
{
    [Theory]
    // Formatos básicos
    [InlineData("cliente@exemplo.com", "(11) 99999-9999")]
    [InlineData("cliente.nome@exemplo.com.br", "(11) 98765-4321")]
    [InlineData("nome.sobrenome@dominio.com", "(11) 1234-5678")]
    
    // Com subdomínios
    [InlineData("usuario@sub.dominio.com", "(11) 91234-5678")]
    [InlineData("usuario@sub.sub2.dominio.com.br", "(11) 91111-1111")]
    
    // Com caracteres especiais permitidos no nome de usuário
    [InlineData("usuario+tag@exemplo.com", "(11) 92222-2222")]
    [InlineData("usuario-teste@exemplo.com", "(11) 93333-3333")]
    [InlineData("usuario_teste@exemplo.com", "(11) 94444-4444")]
    [InlineData("usuario123@exemplo.com", "(11) 95555-5555")]
    [InlineData("usuario.teste@exemplo.com", "(11) 96666-6666")]
    
    // Com domínios incomuns mas válidos
    [InlineData("usuario@dominio-hiphen.com", "(11) 97777-7777")]
    [InlineData("usuario@dominio.co.uk", "(11) 98888-8888")]
    [InlineData("usuario@dominio.io", "(11) 99999-9999")]
    [InlineData("usuario@dominio.name", "(11) 91111-1111")]
    
    // Com TLDs longos
    [InlineData("usuario@exemplo.photography", "(11) 92222-2222")]
    [InlineData("usuario@exemplo.international", "(11) 93333-3333")]
    
    // Com nomes de usuário longos
    [InlineData("nome.muito.longo.de.usuario@exemplo.com", "(11) 94444-4444")]
    
    // Com domínios longos
    [InlineData("usuario@subdominio.muitos.niveis.dominio.com.br", "(11) 95555-5555")]
    
    // Com números no domínio
    [InlineData("usuario@dominio123.com", "(11) 96666-6666")]
    [InlineData("usuario@123dominio.com", "(11) 97777-7777")]
    
    // Com letras maiúsculas e minúsculas (deve ser case-insensitive)
    [InlineData("Usuario@Exemplo.com", "(11) 98888-8888")]
    [InlineData("USUARIO@EXEMPLO.COM", "(11) 99999-9999")]
    
    // Com tags
    [InlineData("usuario+tag@exemplo.com", "(11) 91111-1111")]
    [InlineData("usuario+outratag@exemplo.com", "(11) 92222-2222")]
    
    // Com domínios de nível superior com hífen
    [InlineData("usuario@exemplo.co-uk", "(11) 93333-3333")]
    
    // Com domínios de nível superior longos
    [InlineData("usuario@exemplo.xn--vermgensberatung-pwb", "(11) 94444-4444")] // IDN
    
    // Com domínios de nível superior genéricos
    [InlineData("usuario@exemplo.technology", "(11) 95555-5555")]
    [InlineData("usuario@exemplo.engineering", "(11) 96666-6666")]
    public void Dado_ContatoValido_Quando_Validar_Entao_NaoDeveLancarExcecao(string email, string telefone)
    {
        // Arrange
        var contato = new Contato
        {
            Email = email,
            Telefone = telefone
        };

        // Act
        var action = () => contato.Validar();

        // Assert
        action.Should().NotThrow();
        
        // Verifica se o e-mail foi mantido inalterado (sem normalização)
        contato.Email.Should().Be(email, "o e-mail não deve ser modificado durante a validação");
    }

    [Theory]
    // Casos de e-mail vazio/nulo
    [InlineData("", "O e-mail é obrigatório")]
    [InlineData(null, "O e-mail é obrigatório")]
    [InlineData("   ", "O e-mail é obrigatório")]
    
    // Casos de formato inválido
    [InlineData("semarroba", "E-mail inválido")]
    [InlineData("@semusuario.com", "E-mail inválido")]
    [InlineData("usuario@", "E-mail inválido")]
    [InlineData("usuario@dominio", "E-mail inválido")]
    [InlineData("usuario@dominio.", "E-mail inválido")]
    [InlineData("usuario@.com", "E-mail inválido")]
    [InlineData("usuario@dominio..com", "E-mail inválido")]
    [InlineData("usuario@dominio_com", "E-mail inválido")]
    [InlineData("usuário@exemplo.com", "E-mail inválido")] // Caractere acentuado
    [InlineData("usuario@exemplo.com.", "E-mail inválido")] // Ponto no final
    [InlineData("usuario@-exemplo.com", "E-mail inválido")] // Hífen no início do domínio
    [InlineData("usuario@exemplo-.com", "E-mail inválido")] // Hífen no final do domínio
    [InlineData("usuario@exemplo.c", "E-mail inválido")]    // TLD muito curto
    [InlineData("usuario@exemplo.123", "E-mail inválido")]  // TLD apenas com números
    [InlineData("usuario@exemplo.c1", "E-mail inválido")]   // TLD com número
    [InlineData("usuario@exemplo.c-o-m", "E-mail inválido")] // Hífen no TLD
    [InlineData("usuario@exemplo..com", "E-mail inválido")]  // Dois pontos consecutivos
    [InlineData("usuario@.exemplo.com", "E-mail inválido")]  // Domínio vazio
    [InlineData("usuario@exemplo..com", "E-mail inválido")]  // Dois pontos no domínio
    [InlineData("usuario@exemplo.com.", "E-mail inválido")]  // Ponto no final
    [InlineData("usuario@.exemplo.com", "E-mail inválido")]  // Ponto no início do domínio
    [InlineData("usuario@exemplo..com", "E-mail inválido")]  // Dois pontos consecutivos no domínio
    [InlineData("usuario@exemplo.com/" , "E-mail inválido")] // Caractere inválido no final
    [InlineData("usuario@exemplo.com,br", "E-mail inválido")] // Vírgula no domínio
    [InlineData("usuario@exemplo,com", "E-mail inválido")]   // Vírgula no domínio
    [InlineData("usuario@exemplo/com", "E-mail inválido")]   // Barra no domínio
    [InlineData("usuario@exemplo\\com", "E-mail inválido")]  // Barra invertida no domínio
    [InlineData("usuario@exemplo com", "E-mail inválido")]   // Espaço no domínio
    [InlineData("usuario @exemplo.com", "E-mail inválido")]  // Espaço no usuário
    [InlineData("usuario@ exemplo.com", "E-mail inválido")]  // Espaço após @
    [InlineData("usuario@exemplo .com", "E-mail inválido")]  // Espaço antes do .com
    [InlineData("usuario@exemplo. com", "E-mail inválido")]  // Espaço após o ponto
    [InlineData("usuario@exemplo . com", "E-mail inválido")] // Espaços ao redor do ponto
    [InlineData("usuario@exemplo..com", "E-mail inválido")]  // Dois pontos consecutivos
    [InlineData("usuario@exemplo.com.", "E-mail inválido")]  // Ponto no final
    [InlineData("usuario@exemplo.com ", "E-mail inválido")]  // Espaço no final
    [InlineData(" usuario@exemplo.com", "E-mail inválido")]  // Espaço no início
    [InlineData("\tusuario@exemplo.com", "E-mail inválido")] // Tab no início
    [InlineData("usuario@exemplo.com\t", "E-mail inválido")] // Tab no final
    [InlineData("usuario@exemplo.com\n", "E-mail inválido")]  // Quebra de linha no final
    [InlineData("\nusuario@exemplo.com", "E-mail inválido")] // Quebra de linha no início
    public void Dado_EmailInvalido_Quando_Validar_Entao_DeveLancarExcecao(string emailInvalido, string mensagemEsperada)
    {
        // Arrange
        var contato = new Contato
        {
            Email = emailInvalido,
            Telefone = "(11) 99999-9999"
        };

        // Act & Assert
        contato.Invoking(c => c.Validar())
            .Should().Throw<ArgumentException>()
            .WithMessage(mensagemEsperada);
    }

    [Theory]
    [InlineData("", "O telefone é obrigatório")]
    [InlineData(null, "O telefone é obrigatório")]
    [InlineData("(11) 99999-999", "Telefone inválido")]
    [InlineData("(11) 9999-9999", "Telefone inválido")]
    public void Dado_TelefoneInvalido_Quando_Validar_Entao_DeveLancarExcecao(string telefoneInvalido, string mensagemEsperada)
    {
        // Arrange
        var contato = new Contato
        {
            Email = "cliente@exemplo.com",
            Telefone = telefoneInvalido
        };

        // Act
        var action = () => contato.Validar();

        // Assert
        if (string.IsNullOrEmpty(telefoneInvalido))
        {
            action.Should().Throw<ArgumentException>()
                .WithMessage("O telefone é obrigatório");
        }
        else
        {
            // Remove caracteres não numéricos
            var numeros = new string(telefoneInvalido.Where(char.IsDigit).ToArray());
            
            // Remove o zero inicial se existir
            if (numeros.StartsWith("0"))
                numeros = numeros[1..];
                
            if (numeros.Length < 10 || numeros.Length > 11)
            {
                action.Should().Throw<ArgumentException>()
                    .WithMessage("Telefone inválido");
            }
            else
            {
                action.Should().NotThrow();
            }
        }
    }
    
    [Theory]
    [InlineData("(11) 99999-9999")] // Formato com 9 dígitos
    [InlineData("(11) 9999-9999")]  // Formato com 8 dígitos
    [InlineData("11999999999")]     // Apenas números com 11 dígitos
    [InlineData("1199999999")]      // Apenas números com 10 dígitos
    [InlineData("011999999999")]    // Com DDI
    public void Dado_TelefoneValido_Quando_Validar_Entao_NaoDeveLancarExcecao(string telefoneValido)
    {
        // Arrange
        var contato = new Contato
        {
            Email = "cliente@exemplo.com",
            Telefone = telefoneValido
        };

        // Act
        var action = () => contato.Validar();

        // Assert
        action.Should().NotThrow();
    }

    [Fact]
    public void Dado_ContatoSemEmailETelefone_Quando_Validar_Entao_DeveLancarExcecao()
    {
        // Arrange
        var contato = new Contato
        {
            Email = "",
            Telefone = ""
        };

        // Act & Assert
        contato.Invoking(c => c.Validar())
            .Should().Throw<ArgumentException>()
            .WithMessage("O e-mail é obrigatório");
    }
    
    [Fact]
    public void Dado_ContatoApenasComTelefone_Quando_Validar_Entao_DeveLancarExcecao()
    {
        // Arrange
        var contato = new Contato
        {
            Email = "",
            Telefone = "(11) 99999-9999"
        };

        // Act & Assert
        contato.Invoking(c => c.Validar())
            .Should().Throw<ArgumentException>()
            .WithMessage("O e-mail é obrigatório");
    }
    
    [Fact]
    public void Dado_ContatoApenasComEmail_Quando_Validar_Entao_DeveLancarExcecao()
    {
        // Arrange
        var contato = new Contato
        {
            Email = "cliente@exemplo.com",
            Telefone = ""
        };

        // Act & Assert
        contato.Invoking(c => c.Validar())
            .Should().Throw<ArgumentException>()
            .WithMessage("O telefone é obrigatório");
    }

    [Theory]
    [InlineData("11999999999", "11999999999")]                 // Celular já formatado
    [InlineData("(11) 99999-9999", "11999999999")]            // Celular com formatação
    [InlineData("11 99999-9999", "11999999999")]              // Celular com espaço e hífen
    [InlineData("11 999999999", "11999999999")]                // Celular apenas com espaço
    [InlineData("(11) 9999-9999", "1199999999")]              // Telefone fixo com formatação
    [InlineData("11 9999-9999", "1199999999")]                 // Telefone fixo com espaço e hífen
    [InlineData("11 99999999", "1199999999")]                  // Telefone fixo apenas com espaço
    [InlineData("011999999999", "11999999999")]                // Com DDI
    [InlineData("0011999999999", "11999999999")]               // Com DDI e código do país
    [InlineData("+55 (11) 99999-9999", "5511999999999")]       // Com código do país e formatação
    [InlineData("55 (11) 99999-9999", "5511999999999")]         // Com código do país sem +
    [InlineData("55 11 99999-9999", "5511999999999")]           // Com código do país sem parênteses
    [InlineData("55 11 999999999", "5511999999999")]            // Com código do país sem formatação
    [InlineData("55 (11) 9999-9999", "551199999999")]           // Fixo com código do país
    public void Dado_TelefoneValido_Quando_FormatarTelefone_Entao_DeveRetornarApenasNumeros(string telefone, string esperado)
    {
        // Arrange
        var contato = new Contato();

        // Act
        var resultado = contato.FormatarTelefone(telefone);

        // Assert
        resultado.Should().Be(esperado);
        
        // Verifica se o resultado contém apenas dígitos
        resultado.Should().MatchRegex("^\\d+$", "o telefone formatado deve conter apenas dígitos");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Dado_TelefoneInvalido_Quando_FormatarTelefone_Entao_DeveRetornarStringVazia(string telefone)
    {
        // Arrange
        var contato = new Contato();

        // Act
        var resultado = contato.FormatarTelefone(telefone);

        // Assert
        resultado.Should().BeEmpty();
    }
    
    [Fact]
    public void Dado_TelefoneComCaracteresInvalidos_Quando_FormatarTelefone_Entao_DeveRetornarApenasNumeros()
    {
        // Arrange
        var contato = new Contato();
        var telefone = "(11) 98765-4321 - Ramal 123";
        var esperado = "11987654321";

        // Act
        var resultado = contato.FormatarTelefone(telefone);

        // Assert
        resultado.Should().Be(esperado);
    }
}
