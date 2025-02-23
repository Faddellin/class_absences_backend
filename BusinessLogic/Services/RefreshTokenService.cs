using BusinessLogic.ServiceInterfaces;
using Common.DbModels;

namespace BusinessLogic.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly AppDbContext _appDbContext;

    public RefreshTokenService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task SaveRefreshToken(UserEntity user, string refreshToken, DateTime expiryDate)
    {
        var userEntity = await _appDbContext.Users.FindAsync(user.Id);
        userEntity.RefreshToken = refreshToken;
        userEntity.RefreshTokenExpiryDate = expiryDate;
        await _appDbContext.SaveChangesAsync();
    }

    public async Task<UserEntity?> ValidateRefreshToken(Guid userId, string refreshToken)
    {
        var user = await _appDbContext.Users.FindAsync(userId);
        if (user == null || user.RefreshToken != refreshToken ||
            user.RefreshTokenExpiryDate < DateTime.UtcNow)
        {
            return null;
        }

        return user;
    }
}