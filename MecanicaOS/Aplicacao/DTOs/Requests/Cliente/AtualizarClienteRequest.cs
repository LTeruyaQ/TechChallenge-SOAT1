using Dominio.Entidades;

namespace Aplicacao.DTOs.Requests.Veiculo
{
    public class AtualizarClienteRequest
    {
        public Guid? Id { get; set; }
        public string? Nome { get; set; }
        public string? Sexo { get; set; }
        public string? Documento { get; set; }
        public string? DataNascimento { get; set; }
        public string? TipoCliente { get; set; }
        public Guid? EnderecoId { get; set; }
        public Endereco? Endereco { get; set; }
        public Guid? ContatoId { get; set; }
        public Contato? Contato { get; set; }
        public Guid? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
