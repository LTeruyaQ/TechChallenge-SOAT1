using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Aplicacao.DTOs.Requests.Cliente
{
    [DisplayName("Cadastro de Cliente")]
    [SwaggerSchema(Title = "Cadastro de Cliente", Description = "Dados necessários para cadastrar um novo cliente")]
    public class CadastrarClienteRequest
    {
        [SwaggerSchema(Description = "Identificador único do cliente")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O Nome deve ter no máximo {1} caracteres")]
        [DisplayName("Nome Completo")]
        [SwaggerSchema(Description = "Nome completo do cliente")]
        public string Nome { get; set; } = null!;

        [Required(ErrorMessage = "O campo Sexo é obrigatório")]
        [StringLength(1, ErrorMessage = "O Sexo deve ter 1 caractere (M/F/O)")]
        [RegularExpression(@"^[MFOmfo]$", ErrorMessage = "O Sexo deve ser M, F ou O")]
        [DisplayName("Sexo")]
        [SwaggerSchema(Description = "Sexo do cliente (M - Masculino, F - Feminino, O - Outro)")]
        public string Sexo { get; set; } = null!;

        [Required(ErrorMessage = "O campo Documento é obrigatório")]
        [StringLength(20, ErrorMessage = "O Documento deve ter no máximo {1} caracteres")]
        [DisplayName("Documento")]
        [SwaggerSchema(Description = "Número do documento (RG, CNH, etc.)")]
        public string Documento { get; set; } = null!;

        [Required(ErrorMessage = "O campo Data de Nascimento é obrigatório")]
        [DisplayName("Data de Nascimento")]
        [SwaggerSchema(Description = "Data de nascimento do cliente (formato: dd/MM/yyyy)")]
        public string DataNascimento { get; set; } = null!;

        [Required(ErrorMessage = "O campo Tipo de Cliente é obrigatório")]
        [DisplayName("Tipo de Cliente")]
        [SwaggerSchema(Description = "Tipo de cliente (Física/Jurídica)")]
        public string TipoCliente { get; set; } = null!;

        [Required(ErrorMessage = "O campo Rua é obrigatório")]
        [StringLength(200, ErrorMessage = "A Rua deve ter no máximo {1} caracteres")]
        [DisplayName("Rua")]
        [SwaggerSchema(Description = "Nome da rua do endereço")]
        public string Rua { get; set; } = null!;

        [Required(ErrorMessage = "O campo Bairro é obrigatório")]
        [StringLength(100, ErrorMessage = "O Bairro deve ter no máximo {1} caracteres")]
        [DisplayName("Bairro")]
        [SwaggerSchema(Description = "Nome do bairro")]
        public string Bairro { get; set; } = null!;

        [Required(ErrorMessage = "O campo Cidade é obrigatório")]
        [StringLength(100, ErrorMessage = "A Cidade deve ter no máximo {1} caracteres")]
        [DisplayName("Cidade")]
        [SwaggerSchema(Description = "Nome da cidade")]
        public string Cidade { get; set; } = null!;

        [Required(ErrorMessage = "O campo Número é obrigatório")]
        [StringLength(20, ErrorMessage = "O Número deve ter no máximo {1} caracteres")]
        [DisplayName("Número")]
        [SwaggerSchema(Description = "Número do endereço")]
        public string Numero { get; set; } = null!;

        [Required(ErrorMessage = "O campo CEP é obrigatório")]
        [StringLength(9, ErrorMessage = "O CEP deve ter no máximo {1} caracteres")]
        [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "Formato de CEP inválido. Use o formato: 00000-000 ou 00000000")]
        [DisplayName("CEP")]
        [SwaggerSchema(Description = "CEP do endereço (formato: 00000-000 ou 00000000)")]
        public string CEP { get; set; } = null!;

        [StringLength(100, ErrorMessage = "O Complemento deve ter no máximo {1} caracteres")]
        [DisplayName("Complemento")]
        [SwaggerSchema(Description = "Complemento do endereço")]
        public string? Complemento { get; set; }

        [Required(ErrorMessage = "O campo E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "O Email informado não é válido")]
        [StringLength(100, ErrorMessage = "O Email deve ter no máximo {1} caracteres")]
        [DisplayName("E-mail")]
        [SwaggerSchema(Description = "Endereço de e-mail do cliente")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "O campo Telefone é obrigatório")]
        [StringLength(15, ErrorMessage = "O Telefone deve ter no máximo {1} caracteres")]
        [RegularExpression(@"^\+?[0-9\s-()]*$", ErrorMessage = "Formato de telefone inválido")]
        [DisplayName("Telefone")]
        [SwaggerSchema(Description = "Número de telefone do cliente")]
        public string Telefone { get; set; } = null!;
    }
}