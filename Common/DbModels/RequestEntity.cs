namespace Common.DbModels;

public class RequestEntity(
    DateTime createTime,
    ReasonEntity reason,
    UserEntity user,
    string lessonName,
    DateTime absenceDateFrom,
    DateTime absenceDateTo)
{
    public DateTime CreateTime { get; set; } = createTime;
    public ReasonEntity Reason { get; set; } = reason;
    public RequestStatus Status { get; set; } = RequestStatus.Checking;
    public UserEntity User { get; set; } = user;
    public string LessonName { get; set; } = lessonName;
    public DateTime AbsenceDateFrom { get; set; } = absenceDateFrom;
    public DateTime AbsenceDateTo { get; set; } = absenceDateTo;
}