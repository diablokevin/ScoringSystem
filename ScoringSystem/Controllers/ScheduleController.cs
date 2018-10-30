using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScoringSystem.Controllers
{
    public class ScheduleController : Controller
    {
        // GET: Schedule
        public ActionResult Index()
        {
            ViewBag.PageName = "Main";
            return View();
        }

        //Scheduler

        ScoringSystem.Models.ScoreDbContext appointmentContext = new ScoringSystem.Models.ScoreDbContext();
        ScoringSystem.Models.ScoreDbContext resourceContext = new ScoringSystem.Models.ScoreDbContext();

        public ActionResult SchedulerPartial()
        {
            var appointments = appointmentContext.Schedules;
            var resources = resourceContext.Schedules;

            ViewData["Appointments"] = appointments.ToList();
            ViewData["Resources"] = resources.ToList();

            return PartialView("_SchedulerPartial");
        }
        public ActionResult SchedulerPartialEditAppointment()
        {
            var appointments = appointmentContext.Schedules;
            var resources = resourceContext.Schedules;

            try
            {
                ScoreControllerSchedulerSettings.UpdateEditableDataObject(appointmentContext, resourceContext);
            }
            catch (Exception e)
            {
                ViewData["SchedulerErrorText"] = e.Message;
            }

            ViewData["Appointments"] = appointments.ToList();
            ViewData["Resources"] = resources.ToList();

            return PartialView("_SchedulerPartial");
        }
    }
    public class ScoreControllerSchedulerSettings
    {
        static DevExpress.Web.Mvc.MVCxAppointmentStorage appointmentStorage;
        public static DevExpress.Web.Mvc.MVCxAppointmentStorage AppointmentStorage
        {
            get
            {
                if (appointmentStorage == null)
                {
                    appointmentStorage = new DevExpress.Web.Mvc.MVCxAppointmentStorage();
                    appointmentStorage.Mappings.AppointmentId = "Id";
                    appointmentStorage.Mappings.Start = "PlanBeginTime";
                    appointmentStorage.Mappings.End = "PlanEndTime";
                    appointmentStorage.Mappings.Subject = "Subject";
                    appointmentStorage.Mappings.Description = "";
                    appointmentStorage.Mappings.Location = "";
                    appointmentStorage.Mappings.AllDay = "";
                    appointmentStorage.Mappings.Type = "EventId";
                    appointmentStorage.Mappings.RecurrenceInfo = "";
                    appointmentStorage.Mappings.ReminderInfo = "";
                    appointmentStorage.Mappings.Label = "";
                    appointmentStorage.Mappings.Status = "";
                    appointmentStorage.Mappings.ResourceId = "";
                }
                return appointmentStorage;
            }
        }

        static DevExpress.Web.Mvc.MVCxResourceStorage resourceStorage;
        public static DevExpress.Web.Mvc.MVCxResourceStorage ResourceStorage
        {
            get
            {
                if (resourceStorage == null)
                {
                    resourceStorage = new DevExpress.Web.Mvc.MVCxResourceStorage();
                    resourceStorage.Mappings.ResourceId = "Id";
                    resourceStorage.Mappings.Caption = "Subject";
                }
                return resourceStorage;
            }
        }

        public static void UpdateEditableDataObject(ScoringSystem.Models.ScoreDbContext appointmentContext, ScoringSystem.Models.ScoreDbContext resourceContext)
        {
            InsertAppointments(appointmentContext, resourceContext);
            UpdateAppointments(appointmentContext, resourceContext);
            DeleteAppointments(appointmentContext, resourceContext);
        }

        static void InsertAppointments(ScoringSystem.Models.ScoreDbContext appointmentContext, ScoringSystem.Models.ScoreDbContext resourceContext)
        {
            var appointments = appointmentContext.Schedules.ToList();
            var resources = resourceContext.Schedules;

            var newAppointments = DevExpress.Web.Mvc.SchedulerExtension.GetAppointmentsToInsert<ScoringSystem.Models.Schedule>("Scheduler", appointments, resources,
                AppointmentStorage, ResourceStorage);
            foreach (var appointment in newAppointments)
            {
                appointmentContext.Schedules.Add(appointment);
            }
            appointmentContext.SaveChanges();
        }
        static void UpdateAppointments(ScoringSystem.Models.ScoreDbContext appointmentContext, ScoringSystem.Models.ScoreDbContext resourceContext)
        {
            var appointments = appointmentContext.Schedules.ToList();
            var resources = resourceContext.Schedules;

            var updAppointments = DevExpress.Web.Mvc.SchedulerExtension.GetAppointmentsToUpdate<ScoringSystem.Models.Schedule>("Scheduler", appointments, resources,
                AppointmentStorage, ResourceStorage);
            foreach (var appointment in updAppointments)
            {
                var origAppointment = appointments.FirstOrDefault(a => a.Id == appointment.Id);
                appointmentContext.Entry(origAppointment).CurrentValues.SetValues(appointment);
            }
            appointmentContext.SaveChanges();
        }

        static void DeleteAppointments(ScoringSystem.Models.ScoreDbContext appointmentContext, ScoringSystem.Models.ScoreDbContext resourceContext)
        {
            var appointments = appointmentContext.Schedules.ToList();
            var resources = resourceContext.Schedules;

            var delAppointments = DevExpress.Web.Mvc.SchedulerExtension.GetAppointmentsToRemove<ScoringSystem.Models.Schedule>("Scheduler", appointments, resources,
                AppointmentStorage, ResourceStorage);
            foreach (var appointment in delAppointments)
            {
                var delAppointment = appointments.FirstOrDefault(a => a.Id == appointment.Id);
                if (delAppointment != null)
                    appointmentContext.Schedules.Remove(delAppointment);
            }
            appointmentContext.SaveChanges();
        }
    }
}
