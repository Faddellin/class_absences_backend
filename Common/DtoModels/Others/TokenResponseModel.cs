using System.ComponentModel.DataAnnotations;

namespace Common.DtoModels.Others;

public class TokenResponseModel
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}