using Core.DTOs.Autenticacao;

namespace Core.Interfaces.UseCases
{
    public interface IAutenticacaoUseCases
    {
        Task<AutenticacaoDto> AutenticarUseCaseAsync(AutenticacaoUseCaseDto request);
    }
}