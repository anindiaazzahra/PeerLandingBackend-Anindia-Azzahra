using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Models;
using DAL.Repositories.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DAL.Repositories.Services
{
    public class MonthlyRepaymentServices : IMonthlyRepaymentServices
    {
        private readonly PeerlandingContext _context;
        private readonly IUserServices _userService;
        private readonly ILoanServices _loanService;
        private readonly IRepaymentServices _repaymentService;
        

        public MonthlyRepaymentServices(PeerlandingContext context, IUserServices userService, ILoanServices loanService, IRepaymentServices repaymentService)
        {
            _context = context;
            _userService = userService;
            _loanService = loanService;
            _repaymentService = repaymentService;
        }

        public async Task<string> CreateMonthlyRepayment(string repaymentId)
        {
            var repayment = await _repaymentService.GetRepaymentById(repaymentId);
            if (repayment == null)
            {
                throw new Exception("Repayment not found.");
            }

            for (int i = 1; i <= 12; i++)
            {
                var data = new TrnMonthlyRepayment
                {
                    RepaymentId = repaymentId,
                    Status = false,
                    Amount = jumlahCicilanPerBulan(repayment.Amount, repayment.InterestRate),
                };

                await _context.AddAsync(data);
                await _context.SaveChangesAsync();
            }

            return repayment.Id;
        }

        public decimal jumlahCicilanPerBulan(decimal amount, decimal interestRate)
        {
            decimal monthlyRate = interestRate / 100 / 12;
            decimal denominator = 1 - (decimal)Math.Pow((double)(1 + monthlyRate), -12);

            if (denominator == 0)
            {
                throw new DivideByZeroException("The denominator in the monthly payment calculation cannot be zero.");
            }

            return amount * (monthlyRate / denominator);

        }

        public async Task<List<ResMonthlyRepaymentDto>> GetMonthlyRepaymentByRepaymentId(string repaymentId)
        {
            if (repaymentId == null)
            {
                throw new Exception("Input a valid id.");
            }

            var result = await _context.TrnMonthlyRepayment
                .Select(payment => new ResMonthlyRepaymentDto
                {
                    Id = payment.Id,
                    RepaymentId = payment.RepaymentId,
                    Amount = payment.Amount,
                    Status = payment.Status
                })
                .Where(payment => payment.RepaymentId == repaymentId)
                .OrderBy(payment => payment.Id)
                .ToListAsync();

            return result;

        }

        public async Task<string> UpdateMonthlyRepaymentStatus(List<ReqEditMonthlyRepaymentDto> monthlyRepayments)
        {
            if (monthlyRepayments == null || !monthlyRepayments.Any())
            {
                throw new Exception("Invalid monthly repayment.");
            }

            foreach (var repaymentDto in monthlyRepayments)
            {
                var monthlyRepayment = await _context.TrnMonthlyRepayment
                    .FirstOrDefaultAsync(r => r.RepaymentId == repaymentDto.RepaymentId && r.Status == false);

                if (monthlyRepayment == null)
                {
                    throw new Exception($"Repayment with ID {repaymentDto.RepaymentId} not found.");
                }

                monthlyRepayment.Status = true;
                _context.TrnMonthlyRepayment.Update(monthlyRepayment);


                var repayment = await _repaymentService.GetRepaymentById(repaymentDto.RepaymentId);

                var newRepayment = await _context.TrnRepayment
                .SingleOrDefaultAsync(r => r.LoanId == repayment.LoanId);

                if (newRepayment == null)
                {
                    throw new Exception("funding not found");
                }

                newRepayment.RepaidAmount = newRepayment.RepaidAmount + monthlyRepayment.Amount;

                newRepayment.BalanceAmount = Math.Max(0, newRepayment.BalanceAmount - monthlyRepayment.Amount);

                if (newRepayment.BalanceAmount == 0)
                {
                    newRepayment.RepaidStatus = "done";
                    var loan = await _loanService.GetLoanById(newRepayment.LoanId) ?? throw new Exception("Loan not found."); ;
                    var lender = await _userService.GetUserById(loan.LenderId) ?? throw new Exception("Lender not found."); ;
                    var borrower = await _userService.GetUserById(loan.BorrowerId) ?? throw new Exception("Borrower not found."); ;
                    var newLoan = await _context.MstLoans
                        .SingleOrDefaultAsync(l => l.Id == loan.LoanId);

                    if (loan == null)
                    {
                        throw new Exception("loan not found");
                    }

                    var lenderSaldoDto = new ReqEditSaldoDto
                    {
                        Balance = lender.Balance ?? 0 + loan.Amount
                    };

                    var borrowerSaldoDto = new ReqEditSaldoDto
                    {
                        Balance = borrower.Balance ?? 0 - loan.Amount
                    };

                    await _userService.UpdateSaldo(lender.Id, lenderSaldoDto);
                    await _userService.UpdateSaldo(borrower.Id, borrowerSaldoDto);

                    newLoan.Status = "repaid";
                }

                repayment.PaidAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();


            return "Repayment updated successfully.";
        }
    }
}
