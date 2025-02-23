using System.ComponentModel.DataAnnotations;

namespace Common.DtoModels;

public class Response
{
    [Required]
    [MinLength(1)]
    public string Status { get; set; }

    [Required]
    [MinLength(1)]
    public string Message { get; set; }
}
