﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContestMeter.Web.Site.Database.Entities
{
    public class Contest
    {
        public Contest()
        {
            DevelopmentTools = new HashSet<DevelopmentTool>();
            ExceptionsLogs = new HashSet<ExceptionsLog>();
            Tasks = new HashSet<Task>();
            UserAttempts = new HashSet<UserAttempt>();
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid ContestsTypeId { get; set; }
        [Required]
        public string TeacherId { get; set; }
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
        [Required]
        public bool IsActive { get; set; }

        public virtual ContestsType ContestsType { get; set; }
        public virtual ApplicationUser Teacher { get; set; }
        public virtual ICollection<DevelopmentTool> DevelopmentTools { get; set; }
        public virtual ICollection<ExceptionsLog> ExceptionsLogs { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
        public virtual ICollection<UserAttempt> UserAttempts { get; set; }
    }
}
