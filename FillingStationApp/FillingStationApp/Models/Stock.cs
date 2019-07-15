using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FillingStationApp.Models
{
    public class Stock
    {
        public int Id { get; set; }
        public string Company { get; set; }
        public string Type { get; set; }
        public int Quantity { get; set; }
        public string DealingPerson { get; set; }
        public string CNIC { get; set; }
        public string PhoneNo { get; set; }
        public string Address { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}