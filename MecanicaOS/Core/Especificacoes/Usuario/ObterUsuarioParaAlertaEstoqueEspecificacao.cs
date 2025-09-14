using Core.DTOs.Repositories.Usuarios;
using Core.Especificacoes.Base;
using System.Linq.Expressions;

namespace Core.Especificacoes.Usuario;

public class ObterUsuarioParaAlertaEstoqueEspecificacao : EspecificacaoBase<UsuarioRepositoryDto>
{
    public override Expression<Func<UsuarioRepositoryDto, bool>> Expressao =>
     u => u.RecebeAlertaEstoque;
}