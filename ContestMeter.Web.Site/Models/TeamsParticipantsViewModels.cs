using System;
using System.Linq;
using ContestMeter.Web.Site.Database.Entities;

namespace ContestMeter.Web.Site.Models
{
    public class TeamsParticipantsListViewModel
    {
        public Guid? TeamId { get; set; }
        public IQueryable<ApplicationUser> Participants { get; set; }
        public IQueryable<ApplicationUser> TeamParticipants { get; set; }
    }
}
