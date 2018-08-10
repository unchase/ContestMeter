using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContestMeter.Web.Site.Database.Entities
{
    public class UserAttempt
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? ContestId { get; set; }
        [Required]
        public string UserId { get; set; }
        public Guid? TaskId { get; set; }
        public Guid? DevelopmentToolId { get; set; }
        [Required]
        public byte[] Solution { get; set; }
        public string SolutionExtension { get; set; }
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDate { get; set; }
        [Required]
        public int Score { get; set; }
        [Required]
        public int FailedChecks { get; set; }
        [Required]
        public int FailedRuns { get; set; }

        public virtual Contest Contest { get; set; }
        public virtual DevelopmentTool DevelopmentTool { get; set; }
        public virtual Task Task { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
