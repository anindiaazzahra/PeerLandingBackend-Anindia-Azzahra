using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Models;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services
{
    public class LoanServices : ILoanServices
    {
        private readonly PeerlandingContext _peerLandingContext;
        private readonly IConfiguration _configuration;

        public LoanServices(PeerlandingContext peerLandingContext)
        {
            _peerLandingContext = peerLandingContext;
        }

        public async Task<string> CreateLoan(ReqLoanDto loan)
        {
            var newLoan = new MstLoans
            {
                BorrowerId = loan.BorrowerId,
                Amount = loan.Amount,
                InterestRate = loan.InterestRate,
            };

            await _peerLandingContext.AddAsync(newLoan);
            await _peerLandingContext.SaveChangesAsync();

            return newLoan.BorrowerId;

        }

        public async Task<ResEditLoanDto> EditStatus(string id, ReqEditLoanDto dtoLoan)
        {
            var loan = await _peerLandingContext.MstLoans.SingleOrDefaultAsync(x => x.Id == id);
            if (loan == null)
            {
                throw new Exception("Loan not found");
            }

            loan.Status = dtoLoan.Status;

            _peerLandingContext.MstLoans.Update(loan);
            await _peerLandingContext.SaveChangesAsync();

            return new ResEditLoanDto
            {
                Id = loan.Id,
                Status = loan.Status
            };

        }

        public async Task<List<ResListLoanDto>> LoanList(string status, string? idLender, string? borrowerId)
        {
            var loans = await _peerLandingContext.MstLoans
                .Where(l => (status == null || l.Status == status) &&
                            (idLender == null || _peerLandingContext.TrnFunding.Any(f => f.LoanId == l.Id && f.LenderId == idLender)) &&
                            (borrowerId == null || l.BorrowerId == borrowerId))
                .OrderByDescending(l => l.CreatedAt)
                .Include(l => l.User)
                .Select(loan => new ResListLoanDto
                {
                    LoanId = loan.Id,
                    BorrowerName = loan.User.Name,
                    Amount = loan.Amount,
                    InterestRate = loan.InterestRate,
                    Duration = loan.Duration,
                    Status = loan.Status,
                    CreatedAt = loan.CreatedAt,
                    UpdatedAt = loan.UpdatedAt,
                }).ToListAsync();

            return loans;
        }

        public async Task<ResListLoanDto> GetLoanById(string id)
        {
            var loan = await _peerLandingContext.MstLoans
                .Include(l => l.User) 
                .SingleOrDefaultAsync(l => l.Id == id);

            var funding = await _peerLandingContext.TrnFunding
                .SingleOrDefaultAsync(f => f.LoanId == loan.Id);

            if (loan == null)
            {
                throw new Exception($"Loan with ID {id} not found.");
            }

            return new ResListLoanDto
            {
                LoanId = loan.Id,
                BorrowerName = loan.User.Name, 
                Amount = loan.Amount,
                InterestRate = loan.InterestRate,
                Duration = loan.Duration,
                Status = loan.Status,
                CreatedAt = loan.CreatedAt,
                UpdatedAt = loan.UpdatedAt,
                BorrowerId = loan.User.Id,
                LenderId = funding.LenderId
            };
        }

    }
}
