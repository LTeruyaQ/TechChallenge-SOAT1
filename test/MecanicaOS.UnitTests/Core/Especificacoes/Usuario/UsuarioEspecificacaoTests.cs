using Core.DTOs.Entidades.Usuarios;
using Core.Enumeradores;
using Core.Especificacoes.Usuario;
using MecanicaOS.UnitTests.Fixtures;

namespace MecanicaOS.UnitTests.Core.Especificacoes.Usuario;

public class UsuarioEspecificacaoTests
{
    private List<UsuarioEntityDto> GetUsuariosDeTeste()
    {
        var usuario1 = UsuarioFixture.CriarUsuarioEntityDtoValido();
        usuario1.Email = "admin@mecanicaos.com";
        usuario1.TipoUsuario = TipoUsuario.Admin;
        usuario1.RecebeAlertaEstoque = true;
        usuario1.Id = Guid.NewGuid();

        var usuario2 = UsuarioFixture.CriarUsuarioEntityDtoValido();
        usuario2.Email = "funcionario@mecanicaos.com";
        usuario2.TipoUsuario = TipoUsuario.Cliente;
        usuario2.RecebeAlertaEstoque = false;
        usuario2.Id = Guid.NewGuid();

        var usuario3 = UsuarioFixture.CriarUsuarioEntityDtoValido();
        usuario3.Email = "gerente@mecanicaos.com";
        usuario3.TipoUsuario = TipoUsuario.Admin;
        usuario3.RecebeAlertaEstoque = true;
        usuario3.Id = Guid.NewGuid();

        var usuario4 = UsuarioFixture.CriarUsuarioEntityDtoValido();
        usuario4.Email = "inativo@mecanicaos.com";
        usuario4.Ativo = false;
        usuario4.RecebeAlertaEstoque = false;
        usuario4.Id = Guid.NewGuid();

        return new List<UsuarioEntityDto> { usuario1, usuario2, usuario3, usuario4 };
    }

    [Fact]
    public void ObterUsuarioPorEmailEspecificacao_DeveRetornarUsuarioCorreto()
    {
        // Arrange
        var usuarios = GetUsuariosDeTeste();
        var email = "admin@mecanicaos.com";
        var especificacao = new ObterUsuarioPorEmailEspecificacao(email);

        // Act
        var resultado = usuarios.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(1, "deve retornar apenas o usuário com o email especificado");
        resultado.First().Email.Should().Be(email, "deve retornar o usuário com email correto");
    }

