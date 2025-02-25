using System.ComponentModel.DataAnnotations;

namespace Common.DtoModels.Request;

public class RequestEditModel
{
    public RequestStatus? Status { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public DateTime AbsenceDateFrom { get; set; }
    [Required]
    public DateTime AbsenceDateTo { get; set; }
}