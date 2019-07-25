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
        public ActionResult Index()
        {
            return View(db.DataEntries.ToList());
        }

        // GET: DataEntries/Details/5
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
        public ActionResult Create()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            var types = db.Stocks.Select(x => x.Type).Distinct().ToList();
            foreach(var type in types)
            {
                items.Add(new SelectListItem { Text = type, Value = type});
            }
            if (types.Count > 0)
            {
                ViewBag.EntryTypeList = new SelectList(items, "Value", "Text", types[0]);
            }
            else
                ViewBag.EntryTypeList = new SelectList(items, "Value", "Text");

            var machines = new List<Machine>();

            if (types.Count > 0)
            {
                var type = types[0];
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
            return View();
        }

        // POST: DataEntries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Type,MachineNumber,CurrentReading,CashRecieved")] DataEntry dataEntry)
        {
            if (ModelState.IsValid)
            {
                dataEntry.CreatedBy = Session["LoggedIn"].ToString();
                dataEntry.CreatedOn = DateTime.Now.ToString();
                db.DataEntries.Add(dataEntry);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            List<SelectListItem> items = new List<SelectListItem>();
            var types = db.Stocks.Select(x => x.Type).Distinct().ToList();
            foreach (var type in types)
            {
                items.Add(new SelectListItem { Text = type, Value = type });
            }
            if(types.Count>0)
            ViewBag.EntryTypeList = new SelectList(items, "Value", "Text", types[0]);
            else
                ViewBag.EntryTypeList = new SelectList(items, "Value", "Text");


            var machines = new List<Machine>();

            if (types.Count > 0)
            {
                machines = db.Machines.Where(x => x.Type == types[0]).ToList();
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
        [RoleAuthorize("Admin")]

        public ActionResult Edit(int? id)
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

        // POST: DataEntries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Admin")]

        public ActionResult Edit([Bind(Include = "Id,Type,MachineNumber,CurrentReading,CashRecieved")] DataEntry dataEntry)
        {
            if (ModelState.IsValid)
            {
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

            JavaScriptSerializer js = new JavaScriptSerializer();
            string result = js.Serialize(machines);
            return Json(result,JsonRequestBehavior.AllowGet);
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
