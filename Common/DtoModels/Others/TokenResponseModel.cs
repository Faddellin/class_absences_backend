using System.ComponentModel.DataAnnotations;

namespace Common.DtoModels.Others;

public class TokenResponseModel
{
    [Required]
    public string Token { get; set; }
}