using System.ComponentModel.DataAnnotations;

namespace Common.DtoModels.Request;

public class RequestModel
{
    public DateTime CreateTime { get; set; }

    [Required]
    public Guid Id { get; set; }
    public RequestStatus Status { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string? CheckerUsername { get; set; }
    public string Description { get; set; }
    public List<string> Images { get; set; } = [];

    [Required]
    public Guid UserId { get; set; }
    public DateTime AbsenceDateFrom { get; set; }
    public DateTime AbsenceDateTo { get; set; }
}