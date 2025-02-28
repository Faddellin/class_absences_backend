using Common.DtoModels.User;

namespace BusinessLogic.ServiceInterfaces;

public interface IReportService
{

    public Task<MemoryStream> ExportUserAbsencesInWord(DateTime dateFrom, DateTime dateTo, Guid userId, List<Guid> targetUserId);
    public Task<UserListModel?> GetAllUsers(Guid userId);
}
