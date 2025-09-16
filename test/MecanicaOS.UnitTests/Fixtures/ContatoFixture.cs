using Core.Entidades;
using Core.DTOs.Entidades.Cliente;

namespace MecanicaOS.UnitTests.Fixtures;

public class ContatoFixture : BaseFixture
{
    public static Contato CriarContatoValido()
    {
        var contato = CriarEntidadeComCamposObrigatorios<Contato>();
        contato.Email = "joao.silva@email.com";
        contato.Telefone = "(11) 99999-9999";
        
        return contato;
    }
    
    public static Contato CriarContatoComDadosInvalidos()
    {
        var contato = CriarEntidadeComCamposObrigatorios<Contato>();
        contato.Email = "email-invalido"; // Email inválido
        contato.Telefone = "123"; // Telefone inválido
        
        return contato;
    }
    
    public static ContatoEntityDto CriarContatoEntityDtoValido()
    {
        var dto = CriarRepositoryDtoComCamposObrigatorios<ContatoEntityDto>();
        dto.Email = "joao.silva@email.com";
        dto.Telefone = "(11) 99999-9999";
        
        return dto;
    }
}
