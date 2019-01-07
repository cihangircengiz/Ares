using Ares.EntityData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ares.Models
{
    public class CaseIndexModel
    {
        public double TotalMount { get; set; }
        public double DailyIncome { get; set; }
        public double DailyOutcome { get; set; }
        public double MounthlyIncome { get; set; }
        public double MountlyOutcome { get; set; }
        public List<Payment> allPayments { get; set; }
    }
}