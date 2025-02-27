namespace Common;

public class TokenPayload
{
    public string user_id { get; set; }
    public string token_id { get; set; }
    public string role { get; set; }
    public int nbf { get; set; }
    public int exp { get; set; }
    public int iat { get; set; }
    public string iss { get; set; }
    public string aud { get; set; }
}