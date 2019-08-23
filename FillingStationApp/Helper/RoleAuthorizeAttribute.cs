using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FillingStationApp.Models;

namespace FillingStationApp.Helper
{
    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        StationContext db = new StationContext();
        private readonly string[] allowedroles;
        public RoleAuthorizeAttribute(params string[] roles)
        {
            this.allowedroles = roles;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;
            if(HttpContext.Current.Session["LoggedIn"] == null)
            {
                authorize = false;
            }
            else
            {
                var username = HttpContext.Current.Session["LoggedIn"].ToString();
                var user = db.Users.FirstOrDefault(x => x.Username == username);
                if (allowedroles.Contains(user.Type))
                {
                    authorize = true;
                }
            }
            
            return authorize;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if(HttpContext.Current.Session["LoggedIn"] == null)
            {
                filterContext.Result = new RedirectResult("~/Users/Login");
            }
            else
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
            
        }
    }
}