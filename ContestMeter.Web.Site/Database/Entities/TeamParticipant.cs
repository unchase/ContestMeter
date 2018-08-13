using System;
using System.ComponentModel.DataAnnotations;

namespace ContestMeter.Web.Site.Database.Entities
{
    public class TeamParticipant
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid TeamId { get; set; }
        [Required]
        public string ParticipantId { get; set; }
    }
}
