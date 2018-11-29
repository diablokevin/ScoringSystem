using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ScoringSystem.Models;

namespace ScoringSystem.Controllers
{
    public class JudgeController : Controller
    {
        // GET: Judge
        public ActionResult Index()
        {
            ViewBag.PageName = "Setting";
            return View();
        }

        ScoringSystem.Models.ScoreDbContext db = new ScoringSystem.Models.ScoreDbContext();

        [ValidateInput(false)]
        public ActionResult JudgeGridViewPartial()
        {
            ViewBag.Eventlist = db.Events.ToList();
            var model = db.Judges;
            return PartialView("_JudgeGridViewPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult JudgeGridViewPartialAddNew(ScoringSystem.Models.Judge item)
        {
            var model = db.Judges;
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
            return PartialView("_JudgeGridViewPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult JudgeGridViewPartialUpdate(ScoringSystem.Models.Judge item)
        {
            var model = db.Judges;
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
            return PartialView("_JudgeGridViewPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult JudgeGridViewPartialDelete(System.Int32 Id)
        {
            var model = db.Judges;
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
            return PartialView("_JudgeGridViewPartial", model.ToList());
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

                List<string> t = content.Split('\r', '\n').ToList();
                ViewBag.Count = t.Count;
                ViewBag.FaultCount = 0;
                ViewBag.SuccessCount = 0;
                foreach (string item in t)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        Judge judge = new Judge();
                        try
                        {
                            string staffid = item.Split('\t')[0];
                            string name = item.Split('\t')[1];
                            string eventName = item.Split('\t')[2];

                            judge.StaffId = staffid;
                            judge.Name = name;
                            judge.EventId = db.Events.Where(c => c.Name == eventName).First().Id;
                            db.Judges.Add(judge);

                        }
                        catch (Exception e)
                        {
                            ViewData["EditError"] = e.Message;
                            ViewBag.FaultCount++;
                        }

                    }
                }
                ViewBag.SuccessCount = db.SaveChanges();
                return View();
            }


            return View();
        }

    }
}