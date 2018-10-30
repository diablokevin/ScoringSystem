using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScoringSystem.Controllers
{
    public class ScoreController : Controller
    {
        // GET: Score
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Index(int eventID)
        {
            var scores = from score in db.Schedules
                         where score.EventId == eventID
                         select score;

            ViewBag.eventID = eventID;

         
            ViewBag.eventTitle = db.Events.Find(eventID).Name + "|" + db.Events.Find(eventID).Pro;
            return View(scores.ToList().OrderByDescending(x => x.JudgeTime));

         
        }

        ScoringSystem.Models.ScoreDbContext db = new ScoringSystem.Models.ScoreDbContext();

        [ValidateInput(false)]
        public ActionResult ScoreGridViewPartial()
        {
            var model = db.Schedules;
            return PartialView("_ScoreGridViewPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ScoreGridViewPartialAddNew(ScoringSystem.Models.Schedule item)
        {
            var model = db.Schedules;
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
            return PartialView("_ScoreGridViewPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ScoreGridViewPartialUpdate(ScoringSystem.Models.Schedule item)
        {
            var model = db.Schedules;
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
            return PartialView("_ScoreGridViewPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ScoreGridViewPartialDelete(System.Int32 Id)
        {
            var model = db.Schedules;
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
            return PartialView("_ScoreGridViewPartial", model.ToList());
        }

    }
}