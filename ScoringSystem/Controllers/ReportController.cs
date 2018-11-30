using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScoringSystem.Controllers
{
    public class ReportController : Controller
    {
         ScoringSystem.Models.ScoreDbContext db = new ScoringSystem.Models.ScoreDbContext();
        // GET: Report
        public ActionResult Index()
        {
            ViewBag.PageName = "Report";
            return View();
        }

        [ValidateInput(false)]
        public ActionResult CompanyRankReporGridViewPartial()
        {
            var model = db.Scores;
            var data = from score in db.Scores
                       group score by score.Schedule.Competitor.Company into g
                       select new {
                           g.Key,Score=g.Sum(p=>p.Mark)
                       };
                     
            return PartialView("_CompanyRankReporGridViewPartial", data);
        }
    }
}