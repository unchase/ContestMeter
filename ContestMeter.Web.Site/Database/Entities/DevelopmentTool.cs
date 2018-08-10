﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContestMeter.Web.Site.Database.Entities
{
    public class DevelopmentTool
    {
        public DevelopmentTool()
        {
            this.PostedSolutions = new HashSet<PostedSolution>();
            this.UserAttempts = new HashSet<UserAttempt>();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid? ContestId { get; set; }
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
        [Required]
        [MaxLength(255)]
        public string CompileCommand { get; set; }
        [Required]
        [MaxLength(255)]
        public string CommandArgs { get; set; }

        public virtual Contest Contest { get; set; }
        public virtual ICollection<PostedSolution> PostedSolutions { get; set; }
        public virtual ICollection<UserAttempt> UserAttempts { get; set; }
    }
}
