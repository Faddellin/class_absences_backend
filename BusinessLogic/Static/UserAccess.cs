using Common.DbModels;
using Common.DtoModels;

namespace BusinessLogic.Static;

public static class UserAccess
{
    private static readonly Dictionary<UserType, int> rolesAccess = new Dictionary<UserType, int>()
    {
        [UserType.Student] = 0,
        [UserType.Teacher] = 1,
        [UserType.Dean] = 2,
        [UserType.Admin] = 3
    };

    public static int GetUserAccesLevel(UserType userType)
    {
        int userAccessLevel;
        rolesAccess.TryGetValue(userType, out userAccessLevel);
        return userAccessLevel;
    }

}
