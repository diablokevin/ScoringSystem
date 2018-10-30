using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScoringSystem.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // DXCOMMENT: Pass a data model for GridView
            ViewBag.PageName = "Main";
            return View();
        }
        
    }
}

public enum HeaderViewRenderMode { Full, Title }