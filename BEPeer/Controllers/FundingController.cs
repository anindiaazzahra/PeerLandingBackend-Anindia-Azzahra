using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace BEPeer.Controllers
{
    [Route("rest/v1/funding/[action]")]
    [ApiController]
    public class FundingController : ControllerBase
    {
        private readonly IFundingServices _fundingService;

        public FundingController(IFundingServices fundingService)
        {
            _fundingService = fundingService;
        }

        [Authorize(Roles = "lender")]
        [HttpPost]
        public async Task<IActionResult> ApproveFunding(ReqFundingDto funding)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Any())
                        .Select(x => new
                        {
                            Field = x.Key,
                            Messages = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                        }).ToList();
                    var errorMessage = new StringBuilder("Validation errors occured!");

                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = errorMessage.ToString(),
                        Data = errors
                    });
                }

                var res = await _fundingService.CreateFunding(funding);
                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = "Success approve funding data",
                    Data = res
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "Email already user")
                {
                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    });
                }
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
