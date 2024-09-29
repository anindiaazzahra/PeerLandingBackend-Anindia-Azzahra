using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req
{
    public class ReqLoanDto
    {
        [Required(ErrorMessage = "BorrowedId is required")]
        public string BorrowerId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be a positive valaue")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Interes rate is required")]
        public decimal InterestRate { get; set; }
    }
}
