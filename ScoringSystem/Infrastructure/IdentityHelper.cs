using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;

namespace ScoringSystem.Infrastructure
{
    public static class IdentityHelper
    {
        public static MvcHtmlString GetUserName(this HtmlHelper html, string id)
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            return new MvcHtmlString(userManager.FindByIdAsync(id).Result.UserName);
        }
    }
}