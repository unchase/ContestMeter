using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContestMeter.Web.Site.Database.Entities
{
    public class Task
    {
        public Task()
        {
            this.PostedSolutions = new HashSet<PostedSolution>();
            this.UserAttempts = new HashSet<UserAttempt>();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid? ContestId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [MaxLength(50)]
        public string ExecutableName { get; set; }
        [Required]
        [MaxLength(50)]
        public string CheckerName { get; set; }
        [Required]
        [MaxLength(256)]
        public string TestsFolder { get; set; }
        [Required]
        public int TimeLimit { get; set; }
        [Required]
        public int Weight { get; set; }
        [Required]
        public int MaxSourceSize { get; set; }
        [Required]
        public int MaxMemorySize { get; set; }
        public int Rating { get; set; }

        public virtual Contest Contest { get; set; }
        public virtual ICollection<PostedSolution> PostedSolutions { get; set; }
        public virtual ICollection<UserAttempt> UserAttempts { get; set; }
    }
}
