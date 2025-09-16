using Core.Entidades;
using Core.DTOs.Entidades.Cliente;

namespace MecanicaOS.UnitTests.Fixtures;

public class EnderecoFixture : BaseFixture
{
    public static Endereco CriarEnderecoValido()
    {
        var endereco = CriarEntidadeComCamposObrigatorios<Endereco>();
        endereco.Rua = "Rua das Flores, 123";
        endereco.Bairro = "Centro";
        endereco.Cidade = "São Paulo";
        endereco.CEP = "01234-567";
        endereco.Complemento = "Apto 101";
        
        return endereco;
    }
    
    public static Endereco CriarEnderecoComDadosInvalidos()
    {
        var endereco = CriarEntidadeComCamposObrigatorios<Endereco>();
        endereco.Rua = ""; // Rua vazia - inválida
        endereco.Bairro = ""; // Bairro vazio - inválido
        endereco.Cidade = ""; // Cidade vazia - inválida
        endereco.CEP = "123"; // CEP inválido
        
        return endereco;
    }
    
    public static EnderecoEntityDto CriarEnderecoEntityDtoValido()
    {
        var dto = CriarRepositoryDtoComCamposObrigatorios<EnderecoEntityDto>();
        dto.Rua = "Rua das Flores, 123";
        dto.Bairro = "Centro";
        dto.Cidade = "São Paulo";
        dto.CEP = "01234-567";
        dto.Complemento = "Apto 101";
        dto.Numero = "123";
        dto.IdCliente = ValidGuid;
        
        return dto;
    }
}
