using Aspose.Words;
using Aspose.Words.Bibliography;
using BusinessLogic.ServiceInterfaces;
using Common.DbModels;
using Common.DtoModels;
using Common.DtoModels.Request;
using Common.DtoModels.User;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessLogic.Services;

public class RolesService : IRolesService
{
    private readonly AppDbContext _appDbContext;
    
    private readonly Dictionary<UserType, int> rolesAccess = new Dictionary<UserType, int>()
    {
        [UserType.Student] = 0,
        [UserType.Teacher] = 1,
        [UserType.Dean] = 2,
        [UserType.Admin] = 3
    };

    public RolesService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }


    public async Task ChangeRole(Guid userId, Guid targetUserId, UserType newUserType)
    {
        UserEntity? userEntity = await _appDbContext.Users.FirstOrDefaultAsync(o => o.Id == userId);
        UserEntity? targetUserEntity = await _appDbContext.Users.FirstOrDefaultAsync(o => o.Id == targetUserId);

        throwIfObjIsNull(userEntity);
        throwIfObjIsNull(targetUserEntity);

        int userAccessLevel;
        int targetUserAccessLevel;
        int newUserTypeAccessLevel;
        rolesAccess.TryGetValue(userEntity.UserType, out userAccessLevel);
        rolesAccess.TryGetValue(targetUserEntity.UserType, out targetUserAccessLevel);
        rolesAccess.TryGetValue(newUserType, out newUserTypeAccessLevel);

        if ((targetUserAccessLevel >= userAccessLevel) ||
            (newUserTypeAccessLevel >= userAccessLevel))
        {
            throw new AccessViolationException("User doesn't have enough rights");
        }

        targetUserEntity.UserType = newUserType;
        await _appDbContext.SaveChangesAsync();

        return;
    }

    public class Pair<T1, T2>
    {
        public T1 First { get; set; }
        public T2 Second { get; set; }
        public Pair(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }
    }

    public async Task ExportUserAbsencesInWord(DateTime dateFrom, DateTime dateTo,
                                                            Guid userId, List<Guid> targetUserId)
    {
        UserEntity? userEntity = await _appDbContext.Users.FirstOrDefaultAsync(o => o.Id == userId);

        List<UserEntity>? targetUserEntity = await _appDbContext.Users.Where(o => targetUserId.Contains(o.Id)).ToListAsync();

        foreach (var targetUser in targetUserEntity)
        {
            if (!targetUserId.Contains(targetUser.Id))
            {
                throw new KeyNotFoundException($"User ({targetUser.Id}) is not found");
            }
        }

        throwIfObjIsNull(userEntity);

        int userAccessLevel;
        rolesAccess.TryGetValue(userEntity.UserType, out userAccessLevel);

        //if (targetUserAccessLevel >= 1)
        //{
        //    throw new AccessViolationException("User doesn't have enough rights");
        //}

        if (dateFrom > dateTo)
        {
            throw new ArgumentException("DateFrom can't be higher than DateTo");
        }



        List<Pair<UserEntity, List<RequestEntity>>> pairsOfUserAndTheirRequests = new List<Pair<UserEntity, List<RequestEntity>>>();

        foreach (var user in targetUserEntity)
        {
            List<RequestEntity> usersRequests = await _appDbContext.Requests
            .Include(o => o.User)
            .Where(o => o.User == user &&
            (o.AbsenceDateTo >= dateFrom) &&
            (o.AbsenceDateFrom <= dateTo)).ToListAsync();

            pairsOfUserAndTheirRequests.Add(new Pair<UserEntity, List<RequestEntity>>(user, usersRequests));
        }

        List<string> documentsStrings = new List<string>();

        var doc = new Document();
        var builder = new DocumentBuilder(doc);

        foreach (var pair in pairsOfUserAndTheirRequests)
        {
            List<segmentOfAbsence> segmentsOfDates = divideTimePeriodIntoSkipSegments(dateFrom, dateTo, pair.Second);

            foreach (var dateSegment in segmentsOfDates)
            {
                string reasonOfAbsence = dateSegment.reason == RequestStatus.Confirmed ? "Уважительная" : "Неуважительная";
                documentsStrings.Add($"Период: {dateSegment.startDate} ----> {dateSegment.endDate} \n" +
                        $"Причина пропуска: {reasonOfAbsence}");
            }

            foreach (string docString in documentsStrings)
            {
                builder.Write(docString);
            }

        }

        doc.Save("Output.docx");

    }

    public class segmentOfAbsence
    {
        public DateTime startDate;
        public DateTime endDate;
        public RequestStatus reason;
    }

    private List<segmentOfAbsence> divideTimePeriodIntoSkipSegments(DateTime dateFrom, DateTime dateTo, List<RequestEntity> requestEntityList)
    {
        List<RequestEntity> requestsSorted = requestEntityList.OrderBy(o => o.AbsenceDateTo).ToList();

        List<segmentOfAbsence> segmentsOfDates = new List<segmentOfAbsence>();

        int curIndex = 0;
        DateTime curTime = requestsSorted[0].AbsenceDateFrom < dateFrom ? dateFrom : requestsSorted[0].AbsenceDateFrom;
        //out of range
        while (curIndex < requestsSorted.Count())
        {

            DateTime nextTime = requestsSorted[curIndex].AbsenceDateTo > dateTo ? dateTo : requestsSorted[curIndex].AbsenceDateTo;

            segmentsOfDates.Add(new segmentOfAbsence()
            {
                startDate = curTime,
                endDate = nextTime,
                reason = requestsSorted[curIndex].Status
            });

            curIndex++;
            if (curIndex < requestsSorted.Count())
            {
                curTime = requestsSorted[curIndex].AbsenceDateFrom;
            }


        }

        return segmentsOfDates;
    }

    private void throwIfObjIsNull<Object>(Object? obj)
    {
        if (obj == null)
        {
            throw new KeyNotFoundException($"{typeof(Object).Name} is not found");
        }
    }

}