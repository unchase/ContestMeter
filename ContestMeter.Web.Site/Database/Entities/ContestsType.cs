using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContestMeter.Web.Site.Database.Entities
{
    public class ContestsType
    {
        public ContestsType()
        {
            this.Contests = new HashSet<Contest>();
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        public virtual ICollection<Contest> Contests { get; set; }
    }
}
