using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using ScoringSystem.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ScoringSystem.Controllers {
    public class AccountController : Controller {

    ApplicationSignInManager _signInManager;
        public ApplicationSignInManager SignInManager
        {
            get
            {
                if (_signInManager == null)
                {
                    _signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
                }
                return _signInManager;
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
                if(_roleManager==null)
                {
                    _roleManager= HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();
                }
                return _roleManager;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login() {
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl) {
            if(ModelState.IsValid) {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe.Value, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        return RedirectToLocal(returnUrl);
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                    default:
                        ViewBag.ErrorMessage = "The user name or password provided is incorrect";
                        return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff() {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return Redirect("/");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register() {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterModel model) {
            if(ModelState.IsValid) {
                // Attempt to register the user
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return Redirect("/");
                }
                ViewBag.ErrorMessage = result.Errors.First();
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        public ActionResult ChangePassword() {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordModel model) {
            if(ModelState.IsValid) {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("ChangePasswordSuccess");
                }
                ViewBag.ErrorMessage = result.Errors.First();
                return View(model);
                
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess() {
            return View();
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("Code", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    ModelState.AddModelError("Email", "The user either does not exist or is not confirmed");
                    return View(model);
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);        
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "No user found");
                return View(model);
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("Login");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return Redirect("/");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

        public ActionResult Index()
        {
            ViewBag.PageName = "Setting";
           // ViewBag.Roles= RoleManager.Roles;
            return View();

        }

        

        [ValidateInput(false)]
        public ActionResult AccountGridViewPartial()
        {
            
            //转换users的模型
            List<AccountEditModel> accountEditModels = new List<AccountEditModel>();
            foreach(var item in UserManager.Users)
            {
                //var accountEditModel = new AccountEditModel
                //{
                //    Id = item.Id,
                //    UserName = item.UserName,
                //    RealName = item.RealName,
                //    StaffId = item.StaffId
                //};
                //List<string> list = new List<string>();
                //foreach(var role in item.Roles)  //把rolesID转换到tokenbox中
                //{
                //    accountEditModel.RoleIds.Add(role.RoleId);
                //    list.Add(RoleManager.FindById(role.RoleId).Name);
                //}

                ////把roleId转换为roleName字符串
                //accountEditModel.Roles = string.Join(";", list.ToArray());
                accountEditModels.Add(new AccountEditModel(item, RoleManager.Roles));
            }
            ViewBag.Roles = RoleManager.Roles.ToList();
            
         
            return PartialView("_AccountGridViewPartial", accountEditModels.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> AccountGridViewPartialAddNewAsync(ScoringSystem.Models.AccountEditModel item)
        {

            //转换users的模型
            List<AccountEditModel> model = new List<AccountEditModel>();
            foreach (var applicationUser in UserManager.Users)
            {
                model.Add(new AccountEditModel(applicationUser, RoleManager.Roles));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = new ApplicationUser { UserName = item.UserName, StaffId = item.StaffId, RealName = item.RealName };
                    var result = await UserManager.CreateAsync(user,string.IsNullOrEmpty(item.PassWord)?item.PassWord:"123456"); 
                    if (result.Succeeded)
                    {
                        foreach(var roleId in item.RoleIds)
                        {
                            UserManager.AddToRole(user.Id, RoleManager.FindById(roleId).Name);
                        }
                        
                        return PartialView("_AccountGridViewPartial", model.ToList());
                    }
                    else
                    {
                        ViewData["EditError"] = Infrastructure.MyHandler.IdentityResultErrorsToString(result);
                    }

                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_AccountGridViewPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> AccountGridViewPartialUpdateAsync(ScoringSystem.Models.AccountEditModel item)
        {
            List<AccountEditModel> model = new List<AccountEditModel>();
            foreach (var applicationUser in UserManager.Users)
            {
                model.Add(new AccountEditModel(applicationUser, RoleManager.Roles));
            }
            ApplicationUser user = await UserManager.FindByIdAsync(item.Id);
            if (ModelState.IsValid)
            {
                try
                {
                    if (user != null)
                    {
                        if(!string.IsNullOrEmpty( item.PassWord))
                        {
                            //验证密码是否满足要求
                            IdentityResult validPass = await UserManager.PasswordValidator.ValidateAsync(item.PassWord);
                            if (validPass.Succeeded)
                            {
                                user.PasswordHash = UserManager.PasswordHasher.HashPassword(item.PassWord);
                            }
                            else
                            {
                                ViewData["EditError"] = validPass.Errors;
                            }
                        }


                        user.StaffId = item.StaffId;
                        user.RealName = item.RealName;
                        user.UserName = item.UserName;
                        IdentityResult result = await UserManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            IEnumerable<string> orginroles = user.Roles.Select(r=>r.RoleId);
                            IEnumerable<string> unchangedroles = item.RoleIds.Where(x => orginroles.Any(y => y == x));
                            IEnumerable<string> newrole = item.RoleIds.Except(unchangedroles);
                            IEnumerable<string> deleterole = orginroles.Except(unchangedroles);

                            foreach (string roleId in newrole)
                            {
                                result = await UserManager.AddToRoleAsync(user.Id, RoleManager.FindById(roleId).Name);
                                if (!result.Succeeded)
                                {
                                    ViewData["EditError"] = Infrastructure.MyHandler.IdentityResultErrorsToString(result);
                                }

                            }

                            foreach (string roleId in deleterole)
                            {
                                result = await UserManager.RemoveFromRoleAsync(user.Id, RoleManager.FindById(roleId).Name);
                                if (!result.Succeeded)
                                {
                                    ViewData["EditError"] = Infrastructure.MyHandler.IdentityResultErrorsToString(result);
                                }

                            }

                            return PartialView("_AccountGridViewPartial", model.ToList());
                        }
                        else
                        {
                            ViewData["EditError"] = Infrastructure.MyHandler.IdentityResultErrorsToString(result);
                        }
                        //if (validPass == null || validPass.Succeeded)
                        //{
                        //    IdentityResult result = await UserManager.UpdateAsync(user);
                        //    if (result.Succeeded)
                        //    {
                        //        return RedirectToAction("Index");
                        //    }
                        //    AddErrors(result);
                        //}
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_AccountGridViewPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> AccountGridViewPartialDeleteAsync(System.String Id)
        {
            List<AccountEditModel> model = new List<AccountEditModel>();
            foreach (var applicationUser in UserManager.Users)
            {
                model.Add(new AccountEditModel(applicationUser, RoleManager.Roles));
            }
            ApplicationUser user = await UserManager.FindByIdAsync(Id);
            if (user != null)
            {
                if (user.UserName == "admin")
                {
                    ViewData["EditError"] = "请勿删除管理员！";
                }
                try
                {
                    IdentityResult result = await UserManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        return PartialView("_AccountGridViewPartial", model.ToList());
                    }
                    else
                    {
                        ViewData["EditError"] = Infrastructure.MyHandler.IdentityResultErrorsToString(result);
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_AccountGridViewPartial", model.ToList());
        }

        public ActionResult AccountEditFormPartial(String Id)
        {
            ViewBag.Roles = RoleManager.Roles.ToList();
            if (Id != null)
            {
                ApplicationUser user = UserManager.FindById(Id);
                DevExpress.Web.TokenCollection tokencol = new DevExpress.Web.TokenCollection();

                foreach(Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole role in  user.Roles)
                {
                    tokencol.Add(role.RoleId);
                }

                AccountEditModel model = new AccountEditModel(user, RoleManager.Roles);

                return PartialView("_AccountEditFormPartial", model ?? new AccountEditModel());
            }
            return PartialView("_AccountEditFormPartial", new AccountEditModel());

        }

        public ActionResult Multi(int? id)
        { return View(); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Multi()
        {
            if (ModelState.IsValid)
            {
                string content = Request["List"];
                ViewBag.Content = content;

                List<string> t = content.Split('\r', '\n').ToList();
                ViewBag.Count = t.Count;
                ViewBag.FaultCount = 0;
                ViewBag.SuccessCount=0;

                foreach (string item in t)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                       
                        try
                        {
                            string name = item.Split('\t')[0];
                            string staffid = item.Split('\t')[1];
                            string role = item.Split('\t')[2];
                            string password = item.Split('\t')[3];
                            var user = new ApplicationUser { UserName = staffid, StaffId = staffid, RealName = name };

                            var result = await UserManager.CreateAsync(user, string.IsNullOrEmpty(password) ? password : "123456");
                            if (result.Succeeded)
                            {
                               
                                    UserManager.AddToRole(user.Id, RoleManager.FindByName(role).Name);
                                ViewBag.SuccessCount++;


                            }
                            else
                            {
                                ViewBag.FaultCount++;
                            }

                        }
                        catch (Exception e)
                        {
                            ViewData["EditError"] = e.Message;
                            ViewBag.FaultCount++;
                        }

                    }
                }
             
                return View();
            }


            return View();
        }
    }
}