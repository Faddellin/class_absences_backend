using Common.DtoModels.Others;
using Common.DtoModels.User;

namespace BusinessLogic.ServiceInterfaces;

public interface IUserService
{
    public Task<TokenResponseModel> Register(UserRegisterModel userRegisterModel);
    public Task<TokenResponseModel> Login(LoginCredentialsModel loginCredentialsModel);
    public Task<UserModel> GetProfile(Guid userId);
    public Task<UserRolesModel> GetUserRoles(Guid userId);
    public Task<UserFullModel> GetUserInformation(Guid userId, Guid targetUserId);
    public Task EditProfile(Guid id, UserEditModel doctorEditModel);
}