using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ScoringSystem.Models;

namespace ScoringSystem.Controllers
{
    public class CompanyController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.PageName = "Setting";
            return View();
        }

        ScoringSystem.Models.ScoreDbContext db = new ScoringSystem.Models.ScoreDbContext();

        [ValidateInput(false)]
        public ActionResult CompanyGridViewPartial()
        {
            var model = db.Companies;
            return PartialView("_CompanyGridViewPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CompanyGridViewPartialAddNew(ScoringSystem.Models.Company item)
        {
            var model = db.Companies;
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
            return PartialView("_CompanyGridViewPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CompanyGridViewPartialUpdate(ScoringSystem.Models.Company item)
        {
            var model = db.Companies;
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
            return PartialView("_CompanyGridViewPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CompanyGridViewPartialDelete(System.Int32 Id)
        {
            var model = db.Companies;
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
            return PartialView("_CompanyGridViewPartial", model.ToList());
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

                List<string> t = content.Split('\n','\r').ToList();
                ViewBag.Count = t.Count;

                foreach (string name in t)
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        Company com = new Company();
                        com.Name = name;
                        db.Companies.Add(com);
                        db.SaveChanges();
                    }
                }
                return View();
            }


            return View();
        }

    }
}