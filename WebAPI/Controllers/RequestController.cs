
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
    [Route("api")]

    public class RequestController : ControllerBase
    {

        private readonly IRequestService _requestService;
        //private readonly ITokenService _tokenService;

        public RequestController(IRequestService requestService)
        {
            _requestService = requestService;
        }


        [Authorize]
        [HttpPost]
        [Route("request")]
        public async Task<IActionResult> CreateRequest(
                [FromBody] RequestCreateModel requestCreateModel,
                [FromBody] Guid? reasonId)
        {

            Guid requestId = await _requestService.CreateRequest(requestCreateModel,new Guid("4e19c13d-cab0-467b-8776-423eaee61f2aZ"), reasonId);

            return Ok(requestId);
        }


        [Authorize]
        [HttpGet]
        [Route("request")]
        public async Task<IActionResult> GetRequestsByFilters(
                [FromBody] SortType sortType,
                [FromBody] RequestStatus? requestStatus,
                [FromBody] string? userName)
        {

            RequestListModel requestListModel = await _requestService.GetAllRequests(sortType, requestStatus, userName);

            return Ok(requestListModel);
        }

    }
}
