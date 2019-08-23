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
    public class MachinesController : Controller
    {
        private StationContext db = new StationContext();

        // GET: Machines
        [RoleAuthorize("Admin")]
        public ActionResult Index()
        {
            List<SelectListItem> typeList = new List<SelectListItem>();
            var mt = db.MachineTypes.Distinct().ToList();
            typeList.Add(new SelectListItem() { Text = "Select to search machines ...", Value = "" });
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
            return View(db.Machines.ToList());
        }

        // GET: Machines/Details/5
        [RoleAuthorize("Admin")]

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Machine machine = db.Machines.Find(id);
            if (machine == null)
            {
                return HttpNotFound();
            }
            return View(machine);
        }

        // GET: Machines/Create
        [RoleAuthorize("Admin")]

        public ActionResult Create()
        {
            List<SelectListItem> machineTypes = new List<SelectListItem>();
            var mt = db.MachineTypes.Distinct().ToList();
            foreach(var t in mt)
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

            var m = new Machine();
            var c = db.Machines.ToList();
            var max = 0;
            foreach(var a in c)
            {
                if (Convert.ToInt32(a.MachineNumber) > max)
                {
                    max = Convert.ToInt32(a.MachineNumber);
                }
            }
            m.MachineNumber = (max + 1).ToString();
            return View(m);
        }

        // POST: Machines/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Admin")]

        public ActionResult Create([Bind(Include = "MachineNumber,Type,IsActive")] Machine machine)
        {
            if (ModelState.IsValid)
            {
                var m = db.Machines.Where(x => x.MachineNumber == machine.MachineNumber).Count();
                if (m > 0)
                {
                    List<SelectListItem> macTypes = new List<SelectListItem>();
                    var mts = db.MachineTypes.Distinct().ToList();
                    foreach (var t in mts)
                    {
                        macTypes.Add(new SelectListItem() { Text = t.MType, Value = t.MType });
                    }
                    if (mts.Count > 0)
                    {
                        ViewBag.TypeList = new SelectList(macTypes, "Value", "Text", macTypes[0]);
                    }
                    else
                    {
                        ViewBag.TypeList = new SelectList(macTypes, "Value", "Text");
                    }
                    ViewBag.ErrorMessage = "Machine number already exists";
                    return View(machine);
                }
                else
                {
                    machine.CreatedBy = Session["LoggedIn"].ToString();
                    machine.CreatedOn = DateTime.Now;
                    db.Machines.Add(machine);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                
            }
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
            return View(machine);
        }

        // GET: Machines/Edit/5
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
            Machine machine = db.Machines.Find(id);
            if (machine == null)
            {
                return HttpNotFound();
            }
            return View(machine);
        }

        // POST: Machines/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Admin")]

        public ActionResult Edit([Bind(Include = "Id,MachineNumber,Type,IsActive")] Machine machine)
        {
            if (ModelState.IsValid)
            {
                db.Entry(machine).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(machine);
        }

        // GET: Machines/Delete/5
        [RoleAuthorize("Admin")]

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
            Machine machine = db.Machines.Find(id);
            if (machine == null)
            {
                return HttpNotFound();
            }
            return View(machine);
        }

        // POST: Machines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Admin")]

        public ActionResult DeleteConfirmed(int id)
        {
            Machine machine = db.Machines.Find(id);
            db.Machines.Remove(machine);
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
