using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContestMeter.Web.Site.Database.Entities;

namespace ContestMeter.Web.Site.Models
{
    public class TeamsIndexViewModel
    {
        public IEnumerable<Team> Teams { get; set; }
    }

    public class TeamsCreateViewModel
    {
        [Required(ErrorMessage = "Необходимо выбрать название команды")]
        [MaxLength(150, ErrorMessage = "{0} не должно превышать {1} символов")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать ограничение по количеству участников")]
        [Display(Name = "Максимальное количество участников")]
        [Range(1, int.MaxValue, ErrorMessage = "{0} не может быть меньше {1} и больше {2}")]
        public int MaxTeamNumber { get; set; }
    }

    public class TeamsEditViewModel
    {
        public Guid TeamId { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать название команды")]
        [MaxLength(150, ErrorMessage = "{0} не должно превышать {1} символов")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать ограничение по количеству участников")]
        [Display(Name = "Максимальное количество участников")]
        [Range(1, int.MaxValue, ErrorMessage = "{0} не может быть меньше {1} и больше {2}")]
        public int MaxTeamNumber { get; set; }
    }

}
