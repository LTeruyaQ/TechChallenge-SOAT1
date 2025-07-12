using Dominio.Interfaces.Servicos;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infraestrutura.Autenticacao;

public class ServicoJwt : IServicoJwt
{
    private readonly ConfiguracaoJwt _configuracaoJwt;

    public ServicoJwt(IOptions<ConfiguracaoJwt> configuracaoJwt)
    {
        _configuracaoJwt = configuracaoJwt.Value;
    }

    public string GerarToken(Guid usuarioId, string email, string tipoUsuario)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuracaoJwt.SecretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, tipoUsuario),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(_configuracaoJwt.ExpiryInMinutes),
            Issuer = _configuracaoJwt.Issuer,
            Audience = _configuracaoJwt.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public bool ValidarToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuracaoJwt.SecretKey);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _configuracaoJwt.Issuer,
                ValidAudience = _configuracaoJwt.Audience,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return true;
        }
        catch
        {
            return false;
        }
    }
}
