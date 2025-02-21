namespace Common.DbModels;

public class ReasonModel
{
    public Guid Id { get; set; }
    public List<byte[]> Images { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public string Description { get; set; }
    public ReasonType ReasonType { get; set; }
    public UserEntity User { get; set; }
}