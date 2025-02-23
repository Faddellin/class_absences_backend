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

        bool reasonIsNotFounded = (requestCreateModel.reasonId != null && reasonEntity == null);

        if (reasonIsNotFounded)
        {
            throw new KeyNotFoundException("Reason is not found");
        }

        throwIfObjIsNull(userEntity);

        if (noRightsUsers.Contains(userEntity.UserType) ||
                (reasonEntity != null && userEntity != reasonEntity.User && !allRightsUsers.Contains(userEntity.UserType)))
        {
            throw new AccessViolationException("User doesn't have enough rights");
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

        bool reasonIsNotFounded = (requestCreateModel.reasonId != null && reasonEntity == null);

        if (reasonIsNotFounded)
        {
            throw new KeyNotFoundException("Reason is not found");
        }

        throwIfObjIsNull(userEntity);
        throwIfObjIsNull(requestEntity);

        if (noRightsUsers.Contains(userEntity.UserType) ||
                (reasonEntity != null && userEntity != reasonEntity.User && !allRightsUsers.Contains(userEntity.UserType)))
        {
            throw new AccessViolationException("User doesn't have enough rights");
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

        throwIfObjIsNull(userEntity);

        ReasonEntity? reasonEntity = await _appDbContext.Reasons
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == reasonId);

        throwIfObjIsNull(reasonEntity);

        RequestEntity? requestEntity = await _appDbContext.Requests
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == requestId);

        throwIfObjIsNull(requestEntity);

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

        throwIfObjIsNull(userEntity);

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
        else
        {
            userName = userName.ToLower();
        }


        List<RequestEntity> requestEntityList = await _appDbContext.Requests
            .Include(o => o.User)
            .Where(o => (requestStatus == null || o.Status == requestStatus) &&
            o.User.Name.ToLower().Contains(userName) &&
            (dateFrom == null || o.CreateTime >= dateFrom) &&
            (dateTo == null || o.CreateTime <= dateTo)).ToListAsync();


        List<RequestEntity> requestEntitySorted = sortRequestEntityList(sortType, requestEntityList);

        List<RequestShortModel> requestModelList = new List<RequestShortModel>();

        foreach (RequestEntity requestEntity in requestEntitySorted)
        {
            requestModelList.Add(new RequestShortModel()
            {
                CreateTime = requestEntity.CreateTime,
                Id = requestEntity.Id,
                ReasonId = requestEntity.Id,
                Status = requestEntity.Status,
                FirstName = requestEntity.User.FirstName,
                MiddleName = requestEntity.User.MiddleName,
                LastName = requestEntity.User.LastName,
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


        throwIfObjIsNull(userEntity);
        throwIfObjIsNull(requestEntity);

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
            FirstName = requestEntity.User.FirstName,
            MiddleName = requestEntity.User.MiddleName,
            LastName = requestEntity.User.LastName,
            UserType = requestEntity.User.UserType,
            userId = requestEntity.User.Id,
            lesson = requestEntity.LessonName,
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

        throwIfObjIsNull(userEntity);
        throwIfObjIsNull(targetUserEntity);

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
            .Where(o => (requestStatus == null || o.Status == requestStatus) &&
            o.User == userEntity &&
            (dateFrom == null || o.CreateTime >= dateFrom) &&
            (dateTo == null || o.CreateTime <= dateTo)).ToListAsync();


        List<RequestEntity> requestEntitySorted = sortRequestEntityList(sortType, requestEntityList);

        List<RequestShortModel> requestModelList = new List<RequestShortModel>();

        foreach (RequestEntity requestEntity in requestEntitySorted)
        {
            requestModelList.Add(new RequestShortModel()
            {
                CreateTime = requestEntity.CreateTime,
                Id = requestEntity.Id,
                ReasonId = requestEntity.Id,
                Status = requestEntity.Status,
                FirstName = requestEntity.User.FirstName,
                MiddleName = requestEntity.User.MiddleName,
                LastName = requestEntity.User.LastName,
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
                requestEntitySorted = requestEntityList.OrderBy(o => o.User.LastName).ToList();
                break;
            case SortType.NameDesc:
                requestEntitySorted = requestEntityList.OrderByDescending(o => o.User.LastName).ToList();
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

    private void throwIfObjIsNull<Object>(Object? obj)
    {
        if (obj == null)
        {
            throw new KeyNotFoundException($"{typeof(Object).Name} is not found");
        }
    }

}