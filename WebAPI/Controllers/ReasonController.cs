using System.Drawing;
using System.Drawing.Imaging;
using BusinessLogic.ServiceInterfaces;
using Common.DtoModels.Others;
using Common.DtoModels.Reason;
using Common.DtoModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace class_absences_backend.Controllers;

[ApiController]
[Route("api/reason")]
public class ReasonController : ControllerBase
{
    private readonly IReasonService _reasonService;
    private readonly ITokenService _tokenService;
    
    public ReasonController(IReasonService reasonService, ITokenService tokenService)
    {
        _reasonService = reasonService;
        _tokenService = tokenService;
    }
    
    private async Task<Guid> EnsureTokenIsValid()
    {
        var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        if (!await _tokenService.IsTokenValid(token))
        {
            throw new UnauthorizedAccessException();
        }
        
        return await _tokenService.GetUserIdFromToken(token);
    }
    
    /// <summary>
    /// Get user's reasons
    /// </summary>
    /// <response code="200">Success</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Forbidden</response>
    /// <response code="404">Not found</response>
    /// <response code="500">InternalServerError</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReasonListModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = null!)]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = null!)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = null!)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseModel))]
    [Authorize(Roles = "Student, Teacher")]
    [HttpGet("reasons")]
    public async Task<ActionResult<ReasonListModel>> GetAllReasons()
    {
        try
        {
            var userId = await EnsureTokenIsValid();
            var reasons = await _reasonService.GetAllUsersReasons(userId);

            if (reasons == null)
            {
                return NotFound(new ResponseModel
                {
                    Status = "404",
                    Message = "User was not found."
                });
            }

            return Ok(reasons);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception)
        {
            return Problem();
        }
    }
    
    /// <summary>
    /// Create a reason
    /// </summary>
    /// <response code="200">Success</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Forbidden</response>
    /// <response code="404">Not found</response>
    /// <response code="500">InternalServerError</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = null!)]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = null!)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = null!)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseModel))]
    [Authorize(Roles = "Student, Teacher")]
    [HttpPost("reason")]
    public async Task<ActionResult<ResponseModel>> CreateReason()
    {
        try
        {
            var userId = await EnsureTokenIsValid();
            var files = Request.Form.Files;
            var reasonCreateModel = new ReasonCreateModel
            {
                DateFrom = DateTime.Parse(Request.Form["dateFrom"].ToString()).ToUniversalTime(),
                DateTo = DateTime.Parse(Request.Form["dateTo"].ToString()).ToUniversalTime(),
                Description = Request.Form["description"].ToString()
            };
            
            var reasonId = await _reasonService.CreateReason(userId,
                (files.Count > 0 ? files : null)!, reasonCreateModel);

            if (reasonId == null)
            {
                return NotFound();
            }

            return Ok(reasonId);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception)
        {
            return Problem();
        }
    }
    
    /// <summary>
    /// Get the reason
    /// </summary>
    /// <response code="200">Success</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Not found</response>
    /// <response code="500">InternalServerError</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReasonModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = null!)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = null!)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseModel))]
    [Authorize(Roles = "Student, Teacher")]
    [HttpGet("reason/{reasonId:guid}")]
    public async Task<ActionResult<ReasonModel>> GetTheReason([FromRoute] Guid reasonId)
    {
        try
        {
            var userId = await EnsureTokenIsValid();
            var reason = await _reasonService.GetReason(userId, reasonId);

            if (reason == null)
            {
                return NotFound();
            }

            return Ok(reason);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception)
        {
            return Problem();
        }
    }
}