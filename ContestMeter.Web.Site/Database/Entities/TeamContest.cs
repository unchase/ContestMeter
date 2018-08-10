using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContestMeter.Web.Site.Database.Entities
{
    public class TeamContest
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid TeamId { get; set; }
        [Required]
        public Guid ContestId { get; set; }
    }
}
