using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FillingStationApp.Models
{
    public class StockTracker
    {
        public int Id { get; set; }
        public decimal PurchaseRate { get; set; }
        public string FuelType { get; set; }
        public decimal TotalFuelAmount { get; set; }
        public decimal RemainingFuelAmount { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

    }
}