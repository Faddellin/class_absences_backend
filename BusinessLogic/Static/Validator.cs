using Common.DbModels;
using Common.DtoModels;
using Common.DtoModels.Request;

namespace BusinessLogic.Static;

public static class Validator
{
    public static void ThrowIfNull<Object>(Object? obj)
    {
        if (obj == null)
        {
            throw new KeyNotFoundException($"{typeof(Object).Name} is not found");
        }
    }
    public static void ThrowIfNull<Object>(Object? obj, string text)
    {
        if (obj == null)
        {
            throw new KeyNotFoundException(text);
        }
    }
    public static void ThrowIfNotEnoughAccess(UserType userType, int accesLevel)
    {
        if (UserAccess.GetUserAccesLevel(userType) < accesLevel)
        {
            throw new AccessViolationException("User doesn't have enough rights");
        }
    }
    public static void ThrowIfNotEnoughAccess(UserType userType, UserType targetUserType)
    {
        if (UserAccess.GetUserAccesLevel(userType) < UserAccess.GetUserAccesLevel(targetUserType))
        {
            throw new AccessViolationException("User doesn't have enough rights");
        }
    }

    public static void ThrowIfFirstDateHigherThanSecond(DateTime? dateFrom, DateTime? dateTo)
    {
        if (dateFrom > dateTo)
        {
            throw new ArgumentException("DateFrom can't be higher than DateTo");
        }
    }

    public static void ThrowIfRequestIntersectAnyOtherRequest(RequestEntity request, List<RequestEntity> otherRequests)
    {
        foreach (var otherRequest in otherRequests)
        {
            if (request.AbsenceDateFrom > otherRequest.AbsenceDateFrom &&
                request.AbsenceDateFrom < otherRequest.AbsenceDateTo ||
                request.AbsenceDateTo > otherRequest.AbsenceDateFrom &&
                request.AbsenceDateTo < otherRequest.AbsenceDateTo)
            {
                throw new ArgumentException("Request date range intersects with other request from this user");
            }
        }
    }

    public static void ThrowIfRequestIntersectAnyOtherRequest(RequestCreateModel request, List<RequestEntity> otherRequests)
    {
        foreach (var otherRequest in otherRequests)
        {
            if (request.AbsenceDateFrom > otherRequest.AbsenceDateFrom &&
                request.AbsenceDateFrom < otherRequest.AbsenceDateTo ||
                request.AbsenceDateTo > otherRequest.AbsenceDateFrom &&
                request.AbsenceDateTo < otherRequest.AbsenceDateTo)
            {
                throw new ArgumentException("Request date range intersects with other request from this user");
            }
        }
    }

    public static void ThrowIfRequestIntersectAnyOtherRequest(RequestEditModel request, List<RequestEntity> otherRequests)
    {
        foreach (var otherRequest in otherRequests)
        {
            if (request.AbsenceDateFrom > otherRequest.AbsenceDateFrom &&
                request.AbsenceDateFrom < otherRequest.AbsenceDateTo ||
                request.AbsenceDateTo > otherRequest.AbsenceDateFrom &&
                request.AbsenceDateTo < otherRequest.AbsenceDateTo)
            {
                throw new ArgumentException("Request date range intersects with other request from this user");
            }
        }
    }
}
