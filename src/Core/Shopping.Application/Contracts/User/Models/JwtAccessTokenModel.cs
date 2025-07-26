namespace Shopping.Application.Contracts.User.Models;

public record JwtAccessTokenModel(string AccessToken,double ExpirySeconds,string TokenType = "Bearer");