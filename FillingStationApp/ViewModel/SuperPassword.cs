using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FillingStationApp.ViewModel
{
    public class SuperPassword
    {
        [Required]
        [DataType(DataType.Password)]
        public string SuperPass { get; set; }
    }
}