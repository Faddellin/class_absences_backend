using Common;

namespace BusinessLogic.ServiceInterfaces;

public interface ITokenService
{
    public Task<string> CreateToken(Guid userId);
    public Task<Guid> GetUserIdFromToken(string strToken);
    public Task<Guid> GetTokenIdFromToken(string strToken);
    public Task<TokenPayload?> DecodeTokenPayload(string strToken);
    public Task<bool> IsTokenValid(string strToken);
    public Task AddInvalidToken(string strToken);
    public Task ClearTokens();
}