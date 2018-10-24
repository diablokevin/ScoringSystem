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

    public class RoleModificationModel
    {
        public string RoleName { get; set; }
        public string[] IDsToAdd { get; set; }
        public string[] IDsToDelete { get; set; }
    }
}