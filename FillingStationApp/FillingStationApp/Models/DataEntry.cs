using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FillingStationApp.Models
{
    public class DataEntry
    {
        public int Id { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string MachineNumber { get; set; }
        [Required]
        public string CurrentReading { get; set; }
        [Required]
        public string CashRecieved { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
    }
}