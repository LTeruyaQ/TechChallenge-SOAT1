using Dominio.Entidades;
using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes;

public class ClientePorDocumentoEspecificacao : Especificacao<Cliente>
{
    private readonly string _documento;

    public ClientePorDocumentoEspecificacao(string documento)
    {
        _documento = documento;
    }

    public override Expression<Func<Cliente, bool>> Expressao =>
        cliente => cliente.Documento == _documento;
}