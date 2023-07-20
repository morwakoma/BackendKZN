using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using eRecruitment.Sita.BackEnd.Models;
using System.Web.Security;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity.EntityFramework;
using eRecruitment.Sita.BackEnd.App_Data.Entities.DAL;

using System.Data.Entity;
using eRecruitmentLogger;

namespace eRecruitment.Sita.BackEnd.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext();
        eRecruitment.BusinessDomain.DAL.DataAccess _dal = new BusinessDomain.DAL.DataAccess();


        string IDNumber = "";
        string FirstName = "";
        string SecondName = "";
        string ThirdName = "";
        string Surname = "";
        string EmailAddress = "";
        string PhoneNumber = "";
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        //[AllowAnonymous]
        //public ActionResult Login(string returnUrl)
        //{
        //    ViewBag.ReturnUrl = returnUrl;
        //    return View();
        //}

        [AllowAnonymous]
        public async Task<ActionResult> Login(string returnUrl, string id)
        {

            string WebURL = Request.Url.ToString();

            if (id != null)
            {

                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["WebAPIURL"] + id);
                myRequest.Timeout = 5000;
                HttpWebResponse response = (HttpWebResponse)myRequest.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream dataStream = response.GetResponseStream();
                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.
                    string responseFromServer = reader.ReadToEnd();

                    dynamic data = JsonConvert.DeserializeObject(responseFromServer);

                    FirstName = Convert.ToString(data["name"]);
                    SecondName = Convert.ToString(data["name2"]);
                    ThirdName = Convert.ToString(data["name3"]);
                    Surname = Convert.ToString(data["surname"]);
                    EmailAddress = Convert.ToString(data["email"]);
                    IDNumber = Convert.ToString(data["idNumber"]);
                    PhoneNumber = Convert.ToString(data["phoneNumber"]);

                    // Display the content.
                    ViewBag.TestMessage = FirstName;
                    response.Close();
                    //return true;
                }
                else
                {
                    response.Close();
                    //return false;
                }
            }
            Console.Write("Data Returned");
            //FirstName = "Ntshengedzeni";
            //Surname = "Badamarema";
            //EmailAddress = "Ntshengedzeni.Badamarema@sita.co.za";
            //IDNumber = "7907265091081";
            //PhoneNumber = "0725365413";


            if (EmailAddress != null && EmailAddress != "")
            {

                //var data = (from a in _db.AspNetUsers
                //            join b in _db.tblProfiles on a.Id equals b.UserID
                //            where a.Email == EmailAddress || b.IDNumber == IDNumber
                //            select new { a.Id }).Count();

                var data = _db.tblProfiles.Where(x => x.IDNumber == IDNumber).Count();
                //bool v = 
                //var isActiveUser = _db.AspNetUsers.Where(x => x.Email == EmailAddress && x.IsActive == 1).Count();
                //if (isActiveUser == 0)
                //{
                //    return View("Error403");
                //}
                if (data == 0)
                {
                    if (ModelState.IsValid)
                    {
                        var user = new ApplicationUser { UserName = EmailAddress, Email = EmailAddress };
                        var result = await UserManager.CreateAsync(user, "P@$$w0rd1");
                        if (result.Succeeded)
                        {
                            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                            //Insert User Profile                   
                            if (_dal.CheckIfUserExists(IDNumber) == true)
                            {
                                _db.proc_eRecruitmentUpdateUserProfile(user.Id, IDNumber, Surname, FirstName, PhoneNumber, EmailAddress);
                            }
                            else
                            {
                                _db.proc_eRecruitmentCreateUserProfile(user.Id, IDNumber, string.Empty, Surname, FirstName, PhoneNumber, EmailAddress);
                            }
                            //test this one
                            //if (isActiveUser == 0)
                            //{
                            //    //return View("Error403");
                            //    return RedirectToAction("Error403", "Admin");
                            //}
                            //else
                            //{
                            //    return RedirectToAction("Index", "Home");
                            //}
                            int isActiveUser = (int)_db.AspNetUsers.Where(x => x.Email == EmailAddress && x.IsActive == 1).Count();

                            if (isActiveUser == 0)
                            {
                                return RedirectToAction("Error403", "Admin");
                            }
                            else
                            {
                                return RedirectToAction("Index", "Home");
                            }

                        }
                        AddErrors(result);
                    }
                }
                else
                {
                    _db.sp_UpdateUserFromPortal(IDNumber, PhoneNumber, EmailAddress, FirstName, Surname, string.Empty);


                    var result = await SignInManager.PasswordSignInAsync(EmailAddress, "P@$$w0rd1", false, shouldLockout: false);
                    switch (result)
                    {
                        case SignInStatus.Success:
                            string userId = SignInManager.AuthenticationManager.AuthenticationResponseGrant.Identity.GetUserId();
                            var rolename = UserManager.GetRoles(userId);
                            int isActiveUser = (int)_db.AspNetUsers.Where(x => x.Email == EmailAddress && x.IsActive == 1).Count();

                            //if (isActiveUser == 0)
                            //{
                            //    return View("Error403");
                            //}
                            if (isActiveUser == 0 || rolename.Count <= 0)
                            {
                                return RedirectToAction("Error403", "Admin");
                            }
                            else
                            if (rolename.Count != 0)
                            {
                                if (rolename[0].ToString() == "Admin")
                                {
                                    return RedirectToAction("DivisionList", "Admin");
                                }
                                else if (rolename[0].ToString() == "SysAdmin")
                                {
                                    return RedirectToAction("Index", "Admin");
                                }
                                else if (rolename[0].ToString() == "Recruiter")
                                {
                                    return RedirectToAction("Dashboard", "Vacancy");
                                }
                                else if (rolename[0].ToString() == "Approver")
                                {
                                    return RedirectToAction("Dashboard", "Vacancy");
                                }
                                else if (rolename[0].ToString() == "Recruiter Admin")
                                {
                                    return RedirectToAction("Dashboard", "Vacancy");
                                }
                            }

                            return this.RedirectToAction("Index", "Home");
                        case SignInStatus.LockedOut:
                            return View("Lockout");
                        //case SignInStatus.RequiresVerification:
                        //    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                        case SignInStatus.Failure:
                        default:
                            ModelState.AddModelError("", "Invalid login attempt.");
                            //return View(model);
                            return View();
                    }
                }
            }
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [ChildActionOnly] // action cannot be requested directly via URL
        public ActionResult GetUserID()
        {
            string userid = User.Identity.GetUserId();
            return Content(userid);
        }

        [HttpGet]
        public ActionResult Error403()
        {
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            int isActiveUser = (int)_db.AspNetUsers.Where(x => x.Email == model.Email && x.IsActive == 1).Count();
            //TempData["isActiveUser"] = isActiveUser;
            //TempData.Keep();
            switch (result)
            {
                case SignInStatus.Success:
                    string userId = SignInManager.AuthenticationManager.AuthenticationResponseGrant.Identity.GetUserId();
                    var rolename = UserManager.GetRoles(userId);
                    //TempData["RoleName"] = rolename[0].ToString();
                    //TempData.Keep();
                    //LogHelper.WriteLine("UserId :" + userId + " Email : " + model.Email);
                    //LogHelper.WriteLine("Successfully Logged in :");
                    //LogHelper.WriteLine("IsActive : " + isActiveUser);
                    //LogHelper.WriteLine("Role : " + rolename.Count);
                    //LogHelper.WriteLine("Checking if user is active :");
                    //if (rolename.Count != 0)
                    //{
                    //    LogHelper.WriteLine("Role Name : " + rolename[0].ToString());
                    //}

                    if (isActiveUser == 0 || rolename.Count <= 0)
                    {
                        //LogHelper.WriteLine("user not active and redirecting to Error 403 :");
                        return RedirectToAction("Error403", "Admin");
                    }
                    else
                    {
                        //LogHelper.WriteLine("user is active and Role has value :");
                        if (rolename.Count != 0)
                        {

                            if (rolename[0].ToString() == "Admin" && isActiveUser != 0)
                            {
                                //LogHelper.WriteLine("user is active and Role has value and role is Admin:");
                                return RedirectToAction("DivisionList", "Admin");

                            }
                            else if (rolename[0].ToString() == "SysAdmin" && isActiveUser != 0)
                            {
                                //LogHelper.WriteLine("user is active and Role has value and role is SysAdmin:");
                                return RedirectToAction("Index", "Admin");

                            }
                            else if (rolename[0].ToString() == "Recruiter" && isActiveUser != 0)
                            {
                                //LogHelper.WriteLine("user is active and Role has value and role is Recruiter:");
                                return RedirectToAction("Dashboard", "Vacancy");
                            }
                            else if (rolename[0].ToString() == "Approver" && isActiveUser != 0)
                            {
                                //LogHelper.WriteLine("user is active and Role has value and role is Approver:");
                                return RedirectToAction("Dashboard", "Vacancy");
                            }
                            else if (rolename[0].ToString() == "Recruiter Admin" && isActiveUser != 0)
                            {
                                //LogHelper.WriteLine("user is active and Role has value and role is Recruiter Admin:");
                                return RedirectToAction("Dashboard", "Vacancy");
                            }
                        }
                    }
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
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
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
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
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
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
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
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
                    return View("ExternalLoginFailure");
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
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult GoToPortal()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return Json(new { url = System.Configuration.ConfigurationManager.AppSettings["LogOutURL"].ToString() });
        }
        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
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
            return RedirectToAction("Index", "Home");
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
    }
}