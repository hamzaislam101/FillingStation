using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FillingStationApp.Models
{
    public class DataEntry
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string MachineNumber { get; set; }
        public string CurrentReading { get; set; }
        public string CashRecieved { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
    }
}