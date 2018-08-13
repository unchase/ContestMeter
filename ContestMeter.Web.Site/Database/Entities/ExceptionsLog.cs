using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContestMeter.Web.Site.Database.Entities
{
    public class ExceptionsLog
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? ContestId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDate { get; set; }
        [Required]
        public string Text { get; set; }

        public virtual Contest Contest { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
