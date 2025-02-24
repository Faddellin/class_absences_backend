using Common.DtoModels.Reason;

namespace Common.DbModels;

public class ReasonEntity
{
    public DateTime CreateTime { get; set; }
    public Guid Id { get; set; }
    public List<string?>? Images { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public ReasonType ReasonType { get; set; }
    public UserEntity User { get; set; }
}