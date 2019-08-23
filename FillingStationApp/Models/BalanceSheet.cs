using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FillingStationApp.Models
{
    public class BalanceSheet
    {
        public int Id { get; set; }
        public decimal FuelAmount { get; set; }
        public string FuelType { get; set; }
        public decimal Cash { get; set; }
        public string Type { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}