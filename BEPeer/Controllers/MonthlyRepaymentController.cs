using DAL;
using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Repositories.Services;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace BEPeer.Controllers
{
    [Route("rest/v1/monthly-repayment/[action]")]
    [ApiController]
    public class MonthlyRepaymentController : Controller
    {
        private readonly IMonthlyRepaymentServices _monthlyRepaymentServices;
        private readonly IRepaymentServices _repaymentService;

        public MonthlyRepaymentController(IMonthlyRepaymentServices monthlyRepaymentServices, IRepaymentServices repaymentService)
        {
            _monthlyRepaymentServices = monthlyRepaymentServices;
            _repaymentService = repaymentService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateMonthlyRepayment(string repaymentId)
        {
            try
            {
                var result = await _monthlyRepaymentServices.CreateMonthlyRepayment(repaymentId);

                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = "Monthly repayments created successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [Authorize]
        [HttpGet]
        [Route("{repaymentId}")]
        public async Task<IActionResult> GetMonthlyRepaymentByRepaymentId(string repaymentId)
        {
            try
            {
                var result = await _monthlyRepaymentServices.GetMonthlyRepaymentByRepaymentId(repaymentId);

                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Monthly repayments created successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> updateMonthlyRepaymentStatus([FromBody] List<ReqEditMonthlyRepaymentDto> monthlyRepaymentDto)
        {
            try
            {
                var result = await _monthlyRepaymentServices.UpdateMonthlyRepaymentStatus(monthlyRepaymentDto);

                return Ok(new ResBaseDto<string>
                {
                    Data = result,
                    Success = true,
                    Message = "Payments updated successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}
