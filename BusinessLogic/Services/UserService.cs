using BusinessLogic.ServiceInterfaces;
using BusinessLogic.Static;
using Common.DbModels;
using Common.DtoModels;
using Common.DtoModels.Others;
using Common.DtoModels.User;
using DocumentFormat.OpenXml.Spreadsheet;
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
        UserEntity? user = await _appDbContext.Users
            .FirstOrDefaultAsync(x => x.Email == userRegisterModel.Email);


        if (user != null)
        {
            throw new Exception("User with such Email already exists");
        }

        var passwordService = new PasswordService();

        var userEntity = new UserEntity()
        {
            Id = Guid.NewGuid(),
            FirstName = userRegisterModel.FirstName,
            MiddleName = userRegisterModel.MiddleName,
            LastName = userRegisterModel.LastName,
            Email = userRegisterModel.Email,
            PasswordHash = passwordService.HashPassword(userRegisterModel.Password)
        };
        

        await _appDbContext.Users.AddAsync(userEntity);
        await _appDbContext.SaveChangesAsync();

        var strAccessToken = await _tokenService.CreateToken(userEntity);
        var strRefreshToken = await _tokenService.GenerateAndSaveRefreshToken(userEntity);
        var tokenResponse = new TokenResponseModel
        {
            AccessToken = strAccessToken,
            RefreshToken = strRefreshToken
        };
        
        return tokenResponse;
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
                var strAccessToken = await _tokenService.CreateToken(user);
                var strRefreshToken = await _tokenService.GenerateAndSaveRefreshToken(user);
                var tokenResponse = new TokenResponseModel
                {
                    AccessToken = strAccessToken,
                    RefreshToken = strRefreshToken
                };
        
                return tokenResponse;
            }

        }

        return (new TokenResponseModel { AccessToken = "" });
    }

    public async Task<UserModel> GetProfile(Guid userId)
    {
        var user = await _appDbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        Validator.ThrowIfNull(user);
        
        var userModel = new UserModel
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            MiddleName = user.MiddleName,
            LastName = user.LastName,
            UserTypes = user.UserTypes
        };

        return userModel;
    }
    public async Task<UserRolesModel> GetUserRoles(Guid userId)
    {
        var user = await _appDbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        Validator.ThrowIfNull(user);

        var userRolesModel = new UserRolesModel
        {
            UserTypes = user.UserTypes
        };

        return userRolesModel;
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