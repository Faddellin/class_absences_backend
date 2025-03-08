using Common.DbModels;
using Common.DtoModels;
using Common.DtoModels.Request;
using Common.DtoModels.User;

namespace BusinessLogic.ServiceInterfaces;

public interface IRolesService
{
    public Task AddRole(Guid userId, Guid targetUserId, UserType newUserType);
    public Task DeleteRole(Guid userId, Guid targetUserId, UserType newUserType);
}