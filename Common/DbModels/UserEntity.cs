namespace Common.DbModels;

public class UserEntity(Guid id, 
    string name, 
    string email, 
    string passwordHash, 
    UserType userType)
{
    public Guid Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string Email { get; set; } = email;
    public string PasswordHash { get; set; } = passwordHash;
    public UserType UserType { get; set; } = userType;
}