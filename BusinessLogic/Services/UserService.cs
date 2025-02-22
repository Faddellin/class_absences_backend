using BusinessLogic.ServiceInterfaces;
using Common.DbModels;
using Common.DtoModels;
using Common.DtoModels.Others;
using Common.DtoModels.User;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _appDbContext;
    private readonly ITokenService _tokenService;

    public UserService(AppDbContext appDbContext, ITokenService tokenService)
    {
        _appDbContext = appDbContext;
        _tokenService = tokenService;
    }

    public async Task<bool> IsEmailUnique(string email)
    {
        var user = await _appDbContext.Users
            .FirstOrDefaultAsync(x => x.Email == email);
        return user == null;
    }

    public async Task<TokenResponseModel> Register(UserRegisterModel userRegisterModel)
    {
        var passwordService = new PasswordService();

        var userEntity = new UserEntity()
        {
            Id = Guid.NewGuid(),
            Name = userRegisterModel.Name,
            Email = userRegisterModel.Email,
            PasswordHash = passwordService.HashPassword(userRegisterModel.Password),
            UserType = UserType.Unverified
        };

        await _appDbContext.Users.AddAsync(userEntity);
        await _appDbContext.SaveChangesAsync();

        var strToken = await _tokenService.CreateToken(userEntity.Id);
        var token = new TokenResponseModel
        {
            Token = strToken
        };
        
        return token;
    }

    public async Task<TokenResponseModel> Login(LoginCredentialsModel loginCredentialsModel)
    {
        var user = await _appDbContext.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == loginCredentialsModel.Email.ToLower());
        if (user != null)
        {
            var passwordService = new PasswordService();
            var verificationResult = passwordService.VerifyPassword(loginCredentialsModel.Password, user.PasswordHash);

            if (verificationResult)
            {
                var strToken = await _tokenService.CreateToken(user.Id);
                var token = new TokenResponseModel
                {
                    Token = strToken
                };
                return token;  
            }

        }

        return (new TokenResponseModel { Token = "" });
    }

    public async Task<UserModel> GetProfile(Guid userId)
    {
        var user = await _appDbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new KeyNotFoundException();
        }
        
        var userModel = new UserModel
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            UserType = user.UserType
        };

        return userModel;
    }

    public async Task EditProfile(Guid id, UserEditModel userEditModel)
    {
        var passwordService = new PasswordService();
        var newPassword = passwordService.HashPassword(userEditModel.Password);

        await _appDbContext.Users
            .Where(u => u.Id == id)
            .ExecuteUpdateAsync(x => x
                .SetProperty(e => e.PasswordHash, newPassword));
        await _appDbContext.SaveChangesAsync();
    }
}