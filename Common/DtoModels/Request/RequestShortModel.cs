using Common.DtoModels.Request;
using Common.DtoModels;

namespace Common.DbModels;

public class RequestShortModel
{
    public DateTime CreateTime { get; set; }
    public Guid Id { get; set; }
    public Guid? ReasonId { get; set; }
    public RequestStatus Status { get; set; }
    public string Username {  get; set; }
    public UserType UserType { get; set; }
    public DateTime AbsenceDateFrom { get; set; }
    public DateTime AbsenceDateTo { get; set; }
}