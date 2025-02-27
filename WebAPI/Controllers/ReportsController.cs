﻿using BusinessLogic.ServiceInterfaces;
using class_absences_backend.Controllers;
using Common.DtoModels;
using Common.DtoModels.Others;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessLogic.Controllers
{

    [ApiController]
    [Route("api/report")]

    public class ReportsController : BaseController
    {

        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
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

            var userId = GetUserId();

            await _reportService.ExportUserAbsencesInWord(dateFrom, dateTo, userId, targetUsersId);

            return Ok();
        }



    }
}
