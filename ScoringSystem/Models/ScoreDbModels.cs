using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ScoringSystem.Models
{
    public class Event
    {
        public int Id { get; set; }
        [Display(Name ="名称")]
        public string Name { get; set; }
        [Display(Name = "专业")]
        public string Pro { get; set; }
        [Display(Name = "菜单排序")]
        public int MenuOrder { get; set; }
        [Display(Name = "限时")]
        public TimeSpan TimeLimit { get; set; }
        [Display(Name = "主板ID")]
        public int? ChipId { get; set; }

        public virtual ICollection<Schedule> Schedules { get; set; }

    }

    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Competitor> Competitors { get; set; }
    }

    public class Competitor
    {
        public int Id { get; set; }
        [Display(Name = "姓名")]
        public string Name { get; set; }
        [Display(Name = "专业")]
        public string Pro { get; set; }
        [Display(Name = "员工号")]
        public string StaffId { get; set; }
        [Display(Name = "比赛编号")]
        public string Race_num { get; set; }
        [Display(Name = "公司")]
        public int CompanyId { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
    }

    public class Schedule
    {
        public int Id { get; set; }

        public DateTime PlanBeginTime { get; set; }
        public DateTime PlanEndTime { get; set; }

        public TimeSpan? TimeConsume { get; set; }
        public double? Score { get; set; }

        public Nullable<System.DateTime> JudgeTime { get; set; }
        public Nullable<System.DateTime> ModifyTime { get; set; }


        public int EventId { get; set; }
        public int? CompetitorId { get; set; }
        public int? JudgeId { get; set; }

        public virtual string Subject { get { return Event.Name; } }

        public virtual Competitor Competitor { get; set; }
        public virtual Event Event { get; set; }
        public virtual Judge Judge { get; set; }
    }


    public class Judge
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // public string UserId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public Nullable<int> EventId { get; set; }
        public string StaffId { get; set; }

        public virtual Company Company { get; set; }
        public virtual Event Event { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
    }


}