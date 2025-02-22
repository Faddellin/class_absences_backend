
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
        //private readonly ITokenService _tokenService;

        public RequestController(IRequestService requestService)
        {
            _requestService = requestService;
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

            Guid requestId = await _requestService.CreateRequest(requestCreateModel, userId);

            return Ok(requestId);
        }

        [HttpPut("/{requestId}")]
        public async Task<IActionResult> EditRequest(
                [FromBody] RequestCreateModel requestCreateModel,
                [FromRoute] Guid requestId)
        {

            await _requestService.EditRequest(requestCreateModel, requestId, userId);

            return Ok();
        }

        [HttpPut("/{requestId}/changeReasonOn/{reasonId}")]
        public async Task<IActionResult> ChangeRequestReason(
                [FromRoute] Guid requestId,
                [FromRoute] Guid reasonId)
        {

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

            RequestListModel requestListModel = await _requestService.GetAllRequests(sortType, requestStatus, userName, dateFrom, dateTo, userId);

            return Ok(requestListModel);
        }

        [HttpGet("/user/{targetUserId}")]
        public async Task<IActionResult> GetUserRequests(
                [FromQuery] SortType sortType,
                [FromQuery] RequestStatus? requestStatus,
                [FromQuery] DateTime? dateFrom,
                [FromQuery] DateTime? dateTo,
                [FromQuery] Guid targetUserId)
        {

            RequestListModel requestListModel = await _requestService.GetUserRequests(sortType, requestStatus, dateFrom, dateTo, userId, targetUserId);

            return Ok(requestListModel);
        }

    }
}
