
using BusinessLogic.ServiceInterfaces;
using BusinessLogic.Services;
using Common.DbModels;
using Common.DtoModels;
using Common.DtoModels.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace BusinessLogic.Controllers
{

    [ApiController]
    [Route("api/request")]

    public class RequestController : ControllerBase
    {

        private readonly IRequestService _requestService;
        private readonly ITokenService _tokenService;

        public RequestController(IRequestService requestService, ITokenService tokenService)
        {
            _requestService = requestService;
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
        /// Create request, if the reason is specified, then it must be the reason for this user
        /// </summary>
        /// <param name="requestCreateModel">Create model for request</param>
        /// <response code="200">Patients inspections list retrieved</response>
        [HttpPost]
        public async Task<IActionResult> CreateRequest(
                [FromBody] RequestCreateModel requestCreateModel)
        {

            var userId = await EnsureTokenIsValid();

            Guid requestId = await _requestService.CreateRequest(requestCreateModel, userId);

            return Ok(requestId);
        }

        [Authorize]
        [HttpPut("/{requestId}")]
        public async Task<IActionResult> EditRequest(
                [FromBody] RequestCreateModel requestCreateModel,
                [FromRoute] Guid requestId)
        {

            var userId = await EnsureTokenIsValid();

            await _requestService.EditRequest(requestCreateModel, requestId, userId);

            return Ok();
        }

        [Authorize]
        [HttpPut("/reason/{requestId}")]
        public async Task<IActionResult> ChangeRequestReason(
                [FromRoute] Guid requestId,
                [FromQuery] Guid reasonId)
        {

            var userId = await EnsureTokenIsValid();

            await _requestService.ChangeRequestReason(reasonId, requestId, userId);

            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> GetRequestsByFilters(
                [FromQuery] SortType sortType,
                [FromQuery] RequestStatus? requestStatus,
                [FromQuery] string? userName,
                [FromQuery] DateTime? dateFrom,
                [FromQuery] DateTime? dateTo)
        {

            var userId = await EnsureTokenIsValid();

            RequestListModel requestListModel = await _requestService.GetAllRequests(sortType, requestStatus, userName, dateFrom, dateTo, userId);

            return Ok(requestListModel);
        }

        [HttpGet("/user/{targetUserId}")]
        public async Task<IActionResult> GetUserRequests(
                [FromQuery] SortType sortType,
                [FromQuery] RequestStatus? requestStatus,
                [FromQuery] DateTime? dateFrom,
                [FromQuery] DateTime? dateTo,
                [FromRoute] Guid targetUserId)
        {

            var userId = await EnsureTokenIsValid();

            RequestListModel requestListModel = await _requestService.GetUserRequests(sortType, requestStatus, dateFrom, dateTo, userId, targetUserId);

            return Ok(requestListModel);
        }


        [HttpGet("/{requestId}")]
        public async Task<IActionResult> GetRequest(
                [FromRoute] Guid requestId)
        {

            var userId = await EnsureTokenIsValid();

            RequestModel requestModel = await _requestService.GetRequest(requestId, userId);

            return Ok(requestModel);
        }
    }
}
