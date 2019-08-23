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
        public decimal CurrentReading { get; set; }
        [Required]
        public decimal CashRecieved { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}