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
        public ActionResult Index()
        {
            return View(db.Stocks.ToList());
        }

        // GET: Stocks/Details/5
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

            return View();
        }

        // POST: Stocks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Admin")]

        public ActionResult Create([Bind(Include = "Company,Type,Quantity,DealingPerson,CNIC,PhoneNo,Address")] Stock stock)
        {
            if (ModelState.IsValid)
            {
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
            return View(stock);
        }

        // GET: Stocks/Edit/5
        [RoleAuthorize("Admin")]

        public ActionResult Edit(int? id)
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

        // GET: Stocks/Delete/5
        [RoleAuthorize("Admin")]

        public ActionResult Delete(int? id)
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

        // POST: Stocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Admin")]

        public ActionResult DeleteConfirmed(int id)
        {
            Stock stock = db.Stocks.Find(id);
            db.Stocks.Remove(stock);
            db.SaveChanges();
            return RedirectToAction("Index");
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
