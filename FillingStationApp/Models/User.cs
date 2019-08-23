using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FillingStationApp.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string SuperPassword { get; set; }
        [Required]
        public string Type { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

    }

}