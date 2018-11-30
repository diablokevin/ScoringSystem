using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScoringSystem.Models
{
    public class PersonalRank
    {
        public int Rank { get; set; }
        public string Name { get; set; }
        public string Staffid { get; set; }
        public string Pro { get; set; }
        public string Company { get; set; }
        public double TotalScore { get; set; }
    }

    public class EventRank
    {
        public int Rank { get; set; }
        public string EventName { get; set; }
        public string Competitor_Name { get; set; }
        public string Competitor_Staffid { get; set; }
        public string Competitor_Pro { get; set; }
        public string Competitor_Company { get; set; }
        public double Score { get; set; }
    }

    public class CompanyRank
    {
        public int Rank { get; set; }
        public string CompanyName { get; set; }
      
        public double Score { get; set; }
    }

}