using Core.DTOs.UseCases.Autenticacao;

namespace MecanicaOS.UnitTests.Fixtures.UseCases;

public static class AutenticacaoUseCaseFixture
{
    public static AutenticacaoUseCaseDto CriarAutenticacaoUseCaseDtoValido()
    {
        return new AutenticacaoUseCaseDto
        {
            Email = "usuario@teste.com",
            Senha = "senhaSegura123"
        };
    }

    public static AutenticacaoUseCaseDto CriarAutenticacaoUseCaseDtoAdmin()
    {
        return new AutenticacaoUseCaseDto
        {
            Email = "admin@mecanica.com",
            Senha = "adminPass@456"
        };
    }

    public static AutenticacaoUseCaseDto CriarAutenticacaoUseCaseDtoEmailLongo()
    {
        return new AutenticacaoUseCaseDto
        {
            Email = "usuario.com.nome.muito.longo@empresa.exemplo.com.br",
            Senha = "senhaComplexa@789"
        };
    }

    public static AutenticacaoUseCaseDto CriarAutenticacaoUseCaseDtoSenhaCompleta()
    {
        return new AutenticacaoUseCaseDto
        {
            Email = "teste@sistema.com",
            Senha = "MinhaSenh@Muito$egura123!@#"
        };
    }

    public static List<AutenticacaoUseCaseDto> CriarListaAutenticacaoUseCaseDto()
    {
        return new List<AutenticacaoUseCaseDto>
        {
            CriarAutenticacaoUseCaseDtoValido(),
            CriarAutenticacaoUseCaseDtoAdmin(),
            CriarAutenticacaoUseCaseDtoEmailLongo(),
            CriarAutenticacaoUseCaseDtoSenhaCompleta()
        };
    }
}
