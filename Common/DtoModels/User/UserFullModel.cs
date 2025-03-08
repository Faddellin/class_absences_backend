using Common.DtoModels.Request;

namespace Common.DtoModels.User;

public class UserFullModel
{
    public UserModel User { get; set; }
    public List<RequestModel> Requests { get; set; } = [];
}