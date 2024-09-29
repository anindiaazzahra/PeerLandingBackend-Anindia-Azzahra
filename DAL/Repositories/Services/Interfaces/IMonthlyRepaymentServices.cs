using DAL.DTO.Req;
using DAL.DTO.Res;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services.Interfaces
{
    public interface IMonthlyRepaymentServices
    {
        Task<string> CreateMonthlyRepayment(string repaymentId);

        Task<List<ResMonthlyRepaymentDto>> GetMonthlyRepaymentByRepaymentId(string repaymentId);

        Task<string> UpdateMonthlyRepaymentStatus(List<ReqEditMonthlyRepaymentDto> monthlyRepayments);
    }
}
