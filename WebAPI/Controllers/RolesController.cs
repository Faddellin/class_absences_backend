
using BusinessLogic.ServiceInterfaces;
using BusinessLogic.Services;
using Common.DbModels;
using Common.DtoModels;
using Common.DtoModels.Others;
using Common.DtoModels.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Globalization;

namespace BusinessLogic.Controllers
{

    [ApiController]
    [Route("api/role")]

    public class RolesController : ControllerBase
    {

        private readonly IRolesService _rolesService;
        private readonly ITokenService _tokenService;

        public RolesController(IRolesService rolesService, ITokenService tokenService)
        {
            _rolesService = rolesService;
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
        /// Change concrete user role
        /// </summary>
        /// <param name="targetUserId">ID of the user whose status will be changed</param>
        /// <param name="userType">UserType</param>
        /// <response code="200">User role was changed</response>
        /// <response code="403">User doesn't have enough rights</response>
        /// <response code="400">Invalid arguments</response>
        /// <response code="500">Internal server error</response>
        [Authorize]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        [HttpPut("{targetUserId}")]
        public async Task<IActionResult> ChangeRole(
                [FromRoute] Guid targetUserId,
                [FromQuery] DateTime from,
                [FromQuery] DateTime to,
                [FromQuery] UserType userType)
        {

            var userId = await EnsureTokenIsValid();
            ;
            await _rolesService.ExportUserAbsencesInWord(from, to, new Guid("a31631e3-1ee5-4b8e-a997-458cd3aa6208"), new List<Guid> { targetUserId });
            await _rolesService.ChangeRole(userId, targetUserId, userType);

            return Ok();
        }



    }
}
