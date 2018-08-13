using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContestMeter.Web.Site.Database.Entities
{
    public class Team
    {
        public Team()
        {
            Participants = new HashSet<ApplicationUser>();
            Contests = new HashSet<Contest>();
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
        [Required]
        public int MaxTeamNumber { get; set; }
        [Required]
        public int Rating { get; set; }

        public virtual ICollection<ApplicationUser> Participants { get; set; }
        public virtual ICollection<Contest> Contests { get; set; }
    }
}
