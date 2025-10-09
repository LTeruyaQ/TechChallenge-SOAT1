using Core.DTOs.Entidades.Usuarios;
using Core.Especificacoes.Base;
using System.Linq.Expressions;

namespace Core.Especificacoes.Usuario;

public class ObterUsuarioParaAlertaEstoqueEspecificacao : EspecificacaoBase<UsuarioEntityDto>
{
    public override Expression<Func<UsuarioEntityDto, bool>> Expressao =>
     u => u.RecebeAlertaEstoque;
}