    [Fact]
    public void ObterUsuarioPorEmailEspecificacao_QuandoEmailNaoExiste_DeveRetornarListaVazia()
    {
        // Arrange
        var usuarios = GetUsuariosDeTeste();
        var emailInexistente = "inexistente@mecanicaos.com";
        var especificacao = new ObterUsuarioPorEmailEspecificacao(emailInexistente);

        // Act
        var resultado = usuarios.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().BeEmpty("não deve retornar nenhum usuário quando email não existe");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ObterUsuarioPorEmailEspecificacao_ComEmailInvalido_DeveRetornarListaVazia(string emailInvalido)
    {
        // Arrange
        var usuarios = GetUsuariosDeTeste();
        var especificacao = new ObterUsuarioPorEmailEspecificacao(emailInvalido);

        // Act
        var resultado = usuarios.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().BeEmpty("não deve retornar nenhum usuário quando email é inválido");
    }

    [Fact]
    public void ObterUsuarioPorEmailEspecificacao_ComBuscaCaseSensitive_DeveFuncionarCorretamente()
    {
        // Arrange
        var usuarios = GetUsuariosDeTeste();
        var especificacao1 = new ObterUsuarioPorEmailEspecificacao("admin@mecanicaos.com");
        var especificacao2 = new ObterUsuarioPorEmailEspecificacao("ADMIN@MECANICAOS.COM");

        // Act
        var resultado1 = usuarios.Where(especificacao1.Expressao.Compile()).ToList();
        var resultado2 = usuarios.Where(especificacao2.Expressao.Compile()).ToList();

        // Assert
        resultado1.Should().HaveCount(1, "deve encontrar com email exato");
        resultado2.Should().BeEmpty("não deve encontrar com case diferente");
    }

    [Fact]
    public void ObterUsuarioParaAlertaEstoqueEspecificacao_DeveRetornarUsuariosAdministradores()
    {
        // Arrange
        var usuarios = GetUsuariosDeTeste();
        var especificacao = new ObterUsuarioParaAlertaEstoqueEspecificacao();

        // Act
        var resultado = usuarios.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(2, "deve retornar apenas usuários administradores ativos");
        resultado.Should().OnlyContain(u => u.TipoUsuario == TipoUsuario.Admin,
            "todos os usuários devem ser administradores");
        resultado.Should().OnlyContain(u => u.Ativo,
            "todos os usuários devem estar ativos");
    }

    [Fact]
    public void ObterUsuarioParaAlertaEstoqueEspecificacao_ComUsuariosQueRecebemAlerta_DeveRetornarCorretos()
    {
        // Arrange
        var especificacao = new ObterUsuarioParaAlertaEstoqueEspecificacao();
        var usuarios = GetUsuariosDeTeste();

        // Act
        var resultado = usuarios.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(2, "deve retornar apenas usuários que recebem alerta de estoque");
        resultado.Should().OnlyContain(u => u.RecebeAlertaEstoque,
            "todos os usuários devem ter RecebeAlertaEstoque = true");
    }

    [Fact]
    public void ObterUsuarioParaAlertaEstoqueEspecificacao_ComUsuariosQueNaoRecebemAlerta_DeveRetornarListaVazia()
    {
        // Arrange
        var usuarios = new List<UsuarioEntityDto>
        {
            UsuarioFixture.CriarUsuarioEntityDtoValido(),
            UsuarioFixture.CriarUsuarioEntityDtoValido()
        };

        usuarios.ForEach(u => u.RecebeAlertaEstoque = false);
        var especificacao = new ObterUsuarioParaAlertaEstoqueEspecificacao();

        // Act
        var resultado = usuarios.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().BeEmpty("não deve retornar usuários que não recebem alerta de estoque");
    }

    [Fact]
    public void ObterUsuarioParaAlertaEstoqueEspecificacao_ComUsuariosComDiferentesConfiguracoes_DeveRetornarApenasQueRecebemAlerta()
    {
        // Arrange
        var usuarios = new List<UsuarioEntityDto>
        {
            UsuarioFixture.CriarUsuarioEntityDtoValido(),
            UsuarioFixture.CriarUsuarioEntityDtoValido()
        };

        usuarios[0].RecebeAlertaEstoque = true;
        usuarios[0].Ativo = true;

        usuarios[1].RecebeAlertaEstoque = false;
        usuarios[1].Ativo = false;

        var especificacao = new ObterUsuarioParaAlertaEstoqueEspecificacao();

        // Act
        var resultado = usuarios.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(1, "deve retornar apenas usuários que recebem alerta de estoque");
        resultado.First().RecebeAlertaEstoque.Should().BeTrue("usuário deve receber alerta de estoque");
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void ObterUsuarioParaAlertaEstoqueEspecificacao_ComDiferentesConfiguracoes_DeveFiltrarCorretamente(
        bool recebeAlertaEstoque, bool deveRetornar)
    {
        // Arrange
        var usuario = UsuarioFixture.CriarUsuarioEntityDtoValido();
        usuario.RecebeAlertaEstoque = recebeAlertaEstoque;

        var usuarios = new List<UsuarioEntityDto> { usuario };
        var especificacao = new ObterUsuarioParaAlertaEstoqueEspecificacao();

        // Act
        var resultado = usuarios.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        if (deveRetornar)
        {
            resultado.Should().HaveCount(1, $"deve retornar usuário que {(recebeAlertaEstoque ? "recebe" : "não recebe")} alerta de estoque");
            resultado.First().RecebeAlertaEstoque.Should().Be(recebeAlertaEstoque);
        }
        else
        {
            resultado.Should().BeEmpty($"não deve retornar usuário que {(recebeAlertaEstoque ? "recebe" : "não recebe")} alerta de estoque");
        }
    }

    [Fact]
    public void ObterUsuarioPorEmailEspecificacao_ComEmailsComEspacos_DeveFuncionarCorretamente()
    {
        // Arrange
        var usuarios = new List<UsuarioEntityDto>
        {
            UsuarioFixture.CriarUsuarioEntityDtoValido(),
            UsuarioFixture.CriarUsuarioEntityDtoValido()
        };

        usuarios[0].Email = "teste@email.com";
        usuarios[1].Email = " teste@email.com ";

        var especificacao = new ObterUsuarioPorEmailEspecificacao("teste@email.com");

        // Act
        var resultado = usuarios.Where(especificacao.Expressao.Compile()).ToList();

        // Assert
        resultado.Should().HaveCount(1, "deve encontrar apenas o email exato, sem espaços extras");
        resultado.First().Email.Should().Be("teste@email.com");
    }
}
