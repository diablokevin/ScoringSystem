using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ScoringSystem.Models;

namespace ScoringSystem.Controllers
{
    public class CompetitorController : Controller
    {
        // GET: Competitor
        public ActionResult Index()
        {
            ViewBag.PageName = "Setting";
            return View();
        }

        ScoringSystem.Models.ScoreDbContext db = new ScoringSystem.Models.ScoreDbContext();

        [ValidateInput(false)]
        public ActionResult CompetitorGridViewPartial()
        {
            var model = db.Competitors;
            ViewBag.CompanyList = db.Companies.ToList();
            return PartialView("_CompetitorGridViewPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CompetitorGridViewPartialAddNew(ScoringSystem.Models.Competitor item)
        {
            var model = db.Competitors;
            if (ModelState.IsValid)
            {
                try
                {
                    model.Add(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_CompetitorGridViewPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CompetitorGridViewPartialUpdate(ScoringSystem.Models.Competitor item)
        {
            var model = db.Competitors;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = model.FirstOrDefault(it => it.Id == item.Id);
                    if (modelItem != null)
                    {
                        this.UpdateModel(modelItem);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_CompetitorGridViewPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CompetitorGridViewPartialDelete(System.Int32 Id)
        {
            var model = db.Competitors;
            if (Id >= 0)
            {
                try
                {
                    var item = model.FirstOrDefault(it => it.Id == Id);
                    if (item != null)
                        model.Remove(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_CompetitorGridViewPartial", model.ToList());
        }

        public ActionResult Multi(int? id)
        { return View(); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Multi()
        {
            if (ModelState.IsValid)
            {
                string content = Request["List"];
                ViewBag.Content = content;

                List<string> t = content.Split('\r','\n').ToList();
                ViewBag.Count = t.Count;

                foreach (string item in t)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                         Competitor competitor = new Competitor();
                        try
                        {
                            string company = item.Split('\t')[0];
                            string name = item.Split('\t')[1];
                            string staffid = item.Split('\t')[2];
                            string pro = item.Split('\t')[3];
                            string racenum = item.Split('\t')[4];
                            IQueryable<Company> companies = db.Companies.Where(x => x.Name.Contains(company));
                            competitor.CompanyId = companies.FirstOrDefault().Id;
                            competitor.Name = name;
                            competitor.StaffId = staffid;
                            competitor.Pro = pro;
                            competitor.Race_num = racenum;

                            db.Competitors.Add(competitor);
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            ViewData["EditError"] = e.Message;
                        }

                    }
                }
                return View();
            }


            return View();
        }

        public ActionResult GetCompetitorsByCompanyId(int id)
        {
            var competitors = db.Competitors.Where(x => x.CompanyId == id).Select(x => new { x.Id, x.Name }).ToList();
           
            return GridViewExtension.GetComboBoxCallbackResult(p => {
                p.TextField = "Name";
                p.ValueField = "Id";
                p.BindList(competitors);
            });
        }
    }
}