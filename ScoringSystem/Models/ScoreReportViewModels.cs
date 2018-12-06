using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ScoringSystem.Models
{
    public class PersonalRank
    {
        [Display(Name="排名")]
        public int Rank { get; set; }
        [Display(Name = "姓名")]
        public string Name { get; set; }
        [Display(Name = "员工号")]
        public string Staffid { get; set; }
        [Display(Name = "专业")]
        public string Pro { get; set; }
        [Display(Name = "公司")]
        public string Company { get; set; }
        [Display(Name = "完成数量")]
        public int Count { get; set; }
        [Display(Name = "笔试")]
        public double? Bishi { get; set; }
        [Display(Name = "保险")]
        public double? Baoxian { get; set; }
        [Display(Name = "标线综合")]
        public double? Biaoxian { get; set; }
        [Display(Name = "涂胶")]
        public double? Tujiao { get; set; }
        [Display(Name = "APU拆装")]
        public double? APU { get; set; }
        [Display(Name = "管路施工")]
        public double? Guanlu { get; set; }
        [Display(Name = "大气数据")]
        public double? Daqi { get; set; }
        [Display(Name = "手册")]
        public double? Shouce { get; set; }
        [Display(Name = "翼尖灯")]
        public double? Yijiandeng { get; set; }
        [Display(Name = "水平安定面")]
        public double? Shuiping { get; set; }
        [Display(Name = "电源")]
        public double? Dianyuan { get; set; }
        [Display(Name = "总分")]
        public double? TotalScore {
            get
            {
                if(Pro=="AV")
                {
                    double result = ((Bishi ?? 0) * 0.3 + (((Baoxian ?? 0) + (Biaoxian ?? 0) + (Tujiao ?? 0) + (Daqi ?? 0) + ((Shouce ?? 0) + (Yijiandeng ?? 0) + (Dianyuan ?? 0)) * 2 / 3) / 6 * 0.7));
                    return Math.Round(result,3);
                }
                else if(Pro=="ME")
                {
                    double result = ((Bishi ?? 0) * 0.3 + ((Baoxian ?? 0) + (Biaoxian ?? 0) + (Tujiao ?? 0) + (Guanlu ?? 0) + (APU ?? 0) + ((Shouce ?? 0) + (Shuiping ?? 0) + (Dianyuan ?? 0)) * 2 / 3) / 7 * 0.7);

                    return Math.Round(result, 3);
                }
                else
                {
                    return null;
                }
               
            }
        }
    }

    public class EventRank
    {
        public int Rank { get; set; }
        public string EventName { get; set; }
        public string Competitor_Name { get; set; }
        public string Competitor_Staffid { get; set; }
        public string Competitor_Pro { get; set; }
        public string Competitor_Company { get; set; }
        public double? Score { get; set; }
    }

    public class CompanyRank
    {
        [Display(Name = "排名")]
        public int Rank { get; set; }
        [Display(Name = "公司")]
        public string CompanyName { get; set; }
        [Display(Name = "完成数量")]
        public int Count { get; set; }
        [Display(Name = "总分")]        
        public double? Score { get ; set; }
    }

}