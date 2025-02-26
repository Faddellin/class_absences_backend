using BusinessLogic.ServiceInterfaces;
using Common.DbModels;
using Common.DtoModels;
using Common.DtoModels.Request;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Services;

public class RequestService : IRequestService
{
    private readonly AppDbContext _appDbContext;
    
    private readonly List<UserType> allRightsUsers = new List<UserType>() { UserType.Dean, UserType.Admin }; 

    public RequestService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }


    public async Task<Guid> CreateRequest(RequestCreateModel requestCreateModel, Guid userId, IFormFileCollection files)
    {
        UserEntity? userEntity = await _appDbContext.Users.FirstOrDefaultAsync(o => o.Id == userId);

        throwIfObjIsNull(userEntity);

        if (requestCreateModel.AbsenceDateFrom >= requestCreateModel.AbsenceDateTo)
        {
            throw new ArgumentException("AbsenceDateFrom can't be higher than AbsenceDateTo");
        }

        var userRequests = await _appDbContext.Requests
            .Where(r => r.User == userEntity)
            .ToListAsync();
        foreach (var request in userRequests)
        {
            if (requestCreateModel.AbsenceDateFrom > request.AbsenceDateFrom &&
                requestCreateModel.AbsenceDateFrom < request.AbsenceDateTo ||
                requestCreateModel.AbsenceDateTo > request.AbsenceDateFrom &&
                requestCreateModel.AbsenceDateTo < request.AbsenceDateTo)
            {
                throw new ArgumentException("Пересечение временного промежутка с какой-то другой заявкой");
            }
        }
        
        var fileNames = new List<string>();
        if (files.Count > 0)
        {
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine("static/images/reasons", fileName);
                    fileNames.Add(fileName);

                    using var stream = new MemoryStream();
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    using var image = new Bitmap(stream);
                    image.Save(filePath, ImageFormat.Jpeg);
                }
            }           
        }


        RequestEntity newRequest = new RequestEntity()
        {
            Id = new Guid(),
            CreateTime = DateTime.UtcNow,
            Description = requestCreateModel.Description,
            Status = RequestStatus.Checking,
            User = userEntity,
            Images = fileNames,
            AbsenceDateFrom = DateTime.SpecifyKind(requestCreateModel.AbsenceDateFrom, DateTimeKind.Utc),
            AbsenceDateTo = DateTime.SpecifyKind(requestCreateModel.AbsenceDateTo, DateTimeKind.Utc)
        };

        await _appDbContext.Requests.AddAsync(newRequest);
        await _appDbContext.SaveChangesAsync();

        return newRequest.Id;
    }



    public async Task EditRequest(RequestEditModel requestEditModel, Guid requestId, Guid userId)
    {
        UserEntity? userEntity = await _appDbContext.Users.FirstOrDefaultAsync(o => o.Id == userId);
        RequestEntity? requestEntity = await _appDbContext.Requests
            .Include(o => o.User)
            .Include(o => o.Checker)
            .FirstOrDefaultAsync(o => o.Id == requestId);

        throwIfObjIsNull(userEntity);
        throwIfObjIsNull(requestEntity);

        if (requestEntity != null && userEntity != requestEntity.User && !allRightsUsers.Contains(userEntity.UserType))
        {
            throw new AccessViolationException("User doesn't have enough rights");
        }

        if (requestEditModel.AbsenceDateFrom >= requestEditModel.AbsenceDateTo)
        {
            throw new ArgumentException("AbsenceDateFrom can't be higher than AbsenceDateTo");
        }

        if (requestEditModel.Status != null && userEntity.UserType == UserType.Student)
        {
            throw new AccessViolationException("User doesn't have enough rights");
        }

        if (requestEntity.Status is RequestStatus.Confirmed or RequestStatus.Rejected &&
            userEntity.UserType == UserType.Student)
        {
            throw new AccessViolationException("User cannot edit this request due to its status");
        }
        
        var userRequests = await _appDbContext.Requests
            .Where(r => r.User == requestEntity.User)
            .ToListAsync();
        foreach (var request in userRequests)
        {
            if ((requestEditModel.AbsenceDateFrom > request.AbsenceDateFrom &&
                requestEditModel.AbsenceDateFrom < request.AbsenceDateTo ||
                requestEditModel.AbsenceDateTo > request.AbsenceDateFrom &&
                requestEditModel.AbsenceDateTo < request.AbsenceDateTo) &&
                request != requestEntity)
            {
                throw new ArgumentException("Пересечение временного промежутка с какой-то другой заявкой");
            }
        }

        if (requestEditModel.Status != null)
        {
            requestEntity.Status = (RequestStatus)requestEditModel.Status;
            requestEntity.Checker = userEntity;
        }
        requestEntity.AbsenceDateFrom = DateTime.SpecifyKind(requestEditModel.AbsenceDateFrom, DateTimeKind.Utc);
        requestEntity.AbsenceDateTo = DateTime.SpecifyKind(requestEditModel.AbsenceDateTo, DateTimeKind.Utc);
        requestEntity.Description = requestEditModel.Description;

        await _appDbContext.SaveChangesAsync();
    }

    public async Task<RequestModel> AddImagesToRequest(Guid userId, Guid requestId, IFormFileCollection files)
    {
        var user = await _appDbContext.Users
            .FindAsync(userId);
        var request = await _appDbContext.Requests
            .Include(r => r.User)
            .Include(r => r.Checker)
            .FirstOrDefaultAsync(r => r.User == user && r.Id == requestId);
        
        throwIfObjIsNull(user);
        throwIfObjIsNull(request);

        if (request.Images.Count > 0)
        {
            throw new Exception("This request already has images");
        }
    
        var fileNames = new List<string>();
        if (files.Count > 0)
        {
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine("static/images/reasons", fileName);
                    fileNames.Add(fileName);

                    using var stream = new MemoryStream();
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    using var image = new Bitmap(stream);
                    image.Save(filePath, ImageFormat.Jpeg);
                }
            }           
        }
        
        request.Images.AddRange(fileNames);
        await _appDbContext.SaveChangesAsync();

        var requestModel = new RequestModel
        {
            AbsenceDateFrom = request.AbsenceDateFrom,
            AbsenceDateTo = request.AbsenceDateTo,
            CreateTime = request.CreateTime,
            FirstName = request.User.FirstName,
            Id = request.Id,
            Description = request.Description,
            LastName = request.User.LastName,
            MiddleName = request.User.MiddleName,
            Images = request.Images,
            Status = request.Status,
            UserId = request.User.Id,
            UserType = request.User.UserType,
            CheckerUsername = request.Checker == null? null:
                $"{request.Checker.LastName} {request.Checker.FirstName} {request.Checker.MiddleName}"
        };

        return requestModel;
    }

    public async Task<RequestListModel> GetAllRequests(SortType sortType, RequestStatus? requestStatus, string? userName,
                                                       DateTime? dateFrom, DateTime? dateTo, Guid userId)
    {
        UserEntity? userEntity = await _appDbContext.Users.FirstOrDefaultAsync(o => o.Id == userId);

        throwIfObjIsNull(userEntity);

        if (!allRightsUsers.Contains(userEntity.UserType))
        {
            throw new AccessViolationException("User doesn't have enough rights");
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
            (o.User.FirstName).ToLower().Contains(userName) &&
            (dateFrom == null || o.CreateTime >= dateFrom) &&
            (dateTo == null || o.CreateTime <= dateTo)).ToListAsync();


        List<RequestEntity> requestEntitySorted = SortRequestEntityList(sortType, requestEntityList);

        List<RequestShortModel> requestModelList = new List<RequestShortModel>();

        foreach (RequestEntity requestEntity in requestEntitySorted)
        {
            requestModelList.Add(new RequestShortModel()
            {
                CreateTime = requestEntity.CreateTime,
                Id = requestEntity.Id,
                Status = requestEntity.Status,
                Username = $"{requestEntity.User.LastName} {requestEntity.User.FirstName} {requestEntity.User.MiddleName}",
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
            .Include(o => o.Checker)
            .FirstOrDefaultAsync(o => o.Id == requestId);


        throwIfObjIsNull(userEntity);
        throwIfObjIsNull(requestEntity);

        if (userEntity != requestEntity.User && !allRightsUsers.Contains(userEntity.UserType))
        {
            throw new AccessViolationException("User doesn't have enough rights");
        }

        RequestModel requestModel = new RequestModel()
        {
            CreateTime = requestEntity.CreateTime,
            Id = requestEntity.Id,
            Status = requestEntity.Status,
            FirstName = requestEntity.User.FirstName,
            MiddleName = requestEntity.User.MiddleName,
            LastName = requestEntity.User.LastName,
            Description = requestEntity.Description,
            Images = requestEntity.Images,
            UserType = requestEntity.User.UserType,
            UserId = requestEntity.User.Id,
            AbsenceDateFrom = requestEntity.AbsenceDateFrom,
            AbsenceDateTo = requestEntity.AbsenceDateTo,
            CheckerUsername = requestEntity.Checker == null? null:
                $"{requestEntity.Checker.LastName} {requestEntity.Checker.FirstName} {requestEntity.Checker.MiddleName}"
                
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

        if (userEntity != targetUserEntity && !allRightsUsers.Contains(userEntity.UserType))
        {
            throw new AccessViolationException("User doesn't have enough rights");
        }

        if (dateFrom > dateTo)
        {
            throw new ArgumentException("DateFrom can't be higher than DateTo");
        }


        List<RequestEntity> requestEntityList = await _appDbContext.Requests
            .Include(o => o.User)
            .Where(o => (requestStatus == null || o.Status == requestStatus) &&
            o.User == targetUserEntity &&
            (dateFrom == null || o.CreateTime >= dateFrom) &&
            (dateTo == null || o.CreateTime <= dateTo)).ToListAsync();


        List<RequestEntity> requestEntitySorted = SortRequestEntityList(sortType, requestEntityList);

        List<RequestShortModel> requestModelList = new List<RequestShortModel>();

        foreach (RequestEntity requestEntity in requestEntitySorted)
        {
            requestModelList.Add(new RequestShortModel()
            {
                CreateTime = requestEntity.CreateTime,
                Id = requestEntity.Id,
                Status = requestEntity.Status,
                Username = $"{requestEntity.User.LastName} {requestEntity.User.FirstName} {requestEntity.User.MiddleName}",
                UserType = requestEntity.User.UserType,
                AbsenceDateFrom = requestEntity.AbsenceDateFrom,
                AbsenceDateTo = requestEntity.AbsenceDateTo
            });
        }

        RequestListModel requestListModel = new RequestListModel() { RequestsList = requestModelList };

        return requestListModel;
    }


    private List<RequestEntity> SortRequestEntityList(SortType sortType, List<RequestEntity> requestEntityList)
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