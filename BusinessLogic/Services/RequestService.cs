using BusinessLogic.ServiceInterfaces;
using Common.DbModels;
using Common.DtoModels;
using Common.DtoModels.Request;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BusinessLogic.Services;

public class RequestService : IRequestService
{
    private readonly AppDbContext _appDbContext;

    public RequestService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<Guid> CreateRequest(RequestCreateModel requestCreateModel, Guid userId, Guid? reasonId)
    {
        UserEntity? userEntity = await _appDbContext.Users.FirstOrDefaultAsync(o => o.Id == userId);
        ReasonModel? reasonEntity = await _appDbContext.Reasons.FirstOrDefaultAsync(o => o.Id == reasonId); ;


        if (userEntity == null)
        {
            throw new KeyNotFoundException("User is not found");
        }

        if (requestCreateModel.AbsenceDateFrom < requestCreateModel.AbsenceDateTo)
        {
            throw new ArgumentException("AbsenceDateFrom can't be lower than AbsenceDateTo");
        }

        RequestEntity newRequest = new RequestEntity()
        {
            Id = new Guid(),
            CreateTime = DateTime.Now,
            Reason = reasonEntity,
            Status = RequestStatus.Checking,
            User = userEntity,
            LessonName = requestCreateModel.LessonName,
            AbsenceDateFrom = requestCreateModel.AbsenceDateFrom,
            AbsenceDateTo = requestCreateModel.AbsenceDateTo
        };

        await _appDbContext.Requests.AddAsync(newRequest);
        await _appDbContext.SaveChangesAsync();

        return newRequest.Id;
    }



    public async Task<RequestListModel> GetAllRequests(SortType sortType, RequestStatus? requestStatus, string? userName)
    {

        if (userName == null)
        {
            userName = "";
        }


        List<RequestEntity> requestEntityList = await _appDbContext.Requests
            .Include(o => o.User)
            .Where(o => o.Status == requestStatus && o.User.Name.Contains(userName)).ToListAsync();

        
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

}