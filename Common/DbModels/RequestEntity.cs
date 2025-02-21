namespace Common.DbModels;

public class RequestEntity
{
    public Guid Id { get; set; }
    public DateTime CreateTime { get; set; }
    public ReasonEntity Reason { get; set; }
    public RequestStatus Status { get; set; } = RequestStatus.Checking;
    public UserEntity User { get; set; }
    public string LessonName { get; set; }
    public DateTime AbsenceDateFrom { get; set; }
    public DateTime AbsenceDateTo { get; set; }
}