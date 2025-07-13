using Dominio.Enumeradores;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Aplicacao.DTOs.Requests.Usuario;

[DisplayName("Atualização de Usuário")]
[SwaggerSchema(Title = "Atualização de Usuário", Description = "Dados para atualização de um usuário existente")]
public class AtualizarUsuarioRequest
{
    [EmailAddress(ErrorMessage = "O Email informado não é válido")]
    [StringLength(240, ErrorMessage = "O Email deve ter no máximo {1} caracteres")]
    [DisplayName("E-mail")]
    [SwaggerSchema(Description = "Endereço de e-mail do usuário")]
    public string? Email { get; set; }

    [StringLength(100, MinimumLength = 6, ErrorMessage = "A Senha deve ter entre {2} e {1} caracteres")]
    [DisplayName("Senha")]
    [SwaggerSchema(Description = "Nova senha de acesso (mínimo 6 caracteres)", Format = "password")]
    public string? Senha { get; set; }

    [DisplayName("Data do Último Acesso")]
    [SwaggerSchema(Description = "Data e hora do último acesso do usuário")]
    public DateTime? DataUltimoAcesso { get; set; }

    [DisplayName("Tipo de Usuário")]
    [EnumDataType(typeof(TipoUsuario), ErrorMessage = "Tipo de usuário inválido")]
    [SwaggerSchema(Description = "Tipo de perfil de acesso do usuário")]
    public TipoUsuario? TipoUsuario { get; set; }

    [DisplayName("Recebe Alerta de Estoque")]
    [SwaggerSchema(Description = "Indica se o usuário recebe alertas de estoque baixo")]
    public bool? RecebeAlertaEstoque { get; set; }

    [DisplayName("Documento")]
    [SwaggerSchema(Description = "Documento do cliente")]
    public string? Documento { get; set; }
}