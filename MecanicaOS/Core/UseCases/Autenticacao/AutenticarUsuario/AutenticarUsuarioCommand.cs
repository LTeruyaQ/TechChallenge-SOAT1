using Core.DTOs.UseCases.Autenticacao;

namespace Core.UseCases.Autenticacao.AutenticarUsuario
{
    public class AutenticarUsuarioCommand
    {
        public AutenticacaoUseCaseDto Request { get; set; }

        public AutenticarUsuarioCommand(AutenticacaoUseCaseDto request)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
        }
    }
}
