using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
