using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FillingStationApp.Models
{
    public class Rate
    {
        public int Id { get; set; }
        public DateTime RateDate { get; set; }
        public string FuelType { get; set; }
        public decimal RateValue { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedOn { get; set; }

    }
}