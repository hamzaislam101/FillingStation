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
using FillingStationApp.ViewModel;

namespace FillingStationApp.Controllers
{
    public class UsersController : Controller
    {
        private StationContext db = new StationContext();

        // GET: Users
        [RoleAuthorize("Admin")]

        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        public ActionResult Logout()
        {
            if (Session["LoggedIn"] != null)
            {
                Session["LoggedIn"] = null;
            }

            return RedirectToAction("Login");
        }

        [RoleAuthorize("Admin")]
        public ActionResult EnterSuperPassword()
        {
            return View();
        }

        [HttpPost]
        [RoleAuthorize("Admin")]
        public ActionResult EnterSuperPassword(SuperPassword superPassword)
        {
            if(Session["Redirect"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (string.IsNullOrEmpty(superPassword.SuperPass))
            {
                ViewBag.ErrorMessage = "Incorrect SuperPassword";
                return View();
            }
            var username = Session["LoggedIn"].ToString();
            var user = db.Users.Where(x => x.Username == username).FirstOrDefault();
            if(MainClass.isUserAdmin() && user.SuperPassword == superPassword.SuperPass)
            {
                var redirect = Session["Redirect"].ToString();
                Session["Pass"] = "ok";
                return Redirect(redirect);
            }
            else
            {
                ViewBag.ErrorMessage = "Incorrect SuperPassword";
                return View();
            }
            
        }

        public ActionResult Login()
        {
            ViewBag.ErrorMessage = "";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "Username,Password")] LoginViewModel user)
        {
            if (ModelState.IsValid)
            {
                var loggedIn = db.Users.FirstOrDefault(x => x.Username == user.Username && x.Password == user.Password);
                if (loggedIn == null)
                {
                    ViewBag.ErrorMessage = "Incorrect Username or Password";
                    return View();
                }
                else
                {
                    Session["LoggedIn"] = loggedIn.Username;
                    Session["UserType"] = db.Users.FirstOrDefault(x => x.Username == loggedIn.Username).Type;
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Please provide username and password";
                return View();
            }

        }

        // GET: Users/Details/5
        [RoleAuthorize("Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        [RoleAuthorize("Admin")]

        public ActionResult Create()
        {
            if (Session["Pass"] == null)
            {
                Session["Redirect"] = HttpContext.Request.Url.AbsoluteUri;
                return RedirectToAction("EnterSuperPassword", "Users");
            }
            Session["Pass"] = null;
            Session["Redirect"] = null;
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "Admin", Value = "Admin", Selected = true });
            items.Add(new SelectListItem { Text = "Local", Value = "Local" });
            ViewBag.TypeList = new SelectList(items, "Value", "Text", "Admin");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Admin")]

        public ActionResult Create([Bind(Include = "Name,Username,Password,Type")] User user)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "Admin", Value = "Admin", Selected = true });
            items.Add(new SelectListItem { Text = "Local", Value = "Local" });
            ViewBag.TypeList = new SelectList(items, "Value", "Text", "Admin");
            if (ModelState.IsValid)
            {
                var check = db.Users.Any(x => x.Username == user.Username);
                if (check)
                {
                    ViewBag.ErrorMessage = "Username already Exists.";
                    return View(user);
                }
                else
                {
                    if(user.Type == "Local")
                    {
                        user.SuperPassword = null;
                    }
                    user.CreatedBy = Session["LoggedIn"].ToString();
                    user.CreatedOn = DateTime.Now;
                    db.Users.Add(user);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

            }

            return View(user);
        }

        // GET: Users/Edit/5
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
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "Admin", Value = "Admin", Selected = true });
            items.Add(new SelectListItem { Text = "Local", Value = "Local" });
            ViewBag.TypeList = new SelectList(items, "Value", "Text", "Admin");
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Admin")]

        public ActionResult Edit([Bind(Include = "Id,Name,Username,Password,Type")] User user)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "Admin", Value = "Admin", Selected = true });
            items.Add(new SelectListItem { Text = "Local", Value = "Local" });
            ViewBag.TypeList = new SelectList(items, "Value", "Text", "Admin");
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
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
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Admin")]

        public ActionResult DeleteConfirmed(int id)
        {
            var users = db.Users.Where(x => x.Type == "Admin").Count();
            if (users > 1)
            {
                User user = db.Users.Find(id);
                db.Users.Remove(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "You cannont delete the last Admin account.");
            }

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
