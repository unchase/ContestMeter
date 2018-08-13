using System;
using System.ComponentModel.DataAnnotations;

namespace ContestMeter.Web.Site.Database.Entities
{
    public class ContestTeacher
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid ContestId { get; set; }
        [Required]
        public string TeacherId { get; set; }
    }
}