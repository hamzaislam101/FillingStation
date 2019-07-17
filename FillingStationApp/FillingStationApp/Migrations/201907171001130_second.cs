namespace FillingStationApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class second : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Companies", "CompanyName", c => c.String(nullable: false));
            AlterColumn("dbo.Companies", "ContactNo", c => c.String(nullable: false, maxLength: 11));
            AlterColumn("dbo.Companies", "Dealer", c => c.String(nullable: false));
            AlterColumn("dbo.Companies", "CNIC", c => c.String(maxLength: 13));
            AlterColumn("dbo.DataEntries", "Type", c => c.String(nullable: false));
            AlterColumn("dbo.DataEntries", "MachineNumber", c => c.String(nullable: false));
            AlterColumn("dbo.DataEntries", "CurrentReading", c => c.String(nullable: false));
            AlterColumn("dbo.DataEntries", "CashRecieved", c => c.String(nullable: false));
            AlterColumn("dbo.Machines", "MachineNumber", c => c.String(nullable: false));
            AlterColumn("dbo.Machines", "Type", c => c.String(nullable: false));
            AlterColumn("dbo.Stocks", "Company", c => c.String(nullable: false));
            AlterColumn("dbo.Stocks", "Type", c => c.String(nullable: false));
            AlterColumn("dbo.Stocks", "DealingPerson", c => c.String(nullable: false));
            AlterColumn("dbo.Stocks", "CNIC", c => c.String(nullable: false, maxLength: 13));
            AlterColumn("dbo.Stocks", "PhoneNo", c => c.String(nullable: false, maxLength: 11));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Stocks", "PhoneNo", c => c.String());
            AlterColumn("dbo.Stocks", "CNIC", c => c.String());
            AlterColumn("dbo.Stocks", "DealingPerson", c => c.String());
            AlterColumn("dbo.Stocks", "Type", c => c.String());
            AlterColumn("dbo.Stocks", "Company", c => c.String());
            AlterColumn("dbo.Machines", "Type", c => c.String());
            AlterColumn("dbo.Machines", "MachineNumber", c => c.String());
            AlterColumn("dbo.DataEntries", "CashRecieved", c => c.String());
            AlterColumn("dbo.DataEntries", "CurrentReading", c => c.String());
            AlterColumn("dbo.DataEntries", "MachineNumber", c => c.String());
            AlterColumn("dbo.DataEntries", "Type", c => c.String());
            AlterColumn("dbo.Companies", "CNIC", c => c.String());
            AlterColumn("dbo.Companies", "Dealer", c => c.String());
            AlterColumn("dbo.Companies", "ContactNo", c => c.String());
            AlterColumn("dbo.Companies", "CompanyName", c => c.String());
        }
    }
}
