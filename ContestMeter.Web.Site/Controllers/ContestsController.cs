using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using ContestMeter.Web.Site.Database.Entities;
using ContestMeter.Web.Site.Database;
using ContestMeter.Web.Site.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ContestMeter.Web.Site.Controllers
{
    [Authorize(Roles = "administrator, teacher")]
    public class ContestsController : Controller
    {
        private ContestMeterDbContext _db = new ContestMeterDbContext();

        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            ContestsIndexViewModel model;
            if (!User.IsInRole("administrator"))
            {
                var currentUserId = User.Identity.GetUserId();
                var currentUserName = User.Identity.GetUserName();
                var contestTeachers = _db.ContestTeachers.Where(ct => ct.TeacherId == currentUserId);

                model = new ContestsIndexViewModel
                {
                    Contests =
                        _db.Contests.Include(c => c.ContestsType)
                            .Where(c => contestTeachers.Any(ct => ct.ContestId == c.Id))
                            .Include(c => c.Teacher)
                };
                return View(model);
            }

            model = new ContestsIndexViewModel
            {
                Contests = await _db.Contests.Include(c => c.ContestsType).Include(c => c.Teacher).ToListAsync()
            };

            return View(model);
        }

        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Index(ContestsIndexViewModel model)
        {
            //ToDo: разобраться и доделать. Сейчас всегда возвращает null или model.Contests.Any() = false, этого не должно быть
            if (model != null && model.Contests != null && model.Contests.Any())
            {
                foreach (var contest in model.Contests)
                {
                    db.Entry(contest).State = EntityState.Modified;
                }
                await db.SaveChangesAsync();

                return View(model);
            }
            return RedirectToAction("Index");
        }*/

        public ActionResult Create()
        {
            var model = new ContestsCreateViewModel
            {
                ContestsTypes = new SelectList(_db.ContestsTypes, "Id", "Name")
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Create(ContestsCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _db.Contests.AnyAsync(c => c.ContestsTypeId == model.ContestsTypeId && c.Name == model.Name))
                {
                    ModelState.AddModelError("", "Контест с таким именем и типом уже существует");
                }
                else
                {
                    var contest = new Contest
                    {
                        Id = Guid.NewGuid(),
                        ContestsTypeId = model.ContestsTypeId,
                        TeacherId = User.Identity.GetUserId(),
                        Name = model.Name,
                        IsActive = model.IsActive
                    };

                    var contestTeacher = new ContestTeacher
                    {
                        Id = Guid.NewGuid(),
                        ContestId = contest.Id,
                        TeacherId = contest.TeacherId
                    };

                    _db.Contests.Add(contest);
                    _db.ContestTeachers.Add(contestTeacher);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            model.ContestsTypes = new SelectList(_db.ContestsTypes, "Id", "Name");
            return View(model);
        }

        public async System.Threading.Tasks.Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var contest = await _db.Contests.FindAsync(id);
            if (contest == null)
            {
                return HttpNotFound();
            }
            if (contest.TeacherId != User.Identity.GetUserId() && !User.IsInRole("administrator") && !_db.ContestTeachers.Any(ct => ct.ContestId == contest.Id))
            {
                ViewBag.ErrorMessage = "Вы не имеете право редактировать этот контест";
                return View("Error");
            }
            var model = new ContestsEditViewModel
            {
                ContestId = contest.Id,
                ContestsTypeId = contest.ContestsTypeId,
                TeacherId = contest.TeacherId,
                Name = contest.Name,
                IsActive = contest.IsActive,
                ContestsTypes = new SelectList(_db.ContestsTypes, "Id", "Name")
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Edit(ContestsEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _db.Contests.AnyAsync(c => c.ContestsTypeId == model.ContestsTypeId && c.Name == model.Name && c.Id != model.ContestId))
                {
                    ModelState.AddModelError("", "Контест с таким именем и типом уже существует");
                }
                else
                {
                    var teacherId = User.IsInRole("administrator") ? model.TeacherId : User.Identity.GetUserId();
                    var contest = new Contest
                    {
                        Id = model.ContestId,
                        ContestsTypeId = model.ContestsTypeId,
                        TeacherId = teacherId,
                        Name = model.Name,
                        IsActive = model.IsActive
                    };

                    _db.Entry(contest).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            model.ContestsTypes = new SelectList(_db.ContestsTypes, "Id", "Name");
            return View(model);
        }

        public async System.Threading.Tasks.Task<ActionResult> ExceptionsLogs(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var contest = await _db.Contests.FindAsync(id);
            if (contest == null)
            {
                return HttpNotFound();
            }
            if (contest.TeacherId != User.Identity.GetUserId() && !User.IsInRole("administrator") && !_db.ContestTeachers.Any(ct => ct.ContestId == contest.Id))
            {
                ViewBag.ErrorMessage = "Вы не имеете право смотреть логи исключений для данного контеста";
                return View("Error");
            }
            var model = new ContestsExceptionsLogsViewModel
            {
                ExceptionsLogs = _db.ExceptionsLogs.Include(x => x.User).Where(el => el.ContestId == contest.Id).OrderByDescending(el => el.CreatedDate),
                ContestName = contest.Name
            };
            return View(model);
        }

        public async System.Threading.Tasks.Task<ActionResult> DeleteExceptionsLog(Guid? contestId, Guid? id)
        {
            if (contestId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var contest = await _db.Contests.FindAsync(contestId);
            if (contest == null)
            {
                return HttpNotFound();
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var exceptionsLog = await _db.ExceptionsLogs.FindAsync(id);
            if (exceptionsLog == null)
            {
                return HttpNotFound();
            }
            _db.Entry(exceptionsLog).State = EntityState.Deleted;
            await _db.SaveChangesAsync();

            return RedirectToAction("ExceptionsLogs", new {id = contestId});
        }

        // GET: /Contests/Delete/5
        public async System.Threading.Tasks.Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var contest = await _db.Contests.FindAsync(id);
            if (contest == null)
            {
                return HttpNotFound();
            }
            if (contest.TeacherId != User.Identity.GetUserId() && !User.IsInRole("administrator") && !_db.ContestTeachers.Any(ct => ct.ContestId == contest.Id))
            {
                ViewBag.ErrorMessage = "Вы не имеете право удалять этот контест";
                return View("Error");
            }
            return View(contest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var contest = await _db.Contests.FindAsync(id);
            if (contest == null)
            {
                return HttpNotFound();
            }
            var tasks = await _db.Tasks.Where(t => t.ContestId == id).ToListAsync();
            foreach (var task in tasks)
            {
                _db.Tasks.Remove(task);
                DeleteTaskDescriptionFiles(task.Id);
            }
            var devTools = await _db.DevelopmentTools.Where(t => t.ContestId == id).ToListAsync();
            foreach (var devTool in devTools)
                _db.DevelopmentTools.Remove(devTool);
            var userAttempts = await _db.UserAttempts.Where(t => t.ContestId == id).ToListAsync();
            foreach (var userAttempt in userAttempts)
                userAttempt.ContestId = null;
            var exceptionLogs = await _db.ExceptionsLogs.Where(t => t.ContestId == id).ToListAsync();
            foreach (var exceptionLog in exceptionLogs)
                exceptionLog.ContestId = null;

            _db.Contests.Remove(contest);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #region Teachers
        public ActionResult TeachersList(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var contest = _db.Contests.Find(id);
            if (contest == null)
            {
                return HttpNotFound();
            }
            if (contest.TeacherId != User.Identity.GetUserId() && !User.IsInRole("administrator") && !_db.ContestTeachers.Any(ct => ct.ContestId == contest.Id))
            {
                ViewBag.ErrorMessage = "Вы не имеете право просматривать список преподавателей для этого контеста";
                return View("Error");
            }

            var currentUserId = User.Identity.GetUserId();
            var roleStore = new RoleStore<IdentityRole>(_db);
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var teacherRoleUsers = roleManager.Roles.First(r => r.Name == "teacher").Users;

            var contestTeachers = _db.ContestTeachers.Where(ct => ct.ContestId == contest.Id).ToList();
            var teacherIds = new List<string>();
            var contestTeacherIds = new List<string>();
            foreach (var teacherRoleUser in teacherRoleUsers)
            {
                if (!contestTeachers.Any(ct => ct.ContestId == contest.Id && ct.TeacherId == teacherRoleUser.UserId))
                    teacherIds.Add(teacherRoleUser.UserId);
                else
                    contestTeacherIds.Add(teacherRoleUser.UserId);
            }

            var model = new ContestsTeachersListViewModel
            {
                ContestId = id,
                TeacherId = contest.TeacherId,
                Teachers = _db.Users.Where(t => teacherIds.Contains(t.Id) && !t.IsDeleted && t.Id != currentUserId),
                ContestTeachers = _db.Users.Where(t => contestTeacherIds.Contains(t.Id) && !t.IsDeleted && t.Id != currentUserId)
            };
            return PartialView(model);
        }

        public async System.Threading.Tasks.Task<ActionResult> AddTeachers(Guid? contestId, string id)
        {
            if (contestId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var contest = await _db.Contests.FindAsync(contestId);
            if (contest == null)
            {
                return HttpNotFound();
            }
            if (contest.TeacherId != User.Identity.GetUserId() && !User.IsInRole("administrator") && !_db.ContestTeachers.Any(ct => ct.ContestId == contestId))
            {
                ViewBag.ErrorMessage = "Вы не имеете право добавлять преподавателей для данного контеста";
                return View("Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (id == contest.TeacherId)
            {
                TempData["Message"] = "Вы не можете добавить создателя контеста в список его преподавателей";
                return RedirectToAction("Edit", new { id = contestId });
                //ViewBag.ErrorMessage = "Вы не можете добавить создателя контеста в список его преподавателей";
                //return View("Error");
            }
            var contestTeacher = new ContestTeacher
            {
                Id = Guid.NewGuid(),
                ContestId = (Guid)contestId,
                TeacherId = id
            };

            _db.Entry(contestTeacher).State = EntityState.Added;
            await _db.SaveChangesAsync();

            return RedirectToAction("Edit", new { id = contestId });
        }

        public async System.Threading.Tasks.Task<ActionResult> DeleteTeachers(Guid? contestId, string id)
        {
            if (contestId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var contest = await _db.Contests.FindAsync(contestId);
            if (contest == null)
            {
                return HttpNotFound();
            }
            if (contest.TeacherId != User.Identity.GetUserId() && !User.IsInRole("administrator") && !_db.ContestTeachers.Any(ct => ct.ContestId == contestId))
            {
                TempData["Message"] = "Вы не имеете право удалять преподавателей для данного контеста";
                return RedirectToAction("Edit", new { id = contestId });
                //ViewBag.ErrorMessage = "Вы не имеете право удалять преподавателей для данного контеста";
                //return View("Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (id == contest.TeacherId)
            {
                TempData["Message"] = "Вы не можете удалить создателя контеста из списка его преподавателей";
                return RedirectToAction("Edit", new { id = contestId });
                //ViewBag.ErrorMessage = "Вы не можете удалить создателя контеста из списка его преподавателей";
                //return View("Error");
            }
            var contestTeacher = _db.ContestTeachers.FirstOrDefault(ct => ct.TeacherId == id);
            if (contestTeacher == null)
            {
                return HttpNotFound();
            }
            _db.Entry(contestTeacher).State = EntityState.Deleted;
            await _db.SaveChangesAsync();

            return RedirectToAction("Edit", new { id = contestId });
        }
        #endregion

        #region Tasks
        public ActionResult TasksList(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var contest = _db.Contests.Find(id);
            if (contest == null)
            {
                return HttpNotFound();
            }
            if (contest.TeacherId != User.Identity.GetUserId() && !User.IsInRole("administrator") && !_db.ContestTeachers.Any(ct => ct.ContestId == contest.Id))
            {
                TempData["Message"] = "Вы не имеете право просматривать список задач для этого контеста";
                return RedirectToAction("Edit", new {id });
                //ViewBag.ErrorMessage = "Вы не имеете право просматривать список задач для этого контеста";
                //return View("Error");
            }
            var model = new ContestsTasksListViewModel
            {
                ContestId = id,
                Tasks = _db.Tasks.Where(t => t.ContestId == id).Include(t => t.Contest)
            };
            return PartialView(model);
        }

        public ActionResult TasksCreate(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var contest = _db.Contests.Find(id);
            if (contest == null)
            {
                return HttpNotFound();
            }
            if (contest.TeacherId != User.Identity.GetUserId() && !User.IsInRole("administrator") && !_db.ContestTeachers.Any(ct => ct.ContestId == contest.Id))
            {
                TempData["Message"] = "Вы не имеете право создавать задачи для этого контеста";
                return RedirectToAction("Edit", new { id });
                //ViewBag.ErrorMessage = "Вы не имеете право создавать задачи для этого контеста";
                //return View("Error");
            }
            var model = new ContestsTasksCreateViewModel
            {
                ContestId = id,
                TimeLimit = 1,
                Weight = 1
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> TasksCreate(ContestsTasksCreateViewModel model, HttpPostedFileBase fileUpload, HttpPostedFileBase checkerFile, HttpPostedFileBase[] testsFiles)
        {
            if (ModelState.IsValid)
            {
                if (await _db.Tasks.AnyAsync(t => t.ContestId == model.ContestId && (t.ExecutableName == model.ExecutableName || t.Name == model.Name)))
                {
                    ModelState.AddModelError("", "Задача в данном контесте с таким именем уже существует");
                }
                else
                {
                    var task = new Task
                    {
                        Id = Guid.NewGuid(),
                        ContestId = model.ContestId,
                        Name = model.Name,
                        ExecutableName = model.ExecutableName,
                        CheckerName = model.CheckerName,
                        TestsFolder = model.TestsFolder,
                        TimeLimit = model.TimeLimit,
                        Weight = model.Weight,
                        MaxSourceSize = model.MaxSourceSize,
                        MaxMemorySize = model.MaxMemorySize,
                        Rating = 0
                    };

                    if (string.IsNullOrEmpty(fileUpload?.FileName))
                    {
                        ModelState.AddModelError("", "Не выбран загружаемый файл с описанием задачи");
                        return View(model);
                    }
                    if (string.IsNullOrEmpty(checkerFile?.FileName))
                    {
                        ModelState.AddModelError("", "Не выбран загружаемый файл с чекером задачи");
                        return View(model);
                    }
                    if (testsFiles == null || testsFiles.Any(tf => string.IsNullOrEmpty(tf?.FileName)))
                    {
                        ModelState.AddModelError("", "Не выбраны файлы тестов для задачи");
                        return View(model);
                    }
                    
                    var ext = Path.GetExtension(fileUpload.FileName);
                    if (_allowedExtForTasksDescriptionFiles.Any(e => e == ext))
                    {
                        fileUpload.SaveAs(HostingEnvironment.ApplicationPhysicalPath +
                                            "Documents/TasksDescriptions/" +
                                            task.Id + ext);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Недопустимое расширения файла с описанием задачи");
                        return View(model);
                    }

                    var checkerFolder = ConfigurationManager.AppSettings["SiteConfigFolder"];
                    if (!Directory.Exists(checkerFolder + "\\Checkers\\"))
                        Directory.CreateDirectory(checkerFolder + "\\Checkers\\");
                    checkerFile.SaveAs(checkerFolder + "\\Checkers\\" + task.CheckerName);

                    if (!Directory.Exists(checkerFolder + "\\Tests\\" + task.ExecutableName))
                        Directory.CreateDirectory(checkerFolder + "\\Tests\\" + task.ExecutableName);
                    foreach (var testFile in testsFiles)
                    { 
                        testFile.SaveAs(checkerFolder + "\\Tests\\" + task.ExecutableName + "\\" + testFile.FileName);
                    }

                    _db.Tasks.Add(task);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Edit", new {id = model.ContestId});
                }
            }
            return View(model);
        }

        public async System.Threading.Tasks.Task<ActionResult> TasksEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var task = await _db.Tasks.FindAsync(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            var contest = _db.Contests.Find(task.ContestId);
            if (contest == null)
            {
                return HttpNotFound();
            }
            if (contest.TeacherId != User.Identity.GetUserId() && !User.IsInRole("administrator") && !_db.ContestTeachers.Any(ct => ct.ContestId == contest.Id))
            {
                TempData["Message"] = "Вы не имеете право редактировать задачи для этого контеста";
                return RedirectToAction("Edit", new { id = task.ContestId});
                //ViewBag.ErrorMessage = "Вы не имеете право редактировать задачи для этого контеста";
                //return View("Error");
            }
            var model = new ContestsTasksEditViewModel
            {
                TaskId = id,
                ContestId = task.ContestId,
                Name = task.Name,
                ExecutableName = task.ExecutableName,
                CheckerName = task.CheckerName,
                TestsFolder = task.TestsFolder,
                TimeLimit = task.TimeLimit,
                Weight = task.Weight,
                MaxSourceSize = task.MaxSourceSize,
                MaxMemorySize = task.MaxMemorySize,
                Rating = task.Rating
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> TasksEdit(ContestsTasksEditViewModel model, HttpPostedFileBase fileUpload, HttpPostedFileBase checkerFile, HttpPostedFileBase[] testsFiles)
        {
            if (ModelState.IsValid)
            {
                var task = new Task
                {
                    ContestId = model.ContestId,
                    Id = (Guid)model.TaskId,
                    Name = model.Name,
                    ExecutableName = model.ExecutableName,
                    CheckerName = model.CheckerName,
                    TestsFolder = model.TestsFolder,
                    TimeLimit = model.TimeLimit,
                    Weight = model.Weight,
                    MaxSourceSize = model.MaxSourceSize,
                    MaxMemorySize = model.MaxMemorySize,
                    Rating = model.Rating
                };

                if (string.IsNullOrEmpty(fileUpload?.FileName))
                {
                    ModelState.AddModelError("", "Не выбран загружаемый файл с описанием задачи");
                    return View(model);
                }
                if (string.IsNullOrEmpty(checkerFile?.FileName))
                {
                    ModelState.AddModelError("", "Не выбран загружаемый файл с чекером задачи");
                    return View(model);
                }
                if (testsFiles == null || testsFiles.Any(tf => string.IsNullOrEmpty(tf?.FileName)))
                {
                    ModelState.AddModelError("", "Не выбраны файлы тестов для задачи");
                    return View(model);
                }

                var ext = Path.GetExtension(fileUpload.FileName);
                if (_allowedExtForTasksDescriptionFiles.Any(e => e == ext))
                {
                    fileUpload.SaveAs(HostingEnvironment.ApplicationPhysicalPath +
                                        "Documents/TasksDescriptions/" +
                                        task.Id + ext);
                }
                else
                {
                    ModelState.AddModelError("", "Недопустимое расширения файла с описанием задачи");
                    return View(model);
                }

                var checkerFolder = ConfigurationManager.AppSettings["SiteConfigFolder"];
                if (!Directory.Exists(checkerFolder + "\\Checkers\\"))
                    Directory.CreateDirectory(checkerFolder + "\\Checkers\\");
                checkerFile.SaveAs(checkerFolder + "\\Checkers\\" + task.CheckerName);

                if (!Directory.Exists(checkerFolder + "\\Tests\\" + task.ExecutableName))
                    Directory.CreateDirectory(checkerFolder + "\\Tests\\" + task.ExecutableName);
                foreach (var testFile in testsFiles)
                {
                    testFile.SaveAs(checkerFolder + "\\Tests\\" + task.ExecutableName + "\\" + testFile.FileName);
                }

                _db.Entry(task).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Edit", new { id = model.ContestId });
            }
            return View(model);
        }

        public async System.Threading.Tasks.Task<ActionResult> TasksDelete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var task = await _db.Tasks.FindAsync(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            var contest = _db.Contests.Find(task.ContestId);
            if (contest == null)
            {
                return HttpNotFound();
            }
            if (contest.TeacherId != User.Identity.GetUserId() && !User.IsInRole("administrator") && !_db.ContestTeachers.Any(ct => ct.ContestId == contest.Id))
            {
                TempData["Message"] = "Вы не имеете право удалять задачи этого контеста";
                return RedirectToAction("Edit", new { id = task.ContestId });
                //ViewBag.ErrorMessage = "Вы не имеете право удалять задачи этого контеста";
                //return View("Error");
            }
            var model = new ContestsTasksDeleteViewModel
            {
                ContestId = task.ContestId,
                TaskId = task.Id,
                TaskDescriptionFileName = GetFirstTaskDescriptionFile(task.Id),
                Name = task.Name,
                ExecutableName = task.ExecutableName,
                CheckerName = task.CheckerName,
                TestsFolder = task.TestsFolder,
                TimeLimit = task.TimeLimit,
                Weight = task.Weight,
                MaxSourceSize = task.MaxSourceSize,
                MaxMemorySize = task.MaxMemorySize,
                Rating = task.Rating
            };
            return View(model);
        }

        [HttpPost, ActionName("TasksDelete")]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> TasksDeleteConfirmed(Guid? id)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            var userAttempts = await _db.UserAttempts.Where(t => t.TaskId == id).ToListAsync();
            foreach (var userAttempt in userAttempts)
                userAttempt.TaskId = null;

            DeleteTaskDescriptionFiles(task.Id);

            var checkerFolder = ConfigurationManager.AppSettings["SiteConfigFolder"];
            if (System.IO.File.Exists(checkerFolder + "\\Checkers\\" + task.CheckerName))
                System.IO.File.Delete(checkerFolder + "\\Checkers\\" + task.CheckerName);

            var di = new DirectoryInfo(checkerFolder + "\\Tests\\" + task.ExecutableName + "\\");
            foreach (var testFile in di.GetFiles())
            {
                testFile.Delete();
            }
            if (Directory.Exists(checkerFolder + "\\Tests\\" + task.ExecutableName + "\\"))
                Directory.Delete(checkerFolder + "\\Tests\\" + task.ExecutableName + "\\");

            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #region DevelopmentTools
        public ActionResult DevelopmentToolsList(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var contest = _db.Contests.Find(id);
            if (contest == null)
            {
                return HttpNotFound();
            }
            if (contest.TeacherId != User.Identity.GetUserId() && !User.IsInRole("administrator") && !_db.ContestTeachers.Any(ct => ct.ContestId == contest.Id))
            {
                TempData["Message"] = "Вы не имеете право просматривать список компиляторов для этого контеста";
                return RedirectToAction("Edit", new { id });
                //ViewBag.ErrorMessage = "Вы не имеете право просматривать список компиляторов для этого контеста";
                //return View("Error");
            }
            var model = new ContestsDevelopmentToolsListViewModel
            {
                ContestId = id,
                DevelopmentTools = _db.DevelopmentTools.Where(t => t.ContestId == id).Include(t => t.Contest)
            };
            return PartialView(model);
        }

        public ActionResult DevelopmentToolsCreate(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var contest = _db.Contests.Find(id);
            if (contest == null)
            {
                return HttpNotFound();
            }
            if (contest.TeacherId != User.Identity.GetUserId() && !User.IsInRole("administrator") && !_db.ContestTeachers.Any(ct => ct.ContestId == contest.Id))
            {
                TempData["Message"] = "Вы не имеете право создавать компиляторы для этого контеста";
                return RedirectToAction("Edit", new { id });
                //ViewBag.ErrorMessage = "Вы не имеете право создавать компиляторы для этого контеста";
                //return View("Error");
            }
            var model = new ContestsDevelopmentToolsCreateViewModel
            {
                ContestId = id
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> DevelopmentToolsCreate(ContestsDevelopmentToolsCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _db.DevelopmentTools.AnyAsync(d => d.ContestId == model.ContestId && d.Name == model.Name))
                {
                    ModelState.AddModelError("", "Компилятор в данном контесте с таким именем уже существует");
                }
                else
                {
                    var developmentTool = new DevelopmentTool
                    {
                        Id = Guid.NewGuid(),
                        ContestId = model.ContestId,
                        Name = model.Name,
                        CompileCommand = model.CompileCommand,
                        CommandArgs = model.CommandArgs
                    };
                    _db.DevelopmentTools.Add(developmentTool);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Edit", new { id = model.ContestId });
                }
            }
            return View(model);
        }

        public async System.Threading.Tasks.Task<ActionResult> DevelopmentToolsEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var developmentTool = await _db.DevelopmentTools.FindAsync(id);
            if (developmentTool == null)
            {
                return HttpNotFound();
            }
            var contest = _db.Contests.Find(developmentTool.ContestId);
            if (contest == null)
            {
                return HttpNotFound();
            }
            if (contest.TeacherId != User.Identity.GetUserId() && !User.IsInRole("administrator") && !_db.ContestTeachers.Any(ct => ct.ContestId == contest.Id))
            {
                TempData["Message"] = "Вы не имеете право редактировать компиляторы для этого контеста";
                return RedirectToAction("Edit", new { id = developmentTool.ContestId});
                //ViewBag.ErrorMessage = "Вы не имеете право редактировать компиляторы для этого контеста";
                //return View("Error");
            }
            var model = new ContestsDevelopmentToolsEditViewModel
            {
                DevelopmentToolId = id,
                ContestId = developmentTool.ContestId,
                Name = developmentTool.Name,
                CompileCommand = developmentTool.CompileCommand,
                CommandArgs = developmentTool.CommandArgs
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> DevelopmentToolsEdit(ContestsDevelopmentToolsEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _db.DevelopmentTools.AnyAsync(d => d.ContestId == model.ContestId && d.Name == model.Name))
                {
                    ModelState.AddModelError("", "Компилятор в данном контесте с таким именем уже существует");
                }
                else
                {
                    var developmentTool = new DevelopmentTool
                    {
                        ContestId = model.ContestId,
                        Id = (Guid)model.DevelopmentToolId,
                        Name = model.Name,
                        CompileCommand = model.CompileCommand,
                        CommandArgs = model.CommandArgs
                    };

                    _db.Entry(developmentTool).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Edit", new { id = model.ContestId });
                }
            }
            return View(model);
        }

        public async System.Threading.Tasks.Task<ActionResult> DevelopmentToolsDelete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var developmentTool = await _db.DevelopmentTools.FindAsync(id);
            if (developmentTool == null)
            {
                return HttpNotFound();
            }
            var contest = _db.Contests.Find(developmentTool.ContestId);
            if (contest == null)
            {
                return HttpNotFound();
            }
            if (contest.TeacherId != User.Identity.GetUserId() && !User.IsInRole("administrator") && !_db.ContestTeachers.Any(ct => ct.ContestId == contest.Id))
            {
                TempData["Message"] = "Вы не имеете право удалять компиляторы этого контеста";
                return RedirectToAction("Edit", new { id = developmentTool.ContestId });
                //ViewBag.ErrorMessage = "Вы не имеете право удалять компиляторы этого контеста";
                //return View("Error");
            }
            var model = new ContestsDevelopmentToolsDeleteViewModel
            {
                ContestId = developmentTool.ContestId,
                DevelopmentToolId = developmentTool.Id,
                Name = developmentTool.Name,
                CompileCommand = developmentTool.CompileCommand,
                CommandArgs = developmentTool.CommandArgs
            };
            return View(model);
        }

        [HttpPost, ActionName("DevelopmentToolsDelete")]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> DevelopmentToolsDeleteConfirmed(Guid? id)
        {
            var developmentTool = await _db.DevelopmentTools.FindAsync(id);
            if (developmentTool == null)
            {
                return HttpNotFound();
            }
            var userAttempts = await _db.UserAttempts.Where(d => d.DevelopmentToolId == id).ToListAsync();
            foreach (var userAttempt in userAttempts)
                userAttempt.DevelopmentToolId = null;

            _db.DevelopmentTools.Remove(developmentTool);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #region Additional Methods And Properties
        private readonly string[] _allowedExtForTasksDescriptionFiles =
        {
            ".pdf"
        };

        private string GetBaseUrl()
        {
            var request = HttpContext.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (!string.IsNullOrWhiteSpace(appUrl)) appUrl += "/";

            return string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);
        }

        private string GetFirstTaskDescriptionFile(Guid taskId)
        {
            var taskFiles = Directory.GetFiles(HostingEnvironment.ApplicationPhysicalPath + "Documents/TasksDescriptions/",
                taskId + ".*");
            return GetBaseUrl() + "Documents/TasksDescriptions/" + Path.GetFileName(taskFiles.FirstOrDefault());
        }

        private void DeleteTaskDescriptionFiles(Guid taskId)
        {
            var taskFiles = Directory.GetFiles(HostingEnvironment.ApplicationPhysicalPath + "Documents/TasksDescriptions/",
                taskId + ".*");
            if (taskFiles.Any())
            {
                foreach (var file in taskFiles)
                {
                    var fi = new FileInfo(file);
                    fi.Delete();
                }
            }
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
