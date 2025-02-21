namespace Common.DbModels;

public class Response(string status,
    string message)
{
    public string status { get; set; } = status;

    public string message { get; set; } = message;
}
