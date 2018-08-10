using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using ContestMeter.Common;
using ContestMeter.Web.Site.Database;
using ContestMeter.Web.Site.Database.Entities;
using ContestMeter.Web.Site.Models;
using ContestMeter.Web.Site.Queue;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Task = ContestMeter.Web.Site.Database.Entities.Task;

namespace ContestMeter.Web.Site.Controllers
{
    [Authorize(Roles = "participant")]
    public class ParticipantController : Controller
    {
        private static CheckSolutionQueue checkSolutionQueue = new CheckSolutionQueue(2000, 4);

        private ContestMeterDbContext db = new ContestMeterDbContext();


        public async System.Threading.Tasks.Task<ActionResult> Contests()
        {
            var model = new ParticipantContestsViewModel
            {
                Contests = await db.Contests.Include(c => c.ContestsType).Include(c => c.Teacher).ToListAsync()
            };
            return View(model);
        }

        [AllowAnonymous]
        public async System.Threading.Tasks.Task<ActionResult> Information(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contest contest = await db.Contests.FindAsync(id);
            if (contest == null)
            {
                return HttpNotFound();
            }
            
            var userAttempts =
                db.UserAttempts.Where(ua => ua.ContestId == contest.Id)
                    .Include(ua => ua.Contest)
                    .Include(ua => ua.DevelopmentTool)
                    .Include(ua => ua.Task)
                    .Include(ua => ua.User)
                    .OrderByDescending(dt => dt.CreatedDate);
            
            var model = new ParticipantInformationViewModel
            {
                Contest = contest,
                UserAttempts = userAttempts
            };
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult DownloadLastPostedSolution(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var userAttempt = db.UserAttempts.Find(id);
            if (userAttempt == null)
            {
                return HttpNotFound();
            }

            var currentUserId = User.Identity.GetUserId();
            bool isAdministrator = User.IsInRole("administrator");
            if (!isAdministrator &&
                !db.ContestTeachers.Any(
                    ct =>
                        ct.ContestId == userAttempt.ContestId &&
                        userAttempt.Contest.TeacherId == currentUserId))
            {
                //ToDo: вывести ошибку, что данный пользователь не может просмотреть решение
                TempData["Message"] = "Вы не имеете право просматривать решение данного участника";
                return RedirectToAction("Information", new { id });
            }
            var lastPostedSolution =
                db.PostedSolutions.FirstOrDefault(
                    ps => ps.UserId == userAttempt.UserId && ps.TaskId == userAttempt.TaskId);
            if (lastPostedSolution == null)
            {
                return HttpNotFound();
            }
            var lastPostedSolutionFile = lastPostedSolution.Solution;

            return File(lastPostedSolutionFile, "application/octet-stream", userAttempt.User.UserName + "_" + userAttempt.Task.ExecutableName + "_" + userAttempt.CreatedDate.ToShortDateString() + "_" + userAttempt.CreatedDate.ToShortTimeString() + userAttempt.SolutionExtension);
        }

        [AllowAnonymous]
        public async System.Threading.Tasks.Task<ActionResult> DeleteUserAttempt(Guid? contestId, Guid? id)
        {
            if (contestId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var contest = await db.Contests.FindAsync(contestId);
            if (contest == null)
            {
                return HttpNotFound();
            }
            if (contest.TeacherId != User.Identity.GetUserId() && !User.IsInRole("administrator") && !db.ContestTeachers.Any(ct => ct.ContestId == contestId))
            {
                TempData["Message"] = "Вы не имеете право удалять результаты проверок для данного контеста";
                return RedirectToAction("Information", new {id = contestId});
                //ViewBag.ErrorMessage = "Вы не имеете право удалять результаты проверок для данного контеста";
                //return View("Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userAttempt = await db.UserAttempts.FindAsync(id);
            if (userAttempt == null)
            {
                return HttpNotFound();
            }
            db.Entry(userAttempt).State = EntityState.Deleted;
            await db.SaveChangesAsync();

            return RedirectToAction("Information", new { id = contestId });
        }

        public async System.Threading.Tasks.Task<ActionResult> Participate(Guid? id, int? currentTaskNumber)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (currentTaskNumber == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contest contest = await db.Contests.FindAsync(id);
            if (contest == null)
            {
                return HttpNotFound();
            }
            if (!contest.IsActive)
            {
                TempData["Message"] = "Произошла ошибка: выбранный контест '" + contest.Name + "' не активен.";
                return RedirectToAction("Contests");
                //ViewBag.ErrorMessage = "Произошла ошибка: выбранный контест неактивен";
                //return View("Error");
            }


            var currentUserId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(currentUserId);
            if (currentUser == null)
            {
                return HttpNotFound();
            }
            if (!currentUser.Team.Contests.Any(c => c.Id == contest.Id))
            {
                TempData["Message"] = "Команда '" + currentUser.Team.Name + "' не может участвовать в контесте '" + contest.Name + "'. Пожалуйста, обратитесь к администратору.";
                return RedirectToAction("Contests");
            }


            var tasks = await db.Tasks.Where(t => t.ContestId == contest.Id).ToListAsync();
            if (tasks == null || tasks.Count < 1)
            {
                TempData["Message"] = "Произошла ошибка: кол-во задач в контесте '" + contest.Name + "' равно 0.";
                return RedirectToAction("Contests");
                //ViewBag.ErrorMessage = "Произошла ошибка: кол-во задач должно быть больше 0";
                //return View("Error");
            }
            if (!Directory.Exists(HostingEnvironment.ApplicationPhysicalPath + "Documents/TasksDescriptions"))
            {
                TempData["Message"] = "Произошла ошибка: каталог '" + HostingEnvironment.ApplicationPhysicalPath + "Documents/TasksDescriptions' не найден.";
                return RedirectToAction("Contests");
                //ViewBag.ErrorMessage = "Произошла ошибка: каталог '" + HostingEnvironment.ApplicationPhysicalPath + "Documents/TasksDescriptions' не найден.";
                //return View("Error");
            }
            var taskFiles = Directory.GetFiles(HostingEnvironment.ApplicationPhysicalPath + "Documents/TasksDescriptions/",
                (tasks[(int)currentTaskNumber].Id).ToString() + ".*");
            if (!taskFiles.Any())
            {
                TempData["Message"] = "Произошла ошибка: pdf-файл с описанием задания '" + tasks[(int)currentTaskNumber].Name + "' не найден.";
                return RedirectToAction("Contests");
                //ViewBag.ErrorMessage = "Произошла ошибка: pdf-файл с описанием задания '" + tasks[(int)currentTaskNumber].Name + "' не найден.";
                //return View("Error");
            }

            var model = new ParticipantParticipateViewModel
            {
                ContestId = contest.Id,
                Tasks = tasks,
                CurrentTaskNumber = (int)currentTaskNumber,
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Participate([Bind(Include = "ContestId, CurrentTaskId, CurrentTaskNumber, SelectedDevelopmentToolId")]ParticipantTaskPartialViewModel model, HttpPostedFileBase fileUpload)
        {
            var tasks = await db.Tasks.Where(t => t.ContestId == model.ContestId).ToListAsync();
            if (tasks == null || tasks.Count < 1)
            {
                TempData["Message"] = "Произошла ошибка: кол-во задач и компиляторов должно быть больше 0";
                return RedirectToAction("Contests");
                //ViewBag.ErrorMessage = "Произошла ошибка: кол-во задач и компиляторов должно быть больше 0";
                //return View("Error");
            }
            var model2 = new ParticipantParticipateViewModel
            {
                ContestId = model.ContestId,
                CurrentTaskNumber = model.CurrentTaskNumber,
                Tasks = tasks
            };
            if (ModelState.IsValid)
            {
                if (fileUpload != null && !String.IsNullOrEmpty(fileUpload.FileName))
                {
                    var ext = Path.GetExtension(fileUpload.FileName);
                    if (_allowedExtForSolutionFiles.Any(e => e == ext))
                    {
                        byte[] solutionData;
                        using (var binaryReader = new BinaryReader(fileUpload.InputStream))
                        {
                            solutionData = binaryReader.ReadBytes(fileUpload.ContentLength);
                        }
                        string currentUserId = User.Identity.GetUserId();
                        PostedSolution postedSolution = await db.PostedSolutions.Where(
                                    s => s.TaskId == model.CurrentTaskId && s.UserId == currentUserId)
                                    .FirstOrDefaultAsync();
                        Guid postedSolutionId;
                        if (postedSolution != null)
                        {
                            postedSolutionId = postedSolution.Id;
                            postedSolution.Solution = solutionData;
                            db.Entry(postedSolution).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                        }
                        else
                        {
                            postedSolutionId = Guid.NewGuid();
                            postedSolution = new PostedSolution
                            {
                                Id = postedSolutionId,
                                UserId = User.Identity.GetUserId(),
                                TaskId = model.CurrentTaskId,
                                DevelopmentToolId = model.SelectedDevelopmentToolId,
                                Solution = solutionData,
                                IsChecked = false
                            };

                            db.PostedSolutions.Add(postedSolution);
                            db.SaveChanges();
                        }
                        var task = postedSolution.Task;
                        var contest = task.Contest;
                        var devTool = db.DevelopmentTools.Find(model.SelectedDevelopmentToolId);
                        var user = db.Users.Find(currentUserId);
                        string postedSolutionsRootFolder = ConfigurationManager.AppSettings["PostedSolutionsRootFolder"];
                        string contestTypeName = contest.ContestsType.Name;
                        string contestName = contest.Name;
                        string userIp = user.Ip;
                        string solutionsFolder = Path.Combine(postedSolutionsRootFolder, contestTypeName, contestName,
                            userIp);

                        if (!Directory.Exists(solutionsFolder))
                            Directory.CreateDirectory(solutionsFolder);
                        string sourcePath = Path.Combine(solutionsFolder, task.ExecutableName) + ext;

                        if (ByteArrayToFile(sourcePath, solutionData))
                        {
                            try
                            {
                                var taskCheckSolution = new System.Threading.Tasks.Task<Solution>(() =>
                                {
                                    return CheckPostedSolution(contest, user, task, devTool, solutionsFolder,
                                        LocalFolder, sourcePath);
                                });

                                checkSolutionQueue.AddTask(taskCheckSolution);
                                taskCheckSolution.Wait();
                                Solution solution = taskCheckSolution.Result;

                                var userAttempt = new UserAttempt
                                {
                                    Id = Guid.NewGuid(),
                                    ContestId = contest.Id,
                                    TaskId = task.Id,
                                    UserId = user.Id,
                                    CreatedDate = DateTime.Now,
                                    Solution = solutionData,
                                    SolutionExtension = ext,
                                    Score = solution.WeightedScore,
                                    DevelopmentToolId = devTool.Id,
                                    FailedRuns = solution.FailedRuns,
                                    FailedChecks = solution.FailedChecks
                                };
                                db.UserAttempts.Add(userAttempt);
                                postedSolution.IsChecked = true;
                                db.Entry(postedSolution).State = EntityState.Modified;
                                await db.SaveChangesAsync();

                                TempData["Message"] = "Решение было отправлено и проверено. \nБаллов: " + solution.WeightedScore 
                                    + "\nПроваленных запусков: " + solution.FailedRuns 
                                    + "\nПроваленных тестов: " + solution.FailedChecks;
                                //ToDo: вывести результат в другом виде?
                            }
                            catch (Exception ex)
                            {
                                var exeptionLog = new ExceptionsLog
                                {
                                    Id = Guid.NewGuid(),
                                    ContestId = contest.Id,
                                    CreatedDate = DateTime.Now,
                                    UserId = user.Id,
                                    Text = ex.InnerException.Message
                                };
                                db.ExceptionsLogs.Add(exeptionLog);
                                db.SaveChanges();

                                TempData["Message"] = "При проверке решения возникло исключение: " + ex.InnerException.Message;
                                //ToDo: вывести результат в другом виде?
                            }

                            if (Directory.Exists(postedSolutionsRootFolder))
                                Directory.Delete(postedSolutionsRootFolder, true);
                        }
                        else
                        {
                            TempData["Message"] = "Решение не было сохранено в локальном каталоге участника";
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Недопустимое расширения загружаемого файла решения");
                        return View(model2);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Не выбран загружаемый файл решения");
                    return View(model2);
                }

                //db.Tasks.Add(task);
                //await db.SaveChangesAsync();
                return RedirectToAction("Participate", new { id = model.ContestId, currentTaskNumber = model.CurrentTaskNumber });
            }
            return View(model2);
        }

        public ActionResult TaskPartial(Guid? contestId, int? currentTaskNumber)
        {
            if (currentTaskNumber == null || contestId == null)
            {
                return HttpNotFound();
            }
            var contest = db.Contests.FirstOrDefault(c => c.Id == contestId);
            if (contest == null)
            {
                return HttpNotFound();
            }
            if (!contest.IsActive)
            {
                ViewBag.ErrorMessage = "Произошла ошибка: выбранный контест не активен";
                return PartialView("PartialError");
            }
            var tasks = db.Tasks.Where(t => t.ContestId == contestId).ToList();
            if (tasks.Count < 1)
            {
                ViewBag.ErrorMessage = "Произошла ошибка: кол-во задач должно быть больше 0";
                return PartialView("PartialError");
            }
            if (!Directory.Exists(HostingEnvironment.ApplicationPhysicalPath + "Documents/TasksDescriptions"))
            {
                TempData["Message"] = "Произошла ошибка: каталог '" + HostingEnvironment.ApplicationPhysicalPath + "Documents/TasksDescriptions' не найден.";
                return RedirectToAction("Contests");
                //ViewBag.ErrorMessage = "Произошла ошибка: каталог '" + HostingEnvironment.ApplicationPhysicalPath + "Documents/TasksDescriptions' не найден.";
                //return View("Error");
            }
            var taskFiles = Directory.GetFiles(HostingEnvironment.ApplicationPhysicalPath + "Documents/TasksDescriptions/",
                (tasks[(int)currentTaskNumber].Id).ToString() + ".*");
            if (!taskFiles.Any())
            {
                //ViewBag.ErrorMessage = "Произошла ошибка: pdf-файл с описанием задания '" + tasks[(int)currentTaskNumber].Name + "' не найден.";
                TempData["Message"] = "Произошла ошибка: pdf-файл с описанием задания '" + tasks[(int)currentTaskNumber].Name + "' не найден.";
                return RedirectToAction("Contests");
                //return Content("Произошла ошибка: pdf-файл с описанием задания '" + tasks[(int)currentTaskNumber].Name + "' не найден.");
            }
            var devTools = db.DevelopmentTools.Where(d => d.ContestId == contestId).ToList();
            if (devTools.Count < 1)
            {
                ViewBag.ErrorMessage = "Произошла ошибка: кол-во компиляторов должно быть больше 0";
                return PartialView("PartialError");
            }
            var model = new ParticipantTaskPartialViewModel
            {
                ContestId = (Guid)contestId,
                CurrentTask = tasks[(int)currentTaskNumber],
                CurrentTaskId = tasks[(int)currentTaskNumber].Id,
                CurrentTaskNumber = (int)currentTaskNumber,
                DevelopmentTools = new SelectList(devTools, "Id", "Name"),
                SelectedDevelopmentToolId = devTools.First().Id,
                TaskDescriptionFileName = GetFirstTaskDescriptionFile(tasks[(int)currentTaskNumber].Id)
            };
            return PartialView(model);
        }

        #region Addition

        //ToDo: вынести добавление разрешенных расширений в создание инструментов разработчика
        private string[] _allowedExtForSolutionFiles =
        {
            ".cs", ".c", ".cpp", ".pas", ".py", ".java", ".f", ".cc", ".f77", ".f90"
        };

        //ToDo: возможно, добавить расширения для загружаемых решений участников
        /*".cs", ".c", ".cpp", ".pas", ".json", ".php", ".phtml", ".rb",
        ".py", ".f", ".cc", ".f77", ".f90", ".java", ".ph", ".pl", ".pm", ".perl"*/

        private string GetBaseUrl()
        {
            var request = HttpContext.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (!string.IsNullOrWhiteSpace(appUrl)) appUrl += "/";

            var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

            return baseUrl;
        }

        private string GetFirstTaskDescriptionFile(Guid taskId)
        {
            var taskFiles = Directory.GetFiles(HostingEnvironment.ApplicationPhysicalPath + "Documents/TasksDescriptions/",
                taskId.ToString() + ".*");
            return GetBaseUrl() + "Documents/TasksDescriptions/" + Path.GetFileName(taskFiles.FirstOrDefault());
        }

        public static string LocalFolder
        {
            get
            {
                var folder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tester");

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                return folder;
            }
        }

        public bool ByteArrayToFile(string _FileName, byte[] _ByteArray)
        {
            try
            {
                using (System.IO.FileStream _FileStream =
                    new System.IO.FileStream(_FileName, System.IO.FileMode.Create,
                        System.IO.FileAccess.Write))
                {
                    _FileStream.Write(_ByteArray, 0, _ByteArray.Length);
                }

                return true;
            }
            catch (Exception _Exception)
            {
                //Console.WriteLine("Exception caught in process: {0}",
                //                  _Exception.ToString());
            }

            return false;
        }

        private Solution CheckPostedSolution(Contest contest, ApplicationUser user, Task task, DevelopmentTool devTool, string solutionsFolder, string localFolder, string sourcePath)
        {
            Solution solution = new Solution(new LocalFileSystem());
            solution.Configuration = new Common.Configuration
            {
                FileSystem = new LocalFileSystem(),
                ContestType = contest.ContestsType.Name,
                ContestName = contest.Name,
                Site = ConfigurationManager.AppSettings["SiteConfigFolder"]
            };
            solution.DevTool = new DeveloperTool
            {
                Name = devTool.Name,
                CompileCommand = devTool.CompileCommand,
                CommandArgs = devTool.CommandArgs,
                IsExeFile = false
            };
            solution.Task = new Common.Task
            {
                Name = task.Name,
                ExecutableName = task.ExecutableName,
                CheckerName = task.CheckerName,
                TestsFolder = task.TestsFolder,
                TimeLimit = task.TimeLimit,
                Weight = task.Weight,
                MaxSourceSize = task.MaxSourceSize,
                MaxMemorySize = task.MaxMemorySize
            };
            solution.Path = sourcePath;
            solution.LocalSourcePath = sourcePath;

            solution.Check(LocalFolder, solutionsFolder);

            return solution;
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
	}
}