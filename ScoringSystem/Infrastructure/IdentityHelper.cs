using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
namespace ScoringSystem.Infrastructure
{
    public static class IdentityHelper
    {
        public static MvcHtmlString GetUserName(this HtmlHelper html, string id)
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            return new MvcHtmlString(userManager.FindByIdAsync(id).Result.UserName);
        }
        public static MvcHtmlString GetRealName(this HtmlHelper html, string id)
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            return new MvcHtmlString(userManager.FindByIdAsync(id).Result.RealName);
        }
    }

    public static class MyHandler
    {
        public static string IdentityResultErrorsToString(IdentityResult result)
        {
            string s = "";
            foreach (var error in result.Errors)
            {
                s += error + "\n";
            }
            return s;
        }
    }

 
}