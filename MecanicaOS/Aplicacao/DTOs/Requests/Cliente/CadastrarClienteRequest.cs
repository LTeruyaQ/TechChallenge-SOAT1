using Dominio.Entidades;

namespace Aplicacao.DTOs.Requests.Veiculo
{
    public class CadastrarClienteRequest
    {   
        public string? Nome { get; set; }
        public string? Sexo { get; set; }
        public string? Documento { get; set; }
        public string? DataNascimento { get; set; }
        public string? TipoCliente { get; set; }     
        public Endereco? Endereco { get; set; }
        public Contato? Contato { get; set; }        
        public Usuario? Usuario { get; set; }
    }
}
