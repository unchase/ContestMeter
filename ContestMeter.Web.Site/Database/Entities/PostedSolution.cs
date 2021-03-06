﻿using System;
using System.ComponentModel.DataAnnotations;

namespace ContestMeter.Web.Site.Database.Entities
{
    public class PostedSolution
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public Guid TaskId { get; set; }
        [Required]
        public Guid DevelopmentToolId { get; set; }
        [Required]
        public byte[] Solution { get; set; }

        public bool IsChecked { get; set; }

        public virtual DevelopmentTool DevelopmentTool { get; set; }
        public virtual Task Task { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
