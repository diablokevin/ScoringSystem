using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

using ScoringSystem.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace ScoringSystem.Controllers
{
    public class RoleController : Controller
    {
        // GET: Role

        public ActionResult Index()
        {
            ViewBag.PageName = "Setting";
            return View(RoleManager.Roles);
        }

        [ValidateInput(false)]
        public ActionResult RoleGridViewPartial()
        {
            var model = RoleManager.Roles;
            
            return PartialView("_RoleGridViewPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> RoleGridViewPartialAddNewAsync(ScoringSystem.Models.ApplicationRole item)
        {
            var model = RoleManager.Roles;
            if (ModelState.IsValid)
            {
                try
                {
                    IdentityResult result = await RoleManager.CreateAsync(new ApplicationRole(item.Name));
                    if (result.Succeeded)
                    {
                        return PartialView("_RoleGridViewPartial", model.ToList());
                    }
                    else
                    {
                        ViewData["EditError"] = result.Errors;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_RoleGridViewPartial", model.ToList());
        }
            

        public async Task<ActionResult> RoleGridViewPartialUpdateAsync(ApplicationRole item)
        {
            var model = RoleManager.Roles;
            ApplicationRole role = await RoleManager.FindByIdAsync(item.Id);
            if (ModelState.IsValid)
            {
                try
                {
                   
                    role.Name = item.Name;
                    IdentityResult result = await RoleManager.UpdateAsync(role);
                    if (result.Succeeded)
                    {
                        return PartialView("_RoleGridViewPartial", model.ToList());
                    }
                    else
                    {
                        ViewData["EditError"] = result.Errors;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = "Please, correct all errors.";
            }
               
             return PartialView("_RoleGridViewPartial", model.ToList());
        
            
            //string[] memberIDs = role.Users.Select(x => x.UserId).ToArray();
            //IEnumerable<ApplicationUser> members = UserManager.Users.Where(x => memberIDs.Any(y => y == x.Id));
            //IEnumerable<ApplicationUser> nonMembers = UserManager.Users.Except(members);
            //return View(new RoleEditModel()
            //{
            //    Role = role,
            //    Members = members,
            //    NonMembers = nonMembers
            //});
        }

        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> RoleGridViewPartialDeleteAsync(string Id)
        {
            var model = RoleManager.Roles;
            ApplicationRole role = await RoleManager.FindByIdAsync(Id);

            if (role != null)
            {
                IdentityResult result = await RoleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return PartialView("_RoleGridViewPartial", model.ToList());
                }
                else
                {
                    ViewData["EditError"] = result.Errors;
                }
            }
            else
            {
                ViewData["EditError"] = "无法找到该Role" ;
            }
            return PartialView("_RoleGridViewPartial", model.ToList());
        }


        public async Task<ActionResult> RoleEditFormPartialAsync(String Id)
        {
            if(Id!=null)
            {
                ApplicationRole role = await RoleManager.FindByIdAsync(Id);
                return PartialView("_RoldEditFormPartial", role ?? new ApplicationRole());
            }
            return PartialView("_RoldEditFormPartial", new ApplicationRole());

        }



        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create(string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await RoleManager.CreateAsync(new ApplicationRole(name));
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            return View(name);
        }

        public async Task<ActionResult> Edit(string id)
        {
            ApplicationRole role = await RoleManager.FindByIdAsync(id);
            string[] memberIDs = role.Users.Select(x => x.UserId).ToArray();
            IEnumerable<ApplicationUser> members = UserManager.Users.Where(x => memberIDs.Any(y => y == x.Id));
            IEnumerable<ApplicationUser> nonMembers = UserManager.Users.Except(members);
            return View(new RoleEditModel()
            {
                Role = role,
                Members = members,
                NonMembers = nonMembers
            });
        }

        [HttpPost]

        public async Task<ActionResult> Edit(RoleModificationModel model)
        {
            IdentityResult result;
            if (ModelState.IsValid)
            {
                foreach (string userID in model.IDsToAdd ?? new string[] { })
                {
                    result = await UserManager.AddToRoleAsync(userID, model.RoleName);
                    if (!result.Succeeded)
                    {
                        return View("Error", result.Errors);
                    }

                }

                foreach (string userID in model.IDsToDelete ?? new string[] { })
                {
                    result = await UserManager.RemoveFromRoleAsync(userID, model.RoleName);
                    if (!result.Succeeded)
                    {
                        return View("Error", result.Errors);
                    }

                }
                return RedirectToAction("Index");
            }
            return View("Error", new string[] { "无法找到此角色" });
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            ApplicationRole role = await RoleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await RoleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("Error", result.Errors);
                }
            }
            else
            {
                return View("Error", new string[] { "无法找到该Role" });
            }
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                if (_userManager == null)
                {
                    _userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                }
                return _userManager;
            }
        }

        ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                if (_roleManager == null)
                {
                    _roleManager = HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();
                }
                return _roleManager;
            }
        }
    }
 
}