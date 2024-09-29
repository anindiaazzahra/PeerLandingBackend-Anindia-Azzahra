using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Models;
using DAL.Repositories.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services
{
    public class FundingServices : IFundingServices
    {
        private readonly PeerlandingContext _context;
        private readonly IUserServices _userService;
        private readonly ILoanServices _loanService;
        private readonly IRepaymentServices _repaymentService;
        private readonly IMonthlyRepaymentServices _monthlyRepaymentService;

        public FundingServices(PeerlandingContext context, IUserServices userService, ILoanServices loanService, IRepaymentServices repaymentService, IMonthlyRepaymentServices monthlyRepaymentService)
        {
            _context = context;
            _userService = userService;
            _loanService = loanService;
            _repaymentService = repaymentService;
            _monthlyRepaymentService = monthlyRepaymentService;
        }

        public async Task<string> CreateFunding(ReqFundingDto funding)
        {
            var loan = await _context.MstLoans.Include(x => x.User).SingleOrDefaultAsync(x => x.Id == funding.LoanId) ?? throw new Exception("Loan not found");
            var lender = await _context.MstUsers.SingleOrDefaultAsync(x => x.Id == funding.LenderId) ?? throw new Exception("Lender not found");

            if (loan.User == null)
            {
                throw new Exception("User object in loan is null");
            }

            try
            {
                await _loanService.EditStatus(loan.Id, new ReqEditLoanDto
                {
                    Status = "funded"
                });

                lender.Balance -= loan.Amount;
                await _userService.UpdateSaldo(lender.Id, new ReqEditSaldoDto
                {
                    Balance = lender.Balance ?? 0
                });

                var borrower = await _userService.GetUserById(loan.User.Id) ?? throw new Exception("User not found");
                borrower.Balance += loan.Amount;
                await _userService.UpdateSaldo(borrower.Id, new ReqEditSaldoDto
                {
                    Balance = borrower.Balance ?? 0
                });

                var newFunding = new TrnFunding
                {
                    LenderId = funding.LenderId,
                    LoanId = funding.LoanId,
                    Amount = loan.Amount,
                };

                await _context.AddAsync(newFunding);
                await _context.SaveChangesAsync();

                var repaymentId = await _repaymentService.CreateRepayment(new ReqRepaymentDto
                {
                    LoanId = funding.LoanId,
                    Amount = loan.Amount,
                    RepaidAmount = 0,
                    BalanceAmount = loan.Amount,
                });

                Console.WriteLine(repaymentId);

                await _monthlyRepaymentService.CreateMonthlyRepayment(repaymentId);

                return repaymentId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred: {ex.Message}");
            }
        }
    }
}
