using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FillingStationApp.Helper;
using FillingStationApp.Models;
using Microsoft.Reporting.WebForms;

namespace FillingStationApp.Controllers
{
    public class HomeController : Controller
    {
        StationContext db = new StationContext();
        [RoleAuthorize("Admin","Local")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            if (MainClass.isUserAdmin())
            {
                ViewBag.Message = "Your application description page.";

                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
            
        }
        [RoleAuthorize("Admin")]
        public ActionResult Last30DaySaleReport()
        {
            var last30 = DateTime.Now.AddDays(-30);
            var machineTypes = db.MachineTypes.ToList();
            Dictionary<string, double> stocks = new Dictionary<string, double>();
            double[] sold = new double[15];
            double[] purchased = new double[15];
            double totalSales = 0;
            foreach (var mt in machineTypes)
            {
                var soldStock = (from s in db.BalanceSheets
                                 where s.FuelType == mt.MType && s.CreatedOn >= last30
                                 select s);
                
                double totalSold = 0;
                double totalPurchased = 0;
                
                foreach (var st in soldStock)
                {
                    if(st.Type == "Sale")
                    {
                        totalSold += Convert.ToDouble(st.FuelAmount);
                        totalSales += Convert.ToDouble(st.Cash);
                    }
                    
                    if (st.Type == "Purchase")
                        totalPurchased += Convert.ToDouble(st.FuelAmount);
                }
                if (mt.MType == "Petrol")
                {
                    sold[0] = totalSold;
                    purchased[0] = totalPurchased;
                }
                else if (mt.MType == "Diesel")
                {
                    sold[1] = totalSold;
                    purchased[1] = totalPurchased;
                }
                    

            }



            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reports"), "MonthlySaleReport.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return RedirectToAction("Index");
            }
            //Setting the parameters
            ReportParameterCollection reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("PetrolSold", sold[0].ToString()));
            reportParameters.Add(new ReportParameter("DieselSold", sold[1].ToString()));
            reportParameters.Add(new ReportParameter("DieselPurchased", purchased[1].ToString()));
            reportParameters.Add(new ReportParameter("PetrolPurchased", purchased[0].ToString()));
            reportParameters.Add(new ReportParameter("DateFrom", last30.Date.ToString()));
            reportParameters.Add(new ReportParameter("DateTo", DateTime.Now.Date.ToString()));
            reportParameters.Add(new ReportParameter("TotalSales",totalSales.ToString() ));
            lr.SetParameters(reportParameters);

            //Setting the dataSource
            var ds = db.BalanceSheets.Where(x=>x.CreatedOn >= last30).OrderBy(x=>x.CreatedOn).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", ds);
            lr.DataSources.Add(rd);
            

            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =
                "<DeviceInfo>" +
                "   <OutputFormat>" + reportType + "</OutputFormat>" +
                "   <PageWidth>8.5in</PageWidth>" +
                "   <PageHeight>11in</PageHeight>" +
                "   <MarginTop>0.5in</MarginTop>" +
                "   <MarginLeft>0.25in</MarginLeft>" +
                "   <MarginRight>0.25in</MarginRight>" +
                "   <MarginBottom>0.5in</MarginBottom>" +
                "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = lr.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);

            return File(renderedBytes, mimeType,"MonthlyReport-"+DateTime.Now.ToString()+".pdf");


        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}