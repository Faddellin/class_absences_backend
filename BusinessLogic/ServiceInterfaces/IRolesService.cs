using Common.DbModels;
using Common.DtoModels;
using Common.DtoModels.Request;
using Common.DtoModels.User;

namespace BusinessLogic.ServiceInterfaces;

public interface IRolesService
{
    public Task ChangeRole(Guid userId, Guid targetUserId, UserType newUserType);
    public Task ExportUserAbsencesInWord(DateTime dateFrom, DateTime dateTo, Guid userId, List<Guid> targetUserId);
}