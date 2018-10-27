using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace ScoringSystem.Models
{
    public class RoleEditModel
    {
        public ApplicationRole Role { get; set; }
      
        public IEnumerable<ApplicationUser> Members { get; set; }
        public IEnumerable<ApplicationUser> NonMembers { get; set; }
    }


}