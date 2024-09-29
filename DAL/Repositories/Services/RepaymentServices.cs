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
    public class RepaymentServices : IRepaymentServices
    {
        private readonly PeerlandingContext _context;

        public RepaymentServices(PeerlandingContext context)
        {
            _context = context;
        }

        public async Task<string> CreateRepayment(ReqRepaymentDto repaymentDto)
        {
            var newRepayment = new TrnRepayment
            {
                LoanId = repaymentDto.LoanId,
                Amount = repaymentDto.Amount,
                RepaidAmount = repaymentDto.RepaidAmount,
                BalanceAmount = repaymentDto.BalanceAmount,
                RepaidStatus = repaymentDto.RepaidStatus ?? "on_repay"
            };

            await _context.AddAsync(newRepayment);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Error occurred: {ex.InnerException?.Message}");
            }

            return newRepayment.Id;
        }

        public async Task<List<ResListRepaymentDto>> RepaymentList(string status, string? idLender, string? borrowerId)
        {
            var repayments = await _context.TrnRepayment
                .Join(
                    _context.MstLoans,
                    repayment => repayment.LoanId,
                    loan => loan.Id,
                    (repayment, loan) => new { repayment, loan }
                )
                .Join(
                    _context.TrnFunding,
                    combined => combined.loan.Id,
                    funding => funding.LoanId,
                    (combined, funding) => new { combined.repayment, combined.loan, funding }
                )
                .Join(
                    _context.MstUsers,
                    combined => combined.loan.BorrowerId,
                    user => user.Id,
                    (combined, user) => new { combined.repayment, combined.loan, combined.funding, user }
                )
                .Where(r => (status == null || r.repayment.RepaidStatus == status) &&
                            (idLender == null || r.funding.LenderId == idLender) &&
                            (borrowerId == null || r.loan.BorrowerId == borrowerId))
                .Select(r => new ResListRepaymentDto
                {
                    Id = r.repayment.Id,
                    LoanId = r.loan.Id,
                    LenderId = r.funding.LenderId,
                    BorrowerId = r.loan.BorrowerId,
                    BorrowerName = r.user.Name,
                    Amount = r.repayment.Amount,
                    RepaidAmount = r.repayment.RepaidAmount,
                    BalanceAmount = r.repayment.BalanceAmount,
                    RepaidStatus = r.repayment.RepaidStatus,
                    PaidAt = r.repayment.PaidAt
                })
                .ToListAsync();

            return repayments;
        }

        public async Task<ResRepaymentByIdDto> GetRepaymentById(string id)
        {
            var repayment = await _context.TrnRepayment
            .Join(
                _context.MstLoans,
                repayment => repayment.LoanId,
                loan => loan.Id,
                (repayment, loan) => new { repayment, loan }
            )
            .Join(
                _context.TrnFunding,
                combined => combined.loan.Id,
                funding => funding.LoanId,
                (combined, funding) => new { combined.repayment, combined.loan, funding }
            )
            .Join(
                _context.MstUsers,
                combined => combined.loan.BorrowerId,
                user => user.Id,
                (combined, user) => new { combined.repayment, combined.loan, combined.funding, user }
            )
            .Where(r => r.repayment.Id == id)
            .Select(r => new ResRepaymentByIdDto
            {
                Id = r.repayment.Id,
                LoanId = r.loan.Id,
                LenderId = r.funding.LenderId,
                BorrowerId = r.loan.BorrowerId,
                BorrowerName = r.user.Name,
                InterestRate = r.loan.InterestRate,
                Duration = r.loan.Duration,
                Amount = r.repayment.Amount,
                RepaidAmount = r.repayment.RepaidAmount,
                BalanceAmount = r.repayment.BalanceAmount,
                RepaidStatus = r.repayment.RepaidStatus,
                PaidAt = r.repayment.PaidAt
            })
            .FirstOrDefaultAsync();

            if (repayment == null)
            {
                throw new Exception("Repayment not found");
            }

            return repayment;
        }

        public async Task<ResRepaymentByIdDto> GetRepaymentByLoanId(string loanId)
        {
            var repayment = await _context.TrnRepayment
            .Join(
                _context.MstLoans,
                repayment => repayment.LoanId,
                loan => loan.Id,
                (repayment, loan) => new { repayment, loan }
            )
            .Join(
                _context.TrnFunding,
                combined => combined.loan.Id,
                funding => funding.LoanId,
                (combined, funding) => new { combined.repayment, combined.loan, funding }
            )
            .Join(
                _context.MstUsers,
                combined => combined.loan.BorrowerId,
                user => user.Id,
                (combined, user) => new { combined.repayment, combined.loan, combined.funding, user }
            )
            .Where(r => r.loan.Id == loanId)
            .Select(r => new ResRepaymentByIdDto
            {
                Id = r.repayment.Id,
                LoanId = r.loan.Id,
                LenderId = r.funding.LenderId,
                BorrowerId = r.loan.BorrowerId,
                BorrowerName = r.user.Name,
                InterestRate = r.loan.InterestRate,
                Duration = r.loan.Duration,
                Amount = r.repayment.Amount,
                RepaidAmount = r.repayment.RepaidAmount,
                BalanceAmount = r.repayment.BalanceAmount,
                RepaidStatus = r.repayment.RepaidStatus,
                PaidAt = r.repayment.PaidAt
            })
            .FirstOrDefaultAsync();

                    if (repayment == null)
                    {
                        throw new Exception("Repayment not found for the given loan ID");
                    }

                    return repayment;
                }
    }
}
