using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ContestMeter.Web.Site.Database.Entities
{
    public class NewsItem
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDate { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Text { get; set; }
    }
}