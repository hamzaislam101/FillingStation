using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FillingStationApp.Models;

namespace FillingStationApp.Helper
{
    public static class MainClass
    {
        static StationContext db = new StationContext();
        public static bool isUserAdmin()
        {
            if(HttpContext.Current.Session["LoggedIn"] != null)
            {
                var username = HttpContext.Current.Session["LoggedIn"].ToString();
                var user = db.Users.FirstOrDefault(x => x.Username == username);
                if(user.Type == "Admin")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public static string GetConnString()
        {
            return @"Server=HAMZAISLAM\SQLEXPRESS;Initial Catalog=FillingStation;Integrated Security=true;";
        }
    }
}