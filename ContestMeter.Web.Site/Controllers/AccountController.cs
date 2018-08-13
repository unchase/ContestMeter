using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ContestMeter.Web.Site.Database;
using ContestMeter.Web.Site.Database.Entities;
using ContestMeter.Web.Site.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;

namespace ContestMeter.Web.Site.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ContestMeterDbContext db2 = new ContestMeterDbContext();

        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ContestMeterDbContext())))
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    if (!user.IsDeleted)
                    {
                        await SignInAsync(user, model.RememberMe);

                        using (var db = new ContestMeterDbContext())
                        {
                            var userInfo = await db.UserInfos.FindAsync(user.UserInfoId);
                            if (userInfo != null)
                            {
                                userInfo.LastVisitDate = DateTime.Now;
                                db.Entry(userInfo).State = EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                        }

                        return RedirectToLocal(returnUrl);
                    }
                    ModelState.AddModelError("", "Регистрация пользователя не была подтверждена или он был удален. Администратор должен подтвердить регистрацию/удаление.");
                }
                else
                {
                    ModelState.AddModelError("", "Неправильное имя пользователя или пароль.");
                }
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult RegisterTeacher()
        {
            if (bool.TryParse(ConfigurationManager.AppSettings["UseRecaptcha"], out var useRecaptcha))
            {
                var model = new RegisterTeacherViewModel
                {
                    UseRecaptcha = useRecaptcha
                };
                if (ConfigurationManager.AppSettings["AllowRegisterTeacher"] == "true")
                    return View(model);
                else
                    TempData["Message"] = "Регистрация новых учителей в системе запрещена администратором.";
                    //ViewBag.ErrorMessage = "Регистрация новых учителей в системе запрещена администратором.";
            }
            else
                TempData["Message"] = "Не удалось считать параметр конфигурации приложения.";
                //ViewBag.ErrorMessage = "Не удалось считать параметр конфигурации приложения.";

            return RedirectToAction("Index", "Home");
            //return View("Error");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> RegisterTeacher(RegisterTeacherViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await UserManager.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("", "Пользователь с таким email уже существует.");
                }
                else
                {
                    if (model.UseRecaptcha)
                    {
                        var recaptchaHelper = this.GetRecaptchaVerificationHelper();
                        if (string.IsNullOrEmpty(recaptchaHelper.Response))
                        {
                            ModelState.AddModelError("", "Ответ капчи не может быть пустым.");
                            return View(model);
                        }
                        var recaptchaResult =
                            await recaptchaHelper.VerifyRecaptchaResponseTaskAsync();
                        if (recaptchaResult != RecaptchaVerificationResult.Success)
                        {
                            ModelState.AddModelError("", "Капча введена неверно.");
                            return View(model);
                        }
                    }

                    var userInfo = new UserInfo
                    {
                        Id = Guid.NewGuid(),
                        CreatedDate = DateTime.Now,
                        LastVisitDate = DateTime.Now,
                        School = model.School,
                        Grade = 0,
                        HomeTown = model.HomeTown
                    };
                    var user = new ApplicationUser
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        LastName = model.LastName,
                        FirstName = model.FirstName,
                        MiddleName = model.MiddleName,
                        Ip = GetLocalIP4().ToString(),
                        UserInfoId = userInfo.Id,
                        UserInfo = userInfo,
                        IsDeleted = true,
                        Rating = 0,
                        TeamId = null
                    };
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        UserManager.AddToRole(user.Id, "teacher");
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            if (bool.TryParse(ConfigurationManager.AppSettings["UseRecaptcha"], out var useRecaptcha))
            {
                var model = new RegisterViewModel
                {
                    UseRecaptcha = useRecaptcha
                };
                if (ConfigurationManager.AppSettings["AllowRegister"] == "true")
                    return View(model);
                else
                    TempData["Message"] = "Регистрация новых участников в системе запрещена администратором.";
                    //ViewBag.ErrorMessage = "Регистрация новых участников в системе запрещена администратором.";
            }
            else
                TempData["Message"] = "Не удалось считать параметр конфигурации приложения.";
                //ViewBag.ErrorMessage = "Не удалось считать параметр конфигурации приложения.";

            return RedirectToAction("Index", "Home");
            //return View("Error");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await UserManager.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("", "Пользователь с таким email уже существует.");
                }
                else
                {
                    if (model.UseRecaptcha)
                    {
                        var recaptchaHelper = this.GetRecaptchaVerificationHelper();
                        if (string.IsNullOrEmpty(recaptchaHelper.Response))
                        {
                            ModelState.AddModelError("", "Ответ капчи не может быть пустым.");
                            return View(model);
                        }
                        var recaptchaResult =
                            await recaptchaHelper.VerifyRecaptchaResponseTaskAsync();
                        if (recaptchaResult != RecaptchaVerificationResult.Success)
                        {
                            ModelState.AddModelError("", "Капча введена неверно.");
                            return View(model);
                        }
                    }

                    var userInfo = new UserInfo
                    {
                        Id = Guid.NewGuid(),
                        CreatedDate = DateTime.Now,
                        LastVisitDate = DateTime.Now,
                        School = model.School,
                        Grade = model.Grade,
                        HomeTown = model.HomeTown
                    };

                    using (var db = new ContestMeterDbContext())
                    {
                        var defaultTeam = db.Teams.Include(t => t.Participants).FirstOrDefault(t => t.Name == "Одиночки");
                        if (defaultTeam == null)
                        {
                            return HttpNotFound();
                        }
                        if (defaultTeam.Participants.Count == defaultTeam.MaxTeamNumber)
                        {
                            ModelState.AddModelError("", "Достигнуто максимальное количество участников команды 'Одиночки'. Пожалуйста, обратитесь к администратору, чтобы он зарезервировал Вам место в этой команде.");
                            return View(model);
                        }
                    }

                    var user = new ApplicationUser
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        LastName = model.LastName,
                        FirstName = model.FirstName,
                        MiddleName = model.MiddleName,
                        Ip = GetLocalIP4().ToString(),
                        UserInfoId = userInfo.Id,
                        UserInfo = userInfo,
                        Rating = 0,
                        TeamId = null
                    };

                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        // добавляем для только что созданного пользователя роль "participant" 
                        UserManager.AddToRole(user.Id, "participant");

                        //функция для добавления только что созданного участника в команду по-умолчанию "Одиночки"
                        if (!AddParticipantToDefaultTeam(user.Id))
                            return HttpNotFound();

                        await SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        AddErrors(result);
                    }  
                }
            }
            return View(model);
        }

        public bool AddParticipantToDefaultTeam(string participantId)
        {
            using (var db = new ContestMeterDbContext())
            {
                var user = db.Users.Find(participantId);
                if (user == null)
                {
                    return false;
                }
                var defaultTeam = db.Teams.FirstOrDefault(t => t.Name == "Одиночки");
                if (defaultTeam == null)
                {
                    return false;
                }

                var teamParticipant = new TeamParticipant
                {
                    Id = Guid.NewGuid(),
                    TeamId = defaultTeam.Id,
                    ParticipantId = user.Id
                };
                db.Entry(teamParticipant).State = EntityState.Added;
                user.TeamId = defaultTeam.Id;
                defaultTeam.Participants.Add(user);
                db.Entry(user).State = EntityState.Modified;
                db.Entry(defaultTeam).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            ManageMessageId? message = result.Succeeded ? ManageMessageId.RemoveLoginSuccess : ManageMessageId.Error;
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Ваш пароль был изменен."
                : message == ManageMessageId.SetPasswordSuccess ? "Ваш пароль был установлен."
                : message == ManageMessageId.RemoveLoginSuccess ? "Внешний логин был удален."
                : message == ManageMessageId.Error ? "Произошла ошибка."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Manage(ManageUserViewModel model)
        {
            var hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                var state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
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
        public async System.Threading.Tasks.Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async System.Threading.Tasks.Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
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
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        public async System.Threading.Tasks.Task<ActionResult> RemoveAccount()
        {
            if (User.IsInRole("administrator"))
            {
                TempData["Message"] = "Администратор не может удалить свою учетную запись";
                return RedirectToAction("Manage");
                //ViewBag.ErrorMessage = "Администратор не может удалить свою учетную запись";
                //return View("Error");
            }
            var currentUserId = User.Identity.GetUserId();
            using (var db = new ContestMeterDbContext())
            {
                var user = db.Users.Find(currentUserId);
                if (user != null)
                {
                    user.IsDeleted = true;
                    db.Entry(user).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                else
                    return RedirectToAction("Manage");
            }

            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                db2.Dispose();
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        private async System.Threading.Tasks.Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
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
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        // ToDo: подключить ContestMeter и взять этот метод оттуда (удалить этот)
        public static IPAddress GetLocalIP4()
        {
            var host = Dns.GetHostEntry(string.Empty);
            return host.AddressList.Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                                               && x != IPAddress.Any && x != IPAddress.Broadcast &&
                                               x != IPAddress.Loopback && !IPAddress.IsLoopback(x)).FirstOrDefault();
        }
        #endregion
    }
}