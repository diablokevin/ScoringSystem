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
        public ActionResult Index()
        {
            return View();


        }



        [ValidateInput(false)]
        public ActionResult ScoreGridViewPartial()
        {
            var model = db.Scores.Include(s => s.Schedule);
            //ViewBag.Eventlist = db.Events.ToList();
            //ViewBag.Companylist = db.Companies.ToList();
            //ViewBag.Schedulelist = db.Schedules.ToList();
            return PartialView("_ScoreGridViewPartial", model.ToList());
        }

        public ActionResult ScoreEditFormPartial(int? Id)
        {
            //ViewBag.Eventlist = db.Events.ToList();
            //ViewBag.Companylist = db.Companies.ToList();
            ViewBag.Schedulelist = db.Schedules.ToList();
            if (Id != null)
            { 
                var model = db.Scores.Find(Id);

                return PartialView("_ScoreEditFormPartial", model ?? new Models.Score());
            }
            return PartialView("_ScoreEditFormPartial", new Models.Score());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ScoreGridViewPartialAddNew(ScoringSystem.Models.Score item)
        {
            var model = db.Scores;
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
        public ActionResult ScoreGridViewPartialUpdate(ScoringSystem.Models.Score item)
        {
            var model = db.Scores;
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





        //[ValidateInput(false)]
        //public ActionResult ScoreJudgeGridViewPartial()
        //{
        //    var judgeStaffid = User.Identity.Name;
        //    var eventId=db.Judges.Where(j => j.StaffId == judgeStaffid).FirstOrDefault().EventId;
        //    var model = db.Scores.Where(s=>s.Schedule.EventId==eventId ).OrderBy(s=>s.Schedule.PlanBeginTime);
        //    return PartialView("_ScoreJudgeGridViewPartial", model.ToList());
        //}
        //public ActionResult ScoreJudgeEditFormPartial(int? Id)
        //{

        //    if (Id != null)
        //    {
        //        var model = db.Schedules.Find(Id);

        //        return PartialView("_ScoreJudgeEditFormPartial", model ?? new Models.Schedule());
        //    }
        //    return PartialView("_ScoreJudgeEditFormPartial", new Models.Schedule());

        //}
        //[HttpPost, ValidateInput(false)]
        //public ActionResult ScoreJudgeGridViewPartialAddNew(ScoringSystem.Models.Score item)
        //{
        //    var judgeStaffid = User.Identity.Name;
        //    var judge = db.Judges.Where(j => j.StaffId == judgeStaffid).FirstOrDefault();
        //    var eventId = judge.EventId;

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            item.JudgeId = judge.Id;
        //            item.JudgeTime = DateTime.Now;
        //            item.ModifyTime = DateTime.Now;
        //            db.Scores.Add(item);
        //            db.SaveChanges();
        //        }
        //        catch (Exception e)
        //        {
        //            ViewData["EditError"] = e.Message;
        //        }
        //    }
        //    else
        //        ViewData["EditError"] = "Please, correct all errors.";

        //    var model = db.Scores.Where(s => s.Schedule.EventId == eventId).OrderBy(s => s.Schedule.PlanBeginTime);
        //    return PartialView("_ScoreJudgeGridViewPartial", model.ToList());
        //}
        //[HttpPost, ValidateInput(false)]
        //public ActionResult ScoreJudgeGridViewPartialUpdate(ScoringSystem.Models.Score item)
        //{
        //    var judgeStaffid = User.Identity.Name;
        //    var judge = db.Judges.Where(j => j.StaffId == judgeStaffid).FirstOrDefault();
        //    var eventId = judge.EventId;

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var modelItem = db.Scores.FirstOrDefault(it => it.Id == item.Id);
        //            if (modelItem != null)
        //            {
        //                modelItem.JudgeId = judge.Id;
        //                if (modelItem.JudgeTime==null)
        //                {
        //                    modelItem.JudgeTime = DateTime.Now;
        //                }                            
        //                modelItem.ModifyTime = DateTime.Now;
        //                this.UpdateModel(modelItem);
        //                db.SaveChanges();
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            ViewData["EditError"] = e.Message;
        //        }
        //    }
        //    else
        //        ViewData["EditError"] = "Please, correct all errors.";

        //    var model = db.Scores.Where(s => s.Schedule.EventId == eventId).OrderBy(s => s.Schedule.PlanBeginTime);

        //    return PartialView("_ScoreJudgeGridViewPartial", model.ToList());
        //}
        //[HttpPost, ValidateInput(false)]
        //public ActionResult ScoreJudgeGridViewPartialDelete(System.Int32 Id)
        //{
        //    var model = db.Schedules;
        //    if (Id >= 0)
        //    {
        //        try
        //        {
        //            var item = model.FirstOrDefault(it => it.Id == Id);
        //            if (item != null)
        //                model.Remove(item);
        //            db.SaveChanges();
        //        }
        //        catch (Exception e)
        //        {
        //            ViewData["EditError"] = e.Message;
        //        }
        //    }
        //    return PartialView("_ScoreJudgeGridViewPartial", model.ToList());
        //}


        [ValidateInput(false)]
        public ActionResult ScoreFinishGridViewPartial()
        {
            var judgeStaffid = User.Identity.Name;
           // var eventId = db.Judges.Where(j => j.StaffId == judgeStaffid).FirstOrDefault().EventId;
            var model = db.Scores.Where(s => s.Judge.StaffId == judgeStaffid).OrderByDescending(s => s.JudgeTime);


            return PartialView("_ScoreFinishGridViewPartial", model.ToList());
        }

        [ValidateInput(false)]
        public ActionResult ScoreCompetitorGridViewPartial()
        {
            var competitorStaffid = User.Identity.Name;
            var competitor = db.Competitors.Where(j => j.StaffId == competitorStaffid).FirstOrDefault();

            //var model = db.Schedules.Where(s => s.CompetitorId == competitor.Id).OrderBy(s => s.PlanBeginTime);
            var model = db.Schedules.Where(s => s.CompetitorId == competitor.Id).Include(s => s.Scores).OrderBy(s => s.PlanBeginTime);
            var test = model.ToList();
            return PartialView("_ScoreCompetitorGridViewPartial", model.ToList());
        }

        public ActionResult ScoreJudgeEditFormPartial(int? ScheduleId)
        {

            if (ScheduleId != null)
            {
                var model = new Score();
                model.ScheduleId = ScheduleId.Value;
                model.Schedule = db.Schedules.Find(ScheduleId);

                return PartialView("_ScoreJudgeEditFormPartial", model ?? new Models.Score());
            }
            return PartialView("_ScoreJudgeEditFormPartial", new Models.Score());

        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult ScoreJudgeEditFormPartial(Score item)
        {

            if (ModelState.IsValid)
            {
                var judgeStaffid = User.Identity.Name;
                var judge = db.Judges.Where(j => j.StaffId == judgeStaffid).FirstOrDefault();
              
                item.JudgeId = judge.Id;
                item.JudgeTime = DateTime.Now;
                item.ModifyTime = DateTime.Now;
                db.Scores.Add(item);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return PartialView("_ScoreJudgeEditFormPartial", new Models.Score());

        }
    }
}