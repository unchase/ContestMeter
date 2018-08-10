using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
