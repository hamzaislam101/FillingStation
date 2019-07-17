using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FillingStationApp.Models
{
    public class Machine
    {
        public int Id { get; set; }
        [Required]
        public string MachineNumber { get; set; }
        [Required]
        public string Type { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public bool IsActive { get; set; }


    }
}