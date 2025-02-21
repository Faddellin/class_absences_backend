
using BusinessLogic.ServiceInterfaces;
using BusinessLogic.Services;
using Common.DbModels;
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
        [FromQuery] DateTime AbsenceDateFrom,
        [FromQuery] DateTime AbsenceDateTo,
        [FromQuery] string lessonName)
        {
            try
            {
                //await EnsureTokenIsValid();

                if (AbsenceDateFrom < AbsenceDateTo)
                {
                    throw new ArgumentException("AbsenceDateFrom can't be lower than AbsenceDateTo");
                }

                var list = await _consultationService.GetInspectionsList(
                    grouped == true,
                    icdRoots.Count == 0 ? new List<Guid>() : icdRoots,
                    page,
                    size);
                return Ok(list);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (IncorrectModelException ex)
            {
                return BadRequest(new ResponseModel
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

    }
}
