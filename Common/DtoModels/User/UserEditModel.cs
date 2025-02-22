using System.ComponentModel.DataAnnotations;

namespace Common.DtoModels.User;

public class UserEditModel
{
    [Required]
    [MinLength(8)]
    public string Password { get; set; }
}