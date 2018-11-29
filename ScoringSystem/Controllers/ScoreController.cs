using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using ScoringSystem.Models;
using DevExpress.Web.Mvc;

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
            ViewBag.Eventlist = db.Events.ToList();
            ViewBag.Companylist = db.Companies.ToList();
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
                    item.JudgeTime = DateTime.Now;
                    item.ModifyTime = DateTime.Now;
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
                        modelItem.ModifyTime = DateTime.Now;
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
                        Schedule schedule = new Schedule();
                        try
                        {
                            string date =  item.Split('\t')[0];
                            string eventName = item.Split('\t')[1];
                            string competitorNum = item.Split('\t')[2];
                            string  begin = item.Split('\t')[3];
                            string end = item.Split('\t')[4];

                            schedule.PlanBeginTime = Convert.ToDateTime(date + " " + begin);
                            schedule.PlanEndTime = Convert.ToDateTime(date + " " + end);
                            schedule.EventId = db.Events.Where(c => c.Name == eventName).First().Id;
                            var testData = db.Competitors.ToList();
                            var competitors= db.Competitors.Where(c => c.Race_num == competitorNum);

                            schedule.CompetitorId = competitors.FirstOrDefault().Id;
                            db.Schedules.Add(schedule);

                            
                        }
                        catch (Exception e)
                        {
                            ViewData["EditError"] = e.Message;
                        }
                        finally
                        {
                            ViewBag.SuccessCount= db.SaveChanges();
                        }

                    }
                }
                return View();
            }


            return View();
        }

        public ActionResult ScheduleIndex()
        {
            return View();
        }


        [ValidateInput(false)]
        public ActionResult ScheduleGridViewPartial()
        {
            var model = db.Schedules;
            return PartialView("_ScheduleGridViewPartial", model.ToList());
        }
    }
}