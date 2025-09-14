using Core.DTOs.Repositories.Autenticacao;
using Core.DTOs.Repositories.Veiculo;
using Core.Enumeradores;

namespace Core.DTOs.Repositories.Cliente;

public class ClienteRepositoryDTO : RepositoryDto
{
    public string Nome { get; set; } = default!;
    public string? Sexo { get; set; }
    public string Documento { get; set; } = default!;
    public string DataNascimento { get; set; } = default!;
    public TipoCliente TipoCliente { get; set; }
    public EnderecoRepositoryDto Endereco { get; set; } = null!;
    public ContatoRepositoryDTO Contato { get; set; } = null!;
    public IEnumerable<VeiculoRepositoryDto> Veiculos { get; set; } = [];
}