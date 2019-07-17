using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FillingStationApp.Models
{
    public class Stock
    {
        public int Id { get; set; }
        [Required]
        public string Company { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public string DealingPerson { get; set; }
        [Required]
        [MaxLength(13)]
        public string CNIC { get; set; }
        [Required]
        [MaxLength(11)]
        public string PhoneNo { get; set; }
        public string Address { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}