﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req
{
    public class ReqEditRepaymentDto
    {
        public decimal Amount { get; set; }

        public decimal RepaidAmount { get; set; }

        public decimal BalanceAmount { get; set; }

        public string RepaidStatus { get; set; }
    }
}
