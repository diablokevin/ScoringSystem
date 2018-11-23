using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ScoringSystem.Models;

namespace ScoringSystem.Controllers
{
    public class EventController : Controller
    {


        ScoringSystem.Models.ScoreDbContext db = new ScoringSystem.Models.ScoreDbContext();



        [ValidateInput(false)]
        public ActionResult EventCardViewPartial()
        {
            var model = db.Events.OrderBy(m => m.MenuOrder);
            return PartialView("_EventCardViewPartial", model.ToList());
        }

        [ValidateInput(false)]
        public ActionResult EventGridViewPartial()
        {
            var model = db.Events.OrderBy(m => m.MenuOrder);
            return PartialView("_EventGridViewPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EventGridViewPartialAddNew(ScoringSystem.Models.Event item)
        {
            var model = db.Events;
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
            return PartialView("_EventGridViewPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult EventGridViewPartialDelete(System.Int32 Id)
        {
            var model = db.Events;
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
            return PartialView("_EventGridViewPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult EventGridViewPartialUpdate(ScoringSystem.Models.Event item)
        {
            var model = db.Events;
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
            return PartialView("_EventGridViewPartial", model.ToList());
        }
        // GET: Event
        public ActionResult Index()
        {
            ViewBag.PageName = "Setting";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Multi()
        {
            if (ModelState.IsValid)
            {
                string content = Request["List"];


                List<string> t = content.Split('\r', '\n').ToList();
                ViewBag.Content = t;
                ViewBag.Count = t.Count;
                ViewBag.FaultCount = 0;
                ViewBag.SuccessCount = 0;
                int order = 0;
                if (db.Events.Count()>0)
                {
                    order = db.Events.Max(record => record.MenuOrder);
                }
             
                
                foreach (string item in t)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        try
                        {
                            Event @event = new Event();
                            @event.Name = item.Split('\t')[0];
                            @event.Pro = item.Split('\t')[1];
                            string[] timelimit = item.Split('\t')[2].Split(':');
                            int hour = Convert.ToInt32(timelimit[0]);
                            int min = Convert.ToInt32(timelimit[1]);
                            int sec = Convert.ToInt32(timelimit[2]);
                            @event.TimeLimit = new TimeSpan(hour, min, sec);
                            @event.MenuOrder = ++order;
                            db.Events.Add(@event);
                            
                        }
                        catch
                        {
                            ViewBag.FaultCount++;                       

                        }


                    }
                }
                ViewBag.SuccessCount =  db.SaveChanges();
                return View();
            }


            return View();
        }

        public ActionResult Multi(int? id)
        { return View(); }


    }


}