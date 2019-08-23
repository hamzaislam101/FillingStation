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
    public class CompaniesController : Controller
    {
        private StationContext db = new StationContext();

        // GET: Companies
        [RoleAuthorize("Admin")]

        public ActionResult Index()
        {
            return View(db.Companies.ToList());
        }

        // GET: Companies/Details/5
        [RoleAuthorize("Admin")]

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // GET: Companies/Create
        [RoleAuthorize("Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Admin")]

        public ActionResult Create([Bind(Include = "CompanyName,ContactNo,Dealer,CNIC,Address")] Company company)
        {
            if (ModelState.IsValid)
            {
                company.CreatedBy = Session["LoggedIn"].ToString();
                company.CreatedOn = DateTime.Now;
                db.Companies.Add(company);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(company);
        }

        // GET: Companies/Edit/5
        [RoleAuthorize("Admin")]

        public ActionResult Edit(int? id)
        {
            if(Session["Pass"] == null)
            {
                Session["Redirect"] = HttpContext.Request.Url.AbsoluteUri;
                return RedirectToAction("EnterSuperPassword", "Users");
            }
            else
            {
                Session["Pass"] = null;
                Session["Redirect"] = null;
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Company company = db.Companies.Find(id);
                if (company == null)
                {
                    return HttpNotFound();
                }
                return View(company);
            }
            
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Admin")]

        public ActionResult Edit([Bind(Include = "Id,CompanyName,ContactNo,Dealer,CNIC,Address")] Company company)
        {
            if (ModelState.IsValid)
            {
                db.Entry(company).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(company);
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
