using Dominio.Entidades;

namespace Aplicacao.DTOs.Responses.Estoque
{
    public class ClienteResponse
    {
        public Guid Id { get; set; }                
        public string? Nome { get; set; }
        public string? Sexo { get; set; }
        public string? Documento { get; set; }
        public string? DataNascimento { get; set; }
        public string? TipoCliente { get; set; }        
        public Guid? EnderecoId { get; set; }
        public Guid? ContatoId { get; set; }
        public Guid? UsuarioId { get; set; }
        public string? DataCadastro { get; set; }
        public string? DataAtualizacao { get; set; }



    }
}
