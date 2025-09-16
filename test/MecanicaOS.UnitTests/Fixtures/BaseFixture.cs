using Core.DTOs.Entidades.Autenticacao;
using Core.Entidades.Abstratos;

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
