namespace Common.DtoModels;

public class Response(string status,
    string message)
{
    public string Status { get; set; }

    public string Message { get; set; }
}
