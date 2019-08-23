using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FillingStationApp.Helper;
using FillingStationApp.Models;

namespace FillingStationApp.Controllers
{
    public class StocksController : Controller
    {
        private StationContext db = new StationContext();

        // GET: Stocks
        [RoleAuthorize("Admin")]
        public ActionResult Index()
        {
            var machineTypes = db.MachineTypes.ToList();
            Dictionary<string, double> stocks = new Dictionary<string, double>();
            foreach(var mt in machineTypes)
            {
                double sum = 0;
                var trackers = db.StockTrackers.Where(x => x.FuelType == mt.MType && x.RemainingFuelAmount>0);
                foreach(var t in trackers)
                {
                    sum += Convert.ToDouble(t.RemainingFuelAmount);
                }
                
                stocks.Add(mt.MType, sum);
            }

            ViewBag.StockList = stocks;

            return View(db.Stocks.ToList().OrderByDescending(x=>x.CreatedOn));
        }

        // GET: Stocks/Details/5
        [RoleAuthorize("Admin")]

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stock stock = db.Stocks.Find(id);
            if (stock == null)
            {
                return HttpNotFound();
            }
            return View(stock);
        }

        // GET: Stocks/Create
        [RoleAuthorize("Admin")]

        public ActionResult Create()
        {

            List<SelectListItem> typeList = new List<SelectListItem>();
            var mt = db.MachineTypes.Distinct().ToList();
            foreach (var m in mt)
            {
                typeList.Add(new SelectListItem() { Text = m.MType, Value = m.MType });
            }
            if (mt.Count > 0)
            {
                ViewBag.TypeList = new SelectList(typeList, "Value", "Text", typeList[0]);
            }
            else
            {
                ViewBag.TypeList = new SelectList(typeList, "Value", "Text");
            }

            var stock = new Stock();
            List<SelectListItem> companies = new List<SelectListItem>();
            var types = db.Companies.Select(x => x.CompanyName).Distinct().ToList();
            foreach (var type in types)
            {
                companies.Add(new SelectListItem { Text = type, Value = type });
            }
            if (types.Count > 0)
            {
                var c = types[0];
                ViewBag.CompanyList = new SelectList(companies, "Value", "Text", c);
                var com = db.Companies.Where(x => x.CompanyName == c).FirstOrDefault();
                stock.Address = com.Address;
                stock.CNIC = com.CNIC;
                stock.DealingPerson = com.Dealer;
                stock.PhoneNo = com.ContactNo;
            }
            else
                ViewBag.CompanyList = new SelectList(companies, "Value", "Text");

            return View(stock);
        }

        // POST: Stocks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Admin")]

        public ActionResult Create([Bind(Include = "Company,Type,Quantity,DealingPerson,CNIC,PhoneNo,Address,PriceAfterCarriage,PurchasePricePL,CarriageCharges")] Stock stock)
        {
            if (ModelState.IsValid)
            {
                //Adding the balance sheet entry
                var sheet = new BalanceSheet();
                sheet.Cash = stock.PriceAfterCarriage * stock.Quantity;
                sheet.CreatedOn = DateTime.Now;
                sheet.FuelAmount = stock.Quantity;
                sheet.FuelType = stock.Type;
                sheet.Type = "Purchase";
                db.BalanceSheets.Add(sheet);
                db.SaveChanges();

                //Adding the stock tracker entry
                var tracker = new StockTracker();
                tracker.CreatedOn = DateTime.Now;
                tracker.UpdatedOn = DateTime.Now;
                tracker.FuelType = stock.Type;
                tracker.PurchaseRate = stock.PriceAfterCarriage;
                tracker.RemainingFuelAmount = stock.Quantity;
                tracker.TotalFuelAmount = stock.Quantity;
                db.StockTrackers.Add(tracker);
                db.SaveChanges();

                stock.CreatedBy = Session["LoggedIn"].ToString();
                stock.CreatedOn = DateTime.Now;
                db.Stocks.Add(stock);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            List<SelectListItem> companies = new List<SelectListItem>();
            var types = db.Companies.Select(x => x.CompanyName).Distinct().ToList();
            foreach (var type in types)
            {
                companies.Add(new SelectListItem { Text = type, Value = type });
            }
            if (types.Count > 0)
                ViewBag.CompanyList = new SelectList(companies, "Value", "Text", types[0]);
            else
                ViewBag.CompanyList = new SelectList(companies, "Value", "Text");

            List<SelectListItem> typeList = new List<SelectListItem>();
            var mt = db.MachineTypes.Distinct().ToList();
            foreach (var m in mt)
            {
                typeList.Add(new SelectListItem() { Text = m.MType, Value = m.MType });
            }
            if (mt.Count > 0)
            {
                ViewBag.TypeList = new SelectList(typeList, "Value", "Text", typeList[0]);
            }
            else
            {
                ViewBag.TypeList = new SelectList(typeList, "Value", "Text");
            }

            return View(stock);
        }

        // GET: Stocks/Edit/5
        [RoleAuthorize("Admin")]

        public ActionResult Edit(int? id)
        {
            if (Session["Pass"] == null)
            {
                Session["Redirect"] = HttpContext.Request.Url.AbsoluteUri;
                return RedirectToAction("EnterSuperPassword", "Users");
            }
            Session["Pass"] = null;
            Session["Redirect"] = null;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stock stock = db.Stocks.Find(id);
            if (stock == null)
            {
                return HttpNotFound();
            }
            return View(stock);
        }

        // POST: Stocks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Admin")]

        public ActionResult Edit([Bind(Include = "Id,Company,Type,Quantity,DealingPerson,CNIC,PhoneNo,Address")] Stock stock)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stock).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stock);
        }

        public JsonResult GetCompanyInfo(string company)
        {
            var c = db.Companies.Where(x => x.CompanyName == company).FirstOrDefault();
            return Json(c, JsonRequestBehavior.DenyGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
