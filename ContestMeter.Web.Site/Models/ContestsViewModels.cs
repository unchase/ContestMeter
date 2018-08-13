using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using ContestMeter.Web.Site.Database.Entities;

namespace ContestMeter.Web.Site.Models
{
    public class ContestsIndexViewModel
    {
        public IEnumerable<Contest> Contests { get; set; }
    }

    public class ContestsCreateViewModel
    {
        public SelectList ContestsTypes { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать тип контеста")]
        [Display(Name = "Тип контеста")]
        public Guid ContestsTypeId { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать название контеста")]
        [MaxLength(150, ErrorMessage = "{0} не должно превышать {1} символов")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Активный?")]
        public bool IsActive { get; set; }
    }

    public class ContestsEditViewModel
    {
        public SelectList ContestsTypes { get; set; }

        public Guid ContestId { get; set; }

        public string TeacherId { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать тип контеста")]
        [Display(Name = "Тип контеста")]
        public Guid ContestsTypeId { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать название контеста")]
        [MaxLength(150, ErrorMessage = "{0} не должно превышать {1} символов")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Активный?")]
        public bool IsActive { get; set; }
    }

    public class ContestsExceptionsLogsViewModel
    {
        public IQueryable<ExceptionsLog> ExceptionsLogs { get; set; }

        public string ContestName { get; set; }
    }
}