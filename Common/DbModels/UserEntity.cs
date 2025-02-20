namespace Common.DbModels;

public class UserEntity(Guid id, DateTime createTime, string name, string email, string passwordHash)
{
    public Guid Id { get; set; } = id;
    public DateTime CreateTime { get; set; } = createTime;
    public string Name { get; set; } = name;
    public string Email { get; set; } = email;
    public string PasswordHash { get; set; } = passwordHash;
}