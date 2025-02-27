using Microsoft.AspNetCore.Mvc;

namespace class_absences_backend.Controllers;

public class BaseController : ControllerBase
{
    protected Guid GetUserId()
    {
        if (!HttpContext.Items.TryGetValue("userId", out var userIdObj) ||
            userIdObj is not Guid userId)
        {
            throw new UnauthorizedAccessException();
        }

        return userId;
    }
}