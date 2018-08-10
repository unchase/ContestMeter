using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ContestMeter.Web.Site.Database.Entities;

namespace ContestMeter.Web.Site.Models
{
    public class ContestsTeachersListViewModel
    {
        public Guid? ContestId { get; set; }

        public string TeacherId { get; set; }

        public IQueryable<ApplicationUser> Teachers { get; set; }

        public IQueryable<ApplicationUser> ContestTeachers { get; set; }
    }
}