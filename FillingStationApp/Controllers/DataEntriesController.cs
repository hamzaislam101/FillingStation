using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using FillingStationApp.Helper;
using FillingStationApp.Models;

namespace FillingStationApp.Controllers
{
    public class DataEntriesController : Controller
    {
        private StationContext db = new StationContext();

        // GET: DataEntries
        [RoleAuthorize("Admin","Local")]
        public ActionResult Index()
        {
            if (MainClass.isUserAdmin())
            {
                return View(db.DataEntries.ToList().OrderByDescending(x=>x.CreatedOn));
            }
            else
            {
                var username = Session["LoggedIn"].ToString();
                var entries = db.DataEntries.Where(x => x.CreatedBy == username).ToList();
                var toShow = new List<DataEntry>();
                foreach(var e in entries)
                {
                    if(DateTime.Now.Subtract(e.CreatedOn) < TimeSpan.FromMinutes(180))
                    {
                        toShow.Add(e);
                    }
                }
                return View(toShow);
            }
            
        }

        // GET: DataEntries/Details/5
        [RoleAuthorize("Admin")]

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataEntry dataEntry = db.DataEntries.Find(id);
            if (dataEntry == null)
            {
                return HttpNotFound();
            }
            return View(dataEntry);
        }

        // GET: DataEntries/Create
        [RoleAuthorize("Admin","Local")]
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

            var machines = new List<Machine>();

            if (mt.Count > 0)
            {
                var type = mt[0].MType;
                machines = db.Machines.Where(x => x.Type == type).ToList();
            }
            else
            {
                machines = db.Machines.Distinct().ToList();
            }
            
            List<SelectListItem> mac = new List<SelectListItem>();
            foreach(var machine in machines)
            {
                mac.Add(new SelectListItem { Text = machine.MachineNumber, Value = machine.MachineNumber });
            }
            if(machines.Count > 0)
            ViewBag.MachineList = new SelectList(mac,"Value","Text",machines[0]);
            else
            ViewBag.MachineList = new SelectList(mac,"Value","Text");

            double rate = 0;
            if (typeList.Count > 0)
            {
                var type = mt[0].MType;
                var e = db.Rates.Where(x => x.FuelType == type).OrderByDescending(x => x.AddedOn).FirstOrDefault();
                if(e!=null)
                rate = Convert.ToDouble(e.RateValue);
            }
                
            ViewBag.Rate = rate;

            return View();
        }

        // POST: DataEntries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Admin","Local")]

        public ActionResult Create([Bind(Include = "Type,MachineNumber,CurrentReading,CashRecieved")] DataEntry dataEntry)
        {
            if (ModelState.IsValid)
            {
                //Adding value to balance sheet
                var e = db.DataEntries.Where(x => x.MachineNumber == dataEntry.MachineNumber).OrderByDescending(x=>x.CreatedOn).FirstOrDefault();
                var sheet = new BalanceSheet();
                sheet.Cash = dataEntry.CashRecieved;
                sheet.CreatedOn = DateTime.Now;
                if(e == null)
                {
                    sheet.FuelAmount = dataEntry.CurrentReading;
                }
                else
                {
                    sheet.FuelAmount = dataEntry.CurrentReading - e.CurrentReading;
                }
                
                sheet.FuelType = dataEntry.Type;
                sheet.Type = "Sale";
                db.BalanceSheets.Add(sheet);
                db.SaveChanges();

                //Updating the stock tracker
                var a = sheet.FuelAmount;
                while (a != 0)
                {
                    var s = db.StockTrackers.Where(x => x.FuelType == dataEntry.Type && x.RemainingFuelAmount > 0).OrderBy(x=>x.CreatedOn).FirstOrDefault();
                    s.UpdatedOn = DateTime.Now;
                    if (a < s.RemainingFuelAmount)
                    {
                        s.RemainingFuelAmount -= a;
                        a = 0;
                    }
                    else
                    {
                        a -= s.RemainingFuelAmount;
                        s.RemainingFuelAmount = 0;
                    }
                    db.Entry(s).State = EntityState.Modified;
                    db.SaveChanges();
                }

                
                

                //Creating dataentry
                dataEntry.CreatedBy = Session["LoggedIn"].ToString();
                dataEntry.CreatedOn = DateTime.Now;
                db.DataEntries.Add(dataEntry);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

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

            var machines = new List<Machine>();

            if (mt.Count > 0)
            {
                machines = db.Machines.Where(x => x.Type == mt[0].MType).ToList();
            }
            else
            {
                machines = db.Machines.Distinct().ToList();
            }
            List<SelectListItem> mac = new List<SelectListItem>();
            foreach (var machine in machines)
            {
                mac.Add(new SelectListItem { Text = machine.MachineNumber, Value = machine.MachineNumber });
            }
            if (machines.Count>0)
                ViewBag.MachineList = new SelectList(mac, "Value", "Text", machines[0]);
            else
                ViewBag.MachineList = new SelectList(mac, "Value", "Text");


            return View(dataEntry);
        }

        // GET: DataEntries/Edit/5
        [RoleAuthorize("Admin","Local")]

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
            DataEntry dataEntry = db.DataEntries.Find(id);
            if (dataEntry == null)
            {
                return HttpNotFound();
            }
            if(dataEntry.CreatedBy != Session["LoggedIn"].ToString() && !MainClass.isUserAdmin())
            {
                return new HttpUnauthorizedResult("You are not allowed to view this entry.");
            }
            return View(dataEntry);
        }

        // POST: DataEntries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Admin","Local")]

        public ActionResult Edit([Bind(Include = "Id,Type,MachineNumber,CurrentReading,CashRecieved")] DataEntry dataEntry)
        {
            if (ModelState.IsValid)
            {
                if(!MainClass.isUserAdmin() && dataEntry.CreatedBy != Session["LoggedIn"].ToString())
                {
                    return RedirectToAction("Index");
                }
                db.Entry(dataEntry).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dataEntry);
        }

        // GET: DataEntries/Delete/5
        [RoleAuthorize("Admin")]

        public ActionResult Delete(int? id)
        {
            if(Session["Pass"] == null)
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
            DataEntry dataEntry = db.DataEntries.Find(id);
            if (dataEntry == null)
            {
                return HttpNotFound();
            }
            return View(dataEntry);
        }

        // POST: DataEntries/Delete/5
        // POST: DataEntries/Delete/5
        [RoleAuthorize("Admin")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DataEntry dataEntry = db.DataEntries.Find(id);
            db.DataEntries.Remove(dataEntry);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult GetMachines(string type)
        {
            List<Machine> machines = new List<Machine>();
            machines = db.Machines.Where(x => x.Type == type).ToList();

            double rate = 0;
            var e = db.Rates.Where(x => x.FuelType == type).OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if(e!=null)
            rate = Convert.ToDouble(e.RateValue);

            var obj = new { machines, rate };
            JavaScriptSerializer js = new JavaScriptSerializer();
            string result = js.Serialize(obj);
            return Json(result,JsonRequestBehavior.DenyGet);
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
