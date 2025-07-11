using Shopping.Application.Contracts.User.Models;
using Shopping.Domain.Entities.User;

namespace Shopping.Application.Contracts.User;

public interface IJwtService
{
    Task<JwtAccessTokenModel> GenerateJwtTokenAsync(UserEntity user,CancellationToken cancellationToken);
}