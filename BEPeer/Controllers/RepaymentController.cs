using DAL.DTO.Res;
using DAL.Repositories.Services;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BEPeer.Controllers
{
    [Route("rest/v1/repayment/[action]")]
    [ApiController]
    public class RepaymentController : Controller
    {
        private readonly IRepaymentServices _repaymentServices;

        public RepaymentController(IRepaymentServices repaymentServices)
        {
            _repaymentServices = repaymentServices;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> RepaymentList(string status, string? idLender, string? borrowerId)
        {
            try
            {
                var res = await _repaymentServices.RepaymentList(status, idLender, borrowerId);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Success load repayment",
                    Data = res
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRepaymentById(string id)
        {
            try
            {
                var res = await _repaymentServices.GetRepaymentById(id);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Success load repayment by ID",
                    Data = res
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
        [Route("{loanId}")]
        public async Task<IActionResult> GetRepaymentByLoanId(string loanId)
        {
            try
            {
                var res = await _repaymentServices.GetRepaymentByLoanId(loanId);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Success load repayment by Loan ID",
                    Data = res
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
