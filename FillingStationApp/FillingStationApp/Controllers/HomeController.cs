using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FillingStationApp.Helper;
using FillingStationApp.Models;

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

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}