using Common.DbModels;
using System.ComponentModel.DataAnnotations;

namespace Common.DtoModels.User;

public class UserModel
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    [MinLength(1)]
    public string Name { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    [MinLength(1)]
    public string Email { get; set; } = string.Empty;
    [Required]
    public UserType UserType { get; set; }
}