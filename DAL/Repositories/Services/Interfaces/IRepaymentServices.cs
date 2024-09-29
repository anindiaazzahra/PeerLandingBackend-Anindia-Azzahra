using DAL.DTO.Req;
using DAL.DTO.Res;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services.Interfaces
{
    public interface IRepaymentServices
    {
        Task<string> CreateRepayment(ReqRepaymentDto repaymentDto);

        Task<List<ResListRepaymentDto>> RepaymentList(string status, string? idLender, string? borrowerId);
    
        Task<ResRepaymentByIdDto> GetRepaymentById(string id);

        Task<ResRepaymentByIdDto> GetRepaymentByLoanId(string loanId);
    }
}
