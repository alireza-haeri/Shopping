namespace Shopping.Infrastructure.Identity.Services.Model;

internal class JwtConfiguration
{
    public string EncryptionKey { get; set; } = null!;
    public string SignInKey { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public int ExpirationMinute { get; set; }
}