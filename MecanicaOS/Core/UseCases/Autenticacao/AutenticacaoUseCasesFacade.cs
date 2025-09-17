using Core.DTOs.UseCases.Autenticacao;
using Core.Interfaces.UseCases;
using Core.UseCases.Autenticacao.AutenticarUsuario;

namespace Core.UseCases.Autenticacao
{
    /// <summary>
    /// Facade para manter compatibilidade com a interface IAutenticacaoUseCases
    /// enquanto utiliza os novos casos de uso individuais
    /// </summary>
    public class AutenticacaoUseCasesFacade : IAutenticacaoUseCases
    {
        private readonly AutenticarUsuarioHandler _autenticarUsuarioHandler;

        public AutenticacaoUseCasesFacade(AutenticarUsuarioHandler autenticarUsuarioHandler)
        {
            _autenticarUsuarioHandler = autenticarUsuarioHandler ?? throw new ArgumentNullException(nameof(autenticarUsuarioHandler));
        }

        public async Task<AutenticacaoDto> AutenticarUseCaseAsync(AutenticacaoUseCaseDto request)
        {
            var response = await _autenticarUsuarioHandler.Handle(request);
            return response.Autenticacao;
        }
    }
}
