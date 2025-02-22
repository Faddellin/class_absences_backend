namespace Common;

public static class RegexPatterns
{
    public const string Email = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
    public const string Password = @"^(?=.*\d).+$";
}