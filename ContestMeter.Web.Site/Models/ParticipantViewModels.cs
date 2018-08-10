using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContestMeter.Web.Site.Database.Entities;

namespace ContestMeter.Web.Site.Models
{
    public class ParticipantContestsViewModel
    {
        public List<Contest> Contests { get; set; } 
    }

    public class ParticipantInformationViewModel
    {
        public Contest Contest { get; set; }

        public IQueryable<UserAttempt> UserAttempts { get; set; }
    }

    public class ParticipantParticipateViewModel
    {
        public Guid ContestId { get; set; }

        public List<Task> Tasks { get; set; }
 
        public int CurrentTaskNumber { get; set; }
    }

    public class ParticipantTaskPartialViewModel
    {
        public Guid ContestId { get; set; }

        public Guid CurrentTaskId { get; set; }

        public int CurrentTaskNumber { get; set; }

        public Task CurrentTask { get; set; }

        public SelectList DevelopmentTools { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать компилятор")]
        [Display(Name = "Компилятор")]
        public Guid SelectedDevelopmentToolId { get; set; }

        public string TaskDescriptionFileName { get; set; }
    }
}