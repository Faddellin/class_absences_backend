using Common.DtoModels.Others;
using Common.DtoModels.User;

namespace BusinessLogic.ServiceInterfaces;

public interface IUserService
{
    public Task<bool> IsEmailUnique(string email);
    public Task<TokenResponseModel> Register(UserRegisterModel userRegisterModel);
    public Task<TokenResponseModel> Login(LoginCredentialsModel loginCredentialsModel);
    public Task<UserModel> GetProfile(Guid userId);
    public Task EditProfile(Guid id, UserEditModel doctorEditModel);
}