using BusinessLogic.ServiceInterfaces;
using Common.DtoModels;
using Common.DtoModels.Others;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessLogic.Controllers
{

    [ApiController]
    [Route("api/report")]

    public class ReportsController : ControllerBase
    {

        private readonly IReportService _reportService;
        private readonly ITokenService _tokenService;

        public ReportsController(IReportService reportService, ITokenService tokenService)
        {
            _reportService = reportService;
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
        /// Get users absences report
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="403">User doesn't have enough rights</response>
        /// <response code="400">Invalid arguments</response>
        /// <response code="500">Internal server error</response>
        [Authorize]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        [HttpGet]
        public async Task<IActionResult> GetUsersAbsencesReport(
                [FromQuery] List<Guid> targetUsersId,
                [FromQuery] DateTime dateFrom,
                [FromQuery] DateTime dateTo)
        {

            var userId = await EnsureTokenIsValid();

            await _reportService.ExportUserAbsencesInWord(dateFrom, dateTo, userId, targetUsersId);

            return Ok();
        }



    }
}
