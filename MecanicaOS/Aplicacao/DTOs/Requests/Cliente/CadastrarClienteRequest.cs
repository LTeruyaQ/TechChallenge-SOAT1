namespace Aplicacao.DTOs.Requests.Veiculo
{
    public class CadastrarClienteRequest
    {
        public Guid Id { get; set; }
        public string? Nome { get; set; }
        public string? Sexo { get; set; }
        public string? Documento { get; set; }
        public string? DataNascimento { get; set; }
        public string? TipoCliente { get; set; }
        public string? Rua { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Numero { get; set; }
        public string? CEP { get; set; }
        public string? Complemento { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }

    }
}
