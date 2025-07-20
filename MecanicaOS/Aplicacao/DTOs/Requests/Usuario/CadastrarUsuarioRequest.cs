using Dominio.Enumeradores;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Aplicacao.DTOs.Requests.Usuario;

[DisplayName("Cadastro de Usuário")]
[SwaggerSchema(Title = "Cadastro de Usuário", Description = "Dados necessários para cadastrar um novo usuário")]
public class CadastrarUsuarioRequest
{
    [Required(ErrorMessage = "O campo E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "O Email informado não é válido")]
    [StringLength(240, ErrorMessage = "O Email deve ter no máximo {1} caracteres")]
    [DisplayName("E-mail")]
    [SwaggerSchema(Description = "Endereço de e-mail do usuário")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo Senha é obrigatório")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A Senha deve ter entre {2} e {1} caracteres")]
    [DataType(DataType.Password)]
    [DisplayName("Senha")]
    [SwaggerSchema(Description = "Senha de acesso (mínimo 6 caracteres)", Format = "password")]
    public string Senha { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo Tipo de Usuário é obrigatório")]
    [DisplayName("Tipo de Usuário")]
    [EnumDataType(typeof(TipoUsuario), ErrorMessage = "Tipo de usuário inválido")]
    [SwaggerSchema(Description = "Tipo de perfil de acesso do usuário")]
    public TipoUsuario TipoUsuario { get; set; }

    [DisplayName("Recebe Alerta de Estoque")]
    [SwaggerSchema(Description = "Indica se o usuário receberá alertas de estoque baixo")]
    public bool? RecebeAlertaEstoque { get; set; }

    [StringLength(20, ErrorMessage = "O Documento deve ter no máximo {1} caracteres")]
    [DisplayName("Documento")]
    [SwaggerSchema(Description = "Número do documento (CPF, RG, etc.)")]
    public string? Documento { get; set; }
}
