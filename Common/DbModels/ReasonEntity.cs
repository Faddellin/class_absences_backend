namespace Common.DbModels;

public class ReasonEntity
{
    public ReasonEntity(List<byte[]> images, DateTime dateFrom, DateTime dateTo, string description, ReasonType reasonType, UserEntity user)
    {
        Images = images;
        DateFrom = dateFrom;
        DateTo = dateTo;
        Description = description;
        ReasonType = reasonType;
        User = user;
    }

    public List<byte[]> Images { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public string Description { get; set; }
    public ReasonType ReasonType { get; set; }
    public UserEntity User { get; set; }
}