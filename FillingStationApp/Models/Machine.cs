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
        [Display(Name ="Sr#")]
        public string MachineNumber { get; set; }
        [Required]
        public string Type { get; set; }
        public string Location { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }


    }
}