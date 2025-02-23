using Common.DtoModels.Request;
using Common.DtoModels;
using System.ComponentModel.DataAnnotations;

namespace Common.DbModels;

public class RequestModel
{
    public DateTime CreateTime { get; set; }

    [Required]
    public Guid Id { get; set; }
    public Guid? ReasonId { get; set; }
    public RequestStatus Status { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    
    [Required]
    public string Username {  get; set; }
    
    [Required]
    public UserType UserType { get; set; }

    [Required]
    public Guid userId { get; set; }

    [Required]
    [MinLength(1)]
    public string lesson {  get; set; }
    public DateTime AbsenceDateFrom { get; set; }
    public DateTime AbsenceDateTo { get; set; }
}