using Common.DtoModels.Reason;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.ServiceInterfaces;

public interface IReasonService
{
    public Task<ReasonListModel?> GetAllUsersReasons(Guid userId);
    public Task<Guid?> CreateReason(
        Guid userId, IFormFileCollection files, 
        ReasonCreateModel createModel);

    public Task<ReasonModel?> GetReason(Guid userId, Guid reasonId);
}