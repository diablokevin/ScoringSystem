using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ScoringSystem.Models;

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

        public ActionResult CompanyRank()
        {
            ViewBag.PageName = "Report";
            return View();
        }

        [ValidateInput(false)]
        public ActionResult CompanyRankReporGridViewPartial()
        {
            var model = GetPersonalRank("","");
           

            var data = from personal in model
                       group personal by personal.Company into g   
                       orderby g.Sum(p => p.TotalScore) descending
                       select new
                       {
                         
                           CompanyName = g.FirstOrDefault().Company,
                           Score = g.Sum(p => p.TotalScore),
                           Count=g.Sum(p=>p.Count)
                       }
                       ;
            var table = data.AsEnumerable().Select((score, index) => new CompanyRank {
                CompanyName = score.CompanyName,
                Score = score.Score.Value,
                Count=score.Count,
                Rank = index + 1
            });
                     
            return PartialView("_CompanyRankReporGridViewPartial", table.ToList());
        }

        [ValidateInput(false)]
        public ActionResult CompanyRankDetailGridViewPartial(string company)
        {
          
            ViewBag.Company = company;
            List<PersonalRank> list = GetPersonalRank(null, company);

            return PartialView("_CompanyRankDetailGridViewPartial", list);
        }
        public ActionResult EventRank_ME()
        {
            ViewBag.PageName = "Report";
            ViewBag.Pro = "ME";
            ViewBag.EventList = db.Events.Where(t=>t.Pro.Contains("机电")).ToList();
            return View();
        }
        public ActionResult EventRank_AV()
        {
            ViewBag.PageName = "Report";
            ViewBag.Pro = "AV";
            ViewBag.EventList = db.Events.Where(t => t.Pro.Contains("电子")).ToList();
            return View();
        }
        [ValidateInput(false)]
        public ActionResult EventRankGridViewPartial(int? eventId,string pro, string time)
        {
           
            var data = db.Schedules.Where(t=>true);
            if (eventId!=null)
            {
                data = data.Where(s => s.EventId == eventId);
            }
            if(!string.IsNullOrEmpty(pro))
            {
                data = data.Where(s => s.Competitor.Pro == pro);
            }
            if (!string.IsNullOrEmpty( time))
            {
                try
                {
                    DateTime endTime = Convert.ToDateTime(time);
                    data = data.Where(s => s.PlanBeginTime <= endTime);
                }
                finally
                {

                }
               
            }

            ViewBag.Pro = pro;
            Event @event = db.Events.Find(eventId);
            ViewBag.EventName = @event == null?"":@event.Name;

            var table = data.AsEnumerable().OrderByDescending(s => s.MarkAVG).Select((schedule, Index) => new EventRank {
                Competitor_Company = schedule.Competitor.Company.Name,
                Competitor_Name=schedule.Competitor.Name,
                Competitor_Pro=schedule.Competitor.Pro,
                Competitor_Staffid=schedule.Competitor.StaffId,
                EventName=schedule.Event.Name,
                Score=schedule.MarkAVG,
                Rank=Index+1
            });

            return PartialView("_EventRankGridViewPartial", table.ToList());
        }

       

        [ValidateInput(false)]
        public ActionResult ScorePivotGridPartial()
        {
            var model = db.Schedules;
            return PartialView("_ScorePivotGridPartial", model.ToList());
        }



        public ActionResult PersonalAVRank()
        {
            ViewBag.PageName = "Report";
           
            return View();
        }

        public ActionResult PersonalMERank()
        {
            ViewBag.PageName = "Report";
           
            return View();
        }

        [ValidateInput(false)]
        public ActionResult PersonalRankGridViewPartial(string pro,string company)
        {
            ViewBag.Pro = pro;
            ViewBag.Company = company;
            List<PersonalRank> list = GetPersonalRank(pro,company);

            return PartialView("_PersonalRankGridViewPartial", list);
        }

        private List<PersonalRank> GetPersonalRank(string pro, string company)
        {
            var data = db.Schedules.Where(t => true);
            if (!string.IsNullOrEmpty(pro))
            {
                data = data.Where(s => s.Competitor.Pro == pro);
            }
            if (!string.IsNullOrEmpty(company))
            {
                data = data.Where(s => s.Competitor.Company.Name == company);
            }

            var data2 = from schedule in data
                        group schedule by schedule.CompetitorId into g
                        select new
                        {
                            g.Key,
                            Count=g.Sum(c=>c.Scores.Count),
                            Company = g.FirstOrDefault().Competitor.Company.Name,
                            g.FirstOrDefault().Competitor.Name,
                            g.FirstOrDefault().Competitor.Pro,
                            g.FirstOrDefault().Competitor.StaffId,
                            Bishi = g.FirstOrDefault(t => t.Event.Name == "笔试"),
                            Baoxian = g.FirstOrDefault(t => t.Event.Name == "保险"),
                            Biaoxian = g.FirstOrDefault(t => t.Event.Name == "标线综合"),
                            Tujiao = g.FirstOrDefault(t => t.Event.Name == "涂胶"),
                            APU = g.FirstOrDefault(t => t.Event.Name == "APU拆装"),
                            Guanlu = g.FirstOrDefault(t => t.Event.Name == "管路施工"),
                            Daqi = g.FirstOrDefault(t => t.Event.Name == "大气数据"),
                            Shouce = g.FirstOrDefault(t => t.Event.Name == "手册"),
                            Yijiandeng = g.FirstOrDefault(t => t.Event.Name == "翼尖灯"),
                            Shuiping = g.FirstOrDefault(t => t.Event.Name == "水平安定面"),
                            Dianyuan = g.FirstOrDefault(t => t.Event.Name == "电源")

                        };
            var table = data2.AsEnumerable().Select((score, Index) => new PersonalRank
            {
                Company = score.Company,
                Name = score.Name,
                Pro = score.Pro,
                Staffid = score.StaffId,
                Count=score.Count,
                Bishi = score.Bishi != null ? score.Bishi.MarkAVG : null,
                Baoxian = score.Baoxian != null ? score.Baoxian.MarkAVG : null,
                Biaoxian = score.Biaoxian != null ? score.Biaoxian.MarkAVG : null,
                Tujiao = score.Tujiao != null ? score.Tujiao.MarkAVG : null,
                APU = score.APU != null ? score.APU.MarkAVG : null,
                Guanlu = score.Guanlu != null ? score.Guanlu.MarkAVG : null,
                Daqi = score.Daqi != null ? score.Daqi.MarkAVG : null,
                Shouce = score.Shouce != null ? score.Shouce.MarkAVG : null,
                Yijiandeng = score.Yijiandeng != null ? score.Yijiandeng.MarkAVG : null,
                Shuiping = score.Shuiping != null ? score.Shuiping.MarkAVG : null,
                Dianyuan = score.Dianyuan != null ? score.Dianyuan.MarkAVG : null,

            });
            int i = 1;
            var list = table.OrderByDescending(s => s.TotalScore).ToList();
            foreach (var item in list)
            {
                item.Rank = i++;
            }

            return list;
        }
    }
}