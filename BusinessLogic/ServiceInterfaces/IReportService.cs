namespace BusinessLogic.ServiceInterfaces;

public interface IReportService
{

    public Task<MemoryStream> ExportUserAbsencesInWord(DateTime dateFrom, DateTime dateTo, Guid userId, List<Guid> targetUserId);

}
