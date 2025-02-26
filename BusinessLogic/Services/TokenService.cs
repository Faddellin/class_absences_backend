using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using BusinessLogic.ServiceInterfaces;
using Common;
using Common.DbModels;
using Common.DtoModels;
using Common.DtoModels.Others;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace BusinessLogic.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly ConcurrentDictionary<Guid, string>? _invalidTokens = new();
    private readonly IServiceProvider _serviceProvider;

    public TokenService(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    public async Task<string> CreateToken(UserEntity user)
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
                new Claim("user_id", user.Id.ToString()),
                new Claim("token_id", Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.UserType.ToString())
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

    public async Task<TokenResponseModel> CreateTokenResponse(UserEntity user)
    {
        return new TokenResponseModel
        {
            AccessToken = await CreateToken(user),
            RefreshToken = await GenerateAndSaveRefreshToken(user)
        };
    }

    public async Task<Guid> GetUserIdFromToken(string strToken)
    {
        var payload = DecodeTokenPayload(strToken);
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

    public Task<UserType> GetUserRoleFromToken(string strToken)
    {
        var payload = DecodeTokenPayload(strToken);
        if (payload == null)
        {
            throw new KeyNotFoundException("Incorrect token");
        }

        var role = payload.role;
        if (role == null)
        {
            throw new KeyNotFoundException("Incorrect token");
        }

        var result = Enum.TryParse(role, out UserType userRole);
        return Task.FromResult(result ? userRole : UserType.Student);
    }

    public async Task<Guid> GetTokenIdFromToken(string strToken)
    {
        var payload = DecodeTokenPayload(strToken);
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

    public TokenPayload? DecodeTokenPayload(string strToken)
    {
        var decodedToken = Jose.JWT.Payload(strToken);
        var payload = JsonSerializer.Deserialize<TokenPayload>(decodedToken);

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

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public async Task<string> GenerateAndSaveRefreshToken(UserEntity user)
    {
        using var scope = _serviceProvider.CreateScope();
        var refreshTokenService = scope.ServiceProvider.GetRequiredService<IRefreshTokenService>();
        var refreshToken = GenerateRefreshToken();
        await refreshTokenService.SaveRefreshToken(user, refreshToken,
        DateTime.UtcNow.AddDays(_configuration
            .GetSection("RefreshToken:ExpireInDays")
            .Get<int>()));
        return refreshToken;
    }

    public async Task<UserEntity?> ValidateRefreshToken(Guid userId, string refreshToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var refreshTokenService = scope.ServiceProvider.GetRequiredService<IRefreshTokenService>();
        return await refreshTokenService.ValidateRefreshToken(userId, refreshToken);
    }

    public async Task<TokenResponseModel?> RefreshTokens(RefreshTokenRequestModel request)
    {
        var user = await ValidateRefreshToken(request.UserId, request.RefreshToken);
        if (user == null)
        {
            return null;
        }

        return await CreateTokenResponse(user);
    }
}