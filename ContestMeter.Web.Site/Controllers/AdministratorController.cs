using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ContestMeter.Web.Site.Database;
using ContestMeter.Web.Site.Database.Entities;
using ContestMeter.Web.Site.Database.Migrations;
using ContestMeter.Web.Site.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ContestMeter.Web.Site.Controllers
{
    [Authorize(Roles = "administrator")]
    public class AdministratorController : Controller
    {
        private ContestMeterDbContext db = new ContestMeterDbContext();

        public ActionResult Main()
        {
            var model = new AdministratorMainViewModel();
            model.PostedSolutionsRootFolder = ConfigurationManager.AppSettings["PostedSolutionsRootFolder"];
            model.SiteConfigFolder = ConfigurationManager.AppSettings["SiteConfigFolder"];
            model.RecaptchaPrivateKey = ConfigurationManager.AppSettings["recaptchaPrivateKey"];
            model.RecaptchaPublicKey = ConfigurationManager.AppSettings["recaptchaPublicKey"];
            bool allowRegister;
            if (bool.TryParse(ConfigurationManager.AppSettings["AllowRegister"], out allowRegister))
            {
                model.AllowRegister = allowRegister;
                bool allowRegisterTeacher;
                if (bool.TryParse(ConfigurationManager.AppSettings["AllowRegisterTeacher"], out allowRegisterTeacher))
                {
                    model.AllowRegisterTeacher = allowRegisterTeacher;
                    bool useRecaptcha;
                    if (bool.TryParse(ConfigurationManager.AppSettings["UseRecaptcha"], out useRecaptcha))
                    {
                        model.UseRecaptcha = useRecaptcha;
                        return View(model);
                    }
                }
                ModelState.AddModelError("", "Не удалось считать параметр приложения 'AllowRegisterTeacher'");
                model.AllowRegister = true;
                return View(model);
            }
            ModelState.AddModelError("", "Не удалось считать параметр приложения 'AllowRegister'");
            model.AllowRegister = true;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Main(AdministratorMainViewModel model)
        {
            if (ModelState.IsValid)
            {
                ConfigurationManager.AppSettings.Set("PostedSolutionsRootFolder", model.PostedSolutionsRootFolder.ToLower());
                ConfigurationManager.AppSettings.Set("SiteConfigFolder", model.SiteConfigFolder.ToLower());
                ConfigurationManager.AppSettings.Set("AllowRegister", model.AllowRegister.ToString().ToLower());
                ConfigurationManager.AppSettings.Set("AllowRegisterTeacher", model.AllowRegisterTeacher.ToString().ToLower());
                ConfigurationManager.AppSettings.Set("useRecaptcha", model.UseRecaptcha.ToString().ToLower());
                ConfigurationManager.AppSettings.Set("recaptchaPrivateKey", model.RecaptchaPrivateKey);
                ConfigurationManager.AppSettings.Set("recaptchaPublicKey", model.RecaptchaPublicKey);
                TempData["Message"] = "Конфигурация была сохранена";
                return RedirectToAction("Main");
            }
            ModelState.AddModelError("", "Проверьте правильность введенных данных");
            return RedirectToAction("Main");
        }

        public ActionResult DeletedAccountsList()
        {
            var deletedUsers = db.Users.Include("Roles").Where(u => u.IsDeleted);
            var notDeletedUsers = db.Users.Include("Roles").Where(u => !u.IsDeleted);
            var administratorsRoleId = db.Roles.First(r => r.Name == "administrator").Id;
            var teacherRoleId = db.Roles.First(r => r.Name == "teacher").Id;
            var participantId = db.Roles.First(r => r.Name == "participant").Id;
            var model = new AdministratorAccountsViewModel
            {
                DeletedUsers = deletedUsers,
                NotDeletedUsers = notDeletedUsers,
                AdministratorsRoleId = administratorsRoleId,
                ParticipantId = participantId,
                TeacherRoleId = teacherRoleId
            };
            return View(model);    
        }

        public async System.Threading.Tasks.Task<ActionResult> DeleteAccount(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var administratorsRoleId = db.Roles.First(r => r.Name == "administrator").Id;
            var participantsRoleId = db.Roles.First(r => r.Name == "participant").Id;
            var TeachersRoleId = db.Roles.First(r => r.Name == "teacher").Id;
            var userRoleId = user.Roles.First().RoleId;
            if (administratorsRoleId == userRoleId)
            {
                TempData["Message"] = "Администратор не может удалить свою учетную запись";
                return RedirectToAction("DeletedAccountsList");
                //ViewBag.ErrorMessage = "Администратор не может удалить свою учетную запись";
                //return View("Error");
            }
            else if (participantsRoleId == userRoleId)
            {
                var userTeam = db.Teams.FirstOrDefault(t => t.Id == user.TeamId);
                if (userTeam != null)
                {
                    userTeam.Participants.Remove(user);
                    db.Entry(userTeam).State = EntityState.Modified;
                }

                var teamParticipants =
                    db.TeamParticipants.FirstOrDefault(tp => tp.ParticipantId == user.Id && tp.TeamId == userTeam.Id);
                if (teamParticipants != null)
                {
                    db.Entry(teamParticipants).State = EntityState.Deleted;
                }
            }
            else if (TeachersRoleId == userRoleId)
            {
                var teacherContests = db.Contests.Where(c => c.TeacherId == user.Id);
                foreach (var teacherContest in teacherContests)
                {
                    db.Entry(teacherContest).State = EntityState.Deleted;
                }
                db.SaveChanges();
            }
            

            if (user.UserInfoId != null)
            {
                var userInfo = await db.UserInfos.FindAsync(user.UserInfoId);
                if (userInfo != null)
                {
                    db.Entry(userInfo).State = EntityState.Deleted;
                    await db.SaveChangesAsync();
                }
            }
            
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            userManager.Delete(user);
            await db.SaveChangesAsync();

            TempData["Message"] = "Пользователь " + user.UserName + " был удален.";
            return RedirectToAction("DeletedAccountsList");
        }

        public async System.Threading.Tasks.Task<ActionResult> RestoreAccount(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            user.IsDeleted = false;
            db.Entry(user).State = EntityState.Modified;
            await db.SaveChangesAsync();

            TempData["Message"] = "Пользователь " + user.UserName + " был зарегистрирован/восстановлен.";
            return RedirectToAction("DeletedAccountsList");
        }

        public async System.Threading.Tasks.Task<ActionResult> News()
        {
            var model = await db.NewsItems.OrderByDescending(ni => ni.CreatedDate).ToListAsync();
            return View(model);
        }


        public ActionResult NewsItemCreate()
        {
            var model = new AdministratorNewsItemCreateViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> NewsItemCreate(AdministratorNewsItemCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newsItem = new NewsItem
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    Title = model.Title,
                    Text = model.Text
                };

                db.NewsItems.Add(newsItem);
                await db.SaveChangesAsync();

                TempData["Message"] = "Новость была добавлена.";
                return RedirectToAction("News");   
            }
            return View(model);
        }

        public async System.Threading.Tasks.Task<ActionResult> NewsItemEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsItem newsItem = await db.NewsItems.FindAsync(id);
            if (newsItem == null)
            {
                return HttpNotFound();
            }
            var model = new AdministratorNewsItemEditViewModel
            {
                NewsItemId = id,
                Title = newsItem.Title,
                Text = newsItem.Text
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> NewsItemEdit(AdministratorNewsItemEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newsItem = new NewsItem
                {
                    Id = (Guid)model.NewsItemId,
                    CreatedDate = DateTime.Now,
                    Title = model.Title,
                    Text = model.Text  
                };

                db.Entry(newsItem).State = EntityState.Modified;
                await db.SaveChangesAsync();

                TempData["Message"] = "Новость была отредактирована.";
                return RedirectToAction("News");
            }
            return View(model);
        }

        public async System.Threading.Tasks.Task<ActionResult> NewsItemDelete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsItem newsItem = await db.NewsItems.FindAsync(id);
            if (newsItem == null)
            {
                return HttpNotFound();
            }
                  
            db.Entry(newsItem).State = EntityState.Deleted;
            await db.SaveChangesAsync();

            TempData["Message"] = "Новость была удалена.";
            return RedirectToAction("News");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && db != null)
            {
                db.Dispose();
                db = null;
            }
            base.Dispose(disposing);
        }
	}
}