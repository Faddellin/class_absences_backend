using System.ComponentModel.DataAnnotations;

namespace Common.DtoModels.User;

public class UserRegisterModel
{
    [Required]
    [MinLength(1)]
    [MaxLength(1000)]
    public string Name { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    [MinLength(1)]
    public string Email { get; set; } = string.Empty;
    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;
}