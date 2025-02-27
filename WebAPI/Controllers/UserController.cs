using System.Text.RegularExpressions;
using BusinessLogic.ServiceInterfaces;
using Common;
using Common.DtoModels.Others;
using Common.DtoModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace class_absences_backend.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : BaseController
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    
    public UserController(IUserService userService, ITokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Register new user
    /// </summary>
    /// <response code="200">User was registered</response>
    /// <response code="400">Invalid arguments</response>
    /// <response code="500">InternalServerError</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponseModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = null!)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseModel))]
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<TokenResponseModel>> Register([FromBody] UserRegisterModel model)
    {
        var isValid = ModelState.IsValid;
        
        if (!Regex.IsMatch(model.Email, RegexPatterns.Email))
        {
            ModelState.AddModelError("Email", 
                "Invalid email format");
            isValid = false;
        }
        
        if (!Regex.IsMatch(model.Password, RegexPatterns.Password))
        {
            ModelState.AddModelError("Password", 
                "Password requires at least one digit");
            isValid = false;
        }
        
        if (!isValid)
        {
            return BadRequest(ModelState);
        }

        var tokenResponse = await _userService.Register(model);
        return Ok(tokenResponse);
    }
    
    /// <summary>
    /// Log in to the system
    /// </summary>
    /// <response code="200">User logged in</response>
    /// <response code="400">Invalid arguments</response>
    /// <response code="500">InternalServerError</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponseModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = null!)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseModel))]
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseModel>> Login([FromBody] LoginCredentialsModel model)
    {
        var tokenResponse = await _userService.Login(model);
        if (tokenResponse.AccessToken.Equals(""))
        {
            return BadRequest(new ResponseModel
            {
                Status = "Error",
                Message = "Login failed"
            });
        }
        return Ok(tokenResponse);
    }
    
    /// <summary>
    /// Log out system user
    /// </summary>
    /// <response code="200">Success</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="500">InternalServerError</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = null!)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseModel))]
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        await _tokenService.AddInvalidToken(token);
        return Ok(new ResponseModel
        {
            Status = null,
            Message = "Logged out"
        });
    }
    
    /// <summary>
    /// Refresh user tokens
    /// </summary>
    /// <response code="200">Success</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="500">InternalServerError</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponseModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = null!)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseModel))]
    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<ActionResult<TokenResponseModel>> RefreshToken([FromBody]
        RefreshTokenRequestModel request)
    {
        var result = await _tokenService.RefreshTokens(request);
        if (result == null)
        {
            return BadRequest(new ResponseModel
            {
                Status = "400",
                Message = "Invalid refresh token"
            });
        }

        return Ok(result);
    }
    
    /// <summary>
    /// Get user profile
    /// </summary>
    /// <response code="200">Success</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Not found</response>
    /// <response code="500">InternalServerError</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = null!)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = null!)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseModel))]
    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<UserModel>> GetProfile()
    {
        var userId = GetUserId();
        
        var doctorModel = await _userService.GetProfile(userId);

        return Ok(doctorModel);
    }
    
    /// <summary>
    /// Edit user profile
    /// </summary>
    /// <response code="200">Success</response>
    /// <response code="400">Bad request</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Not found</response>
    /// <response code="500">InternalServerError</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = null!)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = null!)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = null!)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = null!)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseModel))]
    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> ChangeProfile([FromBody] UserEditModel model)
    {
        var userId = GetUserId();
        
        var isValid = ModelState.IsValid;
        
        if (!Regex.IsMatch(model.Password, RegexPatterns.Password))
        {
            ModelState.AddModelError("Password", 
                "Password requires at least one digit");
            isValid = false;
        }
        
        if (!isValid)
        {
            return BadRequest(ModelState);
        }

        await _userService.EditProfile(userId, model);
        return Ok();
    }
}