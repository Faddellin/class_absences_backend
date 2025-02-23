using Common.DbModels;

namespace BusinessLogic.ServiceInterfaces;

public interface IRefreshTokenService
{
    public Task SaveRefreshToken(UserEntity user, string refreshToken, DateTime expiryDate);
    public Task<UserEntity?> ValidateRefreshToken(Guid userId, string refreshToken);
}