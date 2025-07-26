using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shopping.Application.Contracts.User;
using Shopping.Application.Contracts.User.Models;
using Shopping.Domain.Entities.User;
using Shopping.Infrastructure.Identity.IdentitySetup.Factories;
using Shopping.Infrastructure.Identity.Services.Model;

namespace Shopping.Infrastructure.Identity.Services.Implementations;

internal class JwtServiceImplementation(
    AppUserClaimPrincipalFactory claimPrincipalFactory,
    IOptions<JwtConfiguration> jwtConfiguration) : IJwtService
{
    private readonly JwtConfiguration _jwtConfiguration = jwtConfiguration.Value;

    public async Task<JwtAccessTokenModel> GenerateJwtTokenAsync(UserEntity user, CancellationToken cancellationToken)
    {
        var claims = await claimPrincipalFactory.CreateAsync(user);

        var secretKey = Encoding.UTF8.GetBytes(_jwtConfiguration.SignInKey);
        var signInCredential =
            new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256);
        var descriptor = new SecurityTokenDescriptor()
        {
            Issuer = _jwtConfiguration.Issuer,
            Audience = _jwtConfiguration.Audience,
            IssuedAt = DateTime.Now,
            NotBefore = DateTime.Now.AddMinutes(0),
            Expires = DateTime.Now.AddMinutes(_jwtConfiguration.ExpirationMinute),
            SigningCredentials = signInCredential,
            Subject = new ClaimsIdentity(claims.Claims)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(descriptor);

        return new JwtAccessTokenModel(tokenHandler.WriteToken(token)   ,(token.ValidTo - DateTime.Now).TotalSeconds);
    }
}