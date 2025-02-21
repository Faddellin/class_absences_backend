using Common.DtoModels;
using Common.DtoModels.Request;

namespace BusinessLogic.ServiceInterfaces;

public interface IRequestService
{
    public Task<Guid> CreateRequest(RequestCreateModel requestCreateModel, Guid userId, Guid? reasonId);

    public Task AddReasonToRequest(Guid reasonId, Guid requestId);

    public Task<RequestListModel> GetAllRequests(SortType sortType, RequestStatus? requestStatus, string? userName);

}