using Common.DtoModels;

namespace Common.DbModels;

public class UserEntity
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public List<UserType> UserTypes { get; set; } = new List<UserType>() { UserType.Student};
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryDate { get; set; }
}