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

namespace FillingStationApp.Controllers
{
    [RoleAuthorize("Admin")]
    public class MachineTypesController : Controller
    {
        private StationContext db = new StationContext();

        // GET: MachineTypes
        public ActionResult Index()
        {
            return View(db.MachineTypes.ToList());
        }

        // GET: MachineTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MachineType machineType = db.MachineTypes.Find(id);
            if (machineType == null)
            {
                return HttpNotFound();
            }
            return View(machineType);
        }

        // GET: MachineTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MachineTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,MType")] MachineType machineType)
        {
            if (ModelState.IsValid)
            {
                var mt = db.MachineTypes.Where(x => x.MType == machineType.MType).Count();
                if (mt > 0)
                {
                    ViewBag.ErrorMessage = "Machine Type Already Exists";
                    return View(machineType);
                }
                else
                {
                    db.MachineTypes.Add(machineType);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                
            }

            return View(machineType);
        }

        // GET: MachineTypes/Edit/5
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
            MachineType machineType = db.MachineTypes.Find(id);
            if (machineType == null)
            {
                return HttpNotFound();
            }
            return View(machineType);
        }

        // POST: MachineTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,MType")] MachineType machineType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(machineType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(machineType);
        }

        // GET: MachineTypes/Delete/5
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
            MachineType machineType = db.MachineTypes.Find(id);
            if (machineType == null)
            {
                return HttpNotFound();
            }
            return View(machineType);
        }

        // POST: MachineTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MachineType machineType = db.MachineTypes.Find(id);
            db.MachineTypes.Remove(machineType);
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
