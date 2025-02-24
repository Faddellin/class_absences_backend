using Common.DbModels;

namespace Common.DtoModels.Reason;

public class ReasonModel
{
    public Guid Id { get; set; }
    public DateTime CreateTime { get; set; }
    public List<string> Images { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public string Description { get; set; }
    public ReasonType ReasonType { get; set; }
}