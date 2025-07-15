namespace Shopping.Application.Contracts.User.Models;

public record JwtAccessTokenModel(string AccessToken,int ExpirySeconds,string TokenType = "Bearer");