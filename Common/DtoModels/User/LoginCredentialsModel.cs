using System.ComponentModel.DataAnnotations;

namespace Common.DtoModels.User;

public class LoginCredentialsModel
{
    [Required]
    [MinLength(1)]
    [EmailAddress]
    public string? Email { get; set; } = string.Empty;
    [Required]
    [MinLength(1)]
    public string? Password { get; set; } = string.Empty;
}