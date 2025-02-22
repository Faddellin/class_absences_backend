using Common.DbModels;
using Common.DtoModels;
using Common.DtoModels.Request;

namespace BusinessLogic.ServiceInterfaces;

public interface IRequestService
{
    public Task<Guid> CreateRequest(RequestCreateModel requestCreateModel, Guid userId);

    public Task ChangeRequestReason(Guid reasonId, Guid requestId, Guid userId);

    public Task EditRequest(RequestCreateModel requestCreateModel, Guid requestId, Guid userId);

    public Task<RequestListModel> GetAllRequests(SortType sortType, RequestStatus? requestStatus, string? userName, DateTime? dateFrom, DateTime? dateTo, Guid userId);

    public Task<RequestModel> GetRequest(Guid requestId, Guid userId);

    public Task<RequestListModel> GetUserRequests(SortType sortType, RequestStatus? requestStatus, DateTime? dateFrom, DateTime? dateTo, Guid userId, Guid targetUserId);

}