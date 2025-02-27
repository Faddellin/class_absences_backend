using Common;
using Common.DbModels;
using Common.DtoModels;
using Common.DtoModels.Others;

namespace BusinessLogic.ServiceInterfaces;

public interface ITokenService
{
    public Task<string> CreateToken(UserEntity user);
    public Task<TokenResponseModel> CreateTokenResponse(UserEntity user);
    public Task<Guid> GetUserIdFromToken(string strToken);
    public Task<UserType> GetUserRoleFromToken(string strToken);
    public Task<Guid> GetTokenIdFromToken(string strToken);
    public TokenPayload? DecodeTokenPayload(string strToken);
    public Task<bool> IsTokenValid(string strToken);
    public Task AddInvalidToken(string strToken);
    public Task ClearTokens();
    public Task<string> GenerateAndSaveRefreshToken(UserEntity user);
    public Task<UserEntity?> ValidateRefreshToken(Guid userId, string refreshToken);
    public Task<TokenResponseModel?> RefreshTokens(RefreshTokenRequestModel request);
}