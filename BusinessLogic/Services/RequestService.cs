using BusinessLogic.ServiceInterfaces;
using Common.DbModels;
using Common.DtoModels;
using Common.DtoModels.Request;
using Common.DtoModels.User;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessLogic.Services;

public class RequestService : IRequestService
{
    private readonly AppDbContext _appDbContext;
    
    private readonly List<UserType> noRightsUsers = new List<UserType>() { UserType.Unverified };
    private readonly List<UserType> allRightsUsers = new List<UserType>() { UserType.Dean, UserType.Admin }; 

    public RequestService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }


    public async Task<Guid> CreateRequest(RequestCreateModel requestCreateModel, Guid userId)
    {
        UserEntity? userEntity = await _appDbContext.Users.FirstOrDefaultAsync(o => o.Id == userId);
        ReasonEntity? reasonEntity = await _appDbContext.Reasons
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == requestCreateModel.reasonId); ;

        if (requestCreateModel.reasonId != null && reasonEntity == null)
        {
            throw new KeyNotFoundException("Reason is not found");
        }

        if (userEntity == null)
        {
            throw new KeyNotFoundException("User is not found");
        }

        if (reasonEntity != null)
        {
            if (noRightsUsers.Contains(userEntity.UserType) ||
                    (userEntity != reasonEntity.User && !allRightsUsers.Contains(userEntity.UserType)))
            {
                throw new AccessViolationException("User doesen't have enough rights");
            }
        }

        if (requestCreateModel.AbsenceDateFrom >= requestCreateModel.AbsenceDateTo)
        {
            throw new ArgumentException("AbsenceDateFrom can't be higher than AbsenceDateTo");
        }


        RequestEntity newRequest = new RequestEntity()
        {
            Id = new Guid(),
            CreateTime = DateTime.UtcNow,
            Reason = reasonEntity,
            Status = RequestStatus.Checking,
            User = userEntity,
            LessonName = requestCreateModel.LessonName,
            AbsenceDateFrom = DateTime.SpecifyKind(requestCreateModel.AbsenceDateFrom, DateTimeKind.Utc),
            AbsenceDateTo = DateTime.SpecifyKind(requestCreateModel.AbsenceDateTo, DateTimeKind.Utc)
        };

        await _appDbContext.Requests.AddAsync(newRequest);
        await _appDbContext.SaveChangesAsync();

        return newRequest.Id;
    }



    public async Task EditRequest(RequestCreateModel requestCreateModel, Guid requestId, Guid userId)
    {
        UserEntity? userEntity = await _appDbContext.Users.FirstOrDefaultAsync(o => o.Id == userId);
        ReasonEntity? reasonEntity = await _appDbContext.Reasons.FirstOrDefaultAsync(o => o.Id == requestCreateModel.reasonId);
        RequestEntity? requestEntity = await _appDbContext.Requests
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == requestId);

        if (userEntity == null)
        {
            throw new KeyNotFoundException("User is not found");
        }

        if (requestEntity == null)
        {
            throw new KeyNotFoundException("Request is not found");

        }

        if (noRightsUsers.Contains(userEntity.UserType) ||
            (userEntity != requestEntity.User && !allRightsUsers.Contains(userEntity.UserType)))
        {
            throw new AccessViolationException("User doesen't have enough rights");
        }

        if (requestCreateModel.AbsenceDateFrom >= requestCreateModel.AbsenceDateTo)
        {
            throw new ArgumentException("AbsenceDateFrom can't be higher than AbsenceDateTo");
        }



        requestEntity.AbsenceDateFrom = DateTime.SpecifyKind(requestCreateModel.AbsenceDateFrom, DateTimeKind.Utc);
        requestEntity.AbsenceDateTo = DateTime.SpecifyKind(requestCreateModel.AbsenceDateTo, DateTimeKind.Utc);
        requestEntity.LessonName = requestCreateModel.LessonName;
        requestEntity.Reason = reasonEntity;

        await _appDbContext.SaveChangesAsync();

        return;
    }

    public async Task ChangeRequestReason(Guid reasonId, Guid requestId, Guid userId)
    {
        UserEntity? userEntity = await _appDbContext.Users.FirstOrDefaultAsync(o => o.Id == userId);

        if (userEntity == null)
        {
            throw new KeyNotFoundException("User is not found");
        }

        ReasonEntity? reasonEntity = await _appDbContext.Reasons
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == reasonId);

        if (reasonEntity == null)
        {
            throw new KeyNotFoundException("Reason is not found");
        }

        RequestEntity? requestEntity = await _appDbContext.Requests
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == requestId);

        if (requestEntity == null)
        {
            throw new KeyNotFoundException("Request is not found");
        }

        if (noRightsUsers.Contains(userEntity.UserType) ||
            (userEntity != requestEntity.User || userEntity != reasonEntity.User) && !allRightsUsers.Contains(userEntity.UserType))
        {
            throw new AccessViolationException("User doesen't have enough rights");
        }

        requestEntity.Reason = reasonEntity;
        await _appDbContext.SaveChangesAsync();

        return;
    }


    public async Task<RequestListModel> GetAllRequests(SortType sortType, RequestStatus? requestStatus, string? userName,
                                                       DateTime? dateFrom, DateTime? dateTo, Guid userId)
    {
        UserEntity? userEntity = await _appDbContext.Users.FirstOrDefaultAsync(o => o.Id == userId);

        if (userEntity == null)
        {
            throw new KeyNotFoundException("User is not found");
        }

        if (!allRightsUsers.Contains(userEntity.UserType))
        {
            throw new AccessViolationException("User doesen't have enough rights");
        }

        if (dateFrom > dateTo)
        {
            throw new ArgumentException("DateFrom can't be higher than DateTo");
        }

        if (userName == null)
        {
            userName = "";
        }


        List<RequestEntity> requestEntityList = await _appDbContext.Requests
            .Include(o => o.User)
            .Where(o => o.Status == requestStatus &&
            o.User.Name.Contains(userName) &&
            o.CreateTime >= dateFrom &&
            o.CreateTime <= dateTo).ToListAsync();


        List<RequestEntity> requestEntitySorted = sortRequestEntityList(sortType, requestEntityList);

        List<RequestModel> requestModelList = new List<RequestModel>();

        foreach (RequestEntity requestEntity in requestEntitySorted)
        {
            requestModelList.Add(new RequestModel()
            {
                CreateTime = requestEntity.CreateTime,
                Id = requestEntity.Id,
                ReasonId = requestEntity.Id,
                Status = requestEntity.Status,
                Username = requestEntity.User.Name,
                UserType = requestEntity.User.UserType,
                AbsenceDateFrom = requestEntity.AbsenceDateFrom,
                AbsenceDateTo = requestEntity.AbsenceDateTo
            });
        }

        RequestListModel requestListModel = new RequestListModel() { RequestsList = requestModelList };

        return requestListModel;
    }

    public async Task<RequestModel> GetRequest(Guid requestId, Guid userId)
    {
        UserEntity? userEntity = await _appDbContext.Users.FirstOrDefaultAsync(o => o.Id == userId);
        RequestEntity? requestEntity = await _appDbContext.Requests
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == requestId);


        if (userEntity == null)
        {
            throw new KeyNotFoundException("User is not found");
        }

        if (requestEntity == null)
        {
            throw new KeyNotFoundException("Request is not found");
        }

        if (noRightsUsers.Contains(userEntity.UserType) ||
            (userEntity != requestEntity.User && !allRightsUsers.Contains(userEntity.UserType)))
        {
            throw new AccessViolationException("User doesen't have enough rights");
        }

        RequestModel requestModel = new RequestModel()
        {
            CreateTime = requestEntity.CreateTime,
            Id = requestEntity.Id,
            ReasonId = requestEntity.Id,
            Status = requestEntity.Status,
            Username = requestEntity.User.Name,
            UserType = requestEntity.User.UserType,
            AbsenceDateFrom = requestEntity.AbsenceDateFrom,
            AbsenceDateTo = requestEntity.AbsenceDateTo
        };

        return requestModel;
    }


    public async Task<RequestListModel> GetUserRequests(SortType sortType, RequestStatus? requestStatus,
                                                       DateTime? dateFrom, DateTime? dateTo, Guid userId, Guid targetUserId)
    {
        UserEntity? userEntity = await _appDbContext.Users.FirstOrDefaultAsync(o => o.Id == userId);

        UserEntity? targetUserEntity = await _appDbContext.Users.FirstOrDefaultAsync(o => o.Id == targetUserId);

        if (userEntity == null)
        {
            throw new KeyNotFoundException("User is not found");
        }

        if (targetUserEntity == null)
        {
            throw new KeyNotFoundException("TargetUser is not found");
        }

        if (noRightsUsers.Contains(userEntity.UserType) ||
            (userEntity != targetUserEntity && !allRightsUsers.Contains(userEntity.UserType)))
        {
            throw new AccessViolationException("User doesen't have enough rights");
        }

        if (dateFrom > dateTo)
        {
            throw new ArgumentException("DateFrom can't be higher than DateTo");
        }


        List<RequestEntity> requestEntityList = await _appDbContext.Requests
            .Include(o => o.User)
            .Where(o => o.Status == requestStatus &&
            o.User == userEntity &&
            o.CreateTime >= dateFrom &&
            o.CreateTime <= dateTo).ToListAsync();


        List<RequestEntity> requestEntitySorted = sortRequestEntityList(sortType, requestEntityList);

        List<RequestModel> requestModelList = new List<RequestModel>();

        foreach (RequestEntity requestEntity in requestEntitySorted)
        {
            requestModelList.Add(new RequestModel()
            {
                CreateTime = requestEntity.CreateTime,
                Id = requestEntity.Id,
                ReasonId = requestEntity.Id,
                Status = requestEntity.Status,
                Username = requestEntity.User.Name,
                UserType = requestEntity.User.UserType,
                AbsenceDateFrom = requestEntity.AbsenceDateFrom,
                AbsenceDateTo = requestEntity.AbsenceDateTo
            });
        }

        RequestListModel requestListModel = new RequestListModel() { RequestsList = requestModelList };

        return requestListModel;
    }


    private List<RequestEntity> sortRequestEntityList(SortType sortType, List<RequestEntity> requestEntityList)
    {
        List<RequestEntity> requestEntitySorted = new List<RequestEntity>();

        switch (sortType)
        {
            case SortType.NameAsc:
                requestEntitySorted = requestEntityList.OrderBy(o => o.User.Name).ToList();
                break;
            case SortType.NameDesc:
                requestEntitySorted = requestEntityList.OrderByDescending(o => o.User.Name).ToList();
                break;
            case SortType.CreateAsc:
                requestEntitySorted = requestEntityList.OrderBy(o => o.CreateTime).ToList();
                break;
            case SortType.CreateDesc:
                requestEntitySorted = requestEntityList.OrderByDescending(o => o.CreateTime).ToList();
                break;
            default:
                requestEntitySorted = requestEntityList;
                break;
        }

        return requestEntitySorted;
    }

}