using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
namespace ScoringSystem.Controllers
{
    public class ScoreController : Controller
    {
        // GET: Score
        ScoringSystem.Models.ScoreDbContext db = new ScoringSystem.Models.ScoreDbContext();
        public ActionResult Index(int? eventID)
        {
            if (eventID == null)
            {
                var scores = db.Schedules.Include(s => s.Event).Include(s => s.Competitor);
                return View(scores.ToList().OrderByDescending(x => x.JudgeTime));
            }
            else
            {
                var scores = db.Schedules.Where(s => s.EventId == eventID);
                ViewBag.eventID = eventID;
                ViewBag.eventTitle = db.Events.Find(eventID).Name + "|" + db.Events.Find(eventID).Pro;
                return View(scores.ToList().OrderByDescending(x => x.JudgeTime));
            }
            //var scores = from score in db.Schedules
            //             where score.EventId == eventID
            //             select score;

            //ViewBag.eventID = eventID;


            //ViewBag.eventTitle = db.Events.Find(eventID).Name + "|" + db.Events.Find(eventID).Pro;
            //return View(scores.ToList().OrderByDescending(x => x.JudgeTime));


        }



        [ValidateInput(false)]
        public ActionResult ScoreGridViewPartial()
        {
            var model = db.Schedules;
           // ViewBag.Eventlist = db.Events.ToList();
            return PartialView("_ScoreGridViewPartial", model.ToList());
        }

        public ActionResult ScoreEditFormPartial(int? Id)
        {
            ViewBag.Eventlist = db.Events.ToList();
            ViewBag.Companylist = db.Companies.ToList();
            if (Id != null)
            { 
                var model = db.Schedules.Find(Id);

                return PartialView("_ScoreEditFormPartial", model ?? new Models.Schedule());
            }
            return PartialView("_ScoreEditFormPartial", new Models.Schedule());

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