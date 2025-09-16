using Core.Entidades;
using Core.Entidades.Abstratos;
using Core.DTOs.Entidades.Autenticacao;
using Core.DTOs.Entidades.Cliente;
using Core.DTOs.Entidades.Veiculo;
using Core.Enumeradores;

namespace MecanicaOS.UnitTests.Fixtures;

public abstract class BaseFixture
{
    protected static readonly Guid ValidGuid = Guid.NewGuid();
    protected static readonly DateTime ValidDateTime = DateTime.Now;
    protected static readonly string ValidString = "Texto válido";
    
    protected static T CriarEntidadeComCamposObrigatorios<T>() where T : Entidade, new()
    {
        var entidade = new T
        {
            Id = ValidGuid,
            Ativo = true,
            DataCadastro = ValidDateTime,
            DataAtualizacao = ValidDateTime
        };
        
        return entidade;
    }
    
    protected static T CriarRepositoryDtoComCamposObrigatorios<T>() where T : EntityDto, new()
    {
        var dto = new T
        {
            Id = ValidGuid,
            Ativo = true,
            DataCadastro = ValidDateTime,
            DataAtualizacao = ValidDateTime
        };
        
        return dto;
    }
}
