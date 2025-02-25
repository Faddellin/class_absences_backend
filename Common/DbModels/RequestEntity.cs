using Common.DtoModels.Request;

namespace Common.DbModels;

public class RequestEntity
{
    public Guid Id { get; set; }
    public DateTime CreateTime { get; set; }
    public List<string> Images { get; set; } = [];
    public RequestStatus Status { get; set; }
    public UserEntity User { get; set; }
    public UserEntity? Checker { get; set; }
    public string Description { get; set; }
    public DateTime AbsenceDateFrom { get; set; }
    public DateTime AbsenceDateTo { get; set; }
}