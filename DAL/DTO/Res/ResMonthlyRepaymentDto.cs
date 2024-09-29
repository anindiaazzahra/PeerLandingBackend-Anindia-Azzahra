using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res
{
    public class ResMonthlyRepaymentDto
    {
        public string Id { get; set; }

        public string RepaymentId { get; set; }

        public decimal Amount { get; set; }

        public bool Status { get; set; }
    }
}
