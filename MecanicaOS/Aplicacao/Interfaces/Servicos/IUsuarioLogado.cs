using System;

namespace Aplicacao.Interfaces.Servicos;

public interface IUsuarioLogado
{
    Guid? ObterId();
    string ObterLogin();
    string ObterEmail();
    string ObterNome();
    string ObterTipoUsuario();
    bool EstaAutenticado();
}
