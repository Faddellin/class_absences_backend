namespace BusinessLogic.ServiceInterfaces;

public interface IReportService
{

    public Task ExportUserAbsencesInWord(DateTime dateFrom, DateTime dateTo, Guid userId, List<Guid> targetUserId);

}
