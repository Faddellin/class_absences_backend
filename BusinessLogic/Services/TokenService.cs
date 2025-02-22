using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using BusinessLogic.ServiceInterfaces;
using Common;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BusinessLogic.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly ConcurrentDictionary<Guid, string>? _invalidTokens = new();

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> CreateToken(Guid userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var secret = _configuration
            .GetSection("Jwt:Secret")
            .Get<string>();
        var issuer = _configuration
            .GetSection("Jwt:Issuer")
            .Get<string>();
        var audience = _configuration
            .GetSection("Jwt:Audience")
            .Get<string>();
        var expireTime = _configuration
            .GetSection("Jwt:ExpireInMinutes")
            .Get<int>();
        var key = Encoding.ASCII.GetBytes(secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim("user_id", userId.ToString()),
                new Claim("token_id", Guid.NewGuid().ToString())
            ]),
            Expires = DateTime.UtcNow.AddMinutes(expireTime),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            Issuer = issuer,
            Audience = audience
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var strToken = tokenHandler.WriteToken(token);

        await Task.CompletedTask;
        return strToken;
    }

    public async Task<Guid> GetUserIdFromToken(string strToken)
    {
        var payload = await DecodeTokenPayload(strToken);
        if (payload == null)
        {
            throw new KeyNotFoundException("Incorrect token");
        }
        var userIdString = payload.user_id;
        if (userIdString is null or "")
        {
            throw new KeyNotFoundException("Incorrect token");
        }

        if (!Guid.TryParse(userIdString, out var userId))
        {
            throw new KeyNotFoundException("Incorrect token");
        }

        await Task.CompletedTask;
        return userId;
    }

    public async Task<Guid> GetTokenIdFromToken(string strToken)
    {
        var payload = await DecodeTokenPayload(strToken);
        if (payload == null)
        {
            throw new KeyNotFoundException("Incorrect token");
        }
        var tokenIdString = payload.token_id;
        if (tokenIdString == null)
        {
            throw new KeyNotFoundException("Incorrect token");
        }
        if (!Guid.TryParse(tokenIdString, out var tokenId))
        {
            throw new KeyNotFoundException("Incorrect token");
        }

        await Task.CompletedTask;
        return tokenId;
    }

    public async Task<TokenPayload?> DecodeTokenPayload(string strToken)
    {
        var decodedToken = Jose.JWT.Payload(strToken);
        var payload = JsonSerializer.Deserialize<TokenPayload>(decodedToken);

        await Task.CompletedTask;
        return payload;
    }

    public async Task<bool> IsTokenValid(string strToken)
    {
        var tokenId = await GetTokenIdFromToken(strToken);

        await Task.CompletedTask;
        return !_invalidTokens.ContainsKey(tokenId);
    }

    public async Task AddInvalidToken(string strToken)
    {
        var tokenId = await GetTokenIdFromToken(strToken);

        await Task.CompletedTask;
        _invalidTokens[tokenId] = strToken;
    }

    public async Task ClearTokens()
    {
        await Task.CompletedTask;
        _invalidTokens.Clear();
    }
}