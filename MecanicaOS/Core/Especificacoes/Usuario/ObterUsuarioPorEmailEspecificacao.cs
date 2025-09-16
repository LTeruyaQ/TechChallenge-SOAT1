using Core.DTOs.Entidades.Usuarios;
using Core.Especificacoes.Base;
using System.Linq.Expressions;

namespace Core.Especificacoes.Usuario;

public class ObterUsuarioPorEmailEspecificacao : EspecificacaoBase<UsuarioEntityDto>
{
    private string email;

    public ObterUsuarioPorEmailEspecificacao(string email)
    {
        this.email = email;
    }

    public override Expression<Func<UsuarioEntityDto, bool>> Expressao => u => u.Email == email;
}
