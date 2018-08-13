using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using ContestMeter.Web.Site.Database;
using ContestMeter.Web.Site.Database.Entities;
using ContestMeter.Web.Site.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ContestMeter.Web.Site.Controllers
{
    [Authorize(Roles = "administrator")]
    public class TeamController : Controller
    {
        private readonly ContestMeterDbContext _db = new ContestMeterDbContext();

        [AllowAnonymous]
        public ActionResult Index()
        {
            var model = new TeamsIndexViewModel
            {
                Teams = _db.Teams.Include(c => c.Contests).Include(p => p.Participants).ToList()
            };

            return View(model);
        }

        public ActionResult Create()
        {
            if (!User.IsInRole("administrator"))
            {
                TempData["Message"] = "Только администратор имеет право создавать команды";
                return RedirectToAction("Index");
                //ViewBag.ErrorMessage = "Только администратор имеет право создавать команды";
                //return View("Error");
            }
            var model = new TeamsCreateViewModel
            {
                Name = "",
                MaxTeamNumber = 1
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TeamsCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _db.Teams.AnyAsync(c => c.Name == model.Name) || model.Name == "")
                {
                    ModelState.AddModelError("", "Команда с таким названием уже существует или не задано название команды");
                }
                else
                {
                    var team = new Team
                    {
                        Id = Guid.NewGuid(),
                        Name = model.Name,
                        MaxTeamNumber = model.MaxTeamNumber,
                        Rating = 0,
                        Contests = new HashSet<Contest>(),
                        Participants = new List<ApplicationUser>()
                    };

                    _db.Teams.Add(team);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var team = await _db.Teams.FindAsync(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            if (!User.IsInRole("administrator"))
            {
                TempData["Message"] = "Вы не имеете право редактировать эту команду";
                return RedirectToAction("Edit", new {id});
                //ViewBag.ErrorMessage = "Вы не имеете право редактировать эту команду";
                //return View("Error");
            }
            var model = new TeamsEditViewModel
            {
                TeamId = team.Id,
                Name = team.Name,
                MaxTeamNumber = team.MaxTeamNumber 
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(TeamsEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentTeam = _db.Teams.Find(model.TeamId);
                if (currentTeam == null)
                {
                    return HttpNotFound();
                }
                if ((await _db.Teams.AnyAsync(t => t.Name == model.Name) || model.Name == "") && model.MaxTeamNumber == currentTeam.MaxTeamNumber)
                {
                    ModelState.AddModelError("", "Команда с таким названием уже существует или не задано название команды");
                }
                else if (model.Name != "Одиночки" && currentTeam.Name == "Одиночки")
                {
                    ModelState.AddModelError("", "Нельзя изменить название у команды 'Одиночки'");
                }
                else
                {
                    if (model.MaxTeamNumber < currentTeam.Participants.Count)
                    {
                        ModelState.AddModelError("", "Максимальное количество участников в команде меньше, чем текущее количество участников. Пожалуйста, сначала сократите текущее количество участников в команде.");
                    }
                    else
                    {
                        var team = _db.Teams.Find(model.TeamId);
                        if (team == null)
                        {
                            return HttpNotFound();
                        }
                        team.Name = model.Name;
                        team.MaxTeamNumber = model.MaxTeamNumber;

                        _db.Entry(team).State = EntityState.Modified;
                        await _db.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(model);
        }

        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var team = await _db.Teams.FindAsync(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            if (!User.IsInRole("administrator"))
            {
                TempData["Message"] = "Только администратор имеет право удалять команды";
                return RedirectToAction("Edit", new { id });
                //ViewBag.ErrorMessage = "Только администратор имеет право удалять команды";
                //return View("Error");
            }
            return View(team);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var team = await _db.Teams.FindAsync(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            var teamParticipants = _db.Users.Where(u => u.TeamId == id);
            foreach (var teamParticipant in teamParticipants)
            {
                teamParticipant.TeamId = null;
                teamParticipant.Team = null;
                _db.Entry(teamParticipant).State = EntityState.Modified;
            }

            _db.Teams.Remove(team);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #region Contests
        public ActionResult TeamContestsList(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var team = _db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            if (!User.IsInRole("administrator"))
            {
                TempData["Message"] = "Только администратор имеет право просматривать список контестов для этой команды";
                return RedirectToAction("Edit", new { id });
                //ViewBag.ErrorMessage = "Только администратор имеет право просматривать список контестов для этой команды";
                //return View("Error");
            }
            var contests = _db.Contests;

            var teamContests = _db.TeamContests.Where(tc => tc.TeamId == team.Id).ToList();
            var contestIds = new List<Guid>();
            var teamContestIds = new List<Guid>();
            foreach (var contest in contests)
            {
                if (!teamContests.Any(tc => tc.TeamId == team.Id && tc.ContestId == contest.Id))
                    contestIds.Add(contest.Id);
                else
                    teamContestIds.Add(contest.Id);
            }

            var model = new TeamsContestsListViewModel
            {
                TeamId = id,
                Contests = _db.Contests.Include(c => c.Teacher).Include(c => c.ContestsType).Where(c => contestIds.Contains(c.Id)).AsQueryable(),
                TeamContests = _db.Contests.Include(c => c.Teacher).Include(c => c.ContestsType).Where(tc => teamContestIds.Contains(tc.Id)).AsQueryable()
            };

            return PartialView(model);
        }

        public async Task<ActionResult> AddContests(Guid? teamId, Guid? id)
        {
            if (teamId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var team = await _db.Teams.FindAsync(teamId);
            if (team == null)
            {
                return HttpNotFound();
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var teamContest = new TeamContest
            {
                Id = Guid.NewGuid(),
                TeamId = (Guid)teamId,
                ContestId = (Guid)id
            };

            var contest = _db.Contests.Find(id);
            if (contest == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            team.Contests.Add(contest);
            
            _db.Entry(teamContest).State = EntityState.Added;
            _db.Entry(team).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return RedirectToAction("Edit", new { id = teamId });
        }

        public async Task<ActionResult> DeleteContests(Guid? teamId, Guid? id)
        {
            if (teamId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var team = await _db.Teams.FindAsync(teamId);
            if (team == null)
            {
                return HttpNotFound();
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var contest = _db.Contests.Find(id);
            if (contest == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            team.Contests.Remove(contest);

            var teamContest = _db.TeamContests.FirstOrDefault(tc => tc.ContestId == id);
            if (teamContest == null)
            {
                return HttpNotFound();
            }
            _db.Entry(teamContest).State = EntityState.Deleted;
            _db.Entry(team).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return RedirectToAction("Edit", new { id = teamId });
        }
        #endregion

        #region Participants
        public ActionResult TeamParticipantsList(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var team = _db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            if (!User.IsInRole("administrator"))
            {
                TempData["Message"] = "Только администратор имеет право просматривать список участников для этой команды";
                return RedirectToAction("Edit", new { id });
                //ViewBag.ErrorMessage =
                //    "Только администратор имеет право просматривать список участников для этой команды";
                //return View("Error");
            }

            var currentUserId = User.Identity.GetUserId();
            var roleStore = new RoleStore<IdentityRole>(_db);
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var participantRoleUsers = roleManager.Roles.First(r => r.Name == "participant").Users;

            var teamParticipants = _db.TeamParticipants.Where(tp => tp.TeamId == team.Id).ToList();
            var participantIds = new List<string>();
            var teamParicipantIds = new List<string>();
            foreach (var participantRoleUser in participantRoleUsers)
            {
                if (!teamParticipants.Any(tp => tp.TeamId == team.Id && tp.ParticipantId == participantRoleUser.UserId))
                    participantIds.Add(participantRoleUser.UserId);
                else
                    teamParicipantIds.Add(participantRoleUser.UserId);
            }

            var model = new TeamsParticipantsListViewModel
            {
                TeamId = id,
                Participants = _db.Users.Where(p => participantIds.Contains(p.Id) && !p.IsDeleted && p.Id != currentUserId),
                TeamParticipants = _db.Users.Where(p => teamParicipantIds.Contains(p.Id) && !p.IsDeleted && p.Id != currentUserId)
            };
            return PartialView(model);
        }

        public async Task<ActionResult> AddParticipants(Guid? teamId, string id)
        {
            if (teamId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var team = await _db.Teams.FindAsync(teamId);
            if (team == null)
            {
                return HttpNotFound();
            }

            if (!User.IsInRole("administrator") && !_db.TeamParticipants.Any(tp => tp.TeamId == teamId))
            {
                TempData["Message"] = "Вы не имеете право добавлять участников для данной команды";
                return RedirectToAction("Edit", new { id });
                //ViewBag.ErrorMessage = "Вы не имеете право добавлять участников для данной команды";
                //return View("Error");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (team.MaxTeamNumber <= team.Participants.Count)
            {
                TempData["Message"] = "Достигнуто максимальное количество участников команды.";
                return RedirectToAction("Edit", new { id = teamId });
            }

            var participant = _db.Users.Find(id);
            if (participant == null)
            {
                return HttpNotFound();
            }
            if (participant.TeamId != null)
            {
                TempData["Message"] = "Участник '" + participant.UserName + "' не был добавлен в команду '" + team.Name + "', так как состоит в другой команде - '" + participant.Team.Name + "'. Пожалуйста, сначала удалите его из этой команды.";
                return RedirectToAction("Edit", new { id = teamId });
            }
            participant.TeamId = teamId;
            team.Participants.Add(participant);
            participant.Team = team;

            var teamParticipant = new TeamParticipant
            {
                Id = Guid.NewGuid(),
                TeamId = (Guid)teamId,
                ParticipantId = id
            };

            _db.Entry(teamParticipant).State = EntityState.Added;
            _db.Entry(participant).State = EntityState.Modified;
            _db.Entry(team).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return RedirectToAction("Edit", new { id = teamId });
        }

        public async Task<ActionResult> DeleteParticipants(Guid? teamId, string id)
        {
            if (teamId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var team = await _db.Teams.FindAsync(teamId);
            if (team == null)
            {
                return HttpNotFound();
            }

            if (!User.IsInRole("administrator") && !_db.TeamParticipants.Any(tp => tp.TeamId == teamId))
            {
                TempData["Message"] = "Вы не имеете право удалять участников из данной команды";
                return RedirectToAction("Edit", new { id = teamId });
                //ViewBag.ErrorMessage = "Вы не имеете право удалять участников из данной команды";
                //return View("Error");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var participant = _db.Users.Find(id);
            if (participant == null)
            {
                return HttpNotFound();
            }
            participant.TeamId = null;
            participant.Team = null;
            team.Participants.Remove(participant);

            var teamParticipant = _db.TeamParticipants.FirstOrDefault(tp => tp.ParticipantId == id);
            if (teamParticipant == null)
            {
                return HttpNotFound();
            }

            _db.Entry(teamParticipant).State = EntityState.Deleted;
            _db.Entry(participant).State = EntityState.Modified;
            _db.Entry(team).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return RedirectToAction("Edit", new { id = teamId });
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