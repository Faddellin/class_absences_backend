
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
using class_absences_backend.Controllers;

namespace BusinessLogic.Controllers
{

    [ApiController]
    [Route("api/role")]

    public class RolesController : ControllerBase
    {

        private readonly IRolesService _rolesService;

        public RolesController(IRolesService rolesService)
        {
            _rolesService = rolesService;
        }

        /// <summary>
        /// Add role to concrete user
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
        public async Task<IActionResult> AddRole(
                [FromRoute] Guid targetUserId,
                [FromQuery] UserType userType)
        {

            var userId = (Guid)HttpContext.Items["userId"];

            await _rolesService.AddRole(userId, targetUserId, userType);

            return Ok();
        }

        /// <summary>
        /// Remove role from concrete user
        /// </summary>
        /// <param name="targetUserId">ID of the user whose status will be changed</param>
        /// <param name="userType">UserType</param>
        /// <response code="200">User role was changed</response>
        /// <response code="403">User doesn't have enough rights</response>
        /// <response code="400">Invalid arguments</response>
        /// <response code="500">Internal server error</response>
        [Authorize]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        [HttpDelete("{targetUserId}")]
        public async Task<IActionResult> DeleteRole(
                [FromRoute] Guid targetUserId,
                [FromQuery] UserType userType)
        {

            var userId = (Guid)HttpContext.Items["userId"];

            await _rolesService.DeleteRole(userId, targetUserId, userType);

            return Ok();
        }

    }
}
