using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FillingStationApp.Models;
using FillingStationApp.Helper;
using System.Web.Script.Serialization;

namespace FillingStationApp.Controllers
{
    [RoleAuthorize("Admin")]
    public class RatesController : Controller
    {
        private StationContext db = new StationContext();

        // GET: Rates
        public ActionResult Index()
        {
            return View(db.Rates.OrderByDescending(x=>x.AddedOn).ToList());
        }

        // GET: Rates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rate rate = db.Rates.Find(id);
            if (rate == null)
            {
                return HttpNotFound();
            }
            return View(rate);
        }

        // GET: Rates/Create
        public ActionResult Create()
        {
            List<SelectListItem> machineTypes = new List<SelectListItem>();
            var mt = db.MachineTypes.Distinct().ToList();
            foreach (var t in mt)
            {
                machineTypes.Add(new SelectListItem() { Text = t.MType, Value = t.MType });
            }
            if (mt.Count > 0)
            {
                ViewBag.TypeList = new SelectList(machineTypes, "Value", "Text", machineTypes[0]);
            }
            else
            {
                ViewBag.TypeList = new SelectList(machineTypes, "Value", "Text");
            }
            var stocks = new List<StockTracker>();
            
            if (mt.Count > 0)
            {
                var type = mt[0].MType;
                stocks = db.StockTrackers.Where(x => x.RemainingFuelAmount > 0 && x.FuelType == type).ToList();
            }
            decimal sum = 0;
            foreach(var s in stocks)
            {
                sum += s.RemainingFuelAmount;
            }
            ViewBag.TotalStock = sum;
            ViewBag.StockDetail = stocks;

            var r = new Rate();
            r.RateDate = DateTime.Now;

            return View(r);
        }

        // POST: Rates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,RateDate,FuelType,RateValue")] Rate rate)
        {
            if (ModelState.IsValid)
            {
                rate.AddedBy = Session["LoggedIn"].ToString();
                rate.AddedOn = DateTime.Now;
                db.Rates.Add(rate);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rate);
        }

        // GET: Rates/Edit/5
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
            Rate rate = db.Rates.Find(id);
            if (rate == null)
            {
                return HttpNotFound();
            }
            return View(rate);
        }

        // POST: Rates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,RateDate,FuelType,RateValue")] Rate rate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rate);
        }

        // GET: Rates/Delete/5
        public ActionResult Delete(int? id)
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
            Rate rate = db.Rates.Find(id);
            if (rate == null)
            {
                return HttpNotFound();
            }
            return View(rate);
        }

        // POST: Rates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rate rate = db.Rates.Find(id);
            db.Rates.Remove(rate);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult GetRates(string type)
        {
            var stocks = new List<StockTracker>();
            stocks = db.StockTrackers.Where(x => x.RemainingFuelAmount > 0 && x.FuelType == type).ToList();
            decimal sum = 0;
            foreach (var s in stocks)
            {
                sum += s.RemainingFuelAmount;
            }

            var obj = new { stocks };

            JavaScriptSerializer js = new JavaScriptSerializer();
            string result = js.Serialize(obj);
            return Json(result, JsonRequestBehavior.DenyGet);
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
