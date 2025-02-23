using Common.DbModels;

namespace BusinessLogic.ServiceInterfaces;

public interface IRefreshTokenService
{
    Task SaveRefreshToken(UserEntity user, string refreshToken, DateTime expiryDate);
    Task<UserEntity?> ValidateRefreshToken(Guid userId, string refreshToken);
}