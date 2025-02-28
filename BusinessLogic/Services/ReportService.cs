using Common.DbModels;
using Common.DtoModels.Request;
using Common.DtoModels;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using Microsoft.EntityFrameworkCore;
using BusinessLogic.ServiceInterfaces;
using BusinessLogic.Static;
using Common.DtoModels.User;

namespace BusinessLogic.Services;

public class ReportService : IReportService
{
    private readonly AppDbContext _appDbContext;
    public ReportService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    private class Pair<T1, T2>
    {
        public T1 First { get; set; }
        public T2 Second { get; set; }
        public Pair(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }
    }
    private class segmentOfAbsence
    {
        public DateTime startDate;
        public DateTime endDate;
        public RequestStatus reason;
    }

    public async Task<MemoryStream> ExportUserAbsencesInWord(DateTime dateFrom, DateTime dateTo,
                                               Guid userId, List<Guid> targetUserId)
    {
        Validator.ThrowIfFirstDateHigherThanSecond(dateFrom, dateTo);

        UserEntity? userEntity = await _appDbContext.Users.FirstOrDefaultAsync(o => o.Id == userId);
        Validator.ThrowIfNull(userEntity);
        Validator.ThrowIfNotEnoughAccess(userEntity.UserType, 1);
        Validator.ThrowIfFirstDateHigherThanSecond(dateFrom, dateTo);

        List<UserEntity>? targetUserEntity = await _appDbContext.Users.Where(o => targetUserId.Contains(o.Id)).ToListAsync();

        var missingIds = targetUserId.Except(targetUserEntity.Select(e => e.Id)).ToList();
        if (missingIds.Any())
        {
            throw new KeyNotFoundException($"Users with IDs ({string.Join(", ", missingIds)}) are not found");
        }
        
        List<Pair<UserEntity, List<RequestEntity>>> pairsOfUserAndTheirAsences = new List<Pair<UserEntity, List<RequestEntity>>>();

        foreach (var user in targetUserEntity)
        {
            List<RequestEntity> usersRequests = await _appDbContext.Requests
            .Include(o => o.User)
            .Where(o => o.User == user &&
            (o.AbsenceDateTo >= dateFrom) &&
            (o.AbsenceDateFrom <= dateTo)).ToListAsync();

            pairsOfUserAndTheirAsences.Add(new Pair<UserEntity, List<RequestEntity>>(user, usersRequests));
        }

        using (MemoryStream memoryStream = new MemoryStream())
        {
            CreateWordprocessingDocument(memoryStream, pairsOfUserAndTheirAsences, dateFrom, dateTo);
            memoryStream.Position = 0;

            return memoryStream;
        }
    }

    public async Task<UserListModel?> GetAllUsers(Guid userId)
    {
        var user = await _appDbContext.Users.FindAsync(userId);
        Validator.ThrowIfNull(user);
        Validator.ThrowIfNotEnoughAccess(user.UserType, 1);

        var users = await _appDbContext.Users
            .Where(u => u.UserType != UserType.Admin).ToListAsync();
        var userModels = new UserListModel
        {
            UsersList = []
        };
        foreach (var userEntity in users)
        {
            var userModel = new UserModel
            {
                Email = userEntity.Email,
                FirstName = userEntity.FirstName,
                Id = userEntity.Id,
                LastName = userEntity.LastName,
                MiddleName = userEntity.MiddleName,
                UserType = userEntity.UserType
            };
            userModels.UsersList?.Add(userModel);
        }

        return userModels;
    }

    void CreateWordprocessingDocument(MemoryStream memoryStream, List<Pair<UserEntity, List<RequestEntity>>> pairsOfUserAndTheirAsences,
                                    DateTime dateFrom, DateTime dateTo)
    {

        using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document))
        {
            MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

            mainPart.Document = new Document();
            Body body = mainPart.Document.AppendChild(new Body());

            AddParagraphToBody(body, $"Период даты отчёта: {dateFrom} ------ {dateTo}.", "36");

            foreach (var userAbsencesPair in pairsOfUserAndTheirAsences)
            {
                UserEntity user = userAbsencesPair.First;
                List<RequestEntity>? userAbsences = userAbsencesPair.Second;

                string userType = "";

                switch (user.UserType)
                {
                    case UserType.Student:
                        userType = "Студент";
                        break;
                    case UserType.Teacher:
                        userType = "Преподаватель";
                        break;
                    case UserType.Dean:
                        userType = "Декан";
                        break;
                    case UserType.Admin:
                        userType = "Admin";
                        break;
                }

                userType += $" {user.FirstName} {user.MiddleName} {user.LastName}.";

                AddParagraphToBody(body, userType, "30");

                if (userAbsences.Count() == 0)
                {
                    AddParagraphToBody(body, "Нет пропусков за данный период.", "26");
                }
                else
                {
                    List<segmentOfAbsence> segmentsOfDates = divideTimePeriodIntoSkipSegments(dateFrom, dateTo, userAbsences);

                    foreach (var date in segmentsOfDates)
                    {
                        string reasonType = date.reason == RequestStatus.Confirmed ? "уважительной" : "неуважительной";
                        AddParagraphToBody(body, $"Отсутствовал с {date.startDate.ToLocalTime()} по {date.endDate.ToLocalTime()} по {reasonType} причине", "26");

                    }
                }
            }

        }
    }

    void AddParagraphToBody(Body body, string text, string fontSize)
    {
        Paragraph para = body.AppendChild(new Paragraph());

        Run run = para.AppendChild(new Run());

        RunProperties properties = new RunProperties();
        properties.FontSize = new FontSize() { Val = fontSize };
        run.Append(properties);

        run.AppendChild(new Text(text));
    }

    private List<segmentOfAbsence> divideTimePeriodIntoSkipSegments(DateTime dateFrom, DateTime dateTo, List<RequestEntity> requestEntityList)
    {
        List<RequestEntity> requestsSorted = requestEntityList.OrderBy(o => o.AbsenceDateTo).ToList();

        List<segmentOfAbsence> segmentsOfDates = new List<segmentOfAbsence>();

        int curIndex = 0;
        DateTime curTime = requestsSorted[0].AbsenceDateFrom < dateFrom ? dateFrom : requestsSorted[0].AbsenceDateFrom;

        while (curIndex < requestsSorted.Count())
        {
            curTime = requestsSorted[curIndex].AbsenceDateFrom;

            DateTime nextTime = requestsSorted[curIndex].AbsenceDateTo > dateTo ? dateTo : requestsSorted[curIndex].AbsenceDateTo;

            segmentsOfDates.Add(new segmentOfAbsence()
            {
                startDate = curTime,
                endDate = nextTime,
                reason = requestsSorted[curIndex].Status
            });

            curIndex++;


        }

        return segmentsOfDates;
    }
}
