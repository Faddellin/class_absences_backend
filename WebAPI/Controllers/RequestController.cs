﻿
using BusinessLogic.ServiceInterfaces;
using BusinessLogic.Services;
using Common.DbModels;
using Common.DtoModels;
using Common.DtoModels.Others;
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
        /// Create request
        /// </summary>
        /// <param name="requestCreateModel">requestCreateModel</param>
        /// <response code="200">Request was created</response>
        /// <response code="403">User doesn't have enough rights</response>
        /// <response code="400">Invalid arguments</response>
        /// <response code="500">Internal server error</response>
        [Authorize]
        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        [HttpPost]
        public async Task<ActionResult<Guid>> CreateRequest()
        {

            var userId = await EnsureTokenIsValid();
            var files = Request.Form.Files;
            
            var requestCreateModel = new RequestCreateModel()
            {
                AbsenceDateFrom = DateTime.Parse(Request.Form["absenceDateFrom"].ToString()).ToUniversalTime(),
                AbsenceDateTo = DateTime.Parse(Request.Form["absenceDateTo"].ToString()).ToUniversalTime(),
                Description = Request.Form["description"].ToString()
            };

            Guid requestId = await _requestService.CreateRequest(requestCreateModel, userId, files);

            return Ok(requestId);
        }

        /// <summary>
        /// Edit request
        /// </summary>
        /// <param name="requestCreateModel">requestCreateModel</param>
        /// <response code="200">Request was edited</response>
        /// <response code="403">User doesn't have enough rights</response>
        /// <response code="400">Invalid arguments</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        [Authorize]
        [HttpPut("{requestId}")]
        public async Task<IActionResult> EditRequest(
                [FromBody] RequestEditModel requestEditModel,
                [FromRoute] Guid requestId)
        {

            var userId = await EnsureTokenIsValid();

            await _requestService.EditRequest(requestEditModel, requestId, userId);

            return Ok();
        }

        // /// <summary>
        // /// Change request reason
        // /// </summary>
        // /// <response code="200">Request reason was changed</response>
        // /// <response code="403">User doesn't have enough rights</response>
        // /// <response code="400">Invalid arguments</response>
        // /// <response code="500">Internal server error</response>
        // [ProducesResponseType(typeof(ResponseModel), 500)]
        // [Authorize]
        // [HttpPut("reason/{requestId}")]
        // public async Task<IActionResult> ChangeRequestReason(
        //         [FromRoute] Guid requestId,
        //         [FromQuery] Guid reasonId)
        // {
        //
        //     var userId = await EnsureTokenIsValid();
        //
        //     await _requestService.ChangeRequestReason(reasonId, requestId, userId);
        //
        //     return Ok();
        // }

        /// <summary>
        /// Get all requests by filters
        /// </summary>
        /// <response code="200">All requests were returned by filters</response>
        /// <response code="403">User doesn't have enough rights</response>
        /// <response code="400">Invalid arguments</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(typeof(RequestListModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<RequestListModel>> GetRequestsByFilters(
                [FromQuery, DefaultValue(SortType.NameAsc)] SortType sortType,
                [FromQuery] RequestStatus? requestStatus,
                [FromQuery] string? userName,
                [FromQuery] DateTime? dateFrom,
                [FromQuery] DateTime? dateTo)
        {

            var userId = await EnsureTokenIsValid();

            RequestListModel requestListModel = await _requestService.GetAllRequests(sortType, requestStatus, userName, dateFrom, dateTo, userId);

            return Ok(requestListModel);
        }

        /// <summary>
        /// Get concrete user requests by filters
        /// </summary>
        /// <response code="200">All requests by concrete user were returned by filters</response>
        /// <response code="403">User doesn't have enough rights</response>
        /// <response code="400">Invalid arguments</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(typeof(RequestListModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        [Authorize]
        [HttpGet("user/{targetUserId}")]
        public async Task<ActionResult<RequestListModel>> GetUserRequests(
                [FromQuery, DefaultValue(SortType.NameAsc)] SortType sortType,
                [FromQuery] RequestStatus? requestStatus,
                [FromQuery] DateTime? dateFrom,
                [FromQuery] DateTime? dateTo,
                [FromRoute] Guid targetUserId)
        {

            var userId = await EnsureTokenIsValid();

            RequestListModel requestListModel = await _requestService.GetUserRequests(sortType, requestStatus, dateFrom, dateTo, userId, targetUserId);

            return Ok(requestListModel);
        }

        /// <summary>
        /// Get a concrete request
        /// </summary>
        /// <response code="200">Request by concrete id was returned</response>
        /// <response code="403">User doesn't have enough rights</response>
        /// <response code="400">Invalid arguments</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(typeof(RequestModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        [Authorize]
        [HttpGet("{requestId}")]
        public async Task<ActionResult<RequestModel>> GetRequest(
                [FromRoute] Guid requestId)
        {

            var userId = await EnsureTokenIsValid();

            RequestModel requestModel = await _requestService.GetRequest(requestId, userId);

            return Ok(requestModel);
        }
    }
}
