using System;
using System.Linq;
using ContestMeter.Web.Site.Database.Entities;

namespace ContestMeter.Web.Site.Models
{
    public class TeamsContestsListViewModel
    {
        public Guid? TeamId { get; set; }
        public IQueryable<Contest> Contests { get; set; }
        public IQueryable<Contest> TeamContests { get; set; }
    }
}
