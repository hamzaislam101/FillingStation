using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FillingStationApp.Models
{
    public class Company
    {
        public int Id { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        [MaxLength(11)]
        public string ContactNo { get; set; }
        [Required]
        public string Dealer { get; set; }
        [MaxLength(13)]
        public string CNIC { get; set; }
        public string Address { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}