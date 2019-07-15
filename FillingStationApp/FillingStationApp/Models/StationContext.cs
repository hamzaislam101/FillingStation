using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FillingStationApp.Models
{
    public class StationContext : DbContext
    {
        public StationContext() : base("StationConnection")
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Machine> Machines { get; set; }
        public DbSet<DataEntry> DataEntries { get; set; }
        public DbSet<Company> Companies { get; set; }
    }
}