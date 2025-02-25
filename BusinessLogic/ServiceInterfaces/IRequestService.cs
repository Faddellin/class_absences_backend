using Common.DbModels;
using Common.DtoModels;
using Common.DtoModels.Request;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.ServiceInterfaces;

public interface IRequestService
{
    public Task<Guid> CreateRequest(RequestCreateModel requestCreateModel, Guid userId, IFormFileCollection files);

    // public Task ChangeRequestReason(Guid reasonId, Guid requestId, Guid userId);

    public Task EditRequest(RequestEditModel requestEditModel, Guid requestId, Guid userId);

    public Task<RequestModel> AddImagesToRequest(Guid userId, Guid requestId, IFormFileCollection files);

    public Task<RequestListModel> GetAllRequests(SortType sortType, RequestStatus? requestStatus, string? userName, DateTime? dateFrom, DateTime? dateTo, Guid userId);

    public Task<RequestModel> GetRequest(Guid requestId, Guid userId);

    public Task<RequestListModel> GetUserRequests(SortType sortType, RequestStatus? requestStatus, DateTime? dateFrom, DateTime? dateTo, Guid userId, Guid targetUserId);

}