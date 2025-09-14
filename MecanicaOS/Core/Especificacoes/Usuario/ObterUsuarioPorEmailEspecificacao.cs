using Core.DTOs.Repositories.Usuarios;
using Core.Especificacoes.Base;
using System.Linq.Expressions;

namespace Core.Especificacoes.Usuario;

public class ObterUsuarioPorEmailEspecificacao : EspecificacaoBase<UsuarioRepositoryDto>
{
    private string email;

    public ObterUsuarioPorEmailEspecificacao(string email)
    {
        this.email = email;
    }

    public override Expression<Func<UsuarioRepositoryDto, bool>> Expressao => u => u.Email == email;
}
